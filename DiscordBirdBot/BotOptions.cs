using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DiscordBirdBot
{
    public class BotOptions
    {
        [Required]
        [NotNull]
        public string DiscordToken { get; set; }
        
        [Required]
        [NotNull]
        [MinLength(1)]
        public List<string> ImageUrls { get; set; }
    }
}