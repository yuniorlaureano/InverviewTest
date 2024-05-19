using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InterviewTest.Data
{
    public interface ISqlFactory
    {
        Task<SqlConnection> GetConnection();

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
    }
}
