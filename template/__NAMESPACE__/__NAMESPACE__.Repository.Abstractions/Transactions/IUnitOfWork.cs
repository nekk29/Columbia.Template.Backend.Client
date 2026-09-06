namespace __NAMESPACE__.Repository.Abstractions.Transactions
{
    public interface IUnitOfWork
    {
        ITransaction BeginTransaction();
        ITransaction? GetCurrentTransaction();
        void Execute<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded);
        void ExecuteInTransaction<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded);
        Task ExecuteAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, CancellationToken cancellationToken = default);
        TResult Execute<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded);
        TResult ExecuteInTransaction<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded);
        Task<TResult> ExecuteAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, CancellationToken cancellationToken = default);
        Task<TResult> ExecuteInTransactionAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, CancellationToken cancellationToken = default);
        void Commit();
        Task CommitAsync();
        void SendAudit();
    }
}
