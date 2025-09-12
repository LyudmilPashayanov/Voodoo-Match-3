namespace Voodoo.Gameplay
{
    public class GameManager
    {
        private readonly Grid _grid;

        public GameManager(Grid grid)
        {
            _grid = grid;    
        }
        
        public void RequestSwap(int idxA, int idxB)
        {
        //  _grid.SwapIndices(idxA, idxB);
        //  OnSwap?.Invoke(idxA, idxB);

        //  var matches = matchDetection.FindMatchesAfterSwap(grid, idxA, idxB);

        //  if (matches.Count == 0)
        //  {
        //      // rollback
        //      grid.SwapIndices(idxA, idxB);
        //      OnSwap?.Invoke(idxB, idxA);
        //      return;
        //  }

        //  // resolve matches
        //  ResolveMatches(matches);
        }
    }
    
}