using System;
using UnityEngine;
using UnityEngine.UI;

namespace Voodoo.UI.Panels
{
    public class MainMenuView : UIPanelView
    {
        [SerializeField] private Button _startLevelNormalButton;
        [SerializeField] private Button _startLevelBombsButton;
        [SerializeField] private Button _exitGameButton;

        private Action<int> _startGameLevel;
        
        public void Subscribe(Action<int> onStartLevelSelected, Action onExitGameClicked)
        {
            _startGameLevel += onStartLevelSelected;
            _startLevelNormalButton.onClick.AddListener(StartLevelNormal);
            _startLevelBombsButton.onClick.AddListener(StartLevelBombs);
            _exitGameButton.onClick.AddListener(onExitGameClicked.Invoke);
        }

        private void StartLevelNormal()
        {
            _startGameLevel?.Invoke(1);
        }
        
        private void StartLevelBombs()
        {
            _startGameLevel?.Invoke(2);
        }
        
        protected override void OnViewLeft()
        { }
    }
}