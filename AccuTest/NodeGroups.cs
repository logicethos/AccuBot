using AccuTest;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace AccuTest;


    static public class NodeGroups
    {
        public static void Test()
        {
            GetNodeGroups();
            var newNodeGroup = AddNodeGroup();
            GetNodeGroups();

            //Delete the NodeGroup we added.
            Console.WriteLine("Delete NodeGroup:");
            var reply2 = Program.GRPCClient.API.NodeGroupDelete(new ID32() { ID = newNodeGroup.NodeGroupID });

            GetNodeGroups();
        }

        public static void GetNodeGroups()
        {
            //*** Get NodeGroups
            Console.WriteLine("Get NodeGroups:");
            var NodeGroups = Program.GRPCClient.API.NodeGroupListGet(new Empty());

            //Display NodeGroups
            foreach (var NodeGroup in NodeGroups.NodeGroup)
            {
                Console.WriteLine($"Name: {NodeGroup.Name}");
            }
        }

        public static AccuBotCommon.Proto.NodeGroup AddNodeGroup()
        {
            //*** Add a NodeGroup
            Console.WriteLine("Add NodeGroup:");
            var newNodeGroup = new AccuBotCommon.Proto.NodeGroup
            {
                Name = "Devnet",
                NetworkID = 1,
                PingNotifictionID = 0,
                HeightNotifictionID = 0,
                LatencyNotifictionID = 0
            };
            var reply = Program.GRPCClient.API.NodeGroupSet(newNodeGroup);

            if (reply.Status == MsgReply.Types.Status.Ok)
            {
                newNodeGroup.NodeGroupID = reply.NewID32; //The server returns the new ID.
                Console.WriteLine($"new NodeGroup id : {newNodeGroup.NodeGroupID}");
                return newNodeGroup;
            }
            else
            {
                Console.WriteLine(reply.Message); //Error
                return null;
            }
        }
    }
