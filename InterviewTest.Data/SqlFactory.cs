using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;

namespace InterviewTest.Data
{
    public interface ISqlFactory
    {
        Task<SqlConnection> GetConnection();
        SqlParameter CreateParam<T>(string name, T value, SqlDbType type);

    }

    public class SqlFactory : ISqlFactory
    {
        private string ConnectionString { get; set; }

        public SqlFactory(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("InterviewTest");
        }

        public async Task<SqlConnection> GetConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            return connection;
        }

        public SqlParameter CreateParam<T>(string name, T value, SqlDbType type)
        {
            return new SqlParameter()
            {
                ParameterName = name,
                Value = value,
                SqlDbType = type
            };
        }
    }
}
