namespace ExchangeRateMonitor.DAL.EF.Context;

internal sealed class DataContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connection = args.FirstOrDefault();

        if (connection is null)
            throw new InvalidOperationException("The database connection string was not provided in the command-line arguments.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connection);

        return new AppDbContext(optionsBuilder.Options);
    }
}