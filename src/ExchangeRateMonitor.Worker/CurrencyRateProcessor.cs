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
            var latestData = await FetchAndProcessLatestRateData();
            
            if (previousData is null)
            {
                await NotifyAboutBaseline(latestData);
                return;
            }

            var result = CalculateRateDifference(previousData, latestData);

            if (result.IsChanged)
                await NotifyAboutChanges(result, latestData);
            else
                await NotifyNoChanges(latestData);
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

    private async Task<ExchangeRateData> FetchAndProcessLatestRateData()
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

    private async Task NotifyAboutChanges(CurrencyRateChangeResult result, ExchangeRateData data)
    {
        var message =
            $"The exchange rate {result.Direction} by {result.PercentageChange}%. " +
            $"1 {data.BaseCurrency} = {data.Rate:00} {data.TargetCurrency}";

        await _notificationService.NotifyAsync(message);
    }

    private async Task NotifyAboutBaseline(ExchangeRateData data)
    {
        var message =
            $"Exchange rate baseline established. 1 {data.BaseCurrency} = {data.Rate:0.00} {data.TargetCurrency}";

        await _notificationService.NotifyAsync(message);
    }

    private async Task NotifyNoChanges(ExchangeRateData data)
    {
        var message =
            $"Exchange rates remain stable. 1 {data.BaseCurrency} = {data.Rate:0.00} {data.TargetCurrency}";

        await _notificationService.NotifyAsync(message);
    }
}