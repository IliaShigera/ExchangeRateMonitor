namespace ExchangeRateMonitor.Core.Models;

public sealed class CurrencyRateChangeResult
{
    public CurrencyRateChangeResult(bool isChanged, decimal percentageChange, Direction direction)
        : this(isChanged)
    {
        PercentageChange = percentageChange;
        Direction = direction;
    }

    public CurrencyRateChangeResult(bool isChanged)
    {
        IsChanged = isChanged;
    }

    private CurrencyRateChangeResult()
    {
    }

    public bool IsChanged { get; }
    public decimal PercentageChange { get; }
    public Direction Direction { get; }
}