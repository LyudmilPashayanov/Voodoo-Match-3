using System;
using System.Collections.Generic;
using UnityEngine;
using Voodoo.Scripts.UI.Views.Gameplay;

namespace Voodoo.Scripts.GameSystems.Utilities
{
    public sealed class PiecePool : IPiecePool, IDisposable
    {
        private Transform _inactiveRoot;
        private readonly Dictionary<PieceTypeDefinition, Stack<GamePieceView>> _free = new();
        private readonly Dictionary<PieceTypeDefinition, GameObject> _prefabs = new();

        public void Initialize(Dictionary<PieceTypeDefinition, GameObject> availablePrefabsMap, Dictionary<PieceTypeDefinition, int> preloadCounts)
        {
            if (_inactiveRoot == null)
            {
                _inactiveRoot = new GameObject("FreePiecesPool").transform;
                _inactiveRoot.gameObject.SetActive(false);
            }

            _prefabs.Clear();
            foreach (var kv in availablePrefabsMap)
            {
                _prefabs[kv.Key] = kv.Value;
            }

            foreach (var kv in preloadCounts)
            {
                PieceTypeDefinition type = kv.Key;
                int count = Mathf.Max(0, kv.Value);
                if (!_free.ContainsKey(type)) _free[type] = new Stack<GamePieceView>(count);

                GameObject prefab = _prefabs[type];
                for (int i = 0; i < count; i++)
                {
                    GameObject go = UnityEngine.Object.Instantiate(prefab, _inactiveRoot);
                    go.SetActive(false);

                    GamePieceView view = go.GetComponent<GamePieceView>() ?? go.AddComponent<GamePieceView>();
                    view.Bind(type);
                    _free[type].Push(view);
                }
            }
        }

        public GamePieceView Get(PieceTypeDefinition type)
        {
            if (!_prefabs.ContainsKey(type))
                throw new InvalidOperationException($"No prefab registered for '{type?.id}'.");

            if (!_free.TryGetValue(type, out var stack))
            {
                stack = new Stack<GamePieceView>();
                _free[type] = stack;
            }

            GamePieceView view;
            if (stack.Count > 0)
            {
                view = stack.Pop();
            }
            else
            {
                // pool is emtpy -> create extra from provided prefab
                var go = UnityEngine.Object.Instantiate(_prefabs[type]);
                view = go.GetComponent<GamePieceView>() ?? go.AddComponent<GamePieceView>();
                view.Bind(type);
            }

            view.gameObject.SetActive(true);
            return view;
        }

        public void Release(GamePieceView view)
        {
            if (view == null || view.TypeDef == null) return;

            if (!_free.TryGetValue(view.TypeDef, out var stack))
            {
                stack = new Stack<GamePieceView>();
                _free[view.TypeDef] = stack;
            }

            Transform t = view.transform;
            t.SetParent(_inactiveRoot, false);
            view.gameObject.SetActive(false);
            stack.Push(view);
        }

        public void Dispose()
        {
            foreach (var stack in _free.Values)
            {
                while (stack.Count > 0)
                {
                    GamePieceView view = stack.Pop();
                    if (view)
                    {
                        UnityEngine.Object.Destroy(view.gameObject);
                    }
                }
            }
        
            _free.Clear();
            _prefabs.Clear();

            if (_inactiveRoot)
            {
                UnityEngine.Object.Destroy(_inactiveRoot.gameObject);
            }
            _inactiveRoot = null;
        }
    }
}