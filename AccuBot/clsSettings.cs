using System.Xml.Linq;
using Google.Protobuf;
using Proto.API;
namespace AccuBot;

public static class clsSettings
{
    static public string PathSettings = Path.Combine(Program.DataPath, "settings.dat");
    
    
    public static void Load()
    {
#if DEBUG        
        Directory.Delete(Program.DataPath,true);
#endif
        
        Directory.CreateDirectory(Program.DataPath);
        Directory.CreateDirectory(Path.Combine(Program.DataPath, "certs"));
        Directory.CreateDirectory(Path.Combine(Program.DataPath, "www"));
        
        GetSettings();
    }
    
    
    private static void GetSettings()
    {
            
        if (File.Exists(Path.Combine(Program.DataPath,"settings")))
        {  //Read from file
            Program.Settings = Settings.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(Program.DataPath, "settings.dat")));
        }
        else
        {
            //Fill with defaults
            Program.Settings = new Settings()
            {
                BotName = "My Bot",
                DiscordClientID = "955503128705392660",
                DiscordToken = "OTU1NTAzMTI4NzA1MzkyNjYw.Yjinog.ltCgJ9c_-SXlLEDPR7khzTSJBa0",
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
    }

    static public void Save()
    {
        // Write to file
        File.WriteAllBytes(PathSettings,Program.Settings.ToByteArray());
        
    }
    
   
    
}