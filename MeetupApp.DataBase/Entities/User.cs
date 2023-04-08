using System.ComponentModel.DataAnnotations;

namespace MeetupApp.DataBase.Entities
{
    public class User : IBaseEntity
    {
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

        public List<Event>? Events { get; set; }

    }
}
