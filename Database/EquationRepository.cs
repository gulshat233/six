using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using six.Models;

namespace six.Database
{
    public class EquationRepository
    {
        private readonly DatabaseHelper _db;

        public EquationRepository(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<int> SaveAsync(EquationResult r)
        {
            await using var connection = _db.GetConnection();
            await connection.OpenAsync();

            string query = @"
                INSERT INTO equation_results 
                    (x0, epsilon, root, iterations, timestamp, equation_text)
                VALUES (@x0, @eps, @root, @iter, @ts, @text)
                RETURNING id;";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@x0", r.X0);
            command.Parameters.AddWithValue("@eps", r.Epsilon);
            command.Parameters.AddWithValue("@root", r.Root);
            command.Parameters.AddWithValue("@iter", r.Iterations);
            command.Parameters.AddWithValue("@ts", r.Timestamp);
            command.Parameters.AddWithValue("@text", r.EquationText);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<List<EquationResult>> GetAllAsync()
        {
            var list = new List<EquationResult>();

            await using var connection = _db.GetConnection();
            await connection.OpenAsync();

            string query = "SELECT id, x0, epsilon, root, iterations, timestamp, equation_text FROM equation_results ORDER BY id DESC";

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new EquationResult
                {
                    Id = reader.GetInt32(0),
                    X0 = reader.GetDouble(1),
                    Epsilon = reader.GetDouble(2),
                    Root = reader.GetDouble(3),
                    Iterations = reader.GetInt32(4),
                    Timestamp = reader.GetDateTime(5),
                    EquationText = reader.GetString(6)
                });
            }

            return list;
        }

        public async Task ClearAllAsync()
        {
            await using var connection = _db.GetConnection();
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand("DELETE FROM equation_results", connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}