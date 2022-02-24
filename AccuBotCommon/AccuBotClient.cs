using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AccuBotCommon.Proto;
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
        
        public AccuBotClient(string host = "https://localhost:5001")
        {
            HostURL = host;
            Options = new GrpcChannelOptions();

        }

        public bool Connect()
        {
            Channel = GrpcChannel.ForAddress(HostURL,Options);
            API =  new AccuBotAPI.AccuBotAPIClient(Channel);
            return true;
        }

        public void Dispose()
        {
            
            Channel.ShutdownAsync().Wait();
            Channel.Dispose();
        }
        
    }
}