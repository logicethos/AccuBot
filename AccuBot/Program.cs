using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Accubot;
using AccuBot.Monitoring;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Serilog;

using Proto.API;
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
        public static Proto.API.Settings Settings;
        public static string DataPath { get; private set; } 
        public static string DataPathWWW { get; private set; }
        public static string[] DomainNames { get; private set; }
        private static int HttpsPort;
        
        static public DateTime AppStarted = DateTime.UtcNow;
        static public clsBotClient Bot;

        static public clsNetworkProtoDictionaryShadow NetworkProtoDictionaryShadow;
        static public clsNodeProtoDictionaryShadow NodeProtoDictionaryShadow;
        static public clsNotificationPolicyProtoDictionaryShadow NotificationPolicyProtoDictionaryShadow;
        static public clsNodeGroupProtoDictionaryShadow NodeGroupProtoDictionaryShadow;
        
        static public Proto.API.NetworkStatus networkStatus;
        static public DockerManager DockerManager;

        static public clsAlarmManager AlarmManager = new clsAlarmManager();
        
        static public ManualResetEvent ApplicationHold = new ManualResetEvent(false);
        
        static enumRunState RunState = enumRunState.Run;
        public enum enumRunState : int
        {
            Stop=0,
            Run=1,
            Restart=2,
            Update=3,
            PreviousVersion=4,
            MonoArgs=5,
            Error = 100
        }
        
        static public DateTime AlarmOffTime;
        static public uint AlarmOffWarningMultiplier;
        public enum EnumAlarmState { Off, On, Silent }
        static public  DateTime? AlarmStateTimeout;
        static public  EnumAlarmState _alarmState = EnumAlarmState.On;
        static public  EnumAlarmState AlarmState 
        {    get
            {
                return _alarmState;
            }
        
            private set
            {
                if (_alarmState != value)
                {
                    _alarmState = value;
                    if (value == EnumAlarmState.Off) //New state off
                    {
                        AlarmOffTime = DateTime.UtcNow;
                        AlarmOffWarningMultiplier=0;
                    }
                    else if (value == EnumAlarmState.On) //New state on
                    {
                        AlarmStateTimeout = null;
                    }
                }
            }
        }
        
        
        public static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
              //  .WriteTo.File("logfile.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            DataPath = Environment.GetEnvironmentVariable("ACCUBOT_DATA", EnvironmentVariableTarget.Process) ?? "data";
            DataPathWWW = Environment.GetEnvironmentVariable("ACCUBOT_WWW", EnvironmentVariableTarget.Process) ?? "www";
            var DomainName = Environment.GetEnvironmentVariable("ACCUBOT_DOMAIN", EnvironmentVariableTarget.Process);
            if (!String.IsNullOrEmpty(DomainName)) DomainNames = DomainName.Split(new[] { ' ', ',', ';' }, StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(Environment.GetEnvironmentVariable("ACCUBOT_HTTPS", EnvironmentVariableTarget.Process) ?? "0",out HttpsPort);
            
            
            clsSettings.Load();

            DockerManager = new DockerManager();
            

            try
            {
                NetworkProtoDictionaryShadow = new clsNetworkProtoDictionaryShadow();
                NodeGroupProtoDictionaryShadow = new clsNodeGroupProtoDictionaryShadow();
                NotificationPolicyProtoDictionaryShadow = new clsNotificationPolicyProtoDictionaryShadow();
                NodeProtoDictionaryShadow = new clsNodeProtoDictionaryShadow();
            }
            catch ( Exception ex)
            {
                Log.Fatal(ex,"Startup");                
            }


            const int apiTimeout = 2000;
            const int loopWait = 3000;
            Settings.BotName ="AccuBot";
           // Settings.DiscordClientID = "957652292532834394";
           // Settings.DiscordToken = "OTU3NjUyMjkyNTMyODM0Mzk0.YkB5Mg.wg0XltGjBinRbRwMyrQpMnITLTg";
            //https://discord.com/oauth2/authorize?client_id=957652292532834394&scope=bot&permissions=517544008768

            using (Bot = new clsBotClient(Settings.DiscordToken))
            {
                if (!String.IsNullOrEmpty(Settings.DiscordClientID) && !String.IsNullOrEmpty(Settings.DiscordToken)) Bot.RunAsync();
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
                  
                   */
                Log.Information($"Domain: {DomainName ?? "Not Set"}");
                Log.Information($"Port: {HttpsPort}");

                WebHostBuilder host;
                if (DomainNames?.Length > 0)
                {
                    host = new clsWebHostBuilder(HttpsPort);
                }
                else
                {
                    host = new clsWebHostBuilderSelfCert(HttpsPort);
                }

                host.UseStartup<Startup>();
                host.Build().Run();
                
                ApplicationHold.WaitOne(loopWait);
            }
       
        }
        
        static public void SendAlert(String message)
        {
            if (AlarmState == EnumAlarmState.On || AlarmState == EnumAlarmState.Silent)
                Bot.Our_BotAlert.SendMessageAsync(message);
        }
        
        static public void SetRunState(enumRunState runState)
        {
            RunState = runState;
            ApplicationHold.Set();
        }
        
    }
    
}