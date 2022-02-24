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
            var existingNode = exitingNodes.Nodes.First(x => x.NodeID == node.NodeID);
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
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        }
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
            var existingUser = exitingUsers.Users.First(x => x.UserID == user.UserID);
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
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        }
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

    public override Task<MsgReply> NetworkSet(Network request, ServerCallContext context)
    {
        return base.NetworkSet(request, context);
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

    public override Task<MsgReply> NodeGroupSet(NodeGroup request, ServerCallContext context)
    {
        return base.NodeGroupSet(request, context);
    }

    public override Task<NodeGroupList> NodeGroupListGet(Empty request, ServerCallContext context)
    {
        return base.NodeGroupListGet(request, context);
    }

    public override Task<DomainCertificateList> DomainCertificateListGet(Empty request, ServerCallContext context)
    {
        return base.DomainCertificateListGet(request, context);
    }

    public override Task<MsgReply> DomainCertificateSet(DomainCertificate request, ServerCallContext context)
    {
        return base.DomainCertificateSet(request, context);
    }

    public override Task<NotificationPolicyList> NotificationPolicyListGet(Empty request, ServerCallContext context)
    {
        return base.NotificationPolicyListGet(request, context);
    }

    public override Task<MsgReply> NotificationPolicySet(NotificationPolicy request, ServerCallContext context)
    {
        return base.NotificationPolicySet(request, context);
    }
    
}