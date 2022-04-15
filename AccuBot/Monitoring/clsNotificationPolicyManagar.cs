using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNotificationPolicyManagar : IProtoManager<Proto.API.NotificationPolicy,Proto.API.NotificationPolicyList>
{
    public clsProtoShadow<clsNotificationPolicy,Proto.API.NotificationPolicy> NotificationPolicyList { get; init; }
    private readonly String DataFilePath = Path.Combine(Program.DataPath, "notificationPolicyList");
    public Proto.API.NotificationPolicyList ProtoWrapper { get; init; }
    
    public clsNotificationPolicyManagar()
    {
        ProtoWrapper = new Proto.API.NotificationPolicyList();
        NotificationPolicyList = new clsProtoShadow<clsNotificationPolicy, Proto.API.NotificationPolicy>(ProtoWrapper.NotificationPolicyList_,x => x?.NotifictionID, (x, y) => x.NotifictionID = y);
        Load();
    }

    public MsgReply Update(Proto.API.NotificationPolicy notificationPolicy)
    {
        MsgReply msgReply;
        clsNotificationPolicy existingPolicy;
        
        NotificationPolicyList.TryGetValue(notificationPolicy.NotifictionID, out existingPolicy);
        if (existingPolicy == null) //Incorrect ID sent!
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NotificationPolicy not found" };
        }
        else
        {
            existingPolicy.ProtoMessage.Name = notificationPolicy.Name;
            existingPolicy.ProtoMessage.Call = notificationPolicy.Call;
            existingPolicy.ProtoMessage.Discord = notificationPolicy.Discord;
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return msgReply;
    }

    public MsgReply Add(NotificationPolicy network)
    {
        MsgReply msgReply;
        try
        {
            var id=this.NotificationPolicyList.Add(network);
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = id};
        }
        catch (Exception e)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail,Message = e.Message};
        }

        return msgReply;
    }

    
    public MsgReply Delete(UInt32 policyId)
    {
        MsgReply msgReply;
        try
        {
            if (NotificationPolicyList.Remove(policyId))
            {
                Save();
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
            }
            else
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Notification Policy not found" };
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
        Proto.API.NotificationPolicyList notificationPolicyList;
        if (File.Exists(DataFilePath))
        {  //Read from file
            notificationPolicyList = Proto.API.NotificationPolicyList.Parser.ParseFrom(File.ReadAllBytes(DataFilePath));
        }
        else
        {
            notificationPolicyList = new NotificationPolicyList();
            notificationPolicyList.NotificationPolicyList_.Add(new NotificationPolicy
            {
                NotifictionID = 1,
                Name = "Nothing",
                Discord = -1,
                Call = -1
            });
            notificationPolicyList.NotificationPolicyList_.Add(new NotificationPolicy
            {
                NotifictionID = 2,
                Name = "Warning",
                Discord = 120,
                Call = -1
            });
            notificationPolicyList.NotificationPolicyList_.Add(new NotificationPolicy
            {
                NotifictionID = 3,
                Name = "Alert",
                Discord = 0,
                Call = 600
            });
            notificationPolicyList.NotificationPolicyList_.Add(new NotificationPolicy
            {
                NotifictionID = 4,
                Name = "Panic",
                Discord = 0,
                Call = 0
            });
        }
        NotificationPolicyList.Add(notificationPolicyList.NotificationPolicyList_);

    }
    
    private void Save()
    {
        File.WriteAllBytes(DataFilePath, Program.NetworkManager.ProtoWrapper.ToByteArray());
    }
    
    public void Dispose()
    {
    }
    
}