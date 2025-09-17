using System.Collections.Generic;
using UnityEngine;

namespace Voodoo.ConfigScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Match3/LevelsConfig", order = 0)]
    public class LevelsConfig : ScriptableObject
    {
        public List<LevelEntry> Levels;
    }
}