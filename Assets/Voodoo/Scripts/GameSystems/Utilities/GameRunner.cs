using System;
using UnityEngine;

namespace Voodoo.GameSystems.Utilities
{
    public class GameRunner : MonoBehaviour
    {
        public event Action<float> OnTick;
        
        private void Update()
        {
            OnTick?.Invoke(Time.deltaTime);
        }

        public void PauseEngineTime()
        {
            Time.timeScale = 0;
        }
    
        public void ResumeEngineTime()
        {
            Time.timeScale = 1;
        }
    }
}
