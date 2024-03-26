namespace ExchangeRateMonitor.DAL.EF.Context;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExchangeRateEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}