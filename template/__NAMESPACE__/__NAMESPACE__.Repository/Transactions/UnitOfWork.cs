using __NAMESPACE__.Repository.Abstractions.Transactions;
using Microsoft.EntityFrameworkCore;
using Storage = Microsoft.EntityFrameworkCore.Storage;

namespace __NAMESPACE__.Repository.Transactions
{
    public class UnitOfWork<TContext>(TContext dbContext) : IUnitOfWork where TContext : DbContext
    {
        private DbContext Context => dbContext;

        public ITransaction BeginTransaction()
            => new Transaction(Context.Database.BeginTransaction());

        public ITransaction? GetCurrentTransaction()
            => Context.Database.CurrentTransaction != null ? new Transaction(Context.Database.CurrentTransaction) : null;

        public void Execute<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded)
            => ExecuteOperation(state, operation, verifySucceeded);

        public void ExecuteInTransaction<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded)
        {
            ExecuteOperation(
                state,
                stateIn => ExecuteInTransaction(
                    stateIn,
                    stateTransaction => { operation.Invoke(stateTransaction); return string.Empty; }
                ),
                verifySucceeded
            );
        }

        private void ExecuteOperation<TState>(TState state, Action<TState> operation, Func<TState, IExecutionResult> verifySucceeded)
        {
            ExecuteStrategy(
                state,
                stateIn => { operation.Invoke(stateIn); return default!; },
                stateIn =>
                {
                    return verifySucceeded != null ?
                        new Storage.ExecutionResult<TState>(verifySucceeded.Invoke(stateIn).IsSuccessful, default!) :
                        new Storage.ExecutionResult<TState>(true, default!);
                }
            );
        }

        public async Task ExecuteAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, CancellationToken cancellationToken = default)
            => await ExecuteOperationAsync(state, operation, verifySucceeded, cancellationToken);

        public async Task ExecuteInTransactionAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, CancellationToken cancellationToken = default)
        {
            await ExecuteOperationAsync(
                state,
                async (stateIn, cancellationTokenIn) => await ExecuteInTransactionAsync(
                    stateIn,
                    async (stateTransaction, cancellationTokenTransaction) => { await operation.Invoke(stateTransaction, cancellationTokenTransaction); return string.Empty; },
                    cancellationTokenIn
                ),
                verifySucceeded,
                cancellationToken
            );
        }

        private async Task ExecuteOperationAsync<TState>(TState state, Func<TState, CancellationToken, Task> operation, Func<TState, CancellationToken, Task<IExecutionResult>> verifySucceeded, CancellationToken cancellationToken = default)
        {
            await ExecuteStrategyAsync(
                state,
                async (stateIn, cancellationTokenIn) => { await operation.Invoke(stateIn, cancellationTokenIn); return default!; },
                async (stateIn, cancellationTokenIn) =>
                {
                    return verifySucceeded != null ?
                        new Storage.ExecutionResult<TState>((await verifySucceeded.Invoke(stateIn, cancellationTokenIn)).IsSuccessful, default!) :
                        new Storage.ExecutionResult<TState>(true, default!);
                },
                cancellationToken
            );
        }

        public TResult Execute<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded)
            => ExecuteOperation(state, operation, verifySucceeded);

        public TResult ExecuteInTransaction<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded)
        {
            return ExecuteOperation(
                state,
                stateIn => ExecuteInTransaction(stateIn, operation),
                verifySucceeded
            );
        }

        private TResult ExecuteOperation<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, IExecutionResult<TResult>> verifySucceeded)
        {
            return ExecuteStrategy(
                state,
                stateIn => operation.Invoke(stateIn),
                stateIn =>
                {
                    if (verifySucceeded != null)
                    {
                        var result = verifySucceeded.Invoke(stateIn);
                        return new Storage.ExecutionResult<TResult>(result.IsSuccessful, result.Result);
                    }

                    return new Storage.ExecutionResult<TResult>(true, default!);
                }
            );
        }

        public async Task<TResult> ExecuteAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, CancellationToken cancellationToken = default)
            => await ExecuteOperationAsync(state, operation, verifySucceeded, cancellationToken);

        public async Task<TResult> ExecuteInTransactionAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, CancellationToken cancellationToken = default)
        {
            return await ExecuteOperationAsync(
                state,
                async (stateIn, cancellationTokenIn) => await ExecuteInTransactionAsync(stateIn, operation, cancellationTokenIn),
                verifySucceeded,
                cancellationToken
            );
        }

        private async Task<TResult> ExecuteOperationAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<IExecutionResult<TResult>>> verifySucceeded, CancellationToken cancellationToken = default)
        {
            return await ExecuteStrategyAsync(
                state,
                async (stateIn, cancellationTokenIn) => await operation.Invoke(stateIn, cancellationTokenIn),
                async (stateIn, cancellationTokenIn) =>
                {
                    if (verifySucceeded != null)
                    {
                        var result = await verifySucceeded.Invoke(stateIn, cancellationTokenIn);
                        return new Storage.ExecutionResult<TResult>(result.IsSuccessful, result.Result);
                    }

                    return new Storage.ExecutionResult<TResult>(true, default!);
                },
                cancellationToken
            );
        }

        private TResult ExecuteStrategy<TState, TResult>(TState state, Func<TState, TResult> operation, Func<TState, Storage.ExecutionResult<TResult>> verifySucceeded)
        {
            var executionStrategy = Context.Database.CreateExecutionStrategy();

            return executionStrategy.Execute(
                state,
                (context, stateIn) =>
                {
                    try
                    {
                        var result = operation.Invoke(stateIn);
                        Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        ClearChangeTracker();
                        throw;
                    }
                },
                (context, stateIn) => verifySucceeded.Invoke(stateIn)
            );
        }

        private async Task<TResult> ExecuteStrategyAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, Func<TState, CancellationToken, Task<Storage.ExecutionResult<TResult>>> verifySucceeded, CancellationToken cancellationToken = default)
        {
            var executionStrategy = Context.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(
                state,
                async (context, stateIn, cancellationTokenIn) =>
                {
                    try
                    {
                        var result = await operation.Invoke(stateIn, cancellationTokenIn);
                        await CommitAsync();
                        return result;
                    }
                    catch (Exception)
                    {
                        ClearChangeTracker();
                        throw;
                    }
                },
                async (context, stateIn, cancellationTokenIn) => await verifySucceeded.Invoke(stateIn, cancellationTokenIn),
                cancellationToken
            );
        }

        private TResult ExecuteInTransaction<TState, TResult>(TState state, Func<TState, TResult> operation)
        {
            var transactionInCourse = GetCurrentTransaction()?.TransactionId != default;

            if (transactionInCourse)
                return operation.Invoke(state);

            using var transaction = BeginTransaction();

            try
            {
                var result = operation.Invoke(state);
                transaction.Commit();
                return result;
            }
            catch (ResultException<TResult> rex)
            {
                transaction.Rollback();
                ClearChangeTracker();
                return rex.Result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                ClearChangeTracker();
                throw;
            }
        }

        private async Task<TResult> ExecuteInTransactionAsync<TState, TResult>(TState state, Func<TState, CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            var transactionInCourse = GetCurrentTransaction()?.TransactionId != default;

            if (transactionInCourse)
                return await operation.Invoke(state, cancellationToken);

            using var transaction = BeginTransaction();

            try
            {
                var result = await operation.Invoke(state, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch (ResultException<TResult> rex)
            {
                transaction.Rollback();
                ClearChangeTracker();
                return rex.Result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                ClearChangeTracker();
                throw;
            }
        }

        private void ClearChangeTracker()
        {
            Context.ChangeTracker.Clear();
        }

        public void Commit()
        {
            dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public void SendAudit()
        {

        }
    }
}

