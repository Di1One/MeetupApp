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
        [Required(ErrorMessage = "Email is required field")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        [Required(ErrorMessage = "Password is required field")]
        [MinLength(8)]
        public string Password { get; set; }
        /// <summary>
        /// Confirmation of the user password
        /// </summary>
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string PasswordConfirmation { get; set; }
    }
}
