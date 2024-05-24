using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace InterviewTest.Data.Decorators
{
    public class InterviewTestDataBaseConnection : IInterviewTestDataBaseConnection
    {
        private readonly SqlConnection _connection;

        public InterviewTestDataBaseConnection(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public IInterviewTestDataBaseCommand CreateCommand()
        {
            return new InterviewTestDataBaseCommand(_connection.CreateCommand());
        }

        public async Task CloseAsync()
        {
            await _connection.CloseAsync();
        }

        public async Task OpenAsync()
        {
            await _connection.OpenAsync();
        }

        public ValueTask<DbTransaction> BeginTransactionAsync()
        {
            return _connection.BeginTransactionAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
