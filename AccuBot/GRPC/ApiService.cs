using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;


namespace AccuBot.GRPC;

public class ApiService : AccuBotAPI.AccuBotAPIBase
{
    public static ApiService instance;

    public override Task<Settings> SettingsGet(Empty request, ServerCallContext context)
    {
        Settings settings;
        if (File.Exists(Path.Combine("path","settings")))
        {  //Read from file
            settings = Settings.Parser.ParseFrom(File.ReadAllBytes(Path.Combine("path", "settings")));
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
        File.WriteAllBytes(Path.Combine("path", "settings"),node.ToByteArray());
        var msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        return Task.FromResult(msgReply);
    }
    
    public override Task<MsgReply> NodeSet(Node node, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingNodes = NodeListGet(null, null).Result;  //get existing nodes.

        if (node.NodeID == 0) //id not set, so new node
        {
            node.NodeID = exitingNodes.Nodes.Max(x => x.NodeID) + 1; //get new id
            exitingNodes.Nodes.Add(node);
        }
        else
        {
            var existingNode = exitingNodes.Nodes.FirstOrDefault(x => x.NodeID == node.NodeID);
            if (existingNode == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingNode.Host = node.Host;
                existingNode.Monitor = node.Monitor;
                existingNode.Name = node.Name;
                existingNode.NodeGroup = node.NodeGroup;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = node.NodeID};
        File.WriteAllBytes(Path.Combine("path", "settings"),exitingNodes.ToByteArray());
        return Task.FromResult(msgReply);
    }
    
    public override Task<NodeList> NodeListGet(Empty request, ServerCallContext context)
    {
        NodeList nodelist = null;
        if (File.Exists(Path.Combine("path","nodes")))
        {  //Read from file
            var nodes = Settings.Parser.ParseFrom(File.ReadAllBytes(Path.Combine("path", "nodes")));
        }
        else
        {
            nodelist = new NodeList();
            nodelist.Nodes.Add( new Node()
            {
                NodeGroup = "MainNet",
                Name = "my node",
                Host = "123.123.123.123",
                Monitor = true,
            });
        }

        return Task.FromResult(nodelist);
    }

    public override Task<MsgReply> NodeDelete(ID32 nodeID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingNodes = NodeListGet(null, null).Result;  //get existing nodes.

        var node = existingNodes.Nodes.FirstOrDefault(x => x.NodeID == nodeID.ID);
        if (node==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node not found"};
        }
        else
        {
            existingNodes.Nodes.Remove(node);
            File.WriteAllBytes(Path.Combine("path", "nodes"),existingNodes.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }

    public override Task<MsgReply> UserSet(User user, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingUsers = UserListGet(null, null).Result;  //get existing user.

        if (user.UserID == 0) //id not set, so new node
        {
            user.UserID = exitingUsers.Users.Max(x => x.UserID) + 1; //get new id
            exitingUsers.Users.Add(user);
        }
        else
        {
            var existingUser = exitingUsers.Users.FirstOrDefault(x => x.UserID == user.UserID);
            if (existingUser == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "User not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingUser.Name = user.Name;
                existingUser.Password = user.Password;
                existingUser.Name = user.Name;
                existingUser.Discord = user.Discord;
                existingUser.Email = existingUser.Email;
                existingUser.Tel = existingUser.Tel;
                existingUser.TwoFAtype = existingUser.TwoFAtype;
                existingUser.TwoFAData = existingUser.TwoFAData;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = user.UserID };
        File.WriteAllBytes(Path.Combine("path", "users"),exitingUsers.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<UserList> UserListGet(Empty request, ServerCallContext context)
    {
        UserList userlist = null;
        if (File.Exists(Path.Combine("path","userlist")))
        {  //Read from file
            userlist = UserList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine("path", "userlist")));
        }
        else
        {
            userlist = new UserList();
            userlist.Users.Add(new User()
            {
                UserID = 1,
                Name = "John",
                Password = "wewefwef",
                TwoFAtype = User.Types.twoFAtype.Yubikey,
                //TwoFAData ;
                Email = "john@myemail.com",
                Tel = "+44 123456789",
                Discord = "@john"
            });
        }

        return Task.FromResult(userlist);
    }

    public override Task<MsgReply> UserDelete(ID32 userID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingUsers = UserListGet(null, null).Result;  //get existing nodes.

        var user = existingUsers.Users.FirstOrDefault(x => x.UserID == userID.ID);
        if (user==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "User not found"};
        }
        else
        {
            existingUsers.Users.Remove(user);
            File.WriteAllBytes(Path.Combine("path", "userlist"),existingUsers.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }

    public override Task<MsgReply> NetworkSet(Network network, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingNetworks = NetworkListGet(null, null).Result;  //get existing nodes.

        if (network.NetworkID == 0) //id not set, so new network
        {
            network.NetworkID = exitingNetworks.Network.Max(x => x.NetworkID) + 1; //get new id
            exitingNetworks.Network.Add(network);
        }
        else
        {
            var existingNetwork = exitingNetworks.Network.FirstOrDefault(x => x.NetworkID == network.NetworkID);
            if (existingNetwork == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Node not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingNetwork.Name = network.Name;
                existingNetwork.BlockTime = network.BlockTime;
                existingNetwork.StalledAfter = network.StalledAfter;
                existingNetwork.NotifictionID = network.NotifictionID;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = network.NetworkID};
        File.WriteAllBytes(Path.Combine("path", "networklist"),exitingNetworks.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<NetworkList> NetworkListGet(Empty request, ServerCallContext context)
    {
        NetworkList networklist = null;
        if (File.Exists(Path.Combine("path","networklist")))
        {  //Read from file
            networklist = NetworkList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine("path", "networklist")));
        }
        else
        {
            networklist = new NetworkList();
            networklist.Network.Add(new Network
            {
                NetworkID = 1,
                Name = "Mainnet",
                StalledAfter = 60,
                BlockTime = 600,
                NotifictionID = 0,
            });
            networklist.Network.Add(new Network
            {
                NetworkID = 2,
                Name = "Testnet",
                StalledAfter = 300,
                BlockTime = 600,
                NotifictionID = 0,
            });
        }
        return Task.FromResult(networklist);
    }

    public override Task<MsgReply> NetworkDelete(ID32 networkID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingNetworks = NetworkListGet(null, null).Result;  //get existing nodes.

        var user = existingNetworks.Network.FirstOrDefault(x => x.NetworkID == networkID.ID);
        if (user==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Network not found"};
        }
        else
        {
            existingNetworks.Network.Remove(user);
            File.WriteAllBytes(Path.Combine("path", "networklist"),existingNetworks.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }


    public override Task<MsgReply> NodeGroupSet(NodeGroup group, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingNodeGroup = NodeGroupListGet(null, null).Result;  //get existing nodes.

        if (group.NodeGroupID == 0) //id not set, so new nodegroup
        {
            group.NetworkID = exitingNodeGroup.NodeGroup.Max(x => x.NodeGroupID) + 1; //get new id
            exitingNodeGroup.NodeGroup.Add(group);
        }
        else
        {
            var existingNetwork = exitingNodeGroup.NodeGroup.FirstOrDefault(x => x.NetworkID == group.NetworkID);
            if (existingNetwork == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NodeGroup not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingNetwork.Name = group.Name;
                existingNetwork.NetworkID = group.NetworkID;
                existingNetwork.HeightNotifictionID = group.HeightNotifictionID;
                existingNetwork.LatencyNotifictionID = group.LatencyNotifictionID;
                existingNetwork.PingNotifictionID = group.PingNotifictionID;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = group.NetworkID};
        File.WriteAllBytes(Path.Combine("path", "nodeGrouplist"),exitingNodeGroup.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<NodeGroupList> NodeGroupListGet(Empty request, ServerCallContext context)
    {
        NodeGroupList groupList;
        if (File.Exists(Path.Combine("path","networklist")))
        {  //Read from file
            groupList = NodeGroupList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine("path", "nodeGrouplist")));
        }
        else
        {
            groupList = new NodeGroupList();
            groupList.NodeGroup.Add(new NodeGroup()
            {
                NodeGroupID = 1,
                Name = "Validator",
                NetworkID = 1,
                PingNotifictionID = 1,
                HeightNotifictionID = 1,
                LatencyNotifictionID = 1
            });
            groupList.NodeGroup.Add(new NodeGroup
            {
                NodeGroupID = 2,
                Name = "Follower",
                NetworkID = 1,
                PingNotifictionID = 1,
                HeightNotifictionID = 1,
                LatencyNotifictionID = 1
            });
        }
        return Task.FromResult(groupList);
    }

    public override Task<MsgReply> NodeGroupDelete(ID32 nodeGroupID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingNodeGroup = NodeGroupListGet(null, null).Result;  //get existing nodes.

        var nodegroup = existingNodeGroup.NodeGroup.FirstOrDefault(x => x.NetworkID == nodeGroupID.ID);
        if (nodegroup==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NodeGroup not found"};
        }
        else
        {
            existingNodeGroup.NodeGroup.Remove(nodegroup);
            File.WriteAllBytes(Path.Combine("path", "nodeGrouplist"),existingNodeGroup.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }

    public override Task<DomainCertificateList> DomainCertificateListGet(Empty request, ServerCallContext context)
    {
        DomainCertificateList domainCertificateList;
        if (File.Exists(Path.Combine("path","domainCertificateList")))
        {  //Read from file
            domainCertificateList = DomainCertificateList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine("path", "domainCertificateList")));
        }
        else
        {
            domainCertificateList = new DomainCertificateList();
            domainCertificateList.DomainCertificate.Add(new DomainCertificate
            {
                DomainCertificateID = 1,
                Domain = "mydomain.com",
                Monitor = false
            });
        }
        return Task.FromResult(domainCertificateList);
    }

    public override Task<MsgReply> DomainCertificateSet(DomainCertificate domainCertificate, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingDomainCertificate = DomainCertificateListGet(null, null).Result;  //get existing nodes.

        if (domainCertificate.DomainCertificateID == 0) //id not set, so new cert
        {
            domainCertificate.DomainCertificateID = exitingDomainCertificate.DomainCertificate.Max(x => x.DomainCertificateID) + 1; //get new id
            exitingDomainCertificate.DomainCertificate.Add(domainCertificate);
        }
        else
        {
            var existingDomainCertificate = exitingDomainCertificate.DomainCertificate.FirstOrDefault(x => x.DomainCertificateID == domainCertificate.DomainCertificateID);
            if (existingDomainCertificate == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "DomainCertificate not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingDomainCertificate.Domain = domainCertificate.Domain;
                existingDomainCertificate.Monitor = domainCertificate.Monitor;
                existingDomainCertificate.DomainCertificateID = domainCertificate.DomainCertificateID;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = domainCertificate.DomainCertificateID};
        File.WriteAllBytes(Path.Combine("path", "domainCertificateList"),exitingDomainCertificate.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<MsgReply> DomainCertificateDelete(ID32 domainCertificateID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingDomainCertificate = DomainCertificateListGet(null, null).Result;  //get existing nodes.

        var domainCertificate = existingDomainCertificate.DomainCertificate.FirstOrDefault(x => x.DomainCertificateID == domainCertificateID.ID);
        if (domainCertificate==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "DomainCertificate not found"};
        }
        else
        {
            existingDomainCertificate.DomainCertificate.Remove(domainCertificate);
            File.WriteAllBytes(Path.Combine("path", "domainCertificateList"),existingDomainCertificate.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }

    public override Task<NotificationPolicyList> NotificationPolicyListGet(Empty request, ServerCallContext context)
    {
        NotificationPolicyList notificationPolicyList;
        if (File.Exists(Path.Combine("path","notificationPolicyList")))
        {  //Read from file
            notificationPolicyList = NotificationPolicyList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine("path", "notificationPolicyList")));
        }
        else
        {
            notificationPolicyList = new NotificationPolicyList();
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
            
        }
        return Task.FromResult(notificationPolicyList);
    }

    public override Task<MsgReply> NotificationPolicySet(NotificationPolicy notificationPolicy, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingNotifiactionPolicy = NotificationPolicyListGet(null, null).Result;  //get existing nodes.

        if (notificationPolicy.NotifictionID == 0) //id not set, so new policy
        {
            notificationPolicy.NotifictionID = exitingNotifiactionPolicy.NotificationPolicyList_.Max(x => x.NotifictionID) + 1; //get new id
            exitingNotifiactionPolicy.NotificationPolicyList_.Add(notificationPolicy);
        }
        else
        {
            var existingNotificationPolicy = exitingNotifiactionPolicy.NotificationPolicyList_.FirstOrDefault(x => x.NotifictionID == notificationPolicy.NotifictionID);
            if (existingNotificationPolicy == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NotificationPolicy not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingNotificationPolicy.Name = notificationPolicy.Name;
                existingNotificationPolicy.Call = notificationPolicy.Call;
                existingNotificationPolicy.Discord = notificationPolicy.Discord;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = notificationPolicy.NotifictionID};
        File.WriteAllBytes(Path.Combine("path", "notificationPolicyList"),exitingNotifiactionPolicy.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<MsgReply> NotificationPolicyDelete(ID32 notificationPolicyID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingNotificationPolicy = NotificationPolicyListGet(null, null).Result;  //get existing nodes.

        var notificationPolicy = existingNotificationPolicy.NotificationPolicyList_.FirstOrDefault(x => x.NotifictionID == notificationPolicyID.ID);
        if (notificationPolicy==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "NotificationPolicy not found"};
        }
        else
        {
            existingNotificationPolicy.NotificationPolicyList_.Remove(notificationPolicy);
            File.WriteAllBytes(Path.Combine("path", "notificationPolicyList"),existingNotificationPolicy.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }
}