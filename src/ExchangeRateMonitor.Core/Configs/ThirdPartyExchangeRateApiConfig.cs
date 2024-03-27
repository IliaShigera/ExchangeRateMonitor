namespace ExchangeRateMonitor.Core.Configs;

public sealed class ThirdPartyExchangeRateApiConfig
{
    public const string Section = nameof(ThirdPartyExchangeRateApiConfig);

    [Required]
    public string Key { get; set; } = string.Empty;


    [Required]
    public string Url { get; set; } = string.Empty;
    
    
    [Required]
    public string BaseCurrency { get; set; } = string.Empty;

    
    [Required]
    public string TargetCurrency { get; set; } = string.Empty;
}