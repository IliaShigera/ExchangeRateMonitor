namespace ExchangeRateMonitor.Services;

public sealed class ThirdPartyExchangeRateProvider : IExchangeRateProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private ThirdPartyExchangeRateApiConfig _config;

    public ThirdPartyExchangeRateProvider(
        IOptions<ThirdPartyExchangeRateApiConfig> options,
        IHttpClientFactory httpClientFactory,
        ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = options.Value;
    }

    public async Task<ExchangeRateData> GetLatestRatesAsync()
    {
        var baseCurrency = _config.BaseCurrency;
        var targetCurrency = _config.TargetCurrency;
        
        var httpClient = _httpClientFactory.CreateClient();

        var requestUrl = $"{_config.Url}?access_key={_config.Key}&base={baseCurrency}&symbols={targetCurrency}";

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        try
        {
            return await retryPolicy.ExecuteAsync(async () =>
            {
                var response = await httpClient.GetAsync(requestUrl);
                var content = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeAnonymousType(content, new
                {
                    timestamp = 0,
                    date = DateTime.UtcNow,
                    rates = new Dictionary<string, decimal>()
                });

                if (responseData is null)
                    throw new InvalidOperationException("Deserialization failed - API may have returned invalid data");

                _logger.Information("Exchange rates retrieved successfully: {baseCurrency}/{targetCurrency}",
                    baseCurrency,
                    targetCurrency);

                var exchangeRateData = new ExchangeRateData(
                    responseData.timestamp,
                    baseCurrency,
                    targetCurrency,
                    responseData.date.ToUniversalTime(),
                    rate: responseData.rates[targetCurrency]);

                return exchangeRateData;
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, "An error occurred during the API request.");
            throw;
        }
        catch (JsonSerializationException ex)
        {
            _logger.Error(ex, "An error occurred while deserializing exchange rates");
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while fetching exchange rates.");
            throw;
        }
    }
}