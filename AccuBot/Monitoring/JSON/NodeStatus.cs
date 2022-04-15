namespace Accubot.NodeJson;

public class ProtocolVersion
    {
        public string p2p { get; set; }
        public string block { get; set; }
        public string app { get; set; }
    }

    public class Other
    {
        public string tx_index { get; set; }
        public string rpc_address { get; set; }
    }

    public class NodeInfo
    {
        public ProtocolVersion protocol_version { get; set; }
        public string id { get; set; }
        public string listen_addr { get; set; }
        public string network { get; set; }
        public string version { get; set; }
        public string channels { get; set; }
        public string moniker { get; set; }
        public Other other { get; set; }
    }

    public class SyncInfo
    {
        public string latest_block_hash { get; set; }
        public string latest_app_hash { get; set; }
        public string latest_block_height { get; set; }
        public string latest_block_time { get; set; }
        public string earliest_block_hash { get; set; }
        public string earliest_app_hash { get; set; }
        public string earliest_block_height { get; set; }
        public string earliest_block_time { get; set; }
        public string max_peer_block_height { get; set; }
        public bool catching_up { get; set; }
        public string total_synced_time { get; set; }
        public string remaining_time { get; set; }
    }

    public class PubKey
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class ValidatorInfo
    {
        public string address { get; set; }
        public PubKey pub_key { get; set; }
        public string voting_power { get; set; }
    }

    public class Result
    {
        public NodeInfo node_info { get; set; }
        public SyncInfo sync_info { get; set; }
        public ValidatorInfo validator_info { get; set; }
    }

    public class Root
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public Result result { get; set; }
    }