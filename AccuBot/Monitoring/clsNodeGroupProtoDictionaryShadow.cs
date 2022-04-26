using Accubot;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Proto.API;

namespace AccuBot.Monitoring;

public class clsNodeGroupProtoDictionaryShadow : clsProtoDictionaryShadow<clsNodeGroup,Proto.API.NodeGroup,Proto.API.NodeGroupList>
{
   
    public clsNodeGroupProtoDictionaryShadow() : base(Path.Combine(Program.DataPath, "nodeGroupList"),
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
        Load();
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
    
    
}