using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNotificationPolicyManager : AManager<clsNotificationPolicy,Proto.API.NotificationPolicy,Proto.API.NotificationPolicyList>
{

    
    public clsNotificationPolicyManager(): base(Path.Combine(Program.DataPath, "notificationPolicyList"),
        x=>x.NotificationPolicyList_,
        x=>x.NotifictionID,
        (x, y) => x.NotifictionID = y)
    {
        base.MapFields = new Action<NotificationPolicy, NotificationPolicy>((origMessage, newMessage) =>
        {
            origMessage.Name = newMessage.Name;
            origMessage.Call = newMessage.Call;
            origMessage.Discord = newMessage.Discord;
        });
        Load();
    }


    public void Load()
    {
        base.Load(new Func<NotificationPolicyList>(() =>
        {
            var notificationPolicyList = new NotificationPolicyList();
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
            
            return notificationPolicyList;
        }));
        

    }
    
    
}