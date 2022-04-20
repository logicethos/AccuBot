// See https://aka.ms/new-console-template for more information


using System.Linq.Expressions;
using AccuTest;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Proto.API;

namespace AccuTest
{
    public class Program
    {
        static public AccuBotCommon.AccuBotClient GRPCClient;

       
        public static async Task Main(string[] args)
        {
            //GRPCClient = new AccuBotCommon.AccuBotClient("https://localhost:5001");
            GRPCClient = new AccuBotCommon.AccuBotClient("https://red2.logicethos.com");
            var headers = GRPCClient.Connect();

          //  Networks.Test();
         //   await Dashboard.Test();
            
          //  return;
            
           // Users.Test();
           //  Networks.Test();
           //   NotificationPolicys.Test();
              Nodes.Test();
           //   NodeGroups.Test();
           //   Settings.Test();
           Thread.Sleep(10000);
        }
    }

    class TestClass
    {

    }



   

}