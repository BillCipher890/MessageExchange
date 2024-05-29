namespace MessageExchange.Server.WebSocketsFolder
{
    public interface IExchanger
    {
        Task AddReceiver(HttpContext HttpContext);
        Task SendMessageToAll(string message);
    }
}
