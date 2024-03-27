namespace ExchangeRateMonitor.Core.Configs;

public sealed class CurrencyRateChangeThresholdConfig
{
    public const string Section = nameof(CurrencyRateChangeThresholdConfig);
    
    [Required, PositiveValue]
    public decimal Threshold { get; set; }
}