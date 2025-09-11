using System;
using Unity.VisualScripting;
using UnityEngine;
using Voodoo.Gameplay;
using Grid = Voodoo.Gameplay.Grid;

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
        [SerializeField] private GameviewController _gameviewController;
        private GameManager _gameManager;
        
        private void Start()
        {
            Initialize();
            StartGame();
        }

        private void Initialize()
        {
            Grid gameGrid = new Grid(5,5);
            _gameManager = new GameManager(gameGrid);
        }

        private void StartGame()
        {
            //_gameManager.StartGame();
        }
    }
}
