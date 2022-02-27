using AccuTest;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace AccuTest;


static public class Users
{
    public static void Test()
    {


        GetUsers();
        var newUser = AddUser();
        GetUsers();
        
        //Delete the user we added.
        Console.WriteLine("Delete User:");
        var reply2 = Program.GRPCClient.API.UserDelete(new ID32() { ID = newUser.UserID});
        
        GetUsers();
        
    }

    public static void GetUsers()
    {
        //*** Get users
        Console.WriteLine("Get Users:");
        var users = Program.GRPCClient.API.UserListGet(new Empty());

        //Display users
        foreach (var user in users.Users)
        {
            Console.WriteLine($"Name: {user.Name}  email: {user.Email}");
        }
    }

    public static AccuBotCommon.Proto.User AddUser()
    {
        //*** Add a user
        Console.WriteLine("Add User:");
        var newUser = new AccuBotCommon.Proto.User
        {
            UserID = 0, //Set as zero when adding.  Or the original UserID when updating
            Name = "Fred Smith",
            Email = "fred@email.com",
            Tel = "+44 12345678",
            Discord = "@fred"
        };
        var reply = Program.GRPCClient.API.UserSet(newUser);

        if (reply.Status == MsgReply.Types.Status.Ok)
        {
            newUser.UserID = reply.NewID32; //The server returns the new ID.
            Console.WriteLine($"new user id : {newUser.UserID}");
            return newUser;
        }
        else
        {
            Console.WriteLine(reply.Message); //Error
            return null;
        }
    }
    
}