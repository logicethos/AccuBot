using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public abstract class clsManagar<TClass,TPClass,TPClassList> : IProtoManager<TPClass,TPClassList> where TClass:IProtoShadowClass<TPClass> where TPClass:IMessage where TPClassList:IMessage
{
    public clsProtoShadow<TClass,TPClass> ManagerList { get; init; }
    public String DataFilePath { get; init; }
    public TPClassList ProtoWrapper { get; init; }
    private Action<TPClass, UInt32> IndexSelectorWrite;
    private Func<TPClass, IComparable<UInt32>> IndexSelector;
    

    public clsManagar(string path,Func<TPClass, IComparable<RepeatedField<TPClass>>> listSelector,Func<TPClass, IComparable<UInt32>> indexSelector, Action<TPClass, UInt32> indexSelectorWrite)
    {
        IndexSelector = indexSelector;
        IndexSelectorWrite = indexSelectorWrite;
        DataFilePath = path;
        ProtoWrapper = new TPClassList();
        ManagerList = new clsProtoShadow<TClass, TPClass>(listSelector,indexSelector, indexSelectorWrite);
        Load();
    }

    public MsgReply Update(TPClass network)
    {
        MsgReply msgReply;
        TClass existingNetwork;
        ManagerList.TryGetValue((UInt32)IndexSelector(network), out existingNetwork);
        if (existingNetwork == null) //Incorrect ID sent!
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Network not found" };
        }
        else
        {
            existingNetwork.ProtoMessage.Name = network.Name;
            existingNetwork.ProtoMessage.BlockTime = network.BlockTime;
            existingNetwork.ProtoMessage.StalledAfter = network.StalledAfter;
            existingNetwork.ProtoMessage.NotifictionID = network.NotifictionID;
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
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Network not found" };
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
        TPClassList NetworkListProto;
        if (File.Exists(DataFilePath))
        {
            //Read from file
            NetworkListProto = TPClassList.Parser.ParseFrom(File.ReadAllBytes(DataFilePath));
        }

        ManagerList.Add(NetworkListProto.Network);

    }

    private void Save()
    {
        File.WriteAllBytes(DataFilePath, Program.NetworkManager.ProtoWrapper.ToByteArray());
    }
    
    public void Dispose()
    {
    }
    
}