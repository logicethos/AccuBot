using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;

namespace AccuBot.GRPC;

public partial class ApiService
{
    public override Task<MsgReply> NetworkSet(Network network, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingNetworks = NetworkListGet(null, null).Result; //get existing nodes.

        if (network.NetworkID == 0) //id not set, so new network
        {
            network.NetworkID = exitingNetworks.Network.Max(x => x.NetworkID) + 1; //get new id
            exitingNetworks.Network.Add(network);
        }
        else
        {
            var existingNetwork = exitingNetworks.Network.FirstOrDefault(x => x.NetworkID == network.NetworkID);
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
        File.WriteAllBytes(Path.Combine(path, "networklist"), exitingNetworks.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<NetworkList> NetworkListGet(Empty request, ServerCallContext context)
    {
        NetworkList networklist = null;
        if (File.Exists(Path.Combine(path, "networklist")))
        {
            //Read from file
            networklist = NetworkList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(path, "networklist")));
        }
        else
        {
            networklist = new NetworkList();
            networklist.Network.Add(new Network
            {
                NetworkID = 1,
                Name = "Mainnet",
                StalledAfter = 60,
                BlockTime = 600,
                NotifictionID = 0,
            });
            networklist.Network.Add(new Network
            {
                NetworkID = 2,
                Name = "Testnet",
                StalledAfter = 300,
                BlockTime = 600,
                NotifictionID = 0,
            });
        }

        return Task.FromResult(networklist);
    }

    public override Task<MsgReply> NetworkDelete(ID32 networkID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingNetworks = NetworkListGet(null, null).Result; //get existing nodes.

        var user = existingNetworks.Network.FirstOrDefault(x => x.NetworkID == networkID.ID);
        if (user == null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Network not found" };
        }
        else
        {
            existingNetworks.Network.Remove(user);
            File.WriteAllBytes(Path.Combine(path, "networklist"), existingNetworks.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        }

        return Task.FromResult(msgReply);
    }
}