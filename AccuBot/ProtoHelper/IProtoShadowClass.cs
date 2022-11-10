namespace ProtoHelper;

public interface IProtoShadowClass<TID,TProto> where TProto:Google.Protobuf.IMessage
{
    public  TID ID { get; }
    public  TProto ProtoMessage { get; init; }

}