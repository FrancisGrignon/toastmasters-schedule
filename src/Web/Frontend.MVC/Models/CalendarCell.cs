namespace Frontend.MVC.Models
{
    public class CalendarCell
    {
        public int AttendeeId { get; set; }

        public int MeetingId { get; set; }

        public string Value { get; set; }

        public bool CanRefuse { get; set; }

        public bool CanAccept { get; set; }
    }
}
