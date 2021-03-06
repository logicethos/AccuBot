using AccuTest;
using Proto.API;
using Google.Protobuf.WellKnownTypes;

    
namespace AccuTest;


static public class Networks
{
    public static void Test()
    {

        GetNetworks();
        var newNetwork = AddNetwork();
        GetNetworks();
        var reply2 = Program.GRPCClient.API.NetworkDelete(new ID32(){ID = newNetwork.NetworkID},Program.GRPCClient.Headers);
        GetNetworks();
    }
    
    public static void GetNetworks()
    {
        //*** Get Networks
        Console.WriteLine("Get Networks:");
        var networks = Program.GRPCClient.API.NetworkListGet(new Empty(),Program.GRPCClient.Headers);

        //Display Networks
        foreach (var network in networks.Network)
        {
            Console.WriteLine($"Name: {network.Name}");
        }
    }
    
    public static Proto.API.Network AddNetwork()
    {
        //*** Add a user
        Console.WriteLine("Add Network:");
        var newNetwork = new Proto.API.Network
        {
//            NetworkID = 0,
            Name = "My new Network",
            BlockTime = 0,
            StalledAfter = 10,
            NotifictionID = 1,
        };
        var reply = Program.GRPCClient.API.NetworkSet(newNetwork,Program.GRPCClient.Headers);

        if (reply.Status == MsgReply.Types.Status.Ok)
        {
            newNetwork.NetworkID = reply.NewID32; //The server returns the new ID.
            Console.WriteLine($"new network id : {newNetwork.NetworkID}",Program.GRPCClient.Headers);
            return newNetwork;
        }
        else
        {
            Console.WriteLine(reply.Message); //Error
            return null;
        }
    }
}