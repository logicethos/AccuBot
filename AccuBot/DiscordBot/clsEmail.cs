using System;
using System.Net;
using DSharpPlus.EventArgs;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Proto.API;

namespace AccuBot
{
    public class clsEmail
    {
      
        public clsEmail()
        {
        }
        
        
        static public void SendEmail(String To, String Subject, String Message,DSharpPlus.Entities.DiscordChannel ChBotAlert = null)
        {
           
           if (String.IsNullOrEmpty(Program.Settings.EmailSMTPHost))
           {
                if (ChBotAlert!=null) ChBotAlert.SendMessageAsync($"No SMTP host set up");
                return;
           }
        
           try
           {
               using (var client = new SmtpClient(Program.Settings.EmailSMTPHost, (int)Program.Settings.EmailSMTPPort))
               {

                   try
                   {
                       MailAddress from = new MailAddress(Program.Settings.EmailFromAddress, Program.Settings.BotName, System.Text.Encoding.UTF8);
                       MailAddress to = new MailAddress(To);
                       // Specify the message content.
                       MailMessage message = new MailMessage(from, to)
                       {
                           Body = Message,
                           BodyEncoding = System.Text.Encoding.UTF8,
                           Subject = Subject,
                           SubjectEncoding = System.Text.Encoding.UTF8
                       };

                       client.SendCompleted += (sender, args) => { };
                       client.SendAsync(message, "Sent");

                   }
                   catch (Exception ex)
                   {
                       if (ChBotAlert != null)
                       {
                           ChBotAlert.SendMessageAsync($"FAILED: e-mail {To}");
                           ChBotAlert.SendMessageAsync($"```{ex.Message}```");
                       }

                       Console.WriteLine(ex.Message);
                   }
               }
           }
           catch (Exception ex)
           {
                if (ChBotAlert!=null) ChBotAlert.SendMessageAsync($"Send e-mail error {ex.Message}");
           }
            
        }


        static public void EmailAlertList(String message = "")
        {
        
            /*if (!String.IsNullOrEmpty(Program.Settings.EmailSMTPHost))
            {
                string alarmMessage = $"{Program.Settings.BotName} Alarm {message}";
                foreach (var user in Program.UserList.Values.Where(x=>x.OnDuty && !String.IsNullOrEmpty(x.email)))
                {
                    SendEmail(user.email,alarmMessage,alarmMessage);
                }
            }*/
        }
        
        
        static public void email(String names, DSharpPlus.Entities.DiscordChannel ChBotAlert = null)
        {
        
            /*
            if (String.IsNullOrEmpty(Program.Settings.EmailSMTPHost))
            {
                if (ChBotAlert!=null) ChBotAlert.SendMessageAsync($"No SMTP host set up");
                return;
            }
        
            string alarmMessage = $"{Program.Settings.BotName} Alarm (manual Discord request)";
        
            foreach (var nameItem in names.Split(new char []{' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                var name = nameItem.ToLower();
                if (!name.EndsWith("mail")) 
                {
                    clsUser user;
                    if (!Program.UserList.TryGetValue(name,out user))
                    {
                      user = Program.UserList.Values.FirstOrDefault(x=>x.DiscordName.ToLower()==name || x.Name.ToLower()==name);
                    }
                    
                    if (user!=null) 
                        SendEmail(user.email,alarmMessage,alarmMessage,ChBotAlert);
                    else if (ChBotAlert!=null)
                       ChBotAlert.SendMessageAsync("name not found!");
                }
            }
            */
            
        }
        
        static public void email(MessageCreateEventArgs e)
        {
            var toRing = e.Message.Content.ToLower();
            email(toRing,e.Channel);
        }
        
        
    }
}


