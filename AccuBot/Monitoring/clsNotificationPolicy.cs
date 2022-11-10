
using ProtoHelper;
namespace AccuBot.Monitoring;



public class clsNotificationPolicy : IProtoShadowClass<UInt32,Proto.API.NotificationPolicy>
{
    public UInt32 ID => ProtoMessage.NotifictionID;
    public Proto.API.NotificationPolicy ProtoMessage { get; init; }

    public clsNotificationPolicy(Proto.API.NotificationPolicy policy)
    {
        ProtoMessage = policy;
    }
}