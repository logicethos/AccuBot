using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;
using ProtoHelper;

namespace AccuBot.Monitoring;

using TIndex = UInt32;
using TProto = Proto.API.NotificationPolicy;
using TProtoS = clsNotificationPolicy;
using TProtoList = Proto.API.NotificationPolicyList;

public class clsNotificationPolicyProtoDictionaryShadow : clsProtoShadowTableIndexed<TProtoS, TProto, TIndex>
{


    private Action<TProto, TProto> MapFields = null;
    
    public clsNotificationPolicyProtoDictionaryShadow(): base(
                        new Func<TProto, IComparable<TIndex>>(x => x.NotifictionID)
                       ,new Action<TProto, TIndex>((x, y) => x.NotifictionID = y))
    {

        Load();
    }


    public TProtoS Add(TProto notificationPolicy)
    {
        return base.Add(notificationPolicy, new clsNotificationPolicy(notificationPolicy));
    }

    public bool Update(TProto nodeGroup)
    {
        return base.Update(nodeGroup,MapFields);
    }


    public MsgReply Delete(TIndex id)
    {
        var msgReply = new MsgReply();
        msgReply.Status = base.Remove(id) ? MsgReply.Types.Status.Ok : MsgReply.Types.Status.Fail;
        return msgReply;
    }


    public MsgReply AddUpdate(NotificationPolicy notificationPolicy)
    {
        var msgReply = new MsgReply();

        if (notificationPolicy.NotifictionID == 0)
        {
            var shadowClass = Add(notificationPolicy);
            if (shadowClass == null)
            {
                msgReply.Status = MsgReply.Types.Status.Fail;
            }
            else
            {
                msgReply.Status = MsgReply.Types.Status.Ok;
                msgReply.NewID32 = shadowClass.ID;
            }
        }
        else
        {
            msgReply.Status = Update(notificationPolicy) ? MsgReply.Types.Status.Ok : MsgReply.Types.Status.Fail;
        }

        return msgReply;
    }

    public void Load()
    {
        //Path.Combine(Program.DataPath, "notificationPolicyList")

       // base.Load(new Func<NotificationPolicyList>(() =>
       // {
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
            
         //   return notificationPolicyList;
        //}));
        

    }

}