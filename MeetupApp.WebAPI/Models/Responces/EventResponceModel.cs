namespace MeetupApp.WebAPI.Models.Responces
{
    public class EventResponceModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime StartTime { get; set; }
        public string? Location { get; set; }
    }
}
