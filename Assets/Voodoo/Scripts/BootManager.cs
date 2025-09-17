using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Voodoo.ConfigScriptableObjects;
using Voodoo.Gameplay;
using Voodoo.Scripts.GameSystems.Utilities;
using Voodoo.UI;
using Voodoo.UI.Controllers;

namespace Voodoo
{
    /// <summary>
    /// This class is the first thing that runs in the game.
    /// It is responsible to create the classes in the required order
    /// and if needed to inject them with one another, so that they can
    /// hold the references they need.
    /// /// </summary>
    public class BootManager : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<PieceSetConfig> _levelSetConfigRef;
        [SerializeField] private UIManager _UIManager;
        [SerializeField] private GameRunner _gameRunner;
        //[SerializeField] private LevelManager LevelManagerr;

        public void Start()
        {
            GameFlow gameFlow = new GameFlow(_levelSetConfigRef);
            
            MainMenuPresenter mainMenuPresenter = new MainMenuPresenter(_UIManager.MainMenuView, _UIManager);
            GameplayPresenter gameplayPresenter = new GameplayPresenter(_UIManager.GameplayView, _UIManager);
            
            gameplayPresenter.Init(gameFlow, _gameRunner);
            
            _UIManager.Init(mainMenuPresenter, gameplayPresenter);
            _UIManager.ShowMainMenu();
        }
    }

    /*public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<LevelConfig> _levelConfigs;

        public LevelConfig GetLevelConfig(int levelId)
        {
            AssetReferenceT<PieceSetConfig> set;
            foreach (LevelConfig level in _levelConfigs)
            {
                if (level.LevelId == levelId)
                {
                    set = level.PieceSet;
                    return level;
                }
            }

            return null;
        }
    }*/
}
