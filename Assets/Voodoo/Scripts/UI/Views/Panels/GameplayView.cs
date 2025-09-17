using UnityEngine;
using UnityEngine.Android;
using Voodoo.UI.Views.Gameplay;

namespace Voodoo.UI.Panels
{
    public class GameplayView: UIPanelView
    {
        [SerializeField] private BoardView _boardView;
        [SerializeField] private HUDView _HUDView;
        [SerializeField] private FloatingScoreView _floatigScorePrefab;

        public BoardView BoardView => _boardView;
        public HUDView HUDView => _HUDView;
        public FloatingScoreView FloatingScoreViewView => _floatigScorePrefab;
        
        public void ShowLoading()
        {
            //TODO: Add loading screen
        }

        public void HideLoading()
        {
            //TODO: Hide loading screen
        }

        protected override void OnViewLeft()
        { }
    }
}