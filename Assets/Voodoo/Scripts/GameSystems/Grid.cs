namespace Voodoo.Gameplay
{
    public class Grid
    {
        private readonly int _width;
        private readonly int _height;
        private Cell[] _grid;

        public Grid(int width, int height)
        {
            _width = width;
            _height = height;
            _grid = new Cell[_width * _height];
        }
    }
}
