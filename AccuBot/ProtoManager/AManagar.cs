using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public abstract class AManagar<TClass,TPClass,TPClassList> : IProtoManager<TPClass,TPClassList> where TClass:IProtoShadowClass<TPClass> where TPClass:IMessage<TPClass> where TPClassList:IMessage<TPClassList>
{
    public clsProtoShadow<TClass,TPClass> ManagerList { get; init; }
    public String DataFilePath { get; init; }
    public TPClassList ProtoWrapper { get; init; }
    private Action<TPClass, UInt32> IndexSelectorWrite;
    private Func<TPClass, IComparable<UInt32>> IndexSelector;
    private Func<TPClassList, IComparable<RepeatedField<TPClass>>> RepeatedFieldSelector;

    public Action<TPClass, TPClass> MapFields { get; set; } = null;

    public AManagar(string path,Func<TPClassList, IComparable<RepeatedField<TPClass>>> repeatedFieldSelector, Func<TPClass, IComparable<RepeatedField<TPClass>>> listSelector,Func<TPClass, IComparable<UInt32>> indexSelector, Action<TPClass, UInt32> indexSelectorWrite)
    {
        DataFilePath = path;
        IndexSelector = indexSelector;              //Index field of our proto message
        IndexSelectorWrite = indexSelectorWrite;    //Write action for index field.
        RepeatedFieldSelector = repeatedFieldSelector;
        
        ProtoWrapper = (TPClassList)Activator.CreateInstance(typeof(TPClassList));

        var repeatedfield = repeatedFieldSelector(ProtoWrapper) as RepeatedField<TPClass>;

        ManagerList = new clsProtoShadow<TClass, TPClass>(repeatedfield, indexSelector, indexSelectorWrite);
        Load();
    }

    public MsgReply Update(TPClass network)
    {
        MsgReply msgReply;
        TClass existingNetwork;
        ManagerList.TryGetValue((UInt32)IndexSelector(network), out existingNetwork);
        if (existingNetwork == null) //Incorrect ID sent!
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = $"{typeof(TPClass).FullName} not found" };
        }
        else
        {
            ManagerList.Update(network,MapFields);
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return msgReply;
    }

    public MsgReply Add(TPClass network)
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
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = $"{typeof(TPClass).FullName} not found" };
            }
        }
        catch (Exception ex)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail,Message = ex.Message};
        }

        return msgReply;
    }

    public void Load()
    {
        TPClassList networkListProto;
        if (File.Exists(DataFilePath))
        {
            //Read from file
           // networkListProto = TPClassList.Parser.ParseFrom(File.ReadAllBytes(DataFilePath));
            
            var par = new Google.Protobuf.MessageParser<TPClassList>(() => ProtoWrapper);
            networkListProto = par.ParseFrom(File.ReadAllBytes(DataFilePath));
            
            ManagerList.Add(RepeatedFieldSelector(networkListProto) as RepeatedField<TPClass>);    
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