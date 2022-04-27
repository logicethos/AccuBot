using System.Diagnostics;
using AccuTest;
using Proto.API;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;


namespace AccuTest;


static public class Dashboard
{
    public static async Task Test()
    {
        
        int count = 0;
        using (var response = Program.GRPCClient.API.NetworkStatusStream(new StreamRequest() { Milliseconds = 500 }, Program.GRPCClient.Headers))
        {
            Thread.Sleep(500);
            
            var clientCancelled = new CancellationTokenSource();
           // var abc = response.GetStatus();
           Console.WriteLine("Stream Start (Network)");
            var sw = new Stopwatch();
            sw.Start();
            while (await response.ResponseStream.MoveNext(CancellationToken.None))
            {
                Console.WriteLine($"{response.ResponseStream.Current.NetworkID} Height: {response.ResponseStream.Current.Height} AverageTime: {response.ResponseStream.Current.AverageTime} sw: {sw.Elapsed.Milliseconds}");
                
                if (++count == 10) break;
            }
            sw.Stop();
            Console.WriteLine("Stream Stop (Network)");
        }

        count = 0;
        using (var response2 = Program.GRPCClient.API.NodeStatusStream(new StreamRequest() { Milliseconds = 500 }, Program.GRPCClient.Headers))
        {
            Console.WriteLine("Stream Start (Node)");
            var sw = new Stopwatch();
            sw.Start();
            while (await response2.ResponseStream.MoveNext())
            {
                Console.WriteLine($"{response2.ResponseStream.Current.NodeID}   Height: {response2.ResponseStream.Current.Height} Ping: {response2.ResponseStream.Current.Ping} Version: {response2.ResponseStream.Current.Version} NodeID: {response2.ResponseStream.Current.NodeID} sw: {sw.Elapsed.Milliseconds}");
                if (++count == 10) break;
            }
            sw.Stop();
            Console.WriteLine("Stream Stop (Node)");
            response2.Dispose();
        }
    }
    
}