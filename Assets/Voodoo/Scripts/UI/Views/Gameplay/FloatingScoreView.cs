using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Voodoo.Scripts.UI.Views.Gameplay
{
    public class FloatingScoreView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;
        
        /// <summary>
        /// Plays the floating score animation and recycles after.
        /// </summary>
        public void Play(int score, Vector2 spawnLocation, Vector2 targetPosition, Transform parent, Action<FloatingScoreView> onComplete = null)
        {
            transform.SetParent(parent, false);
            transform.position = spawnLocation;

            _text.text = $"+{score}";
            _canvasGroup.alpha = 1f;

            // reset scale
            transform.localScale = Vector3.one;
            
            // Sequence: move toward target + scale down + fade out
            Sequence seq = DOTween.Sequence();

            // Move toward target
            seq.Append(transform.DOMove(targetPosition, 0.8f).SetEase(Ease.InCubic));

            // Scale down over the same duration
            seq.Join(transform.DOScale(0.3f, 0.8f).SetEase(Ease.InCubic));

            // Fade out
            //seq.Join(_canvasGroup.DOFade(0f, 0.8f));

            seq.OnComplete(() =>
            {
                onComplete?.Invoke(this);
            });
        }
    }
}