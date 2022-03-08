using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AccuBotClient;
using AccuBotCommon.Proto;
using Grpc.Net.Client;
using Grpc.Net.Client.Web; 
using Microsoft.AspNetCore.Components;

namespace AccuBotClient
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
          
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient
                    { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton(services => 
            { 
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler())); 
                //var baseUri2 = services.GetRequiredService<NavigationManager>().BaseUri;
                
                var baseUri = "https://localhost:5001/";
                var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
                {
                    HttpClient = httpClient,
                 //   HttpHandler = new HttpClientHandler()
                  //  {
                  //      ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                  //  }
                }); 
                return new AccuBotAPI.AccuBotAPIClient(channel);
            });

            await builder.Build().RunAsync();
        }
    }
}