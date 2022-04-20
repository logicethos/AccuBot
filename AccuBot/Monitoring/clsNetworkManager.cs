using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNetworkManager : AManager<clsNetwork,Proto.API.Network,Proto.API.NetworkList>
{

    public ProtoPubSub<Proto.API.NetworkStatus> NetworkStatusSubSub = new ProtoPubSub<NetworkStatus>();
    

    public clsNetworkManager() : base(Path.Combine(Program.DataPath, "networklist"),
                x=>x.Network,
                x=>x.NetworkID,
                (x, y) => x.NetworkID = y)
    {

        base.MapFields = new Action<Proto.API.Network, Proto.API.Network>((origMessage, newMessage) =>
        {
            origMessage.Name = newMessage.Name;
            origMessage.BlockTime = newMessage.BlockTime;
            origMessage.StalledAfter = newMessage.StalledAfter;
            origMessage.NotifictionID = newMessage.NotifictionID;
        });
        Load();
    }

    
    public void Load()
    {
        base.Load(new Func<Proto.API.NetworkList>(() =>
        {
            var NetworkListProto = new NetworkList();
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
            return NetworkListProto;
        }));
    }

    
}