using AccuTest;
using Proto.API;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace AccuTest;


static public class Settings
{
    public static void Test()
    {
        GetSettings();
        SetSetting();        
        GetSettings();
        
    }

    public static void GetSettings()
    {
        //*** Get Settings
        Console.WriteLine("Get Settings:");
        var Settings = Program.GRPCClient.API.SettingsGet(new Empty(),Program.GRPCClient.Headers);

        Console.WriteLine(Settings.BotName);
        Console.WriteLine(Settings.DiscordClientID);
        Console.WriteLine(Settings.DiscordToken);
        Console.WriteLine(Settings.AccumulateOperatorAlertsCh);
        Console.WriteLine(Settings.DiscordAlertsChannel);
        Console.WriteLine(Settings.SIPUsername);
        Console.WriteLine(Settings.SIPPassword);
        Console.WriteLine(Settings.SIPHost);
        Console.WriteLine(Settings.SIPCallingNumber);
        Console.WriteLine(Settings.TwimletURL);
        Console.WriteLine(Settings.AlarmOffWarningMinutes);
        Console.WriteLine(Settings.LatencyTriggerMultiplier);
        Console.WriteLine(Settings.BotCommandPrefix);
        Console.WriteLine(Settings.EmailSMTPHost);
        Console.WriteLine(Settings.EmailSMTPPort);
        Console.WriteLine(Settings.EmailUsername);
        Console.WriteLine(Settings.EmailPassword);
        Console.WriteLine(Settings.EmailFromAddress);
        

    }

    public static void SetSetting()
    {
        Console.WriteLine("Write Settings:");
        
        var set = new Proto.API.Settings
        {
            BotName = "TestBot",
            DiscordClientID = "TESTID",
            DiscordToken = "DISTOKEN",
            AccumulateOperatorAlertsCh = 1,
            DiscordAlertsChannel = "#Test",
            SIPUsername = "sipuser",
            SIPPassword = "sippassword",
            SIPHost = "host",
            SIPCallingNumber = "sipCalling",
            TwimletURL = "https://twimlet",
            AlarmOffWarningMinutes = 2,
            LatencyTriggerMultiplier = 3,
            BotCommandPrefix = "!",
            EmailSMTPHost = "smtp@smtp.com",
            EmailSMTPPort = 554,
            EmailUsername = "username",
            EmailPassword = "Password",
            EmailFromAddress = "fromEmail",
        };

        var reply = Program.GRPCClient.API.SettingsSet(set,Program.GRPCClient.Headers);

        if (reply.Status == MsgReply.Types.Status.Ok)
        {
            Console.WriteLine($"Setting saved");
            return;
        }
        else
        {
            Console.WriteLine(reply.Message); //Error
            return;
        }
    }
    
}