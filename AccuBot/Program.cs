using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AccuBotCommon.Proto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace AccuBot
{
    public class Program
    {
        public static AccuBotCommon.Proto.Settings Settings;
        public static string DataPath = "data";
        static public DateTime AppStarted = DateTime.UtcNow;
        static public clsBotClient Bot;
        
        public static void Main(string[] args)
        {
            Directory.CreateDirectory(DataPath);
            if (File.Exists(Path.Combine(DataPath,"settings")))
            {  //Read from file
                Settings = Settings.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(DataPath, "settings.dat")));
            }
            else
            {
                //Fill with defaults
                Settings = new Settings()
                {
                    BotName = "My Bot",
                    DiscordClientID = "955503128705392660",
                    DiscordToken = "OTU1NTAzMTI4NzA1MzkyNjYw.Yjinog.ltCgJ9c_-SXlLEDPR7khzTSJBa0",
                    AccumulateOperatorAlertsCh = 443025488655417364,
                    DiscordAlertsChannel = "#Bot-Alerts",
                    SIPUsername = "",
                    SIPPassword = "",
                    SIPHost = "",
                    SIPCallingNumber = "",
                    TwimletURL = "",
                    AlarmOffWarningMinutes = 30,
                    LatencyTriggerMultiplier = 2,
                    BotCommandPrefix = "!",
                    EmailSMTPHost = "",
                    EmailSMTPPort = 587,
                    EmailUsername = "",
                    EmailPassword = "",
                    EmailFromAddress = "",
                };
            }

            
            const int apiTimeout = 2000;
            const int loopWait = 3000;
            Settings.BotName ="TFA-AccBot";
            Settings.DiscordClientID = "957652292532834394";
            Settings.DiscordToken = "OTU3NjUyMjkyNTMyODM0Mzk0.YkB5Mg.wg0XltGjBinRbRwMyrQpMnITLTg";
            //https://discord.com/oauth2/authorize?client_id=957652292532834394&scope=bot&permissions=517544008768
            
            using (Bot = new clsBotClient(Settings.DiscordToken))
                {
                    Bot.RunAsync();
                    /*if (log.Contains("rror:")) Bot.SendAlert($"```{log}```");
                    
                    while (RunState == enumRunState.Run)
                    {
                        //Query every node.
                        foreach (var node in Program.NodesList.Values.Where(x=>x.Monitor))
                        {
                               node.GetHeightAsync(apiTimeout); 
                        }
    
                        ApplicationHold.WaitOne(apiTimeout);  //Wait for the timeout
                        
                        foreach (var group in NodeGroupList.Values)
                        {
                            group.Monitor();
                        }
                        
                        foreach(var network in NetworkList.Values)
                        {
                            network.CheckStall();
                        }
                        
                        if (AlarmStateTimeout.HasValue)
                        {
                            if (AlarmStateTimeout.Value < DateTime.UtcNow) AlarmState = EnumAlarmState.On;
                        }
                        else if (AlarmState == EnumAlarmState.Off && (DateTime.UtcNow - AlarmOffTime).TotalMinutes > (AlarmOffWarningMinutes*(AlarmOffWarningMultiplier+1))) 
                        {
                            Bot.Our_BotAlert.SendMessageAsync($"Warning, the Alarm has been off {(DateTime.UtcNow - AlarmOffTime).TotalMinutes:0} minutes.  Forget to reset it?");
                            AlarmOffWarningMultiplier++;
                        }
                        
                       AlarmManager.Process();

                        //Check the status of the Discord connection.  If it disconnects, it doesn't always restart.
                        if (!Bot.LastHeartbeatd.HasValue || (DateTime.UtcNow - Bot.LastHeartbeatd.Value).TotalSeconds > 90)
                        {
                            if ((DateTime.UtcNow - LastBotStart).TotalSeconds > 120)
                            {
                                Console.WriteLine("Discord not connected. Restarting.....");
                                BotErrorCounter++;
                                if (BotErrorCounter == 2) clsEmail.EmailAlertList("Bot: Discord not connected.");
                                Bot._client.Dispose();
                                LastBotStart = DateTime.UtcNow;
                                Bot = new clsBotClient(DiscordToken);
                                Bot.RunAsync();
                            }
                        }
                        else
                        {
                            BotErrorCounter = 0;
                        }

                        clsSSLCertMonitor.HeartbeatCheck();
                          
                       ApplicationHold.WaitOne(loopWait);*/
                    
                    CreateHostBuilder(args).Build().Run();
                    
                    }

       //   var api = new ApiService();
          
       
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => 
                    
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        // Setup a HTTP/2 endpoint without TLS.
              //          options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http2);
                        
                    });
                    webBuilder.UseStartup<Startup>();
                        
                        /*
                        .ConfigureKestrel(kestrelServerOptions => {
                        kestrelServerOptions.ConfigureHttpsDefaults(opt =>
                        {
                            opt.ClientCertificateMode = ClientCertificateMode.AllowCertificate;

                            // Verify that client certificate was issued by same CA as server certificate
                            opt.ClientCertificateValidation = (certificate, chain, errors) =>
                            {
                                return true; //certificate.Issuer == serverCert.Issuer;
                            };
                        });
                    
                    }); */
                });
    }
}