using System;
using Voodoo.Scripts.UI.Views.Gameplay.Interfaces;

namespace Voodoo.UI.Controllers
{
    public class HUDPresenter
    {
        IHUDView _view;

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
    }
}