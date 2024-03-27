using static System.Decimal;

namespace ExchangeRateMonitor.Services;

public sealed class CurrencyRateChangeAnalyzer : ICurrencyRateChangeAnalyzer
{
    private readonly decimal _threshold;

    public CurrencyRateChangeAnalyzer(IOptions<CurrencyRateChangeThresholdConfig> options)
    {
        _threshold = options.Value.Threshold;
    }

    public CurrencyRateChangeResult CalculateRateDifference(ExchangeRateData previous, ExchangeRateData latest)
    {
        ThrowIfNotValid(previous, latest);

        var difference = (latest.Rate - previous.Rate) / previous.Rate * 100;
        var differenceAbs = Math.Abs(difference);

        var result = new CurrencyRateChangeResult(
            isChanged: differenceAbs >= _threshold,
            percentageChange: differenceAbs,
            direction: DefineDirection(difference));

        return result;
    }

    private void ThrowIfNotValid(ExchangeRateData previous, ExchangeRateData latest)
    {
        ArgumentNullException.ThrowIfNull(previous, nameof(previous));
        ArgumentNullException.ThrowIfNull(latest, nameof(latest));
        ArgumentOutOfRangeException.ThrowIfNegative(_threshold);

        if (previous.Rate is Zero)
            throw new InvalidOperationException("Cannot calculate percentage change when the previous rate is zero.");
    }

    private static Direction DefineDirection(decimal difference) => difference switch
    {
        > Zero => Direction.Increased,
        < Zero => Direction.Decreased,
        _ => Direction.Unchanged
    };
}