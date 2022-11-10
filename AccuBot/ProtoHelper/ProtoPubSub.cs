using System.Threading.Tasks.Dataflow;
using Docker.DotNet.Models;
using Google.Protobuf;
using Grpc.Core;

namespace ProtoHelper;

public class ProtoPubSub<T>  where T:IMessage
{
    
    private List<BufferBlock<T>> BufferList = new List<BufferBlock<T>>();
    
    public ProtoPubSub()
    {
        
    }

    private void Subscribe(BufferBlock<T> buffer)
    {
        BufferList.Add(buffer);
    }

    private void Unsubscribe(BufferBlock<T> buffer)
    {
        BufferList.Remove(buffer);
    }
    
    public void Update(T message)
    {
        foreach (var buffer in BufferList)
        {
            buffer.Post(message);
        }
    }

    public async Task NextMessageWait(IServerStreamWriter<T> serverStreamWriter, ServerCallContext context, int millisecondDelay = 50)
    {
        BufferBlock<T> buffer = new BufferBlock<T>();
        try
        {
            Subscribe(buffer);
            var hashList = new HashSet<T>();
            while (!context?.CancellationToken.IsCancellationRequested ?? false)
            {
                T message = await buffer.ReceiveAsync();
                await Task.Delay(millisecondDelay);
                await serverStreamWriter.WriteAsync(message);

                while (buffer.Count > 0)
                {
                    hashList.Add(message);  //Add the message we sent to buffer list
                    while (buffer.Count > 0)
                    {
                        message = await buffer.ReceiveAsync();
                        if (hashList.Add(message))
                        {
                            await serverStreamWriter.WriteAsync(message);
                        }
                    }
                    hashList.Clear();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            Unsubscribe(buffer);
        }
    }
    
    
}