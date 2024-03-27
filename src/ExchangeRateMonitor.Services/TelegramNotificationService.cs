namespace ExchangeRateMonitor.Services;

public sealed class TelegramNotificationService : INotificationService
{
    private readonly ILogger _logger;
    private readonly TelegramConfig _telegramConfig;
    private readonly TelegramBotClient _botClient;

    public TelegramNotificationService(IOptions<TelegramConfig> options, ILogger logger)
    {
        _logger = logger;
        _telegramConfig = options.Value;
        _botClient = new TelegramBotClient(_telegramConfig.Token);
    }

    public async Task NotifyAsync(string message)
    {
        try
        {
            await _botClient.SendTextMessageAsync(chatId: _telegramConfig.ChatId, text: message);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while sending the Telegram notification");

            throw;
        }
    }
}