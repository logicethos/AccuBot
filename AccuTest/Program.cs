// See https://aka.ms/new-console-template for more information


using AccuTest;
using Grpc.Net.Client;

namespace AccuTest
{
    public class Program
    {
        static public AccuBotCommon.AccuBotClient GRPCClient;
        
        public static void Main(string[] args)
        {
            GRPCClient = new AccuBotCommon.AccuBotClient("https://localhost:5001");
            var headers = GRPCClient.Connect();
            
           //Users.Test();
           //Dashboard.Test();
           
            Networks.Test();
          //  NotificationPolicys.Test();
            Nodes.Test();
          //  NodeGroups.Test();
          //  Settings.Test();
        }
    }
}


