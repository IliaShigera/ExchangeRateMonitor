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

    public bool IsChanged { get; private set; }
    public decimal PercentageChange { get;  private set; }
    public Direction Direction { get; private set; }
}