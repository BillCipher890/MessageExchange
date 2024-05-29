using MessageExchange.Server.Models;

namespace MessageExchange.Server.SQL
{
    public interface ISQLExecutor
    {
        void CreateDatabase();

        Task SaveMessageAsync(Message message);

        Task<IEnumerable<Message>> GetMessagesAsync(DateTime startDate, DateTime endDate);
    }
}
