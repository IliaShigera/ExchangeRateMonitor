namespace ExchangeRateMonitor.DAL.EF.EntityConfig;

internal sealed class ExchangeRateEntityTypeConfiguration : IEntityTypeConfiguration<ExchangeRateData>
{
    public void Configure(EntityTypeBuilder<ExchangeRateData> builder)
    {
        builder.ToTable("ExchangeRates");

        builder.HasKey(er => er.Id);
        
        builder.Property(er => er.BaseCurrency).IsRequired();
        builder.Property(er => er.TargetCurrency).IsRequired();
        builder.Property(er => er.Timestamp).IsRequired();
        builder.Property(er => er.DateUtc).IsRequired();
        builder.Property(er => er.Rate).IsRequired();
    }
}