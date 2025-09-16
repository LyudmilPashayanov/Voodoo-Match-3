using System.Collections.Generic;
using Grid = Voodoo.Gameplay.Grid;

namespace Voodoo.Scripts.GameSystems
{
    public sealed class MatchCluster
    {
        public readonly List<int> Indices = new(); // all cells in this cluster
        public int Length => Indices.Count;
    }
    
    public static class Matcher
    {
        public static IReadOnlyList<MatchCluster> FindAllMatches(Grid grid, bool exitEarly = false)
        {
            List<MatchCluster> clusters = new List<MatchCluster>();
            int w = grid.Width;
            int h = grid.Height;

            // Horizontal scan
            for (int y = 0; y < h; y++)
            {
                if (exitEarly && clusters.Count > 0)
                {
                    return clusters;
                }
                
                int runLen = 1;
                for (int x = 1; x < w; x++)
                {
                    int currIdx = grid.GetIndexAt(x, y);
                    int prevIdx = grid.GetIndexAt(x - 1, y);

                    if (!grid.IsIndexEmpty(currIdx) &&
                        grid.Tiles[currIdx] == grid.Tiles[prevIdx])
                    {
                        runLen++;
                    }
                    else
                    {
                        if (runLen >= 3)
                        {
                            clusters.Add(CreateClusterHorizontal(grid, x - 1, y, runLen));
                        }
                        runLen = 1;
                    }
                }

                // finalize row
                if (runLen >= 3)
                {
                    clusters.Add(CreateClusterHorizontal(grid, w - 1, y, runLen));
                }
            }

            // Vertical scan
            for (int x = 0; x < w; x++)
            {
                if (exitEarly && clusters.Count > 0)
                {
                    return clusters;
                }
                
                int runLen = 1;
                for (int y = 1; y < h; y++)
                {
                    int currIdx = grid.GetIndexAt(x, y);
                    int prevIdx = grid.GetIndexAt(x, y - 1);

                    if (!grid.IsIndexEmpty(currIdx) && grid.Tiles[currIdx] == grid.Tiles[prevIdx])
                    {
                        runLen++;
                    }
                    else
                    {
                        if (runLen >= 3)
                        {
                            clusters.Add(CreateClusterVertical(grid, x, y - 1, runLen));
                        }
                        runLen = 1;
                    }
                }

                // finalize column
                if (runLen >= 3)
                {
                    clusters.Add(CreateClusterVertical(grid, x, h - 1, runLen));
                }
            }

            return clusters;
        }
        
        /// <summary>
        /// Used by spawner: check if placing typeId at (x,y) would immediately create a match.
        /// </summary>
        public static bool IsResultingInMatch(Grid grid,int indexToReplace, int typeIdToCheck)
        {
            sbyte original = grid.Tiles[indexToReplace];

            // temporarily place tile
            grid.Tiles[indexToReplace] = (sbyte)typeIdToCheck;
            bool hasRun = FindAllMatches(grid, true).Count > 0;
            grid.Tiles[indexToReplace] = original;

            return hasRun;
        }
        
        private static MatchCluster CreateClusterHorizontal(Grid grid, int endX, int y, int runLen)
        {
            var cluster = new MatchCluster();
            for (int k = 0; k < runLen; k++)
            {
                cluster.Indices.Add(grid.GetIndexAt(endX - k, y));
            }
            return cluster;
        }

        private static MatchCluster CreateClusterVertical(Grid grid, int x, int endY, int runLen)
        {
            var cluster = new MatchCluster();
            for (int k = 0; k < runLen; k++)
            {
                cluster.Indices.Add(grid.GetIndexAt(x, endY - k));
            }
            return cluster;
        }
    }
}