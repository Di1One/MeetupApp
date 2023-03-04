namespace MeetupApp.WebAPI.Models.Requests
{
    /// <summary>
    /// Model for request creating or modifying an event
    /// </summary>
    public class AddOrUpdateEventRequestModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime StartTime { get; set; }
        public string? Location { get; set; }
    }
}
