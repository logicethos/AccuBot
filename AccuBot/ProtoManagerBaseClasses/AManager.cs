using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public abstract class AManager<TProtoS,TProto,TProtoList> : IManager<TProto,TProtoList> where TProtoS:IProtoShadowClass<TProto> where TProto:IMessage<TProto> where TProtoList:IMessage<TProtoList>
{
    public clsProtoShadow<TProtoS,TProto> ManagerList { get; init; }
    public String DataFilePath { get; init; }
    public TProtoList ProtoWrapper { get; init; }
    private Action<TProto, UInt32> IndexSelectorWrite;
    private Func<TProto, IComparable<UInt32>> IndexSelector;
    private Func<TProtoList, RepeatedField<TProto>> RepeatedFieldSelector;

    public Action<TProto, TProto> MapFields { get; set; } = null;

    public AManager(string path,
                    Func<TProtoList, RepeatedField<TProto>> repeatedFieldSelector,
                    Func<TProto, IComparable<UInt32>> indexSelector,
                    Action<TProto, UInt32> indexSelectorWrite)
    {
        DataFilePath = path;
        IndexSelector = indexSelector;              //Index field of our proto message
        IndexSelectorWrite = indexSelectorWrite;    //Write action for index field.
        RepeatedFieldSelector = repeatedFieldSelector;
        
        ProtoWrapper = (TProtoList)Activator.CreateInstance(typeof(TProtoList));
        var repeatedfield = repeatedFieldSelector(ProtoWrapper);

        ManagerList = new clsProtoShadow<TProtoS, TProto>(repeatedfield, indexSelector, indexSelectorWrite);
        Load();
    }

    public MsgReply Update(TProto network)
    {
        MsgReply msgReply;
        TProtoS existingNetwork;
        ManagerList.TryGetValue((UInt32)IndexSelector(network), out existingNetwork);
        if (existingNetwork == null) //Incorrect ID sent!
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = $"{typeof(TProto).FullName} not found" };
        }
        else
        {
            ManagerList.Update(network,MapFields);
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return msgReply;
    }

    public MsgReply Add(TProto network)
    {
        MsgReply msgReply;
        try
        {
            var id=this.ManagerList.Add(network);
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = id};
        }
        catch (Exception e)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = e.Message};
        }

        return msgReply;
    }
    
    public MsgReply Delete(UInt32 networkId)
    {
        MsgReply msgReply;
        try
        {
            if (ManagerList.Remove(networkId))
            {
                Save();
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
            }
            else
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = $"{typeof(TProto).FullName} not found" };
            }
        }
        catch (Exception ex)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail,Message = ex.Message};
        }

        return msgReply;
    }

    public void Load(Func<TProtoList> demoData = null)
    {
        TProtoList networkListProto;
        var parser = new Google.Protobuf.MessageParser<TProtoList>(() => ProtoWrapper);
        if (File.Exists(DataFilePath))
        {
            //Read from file
            networkListProto = parser.ParseFrom(File.ReadAllBytes(DataFilePath));
            ManagerList.Add(RepeatedFieldSelector(networkListProto) as RepeatedField<TProto>);    
        }
        else
        {
            networkListProto = demoData();
        }
    }

    private void Save()
    {
        File.WriteAllBytes(DataFilePath, Program.NetworkManager.ProtoWrapper.ToByteArray());
    }
    
    public void Dispose()
    {
        ManagerList.Dispose();
    }
    
}