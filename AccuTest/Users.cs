using AccuTest;
using Proto.API;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace AccuTest;


static public class Users
{
    public static void Test()
    {


        GetUsers();
       
        var newUser = AddUser();
        var updateUser = UpdateUser(newUser.UserID);
        
       //var reply2 = Program.GRPCClient.API.UserDelete(new ID32() { ID = 3},Program.GRPCClient.Headers);
        GetUsers();
       
        //Delete the user we added.
        Console.WriteLine("Delete User:");
        var reply2 = Program.GRPCClient.API.UserDelete(new ID32() { ID = newUser.UserID},Program.GRPCClient.Headers);
        
        GetUsers();
        
    }

    public static void GetUsers()
    {
        //*** Get users
        Console.WriteLine("Get Users:");
        var users = Program.GRPCClient.API.UserListGet(new Empty(),Program.GRPCClient.Headers);

        //Display users
        foreach (var user in users.Users)
        {
            Console.WriteLine($"Id: {user.UserID} Name: {user.Name}  email: {user.Email}");
        }
    }

    public static Proto.API.User AddUser()
    {
        //*** Add a user
        Console.WriteLine("Add User:");
        var newUser = new Proto.API.User
        {
            UserID = 0, //Set as zero when adding.  Or the original UserID when updating
            Name = "Fred Smith",
            Email = "fred@email.com",
            Tel = "+44 12345678",
            Discord = "@fred"
        };
        
        var reply = Program.GRPCClient.API.UserSet(newUser,Program.GRPCClient.Headers);

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

    public static Proto.API.User UpdateUser(uint id)
    {
        //*** Add a user
        Console.WriteLine("Update User:");
        var newUser = new Proto.API.User
        {
            UserID = id, //Set as zero when adding.  Or the original UserID when updating
            Name = "Fred Sausage",
            Email = "fred@email.com",
            Tel = "+44 1234eer5678",
            Discord = "@fred"
        };
        
        var reply = Program.GRPCClient.API.UserSet(newUser,Program.GRPCClient.Headers);

        if (reply.Status == MsgReply.Types.Status.Ok)
        {
            newUser.UserID = reply.NewID32; //The server returns the new ID.
            Console.WriteLine($"Update user id : {newUser.UserID}");
            return newUser;
        }
        else
        {
            Console.WriteLine(reply.Message); //Error
            return null;
        }
    }

    
}