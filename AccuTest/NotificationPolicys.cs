using AccuTest;
using Proto.API;
using Google.Protobuf.WellKnownTypes;

namespace AccuTest;


static public class NotificationPolicys
{
    public static void Test()
    {

        GetNotificationPolicy();
        var newNotificationPolicy = AddNotificationPolicy();
        GetNotificationPolicy();
        var reply2 = Program.GRPCClient.API.NotificationPolicyDelete(new ID32() { ID = newNotificationPolicy.NotifictionID },Program.GRPCClient.Headers);
        GetNotificationPolicy();
    }

    public static void GetNotificationPolicy()
    {
        //*** Get NotificationPolicy
        Console.WriteLine("Get NotificationPolicy:");
        var notificationPolicy = Program.GRPCClient.API.NotificationPolicyListGet(new Empty(),Program.GRPCClient.Headers);

        //Display NotificationPolicy List
        foreach (var notify in notificationPolicy.NotificationPolicyList_)
        {
            Console.WriteLine($"Name: {notify.Name}");
        }
    }
    
    public static Proto.API.NotificationPolicy AddNotificationPolicy()
    {
        //*** Add a NotificationPolicy
        Console.WriteLine("Add NotificationPolicy:");
        var newPolicy = new Proto.API.NotificationPolicy
        {
            Name = "New Notify",
            Discord = 60,
            Call = 120
        };
        var reply = Program.GRPCClient.API.NotificationPolicySet(newPolicy,Program.GRPCClient.Headers);

        if (reply.Status == MsgReply.Types.Status.Ok)
        {
            newPolicy.NotifictionID = reply.NewID32; //The server returns the new ID.
            Console.WriteLine($"new policy id : {newPolicy.NotifictionID}");
            return newPolicy;
        }
        else
        {
            Console.WriteLine(reply.Message); //Error
            return null;
        }
    }
    
}