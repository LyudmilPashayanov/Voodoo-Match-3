using UnityEngine;

namespace Voodoo.UI.Panels
{
    public class GameplayView: UIPanelView
    {
        
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