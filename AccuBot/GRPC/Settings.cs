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
    public override Task<Settings> SettingsGet(Empty request, ServerCallContext context)
    {
        return Task.FromResult(Program.Settings);
    }

    public override Task<MsgReply> SettingsSet(Settings settings, ServerCallContext context)
    {
        MsgReply msgReply;
        try
        {
            clsSettings.Save();
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        }
        catch (Exception e)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = e.Message};
        }
        
        return Task.FromResult(msgReply);
    }

}