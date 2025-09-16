using UnityEngine;
using Voodoo.Scripts.UI.Views.Gameplay;

namespace Voodoo.UI.Panels
{
    public class GameplayView: UIPanelView
    {
        [SerializeField] private BoardView _boardView;

        public BoardView BoardView => _boardView;
        
        protected override void OnViewLeft()
        {
            // Cleaning when the view is left
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