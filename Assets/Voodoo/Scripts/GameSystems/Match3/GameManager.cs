using System;
using Voodoo.Scripts.GameSystems;
using Voodoo.Scripts.GameSystems.Match3;

namespace Voodoo.Gameplay
{
    public class GameManager
    {
        public event Action<int, int> OnPieceSpawn;
        public event Action<int[]> OnPiecesClear;
        public event Action<int, int> OnPieceMoved;
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

        }

        public void StartGame()
        {
            Array.Fill(_grid.Tiles, (sbyte)-1);
            InitialFill();
            ResolveCascades();
            OnScoreUpdated?.Invoke(999);
        }
        
        public void RequestSwap(int a, int b)
        {
            if (!Grid.AreAdjacent(a, b, _grid.Width)) return;
            // optional: reject swapping bombs/nonswappables via catalog flags

            _grid.Swap(a, b);
            var matches = Matcher.FindAllMatches(_grid.Tiles, _grid.Width, _grid.Height);
            if (matches.Count == 0) { _grid.Swap(a, b); return; }

            OnPieceMoved?.Invoke(a, b);
            OnPieceMoved?.Invoke(b, a);
            ResolveCascades();
            OnScoreUpdated?.Invoke(999);
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
        
        private void InitialFill()
        {
            int w = _grid.Width;
            int h = _grid.Height;
            
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x <w ; x++)
                {
                    int i = y*w + x;
                    if (_grid.Tiles[i] >= 0) continue;
                    int t = _spawner.PickFiltered(_rng, typeId => !CreatesImmediateRun(_grid.Tiles, w, x, y, typeId));
                    _grid.Tiles[i] = (sbyte)t;
                    OnPieceSpawn?.Invoke(i, t);
                }
            }
        }

        private void ResolveCascades()
        {
            int cascade = 0;
            while (true)
            {
                var hits = Matcher.FindAllMatches(_grid.Tiles, _grid.Width, _grid.Height);
                if (hits.Count == 0)
                {
                    break;
                }

                hits.Sort();
                foreach (int idx in hits) _grid.ClearAtIndex(idx);
                OnPiecesClear?.Invoke(hits.ToArray());

                int largestRun = hits.Count >= 5 ? 5 : hits.Count >= 4 ? 4 : 3;
              //  _score.AddClear(hits.Count, largestRun, cascade);

                var moves = Gravity.Collapse(_grid);
                foreach (var (from, to) in moves) OnPieceMoved?.Invoke(from, to);

                // fill
                int w = _grid.Width, h = _grid.Height;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int i = y * w + x;
                        if (_grid.Tiles[i] >= 0) continue;
                        int t = _spawner.PickFiltered(_rng,
                            typeId => !CreatesImmediateRun(_grid.Tiles, w, x, y, typeId));
                        _grid.Tiles[i] = (sbyte)t;
                        OnPieceSpawn?.Invoke(i, t);
                    }
                }

                cascade++;
            }
        }

        private static bool CreatesImmediateRun(sbyte[] gridTiles, int gridWidth, int x, int y, int typeId)
        {
            if (x >= 2)
            {
                int a = y * gridWidth + (x - 1);
                int b = y * gridWidth + (x - 2);

                if (gridTiles[a] == typeId && gridTiles[b] == typeId)
                {
                    return true;
                }
            }
            if (y >= 2)
            {
                int a = (y - 1) * gridWidth + x;
                int b = (y - 2) * gridWidth + x;

                if (gridTiles[a] == typeId && gridTiles[b] == typeId)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}