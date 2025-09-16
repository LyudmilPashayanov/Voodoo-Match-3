using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using Voodoo.Scripts.GameSystems;
using Voodoo.Scripts.GameSystems.Utilities;

namespace Voodoo.Gameplay
{
    public interface IGameFlow
    { 
        IPiecePool Pool { get; }
        UniTask StartGameAsync(CancellationToken ct = default);
        UniTask EndGameAsync(CancellationToken ct = default);
        
        // Commands from UI
        void PieceClicked(int pieceClickedIndex);
        void SwapPiece(int pieceSwappedIndex, SwipeDirection direction);
        void Pause();
        void Resume();

        // Events (pure data, no Unity types)
        Func<int, PieceTypeDefinition,UniTask> PieceSpawnAsync { get; set; }
        Func<IReadOnlyList<MatchCluster>, UniTask> PiecesClearAsync  { get; set; }
        Func<int,int,UniTask> PieceSwapAsync { get; set; }         
        Func<int,int,UniTask> NoMatchSwapAsync { get; set; }         
        Func<UniTask> InvalidMoveAsync { get; set; }         
        Func<IReadOnlyList<(int from, int to)>, UniTask> OnGravityMovesAsync { get; set; }
        event Action<int> ScoreChanged;
        event Action<int> TimeChanged;             // seconds remaining
        event Action GameOver;
        event Action<int, int> GameLoaded;   // width, height (optional)
    }
}