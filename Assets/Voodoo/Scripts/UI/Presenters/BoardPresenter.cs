using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.UI.Controllers
{
    public class BoardPresenter
    {
        private readonly IPiecePool _pool;
        private readonly BoardView _view;

        public BoardPresenter(IPiecePool pool)
        {
            _pool = pool;
        }
        
        public void SpawnPiece(int index, PieceTypeDefinition type)
        {
            _pool.Get(type);
        }

        public void InitializeBoard(int gridWidth, int gridHeight)
        {
            
        }
    }
}