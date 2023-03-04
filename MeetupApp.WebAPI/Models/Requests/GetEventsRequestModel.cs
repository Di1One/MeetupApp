namespace MeetupApp.WebAPI.Models.Requests
{
    public class GetEventsRequestModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime StartTime { get; set; }
        public string? Location { get; set; }
    }
}
