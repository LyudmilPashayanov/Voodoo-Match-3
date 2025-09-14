using System.Collections.Generic;
using Voodoo.Gameplay;

namespace Voodoo.Scripts.GameSystems.Match3
{
    public static class Gravity
    {
        public static List<(int from,int to)> Collapse(Grid g)
        {
            var moves = new List<(int,int)>(g.Count/3);
            int w=g.Width, h=g.Height;
            for (int x=0;x<w;x++)
            {
                int write = w*(h-1) + x;            // bottom index
                for (int y=h-1;y>=0;y--)
                {
                    int read = y*w + x;
                    if (g.Tiles[read] >= 0)
                    {
                        if (read != write)
                        {
                            g.Tiles[write] = g.Tiles[read];
                            g.Tiles[read] = -1;
                            moves.Add((read, write));
                        }
                        write -= w;
                    }
                }
            }
            return moves;
        }
    }
}