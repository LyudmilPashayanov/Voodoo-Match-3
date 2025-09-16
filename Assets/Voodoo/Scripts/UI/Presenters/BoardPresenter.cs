using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Voodoo.Scripts.GameSystems;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.UI.Controllers
{
    public class BoardPresenter // TODO : implement a IBoardView interface!!! 
    {
        private readonly BoardView _view;
        
        private readonly IPiecePool _pool;
        private readonly Dictionary<int, GamePiecePresenter> _activePieces = new Dictionary<int, GamePiecePresenter>();

        private int _currentClickedIndex = -1;
        public event Action<int> ClickPiece;
        
        public BoardPresenter(BoardView view, IPiecePool pool)
        {
            _view = view;
            _pool = pool;
        }
        
        public void InitializeBoard(int gridWidth, int gridHeight)
        {
            _view.InitializeBoard(gridWidth, gridHeight);
        }

        private void PieceClicked(int indexClicked)
        {
            UpdateArrowOverlay(indexClicked);
            
            ClickPiece?.Invoke(indexClicked);
        }

        private void UpdateArrowOverlay(int indexClicked)
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
            else 
            {
                _currentClickedIndex = indexClicked;
                _view.SetArrowOverlayPosition(indexClicked);
                _view.EnableArrowOverlay(true);
            }
        }

        private void ResetArrowsOverlay()
        {
            _currentClickedIndex = -1;
            _view.EnableArrowOverlay(false);
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

        private async UniTask ClearPiece(GamePiecePresenter piecePresenter)
        {
            ResetArrowsOverlay();
            await piecePresenter.DestroyAnimationAsync();
            _pool.Release(piecePresenter);
            piecePresenter.Clicked -= PieceClicked;
            piecePresenter.Swiped -= PieceSwiped;
        }

        #region EventHandlers

        public async UniTask SpawnPieceAsync(int index, PieceTypeDefinition type)
        {
            _view.BlockInput(true);
            GamePiecePresenter gamePiecePresenter = _pool.Get(type);
            if (_activePieces.TryAdd(index, gamePiecePresenter) == false)
            {
                _activePieces[index] = gamePiecePresenter;
            }
            gamePiecePresenter.SetPieceSize(_view.GetSpriteSize());
            gamePiecePresenter.SetParent(_view.GetBoardTransform());
            gamePiecePresenter.SetIndex(index);
            gamePiecePresenter.SetName(index.ToString());
            
            Vector2 spawnPos = _view.GetBoardPositionBasedOnIndex(index);
            spawnPos.y = spawnPos.y + 1000;
            await gamePiecePresenter.SpawnAnimation(spawnPos ,_view.GetBoardPositionBasedOnIndex(index));
            
            gamePiecePresenter.Clicked += PieceClicked;
            gamePiecePresenter.Swiped += PieceSwiped;
            _view.BlockInput(false);
        }
        
        public async UniTask ClearPiecesAsync(IReadOnlyList<MatchCluster> matchesToClear)
        {
            _view.BlockInput(true);
            ResetArrowsOverlay();
            foreach (MatchCluster cluster in matchesToClear)
            {
                var tasks = new List<UniTask>();
                foreach (int Idx in cluster.Indices)
                {
                    // Animate destorying clusters one by one...
                    if (!_activePieces.TryGetValue(Idx, out var piece)) continue;
                    tasks.Add(ClearPiece(piece));
                    _activePieces.Remove(Idx);   
                }
                await UniTask.WhenAll(tasks);
            }
            _view.BlockInput(false);
        }
        
        public async UniTask OnSwapCommittedAsync(int fromIndex, int toIndex)
        {
            _view.BlockInput(true);
            ResetArrowsOverlay();
            if (_activePieces.TryGetValue(fromIndex, out var pieceA) &&
                _activePieces.TryGetValue(toIndex, out var pieceB))
            {
                // Swap references
                _activePieces[fromIndex] = pieceB;
                _activePieces[toIndex] = pieceA;

                // Move visually
                var fromPos = _view.GetBoardPositionBasedOnIndex(fromIndex);
                var toPos   = _view.GetBoardPositionBasedOnIndex(toIndex);

                pieceA.AnimatePiece(toPos);
                pieceA.SetIndex(toIndex);
                pieceA.SetName(toIndex.ToString());

                await pieceB.AnimatePiece(fromPos);
                pieceB.SetIndex(fromIndex);
                pieceB.SetName(fromIndex.ToString());
            }
            _view.BlockInput(false);
        }
        
        public async UniTask OnNoMatchSwapAsync(int fromIndex, int toIndex)
        {
            _view.BlockInput(true);
            ResetArrowsOverlay();
            if (_activePieces.TryGetValue(fromIndex, out var pieceA) &&
                _activePieces.TryGetValue(toIndex, out var pieceB))
            {
                var fromPos = _view.GetBoardPositionBasedOnIndex(fromIndex);
                var toPos = _view.GetBoardPositionBasedOnIndex(toIndex);
                
                pieceA.AnimatePiece(toPos);
                await pieceB.AnimatePiece(fromPos);
                
                pieceA.AnimatePiece(fromPos);
                await pieceB.AnimatePiece(toPos);
            }

            _view.BlockInput(false);
        }
        
        public async UniTask OnInvalidMoveAsync(int fromIndex, int toIndex)
        {
            _view.BlockInput(true);
            ResetArrowsOverlay();
            await _view.InvalidMoveAnimation();
            _view.BlockInput(false);
        }
        
        public async UniTask OnGravityMovesAsync(IReadOnlyList<(int fromIndex, int toIndex)> moves)
        {
            _view.BlockInput(true);
            ResetArrowsOverlay();
            var tasks = new List<UniTask>();

            foreach (var (fromIndex, toIndex) in moves)
            {
                if (_activePieces.TryGetValue(fromIndex, out var piece))
                {
                    _activePieces.Remove(fromIndex);
                    _activePieces[toIndex] = piece;
                    piece.SetIndex(toIndex);
                    tasks.Add(piece.AnimatePiece(_view.GetBoardPositionBasedOnIndex(toIndex)));
                    piece.SetName(toIndex.ToString());
                }
            }
            await UniTask.WhenAll(tasks);
            _view.BlockInput(false);
        }
        
        #endregion
    }
}