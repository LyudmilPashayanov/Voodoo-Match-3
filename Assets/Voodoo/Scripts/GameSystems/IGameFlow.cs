namespace Voodoo.Gameplay
{
    public interface IGameFlow
    { 
        System.Threading.Tasks.Task StartGameAsync(System.Threading.CancellationToken ct = default);
        System.Threading.Tasks.Task EndGameAsync();
        
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
        event System.Action<int, int> BoardInitialized;   // width, height (optional)
    }
}