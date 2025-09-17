using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Voodoo.ConfigScriptableObjects;
using Voodoo.Gameplay.Core;
using Voodoo.UI.Views.Interfaces;

namespace Voodoo.UI.Presenters
{
    /// <summary>
    /// Presenter responsible for managing the visual state of the Match-3 piece on the board.  
    /// </summary>
    public class GamePiecePresenter : IDisposable
    {
        private const float PIECE_MOVE_ANIMATION_DURATION = 0.5f;
        private IGamePieceView View { get; }
        private bool _pieceClicked = false;
        private int _index;
        
        public PieceTypeDefinition TypeDef { get; private set; }
        public event Action<int> Clicked;
        public event Action<int, Direction> Swiped;
        
        public GamePiecePresenter(IGamePieceView view, PieceTypeDefinition type)
        {
            View = view;
            TypeDef = type;

            View.OnClicked += PieceClicked;
            View.OnSwiped += PieceSwiped;
        }

        public void SetIndex(int index)
        {
            _index = index;
        }
        
        private void SetPosition(Vector2 localLocation)
        {
            View.SetPosition(localLocation);
            View.Enable(true); 
        }
        
        public void SetParent(Transform parent)
        {
            View.SetParent(parent);
        }

        public async UniTask SpawnAnimation(Vector2 spawnLocation, Vector2 toLocation)
        {
            SetPosition(spawnLocation);
            await AnimatePiece(toLocation);
        }

        public Vector2 GetPosition()
        {
            return View.GetWorldPosition();
        }
        
        public async UniTask AnimatePiece(Vector2 toLocation)
        {
           await View.AnimatePiece(toLocation, PIECE_MOVE_ANIMATION_DURATION);
        }

        public void SetPieceSize(float pieceSize)
        {
            View.SetSize(pieceSize);
        }

        private void PieceClicked()
        {
            _pieceClicked = !_pieceClicked;
            View.EnableClickedState(_pieceClicked);
            Clicked?.Invoke(_index);
        }
        
        private void PieceSwiped(Direction direction)
        {
            Swiped?.Invoke(_index, direction);
        }

        public async UniTask DestroyAnimationAsync()
        {
            await View.DestroyAnimationAsync();
        }
        
        public void ReleaseAndReset(Transform newParent)
        {
            View.SetParent(newParent);
            View.ResetState();
        }
        
        public void SetName(string name)
        {
            View.SetName(name);
        }
        
        public void Dispose()
        {
            View.OnClicked -= PieceClicked;
            View.OnSwiped -= PieceSwiped;
            View.DestroyObject();
            TypeDef = null;
        }
    }
}