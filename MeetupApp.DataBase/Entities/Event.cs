using System.ComponentModel.DataAnnotations;

namespace MeetupApp.DataBase.Entities
{
    public class Event : IBaseEntity
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Owner { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedDate { get; set;}

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
