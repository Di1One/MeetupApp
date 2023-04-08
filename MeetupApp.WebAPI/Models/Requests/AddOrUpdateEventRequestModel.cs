using System.ComponentModel.DataAnnotations;

namespace MeetupApp.WebAPI.Models.Requests
{
    /// <summary>
    /// Model for request creating or modifying an event
    /// </summary>
    public class AddorUpdateEventRequestModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Owner { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public string? Location { get; set; }
    }
}
