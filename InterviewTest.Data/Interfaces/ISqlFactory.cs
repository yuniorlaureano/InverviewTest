using InterviewTest.Data.Decorators;

namespace InterviewTest.Data.Interfaces
{
    public interface ISqlFactory
    {
        Task<IInterviewTestDataBaseConnection> GetConnectionAsync();
    }
}
