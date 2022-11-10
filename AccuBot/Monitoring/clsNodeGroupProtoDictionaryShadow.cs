using System.Collections.ObjectModel;
using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;
using ProtoHelper;

namespace AccuBot.Monitoring;

using TIndex = UInt32;
using TProto = Proto.API.NodeGroup;
using TProtoS = clsNodeGroup;
using TProtoList = Proto.API.NodeGroupList;

public class clsNodeGroupProtoDictionaryShadow
{

    public clsProtoShadowTableIndexed<TProtoS, TProto, TIndex> NodeGroupShadow;

    private Action<TProto, TProto> MapFields = null;

    public clsNodeGroupProtoDictionaryShadow()
    {

        MapFields = new Action<TProto, TProto>((origMessage, newMessage) =>
        {
            origMessage.Name = newMessage.Name;
            origMessage.NetworkID = newMessage.NetworkID;
            origMessage.HeightNotifictionID = newMessage.HeightNotifictionID;
            origMessage.LatencyNotifictionID = newMessage.LatencyNotifictionID;
            origMessage.PingNotifictionID = newMessage.PingNotifictionID;
        });

        var indexSelector = new Func<TProto, IComparable<TIndex>>(x => x.NodeGroupID); //Index field of our proto message
        var indexSelectorWrite = new Action<TProto, TIndex>((x, y) => x.NodeGroupID = y); //Write action for index field.

        NodeGroupShadow = new clsProtoShadowTableIndexed<TProtoS, TProto, uint>(indexSelector, indexSelectorWrite);

        Load();
    }


    public MsgReply AddUpdate(TProto nodeGroup)
    {
        var msgReply = new MsgReply();

        if (nodeGroup.NodeGroupID == 0)
        {
            var shadowClass = Add(nodeGroup);
            if (shadowClass == null)
            {
                msgReply.Status = MsgReply.Types.Status.Fail;
            }
            else
            {
                msgReply.Status = MsgReply.Types.Status.Ok;
                msgReply.NewID32 = shadowClass.ID;
            }
        }
        else
        {
            msgReply.Status = Update(nodeGroup) ? MsgReply.Types.Status.Ok : MsgReply.Types.Status.Fail;
        }

        return msgReply;
    }

    public TProtoS Add(TProto nodeGroup)
    {
        return NodeGroupShadow.Add(nodeGroup, new clsNodeGroup(nodeGroup));
    }

    public bool Update(TProto nodeGroup)
    {
        return NodeGroupShadow.Update(nodeGroup,MapFields);
    }


    public MsgReply Delete(TIndex id)
    {
        var msgReply = new MsgReply();
        msgReply.Status = NodeGroupShadow.Remove(id) ? MsgReply.Types.Status.Ok : MsgReply.Types.Status.Fail;
        return msgReply;
    }
    public void Load()
    {
        /*base.Load(new Func<NodeGroupList>(() =>
        {
            var nodeGroupList = new NodeGroupList();
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
            return nodeGroupList;
        }));*/
    }



}