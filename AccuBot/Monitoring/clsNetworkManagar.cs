using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNetworkManagar : IProtoManager<Proto.API.Network,Proto.API.NetworkList>
{
    public clsProtoShadow<clsNetwork,Proto.API.Network> NetworkList { get; init; }
    private readonly String DataFilePath = Path.Combine(Program.DataPath, "networklist");
    public Proto.API.NetworkList ProtoWrapper { get; init; }
    

    public clsNetworkManagar()
    {
        ProtoWrapper = new Proto.API.NetworkList();
        NetworkList = new clsProtoShadow<clsNetwork, Proto.API.Network>(ProtoWrapper.Network,x => x.NetworkID, (x, y) => x.NetworkID = y);
        Load();
    }

    public MsgReply Update(Proto.API.Network network)
    {
        MsgReply msgReply;
        clsNetwork existingNetwork;
        NetworkList.TryGetValue(network.NetworkID, out existingNetwork);
        if (existingNetwork == null) //Incorrect ID sent!
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Network not found" };
        }
        else
        {
            existingNetwork.ProtoMessage.Name = network.Name;
            existingNetwork.ProtoMessage.BlockTime = network.BlockTime;
            existingNetwork.ProtoMessage.StalledAfter = network.StalledAfter;
            existingNetwork.ProtoMessage.NotifictionID = network.NotifictionID;
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return msgReply;
    }

    public MsgReply Add(Network network)
    {
        MsgReply msgReply;
        try
        {
            var id=this.NetworkList.Add(network);
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = id};
        }
        catch (Exception e)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = e.Message};
        }

        return msgReply;
    }

    
    public MsgReply Delete(UInt32 networkId)
    {
        MsgReply msgReply;
        try
        {
            if (NetworkList.Remove(networkId))
            {
                Save();
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok };
            }
            else
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "Network not found" };
            }
        }
        catch (Exception ex)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail,Message = ex.Message};
        }

        return msgReply;
    }

    public void Load()
    {
        Proto.API.NetworkList NetworkListProto;
        if (File.Exists(DataFilePath))
        {
            //Read from file
            NetworkListProto = Proto.API.NetworkList.Parser.ParseFrom(File.ReadAllBytes(DataFilePath));
        }
        else
        {
            NetworkListProto = new NetworkList();
            NetworkListProto.Network.Add(new Network
            {
                NetworkID = 1,
                Name = "Mainnet",
                StalledAfter = 60,
                BlockTime = 600,
                NotifictionID = 0,
            });
            NetworkListProto.Network.Add(new Network
            {
                NetworkID = 2,
                Name = "Testnet",
                StalledAfter = 300,
                BlockTime = 600,
                NotifictionID = 0,
            });
        }

        NetworkList.Add(NetworkListProto.Network);

    }

    private void Save()
    {
        File.WriteAllBytes(DataFilePath, Program.NetworkManager.ProtoWrapper.ToByteArray());
    }
    
    public void Dispose()
    {
    }
    
}