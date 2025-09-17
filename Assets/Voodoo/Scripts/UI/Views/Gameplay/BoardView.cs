using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Voodoo.UI.Views.Interfaces;

namespace Voodoo.UI.Views.Gameplay
{
    public class BoardView : MonoBehaviour, IBoardView
    {
        private const float INVALID_MOVE_SHAKE_DURATION = 0.3f;
        private const float INVALID_MOVE_SHAKE_FORCE = 10f;
        
        [SerializeField] private RectTransform _boardTransform;
        [SerializeField] private RawImage _gridImage;
        [SerializeField] private RectTransform _inputBlocker;
        [SerializeField] private RectTransform _arrowsMoveOverlay;
        [SerializeField] private List<RectTransform> _arrowsTransform;

        private float _spriteSize;
        private int _boardWidth;
        private Vector2 _origin;

        public void InitializeBoard(int gridWidth, int gridHeight)
        {
            _boardWidth = gridWidth;
            _gridImage.uvRect = new Rect(0, 0, gridWidth, gridHeight);
            _spriteSize = _boardTransform.rect.width / _boardWidth;

            SetArrowsOverlaySize();
            GetBoardOriginVector();
            
            _boardTransform.gameObject.SetActive(true);
        }

        public void BlockInput(bool enable)
        {
            _inputBlocker.gameObject.SetActive(enable);
        }
    
        public Transform GetBoardTransform()
        {
            return _boardTransform;
        }
    
        public Vector2 GetBoardPositionBasedOnIndex(int index)
        {
            int x = index % _boardWidth;
            int y = index / _boardWidth;

            float cell = _spriteSize;
            float posX = x * cell;
            float posY = y * cell;

            return _origin + new Vector2(posX, posY);
        }
    
        public float GetSpriteSize()
        {
            return _spriteSize;
        }

        public void SetArrowOverlayPosition(int index)
        {
            _arrowsMoveOverlay.transform.localPosition = GetBoardPositionBasedOnIndex(index);
        }

        public void EnableArrowOverlay(bool enable)
        {
            _arrowsMoveOverlay.gameObject.SetActive(enable);
        }

        public UniTask InvalidMoveAnimation()
        {
            return _boardTransform
                .DOShakePosition(INVALID_MOVE_SHAKE_DURATION, new Vector3(INVALID_MOVE_SHAKE_FORCE, 0f, 0f), 15, 90)
                .SetEase(Ease.OutQuad)
                .ToUniTask();
        }

        public void CleanAndReset()
        {
            BlockInput(false);
            EnableArrowOverlay(false);
        }
        
        private void GetBoardOriginVector()
        {
            float offset = (_boardWidth - 1) * _spriteSize * 0.5f;
            _origin = new Vector2(-offset, -offset);
        }
        
        private void SetArrowsOverlaySize()
        {
            _arrowsMoveOverlay.sizeDelta = new Vector2(_spriteSize,_spriteSize);
            foreach (RectTransform arrow in _arrowsTransform)
            {
                arrow.sizeDelta = new Vector2(_spriteSize / 2, _spriteSize / 2);
            }
        }
    }
}
