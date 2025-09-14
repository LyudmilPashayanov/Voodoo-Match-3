using Cysharp.Threading.Tasks;
using System.Threading;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.Gameplay
{
    public interface IGameFlow
    { 
        IPiecePool Pool { get; }
        UniTask StartGameAsync(CancellationToken ct = default);
        UniTask EndGameAsync(CancellationToken ct = default);
        
        // Commands from UI
        void RequestSwap(int fromIndex, int toIndex);
        void Pause();
        void Resume();

        // Events (pure data, no Unity types)
        event System.Action<int, PieceTypeDefinition> PieceSpawned;
        event System.Action<int[]> PiecesCleared;
        event System.Action<int, int> PieceMoved;         // from -> to
        event System.Action<int> ScoreChanged;
        event System.Action<int> TimeChanged;             // seconds remaining
        event System.Action GameOver;
        event System.Action<int, int> GameLoaded;   // width, height (optional)
    }
}