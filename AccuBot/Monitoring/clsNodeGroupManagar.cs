using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNodeGroupManagar : IProtoManager<Proto.API.NodeGroup,Proto.API.NodeGroupList>
{
    public clsProtoShadow<clsNodeGroup,Proto.API.NodeGroup> NodeGroupList { get; init; }
    private readonly String DataFilePath = Path.Combine(Program.DataPath, "nodeGroupList");
    public Proto.API.NodeGroupList ProtoWrapper { get; init; }
    
    public clsNodeGroupManagar()
    {
        ProtoWrapper = new Proto.API.NodeGroupList();
        NodeGroupList = new clsProtoShadow<clsNodeGroup, Proto.API.NodeGroup>(ProtoWrapper.NodeGroup,x => x.NodeGroupID, (x, y) => x.NodeGroupID = y);
        Load();
    }

    public MsgReply Update(Proto.API.NodeGroup nodeGroup)
    {
        MsgReply msgReply;
        clsNodeGroup existingPolicy;
        
        NodeGroupList.TryGetValue(nodeGroup.NodeGroupID, out existingPolicy);
        if (existingPolicy == null) //Incorrect ID sent!
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NodeGroup not found" };
        }
        else
        {
            existingPolicy.ProtoMessage.Name = nodeGroup.Name;
            existingPolicy.ProtoMessage.NetworkID = nodeGroup.NetworkID;
            existingPolicy.ProtoMessage.HeightNotifictionID = nodeGroup.HeightNotifictionID;
            existingPolicy.ProtoMessage.LatencyNotifictionID = nodeGroup.LatencyNotifictionID;
            existingPolicy.ProtoMessage.PingNotifictionID = nodeGroup.PingNotifictionID;
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return msgReply;
    }

    public MsgReply Add(NodeGroup network)
    {
        MsgReply msgReply;
        try
        {
            var id=this.NodeGroupList.Add(network);
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = id};
        }
        catch (Exception e)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = e.Message};
        }

        return msgReply;
    }

    
    public MsgReply Delete(UInt32 policyId)
    {
        MsgReply msgReply;
        try
        {
            if (NodeGroupList.Remove(policyId))
            {
                Save();
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
            }
            else
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node Group not found" };
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
        Proto.API.NodeGroupList nodeGroupList;
        if (File.Exists(DataFilePath))
        {  //Read from file
            nodeGroupList = Proto.API.NodeGroupList.Parser.ParseFrom(File.ReadAllBytes(DataFilePath));
        }
        else
        {
            nodeGroupList = new NodeGroupList();
            nodeGroupList.NodeGroup.Add(new NodeGroup()
            {
                NodeGroupID = 1,
                Name = "Validator",
                NetworkID = 1,
                PingNotifictionID = 1,
                HeightNotifictionID = 1,
                LatencyNotifictionID = 1
            });
            nodeGroupList.NodeGroup.Add(new NodeGroup
            {
                NodeGroupID = 2,
                Name = "Follower",
                NetworkID = 1,
                PingNotifictionID = 1,
                HeightNotifictionID = 1,
                LatencyNotifictionID = 1
            });
        }

        NodeGroupList.Add(nodeGroupList.NodeGroup);

    }
    
    private void Save()
    {
        File.WriteAllBytes(DataFilePath, Program.NetworkManager.ProtoWrapper.ToByteArray());
    }
    
    public void Dispose()
    {
    }
}