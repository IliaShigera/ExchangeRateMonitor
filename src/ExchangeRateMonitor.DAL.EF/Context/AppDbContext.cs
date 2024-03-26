namespace ExchangeRateMonitor.DAL.EF.Context;

public sealed class AppDbContext : DbContext, IRepository
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ExchangeRateData> ExchangeRates { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExchangeRateEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}