using System.Data.Common;

namespace InterviewTest.Data.Decorators
{
    public interface IInterviewTestDataBaseConnection : IAsyncDisposable
    {
        IInterviewTestDataBaseCommand CreateCommand();
        Task CloseAsync();
        Task OpenAsync();
        ValueTask<DbTransaction> BeginTransactionAsync();
    }
}
