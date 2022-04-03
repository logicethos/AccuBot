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
     public override Task<MsgReply> NodeGroupSet(NodeGroup group, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingNodeGroup = NodeGroupListGet(null, null).Result;  //get existing nodes.

        if (group.NodeGroupID == 0) //id not set, so new nodegroup
        {
            group.NodeGroupID = exitingNodeGroup.NodeGroup.Max(x => x.NodeGroupID) + 1; //get new id
            exitingNodeGroup.NodeGroup.Add(group);
        }
        else
        {
            var existingNetwork = exitingNodeGroup.NodeGroup.FirstOrDefault(x => x.NodeGroupID == group.NodeGroupID);
            if (existingNetwork == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NodeGroup not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingNetwork.Name = group.Name;
                existingNetwork.NetworkID = group.NetworkID;
                existingNetwork.HeightNotifictionID = group.HeightNotifictionID;
                existingNetwork.LatencyNotifictionID = group.LatencyNotifictionID;
                existingNetwork.PingNotifictionID = group.PingNotifictionID;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = group.NodeGroupID};
        File.WriteAllBytes(Path.Combine(path, "nodeGrouplist"),exitingNodeGroup.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<NodeGroupList> NodeGroupListGet(Empty request, ServerCallContext context)
    {
        NodeGroupList groupList;
        if (File.Exists(Path.Combine(path,"nodeGrouplist")))
        {  //Read from file
            groupList = NodeGroupList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(path, "nodeGrouplist")));
        }
        else
        {
            groupList = new NodeGroupList();
            groupList.NodeGroup.Add(new NodeGroup()
            {
                NodeGroupID = 1,
                Name = "Validator",
                NetworkID = 1,
                PingNotifictionID = 1,
                HeightNotifictionID = 1,
                LatencyNotifictionID = 1
            });
            groupList.NodeGroup.Add(new NodeGroup
            {
                NodeGroupID = 2,
                Name = "Follower",
                NetworkID = 1,
                PingNotifictionID = 1,
                HeightNotifictionID = 1,
                LatencyNotifictionID = 1
            });
        }
        return Task.FromResult(groupList);
    }

    public override Task<MsgReply> NodeGroupDelete(ID32 nodeGroupID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingNodeGroup = NodeGroupListGet(null, null).Result;  //get existing nodes.

        var nodegroup = existingNodeGroup.NodeGroup.FirstOrDefault(x => x.NodeGroupID == nodeGroupID.ID);
        if (nodegroup==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NodeGroup not found"};
        }
        else
        {
            existingNodeGroup.NodeGroup.Remove(nodegroup);
            File.WriteAllBytes(Path.Combine(path, "nodeGrouplist"),existingNodeGroup.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }

}