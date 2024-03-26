namespace ExchangeRateMonitor.Core.Interfaces;

/// <summary>
/// Represents a data repository interface. This interface is implemented
/// by a DbContext-derived class to provide out-of-the-box CRUD operations on entities.
/// </summary>
public interface IRepository
{
    DbSet<ExchangeRateData> ExchangeRates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}