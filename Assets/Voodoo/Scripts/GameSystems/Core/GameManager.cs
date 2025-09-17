using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Voodoo.GameSystems.Utilities;

namespace Voodoo.Gameplay.Core
{
    public class GameManager
    {
        public Func<int, int, UniTask> OnPieceSpawnAsync { get; set; }
        public Func<IReadOnlyList<MatchCluster>,UniTask> OnPiecesClearAsync { get; set; }
        public Func<int, int, UniTask> OnSwapCommittedAsync { get; set; }
        public Func<UniTask> OnInvalidMoveAsync { get; set; }
        public Func<int, int, UniTask> OnNoMatchSwapAsync { get; set; }
        public Func<IReadOnlyList<(int from, int to)>, UniTask> OnGravityMovesAsync { get; set; }

        public event Action<int> OnScoreUpdated;
        public event Action OnGameOver;
        public event Action<int> OnTimeChanged;
        
        private readonly Grid _grid;
        private readonly PieceCatalog _catalog;
        private readonly Spawner _spawner;
        private readonly Random _rng = new();
        private int _currentClickedIndex = -1; // -1 -> nothing is clicked

        private float _timeForLevel;
        private int _lastBroadcastSecond = -1;
        private bool _isRunning;
        
        public GameManager(int gridWidth, int gridHeight, PieceCatalog pieceCatalog, int timeForLevel)
        {
            _grid = new Grid(gridWidth, gridHeight);
            _catalog = pieceCatalog;
            _spawner = new Spawner(_catalog);
            _timeForLevel = timeForLevel;
            _rng = new Random(123);
        }

        public void StartGame()
        {
            _ = FillGrid();
            _ = ResolveCascadesAsync();
            _isRunning = true;
        }

        public void TickTime(float deltaTime)
        {
            if (!_isRunning) return;

            _timeForLevel -= deltaTime;
            
            int seconds = (int)Math.Ceiling(_timeForLevel);
            if (seconds != _lastBroadcastSecond)
            {
                _lastBroadcastSecond = seconds;
                OnTimeChanged?.Invoke(seconds);
            }

            if (_timeForLevel <= 0f)
            {
                _isRunning = false;
                OnGameOver?.Invoke();
            }
        }
        
        public void EndGame()
        {
            _isRunning = false;
        }

        public void Resume()
        {
            _isRunning = true;
        }

        public void Pause()
        {
            _isRunning = false;
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
                    await OnInvalidMoveAsync();
                    return;
                }
                
                await TrySwap(previousClickedIndex, indexClicked);
            }
        }
        
        public async UniTask SwipePiece(int indexClicked,  SwipeDirection direction)
        {
            if(_grid.TryGetAdjacentIndex(indexClicked, direction, out int neighborIndex))
            {
                await TrySwap(indexClicked, neighborIndex);
            }
            else
            {
                _currentClickedIndex = -1;
                await OnInvalidMoveAsync();
            }
        }

        private async UniTask TrySwap(int indexA, int indexB)
        {
            _grid.Swap(indexA, indexB);
                
            var matches = Matcher.FindAllMatches(_grid);
            if (matches.Count == 0)
            {
                _grid.Swap(indexA, indexB); 
                _currentClickedIndex = -1;
                await OnNoMatchSwapAsync(indexA, indexB);
                return;
            }
            _currentClickedIndex = -1;
            await OnSwapCommittedAsync(indexA, indexB);
            await ResolveCascadesAsync();
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