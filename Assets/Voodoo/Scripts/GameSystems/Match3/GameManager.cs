using System;
using System.Collections.Generic;
using Voodoo.Scripts.GameSystems;
using Voodoo.Scripts.GameSystems.Match3;

namespace Voodoo.Gameplay
{
    public class GameManager
    {
        public event Action<int, int> OnPieceSpawn;
        public event Action<int[]> OnPiecesClear;
        public event Action<int, int> OnSwapCommitted; 
        public event Action<IReadOnlyList<(int from, int to)>> OnGravityMoves; 

        public event Action<int> OnScoreUpdated;
        public event Action OnGameOver;
        public event Action<int> OnTimeChanged;
        
        private readonly Grid _grid;
        private readonly PieceCatalog _catalog;
        private readonly Spawner _spawner;
        private readonly Random _rng = new();

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
            ResolveCascades();
            OnScoreUpdated?.Invoke(999);
        }
        
        public void RequestSwap(int a, int b)
        {
            if (!Grid.AreAdjacent(a, b, _grid.Width))
            {
                // TODO: Event wrong move -  not even adjacent
                return;
            }

            _grid.Swap(a, b);

            var matches = Matcher.FindAllMatches(_grid);
            if (matches.Count == 0)
            {
                _grid.Swap(a, b); 
                
                // TODO: Event wrong move -  not a match
                return;
            }

            OnSwapCommitted?.Invoke(a,b);

            ResolveCascades();
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
        
        private void FillGrid()
        {
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
                        OnPieceSpawn?.Invoke(indexToFill, typeIdToSpawn);
                    }
                }
            }
        }

        private void ResolveCascades()
        {
            int cascade = 0;
            bool hasMatches = true;

            while (hasMatches)
            {
                var clusters = Matcher.FindAllMatches(_grid);
                if (clusters.Count == 0)
                {
                    hasMatches = false; // exit loop
                    continue;
                }

                // collect all cleared indices
                var allCleared = new List<int>();
                foreach (var cluster in clusters)
                    allCleared.AddRange(cluster.Indices);

                allCleared.Sort();

                foreach (int idx in allCleared)
                    _grid.ClearAtIndex(idx);

                OnPiecesClear?.Invoke(allCleared.ToArray());

                // scoring
                foreach (var cluster in clusters)
                {
                    int size = cluster.Indices.Count;
                    int largestRun = size >= 5 ? 5 : size >= 4 ? 4 : 3;
                    // _score.AddClear(size, largestRun, cascade);
                }

                List<(int from, int to)> moves = Gravity.Collapse(_grid);
                if (moves.Count > 0)
                {
                    OnGravityMoves?.Invoke(moves);
                }

                FillGrid();
                cascade++;
            }
        }
    }
}