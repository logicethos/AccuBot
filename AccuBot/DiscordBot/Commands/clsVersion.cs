using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;
using AccuBot.Git;

namespace AccuBot.DiscordBot.Commands
{
    public class clsVersion : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsVersion()
        {
            MatchCommand = new []{"version"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("```");
            sb.AppendLine(clsGitHead.GetHeadToString());

            try
            {
                Type type = Type.GetType("Mono.Runtime");
                if (type != null)
                {
                    MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                    if (displayName != null) sb.AppendLine($"Mono Runtime: {displayName.Invoke(null, null)}");
                }
            }
            catch { } ;
            
            
            sb.Append("```");
            e.Channel.SendMessageAsync(sb.ToString());
        }
        
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol("version");
        }
    }
}
