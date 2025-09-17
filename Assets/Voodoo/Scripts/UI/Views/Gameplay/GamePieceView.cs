using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Voodoo.GameSystems.Utilities;

namespace Voodoo.Scripts.UI.Views.Gameplay
{
    public class GamePieceView : MonoBehaviour, IGamePieceView,  IPointerClickHandler, IBeginDragHandler,IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _pieceTransform;

        private PieceTypeDefinition TypeDef;
        private Vector2 _dragStart;

        public event Action OnClicked;
        public event Action<SwipeDirection> OnSwiped;
        
        public void EnableClickedState(bool enable)
        {
           // start animation
        }

        public UniTask AnimatePiece(Vector2 toLocation)
        {
            return _pieceTransform
                .DOLocalMove(toLocation, 1f)
                .SetEase(Ease.InOutQuad)
                .ToUniTask();
        }
        
        public void Bind(PieceTypeDefinition typeDefinition)
        {
            TypeDef = typeDefinition;
        }

        public void Enable(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public UniTask DestroyAnimationAsync()
        {
            return _pieceTransform
                .DOScale(Vector2.zero, 1f)
                .SetEase(Ease.InOutQuad)
                .ToUniTask(); 
        }
        
        public void ResetState()
        {
            // reset animation states.
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
                OnSwiped?.Invoke(delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left);
            }
            else
            {
                OnSwiped?.Invoke(delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        { }
        #endregion

   
    }
}
