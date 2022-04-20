using System;
using System.Linq;
using System.Text;
using Proto.API;

namespace AccuBot
{
    public class clsNetwork : IProtoShadowClass<Proto.API.Network>
    {
        public Proto.API.Network ProtoMessage {get; init;}
        
        public Proto.API.NetworkStatus Status {get; init;}
        
        public uint TopHeight {get; private set;}
        public DateTime? LastHeight  {get; private set;}
        public DateTime? NextHeight  {get; private set;}
        int LateHeightCount;
        
        clsRollingAverage AverageBlocktime = new clsRollingAverage(6);
        TimeSpan? LastBlockTimeDuration;
        bool FullBlockMesured = false;
        public UInt32 ID => ProtoMessage.NetworkID;
        
        public clsAlarm NetworkAlarm = null;
        
        public clsNetwork(Proto.API.Network network)
        {
            ProtoMessage = network;
            Status = new NetworkStatus();
            
        }
        
        public void SetTopHeight(uint height)
        {
            TopHeight = height;
            
            if (LastHeight.HasValue)
            {
                if (FullBlockMesured)
                {
                    LastBlockTimeDuration = (DateTime.UtcNow - LastHeight.Value);
                    AverageBlocktime.Add((int)LastBlockTimeDuration.Value.TotalSeconds);
                }
                else
                {
                    FullBlockMesured=true;
                }
            }
            
            LastHeight = DateTime.UtcNow;           
            NextHeight = LastHeight.Value.AddSeconds( (int)ProtoMessage.BlockTime ); //ToDo rename to BlockTimeSeconds
            
            if (LateHeightCount>0)
            {
                LateHeightCount=0;
                Program.AlarmManager.Clear(NetworkAlarm, $"CLEARED: Network Stall Alarm {ProtoMessage.Name}");
                NetworkAlarm = null;
            }
        }
        
        public void CheckStall()
        {
            if (NextHeight.HasValue && DateTime.UtcNow > NextHeight.Value)
            {
                var seconds = (DateTime.UtcNow - NextHeight).Value.TotalSeconds;
                if (seconds>ProtoMessage.StalledAfter)  //ToDo rename to StalledAfterSeconds
                {
                    if (MonitoringSourceCount==0)  //No data sources, so we need to cancel 
                    {
                        NextHeight=null;
                        if (NetworkAlarm!=null)
                        {
                           Program.AlarmManager.Remove(NetworkAlarm);
                           NetworkAlarm = null;
                        }
                    }
                    else if (++LateHeightCount==1)
                    {
                        NetworkAlarm = new clsAlarm(clsAlarm.enumAlarmType.Network,$"WARNING: {ProtoMessage.Name} stall or an election? Network Height {seconds:0} sec late.",this);
                        Program.AlarmManager.New(NetworkAlarm);
                    }
                }
            }
        }
        
        public uint MonitoringSourceCount
        {
            get
            {
                return (uint)Program.NodeManager.ManagerList.Count(x=>x.Value.Network == this && x.Value.ProtoMessage.Monitor);
            }
        }
        
        
        public void AppendDisplayColumns(ref clsColumnDisplay columnDisplay)
        {
            var sources = MonitoringSourceCount;
        
            columnDisplay.AppendCol(ProtoMessage.Name ?? "?");
            columnDisplay.AppendCol($"{TopHeight:#;;'n/a'}");
            columnDisplay.AppendCol($"{sources}");
            
            if (MonitoringSourceCount==0)
            {
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a - no data sources available");
            }
            else if (FullBlockMesured)
            {
                columnDisplay.AppendCol(LastHeight.HasValue ? $"{LastHeight.Value:HH:mm:ss}":"n/a");
                columnDisplay.AppendCol(NextHeight.HasValue ? $"{(NextHeight.Value - DateTime.UtcNow).ToMSDisplay()}":"n/a");
                
                var sb = new StringBuilder();
                foreach (var bt in AverageBlocktime.GetValues().Reverse().Take(3))
                {
                    if (sb.Length>0) sb.Append("<");
                    sb.Append($"[{new TimeSpan(0,0,bt).ToMSDisplay()}]");
                }
                
                if (MonitoringSourceCount==1) sb.Append(" (only one data source)");
                
                columnDisplay.AppendCol(sb.ToString());
            }
            else
            {
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a - please wait for next block");
            }
        }
    }
}
