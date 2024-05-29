using MessageExchange.Server.WebSocketsFolder;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Net.WebSockets;
using System.Text;

namespace MessageExchangeTest
{
    public class WebSocketExchangeTest
    {
        private static WebSocketExchanger CreateExchanger()
        {
            return new WebSocketExchanger();
        }

        [Fact]
        public async Task AddReceiver_ShouldSetStatusCode400_IfNotWebSocketRequest()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var webSocketsMock = new Mock<WebSocketManager>();
            var responseMock = new Mock<HttpResponse>();

            webSocketsMock.Setup(w => w.IsWebSocketRequest).Returns(false);
            httpContextMock.Setup(h => h.WebSockets).Returns(webSocketsMock.Object);
            httpContextMock.Setup(h => h.Response).Returns(responseMock.Object);

            var exchanger = CreateExchanger();

            // Act
            await exchanger.AddReceiver(httpContextMock.Object);

            // Assert
            responseMock.VerifySet(r => r.StatusCode = 400);
        }

        [Fact]
        public async Task SendMessageToAll_ShouldSendMessageToAllOpenWebSockets()
        {
            // Arrange
            var webSocketMock1 = new Mock<WebSocket>();
            var webSocketMock2 = new Mock<WebSocket>();

            webSocketMock1.Setup(w => w.State).Returns(WebSocketState.Open);
            webSocketMock2.Setup(w => w.State).Returns(WebSocketState.Open);

            var exchanger = CreateExchanger();
            var message = "Hello, World!";
            var buffer = Encoding.UTF8.GetBytes(message);

            exchanger.WebSockets.TryAdd("1", webSocketMock1.Object);
            exchanger.WebSockets.TryAdd("2", webSocketMock2.Object);

            // Act
            await exchanger.SendMessageToAll(message);

            // Assert
            webSocketMock1.Verify(w => w.SendAsync(It.Is<ArraySegment<byte>>(a => a.Array.SequenceEqual(buffer)), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);
            webSocketMock2.Verify(w => w.SendAsync(It.Is<ArraySegment<byte>>(a => a.Array.SequenceEqual(buffer)), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendMessageToAll_ShouldNotSendMessageToClosedWebSockets()
        {
            // Arrange
            var webSocketMock1 = new Mock<WebSocket>();
            var webSocketMock2 = new Mock<WebSocket>();

            webSocketMock1.Setup(w => w.State).Returns(WebSocketState.Closed);
            webSocketMock2.Setup(w => w.State).Returns(WebSocketState.Closed);

            var exchanger = CreateExchanger();
            var message = "Hello, World!";

            exchanger.WebSockets.TryAdd("2", webSocketMock2.Object);
            exchanger.WebSockets.TryAdd("1", webSocketMock1.Object);

            // Act
            await exchanger.SendMessageToAll(message);

            // Assert
            webSocketMock1.Verify(w => w.SendAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<WebSocketMessageType>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
            webSocketMock2.Verify(w => w.SendAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<WebSocketMessageType>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SendMessageToAll_ShouldNotThrow_WhenNoWebSockets()
        {
            // Arrange
            var exchanger = CreateExchanger();
            var message = "Hello, World!";

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => exchanger.SendMessageToAll(message));
            Assert.Null(exception);
        }
    }
}