using System;
using System.Text.RegularExpressions;
using Discord.WebSocket;


namespace AccuBot.DiscordBot.Commands
{
    
    public interface IBotCommand
    {
        String[] MatchCommand {get;}
        String[] MatchSubstring {get;}
        Regex[] MatchRegex {get;}
    
        void Run(SocketMessage e);
        
        void HelpString (ref clsColumnDisplay columnDisplay);
        
    }
}
