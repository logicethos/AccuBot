using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using Proto.API;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;

namespace AccuBot.GRPC;

public partial class ApiService 
{
     public override Task<NotificationPolicyList> NotificationPolicyListGet(Empty request, ServerCallContext context)
    {
        return Task.FromResult(Program.NotificationPolicyProtoDictionaryShadow.ProtoWrapper);
    }

    public override Task<MsgReply> NotificationPolicySet(NotificationPolicy notificationPolicy, ServerCallContext context)
    {
        MsgReply msgReply;

        if (notificationPolicy.NotifictionID == 0) //id not set, so new network
            msgReply = Program.NotificationPolicyProtoDictionaryShadow.Add(notificationPolicy);
        else
            msgReply = Program.NotificationPolicyProtoDictionaryShadow.Update(notificationPolicy);
        
        return Task.FromResult(msgReply);
    }

    public override Task<MsgReply> NotificationPolicyDelete(ID32 notificationPolicyID, ServerCallContext context)
    {
        return Task.FromResult(Program.NotificationPolicyProtoDictionaryShadow.Delete(notificationPolicyID.ID));
    }
}