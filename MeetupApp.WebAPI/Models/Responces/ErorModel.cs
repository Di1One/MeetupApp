namespace MeetupApp.WebAPI.Models.Responces
{
    /// <summary>
    /// Model for returning errors from api
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string? Message { get; set; }
    }
}
