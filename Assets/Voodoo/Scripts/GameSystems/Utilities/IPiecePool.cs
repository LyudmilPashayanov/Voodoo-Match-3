using System.Collections.Generic;
using Voodoo.ConfigScriptableObjects;
using Voodoo.UI.Presenters;

namespace Voodoo.GameSystems.Utilities
{
    public interface IPiecePool
    {
        GamePiecePresenter Get(PieceTypeDefinition def);
        void Release(GamePiecePresenter presenter);
        void ReleaseAll(IEnumerable<GamePiecePresenter> activePieces);
    }
}