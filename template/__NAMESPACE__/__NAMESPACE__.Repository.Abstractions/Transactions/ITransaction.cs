namespace __NAMESPACE__.Repository.Abstractions.Transactions
{
    public interface ITransaction : IDisposable
    {
        Guid TransactionId { get; }
        void Commit();
        Task CommitAsync(CancellationToken cancellationToken = default);
        void Rollback();
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
