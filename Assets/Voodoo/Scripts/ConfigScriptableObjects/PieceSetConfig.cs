using UnityEngine;

namespace Voodoo.ConfigScriptableObjects
{
    [CreateAssetMenu(fileName = "PieceSet", menuName = "Match3/Piece Set", order = 0)]
    public class PieceSetConfig : ScriptableObject
    {
        [Tooltip("All the piece types available in this configuration")]
        public PieceTypeDefinition[] availableTypes;
        [Tooltip("The Grid width")]
        public int GridWidth;
        [Tooltip("The Grid height")]
        public int GridHeight;
    }
}