using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Org.BouncyCastle.Crypto.Tls;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNodeManagar : IProtoManager<Proto.API.Node,Proto.API.NodeList>
{
    public clsProtoShadow<clsNode,Proto.API.Node> NodeList { get; init; }
    private readonly String DataFilePath = Path.Combine(Program.DataPath, "nodes");
    public Proto.API.NodeList ProtoWrapper { get; init; }
    
    
    public clsNodeManagar()
    {
        ProtoWrapper = new NodeList();
        NodeList = new clsProtoShadow<clsNode, Node>(ProtoWrapper.Nodes,x => x.NodeID, (x, y) => x.NodeID = y);
        Load();
    }


    public MsgReply Update(Proto.API.Node node)
    {
        MsgReply msgReply;
        clsNode existingNode;
        NodeList.TryGetValue(node.NodeID, out existingNode);
        if (existingNode == null) //Incorrect ID sent!
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node not found" };
        }
        else
        {
            existingNode.ProtoMessage.Name = node.Name;
            existingNode.ProtoMessage.Host = node.Host;
            existingNode.ProtoMessage.Monitor = node.Monitor;
            existingNode.ProtoMessage.NodeGroupID = node.NodeGroupID;

            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return msgReply;
    }
    
    public MsgReply Add(Node network)
    {
        MsgReply msgReply;
        try
        {
            var id=this.NodeList.Add(network);
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = id};
        }
        catch (Exception e)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail,Message = e.Message};
        }

        return msgReply;
    }
    
    public void Dispose()
    {
        
    }

    public MsgReply Delete(uint nodeId)
    {
        MsgReply msgReply;
        try
        {
            if (NodeList.Remove(nodeId))
            {
                Save();
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
            }
            else
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node not found" };
            }
        }
        catch (Exception ex)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail,Message = ex.Message};
        }

        return msgReply;
    }

    public void Load()
    {
        Proto.API.NodeList NodeListProto;
        if (File.Exists(DataFilePath))
        {  //Read from file
            NodeListProto = Proto.API.NodeList.Parser.ParseFrom(File.ReadAllBytes(DataFilePath));
        }
        else
        {
            NodeListProto = new Proto.API.NodeList();
            NodeListProto.Nodes.Add( new Node()
            {
                NodeGroupID = 1,
                Name = "NY Node",
                Host = "100.23.123.12",
                Monitor = false,
            });
            NodeListProto.Nodes.Add( new Node()
            {
                NodeID = 2,
                NodeGroupID = 1,
                Name = "London Node",
                Host = "10.3.44.88",
                Monitor = false,
            });
            NodeListProto.Nodes.Add( new Node()
            {
                NodeGroupID = 2,
                Name = "Frankfurt Node",
                Host = "155.22.14.184",
                Monitor = false,
            });
        }
        NodeList.Add(NodeListProto.Nodes);
    }
    
    public void Save()
    {
        File.WriteAllBytes(DataFilePath, Program.NodeManager.ProtoWrapper.ToByteArray());
    }
    
}