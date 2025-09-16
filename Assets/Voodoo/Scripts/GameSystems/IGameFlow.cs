using System;
using System.Collections.Generic;
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
        void PieceClicked(int pieceClickedIndex);
        void Pause();
        void Resume();

        // Events (pure data, no Unity types)
        event Action<int, PieceTypeDefinition> PieceSpawned;
        event Action<int[]> PiecesCleared;
        event Action<int, int> PiecesSwapped;         // from -> to
        event Action<IReadOnlyList<(int from, int to)>> GravityMoves;
        event Action<int> ScoreChanged;
        event Action<int> TimeChanged;             // seconds remaining
        event Action GameOver;
        event Action<int, int> GameLoaded;   // width, height (optional)
    }
}