using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Proto.API;
using Proto.Authentication;
using Grpc.Core;
using Grpc.Net.Client;

namespace AccuBotCommon
{
    public class AccuBotClient : IDisposable
    {
        private GrpcChannel Channel;
        private String HostURL;
        private GrpcChannelOptions Options;
        public AccuBotAPI.AccuBotAPIClient API { get; private set; }
        public Metadata Headers { get; private set; }
        
        public AccuBotClient(string host = "https://localhost:5001", GrpcChannelOptions options = null)
        {
            if (options == null)
            {
                options = new GrpcChannelOptions();

                if (host.Contains("localhost") || host.Contains("127.0.0."))  //Accept self
                {
                    options.HttpHandler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                }
            }

            HostURL = host;
            Options = options;
        }

        public Metadata Connect()
        {
            Channel = GrpcChannel.ForAddress(HostURL,Options);
            API =  new AccuBotAPI.AccuBotAPIClient(Channel);
            var authenticationClient = new Proto.Authentication.AccuBotAuthentication.AccuBotAuthenticationClient(Channel);
            var authenticationReply = authenticationClient.Authenticate(new AuthenticationRequest()
            {
                Username = "admin",
                Password = "admin"
            });

            Console.WriteLine($"Token: {authenticationReply.AccessToken}");
            
            Headers = new Metadata();
            Headers.Add("Authorization",$"Bearer {authenticationReply.AccessToken}");

            return Headers;
        }

        public void Dispose()
        {
            
            Channel.ShutdownAsync().Wait();
            Channel.Dispose();
        }
        
    }
}