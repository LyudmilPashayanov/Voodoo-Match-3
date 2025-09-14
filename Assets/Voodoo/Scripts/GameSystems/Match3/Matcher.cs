using System.Collections.Generic;

namespace Voodoo.Scripts.GameSystems
{
    public static class Matcher
    {
        public static List<int> FindAllMatches(sbyte[] tiles, int w, int h)
        {
            var hits = new List<int>(w*h/3);

            // rows
            for (int y=0; y<h; y++)
            {
                int start = y*w, runLen=1;
                for (int x=1; x<w; x++)
                {
                    int a = tiles[start + x - 1], b = tiles[start + x];
                    if (a>=0 && b==a) runLen++;
                    else { if (runLen>=3 && tiles[start + x - 1] >= 0) for (int k=runLen;k>0;k--) hits.Add(start + x - k); runLen=1; }
                }
                if (runLen>=3 && tiles[start + w - 1] >= 0) for (int k=runLen;k>0;k--) hits.Add(start + w - k);
            }

            // cols
            for (int x=0; x<w; x++)
            {
                int runLen=1;
                for (int y=1; y<h; y++)
                {
                    int a = tiles[(y-1)*w + x], b = tiles[y*w + x];
                    if (a>=0 && b==a) runLen++;
                    else { if (runLen>=3 && tiles[(y-1)*w + x] >= 0) for (int k=runLen;k>0;k--) hits.Add((y - k)*w + x); runLen=1; }
                }
                if (runLen>=3 && tiles[(h-1)*w + x] >= 0) for (int k=runLen;k>0;k--) hits.Add((h - k)*w + x);
            }

            // dedup (optional small hashset)
            if (hits.Count <= 1) return hits;
            var set = new HashSet<int>(hits); hits.Clear(); hits.AddRange(set);
            return hits;
        }
    }
}