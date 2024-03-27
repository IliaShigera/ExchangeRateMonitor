namespace ExchangeRateMonitor.Core.Interfaces;

public interface INotificationService
{
    public Task NotifyAsync(string message);
}