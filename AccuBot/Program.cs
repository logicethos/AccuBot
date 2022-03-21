using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccuTest.GRPC;
using AccuBotCommon.Proto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace AccuTest
{
    public class Program
    {
        public static AccuBotCommon.Proto.Settings Settings;
        public static string DataPath = "data";
        static public DateTime AppStarted = DateTime.UtcNow;
        
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

            CreateHostBuilder(args).Build().Run();

            
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