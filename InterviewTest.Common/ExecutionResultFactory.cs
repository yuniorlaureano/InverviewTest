namespace InterviewTest.Common
{
    public interface IExecutionResultFactory
    {
        ExecutionResult Create();
        ExecutionResult AsFailure();
        ExecutionResult AsSuccessful();
        ExecutionResult<T> Create<T>(T data);
        ExecutionResult<T> AsFailure<T>(T data);
        ExecutionResult<T> AsSuccessful<T>(T data);
    }

    public class ExecutionResultFactory : IExecutionResultFactory
    {
        public ExecutionResult Create()
            => new ExecutionResult();

        public ExecutionResult AsFailure()
            => new ExecutionResult()
                .AsFailure();

        public ExecutionResult AsSuccessful()
            => new ExecutionResult()
                .AsSuccessful();

        public ExecutionResult<T> Create<T>(T data)
            => new ExecutionResult<T>()
                    .WithData(data);

        public ExecutionResult<T> AsFailure<T>(T data)
            => new ExecutionResult<T>()
                .WithData(data)
                .AsFailure();

        public ExecutionResult<T> AsSuccessful<T>(T data)
            => new ExecutionResult<T>()
                .WithData(data)
                .AsSuccessful();
    }
}
