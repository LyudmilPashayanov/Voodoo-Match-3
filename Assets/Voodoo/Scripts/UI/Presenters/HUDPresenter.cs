using System;
using UnityEngine;
using Voodoo.GameSystems.Utilities;
using Voodoo.UI.Views.Interfaces;

namespace Voodoo.UI.Presenters
{
    public class HUDPresenter : IDisposable
    {
        IHUDView _view;
        
        private FloatingScoreFactory _floatingScoreFactory;

        public event Action GamePaused;
        public event Action GameResumed;
        public event Action QuitToMenu;
        
        public HUDPresenter(IHUDView view)
        {
            _view = view;
            _view.PauseButtonPressed += PauseGame;
            _view.ResumeButtonPressed += ResumeGame;
            _view.QuitToMenuButtonPressed += QuitGameToMenu;
        }
        
        public void InitializeHUD(FloatingScoreFactory floatingScoreFactory)
        {
            _floatingScoreFactory = floatingScoreFactory;
        }
        
        public void ShowEndScreen()
        {
            _view.ShowPauseMenu(false);
        }
        
        public void UpdateTime(int time)
        {
            _view.UpdateTime(time);
        }

        public void UpdateScore(int score)
        {
            _view.UpdateScore(score);
        }

        public void ShowFloatingScoreText(int score, Vector2 spawnPosition)
        {
            var floating = _floatingScoreFactory.Get();
            floating.gameObject.SetActive(true);

            floating.Play(score, spawnPosition, _view.GetScorePosition(), _view.GetFloatingScoreParent() , view => _floatingScoreFactory.Release(view));

        }
        
        private void PauseGame()
        {
            _view.ShowPauseMenu(true);
            GamePaused?.Invoke();
        }  
        
        private void ResumeGame()
        {
            _view.HidePauseMenu();
            GameResumed?.Invoke();
        } 
        
        private void QuitGameToMenu()
        {
            QuitToMenu?.Invoke();
        }

        public void Dispose()
        {
            _view.PauseButtonPressed -= PauseGame;
            _view.ResumeButtonPressed -= ResumeGame;
            _view.QuitToMenuButtonPressed -= QuitGameToMenu;
            _view.CleanAndReset();
        }
    }
}