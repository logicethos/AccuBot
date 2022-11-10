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
        var msgReply = Program.NodeGroupProtoDictionaryShadow.AddUpdate(group);

        return Task.FromResult(msgReply);
    }

    public override Task<NodeGroupList> NodeGroupListGet(Empty request, ServerCallContext context)
    {

        var proto = new NodeGroupList();
        Program.NodeGroupProtoDictionaryShadow.PopulateRepeatedField(proto.NodeGroup);
        return Task.FromResult(proto);

    }

    public override Task<MsgReply> NodeGroupDelete(ID32 nodeGroupID, ServerCallContext context)
    {
        return Task.FromResult(Program.NetworkProtoDictionaryShadow.Delete(nodeGroupID.ID));
    }

}