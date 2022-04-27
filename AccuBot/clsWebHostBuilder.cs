using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Serilog;

namespace AccuBot;

public class clsWebHostBuilder :  WebHostBuilder
{

public clsWebHostBuilder()
{
    this.UseSerilog();
    this.UseUrls("https://0.0.0.0:443");
    this.UseKestrel(k =>
            {
                var appServices = k.ApplicationServices;
                k.ConfigureHttpsDefaults(h =>
                {
                    h.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    h.UseLettuceEncrypt(appServices);
                });
            });
    }
    
    
}