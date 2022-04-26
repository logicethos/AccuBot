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
    public override Task<MsgReply> NetworkSet(Network network, ServerCallContext context)
    {
        MsgReply msgReply;

        if (network.NetworkID == 0) //id not set, so new network
            msgReply = Program.NetworkProtoDictionaryShadow.Add(network);
        else
            msgReply = Program.NetworkProtoDictionaryShadow.Update(network);
        
        return Task.FromResult(msgReply);
    }

    public override Task<NetworkList> NetworkListGet(Empty request, ServerCallContext context)
    {
        return Task.FromResult(Program.NetworkProtoDictionaryShadow.ProtoWrapper);
    }

    public override Task<MsgReply> NetworkDelete(ID32 networkID, ServerCallContext context)
    {
        return Task.FromResult(Program.NetworkProtoDictionaryShadow.Delete(networkID.ID));
    }
}