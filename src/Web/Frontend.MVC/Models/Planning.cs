namespace Frontend.MVC.Models
{
    public class Planning
    {
        private readonly int _maxX;
        private readonly int _maxY;
        private readonly string[,] _contents;

        public Planning(int x, int y)
        {
            _contents = new string[x, y];
            _maxX = x;
            _maxY = y;
        }

        public int MaxY => _maxY;

        public int MaxX => _maxX;

        public string this[int x, int y]
        {
            get
            {
                return _contents[x, y];
            }
            set
            {
                _contents[x, y] = value;
            }
        }
    }
}
