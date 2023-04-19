using System.ComponentModel.DataAnnotations;

namespace MeetupApp.WebAPI.Models.Requests
{
    /// <summary>
    /// Model for request creating or modifying an event
    /// </summary>
    public class AddorUpdateEventRequestModel
    {
        [Required(ErrorMessage = "Name is required field")]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Owner(Email) is required field")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Owner { get; set; }

        [Required(ErrorMessage = "Start time is required field")]
        public DateTime StartTime { get; set; }
        public string? Location { get; set; }
    }
}
