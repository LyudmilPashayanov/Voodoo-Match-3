using System;
using Voodoo.ConfigScriptableObjects;
using Voodoo.Gameplay;
using Voodoo.GameSystems.Utilities;
using Voodoo.UI.Panels;

namespace Voodoo.UI.Presenters
{
    public class GameplayPresenter : IDisposable 
    {
        private readonly IUINavigator _uiNavigator;
        private GameplayView _view;
        private IGameFlow _gameFlow;
        private BoardPresenter _boardPresenter;
        private HUDPresenter _HUDPresenter;
        private GameRunner _gameRunner;
        private PiecePoolFactory _piecePoolFactory;
        private LevelsConfig _levelsConfig;
        private ScoreRulesConfig _scoreRulesConfig;
        private FloatingScoreFactory _floatingScoreFactory;

        public GameplayPresenter(GameplayView view, IUINavigator uiNavigator)
        {
            _view = view;
            _uiNavigator =  uiNavigator;
        }

        public void Init(GameRunner gameRunner, PiecePoolFactory  piecePoolFactory, LevelsConfig levelsConfig, ScoreRulesConfig scoreRulesConfig)
        {
            _gameRunner = gameRunner;
            _piecePoolFactory = piecePoolFactory;
            _levelsConfig = levelsConfig;
            _scoreRulesConfig = scoreRulesConfig;
        }
        
        public void LoadGame(int levelToLoad)
        { 
            _view.ShowLoading();
            
            LevelEntry entry = _levelsConfig.Levels.Find(l => l.LevelId == levelToLoad);
            if (entry == null)
            {
                throw new Exception("No level with id " + levelToLoad);
            }
            
            _gameFlow = new GameFlow(entry.PieceSetRef, _piecePoolFactory, _scoreRulesConfig);
            
            _gameFlow.GameLoaded += GameLoaded;
            _gameFlow.GameOver += GameOver;
            
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
            
            _gameRunner.OnTick += _gameFlow.Tick;

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
        
        private void QuitGameplay()
        {
            _gameRunner.ResumeEngineTime();
            _uiNavigator.ShowMainMenu();
            _gameFlow.EndGame();
            Dispose();
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

            _floatingScoreFactory ??= new FloatingScoreFactory(_view.FloatingScoreViewView);

            _HUDPresenter.InitializeHUD(_floatingScoreFactory);
            _gameFlow.TimeChanged += _HUDPresenter.UpdateTime;
            _gameFlow.ScoreChanged += _HUDPresenter.UpdateScore;
            _gameFlow.GameOver += _HUDPresenter.ShowEndScreen;

            _boardPresenter.ScorePopupRequested += _HUDPresenter.ShowFloatingScoreText;
            
            _HUDPresenter.GamePaused += PauseGame;
            _HUDPresenter.GameResumed += ResumeGame;
            _HUDPresenter.QuitToMenu += QuitGameplay;
            
        }

        public void Dispose()
        {
            _boardPresenter.Dispose();
            _HUDPresenter.Dispose();
            
            _gameFlow.GameLoaded -= GameLoaded;
            _gameFlow.GameOver -= GameOver;
            
            _gameFlow.TimeChanged -= _HUDPresenter.UpdateTime;
            _gameFlow.ScoreChanged -= _HUDPresenter.UpdateScore;
            _gameFlow.GameOver -= _HUDPresenter.ShowEndScreen;

            _gameRunner.OnTick -= _gameFlow.Tick;
            
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