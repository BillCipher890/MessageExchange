using MessageExchange.Server.Models;
using MessageExchange.Server.Repositories;
using MessageExchange.Server.SQL;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchangeTest
{
    public class MessageRepositoryTest
    {
        private readonly Mock<ISQLExecutor> _mockSQLExecutor;
        private readonly MessageRepository _repository;

        public MessageRepositoryTest()
        {
            _mockSQLExecutor = new Mock<ISQLExecutor>();
            _repository = new MessageRepository(_mockSQLExecutor.Object);
        }

        [Fact]
        public async Task GetMessagesAsync_ShouldCallSQLExecutorWithCorrectParameters()
        {
            // Arrange
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 12, 31);
            var expectedMessages = new List<Message>
            {
                new("Test Message 1"),
                new("Test Message 2")
            };

            _mockSQLExecutor.Setup(executor => executor.GetMessagesAsync(startDate, endDate))
                            .ReturnsAsync(expectedMessages);

            // Act
            var result = await _repository.GetMessagesAsync(startDate, endDate);

            // Assert
            _mockSQLExecutor.Verify(executor => executor.GetMessagesAsync(startDate, endDate), Times.Once);
            Assert.Equal(expectedMessages, result);
        }

        [Fact]
        public async Task SaveMessage_ShouldCallSQLExecutorWithCorrectMessage()
        {
            // Arrange
            var messageContent = "New Test Message";
            var message = new Message(messageContent);

            // Act
            await _repository.SaveMessage(messageContent);

            // Assert
            _mockSQLExecutor.Verify(executor => executor.SaveMessageAsync(It.Is<Message>(m => m.Body == messageContent)), Times.Once);
        }
    }
}
