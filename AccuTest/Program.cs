﻿// See https://aka.ms/new-console-template for more information


using AccuTest;
using Grpc.Net.Client;
using NotificationPolicy = AccuBotCommon.Proto.NotificationPolicy;

namespace AccuBot
{
    public class Program
    {
        static public AccuBotCommon.AccuBotClient GRPCClient;
        
        public static void Main(string[] args)
        {
            GRPCClient = new AccuBotCommon.AccuBotClient("https://localhost:5001");
            GRPCClient.Connect();
            Users.Test();
            Networks.Test();
            NotificationPolicys.Test();
        }
    }
}

