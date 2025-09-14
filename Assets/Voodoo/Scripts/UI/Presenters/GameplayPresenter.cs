using Voodoo.Gameplay;
using Voodoo.UI.Panels;

namespace Voodoo.UI.Controllers
{
    public class GameplayPresenter
    {
        private GameplayView _view;
        private IGameFlow _gameFlow;
        private BoardPresenter _boardPresenter;

        public GameplayPresenter(GameplayView view, IGameFlow gameFlow)
        {
            _view = view;
            _gameFlow = gameFlow;
                
            _view.OnShowComplete += LoadGame;
            gameFlow.GameLoaded += GameLoaded;
        }
        
        private void LoadGame()
        { 
            _view.ShowLoading();
            _gameFlow.StartGameAsync();
        }

        private void GameLoaded(int gridWidth, int gridHeight)
        {
            _boardPresenter = new BoardPresenter(_view.BoardView, _gameFlow.Pool);
            
            _boardPresenter.InitializeBoard(gridWidth, gridHeight);
            
            _gameFlow.PieceSpawned += _boardPresenter.SpawnPiece;
            _gameFlow.PiecesCleared += _boardPresenter.ClearPiece;
            
            _view.HideLoading();
        }
    }
}