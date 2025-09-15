using System.Collections.Generic;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.UI.Controllers
{
    public class BoardPresenter
    {
        private readonly BoardView _view;

        private readonly IPiecePool _pool;
        private readonly Dictionary<int, GamePiecePresenter> _activePieces = new Dictionary<int, GamePiecePresenter>();

        private int _currentClickedIndex;
        
        public BoardPresenter(BoardView view, IPiecePool pool)
        {
            _view = view;
            _pool = pool;
        }
        
        public void SpawnPiece(int index, PieceTypeDefinition type)
        {
            GamePiecePresenter gamePiecePresenter = _pool.Get(type);
            _activePieces.Add(index, gamePiecePresenter);
            gamePiecePresenter.SetPieceSize(_view.GetSpriteSize());
            gamePiecePresenter.PlacePiece(_view.GetBoardTransform() ,_view.GetBoardPositionBasedOnIndex(index), index);
            gamePiecePresenter.SetName(index.ToString());
            gamePiecePresenter.Clicked += PieceClicked;
        }

        private void PieceClicked(int indexClicked)
        {
            if (indexClicked == _currentClickedIndex)
            {
                _currentClickedIndex = -1;
                _view.EnableArrowOverlay(false);
            }
            else
            {
                _currentClickedIndex = indexClicked;
                _view.SetArrowOverlayPosition(indexClicked);
                _view.EnableArrowOverlay(true);
            }
        }
        
        public void InitializeBoard(int gridWidth, int gridHeight)
        {
            _view.InitializeBoard(gridWidth, gridHeight);
        }
        
        public void ClearPieces(int[] indices)
        {
            foreach (var idx in indices)
            {
                if (!_activePieces.TryGetValue(idx, out var piece)) continue;
                piece.Destroy(/*OnDestroyAnimation => PiecePool.Instance.Release(piece)*/);
                _activePieces.Remove(idx);
            }
        }
    }
}