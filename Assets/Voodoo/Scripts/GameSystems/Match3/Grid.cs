using System;

namespace Voodoo.Gameplay
{
    public class Grid
    {
        public readonly int Width;
        public readonly int Height;
        public readonly sbyte[] Tiles; // -1 empty, else 0 ... TypeCount-1
        public int Count => Tiles.Length;

        public Grid(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            
            Width = width;
            Height = height;
            Tiles = new sbyte[width * height];
            Array.Fill(Tiles, (sbyte)-1);
        }

        public int GetIndexAt(int x, int y) => y * Width + x;
        public static bool AreAdjacent(int a, int b, int w)
        {
            int ax=a%w, ay=a/w, bx=b%w, by=b/w;
            return Math.Abs(ax-bx)+Math.Abs(ay-by)==1;
        }

        public void Swap(int a, int b)
        {
            sbyte t = Tiles[a];
            Tiles[a] = Tiles[b];
            Tiles[b] = t;
        }
        
        public void ClearAtIndex(int idx) => Tiles[idx] = -1;
        public bool IsIndexEmpty(int idx) => Tiles[idx] < 0;
    }
}
