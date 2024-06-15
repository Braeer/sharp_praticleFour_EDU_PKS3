namespace Practice_web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;

        public IEnumerable<Message> SentMessages { get; set; } = null!;
        public IEnumerable<Message> ReceiveMessages { get; set; } = null!;
    }
}
