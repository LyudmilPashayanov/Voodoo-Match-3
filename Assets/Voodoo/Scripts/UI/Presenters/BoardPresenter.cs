using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.UI.Controllers
{
    public class BoardPresenter
    {
        private readonly IPiecePool _pool;
        private readonly BoardView _view;

        public BoardPresenter(BoardView view, IPiecePool pool)
        {
            _view = view;
            _pool = pool;
        }
        
        public void SpawnPiece(int index, PieceTypeDefinition type)
        {
            _view.PlacePiece(_pool.Get(type), index);
        }

        public void InitializeBoard(int gridWidth, int gridHeight)
        {
            _view.InitializeBoard(gridWidth, gridHeight);
        }

        public void ClearPiece(int[] indices)
        {
            _view.ClearPieces(indices);
        }
    }
}