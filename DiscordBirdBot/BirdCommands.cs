using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBirdBot;

// ReSharper disable once ClassNeverInstantiated.Global
public class BirdCommands : ApplicationCommandModule
{
    private readonly IOptions<BirdCommandOptions> _options;
    private readonly ILogger<BirdCommands> _logger;
    private readonly Random _random = new();

    public BirdCommands(IOptions<BirdCommandOptions> options, ILogger<BirdCommands> logger)
    {
        _logger = logger;
        _options = options;
    }

    [SlashCommand("bird", "Tweet tweet!")]
    public async Task BirdCommand(InteractionContext ctx)
    {
        using var _ = _logger.BeginScope(nameof(BirdCommand));
        try
        {
            _logger.LogDebug("Invoked by [{user}] in [{guild}]", ctx.User, ctx.Guild);

            // Get random url
            var picUrl = GetRandomImageUrl();
            
            // Var respond to user
            var imageEmbed = new DiscordEmbedBuilder().WithImageUrl(picUrl).Build();
            var response = new DiscordInteractionResponseBuilder().AddEmbed(imageEmbed);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in message handler");
        }
    }

    private string GetRandomImageUrl()
    {
        var urls = _options.Value.ImageUrls;
        var imageIdx = _random.Next(urls.Count);
        return urls[imageIdx];
    }
}

public class BirdCommandOptions
{
    [Required]
    [MinLength(1)]
    public List<string> ImageUrls { get; init; } = new();
}