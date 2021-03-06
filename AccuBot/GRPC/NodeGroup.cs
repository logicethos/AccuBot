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
        MsgReply msgReply;

        if (group.NodeGroupID == 0) //id not set, so new group
            msgReply = Program.NodeGroupProtoDictionaryShadow.Add(group);
        else
            msgReply = Program.NodeGroupProtoDictionaryShadow.Update(group);
        
        return Task.FromResult(msgReply);
    }

    public override Task<NodeGroupList> NodeGroupListGet(Empty request, ServerCallContext context)
    {
        return Task.FromResult(Program.NodeGroupProtoDictionaryShadow.ProtoWrapper);
    }

    public override Task<MsgReply> NodeGroupDelete(ID32 nodeGroupID, ServerCallContext context)
    {
        return Task.FromResult(Program.NetworkProtoDictionaryShadow.Delete(nodeGroupID.ID));
    }

}