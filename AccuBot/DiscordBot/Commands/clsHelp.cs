﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace AccuBot.DiscordBot.Commands
{
    public class clsHelp : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsHelp()
        {
            MatchCommand = new []{"help"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            try
            {
               var helpText = clsCommands.Instance.GetHelpString();
               
               foreach (var text in helpText.SplitAfter(1500))
               {
                    e.Channel.SendMessageAsync($"```{text}```");
               }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
        }
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol("help");
        }        
        
    }
}
