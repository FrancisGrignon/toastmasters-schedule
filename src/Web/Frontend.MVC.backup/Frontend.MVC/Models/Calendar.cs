namespace Frontend.MVC.Models
{
    public class Calendar
    {
        private readonly int _maxX;
        private readonly int _maxY;
        private readonly CalendarCell[,] _contents;

        public Calendar(int x, int y)
        {
            _contents = new CalendarCell[x, y];
            _maxX = x;
            _maxY = y;
        }

        public int MaxY => _maxY;

        public int MaxX => _maxX;

        public CalendarCell this[int x, int y]
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
