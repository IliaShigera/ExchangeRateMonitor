namespace ExchangeRateMonitor.DAL.EF.Context;

public static class MigrationApplier
{
    public static void ApplyMigrations(DbContext dbContext)
    {
        if (dbContext.Database.IsNpgsql())
        {
            dbContext.Database.Migrate();
        }
    }
}