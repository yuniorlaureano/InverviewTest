using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewTest.Data.Decorators
{
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
}
