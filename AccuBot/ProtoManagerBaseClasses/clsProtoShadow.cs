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
/// <typeparam name="TProtoS">Our shadow class</typeparam>
/// <typeparam name="TProto">The proto message class, we want to extend</typeparam>
public class clsProtoShadow<TProtoS,TProto> : Dictionary<UInt32,TProtoS>,IDisposable where TProto:IMessage where TProtoS:IProtoShadowClass<TProto>
{
    public RepeatedField<TProto> ProtoRepeatedField { get; init; }
    private Action<TProto, UInt32> IndexSelectorWrite;
    private Func<TProto, IComparable<UInt32>> IndexSelector;
    private UInt32 MaxID;

    EventWaitHandle NewMessageWait = new EventWaitHandle(false, EventResetMode.ManualReset);
    private readonly object _lock = new object();

    private TProto _lastMessage;
    private TProto LastMessage
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


    public delegate void MapFields(TProto origMessage, TProto newMessage);
    
    public clsProtoShadow(RepeatedField<TProto> protoRepeatedField, Func<TProto, IComparable<UInt32>> indexSelector, Action<TProto, UInt32> indexSelectorWrite) : base()
    {
        ProtoRepeatedField = protoRepeatedField;
        IndexSelector = indexSelector;
        IndexSelectorWrite = indexSelectorWrite;
    }

    public void Update(TProto newMessage,Action<TProto,TProto> mapfields = null)
    {
        lock (_lock)
        {
            var id = (UInt32)IndexSelector(newMessage);
            TProto origMessage = this[id].ProtoMessage;

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
    
    
    public new void Add(RepeatedField<TProto> repeatedField)
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

                var valueClass = (TProtoS)Activator.CreateInstance(typeof(TProtoS), protoMessage);
                base.Add(id, valueClass);
            }

            ProtoRepeatedField.AddRange(repeatedField);
        }
    }

    private UInt32 GetMaxID(RepeatedField<TProto> repeatedField)
    {
        UInt32 maxVal = 0;
        foreach (var message in repeatedField)
        {
            var value = (UInt32)IndexSelector(message);
            if (value > maxVal) maxVal = value;
        }
        return maxVal;
    }
    
    
    public new UInt32 Add(TProto value)
    {
        lock (_lock)
        {
            if ((UInt32)IndexSelector(value) != 0) throw new Exception("Index cannot be set");

            MaxID++;
            IndexSelectorWrite(value, MaxID);

            ProtoRepeatedField.Add(value);
            var valueClass = (TProtoS)Activator.CreateInstance(typeof(TProtoS), value);

            base.Add(MaxID, valueClass);
            LastMessage = value;
            return MaxID;
        }
    }

    public new bool Remove(TProto value)
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

    public async Task NextMessageWait(IServerStreamWriter<TProto> serverStreamWriter)
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