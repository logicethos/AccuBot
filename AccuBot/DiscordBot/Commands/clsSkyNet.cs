﻿using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace AccuBot.DiscordBot.Commands
{
    public class clsSkynet : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsSkynet()
        {
            MatchCommand = new []{"skynet"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            e.Channel.SendMessageAsync("Skynet Activated");
        }
        
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
                return;
        }
    }
}
