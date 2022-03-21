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
    public override Task<Settings> SettingsGet(Empty request, ServerCallContext context)
    {
        Settings settings;
        return Task.FromResult(Program.Settings);
    }

    public override Task<MsgReply> SettingsSet(Settings node, ServerCallContext context)
    {
        // Write to file
        File.WriteAllBytes(Path.Combine(Program.DataPath, "settings.dat"),node.ToByteArray());
        var msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        return Task.FromResult(msgReply);
    }

}