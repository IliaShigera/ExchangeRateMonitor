namespace ExchangeRateMonitor.Core.Interfaces;

public interface ICurrencyRateChangeAnalyzer
{
    CurrencyRateChangeResult CalculateRateDifference(ExchangeRateData previous, ExchangeRateData latest);
}