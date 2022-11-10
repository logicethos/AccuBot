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
    public override Task<MsgReply> NotificationPolicySet(NotificationPolicy notificationPolicy, ServerCallContext context)
    {
        var msgReply = Program.NotificationPolicyProtoDictionaryShadow.AddUpdate(notificationPolicy);

        return Task.FromResult(msgReply);
    }

    public override Task<NotificationPolicyList> NotificationPolicyListGet(Empty request, ServerCallContext context)
    {
        var proto = new NotificationPolicyList();
        Program.NotificationPolicyProtoDictionaryShadow.NotificationPolicy.PopulateRepeatedField(proto.NotificationPolicyList_);
        return Task.FromResult(proto);
    }


    public override Task<MsgReply> NotificationPolicyDelete(ID32 notificationPolicyID, ServerCallContext context)
    {
        return Task.FromResult(Program.NotificationPolicyProtoDictionaryShadow.Delete(notificationPolicyID.ID));
    }
}