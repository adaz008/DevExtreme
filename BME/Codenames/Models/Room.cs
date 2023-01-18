namespace Codenames.Models
{
    public class Room
    {
        public string Name { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public bool RequiresPasskey { get; set; }
    }
}
