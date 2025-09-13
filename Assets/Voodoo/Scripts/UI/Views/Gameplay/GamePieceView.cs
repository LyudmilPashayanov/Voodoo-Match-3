using UnityEngine;

namespace Voodoo.Scripts.UI.Views.Gameplay
{
    public class GamePieceView : MonoBehaviour
    {
        public PieceTypeDefinition TypeDef { get; private set; }

        public void Bind(PieceTypeDefinition typeDefinition)
        {
            TypeDef = typeDefinition;
        }
    }
}
