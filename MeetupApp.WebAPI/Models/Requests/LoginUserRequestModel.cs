using System.ComponentModel.DataAnnotations;

namespace MeetupApp.WebAPI.Models.Requests
{
    /// <summary>
    /// Login model
    /// </summary>
    public class LoginUserRequestModel
    {
        /// <summary>
        /// User email
        /// </summary>
        [Required(ErrorMessage = "Email is required field")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        [Required(ErrorMessage = "Password is required field")]
        public string Password { get; set; }
    }
}
