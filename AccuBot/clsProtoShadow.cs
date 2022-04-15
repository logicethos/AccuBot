using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Accubot;
using Google.Protobuf.Collections;
using Proto.API;


namespace AccuBot;
/// <summary>
/// Dictionary of proto messages, that are twinned with another class (a shadow class).
/// </summary>
/// <typeparam name="IProtoShadowClass">Our shadow class</typeparam>
/// <typeparam name="TProtoMessage">The proto message class, we want to extend</typeparam>
public class clsProtoShadow<IProtoShadowClass,TProtoMessage> : Dictionary<UInt32,IProtoShadowClass>
{
    public RepeatedField<TProtoMessage> ProtoRepeatedField { get; init; }
    private Action<TProtoMessage, UInt32> IndexSelectorWrite;
    private Func<TProtoMessage, IComparable<UInt32>> IndexSelector;
    private UInt32 MaxID;

    
    public clsProtoShadow(RepeatedField<TProtoMessage> protoRepeatedField, Func<TProtoMessage, IComparable<UInt32>> indexSelector, Action<TProtoMessage, UInt32> indexSelectorWrite) : base()
    {
        ProtoRepeatedField = protoRepeatedField;
        IndexSelector = indexSelector;
        IndexSelectorWrite = indexSelectorWrite;
        
    }

    public new void Add(RepeatedField<TProtoMessage> repeatedField)
    {
        var maxID = GetMaxID(repeatedField);
        if (maxID > MaxID) MaxID = maxID;
        
        foreach (var protoMessage in repeatedField)
        {
            var id = (UInt32)IndexSelector(protoMessage);
            if (id == 0) //ID not set
            {
                id = ++MaxID;
                IndexSelectorWrite(protoMessage,MaxID);
            }
            
            var valueClass = (IProtoShadowClass)Activator.CreateInstance(typeof(IProtoShadowClass),protoMessage);
            base.Add(id,valueClass);
        }

        ProtoRepeatedField.AddRange(repeatedField);
    }

    public UInt32 GetMaxID(RepeatedField<TProtoMessage> repeatedField)
    {
        UInt32 maxVal = 0;
        foreach (var message in repeatedField)
        {
            var value = (UInt32)IndexSelector(message);
            if (value > maxVal) maxVal = value;
        }
        return maxVal;
    }
    
    
    public new UInt32 Add(TProtoMessage value)
    {
        if ((UInt32)IndexSelector(value) != 0) throw new Exception("Index cannot be set");
        
        MaxID++;
        IndexSelectorWrite(value,MaxID);

        ProtoRepeatedField.Add(value);
        var valueClass = (IProtoShadowClass)Activator.CreateInstance(typeof(IProtoShadowClass),value);
        
        base.Add(MaxID,valueClass);
       
        return MaxID;
    }

    public new bool Remove(TProtoMessage value)
    {
        return Remove((UInt32)IndexSelector(value));
    }
    
    public new bool Remove(UInt32 id)
    {
        if (base.Remove(id))
        {
            var value = ProtoRepeatedField.FirstOrDefault(x => IndexSelector(x).Equals(id));
            if (value != null) ProtoRepeatedField.Remove(value);
            return true;
        }
        return false;
    }

    
}