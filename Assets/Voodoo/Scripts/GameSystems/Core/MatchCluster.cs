using System.Collections.Generic;

namespace Voodoo.Gameplay.Core
{
    public sealed class MatchCluster
    {
        public readonly HashSet<int> Indices;
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
}