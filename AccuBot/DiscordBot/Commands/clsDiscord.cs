using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace AccuBot.DiscordBot.Commands
{
    public class clsDiscord : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsDiscord()
        {
            MatchCommand = new [] {"discord"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            var sb = new StringBuilder();
            
            String alertChannelString = Program.Settings.DiscordAlertsChannel.ToLower().Replace("#","");
            
            sb.AppendLine($"Discord Gateway: {Program.Bot._client.GatewayUri} Version: {Program.Bot._client.GatewayVersion}");
            sb.AppendLine($"Discord Info: {Program.Bot._client.GatewayInfo}");
            sb.AppendLine($"                 {Program.Bot._client.CurrentUser}");
            sb.AppendLine($"                 Ping {Program.Bot._client.Ping}");
            
            
            sb.AppendLine();
            foreach (var discord in Program.Bot._client.Guilds.Values)
            {
                sb.AppendLine($"{discord.Name} {discord.Id}");
                sb.AppendLine($"   Channels:{discord.Channels.Count} Members:{discord.MemberCount} Owner:{discord.Owner.DisplayName}#{discord.Owner.Discriminator}");
                sb.AppendLine($"   Joined:{discord.JoinedAt:yyyy-MM-dd}");
                
                if (discord.Id != 419201548372017163)
                {
                    sb.Append($"   Alert Channel: '{alertChannelString}' ");
                    sb.AppendLine(discord.Channels.Any(x => x.Value.Name == alertChannelString) ? "" : "NOT FOUND");
                }
                sb.AppendLine();
            }
            e.Channel.SendMessageAsync($"```{sb.ToString()}```");
        }

        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
                columnDisplay.AppendCol("discord","","Display discord status");
        }
    }
}
