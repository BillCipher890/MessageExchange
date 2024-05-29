using MessageExchange.Server.Models;
using MessageExchange.Server.SQL;

namespace MessageExchange.Server.Repositories
{
    public class MessageRepository : IRepository
    {
        public ISQLExecutor SQLExecutor { get; set; }

        public MessageRepository(ISQLExecutor SQLExecutor)
        {
            this.SQLExecutor = SQLExecutor;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime startDate, DateTime endDate)
        {
            return await SQLExecutor.GetMessagesAsync(startDate, endDate);
        }

        public async Task SaveMessage(string message)
        {
            await SQLExecutor.SaveMessageAsync(new Message(message));
        }
    }
}
