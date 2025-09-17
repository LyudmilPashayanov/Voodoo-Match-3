using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Voodoo.Gameplay.Core;

namespace Voodoo.Scripts.UI.Views.Gameplay
{
    public interface IGamePieceView
    {
        void Bind(PieceTypeDefinition type);
        void SetSize(float size);
        void SetPosition(Vector2 localPosition);
        void SetParent(Transform parent);
        void SetName(string name);
        void Enable(bool enable);
        UniTask DestroyAnimationAsync();
        event Action OnClicked;
        event Action<Direction> OnSwiped;
        void EnableClickedState(bool enable);
        UniTask AnimatePiece(Vector2 toLocation);
        void DestroyObject();
        void ResetState();
        
        Vector2 GetWorldPosition();
    }
}