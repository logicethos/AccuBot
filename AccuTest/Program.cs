// See https://aka.ms/new-console-template for more information


using AccuTest;
using Grpc.Net.Client;
using NotificationPolicy = AccuBotCommon.Proto.NotificationPolicy;

namespace AccuTest
{
    public class Program
    {
        static public AccuBotCommon.AccuBotClient GRPCClient;
        
        public static void Main(string[] args)
        {
            GRPCClient = new AccuBotCommon.AccuBotClient("http://localhost:5000");
            GRPCClient.Connect();
          //  Users.Test();
          //  Networks.Test();
          //  NotificationPolicys.Test();
            Nodes.Test();
            NodeGroups.Test();
        }
    }
}


