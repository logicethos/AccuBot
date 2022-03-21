using System;
using System.Text.RegularExpressions;


namespace AccuBot.DiscordBot.Commands
{
    
    public interface IBotCommand
    {
        String[] MatchCommand {get;}
        String[] MatchSubstring {get;}
        Regex[] MatchRegex {get;}

    
       // void Run(MessageCreateEventArgs e);
        
        void HelpString (ref clsColumnDisplay columnDisplay);
        
    }
}
