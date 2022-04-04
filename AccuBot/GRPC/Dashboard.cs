using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using Proto.API;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;

namespace AccuBot.GRPC;

public partial class ApiService
{
    public override async Task NodeStatusStream(StreamRequest request, IServerStreamWriter<NodeStatus> responseStream, ServerCallContext context)
    {
        try
        {
            var random = new Random();
            UInt32 height = 1000000000;
            await Task.Delay(10);
            while (!context.CancellationToken.IsCancellationRequested)
            {
                height++;
                foreach (var node in Program.nodelist.Nodes)
                {
                    await responseStream.WriteAsync(new NodeStatus
                    {
                        NodeID = node.NodeID,
                        Version = "v1.1.2",
                        Height = height,
                        Ping = 5 * random.NextSingle()
                    });
                }
                await Task.Delay((int)request.Seconds * 1000, context.CancellationToken);
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Connection Closed");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
    }

    public override async Task NetworkStatusStream(StreamRequest request, IServerStreamWriter<NetworkStatus> responseStream, ServerCallContext context)
    {
        try
        {
            var random = new Random();
            UInt32 height = 1000000000;
            await Task.Delay(10);
            while (!context.CancellationToken.IsCancellationRequested)
            {
                height++;
                foreach (var network in Program.networkList.Network)
                {
                    await responseStream.WriteAsync(new NetworkStatus
                    {
                        NetworkID = network.NetworkID,
                        Height = height,
                        AverageTime = 1+(float)random.NextDouble()
                    });
                }
                await Task.Delay((int)request.Seconds * 1000, context.CancellationToken);
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Connection Closed");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}