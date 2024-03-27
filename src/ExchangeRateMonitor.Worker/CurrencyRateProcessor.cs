namespace ExchangeRateMonitor.Worker;

public sealed class CurrencyRateProcessor
{
    private readonly ILogger _logger;
    private readonly IRepository _repository;
    private readonly IExchangeRateProvider _exchangeRateProvider;
    private readonly ICurrencyRateChangeAnalyzer _currencyRateChangeAnalyzer;
    private readonly INotificationService _notificationService;

    public CurrencyRateProcessor(
        ILogger logger,
        IRepository repository,
        IExchangeRateProvider exchangeRateProvider,
        ICurrencyRateChangeAnalyzer currencyRateChangeAnalyzer,
        INotificationService notificationService)
    {
        _logger = logger;
        _repository = repository;
        _exchangeRateProvider = exchangeRateProvider;
        _currencyRateChangeAnalyzer = currencyRateChangeAnalyzer;
        _notificationService = notificationService;
    }

    public async Task RunAsync()
    {
        _logger.Information("Rate processing cycle started.");
        
        try
        {
            var previousData = await GetPreviousRateData();

            if (previousData is null)
            {
                _logger.Information("No previous exchange rate data found. Initializing baseline.");

                await FetchAndProcessLatestRateData();
                return;
            }

            var latestData = await FetchAndProcessLatestRateData();
            
            var result = CalculateRateDifference(previousData, latestData);

            if (result.IsChanged) await NotifyAboutChanges(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Rate processing cycle failed.");
        }
        finally
        {
            _logger.Information("Rate processing cycle ended.");
        }
    }

    private async Task<ExchangeRateData?> GetPreviousRateData() =>
        await _repository.ExchangeRates
            .AsNoTracking()
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefaultAsync();

    private async Task<ExchangeRateData?> FetchAndProcessLatestRateData()
    {
        var latestData = await _exchangeRateProvider.GetLatestRatesAsync();

        if (latestData is null)
            throw new InvalidOperationException("Exchange rate provider returned null. Unable to process rates.");
        
        await _repository.ExchangeRates.AddAsync(latestData);
        await _repository.SaveChangesAsync();

        return latestData;
    }

    private CurrencyRateChangeResult CalculateRateDifference(ExchangeRateData previous, ExchangeRateData latest) =>
        _currencyRateChangeAnalyzer.CalculateRateDifference(previous, latest);

    private async Task NotifyAboutChanges(CurrencyRateChangeResult result)
    {
        var message = string.Format("The exchange rate {Direction} by {PercentageChange}%",
            result.Direction,
            result.PercentageChange);

        await _notificationService.NotifyAsync(message);
    }
}