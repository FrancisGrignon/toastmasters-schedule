namespace Frontend.MVC.Models
{
    public class Attendee
    {
        public int Id { get; set; }

        public Role Role { get; set; }

        public Member Member { get; set; }
    }
}
