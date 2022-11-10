namespace ProtoHelper;

public interface IProtoListClass<TProtoList,TProto,TProtoS,TIndex>
   where TProtoList:Google.Protobuf.IMessage
   where TProto:Google.Protobuf.IMessage
   where TProtoS:class
   where TIndex:struct,
   IComparable,
   IComparable<TIndex>,
   IConvertible,
   IEquatable<TIndex>,
   IFormattable

{
   // public  TID ID { get; }
   // public  TProto ProtoMessage { get; init; }

}