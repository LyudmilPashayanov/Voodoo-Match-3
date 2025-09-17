using System;

namespace Voodoo.Scripts.UI.Views.Gameplay.Interfaces
{
    public interface IHUDView
    {
        event Action PauseButtonPressed;
        event Action ResumeButtonPressed;
        event Action QuitToMenuButtonPressed;
        void ShowPauseMenu(bool enableResume);
        void HidePauseMenu();
        public void UpdateTime(int time);
        public void UpdateScore(int score);
    }
}