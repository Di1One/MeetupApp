using System.ComponentModel.DataAnnotations;

namespace MeetupApp.WebAPI.Models.Requests
{
    /// <summary>
    /// Register model
    /// </summary>
    public class RegisterUserRequestModel
    {
        /// <summary>
        /// User email
        /// </summary>
        [Required]
        public string Email { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// Confirmation of the user password
        /// </summary>
        public string PasswordConfirmation { get; set; }
    }
}
