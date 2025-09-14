using UnityEngine;

namespace Voodoo.Scripts.UI.Views.Gameplay
{
    public class GamePieceView : MonoBehaviour
    {
        [SerializeField] private RectTransform _pieceTransform;
        public PieceTypeDefinition TypeDef { get; private set; }
        
        public void Bind(PieceTypeDefinition typeDefinition)
        {
            TypeDef = typeDefinition;
        }

        public void SetSize(float size)
        {
            _pieceTransform.sizeDelta = new Vector2(size, size);
            _pieceTransform.anchorMin = Vector2.zero;
            _pieceTransform.anchorMax = Vector2.zero;
        }
    }
}
