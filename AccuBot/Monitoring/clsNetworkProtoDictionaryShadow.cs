using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;
using ProtoHelper;

namespace AccuBot.Monitoring;

using TIndex = UInt32;
using TProto = Proto.API.Network;
using TProtoS = clsNetwork;
using TProtoList = Proto.API.NetworkList;

public class clsNetworkProtoDictionaryShadow
{

    public ProtoPubSub<Proto.API.NetworkStatus> NetworkStatusSubSub = new ProtoPubSub<NetworkStatus>();

    public clsProtoShadowTableIndexed<TProtoS, TProto, TIndex> NetworkShadowList;
    //private Proto.API.NetworkList NetworkList = new NetworkList();

    private Action<TProto, TProto> MapFields = null;

    public clsNetworkProtoDictionaryShadow()
    {

        MapFields = new Action<Proto.API.Network, Proto.API.Network>((origMessage, newMessage) =>
        {
            origMessage.Name = newMessage.Name;
            origMessage.BlockTime = newMessage.BlockTime;
            origMessage.StalledAfter = newMessage.StalledAfter;
            origMessage.NotifictionID = newMessage.NotifictionID;
        });

        var indexSelector = new Func<TProto, IComparable<TIndex>>(x => x.NetworkID); //Index field of our proto message
        var indexSelectorWrite = new Action<TProto, TIndex>((x, y) => x.NetworkID = y); //Write action for index field.

        NetworkShadowList = new clsProtoShadowTableIndexed<TProtoS, TProto, TIndex>(indexSelector, indexSelectorWrite);

        Load();
    }

    public TProtoS Add(TProto nodeGroup)
    {
        return NetworkShadowList.Add(nodeGroup, new clsNetwork(nodeGroup));
    }

    public bool Update(TProto network)
    {
        return NetworkShadowList.Update(network,MapFields);
    }


    public MsgReply Delete(TIndex id)
    {
        var msgReply = new MsgReply();
        msgReply.Status = NetworkShadowList.Remove(id) ? MsgReply.Types.Status.Ok : MsgReply.Types.Status.Fail;
        return msgReply;
    }

    public MsgReply AddUpdate(Network network)
    {
        var msgReply = new MsgReply();

        if (network.NotifictionID == 0)
        {
            var shadowClass = Add(network);
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
            msgReply.Status = Update(network) ? MsgReply.Types.Status.Ok : MsgReply.Types.Status.Fail;
        }

        return msgReply;
    }
    
    public void Load()
    {
        /*base.Load(new Func<Proto.API.NetworkList>(() =>
        {
            var NetworkListProto = new NetworkList();
            NetworkListProto.Network.Add(new Network
            {
                NetworkID = 1,
                Name = "Mainnet",
                StalledAfter = 60,
                BlockTime = 600,
                NotifictionID = 0,
            });
            NetworkListProto.Network.Add(new Network
            {
                NetworkID = 2,
                Name = "Testnet",
                StalledAfter = 300,
                BlockTime = 600,
                NotifictionID = 0,
            });
            return NetworkListProto;
        }));*/
    }

}