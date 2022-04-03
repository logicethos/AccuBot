using AccuTest;
using Proto.API;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;


namespace AccuTest;


static public class Dashboard
{
    public static async void Test()
    {
        
        int count = 0;
        using (var response = Program.GRPCClient.API.NetworkStatusStream(new StreamRequest() { Seconds = 1 }, Program.GRPCClient.Headers))
        {
            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine($"Height: {response.ResponseStream.Current.Height}");
                Console.WriteLine($"AverageTime: {response.ResponseStream.Current.AverageTime}");
                if (++count == 5) break;
            }
        }

        count = 0;
        var response2 = Program.GRPCClient.API.NodeStatusStream(new StreamRequest() { Seconds = 1 },Program.GRPCClient.Headers);
        while (await response2.ResponseStream.MoveNext())
        {
            Console.WriteLine($"Height: {response2.ResponseStream.Current.Height}");
            Console.WriteLine($"Ping: {response2.ResponseStream.Current.Ping}");
            Console.WriteLine($"Version: {response2.ResponseStream.Current.Version}");
            Console.WriteLine($"NodeID: {response2.ResponseStream.Current.NodeID}");
            if (++count == 5) break;
        }
        response2.Dispose();
        
    }
    
}