using MessageExchange.Server.Models;
using Npgsql;

namespace MessageExchange.Server.SQL
{
    public class PostgreSQLExecutor : ISQLExecutor
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }

        public PostgreSQLExecutor(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }

        public void CreateDatabase()
        {
            var createDbConnectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Database = "postgres"
            }.ToString();

            using var connection = new NpgsqlConnection(createDbConnectionString);
            connection.Open();

            using var cmdCheckDBExist = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{DatabaseName}'", connection);
            var result = cmdCheckDBExist.ExecuteScalar();
            if (result == null)
            {
                using var cmdCreateDB = new NpgsqlCommand($"CREATE DATABASE {DatabaseName}", connection);
                cmdCreateDB.ExecuteNonQuery();
            }
        }

        public void CreateDefaultTables()
        {
            var createDbConnectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Database = DatabaseName
            }.ToString();

            using var connection = new NpgsqlConnection(createDbConnectionString);
            connection.Open();

            var createMessageTableString = "CREATE TABLE IF NOT EXISTS message (id SERIAL PRIMARY KEY, created_at timestamptz DEFAULT NOW(), body TEXT)";
            using var createMessageTableCommand = new NpgsqlCommand(createMessageTableString, connection);
            createMessageTableCommand.ExecuteNonQuery();
        }

        public async Task SaveMessageAsync(Message message)
        {
            var createDbConnectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Database = DatabaseName
            }.ToString();

            using var connection = new NpgsqlConnection(createDbConnectionString);
            connection.Open();

            await using var command = new NpgsqlCommand($"INSERT INTO message(body) VALUES (\'{message.Body}\')", connection);
            command.ExecuteNonQuery();
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime startDate, DateTime endDate)
        {
            var createDbConnectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Database = DatabaseName
            }.ToString();

            using var connection = new NpgsqlConnection(createDbConnectionString);
            connection.Open();

            await using var command = new NpgsqlCommand($"SELECT * FROM message WHERE created_at >= \'{startDate}\' AND created_at <= \'{endDate}\'", connection);
            var reader = command.ExecuteReader();
            
            var resultList = new List<Message>();
            while (reader.Read())
            {
                var idTask = reader.GetFieldValueAsync<int>(0);
                var createdAtTask = reader.GetFieldValueAsync<DateTime>(1);
                var messageBodyTask = reader.GetFieldValueAsync<string>(2);
                resultList.Add(new Message(await idTask, await createdAtTask, await messageBodyTask));
            }
            
            return resultList;
        }
    }
}
