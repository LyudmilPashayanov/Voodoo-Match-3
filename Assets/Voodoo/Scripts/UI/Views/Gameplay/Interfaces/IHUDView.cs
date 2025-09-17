using System;
using UnityEngine;

namespace Voodoo.UI.Views.Interfaces
{
    public interface IHUDView
    {
        event Action PauseButtonPressed;
        event Action ResumeButtonPressed;
        event Action QuitToMenuButtonPressed;
        void ShowPauseMenu(bool enableResume);
        void HidePauseMenu();
        void UpdateTime(int time);
        void UpdateScore(int score);
        void CleanAndReset();
        Vector2 GetScorePosition();
        Transform GetRootTransform();
    }
}