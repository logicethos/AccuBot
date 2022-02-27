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
        if (File.Exists(Path.Combine(path,"settings")))
        {  //Read from file
            settings = Settings.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(path, "settings")));
        }
        else
        {   //Fill with defaults
            settings = new Settings()
            {
                BotName = "My Bot",
                DiscordClientID = "#123",
                DiscordToken = "uiygweduyg",
                AccumulateOperatorAlertsCh = 443025488655417364,
                DiscordAlertsChannel = "#Bot-Alerts",
                SIPUsername = "",
                SIPPassword = "",
                SIPHost = "",
                SIPCallingNumber = "",
                TwimletURL = "",
                AlarmOffWarningMinutes = 30,
                LatencyTriggerMultiplier = 2,
                BotCommandPrefix = "!",
                EmailSMTPHost = "",
                EmailSMTPPort = 587,
                EmailUsername = "",
                EmailPassword = "",
                EmailFromAddress = "",
            };
        }
        return Task.FromResult(settings);
    }

    public override Task<MsgReply> SettingsSet(Settings node, ServerCallContext context)
    {
        // Write to file
        File.WriteAllBytes(Path.Combine(path, "settings"),node.ToByteArray());
        var msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        return Task.FromResult(msgReply);
    }

}