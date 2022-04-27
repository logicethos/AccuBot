using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grpc.Core;
using Proto.API;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;
using Serilog;

namespace AccuBot.GRPC;

public partial class ApiService
{
    public override async Task NodeStatusStream(StreamRequest request, IServerStreamWriter<NodeStatus> responseStream, ServerCallContext context)
    {
        try
        {
            var random = new Random();
            UInt32 height = 10000000;
            Console.WriteLine("NodeStatusStream started");
            while (!context.CancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("NodeStatusStream next");
                height++;
                foreach (var node in Program.NodeProtoDictionaryShadow.ManagerList)
                {
                    await responseStream.WriteAsync(new NodeStatus
                    {
                        NodeID = node.Key,
                        Version = "v1.1.2",
                        Height = height,
                        Ping = 5 * random.NextSingle()
                    });
                    Log.Information($"{node.Key}");
                    await Task.Delay((int)request.Milliseconds / Program.NodeProtoDictionaryShadow.ManagerList.ProtoRepeatedField.Count, context.CancellationToken);
                }
            }
            Console.WriteLine("NodeStatusStream End");
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
            Console.WriteLine("NetworkStatusStream started");
            //context.CancellationToken.
            while (!context.CancellationToken.IsCancellationRequested)
            {
                height++;
                foreach (var network in Program.NetworkProtoDictionaryShadow.ManagerList)
                {
                    Console.WriteLine("Send");
                    await responseStream.WriteAsync(new NetworkStatus
                    {
                        NetworkID = network.Key,
                        Height = height,
                        AverageTime = 1+(float)random.NextDouble()
                    });
                    await Task.Delay((int)request.Milliseconds / Program.NetworkProtoDictionaryShadow.ManagerList.ProtoRepeatedField.Count, context.CancellationToken);
                }

            }
            Console.WriteLine($"NetworkStatusStream End CanRequest: {context.CancellationToken.IsCancellationRequested}");
            ;
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