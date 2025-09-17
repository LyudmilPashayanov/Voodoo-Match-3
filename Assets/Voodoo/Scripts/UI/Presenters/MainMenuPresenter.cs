using Voodoo.UI.Panels;

namespace Voodoo.UI.Presenters
{
    public class MainMenuPresenter
    {
        private MainMenuView _view;
        private readonly IUINavigator _uiNavigator;

        public MainMenuPresenter(MainMenuView view, IUINavigator uiNavigator)
        {
            _view = view;
            _uiNavigator = uiNavigator;
            view.Subscribe(OnPlayClicked, OnQuitClicked);
        }
        
        private void OnPlayClicked(int levelId)
        {
            _uiNavigator.StartGameplay(levelId);
        }

        private void OnQuitClicked()
        {
            UnityEngine.Application.Quit();
        }
    }
}