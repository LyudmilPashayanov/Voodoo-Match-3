using UnityEngine;

namespace Voodoo.ConfigScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Match3/LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        [Tooltip("The level number")]
        public int LevelId;
        [Tooltip("The set of pieces used in this level")]
        public PieceSetConfig PieceSet;
    }
}