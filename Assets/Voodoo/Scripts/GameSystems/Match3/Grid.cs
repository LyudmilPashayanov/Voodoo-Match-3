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

        public int GetIndexAt(int x, int y)
        {
            return y * Width + x;
        } 
        
        public void GetCoordsAt(int index, out int x, out int y)
        {
            x = index % Width;
            y = index / Width;
        }
        
        public static bool AreAdjacent(int a, int b, int width)
        {
            int ax = a % width, ay = a / width;
            int bx = b % width, by = b / width;
            return Math.Abs(ax - bx) + Math.Abs(ay - by) == 1;
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
