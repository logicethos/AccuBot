using Google.Protobuf;
using Proto.API;

namespace AccuBot;
public interface IManager<TProto,TProtoList> : IDisposable where TProto:Google.Protobuf.IMessage where TProtoList:Google.Protobuf.IMessage
//public interface IProtoManager<TProto,TProtoShadow> where  TProto:Google.Protobuf.IMessage where TProtoShadow:IProtoShadowClass<TProto>
{
    public TProtoList ProtoWrapper { get; init; }
    public void Load(Func<TProtoList> defaultData);
    
    public Action<TProto, TProto> MapFields { get; init; }
    
    public MsgReply Update(TProto proto);
    public MsgReply Add(TProto proto);
    public MsgReply Delete(UInt32 ID);
}