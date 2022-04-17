namespace AccuBot;

public interface IProtoShadowClass<TProto> where TProto:Google.Protobuf.IMessage
{
    public  UInt32 ID { get; }
    public  TProto ProtoMessage { get; init; }

}