namespace Meetings.Reminder.Models
{
    public class Calendar
    {
        private readonly string[,] _contents;

        public Calendar(int row, int colunm)
        {
            _contents = new string[row, colunm];

            RowCount = row;
            ColumnCount = colunm;
        }

        public int ColumnCount { get; }

        public int RowCount { get; }

        public string this[int row, int column]
        {
            get
            {
                return _contents[row, column];
            }
            set
            {
                _contents[row, column] = value;
            }
        }
    }
}
