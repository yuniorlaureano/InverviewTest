using InterviewTest.Data.Decorators;
using InterviewTest.Data.Interfaces;
using Microsoft.Extensions.Configuration;

namespace InterviewTest.Data
{
    public class SqlFactory : ISqlFactory
    {
        private string ConnectionString { get; set; }

        public SqlFactory(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("InterviewTest");
        }

        public async Task<IInterviewTestDataBaseConnection> GetConnectionAsync()
        {
            var connection = new InterviewTestDataBaseConnection(ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
