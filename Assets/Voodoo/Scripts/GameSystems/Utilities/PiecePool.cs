using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Voodoo.ConfigScriptableObjects;
using Voodoo.Scripts.UI.Views.Gameplay;

namespace Voodoo.Scripts.GameSystems.Utilities
{
    public sealed class PiecePool : IDisposable
    {
        private const float SPAWN_BUFFER = 1.25f;

        private sealed class Pool
        {
            public PieceTypeDefinition def;
            public AsyncOperationHandle<GameObject> prefabHandle;
            public GameObject prefab;
            public readonly Stack<GamePieceView> free = new();
        }
        
        private Transform _inactivePiecesParent;
        private readonly Dictionary<PieceTypeDefinition, Pool> _pools = new();
        private bool _initialized;

        /// <summary>
        /// Loads prefabs for the given types and prewarms counts per type.
        /// </summary>
        public async Task InitializeAsync(PieceSetConfig setConfig, CancellationToken ct = default)
        {
            if (_initialized) return;

            if (_inactivePiecesParent == null)
            {
                _inactivePiecesParent = new GameObject("FreePiecesPool").transform;
            }

            PieceTypeDefinition[] types = setConfig.availableTypes;
            // Load all prefab assets in parallel
            List<Task> loadOps = new List<Task>(types.Length);
            foreach (PieceTypeDefinition def in types)
            {
                var handle = def.prefabReference.LoadAssetAsync<GameObject>();
                Pool pool = new Pool { def = def, prefabHandle = handle };
                _pools[def] = pool;
                loadOps.Add(handle.Task);
            }
            await Task.WhenAll(loadOps);

            // Load prefab refs
            foreach (var kv in _pools)
            {
                var pool = kv.Value;
                if (pool.prefabHandle.Status != AsyncOperationStatus.Succeeded)
                    throw new Exception($"Failed to load prefab for type '{pool.def.id}'.");
                pool.prefab = pool.prefabHandle.Result;
            }

            var preloadCounts = ComputePrewarmEven(types, setConfig.GridWidth, setConfig.GridHeight);

            // Pre load instances
            foreach (var kv in preloadCounts)
            {
                if (!_pools.TryGetValue(kv.Key, out var pool)) continue;
                int count = Mathf.Max(0, kv.Value);
            
                for (int i = 0; i < count; i++)
                {
                    GameObject go = UnityEngine.Object.Instantiate(pool.prefab, _inactivePiecesParent);
                    go.SetActive(false);
                    GamePieceView pieceView = go.GetComponent<GamePieceView>() ?? go.AddComponent<GamePieceView>();
                    pieceView.Bind(pool.def);
                    pool.free.Push(pieceView);
                }
            }

            _initialized = true;
        }

        public GamePieceView Get(PieceTypeDefinition def)
        {
            if (!_pools.TryGetValue(def, out var pool))
                throw new InvalidOperationException($"Pool for '{def?.id}' not initialized.");

            GamePieceView view;
        
            if (pool.free.Count > 0)
            {
                view = pool.free.Pop();
            }
            else
            {
                view = CreateExtra(pool);
            }
        
            return view;
        }

        public void Release(GamePieceView pieceView)
        {
            if (pieceView == null || pieceView.TypeDef == null) return;
            if (!_pools.TryGetValue(pieceView.TypeDef, out var pool)) return;

            Transform releasedPiece = pieceView.transform;
            
            if (_inactivePiecesParent == null)
            {
                _inactivePiecesParent = new GameObject("InactivePiecesPool").transform;
            }
            
            releasedPiece.SetParent(_inactivePiecesParent, false);
            pieceView.gameObject.SetActive(false);
            pool.free.Push(pieceView);
        }

        private static GamePieceView CreateExtra(Pool pool)
        {
            GameObject go = UnityEngine.Object.Instantiate(pool.prefab);
            GamePieceView pieceView = go.GetComponent<GamePieceView>() ?? go.AddComponent<GamePieceView>(); // Shouldn't be the case, but just to be sure.
            pieceView.Bind(pool.def);
            return pieceView;
        }

        private Dictionary<PieceTypeDefinition, int> ComputePrewarmEven(PieceTypeDefinition[] types, int w, int h)
        {
            int total = Mathf.CeilToInt(w * h * SPAWN_BUFFER);
            var dict = new Dictionary<PieceTypeDefinition,int>(types.Length);
            if (types.Length == 0) return dict;
            int baseEach = total / types.Length;
            int rem = total % types.Length;
            for (int i = 0; i < types.Length; i++)
                dict[types[i]] = baseEach + (i < rem ? 1 : 0);
            return dict;
        }
        
        public void Dispose()
        {
            // Destroy pooled instances
            foreach (var poolType in _pools)
            {
                Pool pool = poolType.Value;
                while (pool.free.Count > 0)
                {
                    GamePieceView piece = pool.free.Pop();
                    if (piece)
                    {
                        UnityEngine.Object.Destroy(piece.gameObject);
                    }
                }

                if (pool.prefabHandle.IsValid())
                    Addressables.Release(pool.prefabHandle);
            }
            _pools.Clear();
        }
    }
}