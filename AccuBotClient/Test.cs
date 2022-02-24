using System;
using AccuBotCommon;
using AccuBotCommon.Proto;

namespace AccuBotClient;

static public class Test
{
    public static void test()
    {

        //*** Get users
        var users = Program.GRPCClient.API.UserListGet(null);

        //Display users    
        foreach (var user in users.Users)
        {
            Console.WriteLine($"Name: {user.Name}  email: {user.Email}");
        }

        //*** Add a user
        var newUser = new AccuBotCommon.Proto.User
        {
            UserID = 0, //Set as zero when adding.  Or the original UserID when updating
            Name = "Fred Smith",
            Password = "abcdef",  //Need to hash this at some point. We do security at a later date.
            TwoFAtype = User.Types.twoFAtype.Yubikey,
            TwoFAData = null,
            Email = "fred@email.com",
            Tel = "+44 12345678",
            Discord = "@fred"
        };
        var reply = Program.GRPCClient.API.UserSet(newUser);

        if (reply.Status == MsgReply.Types.Status.Ok)
        {
            newUser.UserID = reply.NewID32; //The server returns the new ID.
        }
        else
        {
            Console.WriteLine(reply.Message); //Error
        }
    }
}