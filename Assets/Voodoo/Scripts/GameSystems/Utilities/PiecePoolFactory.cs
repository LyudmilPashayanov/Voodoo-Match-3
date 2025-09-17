using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Voodoo.ConfigScriptableObjects;

namespace Voodoo.GameSystems.Utilities
{
    public class PiecePoolFactory
    {
        private LevelConfig _cachedSet;
        private PiecePool _cachedPool;
    
        public async UniTask<PiecePool> GetOrCreateAsync(LevelConfig set, CancellationToken ct = default)
        {
            if (_cachedSet == set && _cachedPool != null)
            {
                return _cachedPool;
            }
    
            // Dispose old one if different set is requested
            ReleaseCurrent();
    
            // Create new pool
            _cachedSet = set;
            _cachedPool = await CreateAsync(set, ct);
            return _cachedPool;
        }
    
        /// <summary>
        /// Loads the asset needed for the pool
        /// </summary>
        private async UniTask<PiecePool> CreateAsync(LevelConfig set, CancellationToken ct = default)
        {
            var prefabMap = new Dictionary<PieceTypeDefinition, GameObject>(set.availableTypes.Length);
            var addressableHandles = new List<AsyncOperationHandle<GameObject>>(set.availableTypes.Length);

            foreach (PieceTypeDefinition definitionToLoad in set.availableTypes)
            {
                addressableHandles.Add(definitionToLoad.prefabReference.LoadAssetAsync<GameObject>());
            }
    
            List<UniTask> tasks = new List<UniTask>(addressableHandles.Count);
            foreach (var h in addressableHandles)
            {
                tasks.Add(h.ToUniTask(cancellationToken: ct));
            }

            await UniTask.WhenAll(tasks);
            
            for (int i = 0; i < set.availableTypes.Length; i++)
            {
                PieceTypeDefinition def = set.availableTypes[i];
                prefabMap[def] = addressableHandles[i].Result;
            }
    
            Dictionary<PieceTypeDefinition, int> preloadCounts = ComputePreloadedCounts(set.availableTypes, set.GridWidth, set.GridHeight);
    
            var pool = new PiecePool();
            pool.Initialize(prefabMap, preloadCounts);
    
            return pool;
        }
    
        private void ReleaseCurrent()
        {
            if (_cachedPool != null)
            {
                _cachedPool.Dispose();
                _cachedPool = null;
                _cachedSet = null;
            }
        }
    
        private Dictionary<PieceTypeDefinition, int> ComputePreloadedCounts(PieceTypeDefinition[] allowedTypes, int gridWidth, int gridHeight)
        {
            var dict = new Dictionary<PieceTypeDefinition, int>(allowedTypes.Length);
            if (allowedTypes.Length == 0) return dict;
    
            int total = gridWidth * gridHeight;
            int baseCount = total / allowedTypes.Length;
            int remainder = total % allowedTypes.Length;

            for (int i = 0; i < allowedTypes.Length; i++)
            {
                dict[allowedTypes[i]] = baseCount + (i < remainder ? 1 : 0);
            }
    
            return dict;
        }
    }
}