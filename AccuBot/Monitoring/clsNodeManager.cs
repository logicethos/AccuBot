using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Org.BouncyCastle.Crypto.Tls;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNodeManager : AManager<clsNode, Proto.API.Node,Proto.API.NodeList>
{
   
    public clsNodeManager(): base(Path.Combine(Program.DataPath, "nodes"),
        x=>x.Nodes,
        x=>x.NodeGroupID,
        (x, y) => x.NodeID = y)
    {
        
        base.MapFields = new Action<Proto.API.Node, Proto.API.Node>((origMessage, newMessage) =>
        {
            origMessage.Name = newMessage.Name;
            origMessage.Host = newMessage.Host;
            origMessage.Monitor = newMessage.Monitor;
            origMessage.NodeGroupID = newMessage.NodeGroupID;
        });
        
        
    }

    

    public void Load()
    {
        base.Load(new Func<NodeList>(() =>
        {
            var NodeListProto = new Proto.API.NodeList();
            NodeListProto.Nodes.Add(new Node()
            {
                NodeGroupID = 1,
                Name = "NY Node",
                Host = "100.23.123.12",
                Monitor = false,
            });
            NodeListProto.Nodes.Add(new Node()
            {
                NodeID = 2,
                NodeGroupID = 1,
                Name = "London Node",
                Host = "10.3.44.88",
                Monitor = false,
            });
            NodeListProto.Nodes.Add(new Node()
            {
                NodeGroupID = 2,
                Name = "Frankfurt Node",
                Host = "155.22.14.184",
                Monitor = false,
            });
            return NodeListProto;
        }));
    }
    
}