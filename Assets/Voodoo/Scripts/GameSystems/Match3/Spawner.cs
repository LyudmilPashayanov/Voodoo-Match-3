using System;
using System.Collections.Generic;
using Voodoo.Gameplay;

namespace Voodoo.Scripts.GameSystems
{
    public sealed class Spawner
    {
        private readonly int[] _ids;
        private readonly int[] _weights;
        private readonly int _total;

        public Spawner(PieceCatalog catalog)
        {
            var ids = new List<int>(catalog.TypeCount);
            var wts = new List<int>(catalog.TypeCount);
            int total = 0;
            for (int t=0;t<catalog.TypeCount;t++)
            {
                if (!catalog.AllowRandom(t)) continue;
                int w = catalog.WeightOf(t);
                if (w <= 0) continue;
                ids.Add(t); wts.Add(w); total += w;
            }
            if (total <= 0) throw new InvalidOperationException("No spawnable types.");
            _ids = ids.ToArray(); _weights = wts.ToArray(); _total = total;
        }

        public int Pick(Random rng)
        {
            int r = rng.Next(1, _total+1), acc = 0;
            for (int i=0;i<_ids.Length;i++){ acc+=_weights[i]; if(r<=acc) return _ids[i]; }
            return _ids[^1];
        }

        public int PickFiltered(Random rng, Func<int,bool> allow)
        {
            Span<int> ids = stackalloc int[_ids.Length];
            Span<int> cum = stackalloc int[_ids.Length];
            int n=0,total=0;
            for (int i=0;i<_ids.Length;i++)
            {
                int t=_ids[i]; if(!allow(t)) continue;
                int w=_weights[i]; if (w<=0) continue;
                total+=w; ids[n]=t; cum[n]=total; n++;
            }
            if (n==0) return Pick(rng);
            int r=rng.Next(1,total+1), lo=0, hi=n-1;
            while (lo<hi){ int m=(lo+hi)>>1; if(r<=cum[m]) hi=m; else lo=m+1; }
            return ids[lo];
        }
    }
}
