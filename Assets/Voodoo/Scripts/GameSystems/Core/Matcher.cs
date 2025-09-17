using System.Collections.Generic;

namespace Voodoo.Gameplay.Core
{
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
                            MatchCluster newCluster = CreateClusterHorizontal(grid, x - 1, y, runLen);
                            AddOrMergeCluster(clusters, newCluster);
                        }
                        runLen = 1;
                    }
                }

                // finalize row
                if (runLen >= 3)
                {
                    MatchCluster newCluster = CreateClusterHorizontal(grid, w - 1, y, runLen);
                    AddOrMergeCluster(clusters, newCluster);
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
                            MatchCluster newCluster = CreateClusterVertical(grid, x, y - 1, runLen);
                            AddOrMergeCluster(clusters, newCluster);
                        }
                        runLen = 1;
                    }
                }

                // finalize column
                if (runLen >= 3)
                {
                    MatchCluster newCluster = CreateClusterVertical(grid, x, h - 1, runLen);
                    AddOrMergeCluster(clusters, newCluster);
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
        
        public static List<MatchCluster> TriggerBombClusters(Grid grid, PieceCatalog catalog, int startIndex, int radius)
        {
            var result = new List<MatchCluster>();
            var visitedBombs = new HashSet<int>();
            var queue = new Queue<int>();

            queue.Enqueue(startIndex);

            while (queue.Count > 0)
            {
                int center = queue.Dequeue();
                if (!visitedBombs.Add(center))
                    continue;

                grid.GetCoordsAt(center, out int cx, out int cy);
                var cleared = new List<int>();

                for (int dy = -radius; dy <= radius; dy++)
                for (int dx = -radius; dx <= radius; dx++)
                {
                    int nx = cx + dx;
                    int ny = cy + dy;

                    if (nx < 0 || nx >= grid.Width || ny < 0 || ny >= grid.Height)
                        continue;

                    int idx = grid.GetIndexAt(nx, ny);
                    if (grid.IsIndexEmpty(idx))
                        continue;

                    cleared.Add(idx);

                    if (catalog.RoleOf(grid.Tiles[idx]) == PieceRole.Bomb && !visitedBombs.Contains(idx))
                    {
                        queue.Enqueue(idx);
                    }
                }

                if (cleared.Count > 0)
                {
                    result.Add(new MatchCluster(cleared, ClusterType.Bomb));
                }
            }

            return result;
        }
        
        private static MatchCluster CreateClusterHorizontal(Grid grid, int endX, int y, int runLen)
        {
            HashSet<int> clusterIndeces = new();
            for (int k = 0; k < runLen; k++)
            {
                clusterIndeces.Add(grid.GetIndexAt(endX - k, y));
            }
            return new MatchCluster(clusterIndeces);
        }

        private static MatchCluster CreateClusterVertical(Grid grid, int x, int endY, int runLen)
        {
            HashSet<int> clusterIndexes = new();
            for (int k = 0; k < runLen; k++)
            {
                clusterIndexes.Add(grid.GetIndexAt(x, endY - k));
            }
            return new MatchCluster(clusterIndexes);
        }
        
        /// <summary>
        /// See if new cluster overlaps an existing one, otherwise just adds it.
        /// </summary>
        private static void AddOrMergeCluster(List<MatchCluster> clusters, MatchCluster newCluster)
        {
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].Overlaps(newCluster))
                {
                    clusters[i] = clusters[i].Merge(newCluster);
                    return;
                }
            }

            clusters.Add(newCluster);
        }
    }
}