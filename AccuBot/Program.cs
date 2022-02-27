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
        public static void Main(string[] args)
        {
            Directory.CreateDirectory("data");
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
                        //options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http2);
                    //    options.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http2);
                    });
                    webBuilder.UseStartup<Startup>()
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
                    });
                });
    }
}