using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace IoTService.Services
{
    public class MySQLService
    {
        private static string server => "localhost";
        private static string user => "iotuser";
        private static string pass => "iotuser";
        private static string db => "iothome";
        private static string ConnectionString => $"Server={server};User ID={user};Password={pass};Database={db}";


        public static async Task<IEnumerable<T>> Query<T>(string query, Func<MySqlDataReader, T> createFunc)
        {
            List<T> output = new List<T>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                var command = new MySqlCommand(query, connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        output.Add(createFunc(reader));
                    }
                }
            }

            return output;

        }

        public static async Task NonQuery<T>(IEnumerable<T> inserts, Func<T, string> CommandFunction)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                foreach (T item in inserts)
                {
                    MySqlCommand command = new MySqlCommand(CommandFunction(item), connection);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task<int> Insert<T>(T element, Func<T, string> CommandFunction)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                    MySqlCommand command = new MySqlCommand(CommandFunction(element), connection);
                    await command.ExecuteNonQueryAsync();
                return await LastId(connection);
            }
        }

        private static async Task<int> LastId(MySqlConnection connection)
        {
                MySqlCommand command = new MySqlCommand("SELECT LAST_INSERT_ID();", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return (await reader.ReadAsync() ? reader.GetInt32(0) : 0);
                }
            
        }
    }
}
