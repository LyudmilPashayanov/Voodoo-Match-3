using System.Collections.Generic;
using UnityEngine;
using Voodoo.UI.Views.Gameplay;

namespace Voodoo.GameSystems.Utilities
{
    public class FloatingScoreFactory
    {
        private readonly Stack<FloatingScoreView> _pool = new();
        private readonly FloatingScoreView _prefab;

        public FloatingScoreFactory(FloatingScoreView prefab)
        {
            _prefab = prefab;
        }

        public FloatingScoreView Get()
        {
            return _pool.Count > 0 ? _pool.Pop() : Object.Instantiate(_prefab);
        }

        public void Release(FloatingScoreView view)
        {
            view.gameObject.SetActive(false);
            _pool.Push(view);
        }
    }
}