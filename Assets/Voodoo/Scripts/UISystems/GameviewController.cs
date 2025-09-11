using UnityEngine;
using Voodoo.Gameplay;
using Grid = Voodoo.Gameplay.Grid;

public class GameviewController : MonoBehaviour
{
    private GameManager _gameManager;
    
    public void Init(GameManager gameManager)
    {
        _gameManager = gameManager;
        
        // Subscribe and listen to the events from the game manager.
    }
    
    // On user input notify the game manager for what has changed.
}
