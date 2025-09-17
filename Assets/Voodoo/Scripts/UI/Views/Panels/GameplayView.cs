using UnityEngine;
using Voodoo.Scripts.UI.Views.Gameplay;

namespace Voodoo.UI.Panels
{
    public class GameplayView: UIPanelView
    {
        [SerializeField] private BoardView _boardView;
        [SerializeField] private HUDView _HUDView;

        public BoardView BoardView => _boardView;
        public HUDView HUDView => _HUDView;
        
        protected override void OnViewLeft()
        {
            _boardView.CleanAndReset();
            _HUDView.CleanAndReset();
        }

        public void ShowLoading()
        {
            Debug.Log("Loading...");
        }

        public void HideLoading()
        {
            Debug.Log("GAME READY");
        }
    }
}