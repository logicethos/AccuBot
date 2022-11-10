using ProtoHelper;

namespace AccuBot.Monitoring;

public class clsNodeGroup : IProtoShadowClass<UInt32,Proto.API.NodeGroup>
{
    public UInt32 ID => ProtoMessage.NodeGroupID;
    public Proto.API.NodeGroup ProtoMessage { get; init; }

    public clsNodeGroup(Proto.API.NodeGroup group)
    {
        ProtoMessage = group;
    }
    
}