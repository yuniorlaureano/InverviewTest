using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace InterviewTest.Data
{
    public interface ISqlFactory
    {
        Task<IInterviewTestDataBaseConnection> GetConnection();
    }

    public class SqlFactory : ISqlFactory
    {
        private string ConnectionString { get; set; }

        public SqlFactory(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("InterviewTest");
        }

        public async Task<IInterviewTestDataBaseConnection> GetConnection()
        {
            var connection = new InterviewTestDataBaseConnection(ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }

    public interface IInterviewTestDataBaseCommand : IAsyncDisposable
    {
        Task<SqlDataReader> ExecuteReaderAsync();
        Task<object?> ExecuteScalarAsync();
        Task<int> ExecuteNonQueryAsync();
        SqlParameterCollection Parameters { get; }
        SqlTransaction Transaction { get; set; }
        string CommandText { get; set; }
    }

    public class InterviewTestDataBaseCommand : IInterviewTestDataBaseCommand
    {
        private readonly SqlCommand _command;

        public InterviewTestDataBaseCommand()
        {
            _command = new SqlCommand();
        }

        public InterviewTestDataBaseCommand(SqlCommand command)
        {
            _command = command;
        }

        public Task<SqlDataReader> ExecuteReaderAsync()
        {
            return _command.ExecuteReaderAsync();
        }

        public Task<object?> ExecuteScalarAsync()
        {
            return _command.ExecuteScalarAsync();
        }

        public Task<int> ExecuteNonQueryAsync()
        {
            return _command.ExecuteNonQueryAsync();
        }

        public SqlParameterCollection Parameters
        {
            get
            {
                return _command.Parameters;
            }
        }

        public SqlTransaction Transaction
        {
            get
            {
                return _command.Transaction;
            }
            set { _command.Transaction = value; }
        }

        public string CommandText
        {
            get
            {
                return _command.CommandText;
            }
            set { _command.CommandText = value; }
        }

        public async ValueTask DisposeAsync()
        {
            await _command.DisposeAsync();
        }
    }

    public interface IInterviewTestDataBaseConnection : IAsyncDisposable
    {
        IInterviewTestDataBaseCommand CreateCommand();
        Task CloseAsync();
        Task OpenAsync();
        ValueTask<DbTransaction> BeginTransactionAsync();
    }

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
