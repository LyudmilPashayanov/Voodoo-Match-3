using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Voodoo.Scripts.GameSystems.Utilities;
using Voodoo.Scripts.UI.Views.Gameplay.Interfaces;

namespace Voodoo.UI.Controllers
{
    public class GamePiecePresenter : IDisposable
    {
        private IGamePieceView View { get; }
        private bool _pieceClicked = false;
        private int _index; 

        public PieceTypeDefinition TypeDef { get; private set; }
        public event Action<int> Clicked;
        public event Action<int, SwipeDirection> Swiped;
        public GamePiecePresenter(IGamePieceView view, PieceTypeDefinition type)
        {
            View = view;
            TypeDef = type;
            View.Bind(type);
            View.OnClicked += PieceClicked;
            View.OnSwiped += PieceSwiped;
        }

        public void SetIndex(int index)
        {
            _index = index;
        }
        
        public void SetPosition(Vector2 localLocation)
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
        
        public async UniTask AnimatePiece(Vector2 toLocation)
        {
           await View.AnimatePiece(toLocation);
        }

        public void SetPieceSize(float pieceSize)
        {
            View.SetSize(pieceSize);
        }

        private void PieceClicked()
        {
            if (TypeDef.pieceType.Role == PieceRole.Bomb)
            {
                // Custom bomb behavior here (clear 3x3, trigger cascades, etc.)
            }
            else
            {
                _pieceClicked = !_pieceClicked;
                View.EnableClickedState(_pieceClicked);
                Clicked?.Invoke(_index);
            }
        }
        
        private void PieceSwiped(SwipeDirection direction)
        {
            if (TypeDef.pieceType.Role == PieceRole.Bomb)
            {
                // Custom bomb behavior here (clear 3x3, trigger cascades, etc.)
            }
            else
            {
                Swiped?.Invoke(_index, direction);
            }
        }

        public async UniTask DestroyAnimationAsync()
        {
            await View.DestroyAnimationAsync();
            // Play destroyed animation
        }
        
        public void ReleaseAndReset(Transform newParent)
        {
            View.SetParent(newParent);
            View.ResetState();
        }

        public void Dispose()
        {
            View.OnClicked -= PieceClicked;
           // View.OnSwiped -= PieceSwiped; // if you have swipes too
            View.DestroyObject();
            TypeDef = null;
        }

        public void SetName(string name)
        {
            View.SetName(name);
        }
    }
}