using MessageExchange.Server.SQL;

namespace MessageExchangeTest
{
    public class PostgreSQLExecutorTest
    {
        private readonly string _connectionString = "Host=localhost;Username=admin;Password=postgreSQL12345;";
        private readonly string _databaseName = "testdb";

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange & Act
            var executor = new PostgreSQLExecutor(_connectionString, _databaseName);

            // Assert
            Assert.Equal(_connectionString, executor.ConnectionString);
            Assert.Equal(_databaseName, executor.DatabaseName);
        }
    }
}
