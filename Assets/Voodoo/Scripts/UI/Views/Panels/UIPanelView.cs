using System;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace Voodoo.UI.Panels
{
    /// <summary>
    /// Base class for UI panels in the game.  
    /// </summary>
    public abstract class UIPanelView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        private Sequence _showAnimation;
        private Sequence _hideAnimation;
        public event Action OnHideComplete;
        public event Action OnShowComplete;
        
        protected void Awake()
        {
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        protected void OnDestroy()
        {
            _showAnimation.Kill();
            _hideAnimation.Kill();
        }

        public void HidePanel(bool playAnimation = true)
        {
            _showAnimation?.Pause();
            
            if (playAnimation)
            {
                if (_hideAnimation == null)
                {
                    CreateHideAnimation();
                }
                _hideAnimation.Restart();
            }
            else
            {
                // Immediate hide
                _canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
                OnViewLeft();
                OnHideComplete?.Invoke();
            }
        }
        
        public void ShowPanel(bool playAnimation = true)
        {
            gameObject.SetActive(true);
            _hideAnimation?.Pause();

            if (playAnimation)
            {
                if (_showAnimation == null)
                {
                    CreateShowAnimation();
                }
                _showAnimation.Restart();
            }
            else
            {
                // Immediate show
                _canvasGroup.alpha = 1f;
                OnShowComplete?.Invoke();
            }
        }
        
        private void CreateShowAnimation()
        {
            _showAnimation = DOTween.Sequence();
            _showAnimation.SetAutoKill(false);
            _showAnimation.Append(
                _canvasGroup.DOFade(1f, 0.8f)
                    .SetEase(Ease.OutQuint)
            );
            _showAnimation.OnComplete(ShowCompleted);
        }
        
        private void CreateHideAnimation()
        {
            _hideAnimation = DOTween.Sequence();
            _hideAnimation.SetAutoKill(false);
            _hideAnimation.Append(
                _canvasGroup.DOFade(0f, 0.8f)
                    .SetEase(Ease.InQuint)
            );
            _hideAnimation.OnComplete(HideCompleted);
        }

        private void ShowCompleted()
        {
            OnShowComplete?.Invoke();
        }
        
        private void HideCompleted()
        {
            gameObject.SetActive(false);
            OnViewLeft();
            OnHideComplete?.Invoke();
        }
        
        protected abstract void OnViewLeft();
    }
}