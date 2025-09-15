using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.Scripts.UI.Views.Gameplay
{
    public interface IGamePieceView
    {
        void Bind(PieceTypeDefinition type);
        void SetSize(float size);
        void SetParentAndPosition(Transform parent, Vector2 localPosition);
        void SetName(string name);
        void Enable(bool enable);
        void Destroy();
        
        event Action OnClicked;
        event Action<SwipeDirection> OnSwiped;
        void EnableClickedState(bool enable);
    }
    
    public class GamePieceView : MonoBehaviour, IGamePieceView,  IPointerClickHandler, IBeginDragHandler, IEndDragHandler
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

        public void Bind(PieceTypeDefinition typeDefinition)
        {
            TypeDef = typeDefinition;
        }

        public void Enable(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void SetParentAndPosition(Transform parent, Vector2 localPosition)
        {
            transform.SetParent(parent, false);
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

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke();
            Debug.Log("clicked!!!");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragStart = eventData.position;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.position - _dragStart;
            if (delta.magnitude < 50f) return; // threshold

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                OnSwiped?.Invoke(delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left);
            }
            else
            {
                OnSwiped?.Invoke(delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down);
            }
        }
    }
}
