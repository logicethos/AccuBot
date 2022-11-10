using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Org.BouncyCastle.Crypto.Tls;
using Proto.API;
using ProtoHelper;

namespace AccuBot.Monitoring;

using TIndex = UInt32;
using TProto = Proto.API.Node;
using TProtoS = clsNode;
using TProtoList = Proto.API.NodeList;

//<TProtoList,TProto,TProtoS,TIndex>
//<Proto.API.NodeList, Proto.API.Node,clsNode,UInt32>

public class clsNodeProtoDictionaryShadow : clsProtoShadowTableIndexed<TProtoS, TProto, TIndex>
    /*where TProtoList : IMessage
    where TProto : IMessage
    where TProtoS : IProtoShadowClass<TIndex,TProto>
    where TIndex : class*/
{

    public ProtoPubSub<Proto.API.NodeStatus> NodeStatusSubSub = new ProtoPubSub<NodeStatus>();

  //  public clsProtoShadowTableIndexed<TProtoS, TProto, TIndex> NodeShadowList;

    private Action<TProto, TProto> MapFields = null;

    public clsNodeProtoDictionaryShadow() : base(new Func<TProto, IComparable<TIndex>>(x => x.NodeID)
                                            ,new Action<TProto, TIndex>((x, y) => x.NodeID = y))
    {
        
        MapFields = new Action<Proto.API.Node, Proto.API.Node>((origMessage, newMessage) =>
        {
            origMessage.Name = newMessage.Name;
            origMessage.Host = newMessage.Host;
            origMessage.Monitor = newMessage.Monitor;
            origMessage.NodeGroupID = newMessage.NodeGroupID;
        });

      //  var indexSelector = new Func<TProto, IComparable<TIndex>>(x => x.NodeID); //Index field of our proto message
      //  var indexSelectorWrite = new Action<TProto, TIndex>((x, y) => x.NodeID = y); //Write action for index field.

       // NodeShadowList = new clsProtoShadowTableIndexed<TProtoS, TProto, TIndex>(indexSelector,indexSelectorWrite);

        Load();
    }


    public TProtoS Add(TProto node)
    {
        return base.Add(node, new clsNode(node));
    }

    public bool Update(TProto nodeGroup)
    {
        return base.Update(nodeGroup,MapFields);
    }
    public MsgReply Delete(TIndex id)
    {
        var msgReply = new MsgReply();
        msgReply.Status = base.Remove(id) ? MsgReply.Types.Status.Ok : MsgReply.Types.Status.Fail;
        return msgReply;
    }

    public MsgReply AddUpdate(Node node)
    {
        var msgReply = new MsgReply();

        if (node.NodeID == 0)
        {
            var shadowClass = Add(node);
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
            msgReply.Status = Update(node) ? MsgReply.Types.Status.Ok : MsgReply.Types.Status.Fail;
        }

        return msgReply;
    }
    
//Path.Combine(Program.DataPath, "nodes"
    public void Load()
    {
        /*base.Load(new Func<NodeList>(() =>
        {
            var NodeListProto = new Proto.API.NodeList();
            NodeListProto.Nodes.Add(new Node()
            {
                NodeGroupID = 1,
                Name = "NY Node",
                Host = "100.23.123.12",
                Monitor = false,
            });
            NodeListProto.Nodes.Add(new Node()
            {
                NodeID = 2,
                NodeGroupID = 1,
                Name = "London Node",
                Host = "10.3.44.88",
                Monitor = false,
            });
            NodeListProto.Nodes.Add(new Node()
            {
                NodeGroupID = 2,
                Name = "Frankfurt Node",
                Host = "155.22.14.184",
                Monitor = false,
            });
            return NodeListProto;
        }));*/
    }
    
}