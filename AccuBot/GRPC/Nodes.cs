using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;

namespace AccuTest.GRPC;

public partial class ApiService 
{
    public override Task<MsgReply> NodeSet(Node node, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingNodes = NodeListGet(null, null).Result;  //get existing nodes.

        if (node.NodeID == 0) //id not set, so new node
        {
            node.NodeID = exitingNodes.Nodes.Max(x => x.NodeID) + 1; //get new id
            exitingNodes.Nodes.Add(node);
        }
        else
        {
            var existingNode = exitingNodes.Nodes.FirstOrDefault(x => x.NodeID == node.NodeID);
            if (existingNode == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingNode.Host = node.Host;
                existingNode.Monitor = node.Monitor;
                existingNode.Name = node.Name;
                existingNode.NodeGroupID = node.NodeGroupID;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = node.NodeID};
        File.WriteAllBytes(Path.Combine(path, "nodes"),exitingNodes.ToByteArray());
        return Task.FromResult(msgReply);
    }
    
    public override Task<NodeList> NodeListGet(Empty request, ServerCallContext context)
    {
        NodeList nodelist = null;
        if (File.Exists(Path.Combine(path,"nodes")))
        {  //Read from file
            nodelist = NodeList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(path, "nodes")));
        }
        else
        {
            nodelist = new NodeList();
            nodelist.Nodes.Add( new Node()
            {
                NodeGroupID = 1,
                Name = "my node",
                Host = "123.123.123.123",
                Monitor = true,
            });
        }

        return Task.FromResult(nodelist);
    }

    public override Task<MsgReply> NodeDelete(ID32 nodeID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingNodes = NodeListGet(null, null).Result;  //get existing nodes.

        var node = existingNodes.Nodes.FirstOrDefault(x => x.NodeID == nodeID.ID);
        if (node==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node not found"};
        }
        else
        {
            existingNodes.Nodes.Remove(node);
            File.WriteAllBytes(Path.Combine(path, "nodes"),existingNodes.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }
}