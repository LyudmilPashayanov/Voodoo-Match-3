using System.Collections.Generic;

namespace Voodoo.Gameplay.Core
{
    public sealed class MatchCluster
    {
        /// <summary>
        /// The board indices that belong to this cluster.
        /// </summary>
        public readonly HashSet<int> Indices;
        /// <summary>
        /// Number of tiles in this cluster.
        /// </summary>
        public int Length => Indices.Count;
        /// <summary>
        /// Score value calculated and assigned by ScoreManager.
        /// </summary>
        public int ScoreValue { get; internal set; }
        public ClusterType  ClusterType { get; internal set; }
        
        public MatchCluster(IEnumerable<int> indices, ClusterType clusterType = ClusterType.Normal)
        {
            Indices = new HashSet<int>(indices);
            ClusterType = clusterType;
            ScoreValue = 0;
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
}