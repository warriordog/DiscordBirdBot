using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBirdBot
{
    public class BirdBotMain
    {
        private readonly DiscordClient        _discord;
        private readonly ILogger<BirdBotMain> _logger;
        private readonly Random               _random;
        private readonly BotOptions           _options;

        public BirdBotMain(ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, ILogger<BirdBotMain> logger)
        {
            _logger = logger;
            _options = botOptions.Value;

            _random = new Random();

            // Create discord client
            _discord = new DiscordClient(
                new DiscordConfiguration
                {
                    Token = _options.DiscordToken,
                    TokenType = TokenType.Bot,
                    LoggerFactory = loggerFactory,
                    Intents = DiscordIntents.DirectMessages | DiscordIntents.GuildMessages
                }
            );

            // Register event handler
            _discord.MessageCreated += HandleMessage;
        }

        private async Task HandleMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            try
            {
                // Don't reply to ourself
                if (e.Author.Equals(sender.CurrentUser))
                {
                    return;
                }
                
                // Check permissions
                if (!e.Channel.IsPrivate)
                {
                    // Get current member (user from current channel)
                    var currentMember = e.Channel.Users.FirstOrDefault(mbr => mbr.Equals(sender.CurrentUser));
                    
                    // Check for chat permissions
                    if (currentMember == null || !e.Channel.PermissionsFor(currentMember).HasPermission(Permissions.SendMessages))
                    {
                        return;
                    }
                }
                
                // Check for trigger message
                if (e.Message.Content.ToLower().Contains("bird!"))
                {
                    // Get random url
                    var picUrl = GetRandomImageUrl();
                    
                    // Send response
                    await e.Message.RespondAsync(
                    new DiscordEmbedBuilder()
                        .WithImageUrl(picUrl)
                        .Build()
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in message handler");
            }
        }

        private string GetRandomImageUrl()
        {
            var imageIdx = _random.Next(_options.ImageUrls.Count);
            return _options.ImageUrls[imageIdx];
        }
        
        public async Task StartAsync()
        {
            _logger.LogInformation($"BirdBot {GetType().Assembly.GetName().Version} starting");
            await _discord.ConnectAsync();
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("BirdBot stopping");
            await _discord.DisconnectAsync();
        }
    }
}