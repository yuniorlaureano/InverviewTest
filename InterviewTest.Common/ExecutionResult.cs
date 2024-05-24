namespace InterviewTest.Common
{
    public class ExecutionResult
    {
        public bool IsSuccess { get; protected set; }
        public Dictionary<string, List<string>> Errors { get; protected set; } = new Dictionary<string, List<string>>();

        public virtual ExecutionResult AddError(string name, string message)
        {
            if(Errors.TryGetValue(name, out var result))
            {
                result.Add(message);
                return this;
            }

            Errors.Add(name, new List<string>
            {
                message
            });

            return this;
        }

        public virtual ExecutionResult AsSuccessful()
        {
            IsSuccess = true;
            return this;
        }

        public virtual ExecutionResult AsFailure()
        {
            IsSuccess = false;
            return this;
        }
    }

    public class ExecutionResult<T> : ExecutionResult
    {
        public T Data { get; set; }

        public ExecutionResult<T> WithData(T data)
        {
            Data = data;
            return this;
        }

        public override ExecutionResult<T> AddError(string name, string message)
        {
            base.AddError(name, message);
            return this;
        }

        public override ExecutionResult<T> AsSuccessful()
        {
            base.AsSuccessful();
            return this;
        }

        public override ExecutionResult<T> AsFailure()
        {
            base.AsFailure();
            return this;
        }

        public ExecutionResult<NT> Clone<NT>(NT data)
        {
            return new ExecutionResult<NT>()
            {
                Errors = this.Errors,
                IsSuccess = this.IsSuccess,
            }.WithData(data);
        }
    }
}
