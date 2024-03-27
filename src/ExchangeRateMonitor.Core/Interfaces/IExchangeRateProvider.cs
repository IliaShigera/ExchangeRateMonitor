namespace ExchangeRateMonitor.Core.Interfaces;

public interface IExchangeRateProvider
{
    Task<ExchangeRateData> GetLatestRatesAsync();
}