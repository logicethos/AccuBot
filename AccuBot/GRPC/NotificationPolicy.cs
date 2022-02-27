using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;

namespace AccuTest.GRPC;

public partial class ApiService 
{
     public override Task<NotificationPolicyList> NotificationPolicyListGet(Empty request, ServerCallContext context)
    {
        NotificationPolicyList notificationPolicyList;
        if (File.Exists(Path.Combine(path,"notificationPolicyList")))
        {  //Read from file
            notificationPolicyList = NotificationPolicyList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(path, "notificationPolicyList")));
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
        return Task.FromResult(notificationPolicyList);
    }

    public override Task<MsgReply> NotificationPolicySet(NotificationPolicy notificationPolicy, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingNotifiactionPolicy = NotificationPolicyListGet(null, null).Result;  //get existing nodes.

        if (notificationPolicy.NotifictionID == 0) //id not set, so new policy
        {
            notificationPolicy.NotifictionID = exitingNotifiactionPolicy.NotificationPolicyList_.Max(x => x.NotifictionID) + 1; //get new id
            exitingNotifiactionPolicy.NotificationPolicyList_.Add(notificationPolicy);
        }
        else
        {
            var existingNotificationPolicy = exitingNotifiactionPolicy.NotificationPolicyList_.FirstOrDefault(x => x.NotifictionID == notificationPolicy.NotifictionID);
            if (existingNotificationPolicy == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NotificationPolicy not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingNotificationPolicy.Name = notificationPolicy.Name;
                existingNotificationPolicy.Call = notificationPolicy.Call;
                existingNotificationPolicy.Discord = notificationPolicy.Discord;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = notificationPolicy.NotifictionID};
        File.WriteAllBytes(Path.Combine(path, "notificationPolicyList"),exitingNotifiactionPolicy.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<MsgReply> NotificationPolicyDelete(ID32 notificationPolicyID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingNotificationPolicy = NotificationPolicyListGet(null, null).Result;  //get existing nodes.

        var notificationPolicy = existingNotificationPolicy.NotificationPolicyList_.FirstOrDefault(x => x.NotifictionID == notificationPolicyID.ID);
        if (notificationPolicy==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NotificationPolicy not found"};
        }
        else
        {
            existingNotificationPolicy.NotificationPolicyList_.Remove(notificationPolicy);
            File.WriteAllBytes(Path.Combine(path, "notificationPolicyList"),existingNotificationPolicy.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }
}