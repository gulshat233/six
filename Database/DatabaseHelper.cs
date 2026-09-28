using Npgsql;

namespace six.Database
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            _connectionString = "Host=localhost;Port=5432;Database=newton_db;Username=postgres;Password=68Gulshat23";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string createTable = @"
                CREATE TABLE IF NOT EXISTS equation_results (
                    id SERIAL PRIMARY KEY,
                    x0 DOUBLE PRECISION NOT NULL,
                    epsilon DOUBLE PRECISION NOT NULL,
                    root DOUBLE PRECISION NOT NULL,
                    iterations INTEGER NOT NULL,
                    timestamp TIMESTAMP NOT NULL,
                    equation_text TEXT NOT NULL
                );";

            using var command = new NpgsqlCommand(createTable, connection);
            command.ExecuteNonQuery();
        }

        public NpgsqlConnection GetConnection() => new NpgsqlConnection(_connectionString);
    }
}