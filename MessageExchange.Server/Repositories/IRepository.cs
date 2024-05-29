using MessageExchange.Server.Models;

namespace MessageExchange.Server.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<Message>> GetMessagesAsync(DateTime startDate, DateTime endDate);
        Task SaveMessage(string message);
    }
}
