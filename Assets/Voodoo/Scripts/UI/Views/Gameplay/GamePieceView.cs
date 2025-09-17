using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Voodoo.Gameplay.Core;
using Voodoo.UI.Views.Interfaces;

namespace Voodoo.UI.Views.Gameplay
{
    public class GamePieceView : MonoBehaviour, IGamePieceView,  IPointerClickHandler, IBeginDragHandler,IDragHandler, IEndDragHandler
    {
        private const float DESTROY_ANIMATION_DURATION = 0.5f;
            
        [SerializeField] private RectTransform _pieceTransform;

        private Vector2 _dragStart;

        public event Action OnClicked;
        public event Action<Direction> OnSwiped;
        
        public void EnableClickedState(bool enable)
        {
           // start animation
        }

        public UniTask AnimatePiece(Vector2 toLocation, float animationDuration)
        {
            return _pieceTransform
                .DOLocalMove(toLocation, animationDuration)
                .SetEase(Ease.InOutQuad)
                .ToUniTask();
        }

        public void Enable(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public UniTask DestroyAnimationAsync()
        {
            return _pieceTransform
                .DOScale(Vector2.zero, DESTROY_ANIMATION_DURATION)
                .SetEase(Ease.InOutQuad)
                .ToUniTask(); 
        }
        
        public void ResetState()
        {
            Enable(true);
            _pieceTransform.localScale = Vector3.one;
        }
        
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent, false);
        }
        
        public void SetPosition(Vector2 localPosition)
        {
            transform.localPosition = localPosition;
        } 
        
        public Vector2 GetWorldPosition()
        {
            return transform.position;
        }

        public void SetName(string name)
        {
            gameObject.name = name;
        }
        
        public void SetSize(float size)
        {
            _pieceTransform.sizeDelta = new Vector2(size, size);
            _pieceTransform.anchorMin = Vector2.zero;
            _pieceTransform.anchorMax = Vector2.zero;
        }
        
        public void DestroyObject()
        {
            Destroy(gameObject);
        }

        #region EventHandlers

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragStart = eventData.position;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.position - _dragStart;
            
            if (delta.magnitude < 50f) return;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                OnSwiped?.Invoke(delta.x > 0 ? Direction.Right : Direction.Left);
            }
            else
            {
                OnSwiped?.Invoke(delta.y > 0 ? Direction.Up : Direction.Down);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        { }
        #endregion

   
    }
}
