using Microsoft.Data.SqlClient;

namespace InterviewTest.Data.Decorators
{
    public interface IInterviewTestDataBaseCommand : IAsyncDisposable
    {
        Task<SqlDataReader> ExecuteReaderAsync();
        Task<object?> ExecuteScalarAsync();
        Task<int> ExecuteNonQueryAsync();
        SqlParameterCollection Parameters { get; }
        SqlTransaction Transaction { get; set; }
        string CommandText { get; set; }
    }
}
