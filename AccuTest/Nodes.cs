using AccuTest;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace AccuTest;


static public class Nodes
{
    public static void Test()
    {

        GetNodes();
        var newNode = AddNode();
        GetNodes();
        
        //Delete the node we added.
        Console.WriteLine("Delete Node:");
        var reply2 = Program.GRPCClient.API.NodeDelete(new ID32() { ID = newNode.NodeID});
        
        GetNodes();
        
    }

    public static void GetNodes()
    {
        //*** Get Nodes
        Console.WriteLine("Get Nodes:");
        var Nodes = Program.GRPCClient.API.NodeListGet(new Empty());

        //Display Nodes
        foreach (var Node in Nodes.Nodes)
        {
            Console.WriteLine($"Name: {Node.Name}  email: {Node.Host}");
        }
    }

    public static AccuBotCommon.Proto.Node AddNode()
    {
        //*** Add a node
        Console.WriteLine("Add Node:");
        var newNode = new AccuBotCommon.Proto.Node
        {
            NodeID = 0, //Set as zero when adding.  Or the original NodeID when updating
            NodeGroupID = 1, 
            Name = "London Node",
            Host = "123.34.321.1",
            Monitor = true,
        };
        var reply = Program.GRPCClient.API.NodeSet(newNode);

        if (reply.Status == MsgReply.Types.Status.Ok)
        {
            newNode.NodeID = reply.NewID32; //The server returns the new ID.
            Console.WriteLine($"new Node id : {newNode.NodeID}");
            return newNode;
        }
        else
        {
            Console.WriteLine(reply.Message); //Error
            return null;
        }
    }
    
}