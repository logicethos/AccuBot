using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AccuBotClient;

namespace AccuBotClient
{
    public class Program
    {
        public static AccuBotCommon.AccuBotClient GRPCClient;

        public static async Task Main(string[] args)
        {
            GRPCClient = new AccuBotCommon.AccuBotClient("https://localhost:5001");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient
                { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}