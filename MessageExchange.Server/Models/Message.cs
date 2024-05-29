namespace MessageExchange.Server.Models
{
    public class Message
    {
        public int Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Body { get; private set; }

        public Message(string body)
        {
            Body = body;
        }

        public Message(int id, DateTime createdAt, string body)
        {
            Id = id;
            CreatedAt = createdAt;
            Body = body;
        }
    }
}
