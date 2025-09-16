using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Voodoo.Scripts.GameSystems;
using Voodoo.Scripts.GameSystems.Match3;

namespace Voodoo.Gameplay
{
    public class GameManager
    {
        public Func<int, int, UniTask> OnPieceSpawnAsync { get; set; }
        public Func<IReadOnlyList<MatchCluster>,UniTask> OnPiecesClearAsync { get; set; }
        public Func<int, int, UniTask> OnSwapCommittedAsync { get; set; }
        public Func<int, int, UniTask> OnInvalidMoveAsync { get; set; }
        public Func<int, int, UniTask> OnNoMatchSwapAsync { get; set; }
        public Func<IReadOnlyList<(int from, int to)>, UniTask> OnGravityMovesAsync { get; set; } // public event Action<IReadOnlyList<(int from, int to)>> OnGravityMoves; 

        public event Action<int> OnScoreUpdated;
        public event Action OnGameOver;
        public event Action<int> OnTimeChanged;
        
        private readonly Grid _grid;
        private readonly PieceCatalog _catalog;
        private readonly Spawner _spawner;
        private readonly Random _rng = new();
        private int _currentClickedIndex = -1; // if -1 - nothing is clicked
        
        public GameManager(int gridWidth, int gridHeight, PieceCatalog pieceCatalog)
        {
            _grid = new Grid(gridWidth, gridHeight);
            _catalog = pieceCatalog;
            _spawner = new Spawner(_catalog);
            _rng = new Random(123);
        }

        public void StartGame()
        {
            FillGrid();
            ResolveCascadesAsync();
            OnScoreUpdated?.Invoke(999);
        }
        
        public async UniTask ClickPiece(int indexClicked)
        {
            if (_currentClickedIndex == -1) // first click
            {
                _currentClickedIndex = indexClicked;
                
                // TODO: Check if it is a bomb piece.
            }
            else if (_currentClickedIndex == indexClicked) // player cancelling his click
            {
                _currentClickedIndex = -1;
            }
            else if (_currentClickedIndex != indexClicked) // user attempts to swap
            {
                int previousClickedIndex = _currentClickedIndex;
                if (!Grid.AreAdjacent(previousClickedIndex, indexClicked, _grid.Width))
                {
                    _currentClickedIndex = -1;
                    await OnInvalidMoveAsync(previousClickedIndex, indexClicked);
                    return;
                }
                
                _grid.Swap(previousClickedIndex, indexClicked);
                
                var matches = Matcher.FindAllMatches(_grid);
                if (matches.Count == 0)
                {
                    _grid.Swap(previousClickedIndex, indexClicked); 
                    _currentClickedIndex = -1;
                    await OnNoMatchSwapAsync(previousClickedIndex, indexClicked);
                    return;
                }
                _currentClickedIndex = -1;
                await OnSwapCommittedAsync(previousClickedIndex, indexClicked);
                await ResolveCascadesAsync();
            }
        }

        public void EndGame()
        {
            
        }

        public void Resume()
        {
            // Resuming the timer and game flow.
        }

        public void Pause()
        {
            // Pausing the timer and game flow.
        }
        
        private async UniTask FillGrid()
        {
            var tasks = new List<UniTask>();

            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    int indexToFill = _grid.GetIndexAt(x, y);
                    if (_grid.IsIndexEmpty(indexToFill))
                    {
                        int typeIdToSpawn = _spawner.PickPieceWithFilter(_rng,
                            t => !Matcher.IsResultingInMatch(_grid, indexToFill, t));
                        _grid.Tiles[indexToFill] = (sbyte)typeIdToSpawn;
                        
                        tasks.Add(OnPieceSpawnAsync(indexToFill, typeIdToSpawn));
                    }
                }
            }
            
            await UniTask.WhenAll(tasks);
        }

        private async UniTask ResolveCascadesAsync(CancellationToken ct = default)
        {
            int cascade = 0;
            bool hasMatches = true;

            while (hasMatches)
            {
                IReadOnlyList<MatchCluster> clustersToDestroy = Matcher.FindAllMatches(_grid);
                if (clustersToDestroy.Count == 0)
                {
                    hasMatches = false; // exit loop
                    continue;
                }

                // collect all cleared indices
                var allCleared = new List<int>();
                foreach (var cluster in clustersToDestroy)
                    allCleared.AddRange(cluster.Indices);

                allCleared.Sort();

                foreach (int idx in allCleared)
                    _grid.ClearAtIndex(idx);

                await OnPiecesClearAsync(clustersToDestroy);

                // scoring
                foreach (var cluster in clustersToDestroy)
                {
                    int size = cluster.Indices.Count;
                    int largestRun = size >= 5 ? 5 : size >= 4 ? 4 : 3;
                    // _score.AddClear(size, largestRun, cascade);
                }

                List<(int from, int to)> moves = Gravity.Collapse(_grid);
                if (moves.Count > 0)
                {
                   await OnGravityMovesAsync(moves);
                }

               await FillGrid();
                cascade++;
            }
        }
    }
}