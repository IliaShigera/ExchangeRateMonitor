namespace ExchangeRateMonitor.Core.Configs;

public sealed class TelegramConfig
{
    public const string Section = nameof(TelegramConfig);
    
    public string ChatId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}