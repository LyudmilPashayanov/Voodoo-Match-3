using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Voodoo.Scripts.UI.Views.Gameplay.Interfaces
{
    public interface IBoardView
    {
        public void InitializeBoard(int gridWidth, int gridHeight);
        public void BlockInput(bool enable);
        public Transform GetBoardTransform();
        public Vector2 GetBoardPositionBasedOnIndex(int index);
        public float GetSpriteSize();
        public void SetArrowOverlayPosition(int index);
        public void EnableArrowOverlay(bool enable);
        public UniTask InvalidMoveAnimation();
    }
}