using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBirdBot;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        // Create environment
        using var host = CreateHost(args);

        // Run application
        await host.RunAsync();
    }

    private static IHost CreateHost(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, configuration) =>
                {
                    // Add configuration file from bin directory
                    configuration.AddJsonFile("bin/appsettings.json", true, true);
                }
            )
            .ConfigureServices(
                (ctx, services) =>
                {
                    // Inject config
                    services.AddOptions<BotOptions>()
                        .Bind(ctx.Configuration.GetSection(nameof(BotOptions)))
                        .ValidateDataAnnotations();
                    services.AddOptions<BirdCommandOptions>()
                        .Bind(ctx.Configuration.GetSection(nameof(BirdCommandOptions)))
                        .ValidateDataAnnotations();

                    // Inject main app logic
                    services.AddHostedService<BirdService>();
                }
            )
            .Build();
    }
}