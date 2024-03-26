namespace ExchangeRateMonitor.Core;

public sealed class ExchangeRateData
{
    public ExchangeRateData(int timestamp, string baseCurrency, string targetCurrency, DateTime dateUtc, decimal rate)
    {
        Timestamp = timestamp;
        BaseCurrency = baseCurrency;
        TargetCurrency = targetCurrency;
        DateUtc = dateUtc;
        Rate = rate;
    }

    private ExchangeRateData()
    {
        // EF only
    }

    public Guid Id { get; private set; }
    public string BaseCurrency { get; init; }
    public string TargetCurrency { get; init; }
    public int Timestamp { get; init; }
    public DateTime DateUtc { get; init; }
    public decimal Rate { get; init; }
}