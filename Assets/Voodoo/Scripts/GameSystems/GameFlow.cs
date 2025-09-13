using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Voodoo.ConfigScriptableObjects;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.Gameplay
{
    public sealed class GameFlow : IGameFlow, IDisposable
    {
        private GameManager _gameManager;
        
        private readonly AssetReferenceT<PieceSetConfig> _setRef;

        private AsyncOperationHandle<PieceSetConfig> _setHandle;
        private PieceSetConfig _set;
        private PiecePool _pool;

        public event Action<int, PieceTypeDefinition> PieceSpawned;
        public event Action<int[]> PiecesCleared;
        public event Action<int, int> PieceMoved;
        public event Action<int> ScoreChanged;
        public event Action<int> TimeChanged;
        public event Action GameOver;
        public event Action<int, int> BoardInitialized;
        
       public GameFlow(AssetReferenceT<PieceSetConfig> setRef)
        {
            _setRef = setRef;
        }

        // TODO: Maybe use a AssetLoader class to load the dependencies in the game?
        public async Task StartGameAsync(CancellationToken ct = default)
        {
            if (_gameManager != null) return;

            // Load piece set
            _setHandle = _setRef.LoadAssetAsync();
            await _setHandle.Task;
            if (_setHandle.Status != AsyncOperationStatus.Succeeded)
                throw new Exception("Failed to load PieceSetConfig");
            _set = _setHandle.Result;

            int gridWidth = _set.GridWidth;
            int gridHeight = _set.GridHeight;
            
            // Build & prewarm pool
            _pool ??= new PiecePool();
            await _pool.InitializeAsync(_set, ct);

            // Create GameManager (owns Grid) and start gameplay
            _gameManager = new GameManager(_pool, gridWidth, gridHeight, _set.availableTypes);
            WireModelEvents(_gameManager);
            _gameManager.StartGame();
        }

        public async Task EndGameAsync()
        {
            // Optional: tell GameManager to teardown model/view
            _gameManager?.EndGame();
            UnwireModelEvents(_gameManager);
            _gameManager = null;

            // Dispose pool and release set (free memory between sessions)
            _pool?.Dispose();
            _pool = null;

            if (_setHandle.IsValid())
            {
                Addressables.Release(_setHandle);
                _setHandle = default;
            }

            await Task.CompletedTask;
        }
        
        private void WireModelEvents(GameManager gm)
        {
            gm.OnPieceSpawn += PieceSpawned;
            gm.OnPiecesClear += PiecesCleared;
            gm.OnPieceMoved += PieceMoved;
            gm.OnScoreUpdated += ScoreChanged;
            //gm.OnTimeChanged += t => TimeChanged?.Invoke(t);
            gm.OnGameOver += GameOver;
        }

        private void UnwireModelEvents(GameManager gm)
        {
            gm.OnPieceSpawn -= PieceSpawned;
            gm.OnPiecesClear -= PiecesCleared;
            gm.OnPieceMoved -= PieceMoved;
            gm.OnScoreUpdated -= ScoreChanged;
            //gm.OnTimeChanged -= null;
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