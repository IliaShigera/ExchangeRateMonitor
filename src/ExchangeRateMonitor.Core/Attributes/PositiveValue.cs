using static System.Decimal;

namespace ExchangeRateMonitor.Core.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class PositiveValue : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is >= Zero;
    }
}
