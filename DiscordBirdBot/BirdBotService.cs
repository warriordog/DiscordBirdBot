using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBirdBot
{
    public class BirdBotService : IHostedService
    {
        private readonly BirdBotMain _birdBotMain;

        public BirdBotService(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            _birdBotMain = scope.ServiceProvider.GetRequiredService<BirdBotMain>();
        }

        public Task StartAsync(CancellationToken _) => _birdBotMain.StartAsync();
        public Task StopAsync(CancellationToken _) => _birdBotMain.StopAsync();
    }
}