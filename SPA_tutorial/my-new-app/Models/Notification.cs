namespace my_new_app.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string? Subject { get; set; }

        public string? Sender { get; set; }

        public string? Message { get; set; }
        public DateTime Date { get; set; }

        public string? Status { get; set; }
    }
}
