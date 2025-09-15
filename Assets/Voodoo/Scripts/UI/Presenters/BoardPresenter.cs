using System;
using System.Collections.Generic;
using UnityEngine;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.UI.Controllers
{
    public class BoardPresenter
    {
        private readonly BoardView _view;
        
        private readonly IPiecePool _pool;
        private readonly Dictionary<int, GamePiecePresenter> _activePieces = new Dictionary<int, GamePiecePresenter>();

        private int _currentClickedIndex = -1;
        public event Action<int, int> SwapPieces;
        public BoardPresenter(BoardView view, IPiecePool pool)
        {
            _view = view;
            _pool = pool;
        }
        
        public void SpawnPiece(int index, PieceTypeDefinition type)
        {
            GamePiecePresenter gamePiecePresenter = _pool.Get(type);
            if (_activePieces.TryAdd(index, gamePiecePresenter) == false)
            {
                _activePieces[index] = gamePiecePresenter;
            }
            gamePiecePresenter.SetPieceSize(_view.GetSpriteSize());
            gamePiecePresenter.PlacePiece(_view.GetBoardTransform() ,_view.GetBoardPositionBasedOnIndex(index), index);
            gamePiecePresenter.SetName(index.ToString());
            gamePiecePresenter.Clicked += PieceClicked;
            gamePiecePresenter.Swiped += PieceSwiped;
        }

        private void PieceClicked(int indexClicked)
        {
            if (_currentClickedIndex == indexClicked) // same piece clicked
            {
                _currentClickedIndex = -1;
                _view.EnableArrowOverlay(false);
            }
            else if(_currentClickedIndex == -1) // first click
            {
                _currentClickedIndex = indexClicked;
                _view.SetArrowOverlayPosition(indexClicked);
                _view.EnableArrowOverlay(true);
            }
            else // attempt move
            {
                SwapPieces?.Invoke(_currentClickedIndex, indexClicked);
                _currentClickedIndex = -1;
                _view.EnableArrowOverlay(false);
            }
        }
        
        private void PieceSwiped(int indexSwiped, SwipeDirection direction)
        {
          //  if (indexClicked == _currentClickedIndex)
          //  {
          //      _currentClickedIndex = -1;
          //      _view.EnableArrowOverlay(false);
          //  }
          //  else
          //  {
          //      _currentClickedIndex = indexClicked;
          //      _view.SetArrowOverlayPosition(indexClicked);
          //      _view.EnableArrowOverlay(true);
          //  }
        }
        
        public void InitializeBoard(int gridWidth, int gridHeight)
        {
            _view.InitializeBoard(gridWidth, gridHeight);
        }
        
        public void ClearPieces(int[] indices)
        {
            foreach (int idx in indices)
            {
                if (!_activePieces.TryGetValue(idx, out var piece)) continue;
                ClearPiece(piece);
                _activePieces.Remove(idx);
            }
        }

        private void ClearPiece(GamePiecePresenter piecePresenter)
        {
            piecePresenter.Destroy(/*OnDestroyAnimation => PiecePool.Instance.Release(piece)*/);
            _pool.Release(piecePresenter);
            piecePresenter.Clicked -= PieceClicked;
            piecePresenter.Swiped -= PieceSwiped;
        }

        public void OnSwapCommitted(int fromIndex, int toIndex)
        {
            if (_activePieces.TryGetValue(fromIndex, out var pieceA) &&
                _activePieces.TryGetValue(toIndex, out var pieceB))
            {
                // Swap references
                _activePieces[fromIndex] = pieceB;
                _activePieces[toIndex] = pieceA;

                // Move visually
                var fromPos = _view.GetBoardPositionBasedOnIndex(fromIndex);
                var toPos   = _view.GetBoardPositionBasedOnIndex(toIndex);

                pieceA.PlacePiece(_view.GetBoardTransform(), toPos, toIndex);
                pieceA.SetName(toIndex.ToString());

                pieceB.PlacePiece(_view.GetBoardTransform(), fromPos, fromIndex);
                pieceB.SetName(fromIndex.ToString());
            }
        }

        public void OnGravityMoves(IReadOnlyList<(int from, int to)> moves)
        {
            foreach (var (from, to) in moves)
            {
                if (_activePieces.TryGetValue(from, out var piece))
                {
                    _activePieces.Remove(from);
                    _activePieces[to] = piece;

                    piece.PlacePiece(
                        _view.GetBoardTransform(),
                        _view.GetBoardPositionBasedOnIndex(to),
                        to
                    );
                    piece.SetName(to.ToString());
                }
            }
        }
    }
}