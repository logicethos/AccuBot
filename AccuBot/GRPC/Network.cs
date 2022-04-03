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
        MsgReply msgReply = null;

        if (network.NetworkID == 0) //id not set, so new network
        {
            network.NetworkID = Program.networkList.Network.Max(x => x.NetworkID) + 1; //get new id
            Program.networkList.Network.Add(network);
        }
        else
        {
            var existingNetwork = Program.networkList.Network.FirstOrDefault(x => x.NetworkID == network.NetworkID);
            if (existingNetwork == null) //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node not found" };
                return Task.FromResult(msgReply);
            }
            else
            {
                existingNetwork.Name = network.Name;
                existingNetwork.BlockTime = network.BlockTime;
                existingNetwork.StalledAfter = network.StalledAfter;
                existingNetwork.NotifictionID = network.NotifictionID;
            }
        }

        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = network.NetworkID };
        File.WriteAllBytes(Path.Combine(path, "networklist"), Program.networkList.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<NetworkList> NetworkListGet(Empty request, ServerCallContext context)
    {
        return Task.FromResult(Program.networkList);
    }

    public override Task<MsgReply> NetworkDelete(ID32 networkID, ServerCallContext context)
    {
        MsgReply msgReply = null;

        var user = Program.networkList.Network.FirstOrDefault(x => x.NetworkID == networkID.ID);
        if (user == null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Network not found" };
        }
        else
        {
            Program.networkList.Network.Remove(user);
            File.WriteAllBytes(Path.Combine(path, "networklist"), Program.networkList.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        }

        return Task.FromResult(msgReply);
    }
}