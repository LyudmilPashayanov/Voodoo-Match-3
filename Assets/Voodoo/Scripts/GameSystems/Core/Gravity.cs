using System.Collections.Generic;

namespace Voodoo.Gameplay.Core
{
    public static class Gravity
    {
        public static List<(int from,int to)> Collapse(Grid grid)
        {
            var moves = new List<(int,int)>(grid.Count/3);
            int w = grid.Width;
            int h = grid.Height;
            for (int x = 0; x < w; x++)
            {
                int writeY = 0; // bottom row
                for (int y = 0; y < h; y++) // scan from bottom to top
                {
                    int readIndex = grid.GetIndexAt(x, y);
                    sbyte tile = grid.Tiles[readIndex];

                    if (tile >= 0)
                    {
                        int writeIndex = grid.GetIndexAt(x, writeY);
                        if (readIndex != writeIndex)
                        {
                            grid.Tiles[writeIndex] = tile;
                            grid.Tiles[readIndex] = -1;
                            moves.Add((readIndex, writeIndex));
                        }
                        writeY++;
                    }
                }
            }
            return moves;
        }
    }
}