namespace MeetupApp.DataBase.Entities
{
    public class Event : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }

        public List<User>? Users { get; set; }

        public DateTime StartTime { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedDate { get; set;}
    }
}
