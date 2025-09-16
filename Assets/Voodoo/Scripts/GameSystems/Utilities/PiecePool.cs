using System;
using System.Collections.Generic;
using UnityEngine;
using Voodoo.Scripts.UI.Views.Gameplay;
using Voodoo.UI.Controllers;

namespace Voodoo.Scripts.GameSystems.Utilities
{
    public sealed class PiecePool : IPiecePool, IDisposable
    {
        private Transform _inactiveRoot;
        private readonly Dictionary<PieceTypeDefinition, Stack<GamePiecePresenter>> _free = new();
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
                if (!_free.ContainsKey(type)) _free[type] = new Stack<GamePiecePresenter>(count);

                GameObject prefab = _prefabs[type];
                for (int i = 0; i < count; i++)
                {  
                    GameObject viewGo = UnityEngine.Object.Instantiate(prefab, _inactiveRoot);
                    GamePieceView view = viewGo.GetComponent<GamePieceView>() ?? viewGo.AddComponent<GamePieceView>();
                    GamePiecePresenter presenter = new GamePiecePresenter(view, type);
                    _free[type].Push(presenter);
                }
            }
        }

        public GamePiecePresenter Get(PieceTypeDefinition type)
        {
            if (!_prefabs.ContainsKey(type))
                throw new InvalidOperationException($"No prefab registered for '{type?.id}'.");

            if (!_free.TryGetValue(type, out var stack))
            {
                stack = new Stack<GamePiecePresenter>();
                _free[type] = stack;
            }

            GamePiecePresenter presenter;
            if (stack.Count > 0)
            {
                presenter = stack.Pop();
            }
            else
            {
                // pool is emtpy -> create extra from provided prefab
                GameObject viewGo = UnityEngine.Object.Instantiate(_prefabs[type]);
                GamePieceView view = viewGo.GetComponent<GamePieceView>() ?? viewGo.AddComponent<GamePieceView>();
                presenter = new GamePiecePresenter(view, type);
            }

            return presenter;
        }

        public void Release(GamePiecePresenter presenter)
        {
            if (presenter == null || presenter.TypeDef == null) return;

            if (!_free.TryGetValue(presenter.TypeDef, out var stack))
            {
                stack = new Stack<GamePiecePresenter>();
                _free[presenter.TypeDef] = stack;
            }
            
            presenter.ReleaseAndReset(_inactiveRoot);
            stack.Push(presenter);
        }

        public void Dispose()
        {
            foreach (var stack in _free.Values)
            {
                while (stack.Count > 0)
                {
                    GamePiecePresenter presenter = stack.Pop();
                    if (presenter != null)
                    {
                        presenter.Dispose();
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