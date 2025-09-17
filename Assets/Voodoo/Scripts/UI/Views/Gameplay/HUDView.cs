using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Voodoo.Scripts.UI.Views.Gameplay
{
    public class HUDView: MonoBehaviour, IHUDView
    {
        [SerializeField] private RectTransform _pauseMenu;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _backToMenuButton;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _pauseMenuScoreText;

        public event Action PauseButtonPressed;
        public event Action ResumeButtonPressed;
        public event Action QuitToMenuButtonPressed;

        private void Start()
        {
            _pauseButton?.onClick.AddListener(PausePress);
            _resumeButton?.onClick.AddListener(ResumeGamePress);
            _backToMenuButton?.onClick.AddListener(BackToMenuPress);
        }

        public void ShowPauseMenu(bool enableResume)
        {
            _pauseMenu.gameObject.SetActive(true);
            _resumeButton.gameObject.SetActive(enableResume);
        } 
        
        public void HidePauseMenu()
        {
            _pauseMenu.gameObject.SetActive(false);
        }
        
        public void UpdateTime(int time)
        {
            _timerText.text = time.ToString();
        }

        public void UpdateScore(int score)
        {
            _scoreText.text = score.ToString();
            _pauseMenuScoreText.text = score.ToString();
        }

        private void PausePress()
        {
            PauseButtonPressed?.Invoke();
        }
        
        private void ResumeGamePress()
        {
            ResumeButtonPressed?.Invoke();
        } 
        
        private void BackToMenuPress()
        {
            QuitToMenuButtonPressed?.Invoke();
        }

        public void CleanAndReset()
        {
            HidePauseMenu();
            UpdateTime(0);
            UpdateScore(0);
        }
    }
}