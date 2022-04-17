using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using Proto.API;


namespace AccuBot;
/// <summary>
/// Dictionary of proto messages, that are twinned with another class (a shadow class).
/// </summary>
/// <typeparam name="TClass">Our shadow class</typeparam>
/// <typeparam name="TPClass">The proto message class, we want to extend</typeparam>
public class clsProtoShadow<TClass,TPClass> : Dictionary<UInt32,TClass>,IDisposable where TPClass:IMessage where TClass:IProtoShadowClass<TPClass>
{
    public RepeatedField<TPClass> ProtoRepeatedField { get; init; }
    private Action<TPClass, UInt32> IndexSelectorWrite;
    private Func<TPClass, IComparable<UInt32>> IndexSelector;
    private UInt32 MaxID;

    EventWaitHandle NewMessageWait = new EventWaitHandle(false, EventResetMode.ManualReset);
    private readonly object _lock = new object();

    private TPClass _lastMessage;
    private TPClass LastMessage
    {
        get
        {
            return _lastMessage;
        }
        set
        {
            _lastMessage = value;
            NewMessageWait.Set();
            NewMessageWait.Reset();
        }
    }


    public delegate void MapFields(TPClass origMessage, TPClass newMessage);
    
    public clsProtoShadow(RepeatedField<TPClass> protoRepeatedField, Func<TPClass, IComparable<UInt32>> indexSelector, Action<TPClass, UInt32> indexSelectorWrite) : base()
    {
        ProtoRepeatedField = protoRepeatedField;
        IndexSelector = indexSelector;
        IndexSelectorWrite = indexSelectorWrite;
    }

    public void Update(TPClass newMessage,Action<TPClass,TPClass> mapfields = null)
    {
        lock (_lock)
        {
            var id = (UInt32)IndexSelector(newMessage);
            TPClass origMessage = this[id].ProtoMessage;

            if (mapfields == null)
            {
                var originalProperties = origMessage.GetType().GetProperties();

                //For each updated property
                foreach (var updateProperty in newMessage.GetType().GetProperties())
                {
                    var originalProperty = originalProperties.FirstOrDefault(x =>
                        x.Name == updateProperty.Name && x.GetValue(x) == updateProperty.GetValue(updateProperty));
                    if (originalProperty != null)
                        originalProperty.SetValue(originalProperty, updateProperty.GetValue(updateProperty));
                }
            }
            else
            {
                mapfields(origMessage, newMessage);
            }

            LastMessage = newMessage;
        }
    }
    
    
    public new void Add(RepeatedField<TPClass> repeatedField)
    {
        lock (_lock)
        {
            var maxID = GetMaxID(repeatedField);
            if (maxID > MaxID) MaxID = maxID;

            foreach (var protoMessage in repeatedField)
            {
                var id = (UInt32)IndexSelector(protoMessage);
                if (id == 0) //ID not set
                {
                    id = ++MaxID;
                    IndexSelectorWrite(protoMessage, MaxID);
                }

                var valueClass = (TClass)Activator.CreateInstance(typeof(TClass), protoMessage);
                base.Add(id, valueClass);
            }

            ProtoRepeatedField.AddRange(repeatedField);
        }
    }

    private UInt32 GetMaxID(RepeatedField<TPClass> repeatedField)
    {
        UInt32 maxVal = 0;
        foreach (var message in repeatedField)
        {
            var value = (UInt32)IndexSelector(message);
            if (value > maxVal) maxVal = value;
        }
        return maxVal;
    }
    
    
    public new UInt32 Add(TPClass value)
    {
        lock (_lock)
        {
            if ((UInt32)IndexSelector(value) != 0) throw new Exception("Index cannot be set");

            MaxID++;
            IndexSelectorWrite(value, MaxID);

            ProtoRepeatedField.Add(value);
            var valueClass = (TClass)Activator.CreateInstance(typeof(TClass), value);

            base.Add(MaxID, valueClass);
            LastMessage = value;
            return MaxID;
        }
    }

    public new bool Remove(TPClass value)
    {
        return Remove((UInt32)IndexSelector(value));
    }
    
    public new bool Remove(UInt32 id)
    {
        lock (_lock)
        {
            if (base.Remove(id))
            {
                var value = ProtoRepeatedField.FirstOrDefault(x => IndexSelector(x).Equals(id));
                if (value != null) ProtoRepeatedField.Remove(value);
                LastMessage = value;
                return true;
            }

            return false;
        }
    }

    public async Task NextMessageWait(IServerStreamWriter<TPClass> serverStreamWriter)
    {
        while (NewMessageWait.WaitOne())
        {
            await serverStreamWriter.WriteAsync(LastMessage);
        }
    }

    public void Dispose()
    {
        NewMessageWait.Dispose();
    }
    
}