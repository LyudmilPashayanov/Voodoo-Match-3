using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using Voodoo.Gameplay.Core;
using Voodoo.GameSystems.Utilities;

namespace Voodoo.Gameplay
{
    public interface IGameFlow
    { 
        IPiecePool Pool { get; }
        UniTask StartGameAsync(CancellationToken ct = default);
        void EndGame();
        
        // Commands from Unity
        void PieceClicked(int pieceClickedIndex);
        void SwapPiece(int pieceSwappedIndex, Direction direction);
        void Pause();
        void Resume();
        void Tick(float deltaTime);

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