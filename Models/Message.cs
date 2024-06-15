namespace Practice_web.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTime SentDate { get; set; }
        public bool IsRead {  get; set; }

        public int FromUserId { get; set; }
        public User? FromUser { get; set; }

        public int ToUserId { get; set; }
        public User? ToUser { get; set; }
    }
}
