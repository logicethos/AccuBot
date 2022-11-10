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
        var msgReply = Program.NetworkProtoDictionaryShadow.AddUpdate(network);

        return Task.FromResult(msgReply);
    }

    public override Task<NetworkList> NetworkListGet(Empty request, ServerCallContext context)
    {
        var proto = new NetworkList();
        Program.NetworkProtoDictionaryShadow.NetworkShadowList.PopulateRepeatedField(proto.Network);
        return Task.FromResult(proto);
    }

    public override Task<MsgReply> NetworkDelete(ID32 networkID, ServerCallContext context)
    {
        return Task.FromResult(Program.NetworkProtoDictionaryShadow.Delete(networkID.ID));
    }
}