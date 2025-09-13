using System;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.Gameplay
{
    public class GameManager
    {
        private readonly Grid _grid;
        private PiecePool _pool;

        public event Action<int, PieceTypeDefinition> OnPieceSpawn;
        public event Action<int[]> OnPiecesClear;
        public event Action<int, int> OnPieceMoved;
        public event Action<int> OnScoreUpdated;
        public event Action OnGameOver;

        public GameManager(PiecePool pool, int gridWidth, int gridHeight, PieceTypeDefinition[] pieceTypes)
        {
            _pool = pool;
            Grid grid = new Grid(gridWidth, gridHeight, pieceTypes);
        }

        public void StartGame()
        {
            
        }
        
        public void RequestSwap(int idxA, int idxB)
        {
            
        }

        public void EndGame()
        {
            
        }

        public void Resume()
        {
            // Resuming the timer and game flow.
        }

        public void Pause()
        {
            // Pausing the timer and game flow.
        }
    }
    
}