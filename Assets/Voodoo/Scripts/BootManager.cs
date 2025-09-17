using UnityEngine;
using Voodoo.ConfigScriptableObjects;
using Voodoo.GameSystems.Utilities;
using Voodoo.UI;
using Voodoo.UI.Presenters;

namespace Voodoo
{
    /// <summary>
    /// This class is the first thing that runs in the game.
    /// It is responsible to create the classes in the required order
    /// and if needed to inject them with one another.
    /// /// </summary>
    public class BootManager : MonoBehaviour
    {
        [SerializeField] private UIManager _UIManager;
        [SerializeField] private GameRunner _gameRunner;
        [SerializeField] private LevelsConfig _levelsConfig;
        [SerializeField] private ScoreRulesConfig _scoreRulesConfig;
        
        public void Start()
        {
            PiecePoolFactory poolFactory = new PiecePoolFactory();
            
            MainMenuPresenter mainMenuPresenter = new MainMenuPresenter(_UIManager.MainMenuView, _UIManager);
            GameplayPresenter gameplayPresenter = new GameplayPresenter(_UIManager.GameplayView, _UIManager);
            
            gameplayPresenter.Init(_gameRunner, poolFactory, _levelsConfig, _scoreRulesConfig);
            
            _UIManager.Init(mainMenuPresenter, gameplayPresenter);
            _UIManager.ShowMainMenu();
        }
    }
}
