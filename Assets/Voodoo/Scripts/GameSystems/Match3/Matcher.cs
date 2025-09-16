using System.Collections.Generic;
using Grid = Voodoo.Gameplay.Grid;

namespace Voodoo.Scripts.GameSystems
{
    public sealed class MatchCluster
    {
        public readonly HashSet<int> Indices; // all cells in this cluster
        public int Length => Indices.Count;
        
        public MatchCluster(IEnumerable<int> indices)
        {
            Indices = new HashSet<int>(indices);
        }
        public bool Overlaps(MatchCluster other)
        {
            return Indices.Overlaps(other.Indices);
        }

        public MatchCluster Merge(MatchCluster other)
        {
            var merged = new HashSet<int>(Indices);
            merged.UnionWith(other.Indices);
            return new MatchCluster(merged);
        }
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
        
        private static void AddOrMergeCluster(List<MatchCluster> clusters, MatchCluster newCluster)
        {
            // See if new cluster overlaps an existing one
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].Overlaps(newCluster))
                {
                    clusters[i] = clusters[i].Merge(newCluster);
                    return;
                }
            }

            // Otherwise, just add it
            clusters.Add(newCluster);
        }
    }
}