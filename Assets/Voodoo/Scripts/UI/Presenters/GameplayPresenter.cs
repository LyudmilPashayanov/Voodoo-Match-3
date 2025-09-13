using Voodoo.Gameplay;
using Voodoo.UI.Panels;

namespace Voodoo.UI.Controllers
{
    public class GameplayPresenter
    {
        private GameplayView _view;
        private IGameFlow _gameFlow;
        
        public GameplayPresenter(GameplayView view, IGameFlow gameFlow)
        {
            _view = view;
            _view.OnShowComplete += LoadGame;
            _gameFlow = gameFlow;
        }
        
        private void LoadGame()
        {
            _gameFlow.StartGameAsync();
        }
    }
}