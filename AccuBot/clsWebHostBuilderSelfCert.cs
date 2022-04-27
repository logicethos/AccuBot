using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Serilog;

namespace AccuBot;

public class clsWebHostBuilderSelfCert :  WebHostBuilder
{

public clsWebHostBuilderSelfCert(int port)
{
        if (port == 0) port = 5001;
        this.UseSerilog();
        this.UseKestrel(k =>
        {
            var appServices = k.ApplicationServices;

            k.ConfigureHttpsDefaults(h =>
            {
                h.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                h.ClientCertificateValidation = (certificate, chain, errors) =>
                {
                    return true; //certificate.Issuer == serverCert.Issuer;
                };

            });
            k.Listen(IPAddress.Any, port,
                options => { options.UseHttps(SelfSignedCertificate.GetSelfSignedCertificate()); });
        });
        
    }
    
}