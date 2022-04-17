using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNodeGroupManager : AManager<clsNodeGroup,Proto.API.NodeGroup,Proto.API.NodeGroupList>
{
   
    public clsNodeGroupManager() : base(Path.Combine(Program.DataPath, "nodeGroupList"),
                                   x=>x.NodeGroup,
                                   x=>x.NodeGroupID,
                                   (x, y) => x.NodeGroupID = y)
    {

        base.MapFields = new Action<NodeGroup, NodeGroup>((origMessage, newMessage) =>
        {
            origMessage.Name = newMessage.Name;
            origMessage.NetworkID = newMessage.NetworkID;
            origMessage.HeightNotifictionID = newMessage.HeightNotifictionID;
            origMessage.LatencyNotifictionID = newMessage.LatencyNotifictionID;
            origMessage.PingNotifictionID = newMessage.PingNotifictionID;
        });
        
    }

    

    public void Load()
    {
        base.Load(new Func<NodeGroupList>(() =>
        {
            var nodeGroupList = new NodeGroupList();
            nodeGroupList.NodeGroup.Add(new NodeGroup()
            {
                NodeGroupID = 1,
                Name = "Validator",
                NetworkID = 1,
                PingNotifictionID = 1,
                HeightNotifictionID = 1,
                LatencyNotifictionID = 1
            });
            nodeGroupList.NodeGroup.Add(new NodeGroup
            {
                NodeGroupID = 2,
                Name = "Follower",
                NetworkID = 1,
                PingNotifictionID = 1,
                HeightNotifictionID = 1,
                LatencyNotifictionID = 1
            });
            return nodeGroupList;
        }));
    }
    
    
    public void Dispose()
    {
    }
}