using MessageExchange.Server.Models;
using MessageExchange.Server.Repositories;
using MessageExchange.Server.WebSocketsFolder;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MessageExchange.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IRepository _messageRepository;
        private readonly IExchanger _webSocketExchanger;

        public MessageController(IRepository messageRepository, IExchanger webSocketExchanger)
        {
            _messageRepository = messageRepository;
            _webSocketExchanger = webSocketExchanger;
        }

        [HttpGet("GetMessages")]
        public async Task<IEnumerable<Message>> GetMessages(DateTime startDate, DateTime endDate)
        {
            return await _messageRepository.GetMessagesAsync(startDate, endDate);
        }

        [HttpGet("SaveMessage")]
        public async Task<IActionResult> SaveMessage([MaxLength(128)]string message)
        {
            await _messageRepository.SaveMessage(message);
            await _webSocketExchanger.SendMessageToAll(message);
            return Ok();
        }

        [HttpGet("ws")]
        public async Task AddToReceivers()
        {
            await _webSocketExchanger.AddReceiver(HttpContext);
        }
    }
}
