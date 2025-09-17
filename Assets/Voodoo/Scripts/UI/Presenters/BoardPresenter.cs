using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Voodoo.Gameplay.Core;
using Voodoo.GameSystems.Utilities;
using Voodoo.Scripts.UI.Views.Gameplay;

namespace Voodoo.UI.Presenters
{
    public class BoardPresenter : IDisposable
    {
        private readonly IBoardView _view;
        
        private readonly IPiecePool _pool;
        private readonly Dictionary<int, GamePiecePresenter> _activePieces = new Dictionary<int, GamePiecePresenter>();

        private int _currentClickedIndex = -1;
        public event Action<int> ClickPiece;
        public event Action<int, Direction> SwapPiece;
        public event Action<int, Vector2> ScorePopupRequested;

        public BoardPresenter(IBoardView view, IPiecePool pool)
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

        private void ResetClickState()
        {
            _currentClickedIndex = -1;
            _view.EnableArrowOverlay(false);
        }

        private void PieceSwiped(int indexSwiped, Direction direction)
        {
            ResetClickState();
            SwapPiece?.Invoke(indexSwiped, direction);
        }
        
        private void ShowClusterScore(MatchCluster cluster, Vector2 worldPos)
        {
            ScorePopupRequested?.Invoke(cluster.ScoreValue, worldPos);
        }
        
        private async UniTask ClearPiece(GamePiecePresenter piecePresenter)
        {
            ResetClickState();
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
            ResetClickState();
            
            var tasks = new List<UniTask>();
            
            foreach (MatchCluster cluster in matchesToClear)
            {
                GamePiecePresenter piece = null;
                foreach (int Idx in cluster.Indices) // Animate destroying cluster one by one...
                {
                    if (!_activePieces.TryGetValue(Idx, out piece))
                    {
                        throw new Exception("Invalid index given inside Match Cluster!");
                    }
                    
                    tasks.Add(ClearPiece(piece));
                    _activePieces.Remove(Idx);
                }

                if (piece != null)
                {
                    ShowClusterScore(cluster, piece.GetPosition());
                }

                // await UniTask.WhenAll(tasks); If awaiting here it will make each animation one after another
            }
            
            await UniTask.WhenAll(tasks); // If awaiting here it will all animations run at the same time

            
            _view.BlockInput(false);
        }
        
        public async UniTask OnSwapCommittedAsync(int fromIndex, int toIndex)
        {
            _view.BlockInput(true);
            ResetClickState();
            if (_activePieces.TryGetValue(fromIndex, out var pieceA) &&
                _activePieces.TryGetValue(toIndex, out var pieceB))
            {
                // Swap references
                _activePieces[fromIndex] = pieceB;
                _activePieces[toIndex] = pieceA;

                // Move visually
                var fromPos = _view.GetBoardPositionBasedOnIndex(fromIndex);
                var toPos   = _view.GetBoardPositionBasedOnIndex(toIndex);

                _ = pieceA.AnimatePiece(toPos);
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
            ResetClickState();
            if (_activePieces.TryGetValue(fromIndex, out var pieceA) &&
                _activePieces.TryGetValue(toIndex, out var pieceB))
            {
                var fromPos = _view.GetBoardPositionBasedOnIndex(fromIndex);
                var toPos = _view.GetBoardPositionBasedOnIndex(toIndex);
                
                _ = pieceA.AnimatePiece(toPos);
                await pieceB.AnimatePiece(fromPos);
                
                _ = pieceA.AnimatePiece(fromPos);
                await pieceB.AnimatePiece(toPos);
            }

            _view.BlockInput(false);
        }
        
        public async UniTask OnInvalidMoveAsync()
        {
            _view.BlockInput(true);
            ResetClickState();
            await _view.InvalidMoveAnimation();
            _view.BlockInput(false);
        }
        
        public async UniTask OnGravityMovesAsync(IReadOnlyList<(int fromIndex, int toIndex)> moves)
        {
            _view.BlockInput(true);
            ResetClickState();
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

        public void Dispose()
        {
            _pool.ReleaseAll(_activePieces.Values);
            _activePieces.Clear();
            _view.CleanAndReset();
        }
    }
}