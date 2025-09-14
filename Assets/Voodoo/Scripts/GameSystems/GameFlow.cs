using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Voodoo.ConfigScriptableObjects;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.Gameplay
{
    public sealed class GameFlow : IGameFlow, IDisposable
    {
        private GameManager _gameManager;
        
        private readonly AssetReferenceT<PieceSetConfig> _setReference;
       
        // store handles so we can release later
        private List<AsyncOperationHandle<GameObject>> _prefabHandles;
        private AsyncOperationHandle<PieceSetConfig> _setHandle;
        
        private PieceSetConfig _set;
        private PieceTypeDefinition[] _availableTypes;     // index == typeId used by GameManager
        private PiecePool _pool;
        public IPiecePool Pool => _pool;

        public bool IsPrepared { get; private set; }

        public event Action<int, PieceTypeDefinition> PieceSpawned;
        public event Action<int[]> PiecesCleared;
        public event Action<int, int> PieceMoved;
        public event Action<int> ScoreChanged;
        public event Action<int> TimeChanged;
        public event Action GameOver;
        public event Action<int, int> GameLoaded;
        
        public GameFlow(AssetReferenceT<PieceSetConfig> setReference)
        {
            _setReference = setReference;
        }


        public async UniTask StartGameAsync(CancellationToken ct = default)
        {
            if (_gameManager != null) return;
            if (!IsPrepared) await PrepareAsync(ct);
    
            int w = _set.GridWidth;
            int h = _set.GridHeight;
            _availableTypes = _set.availableTypes;

            await UniTask.SwitchToMainThread(ct);
            
            List<PieceType> types = new List<PieceType>();
            foreach (var type in _availableTypes)
            {
                types.Add(type.pieceType);
            }
            
            PieceCatalog catalog = new PieceCatalog(types);
            _gameManager = new GameManager(w, h, catalog);

            WireModelEvents(_gameManager);
    
            GameLoaded?.Invoke(w, h);
            _gameManager.StartGame();
            
        }
        
        private async UniTask PrepareAsync(CancellationToken ct = default)
        {
            if (IsPrepared) return;
    
            // Load the set
            _setHandle = _setReference.LoadAssetAsync();
            await _setHandle.ToUniTask(cancellationToken: ct);
            if (_setHandle.Status != AsyncOperationStatus.Succeeded)
                throw new System.Exception("Failed to load PieceSetConfig");
            _set = _setHandle.Result;
    
            // Load each prefab and build the map
            var prefabMap = new Dictionary<PieceTypeDefinition, GameObject>(_set.availableTypes.Length);
            var handles = new List<AsyncOperationHandle<GameObject>>(_set.availableTypes.Length);
    
            foreach (var def in _set.availableTypes)
            {
                var h = def.prefabReference.LoadAssetAsync<GameObject>();
                handles.Add(h);
            }
            // await all
            foreach (var h in handles) await h.ToUniTask(cancellationToken: ct);
    
            // collect into map (parallel order with availableTypes)
            for (int i = 0; i < _set.availableTypes.Length; i++)
            {
                var def = _set.availableTypes[i];
                var h = handles[i];
                if (h.Status != AsyncOperationStatus.Succeeded)
                    throw new System.Exception($"Failed to load prefab for '{def.id}'");
                prefabMap[def] = h.Result;
            }
    
            // Compute prewarm counts (≈ board slots * 1.25 spread across types)
            var prewarm = ComputePrewarmEven(_set.availableTypes, _set.GridWidth, _set.GridHeight);
    
            // Initialize the pool synchronously with the prefabs (no async here)
            _pool ??= new PiecePool();
            _pool.Initialize(prefabMap, prewarm);
    
            // Keep prefab handles to release on EndGameAsync
            _prefabHandles = handles;
    
            IsPrepared = true;
        }
      
        private static Dictionary<PieceTypeDefinition, int> ComputePrewarmEven(PieceTypeDefinition[] types, int w, int h, float buffer = 1.25f)
        {
            int total = Mathf.CeilToInt(w * h * Mathf.Max(buffer, 1f));
            var dict = new Dictionary<PieceTypeDefinition, int>(types.Length);
            if (types.Length == 0) return dict;

            int baseEach = total / types.Length;
            int rem = total % types.Length;
            for (int i = 0; i < types.Length; i++)
                dict[types[i]] = baseEach + (i < rem ? 1 : 0);

            return dict;
        }
        
        public async UniTask EndGameAsync(CancellationToken ct = default)
        {
            if (_gameManager != null)
            {
                _gameManager.EndGame();
                UnwireModelEvents(_gameManager);
                _gameManager = null;

                // Let any final animations/process catch up
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
            }

            // Dispose pool instances (does not touch Addressables)
            if (_pool != null)
            {
                _pool.Dispose();
                _pool = null;
            }

            // Release prefab handles
            if (_prefabHandles != null)
            {
                foreach (var h in _prefabHandles)
                    if (h.IsValid()) Addressables.Release(h);
                _prefabHandles = null;
            }

            // Release set
            if (_setHandle.IsValid())
            {
                Addressables.Release(_setHandle);
                _setHandle = default;
            }

            _set = null;
            IsPrepared = false;

            await UniTask.CompletedTask;
        }
        
        /// <summary>
        /// Matches the id spawned in the GameManager to a Unity PieceType so that UI can work.
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="typeId"></param>
        private void OnModelPieceSpawned(int cellIndex, int typeId)
        {
            if ((uint)typeId >= (uint)_availableTypes.Length)
                return;
            
            
            PieceSpawned?.Invoke(cellIndex, _availableTypes[typeId]);
        }
        
        private void WireModelEvents(GameManager gm)
        {
            gm.OnPieceSpawn += OnModelPieceSpawned;
            gm.OnPiecesClear += PiecesCleared;
            gm.OnPieceMoved += PieceMoved;
            gm.OnScoreUpdated += ScoreChanged;
            gm.OnTimeChanged += TimeChanged;
            gm.OnGameOver += GameOver;
        }

        private void UnwireModelEvents(GameManager gm)
        {
            gm.OnPieceSpawn -= OnModelPieceSpawned;
            gm.OnPiecesClear -= PiecesCleared;
            gm.OnPieceMoved -= PieceMoved;
            gm.OnScoreUpdated -= ScoreChanged;
            gm.OnTimeChanged -= TimeChanged;
            gm.OnGameOver -= GameOver;
        }
        
        public void RequestSwap(int fromIndex, int toIndex)
        {
            _gameManager.RequestSwap(fromIndex, toIndex);
        }

        public void Pause()
        {
            _gameManager.Pause();
        }

        public void Resume()
        {
            _gameManager.Resume();
        }
        
        public void Dispose() => _ = EndGameAsync();
    }
}