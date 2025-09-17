using UnityEngine;
using Voodoo.Scripts.UI.Views.Gameplay;

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
            Debug.Log("Loading...");
        }

        public void HideLoading()
        {
            Debug.Log("GAME READY");
        }

        protected override void OnViewLeft()
        { }
    }
}