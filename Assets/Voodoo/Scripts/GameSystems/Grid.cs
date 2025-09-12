namespace Voodoo.Gameplay
{
    public class Grid
    {
        private readonly int _width;
        private readonly int _height;
        private Cell[] _cells;
        private readonly PieceTypeDefinition[] _availableTypes;
        private readonly System.Random _random;
        
        public Grid(int width, int height, PieceTypeDefinition[] availableTypes)
        {
            _width = width;
            _height = height;
            _cells = new Cell[_width * _height];
            _availableTypes = availableTypes;
            _random = new System.Random();
        }

        public int GetIndex(int x, int y)
        {
            return y * _width + x;
        }
        
        private void FillGrid()
        {
            // TODO: Ensure there are possible moves!
            for (int i = 0; i < _cells.Length; i++)
            {
                var type = _availableTypes[_random.Next(_availableTypes.Length)];
                _cells[i] = new Cell(i % _width, i / _width, type);
            }
        }
    }
}
