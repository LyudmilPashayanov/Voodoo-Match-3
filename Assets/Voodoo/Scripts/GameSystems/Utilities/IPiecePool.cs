using Voodoo.Scripts.UI.Views.Gameplay;

namespace Voodoo.Scripts.GameSystems.Utilities
{
    public interface IPiecePool
    {
        GamePieceView Get(PieceTypeDefinition def);
        void Release(GamePieceView view);
    }
}