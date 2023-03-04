namespace MeetupApp.Core.DataTransferObjects
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime StartTime { get; set; }
        public string? Location { get; set; }
    }
}
