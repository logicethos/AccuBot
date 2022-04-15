using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accubot;
using AccuBot.Monitoring;
using DSharpPlus.Entities;

namespace AccuBot
{
    public class clsAlarm
    {
        DateTime Opened = DateTime.UtcNow;
        DateTime? TimeDiscord;
        DateTime? TimeCall;
        DateTime? DelayUntil;
        public clsNotificationPolicy notificationPolicy;
        
        public enumAlarmType AlarmType {get; private set;}
        public enum enumAlarmType
        {
            Error,
            Syncing,
            NoResponse,
            Height,
            Latency,
            Network
        };
        
        public clsNode Node {get; private set;}
        public clsNetwork Network {get; private set;}
        public String Message;
        
        List<string> Notes = new List<string>();
       
        //New alarm, from node
        public clsAlarm(enumAlarmType alarmType, String message, clsNode node)
        {
            AlarmType = alarmType;
            Message = message;
            Node = node;
                    
            switch(alarmType)
            {
                case enumAlarmType.NoResponse:
                    Program.NotificationPolicyManager.NotificationPolicyList.TryGetValue(Node.NodeGroup.ProtoMessage.PingNotifictionID,out notificationPolicy); break;
                case enumAlarmType.Height:
                    Program.NotificationPolicyManager.NotificationPolicyList.TryGetValue(Node.NodeGroup.ProtoMessage.HeightNotifictionID,out notificationPolicy); break;
                case enumAlarmType.Latency:
                    Program.NotificationPolicyManager.NotificationPolicyList.TryGetValue(Node.NodeGroup.ProtoMessage.LatencyNotifictionID,out notificationPolicy); break;
                case enumAlarmType.Error:
                case enumAlarmType.Network:
                    Program.NotificationPolicyManager.NotificationPolicyList.TryGetValue(Node.NodeGroup.ProtoMessage.NetworkID,out notificationPolicy); break;
            }
        }
        
        //New alarm
        public clsAlarm(enumAlarmType alarmType, String message, TimeSpan delay)
        {
                AlarmType = alarmType;
                Message = message;
                DelayUntil = DateTime.UtcNow.Add(delay);
        }
        
        
        //New alarm from Network
        public clsAlarm(enumAlarmType alarmType, String message, clsNetwork network)
        {
            AlarmType = alarmType;
            Message = message;
            Network = network;
                     
          
        //   Program.NotificationPolicyList.TryGetValue(network.StallNotification,out notificationPolicy);
           
          // Program.notificationPolicyList.NotificationPolicyList_.FirstOrDefault(x=>x.)
           
        }
        
        public void Clear(string message = null)
        {
            if (TimeDiscord.HasValue) //Alarm message has been displayed, so allow clear to be displayed
            {
                var sb = new StringBuilder();
                if (String.IsNullOrEmpty(message))
                    sb.AppendLine($"{AlarmType} alarm cleared");
                else
                    sb.AppendLine(message);
                    
                foreach( var line in Notes)
                {
                    sb.AppendLine(line);
                }
                Notes.Clear();
                
                clsBotClient.Instance.Our_BotAlert.SendMessageAsync(sb.ToString());
            }
        }
        
        public void Process()
        {
            if (notificationPolicy!=null)
            {   
                //Do we need to make a call?         
                if (Program.AlarmState == Program.EnumAlarmState.On)
                {
                    if (!TimeCall.HasValue && notificationPolicy.ProtoMessage.Call>=0 && DateTime.UtcNow > Opened.AddSeconds(notificationPolicy.ProtoMessage.Call))
                    {
                        TimeCall = DateTime.UtcNow;
                   //     clsDialler.CallAlertList();
                        clsEmail.EmailAlertList();
                    }
                }
                
                //Do we need to send Discord a message? 
                if (Program.AlarmState != Program.EnumAlarmState.Off)
                {
                    if (!TimeDiscord.HasValue && notificationPolicy.ProtoMessage.Discord>=0 && DateTime.UtcNow > Opened.AddSeconds(notificationPolicy.ProtoMessage.Discord))
                    {
                        TimeDiscord = DateTime.UtcNow;
                        var sb = new StringBuilder();
                        sb.AppendLine(Message);
                        foreach( var line in Notes)
                        {
                            sb.AppendLine(line);
                        }
                        Notes.Clear();
                        clsBotClient.Instance.Our_BotAlert.SendMessageAsync(sb.ToString());
                    }
                }
            }
            else
            {
                if (!TimeDiscord.HasValue)
                {
                    if (!DelayUntil.HasValue || DelayUntil.Value < DateTime.UtcNow)
                    {
                        if (Program.Bot.Our_BotAlert == null)
                            AddNote(Message);
                        else
                            Program.Bot.Our_BotAlert.SendMessageAsync(Message);
                        
                        TimeDiscord = DateTime.UtcNow;
                    }
                }
            }
        }
        
        public new String ToString()
        {
            return $"{AlarmType.ToString().PadRight(15)} {Node?.ProtoMessage.Name.PadRight(15) ?? Network?.ProtoMessage.Name.PadRight(20) ?? ""} {Opened} {Message}";
        }

        /// <summary>
        /// Add a note to an alarm.  If alarm not yet announced, then save for later
        /// </summary>
        /// <param name="text">Text.</param>
        internal void AddNote(String text)
        {        
            if (TimeDiscord.HasValue)
            {
                clsBotClient.Instance.Our_BotAlert.SendMessageAsync(text);
            }
            else
            {
                Notes.Add(text);
            }
        }
    }
}
