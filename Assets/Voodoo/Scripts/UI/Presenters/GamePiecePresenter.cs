using System;
using UnityEngine;
using Voodoo.Scripts.GameSystems.Utilities;
using Voodoo.Scripts.UI.Views.Gameplay;

namespace Voodoo.UI.Controllers
{
    public class GamePiecePresenter : IDisposable
    {
        private IGamePieceView View { get; }
        public PieceTypeDefinition TypeDef { get; private set; }
        
        public event Action<int> Clicked;
        public event Action<GamePiecePresenter, SwipeDirection> Swiped;
        private bool _pieceClicked = false;
        public int Index { get; private set; }
        public GamePiecePresenter(IGamePieceView view, PieceTypeDefinition type)
        {
            View = view;
            TypeDef = type;
            View.Bind(type);
            View.OnClicked += PieceClicked;
        }

        public void PlacePiece(Transform parent, Vector2 localLocation, int index)
        {
            View.SetParentAndPosition(parent, localLocation);
            Index = index;
        } 
        
        public void AnimatePiece(float fromLocation, float toLocation)
        {
            
        }

        public void SetPieceSize(float pieceSize)
        {
            View.SetSize(pieceSize);
        }

        private void PieceClicked()
        {
            Debug.Log("piece clicked");
            if (TypeDef.pieceType.Role == PieceRole.Bomb)
            {
                // Custom bomb behavior here (clear 3x3, trigger cascades, etc.)
            }
            else
            {
                _pieceClicked = !_pieceClicked;
                View.EnableClickedState(_pieceClicked);
                Clicked?.Invoke(Index);
            }
        }

        public void Destroy(/*destroy type of animation based on the matchChunk found*/)
        {
            // Play destroyed animation
        }
        
        public void Release(Transform newParent)
        {
            View.Enable(false);
            View.SetParentAndPosition(newParent, Vector3.zero);
        }

        public void Dispose()
        {
            View.OnClicked -= PieceClicked;
           // View.OnSwiped -= PieceSwiped; // if you have swipes too
            View.Destroy();
            TypeDef = null;
        }

        public void SetName(string name)
        {
            View.SetName(name);
        }
    }
}