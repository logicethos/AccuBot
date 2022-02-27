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

    public override Task<MsgReply> UserSet(User user, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingUsers = UserListGet(null, null).Result; //get existing user.

        if (user.UserID == 0) //id not set, so new node
        {
            user.UserID = exitingUsers.Users.Max(x => x.UserID) + 1; //get new id
            exitingUsers.Users.Add(user);
        }
        else
        {
            var existingUser = exitingUsers.Users.FirstOrDefault(x => x.UserID == user.UserID);
            if (existingUser == null) //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "User not found" };
                return Task.FromResult(msgReply);
            }
            else
            {
                existingUser.Name = user.Name;
                existingUser.Name = user.Name;
                existingUser.Discord = user.Discord;
                existingUser.Email = existingUser.Email;
                existingUser.Tel = existingUser.Tel;
            }
        }

        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = user.UserID };
        File.WriteAllBytes(Path.Combine(path, "users"), exitingUsers.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<UserList> UserListGet(Empty request, ServerCallContext context)
    {
        UserList userlist = null;
        if (File.Exists(Path.Combine(path, "users")))
        {
            //Read from file
            userlist = UserList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(path, "users")));
        }
        else
        {
            userlist = new UserList();
            userlist.Users.Add(new User()
            {
                UserID = 1,
                Name = "John",
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
        var existingUsers = UserListGet(null, null).Result; //get existing nodes.

        var user = existingUsers.Users.FirstOrDefault(x => x.UserID == userID.ID);
        if (user == null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "User not found" };
        }
        else
        {
            existingUsers.Users.Remove(user);
            File.WriteAllBytes(Path.Combine(path, "users"), existingUsers.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
        }

        return Task.FromResult(msgReply);
    }
}