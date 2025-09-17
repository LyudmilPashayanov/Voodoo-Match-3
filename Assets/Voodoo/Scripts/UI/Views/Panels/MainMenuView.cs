using System;
using UnityEngine;
using UnityEngine.UI;

namespace Voodoo.UI.Panels
{
    public class MainMenuView : UIPanelView
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _exitGameButton;

        public void Subscribe(Action onStartGameClicked, Action onExitGameClicked)
        {
            _startGameButton.onClick.AddListener(onStartGameClicked.Invoke);
            _exitGameButton.onClick.AddListener(onExitGameClicked.Invoke);
        }
        
        protected override void OnViewLeft()
        { }
    }
}