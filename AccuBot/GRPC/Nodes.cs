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
    public override Task<MsgReply> NodeSet(Node node, ServerCallContext context)
    {
        MsgReply msgReply;

        if (node.NodeID == 0) //id not set, so new network
            msgReply = Program.NodeProtoDictionaryShadow.Add(node);
        else
            msgReply = Program.NodeProtoDictionaryShadow.Update(node);
        
        return Task.FromResult(msgReply);
    }
    
    public override Task<Proto.API.NodeList> NodeListGet(Empty request, ServerCallContext context)
    {
        return Task.FromResult(Program.NodeProtoDictionaryShadow.ProtoWrapper);
    }

    public override Task<MsgReply> NodeDelete(ID32 nodeID, ServerCallContext context)
    {
        return Task.FromResult(Program.NodeProtoDictionaryShadow.Delete(nodeID.ID));
    }
}