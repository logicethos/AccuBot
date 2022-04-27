using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Serilog;

namespace AccuBot;

public class clsWebHostBuilderSelfCert :  WebHostBuilder
{

public clsWebHostBuilderSelfCert()
    {
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
            k.Listen(IPAddress.Any, 5001,
                options => { options.UseHttps(SelfSignedCertificate.GetSelfSignedCertificate()); });
        });
        
    }
    
}