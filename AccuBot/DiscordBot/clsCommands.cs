using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AccuBot.DiscordBot.Commands;
using AccuTest;
using Discord.WebSocket;

namespace AccuBot.DiscordBot
{
    public class clsCommands
    {
        List <(string,IBotCommand)> MatchCommand = new List<(string, IBotCommand)>();
        List <(string,IBotCommand)> MatchSubstring = new List<(string, IBotCommand)>();
        List <(Regex,IBotCommand)> MatchRegex = new List<(Regex, IBotCommand)>();
    
        public static clsCommands Instance;

    
        public clsCommands()
        {
            Instance = this;
        }
        
        
        public void DiscordMessage(SocketMessage e)
        {
        
            String Message;
            try
            {
                if (String.IsNullOrEmpty(e.Content)) return;
                
                if (String.IsNullOrEmpty(Program.Settings.BotCommandPrefix))
                {
                    Message = e.Content;
                }
                else if (e.Content.StartsWith(Program.Settings.BotCommandPrefix) &&
                        (e.Content.Length > Program.Settings.BotCommandPrefix.Length))
                {
                    Message = e.Content.Substring(Program.Settings.BotCommandPrefix.Length);
                }
                else
                {
                    return;
                }
                
                var lowMessage = Message.ToLower();
                var firstword = lowMessage.Split(new []{' '},2,StringSplitOptions.RemoveEmptyEntries);            
                
                foreach (var command in MatchCommand.Where(x =>firstword[0] == x.Item1))
                {
                    command.Item2.Run(e);
                }
                
                foreach (var command in MatchSubstring.Where(x => lowMessage.Contains(x.Item1)))
                {
                    command.Item2.Run(e);
                }
                
                foreach (var command in MatchRegex.Where(x =>x.Item1.IsMatch(Message)))
                {
                    command.Item2.Run(e);
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"DiscordMessage Error: {ex.Message}");
            }
        }
        
        public void LoadCommandClasses()
        {
            var type = typeof(IBotCommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)&& !p.IsInterface);
                
            foreach (var commandType in types)
            {
               IBotCommand command = (IBotCommand)Activator.CreateInstance(commandType);
               
               if (command.MatchCommand!=null)
                    foreach (var match in command.MatchCommand) { MatchCommand.Add((match,command)); }
               if (command.MatchSubstring!=null)
                    foreach (var match in command.MatchSubstring) { MatchSubstring.Add((match,command)); }
               if (command.MatchRegex != null)     
                    foreach (var match in command.MatchRegex) { MatchRegex.Add((match,command)); }

            }    
        }

        public String GetHelpString(IBotCommand command = null)
        {
            try {
        
            var cd = new clsColumnDisplay();
            cd.ColumnChar=' ';
            cd.AppendLine($"The Factoid Authority Bot                          Uptime {(DateTime.UtcNow-Program.AppStarted).ToDHMDisplay() }"); 
            cd.AppendCol("Command");
            cd.AppendCol("Args");
            cd.AppendCol("Description");
             
            cd.AppendCharLine('-');
            
            if (command!=null)
            {
                command.HelpString(ref cd);
            }
            else
            {
                foreach (var commandItem in MatchCommand.DistinctBy(x=>x.Item2).OrderBy(x=>x.Item1))
                {
                    commandItem.Item2.HelpString(ref cd);
                    cd.NewLine();
                }
            }
            //cd.Append(Program.BotURL);
            return cd.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
