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
        private MainMenuPresenter _mainMenuPresenter;
        private GameplayPresenter _gameplayPresenter;
        
        public MainMenuView MainMenuView => _mainMenuView;
        public GameplayView GameplayView => _gameplayView;
        
        public void Init(MainMenuPresenter  mainMenuPresenter, GameplayPresenter gameplayPresenter)
        {
            _mainMenuPresenter = mainMenuPresenter;
            _gameplayPresenter = gameplayPresenter;
        }
        
        public void ShowMainMenu()
        {
            ShowPanel(_mainMenuView);
        }

        public void StartGameplay()
        {
            ShowPanel(_gameplayView);
            _gameplayPresenter.LoadGame();
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