try
{
    var builder = new ConfigurationBuilder();

    BuildConfig(builder);

    Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) => AddServices(context.Configuration, services))
        .UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration))
        .Build();

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
    }
}