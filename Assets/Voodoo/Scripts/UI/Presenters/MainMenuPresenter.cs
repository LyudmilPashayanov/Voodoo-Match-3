using Voodoo.UI.Panels;

namespace Voodoo.UI.Controllers
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
        
        private void OnPlayClicked()
        {
            _uiNavigator.StartGameplay();
        }

        private void OnQuitClicked()
        {
            UnityEngine.Application.Quit();
        }
    }
}