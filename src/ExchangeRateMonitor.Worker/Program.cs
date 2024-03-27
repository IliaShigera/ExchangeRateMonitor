using ExchangeRateMonitor.Core.Configs;

try
{
    var builder = new ConfigurationBuilder();

    BuildConfig(builder);

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) => AddServices(context.Configuration, services))
        .UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration))
        .Build();

    EnsureDatabaseUpToDate(host);
    
    Log.Information("Starting up!");
}
catch (Exception ex)
{
    Log.Error("The following {Exception} was thrown during application startup", ex);
}
finally
{
    Log.CloseAndFlush();
}

internal static partial class Program // startup
{
    private static IConfiguration BuildConfig(IConfigurationBuilder builder)
    {
        return builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static void AddServices(IConfiguration configuration, IServiceCollection services)
    {
        var connection = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connection));

        services.AddOptions<ThirdPartyExchangeRateApiConfig>()
            .BindConfiguration(ThirdPartyExchangeRateApiConfig.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<CurrencyRateChangeThresholdConfig>()
            .BindConfiguration(CurrencyRateChangeThresholdConfig.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient();

        services.AddScoped<IRepository, AppDbContext>();
        services.AddScoped<IExchangeRateProvider, ThirdPartyExchangeRateProvider>();
        services.AddScoped<ICurrencyRateChangeAnalyzer, CurrencyRateChangeAnalyzer>();
    }

    private static void EnsureDatabaseUpToDate(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        MigrationApplier.ApplyMigrations(dbContext);

        Log.Information("Database is up-to-date.");
    }
}