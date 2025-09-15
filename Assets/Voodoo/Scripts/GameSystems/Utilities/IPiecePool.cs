using Voodoo.UI.Controllers;

namespace Voodoo.Scripts.GameSystems.Utilities
{
    public interface IPiecePool
    {
        GamePiecePresenter Get(PieceTypeDefinition def);
        void Release(GamePiecePresenter presenter);
    }
}