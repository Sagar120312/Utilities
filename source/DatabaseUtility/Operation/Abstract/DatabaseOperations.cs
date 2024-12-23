using DatabaseUtility.Operation.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatabaseUtility.Operation.Abstract
{
    public class DatabaseOperations : IDatabaseOperations
    {
        private readonly string _connectionString;

        public DatabaseOperations(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteStoredProcedure(string procedureName, Dictionary<string, object>? parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    var dataTable = new DataTable();
                    var adapter = new SqlDataAdapter(command);

                    connection.Open();
                    adapter.Fill(dataTable);

                    return dataTable;
                }
            }
        }

        public DataTable ExecuteRawQuery(string query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    var dataTable = new DataTable();
                    var adapter = new SqlDataAdapter(command);

                    connection.Open();
                    adapter.Fill(dataTable);

                    return dataTable;
                }
            }
        }
    }
}
