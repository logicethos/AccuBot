using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using Proto.API;


namespace ProtoHelper;
/// <summary>
/// Dictionary of proto messages, that are twinned with another class (a shadow class).
/// </summary>
/// <typeparam name="TProtoS">Our shadow class</typeparam>
/// <typeparam name="TProto">The proto message class, we want to extend</typeparam>
public class clsProtoShadowTableIndexed<TProtoS,TProto,TIndex> : Dictionary<TIndex,TProtoS>,IDisposable where TProto:IMessage where TProtoS:IProtoShadowClass<TIndex,TProto>
{
    private RepeatedField<TProto> ProtoRepeatedField { get; init; }
  //  private Action<TProto, UInt64> IndexSelectorWrite;
    private Func<TProto, IComparable<TIndex>> IndexSelector;

    EventWaitHandle NewMessageWait = new EventWaitHandle(false, EventResetMode.ManualReset);
    public event EventHandler<TProto> NewMessage;
    
    private readonly object _lock = new object();

    private TProto? _lastMessage;
    private TProto? LastMessage
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
    
    public clsProtoShadowTableIndexed(Func<TProto, IComparable<TIndex>> indexSelector, Action<TProto, TIndex> indexSelectorWrite) : base()
    {
        ProtoRepeatedField = new RepeatedField<TProto>();
        IndexSelector = indexSelector;
      //  IndexSelectorWrite = indexSelectorWrite;
    }

    public RepeatedField<TProto> GetProtoRepeatedField()
    {
        lock (_lock)
        {
            return ProtoRepeatedField.Clone();
        }
    }

    public void PopulateRepeatedField (RepeatedField<TProto> rp)
    {
        lock (_lock)
        {
            rp.AddRange(ProtoRepeatedField);
        }
    }

    public bool Update(TProto protoMessage, Action<TProto, TProto>? mapfields = null)
    {
        bool success;
        var id = (TIndex)IndexSelector(protoMessage);
        lock (_lock)
        {
            success = Update(id,protoMessage, mapfields);
        }

        if (success)
        {
            NewMessage?.Invoke(this, protoMessage);
            LastMessage = protoMessage;
        }
        return success;
    }

    public void Update(RepeatedField<TProto> repeatedField,Action<TProto,TProto>? mapfields = null)
    {
        lock (_lock)
        {
            foreach (var protoMessage in repeatedField)
            {
                var id = (TIndex)IndexSelector(protoMessage);

                if(EqualityComparer<TIndex>.Default.Equals(id, default(TIndex))) //ID not set
                {
                    if (this.ContainsKey(id))
                        Update(id, protoMessage, mapfields);
               //     else
              //          AddPrivate(id, protoMessage);
                }
            }
        }
    }
    
    private bool Update(TIndex id, TProto newMessage,Action<TProto,TProto>? mapfields = null)
    {
        if (!this.ContainsKey(id)) return false;

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

        return true;
    }
    
    /*/// <summary>
    /// Add bulk items.  Note: No Events triggered here, for subscribed consumers.  
    /// </summary>
    /// <param name="repeatedField"></param>
    public new void Add(RepeatedField<TProto> repeatedField)
    {
        lock (_lock)
        {
            foreach (var protoMessage in repeatedField)
            {
                var id = (UInt64)IndexSelector(protoMessage);
                if (id != 0) //ID not set
                {
                    var valueClass = (TProtoS)Activator.CreateInstance(typeof(TProtoS), protoMessage);
                    base.Add(id, valueClass);
                }
            }
            
            ProtoRepeatedField.AddRange(repeatedField);
        }
    }*/

    
    public TProtoS Add(TProto value, TProtoS shadowClass)
    {
        var ID = (TIndex)IndexSelector(value);
        if (shadowClass.ID.Equals(ID)) throw new Exception();

        if (EqualityComparer<TIndex>.Default.Equals(ID, default(TIndex))) throw new Exception("Index not set");

        lock (_lock)
        {
            if (!base.TryAdd(ID, shadowClass)) return default;
            ProtoRepeatedField.Add(value);
        }
        LastMessage = value;
        NewMessage?.Invoke(this,value);
        return shadowClass;
    }

    public bool Remove(TProto value)
    {
        return Remove((TIndex)IndexSelector(value));
    }
    
    public new bool Remove(TIndex id)
    {
        lock (_lock)
        {
            if (base.Remove(id))
            {
                var value = ProtoRepeatedField.FirstOrDefault(x => IndexSelector(x).Equals(id));
                if (value != null) ProtoRepeatedField.Remove(value);
                NewMessage?.Invoke(this,value);
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

    /*public async void Load<TProtoList>(string path, Func<RepeatedField<TProto>> defaultData = null)
    {
        RepeatedField<TProto> networkListProto;
        var parser = new Google.Protobuf.MessageParser<TProtoList>(() => ProtoWrapper);
        if (File.Exists(path))
        {
            //Read from file
            networkListProto = parser.ParseFrom(File.ReadAllBytes(path));
            ManagerList.Add(RepeatedFieldSelector(networkListProto));
        }
        else if (defaultData != null)
        {
            networkListProto = defaultData();
            ManagerList.Add(RepeatedFieldSelector(networkListProto));
        }
    }

    private void Save(string path)
    {
    //    File.WriteAllBytes(path, Program.NetworkProtoDictionaryShadow.ProtoWrapper.ToByteArray());
    }*/

    public void Dispose()
    {
        NewMessageWait.Dispose();
    }
    
}