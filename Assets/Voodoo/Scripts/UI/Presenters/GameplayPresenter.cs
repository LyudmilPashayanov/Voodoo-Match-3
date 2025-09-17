using System;
using DG.Tweening;
using Voodoo.Gameplay;
using Voodoo.Scripts.GameSystems.Utilities;
using Voodoo.UI.Panels;

namespace Voodoo.UI.Controllers
{
    public class GameplayPresenter : IDisposable 
    {
        private readonly IUINavigator _uiNavigator;
        private GameplayView _view;
        private IGameFlow _gameFlow;
        private BoardPresenter _boardPresenter;
        private HUDPresenter _HUDPresenter;
        private GameRunner _gameRunner;

        public GameplayPresenter(GameplayView view, IUINavigator uiNavigator)
        {
            _view = view;
            _uiNavigator =  uiNavigator;
        }

        public void Init(IGameFlow gameFlow, GameRunner gameRunner)
        {
            _gameFlow = gameFlow;
            _gameRunner = gameRunner;

            gameFlow.GameLoaded += GameLoaded;
            gameFlow.GameOver += GameOver;

            _gameRunner.OnTick += gameFlow.Tick;
        }
        
        public void LoadGame()
        { 
            _view.ShowLoading();
            _gameFlow.StartGameAsync();
        }

        private void GameOver()
        {
            _HUDPresenter.ShowEndScreen();
        }
        
        private void GameLoaded(int gridWidth, int gridHeight)
        {
            InitializeBoard(gridWidth, gridHeight);
            InitializeHUD();
            
            _view.HideLoading();
        }

        private void ResumeGame()
        {
            _gameFlow.Resume();
            _gameRunner.ResumeEngineTime();
        }

        private void PauseGame()
        {
            _gameFlow.Pause();
            _gameRunner.PauseEngineTime();
        }
        
        private void QuitToMenu()
        {
            _gameRunner.ResumeEngineTime();
            _uiNavigator.ShowMainMenu();
        }
        
        private void InitializeBoard(int gridWidth, int gridHeight)
        {
            _boardPresenter = new BoardPresenter(_view.BoardView, _gameFlow.Pool);

            _boardPresenter.InitializeBoard(gridWidth, gridHeight);

            _gameFlow.PieceSpawnAsync = _boardPresenter.SpawnPieceAsync;
            _gameFlow.PiecesClearAsync = _boardPresenter.ClearPiecesAsync;
            _gameFlow.PieceSwapAsync = _boardPresenter.OnSwapCommittedAsync;
            _gameFlow.NoMatchSwapAsync = _boardPresenter.OnNoMatchSwapAsync;
            _gameFlow.InvalidMoveAsync = _boardPresenter.OnInvalidMoveAsync;
            _gameFlow.OnGravityMovesAsync = _boardPresenter.OnGravityMovesAsync;
           
            _boardPresenter.ClickPiece += _gameFlow.PieceClicked;
            _boardPresenter.SwapPiece += _gameFlow.SwapPiece;      
        }

        private void InitializeHUD()
        {
            _HUDPresenter = new HUDPresenter(_view.HUDView);
            
            _gameFlow.TimeChanged += _HUDPresenter.UpdateTime;
            _gameFlow.ScoreChanged += _HUDPresenter.UpdateScore;
            _gameFlow.GameOver += _HUDPresenter.ShowEndScreen;

            _HUDPresenter.GamePaused += PauseGame;
            _HUDPresenter.GameResumed += ResumeGame;
            _HUDPresenter.QuitToMenu += QuitToMenu;
            
        }

        public void Dispose()
        {
            _gameFlow.GameLoaded -= GameLoaded;
            _gameFlow.GameOver -= GameOver;
            
            _gameFlow.TimeChanged -= _HUDPresenter.UpdateTime;
            _gameFlow.ScoreChanged -= _HUDPresenter.UpdateScore;
            _gameFlow.GameOver -= _HUDPresenter.ShowEndScreen;

            _gameRunner.OnTick += _gameFlow.Tick;

            // maybe these are not needed as they will be replaced in case the gameplayPresenter changes.
            _gameFlow.PieceSpawnAsync = null;
            _gameFlow.PiecesClearAsync = null;
            _gameFlow.PieceSwapAsync = null;
            _gameFlow.NoMatchSwapAsync = null;
            _gameFlow.InvalidMoveAsync = null;
            _gameFlow.OnGravityMovesAsync = null;
        }
    }
}