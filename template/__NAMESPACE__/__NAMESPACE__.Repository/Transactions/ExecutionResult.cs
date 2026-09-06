using __NAMESPACE__.Repository.Abstractions.Transactions;

namespace __NAMESPACE__.Repository.Transactions
{
    public class ExecutionResult(bool isSuccessful) : IExecutionResult
    {
        public bool IsSuccessful { get; } = isSuccessful;
    }

    public class ExecutionResult<TResult>(bool isSuccessful, TResult result) : ExecutionResult(isSuccessful), IExecutionResult<TResult>
    {
        public TResult Result { get; } = result;
    }
}
