using Google.Protobuf;
using Proto.API;

namespace AccuBot;
public interface IProtoManager<TProto,TProtoWrapper> : IDisposable where TProto:Google.Protobuf.IMessage where TProtoWrapper:Google.Protobuf.IMessage
//public interface IProtoManager<TProto,TProtoShadow> where  TProto:Google.Protobuf.IMessage where TProtoShadow:IProtoShadowClass<TProto>
{
    public TProtoWrapper ProtoWrapper { get; init; }
    public void Load();
    public MsgReply Update(TProto proto);
    public MsgReply Add(TProto proto);
    public MsgReply Delete(UInt32 ID);
}