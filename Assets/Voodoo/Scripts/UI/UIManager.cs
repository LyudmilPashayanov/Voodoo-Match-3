using UnityEngine;
using Voodoo.Gameplay;
using Voodoo.UI.Controllers;
using Voodoo.UI.Panels;

namespace Voodoo.UI
{
    public class UIManager : MonoBehaviour, IUINavigator
    {
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private GameplayView _gameplayView;
        
        private UIPanelView _activePanelView;
        private UIPanelView _pendingPanelView;

        public void Init(GameManager gameManager)
        {
            MainMenuPresenter mainMenuPresenter = new MainMenuPresenter(_mainMenuView, this);
            GameplayPresenter gameplayPresenter = new GameplayPresenter(_gameplayView, gameManager);
            ShowMainMenu();
        }
        
        public void ShowMainMenu()
        {
            ShowPanel(_mainMenuView);
        }

        public void ShowGameplay()
        {
            ShowPanel(_gameplayView);
        }
        
        private void ShowPanel(UIPanelView panelToShow)
        {
            if (_activePanelView == panelToShow)
                return;
                
            if (_activePanelView)
            {
                _pendingPanelView = panelToShow;
                _activePanelView.OnHideComplete += OnCurrentPanelHidden;
                _activePanelView.HidePanel();
            }
            else
            {
                _activePanelView = panelToShow;
                _activePanelView.ShowPanel();
            }
        }
        
        private void OnCurrentPanelHidden()
        {
            if (_activePanelView)
            {
                _activePanelView.OnHideComplete -= OnCurrentPanelHidden;
            }
            
            _activePanelView = _pendingPanelView;
            _pendingPanelView = null;
            
            if (_activePanelView)
            {
                _activePanelView.ShowPanel();
            }
        }
        
        private void OnDestroy()
        {
            if (_activePanelView)
            {
                _activePanelView.OnHideComplete -= OnCurrentPanelHidden;
            }
        }
    }
}