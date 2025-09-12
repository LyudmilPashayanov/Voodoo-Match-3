using Voodoo.Gameplay;
using Voodoo.UI.Panels;

namespace Voodoo.UI.Controllers
{
    public class GameplayPresenter
    {
        private GameplayView _view;
        private GameManager _gameManager;
        
        public GameplayPresenter(GameplayView view, GameManager gameManager)
        {
            _view = view;
        }
    }
}