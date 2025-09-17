using UnityEngine;

namespace Voodoo.ConfigScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Match3/LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        [Tooltip("All the piece types available in this level")]
        public PieceTypeDefinition[] availableTypes;
        [Tooltip("The Grid width for this level")]
        public int GridWidth;
        [Tooltip("The Grid height for this level")]
        public int GridHeight;
        [Tooltip("The time allowed for this level")]
        public int timeForLevel;
    }
}