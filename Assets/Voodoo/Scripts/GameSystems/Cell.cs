namespace Voodoo.Gameplay
{
    public struct Cell
    {
        public PieceTypeDefinition Type;
        public readonly int X;
        public readonly int Y;
        
        public Cell(int x, int y, PieceTypeDefinition type)
        {
            X = x;
            Y = y;
            Type = type;
        }
        
        // overload the == operator maybe
    }
}