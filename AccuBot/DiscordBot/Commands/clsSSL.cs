using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace AccuBot.DiscordBot.Commands
{
    public class clsSSL : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsSSL()
        {
            MatchCommand = new []{"ssl"};
        }

        void IBotCommand.Run(MessageCreateEventArgs e)
        {

            try
            {

                if (e.Message.Content == "ssl")
                {
                    var cd = new clsColumnDisplay();
                    cd.ColumnChar = ' ';
                    cd.Append("```");
                    cd.AppendLine($"SSL Certificates checked {clsSSLCertMonitor.LastTest.Value:yyyy-MM-dd HH:mm}");
                    cd.AppendCol("URL");
                    cd.AppendCol("Days");
                    cd.AppendCharLine('-');

                    foreach (var ssl in Program.SSLCertsList.Values)
                    {
                        cd.AppendCol(ssl.URL);
                        if (ssl.CertExpiry.HasValue)
                            cd.AppendCol((ssl.CertExpiry - DateTime.UtcNow).Value.Days.ToString());
                        else
                            cd.AppendCol("?");
                        if (!ssl.Monitor) cd.AppendCol("MONITOR OFF");
                        cd.NewLine();
                    }
                    cd.Append("```");
                    e.Channel.SendMessageAsync(cd.ToString());

                }

                else if (e.Message.Content == "ssl update")
                {
                    clsSSLCertMonitor.HeartbeatCheck(true);
                }
                else if (e.Message.Content.Length > 4)
                {
                    var url = e.Message.Content.Substring(4).Trim();
                    if (!url.StartsWith("https://")) url = $"https://{url}";

                    HttpWebRequest request;
                    request = (HttpWebRequest)WebRequest.Create(url);
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        response.Close();
                    }
                    catch { }


                    var cert = request.ServicePoint.Certificate;
                    var cert2 = new X509Certificate2(cert);
                    var days = (DateTime.Parse(cert2.GetExpirationDateString()) - DateTime.UtcNow).Days;
                    e.Channel.SendMessageAsync($"{days} days");

                }
            }
            catch (Exception ex)
            {
                e.Channel.SendMessageAsync(ex.Message);
            }

        }

        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol("ssl","<https url>");
        }
    }
}
