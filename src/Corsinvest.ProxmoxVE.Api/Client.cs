using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api
{
    /// <summary>
    /// Proxmox VE Client
    /// </summary>
    public class Client : ClientBase
    {
#pragma warning disable 1591
        private readonly Client _client;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Client(string hostname, int port = 8006) : base(hostname, port)
        {
            _client = this;
        }

        private PVECluster _cluster;
        public PVECluster Cluster => _cluster ?? (_cluster = new PVECluster(_client));
        private PVENodes _nodes;
        public PVENodes Nodes => _nodes ?? (_nodes = new PVENodes(_client));
        private PVEStorage _storage;
        public PVEStorage Storage => _storage ?? (_storage = new PVEStorage(_client));
        private PVEAccess _access;
        public PVEAccess Access => _access ?? (_access = new PVEAccess(_client));
        private PVEPools _pools;
        public PVEPools Pools => _pools ?? (_pools = new PVEPools(_client));
        private PVEVersion _version;
        public PVEVersion Version => _version ?? (_version = new PVEVersion(_client));
        public class PVECluster
        {
            private readonly Client _client;

            internal PVECluster(Client client) { _client = client; }
            private PVEReplication _replication;
            public PVEReplication Replication => _replication ?? (_replication = new PVEReplication(_client));
            private PVEConfig _config;
            public PVEConfig Config => _config ?? (_config = new PVEConfig(_client));
            private PVEFirewall _firewall;
            public PVEFirewall Firewall => _firewall ?? (_firewall = new PVEFirewall(_client));
            private PVEBackup _backup;
            public PVEBackup Backup => _backup ?? (_backup = new PVEBackup(_client));
            private PVEHa _ha;
            public PVEHa Ha => _ha ?? (_ha = new PVEHa(_client));
            private PVEAcme _acme;
            public PVEAcme Acme => _acme ?? (_acme = new PVEAcme(_client));
            private PVELog _log;
            public PVELog Log => _log ?? (_log = new PVELog(_client));
            private PVEResources _resources;
            public PVEResources Resources => _resources ?? (_resources = new PVEResources(_client));
            private PVETasks _tasks;
            public PVETasks Tasks => _tasks ?? (_tasks = new PVETasks(_client));
            private PVEOptions _options;
            public PVEOptions Options => _options ?? (_options = new PVEOptions(_client));
            private PVEStatus _status;
            public PVEStatus Status => _status ?? (_status = new PVEStatus(_client));
            private PVENextid _nextid;
            public PVENextid Nextid => _nextid ?? (_nextid = new PVENextid(_client));
            private PVECeph _ceph;
            public PVECeph Ceph => _ceph ?? (_ceph = new PVECeph(_client));
            public class PVEReplication
            {
                private readonly Client _client;

                internal PVEReplication(Client client) { _client = client; }
                public PVEItemId this[object id] => new PVEItemId(_client, id);
                public class PVEItemId
                {
                    private readonly Client _client;
                    private readonly object _id;
                    internal PVEItemId(Client client, object id) { _client = client; _id = id; }
                    /// <summary>
                    /// Mark replication job for removal.
                    /// </summary>
                    /// <param name="force">Will remove the jobconfig entry, but will not cleanup.</param>
                    /// <param name="keep">Keep replicated data at target (do not remove).</param>
                    /// <returns></returns>
                    public Result DeleteRest(bool? force = null, bool? keep = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        parameters.Add("keep", keep);
                        return _client.Delete($"/cluster/replication/{_id}", parameters);
                    }

                    /// <summary>
                    /// Mark replication job for removal.
                    /// </summary>
                    /// <param name="force">Will remove the jobconfig entry, but will not cleanup.</param>
                    /// <param name="keep">Keep replicated data at target (do not remove).</param>
                    /// <returns></returns>
                    public Result Delete(bool? force = null, bool? keep = null) => DeleteRest(force, keep);
                    /// <summary>
                    /// Read replication job configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/replication/{_id}"); }

                    /// <summary>
                    /// Read replication job configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result Read() => GetRest();
                    /// <summary>
                    /// Update replication job configuration.
                    /// </summary>
                    /// <param name="comment">Description.</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="disable">Flag to disable/deactivate the entry.</param>
                    /// <param name="rate">Rate limit in mbps (megabytes per second) as floating point number.</param>
                    /// <param name="remove_job">Mark the replication job for removal. The job will remove all local replication snapshots. When set to 'full', it also tries to remove replicated volumes on the target. The job then removes itself from the configuration file.
                    ///   Enum: local,full</param>
                    /// <param name="schedule">Storage replication schedule. The format is a subset of `systemd` calender events.</param>
                    /// <param name="source">Source of the replication.</param>
                    /// <returns></returns>
                    public Result SetRest(string comment = null, string delete = null, string digest = null, bool? disable = null, int? rate = null, string remove_job = null, string schedule = null, string source = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        parameters.Add("delete", delete);
                        parameters.Add("digest", digest);
                        parameters.Add("disable", disable);
                        parameters.Add("rate", rate);
                        parameters.Add("remove_job", remove_job);
                        parameters.Add("schedule", schedule);
                        parameters.Add("source", source);
                        return _client.Set($"/cluster/replication/{_id}", parameters);
                    }

                    /// <summary>
                    /// Update replication job configuration.
                    /// </summary>
                    /// <param name="comment">Description.</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="disable">Flag to disable/deactivate the entry.</param>
                    /// <param name="rate">Rate limit in mbps (megabytes per second) as floating point number.</param>
                    /// <param name="remove_job">Mark the replication job for removal. The job will remove all local replication snapshots. When set to 'full', it also tries to remove replicated volumes on the target. The job then removes itself from the configuration file.
                    ///   Enum: local,full</param>
                    /// <param name="schedule">Storage replication schedule. The format is a subset of `systemd` calender events.</param>
                    /// <param name="source">Source of the replication.</param>
                    /// <returns></returns>
                    public Result Update(string comment = null, string delete = null, string digest = null, bool? disable = null, int? rate = null, string remove_job = null, string schedule = null, string source = null) => SetRest(comment, delete, digest, disable, rate, remove_job, schedule, source);
                }
                /// <summary>
                /// List replication jobs.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/replication"); }

                /// <summary>
                /// List replication jobs.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
                /// <summary>
                /// Create a new replication job
                /// </summary>
                /// <param name="id">Replication Job ID. The ID is composed of a Guest ID and a job number, separated by a hyphen, i.e. '&amp;lt;GUEST&amp;gt;-&amp;lt;JOBNUM&amp;gt;'.</param>
                /// <param name="target">Target node.</param>
                /// <param name="type">Section type.
                ///   Enum: local</param>
                /// <param name="comment">Description.</param>
                /// <param name="disable">Flag to disable/deactivate the entry.</param>
                /// <param name="rate">Rate limit in mbps (megabytes per second) as floating point number.</param>
                /// <param name="remove_job">Mark the replication job for removal. The job will remove all local replication snapshots. When set to 'full', it also tries to remove replicated volumes on the target. The job then removes itself from the configuration file.
                ///   Enum: local,full</param>
                /// <param name="schedule">Storage replication schedule. The format is a subset of `systemd` calender events.</param>
                /// <param name="source">Source of the replication.</param>
                /// <returns></returns>
                public Result CreateRest(string id, string target, string type, string comment = null, bool? disable = null, int? rate = null, string remove_job = null, string schedule = null, string source = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("id", id);
                    parameters.Add("target", target);
                    parameters.Add("type", type);
                    parameters.Add("comment", comment);
                    parameters.Add("disable", disable);
                    parameters.Add("rate", rate);
                    parameters.Add("remove_job", remove_job);
                    parameters.Add("schedule", schedule);
                    parameters.Add("source", source);
                    return _client.Create($"/cluster/replication", parameters);
                }

                /// <summary>
                /// Create a new replication job
                /// </summary>
                /// <param name="id">Replication Job ID. The ID is composed of a Guest ID and a job number, separated by a hyphen, i.e. '&amp;lt;GUEST&amp;gt;-&amp;lt;JOBNUM&amp;gt;'.</param>
                /// <param name="target">Target node.</param>
                /// <param name="type">Section type.
                ///   Enum: local</param>
                /// <param name="comment">Description.</param>
                /// <param name="disable">Flag to disable/deactivate the entry.</param>
                /// <param name="rate">Rate limit in mbps (megabytes per second) as floating point number.</param>
                /// <param name="remove_job">Mark the replication job for removal. The job will remove all local replication snapshots. When set to 'full', it also tries to remove replicated volumes on the target. The job then removes itself from the configuration file.
                ///   Enum: local,full</param>
                /// <param name="schedule">Storage replication schedule. The format is a subset of `systemd` calender events.</param>
                /// <param name="source">Source of the replication.</param>
                /// <returns></returns>
                public Result Create(string id, string target, string type, string comment = null, bool? disable = null, int? rate = null, string remove_job = null, string schedule = null, string source = null) => CreateRest(id, target, type, comment, disable, rate, remove_job, schedule, source);
            }
            public class PVEConfig
            {
                private readonly Client _client;

                internal PVEConfig(Client client) { _client = client; }
                private PVENodes _nodes;
                public PVENodes Nodes => _nodes ?? (_nodes = new PVENodes(_client));
                private PVEJoin _join;
                public PVEJoin Join => _join ?? (_join = new PVEJoin(_client));
                private PVETotem _totem;
                public PVETotem Totem => _totem ?? (_totem = new PVETotem(_client));
                private PVEQdevice _qdevice;
                public PVEQdevice Qdevice => _qdevice ?? (_qdevice = new PVEQdevice(_client));
                public class PVENodes
                {
                    private readonly Client _client;

                    internal PVENodes(Client client) { _client = client; }
                    public PVEItemNode this[object node] => new PVEItemNode(_client, node);
                    public class PVEItemNode
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEItemNode(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Removes a node from the cluster configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/cluster/config/nodes/{_node}"); }

                        /// <summary>
                        /// Removes a node from the cluster configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Delnode() => DeleteRest();
                        /// <summary>
                        /// Adds a node to the cluster configuration. This call is for internal use.
                        /// </summary>
                        /// <param name="force">Do not throw error if node already exists.</param>
                        /// <param name="linkN">Address and priority information of a single corosync link.</param>
                        /// <param name="nodeid">Node id for this node.</param>
                        /// <param name="votes">Number of votes for this node</param>
                        /// <returns></returns>
                        public Result CreateRest(bool? force = null, IDictionary<int, string> linkN = null, int? nodeid = null, int? votes = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("force", force);
                            parameters.Add("nodeid", nodeid);
                            parameters.Add("votes", votes);
                            AddIndexedParameter(parameters, "link", linkN);
                            return _client.Create($"/cluster/config/nodes/{_node}", parameters);
                        }

                        /// <summary>
                        /// Adds a node to the cluster configuration. This call is for internal use.
                        /// </summary>
                        /// <param name="force">Do not throw error if node already exists.</param>
                        /// <param name="linkN">Address and priority information of a single corosync link.</param>
                        /// <param name="nodeid">Node id for this node.</param>
                        /// <param name="votes">Number of votes for this node</param>
                        /// <returns></returns>
                        public Result Addnode(bool? force = null, IDictionary<int, string> linkN = null, int? nodeid = null, int? votes = null) => CreateRest(force, linkN, nodeid, votes);
                    }
                    /// <summary>
                    /// Corosync node list.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/config/nodes"); }

                    /// <summary>
                    /// Corosync node list.
                    /// </summary>
                    /// <returns></returns>
                    public Result Nodes() => GetRest();
                }
                public class PVEJoin
                {
                    private readonly Client _client;

                    internal PVEJoin(Client client) { _client = client; }
                    /// <summary>
                    /// Get information needed to join this cluster over the connected node.
                    /// </summary>
                    /// <param name="node">The node for which the joinee gets the nodeinfo. </param>
                    /// <returns></returns>
                    public Result GetRest(string node = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("node", node);
                        return _client.Get($"/cluster/config/join", parameters);
                    }

                    /// <summary>
                    /// Get information needed to join this cluster over the connected node.
                    /// </summary>
                    /// <param name="node">The node for which the joinee gets the nodeinfo. </param>
                    /// <returns></returns>
                    public Result JoinInfo(string node = null) => GetRest(node);
                    /// <summary>
                    /// Joins this node into an existing cluster.
                    /// </summary>
                    /// <param name="fingerprint">Certificate SHA 256 fingerprint.</param>
                    /// <param name="hostname">Hostname (or IP) of an existing cluster member.</param>
                    /// <param name="password">Superuser (root) password of peer node.</param>
                    /// <param name="force">Do not throw error if node already exists.</param>
                    /// <param name="linkN">Address and priority information of a single corosync link.</param>
                    /// <param name="nodeid">Node id for this node.</param>
                    /// <param name="votes">Number of votes for this node</param>
                    /// <returns></returns>
                    public Result CreateRest(string fingerprint, string hostname, string password, bool? force = null, IDictionary<int, string> linkN = null, int? nodeid = null, int? votes = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("fingerprint", fingerprint);
                        parameters.Add("hostname", hostname);
                        parameters.Add("password", password);
                        parameters.Add("force", force);
                        parameters.Add("nodeid", nodeid);
                        parameters.Add("votes", votes);
                        AddIndexedParameter(parameters, "link", linkN);
                        return _client.Create($"/cluster/config/join", parameters);
                    }

                    /// <summary>
                    /// Joins this node into an existing cluster.
                    /// </summary>
                    /// <param name="fingerprint">Certificate SHA 256 fingerprint.</param>
                    /// <param name="hostname">Hostname (or IP) of an existing cluster member.</param>
                    /// <param name="password">Superuser (root) password of peer node.</param>
                    /// <param name="force">Do not throw error if node already exists.</param>
                    /// <param name="linkN">Address and priority information of a single corosync link.</param>
                    /// <param name="nodeid">Node id for this node.</param>
                    /// <param name="votes">Number of votes for this node</param>
                    /// <returns></returns>
                    public Result Join(string fingerprint, string hostname, string password, bool? force = null, IDictionary<int, string> linkN = null, int? nodeid = null, int? votes = null) => CreateRest(fingerprint, hostname, password, force, linkN, nodeid, votes);
                }
                public class PVETotem
                {
                    private readonly Client _client;

                    internal PVETotem(Client client) { _client = client; }
                    /// <summary>
                    /// Get corosync totem protocol settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/config/totem"); }

                    /// <summary>
                    /// Get corosync totem protocol settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result Totem() => GetRest();
                }
                public class PVEQdevice
                {
                    private readonly Client _client;

                    internal PVEQdevice(Client client) { _client = client; }
                    /// <summary>
                    /// Get QDevice status
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/config/qdevice"); }

                    /// <summary>
                    /// Get QDevice status
                    /// </summary>
                    /// <returns></returns>
                    public Result Status() => GetRest();
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/config"); }

                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
                /// <summary>
                /// Generate new cluster configuration.
                /// </summary>
                /// <param name="clustername">The name of the cluster.</param>
                /// <param name="linkN">Address and priority information of a single corosync link.</param>
                /// <param name="nodeid">Node id for this node.</param>
                /// <param name="votes">Number of votes for this node.</param>
                /// <returns></returns>
                public Result CreateRest(string clustername, IDictionary<int, string> linkN = null, int? nodeid = null, int? votes = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("clustername", clustername);
                    parameters.Add("nodeid", nodeid);
                    parameters.Add("votes", votes);
                    AddIndexedParameter(parameters, "link", linkN);
                    return _client.Create($"/cluster/config", parameters);
                }

                /// <summary>
                /// Generate new cluster configuration.
                /// </summary>
                /// <param name="clustername">The name of the cluster.</param>
                /// <param name="linkN">Address and priority information of a single corosync link.</param>
                /// <param name="nodeid">Node id for this node.</param>
                /// <param name="votes">Number of votes for this node.</param>
                /// <returns></returns>
                public Result Create(string clustername, IDictionary<int, string> linkN = null, int? nodeid = null, int? votes = null) => CreateRest(clustername, linkN, nodeid, votes);
            }
            public class PVEFirewall
            {
                private readonly Client _client;

                internal PVEFirewall(Client client) { _client = client; }
                private PVEGroups _groups;
                public PVEGroups Groups => _groups ?? (_groups = new PVEGroups(_client));
                private PVERules _rules;
                public PVERules Rules => _rules ?? (_rules = new PVERules(_client));
                private PVEIpset _ipset;
                public PVEIpset Ipset => _ipset ?? (_ipset = new PVEIpset(_client));
                private PVEAliases _aliases;
                public PVEAliases Aliases => _aliases ?? (_aliases = new PVEAliases(_client));
                private PVEOptions _options;
                public PVEOptions Options => _options ?? (_options = new PVEOptions(_client));
                private PVEMacros _macros;
                public PVEMacros Macros => _macros ?? (_macros = new PVEMacros(_client));
                private PVERefs _refs;
                public PVERefs Refs => _refs ?? (_refs = new PVERefs(_client));
                public class PVEGroups
                {
                    private readonly Client _client;

                    internal PVEGroups(Client client) { _client = client; }
                    public PVEItemGroup this[object group] => new PVEItemGroup(_client, group);
                    public class PVEItemGroup
                    {
                        private readonly Client _client;
                        private readonly object _group;
                        internal PVEItemGroup(Client client, object group) { _client = client; _group = group; }
                        public PVEItemPos this[object pos] => new PVEItemPos(_client, _group, pos);
                        public class PVEItemPos
                        {
                            private readonly Client _client;
                            private readonly object _group;
                            private readonly object _pos;
                            internal PVEItemPos(Client client, object group, object pos)
                            {
                                _client = client; _group = group;
                                _pos = pos;
                            }
                            /// <summary>
                            /// Delete rule.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result DeleteRest(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                return _client.Delete($"/cluster/firewall/groups/{_group}/{_pos}", parameters);
                            }

                            /// <summary>
                            /// Delete rule.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result DeleteRule(string digest = null) => DeleteRest(digest);
                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/cluster/firewall/groups/{_group}/{_pos}"); }

                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRule() => GetRest();
                            /// <summary>
                            /// Modify rule data.
                            /// </summary>
                            /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                            /// <param name="comment">Descriptive comment.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="enable">Flag to enable/disable a rule.</param>
                            /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                            /// <param name="log">Log level for firewall rule.
                            ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                            /// <param name="macro">Use predefined standard macro.</param>
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            /// <returns></returns>
                            public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("action", action);
                                parameters.Add("comment", comment);
                                parameters.Add("delete", delete);
                                parameters.Add("dest", dest);
                                parameters.Add("digest", digest);
                                parameters.Add("dport", dport);
                                parameters.Add("enable", enable);
                                parameters.Add("iface", iface);
                                parameters.Add("log", log);
                                parameters.Add("macro", macro);
                                parameters.Add("moveto", moveto);
                                parameters.Add("proto", proto);
                                parameters.Add("source", source);
                                parameters.Add("sport", sport);
                                parameters.Add("type", type);
                                return _client.Set($"/cluster/firewall/groups/{_group}/{_pos}", parameters);
                            }

                            /// <summary>
                            /// Modify rule data.
                            /// </summary>
                            /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                            /// <param name="comment">Descriptive comment.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="enable">Flag to enable/disable a rule.</param>
                            /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                            /// <param name="log">Log level for firewall rule.
                            ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                            /// <param name="macro">Use predefined standard macro.</param>
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            /// <returns></returns>
                            public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) => SetRest(action, comment, delete, dest, digest, dport, enable, iface, log, macro, moveto, proto, source, sport, type);
                        }
                        /// <summary>
                        /// Delete security group.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/cluster/firewall/groups/{_group}"); }

                        /// <summary>
                        /// Delete security group.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteSecurityGroup() => DeleteRest();
                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/firewall/groups/{_group}"); }

                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRules() => GetRest();
                        /// <summary>
                        /// Create new rule.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                        /// <param name="log">Log level for firewall rule.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("type", type);
                            parameters.Add("comment", comment);
                            parameters.Add("dest", dest);
                            parameters.Add("digest", digest);
                            parameters.Add("dport", dport);
                            parameters.Add("enable", enable);
                            parameters.Add("iface", iface);
                            parameters.Add("log", log);
                            parameters.Add("macro", macro);
                            parameters.Add("pos", pos);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            return _client.Create($"/cluster/firewall/groups/{_group}", parameters);
                        }

                        /// <summary>
                        /// Create new rule.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                        /// <param name="log">Log level for firewall rule.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) => CreateRest(action, type, comment, dest, digest, dport, enable, iface, log, macro, pos, proto, source, sport);
                    }
                    /// <summary>
                    /// List security groups.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/firewall/groups"); }

                    /// <summary>
                    /// List security groups.
                    /// </summary>
                    /// <returns></returns>
                    public Result ListSecurityGroups() => GetRest();
                    /// <summary>
                    /// Create new security group.
                    /// </summary>
                    /// <param name="group">Security Group name.</param>
                    /// <param name="comment"></param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="rename">Rename/update an existing security group. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing group.</param>
                    /// <returns></returns>
                    public Result CreateRest(string group, string comment = null, string digest = null, string rename = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("group", group);
                        parameters.Add("comment", comment);
                        parameters.Add("digest", digest);
                        parameters.Add("rename", rename);
                        return _client.Create($"/cluster/firewall/groups", parameters);
                    }

                    /// <summary>
                    /// Create new security group.
                    /// </summary>
                    /// <param name="group">Security Group name.</param>
                    /// <param name="comment"></param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="rename">Rename/update an existing security group. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing group.</param>
                    /// <returns></returns>
                    public Result CreateSecurityGroup(string group, string comment = null, string digest = null, string rename = null) => CreateRest(group, comment, digest, rename);
                }
                public class PVERules
                {
                    private readonly Client _client;

                    internal PVERules(Client client) { _client = client; }
                    public PVEItemPos this[object pos] => new PVEItemPos(_client, pos);
                    public class PVEItemPos
                    {
                        private readonly Client _client;
                        private readonly object _pos;
                        internal PVEItemPos(Client client, object pos) { _client = client; _pos = pos; }
                        /// <summary>
                        /// Delete rule.
                        /// </summary>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <returns></returns>
                        public Result DeleteRest(string digest = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("digest", digest);
                            return _client.Delete($"/cluster/firewall/rules/{_pos}", parameters);
                        }

                        /// <summary>
                        /// Delete rule.
                        /// </summary>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <returns></returns>
                        public Result DeleteRule(string digest = null) => DeleteRest(digest);
                        /// <summary>
                        /// Get single rule data.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/firewall/rules/{_pos}"); }

                        /// <summary>
                        /// Get single rule data.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRule() => GetRest();
                        /// <summary>
                        /// Modify rule data.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                        /// <param name="log">Log level for firewall rule.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <returns></returns>
                        public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("dest", dest);
                            parameters.Add("digest", digest);
                            parameters.Add("dport", dport);
                            parameters.Add("enable", enable);
                            parameters.Add("iface", iface);
                            parameters.Add("log", log);
                            parameters.Add("macro", macro);
                            parameters.Add("moveto", moveto);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            parameters.Add("type", type);
                            return _client.Set($"/cluster/firewall/rules/{_pos}", parameters);
                        }

                        /// <summary>
                        /// Modify rule data.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                        /// <param name="log">Log level for firewall rule.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <returns></returns>
                        public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) => SetRest(action, comment, delete, dest, digest, dport, enable, iface, log, macro, moveto, proto, source, sport, type);
                    }
                    /// <summary>
                    /// List rules.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/firewall/rules"); }

                    /// <summary>
                    /// List rules.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRules() => GetRest();
                    /// <summary>
                    /// Create new rule.
                    /// </summary>
                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                    /// <param name="type">Rule type.
                    ///   Enum: in,out,group</param>
                    /// <param name="comment">Descriptive comment.</param>
                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    /// <param name="enable">Flag to enable/disable a rule.</param>
                    /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                    /// <param name="log">Log level for firewall rule.
                    ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                    /// <param name="macro">Use predefined standard macro.</param>
                    /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    /// <returns></returns>
                    public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("action", action);
                        parameters.Add("type", type);
                        parameters.Add("comment", comment);
                        parameters.Add("dest", dest);
                        parameters.Add("digest", digest);
                        parameters.Add("dport", dport);
                        parameters.Add("enable", enable);
                        parameters.Add("iface", iface);
                        parameters.Add("log", log);
                        parameters.Add("macro", macro);
                        parameters.Add("pos", pos);
                        parameters.Add("proto", proto);
                        parameters.Add("source", source);
                        parameters.Add("sport", sport);
                        return _client.Create($"/cluster/firewall/rules", parameters);
                    }

                    /// <summary>
                    /// Create new rule.
                    /// </summary>
                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                    /// <param name="type">Rule type.
                    ///   Enum: in,out,group</param>
                    /// <param name="comment">Descriptive comment.</param>
                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    /// <param name="enable">Flag to enable/disable a rule.</param>
                    /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                    /// <param name="log">Log level for firewall rule.
                    ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                    /// <param name="macro">Use predefined standard macro.</param>
                    /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    /// <returns></returns>
                    public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) => CreateRest(action, type, comment, dest, digest, dport, enable, iface, log, macro, pos, proto, source, sport);
                }
                public class PVEIpset
                {
                    private readonly Client _client;

                    internal PVEIpset(Client client) { _client = client; }
                    public PVEItemName this[object name] => new PVEItemName(_client, name);
                    public class PVEItemName
                    {
                        private readonly Client _client;
                        private readonly object _name;
                        internal PVEItemName(Client client, object name) { _client = client; _name = name; }
                        public PVEItemCidr this[object cidr] => new PVEItemCidr(_client, _name, cidr);
                        public class PVEItemCidr
                        {
                            private readonly Client _client;
                            private readonly object _name;
                            private readonly object _cidr;
                            internal PVEItemCidr(Client client, object name, object cidr)
                            {
                                _client = client; _name = name;
                                _cidr = cidr;
                            }
                            /// <summary>
                            /// Remove IP or Network from IPSet.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result DeleteRest(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                return _client.Delete($"/cluster/firewall/ipset/{_name}/{_cidr}", parameters);
                            }

                            /// <summary>
                            /// Remove IP or Network from IPSet.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result RemoveIp(string digest = null) => DeleteRest(digest);
                            /// <summary>
                            /// Read IP or Network settings from IPSet.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/cluster/firewall/ipset/{_name}/{_cidr}"); }

                            /// <summary>
                            /// Read IP or Network settings from IPSet.
                            /// </summary>
                            /// <returns></returns>
                            public Result ReadIp() => GetRest();
                            /// <summary>
                            /// Update IP or Network settings
                            /// </summary>
                            /// <param name="comment"></param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="nomatch"></param>
                            /// <returns></returns>
                            public Result SetRest(string comment = null, string digest = null, bool? nomatch = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("comment", comment);
                                parameters.Add("digest", digest);
                                parameters.Add("nomatch", nomatch);
                                return _client.Set($"/cluster/firewall/ipset/{_name}/{_cidr}", parameters);
                            }

                            /// <summary>
                            /// Update IP or Network settings
                            /// </summary>
                            /// <param name="comment"></param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="nomatch"></param>
                            /// <returns></returns>
                            public Result UpdateIp(string comment = null, string digest = null, bool? nomatch = null) => SetRest(comment, digest, nomatch);
                        }
                        /// <summary>
                        /// Delete IPSet
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/cluster/firewall/ipset/{_name}"); }

                        /// <summary>
                        /// Delete IPSet
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteIpset() => DeleteRest();
                        /// <summary>
                        /// List IPSet content
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/firewall/ipset/{_name}"); }

                        /// <summary>
                        /// List IPSet content
                        /// </summary>
                        /// <returns></returns>
                        public Result GetIpset() => GetRest();
                        /// <summary>
                        /// Add IP or Network to IPSet.
                        /// </summary>
                        /// <param name="cidr">Network/IP specification in CIDR format.</param>
                        /// <param name="comment"></param>
                        /// <param name="nomatch"></param>
                        /// <returns></returns>
                        public Result CreateRest(string cidr, string comment = null, bool? nomatch = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("cidr", cidr);
                            parameters.Add("comment", comment);
                            parameters.Add("nomatch", nomatch);
                            return _client.Create($"/cluster/firewall/ipset/{_name}", parameters);
                        }

                        /// <summary>
                        /// Add IP or Network to IPSet.
                        /// </summary>
                        /// <param name="cidr">Network/IP specification in CIDR format.</param>
                        /// <param name="comment"></param>
                        /// <param name="nomatch"></param>
                        /// <returns></returns>
                        public Result CreateIp(string cidr, string comment = null, bool? nomatch = null) => CreateRest(cidr, comment, nomatch);
                    }
                    /// <summary>
                    /// List IPSets
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/firewall/ipset"); }

                    /// <summary>
                    /// List IPSets
                    /// </summary>
                    /// <returns></returns>
                    public Result IpsetIndex() => GetRest();
                    /// <summary>
                    /// Create new IPSet
                    /// </summary>
                    /// <param name="name">IP set name.</param>
                    /// <param name="comment"></param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                    /// <returns></returns>
                    public Result CreateRest(string name, string comment = null, string digest = null, string rename = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("name", name);
                        parameters.Add("comment", comment);
                        parameters.Add("digest", digest);
                        parameters.Add("rename", rename);
                        return _client.Create($"/cluster/firewall/ipset", parameters);
                    }

                    /// <summary>
                    /// Create new IPSet
                    /// </summary>
                    /// <param name="name">IP set name.</param>
                    /// <param name="comment"></param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                    /// <returns></returns>
                    public Result CreateIpset(string name, string comment = null, string digest = null, string rename = null) => CreateRest(name, comment, digest, rename);
                }
                public class PVEAliases
                {
                    private readonly Client _client;

                    internal PVEAliases(Client client) { _client = client; }
                    public PVEItemName this[object name] => new PVEItemName(_client, name);
                    public class PVEItemName
                    {
                        private readonly Client _client;
                        private readonly object _name;
                        internal PVEItemName(Client client, object name) { _client = client; _name = name; }
                        /// <summary>
                        /// Remove IP or Network alias.
                        /// </summary>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <returns></returns>
                        public Result DeleteRest(string digest = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("digest", digest);
                            return _client.Delete($"/cluster/firewall/aliases/{_name}", parameters);
                        }

                        /// <summary>
                        /// Remove IP or Network alias.
                        /// </summary>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <returns></returns>
                        public Result RemoveAlias(string digest = null) => DeleteRest(digest);
                        /// <summary>
                        /// Read alias.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/firewall/aliases/{_name}"); }

                        /// <summary>
                        /// Read alias.
                        /// </summary>
                        /// <returns></returns>
                        public Result ReadAlias() => GetRest();
                        /// <summary>
                        /// Update IP or Network alias.
                        /// </summary>
                        /// <param name="cidr">Network/IP specification in CIDR format.</param>
                        /// <param name="comment"></param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="rename">Rename an existing alias.</param>
                        /// <returns></returns>
                        public Result SetRest(string cidr, string comment = null, string digest = null, string rename = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("cidr", cidr);
                            parameters.Add("comment", comment);
                            parameters.Add("digest", digest);
                            parameters.Add("rename", rename);
                            return _client.Set($"/cluster/firewall/aliases/{_name}", parameters);
                        }

                        /// <summary>
                        /// Update IP or Network alias.
                        /// </summary>
                        /// <param name="cidr">Network/IP specification in CIDR format.</param>
                        /// <param name="comment"></param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="rename">Rename an existing alias.</param>
                        /// <returns></returns>
                        public Result UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null) => SetRest(cidr, comment, digest, rename);
                    }
                    /// <summary>
                    /// List aliases
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/firewall/aliases"); }

                    /// <summary>
                    /// List aliases
                    /// </summary>
                    /// <returns></returns>
                    public Result GetAliases() => GetRest();
                    /// <summary>
                    /// Create IP or Network Alias.
                    /// </summary>
                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                    /// <param name="name">Alias name.</param>
                    /// <param name="comment"></param>
                    /// <returns></returns>
                    public Result CreateRest(string cidr, string name, string comment = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cidr", cidr);
                        parameters.Add("name", name);
                        parameters.Add("comment", comment);
                        return _client.Create($"/cluster/firewall/aliases", parameters);
                    }

                    /// <summary>
                    /// Create IP or Network Alias.
                    /// </summary>
                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                    /// <param name="name">Alias name.</param>
                    /// <param name="comment"></param>
                    /// <returns></returns>
                    public Result CreateAlias(string cidr, string name, string comment = null) => CreateRest(cidr, name, comment);
                }
                public class PVEOptions
                {
                    private readonly Client _client;

                    internal PVEOptions(Client client) { _client = client; }
                    /// <summary>
                    /// Get Firewall options.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/firewall/options"); }

                    /// <summary>
                    /// Get Firewall options.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetOptions() => GetRest();
                    /// <summary>
                    /// Set Firewall options.
                    /// </summary>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="ebtables">Enable ebtables rules cluster wide.</param>
                    /// <param name="enable">Enable or disable the firewall cluster wide.</param>
                    /// <param name="log_ratelimit">Log ratelimiting settings</param>
                    /// <param name="policy_in">Input policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <param name="policy_out">Output policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <returns></returns>
                    public Result SetRest(string delete = null, string digest = null, bool? ebtables = null, int? enable = null, string log_ratelimit = null, string policy_in = null, string policy_out = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("delete", delete);
                        parameters.Add("digest", digest);
                        parameters.Add("ebtables", ebtables);
                        parameters.Add("enable", enable);
                        parameters.Add("log_ratelimit", log_ratelimit);
                        parameters.Add("policy_in", policy_in);
                        parameters.Add("policy_out", policy_out);
                        return _client.Set($"/cluster/firewall/options", parameters);
                    }

                    /// <summary>
                    /// Set Firewall options.
                    /// </summary>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="ebtables">Enable ebtables rules cluster wide.</param>
                    /// <param name="enable">Enable or disable the firewall cluster wide.</param>
                    /// <param name="log_ratelimit">Log ratelimiting settings</param>
                    /// <param name="policy_in">Input policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <param name="policy_out">Output policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <returns></returns>
                    public Result SetOptions(string delete = null, string digest = null, bool? ebtables = null, int? enable = null, string log_ratelimit = null, string policy_in = null, string policy_out = null) => SetRest(delete, digest, ebtables, enable, log_ratelimit, policy_in, policy_out);
                }
                public class PVEMacros
                {
                    private readonly Client _client;

                    internal PVEMacros(Client client) { _client = client; }
                    /// <summary>
                    /// List available macros
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/firewall/macros"); }

                    /// <summary>
                    /// List available macros
                    /// </summary>
                    /// <returns></returns>
                    public Result GetMacros() => GetRest();
                }
                public class PVERefs
                {
                    private readonly Client _client;

                    internal PVERefs(Client client) { _client = client; }
                    /// <summary>
                    /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                    /// </summary>
                    /// <param name="type">Only list references of specified type.
                    ///   Enum: alias,ipset</param>
                    /// <returns></returns>
                    public Result GetRest(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return _client.Get($"/cluster/firewall/refs", parameters);
                    }

                    /// <summary>
                    /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                    /// </summary>
                    /// <param name="type">Only list references of specified type.
                    ///   Enum: alias,ipset</param>
                    /// <returns></returns>
                    public Result Refs(string type = null) => GetRest(type);
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/firewall"); }

                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
            }
            public class PVEBackup
            {
                private readonly Client _client;

                internal PVEBackup(Client client) { _client = client; }
                public PVEItemId this[object id] => new PVEItemId(_client, id);
                public class PVEItemId
                {
                    private readonly Client _client;
                    private readonly object _id;
                    internal PVEItemId(Client client, object id) { _client = client; _id = id; }
                    /// <summary>
                    /// Delete vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest() { return _client.Delete($"/cluster/backup/{_id}"); }

                    /// <summary>
                    /// Delete vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteJob() => DeleteRest();
                    /// <summary>
                    /// Read vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/backup/{_id}"); }

                    /// <summary>
                    /// Read vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public Result ReadJob() => GetRest();
                    /// <summary>
                    /// Update vzdump backup job definition.
                    /// </summary>
                    /// <param name="starttime">Job Start time.</param>
                    /// <param name="all">Backup all known guest systems on this host.</param>
                    /// <param name="bwlimit">Limit I/O bandwidth (KBytes per second).</param>
                    /// <param name="compress">Compress dump file.
                    ///   Enum: 0,1,gzip,lzo</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="dow">Day of week selection.</param>
                    /// <param name="dumpdir">Store resulting files to specified directory.</param>
                    /// <param name="enabled">Enable or disable the job.</param>
                    /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                    /// <param name="exclude_path">Exclude certain files/directories (shell globs).</param>
                    /// <param name="ionice">Set CFQ ionice priority.</param>
                    /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                    /// <param name="mailnotification">Specify when to send an email
                    ///   Enum: always,failure</param>
                    /// <param name="mailto">Comma-separated list of email addresses that should receive email notifications.</param>
                    /// <param name="maxfiles">Maximal number of backup files per guest system.</param>
                    /// <param name="mode">Backup mode.
                    ///   Enum: snapshot,suspend,stop</param>
                    /// <param name="node">Only run if executed on this node.</param>
                    /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                    /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                    /// <param name="quiet">Be quiet.</param>
                    /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                    /// <param name="script">Use specified hook script.</param>
                    /// <param name="size">Unused, will be removed in a future release.</param>
                    /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                    /// <param name="stop">Stop running backup jobs on this host.</param>
                    /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                    /// <param name="storage">Store resulting file to this storage.</param>
                    /// <param name="tmpdir">Store temporary files to specified directory.</param>
                    /// <param name="vmid">The ID of the guest system you want to backup.</param>
                    /// <returns></returns>
                    public Result SetRest(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string delete = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, string pool = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("starttime", starttime);
                        parameters.Add("all", all);
                        parameters.Add("bwlimit", bwlimit);
                        parameters.Add("compress", compress);
                        parameters.Add("delete", delete);
                        parameters.Add("dow", dow);
                        parameters.Add("dumpdir", dumpdir);
                        parameters.Add("enabled", enabled);
                        parameters.Add("exclude", exclude);
                        parameters.Add("exclude-path", exclude_path);
                        parameters.Add("ionice", ionice);
                        parameters.Add("lockwait", lockwait);
                        parameters.Add("mailnotification", mailnotification);
                        parameters.Add("mailto", mailto);
                        parameters.Add("maxfiles", maxfiles);
                        parameters.Add("mode", mode);
                        parameters.Add("node", node);
                        parameters.Add("pigz", pigz);
                        parameters.Add("pool", pool);
                        parameters.Add("quiet", quiet);
                        parameters.Add("remove", remove);
                        parameters.Add("script", script);
                        parameters.Add("size", size);
                        parameters.Add("stdexcludes", stdexcludes);
                        parameters.Add("stop", stop);
                        parameters.Add("stopwait", stopwait);
                        parameters.Add("storage", storage);
                        parameters.Add("tmpdir", tmpdir);
                        parameters.Add("vmid", vmid);
                        return _client.Set($"/cluster/backup/{_id}", parameters);
                    }

                    /// <summary>
                    /// Update vzdump backup job definition.
                    /// </summary>
                    /// <param name="starttime">Job Start time.</param>
                    /// <param name="all">Backup all known guest systems on this host.</param>
                    /// <param name="bwlimit">Limit I/O bandwidth (KBytes per second).</param>
                    /// <param name="compress">Compress dump file.
                    ///   Enum: 0,1,gzip,lzo</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="dow">Day of week selection.</param>
                    /// <param name="dumpdir">Store resulting files to specified directory.</param>
                    /// <param name="enabled">Enable or disable the job.</param>
                    /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                    /// <param name="exclude_path">Exclude certain files/directories (shell globs).</param>
                    /// <param name="ionice">Set CFQ ionice priority.</param>
                    /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                    /// <param name="mailnotification">Specify when to send an email
                    ///   Enum: always,failure</param>
                    /// <param name="mailto">Comma-separated list of email addresses that should receive email notifications.</param>
                    /// <param name="maxfiles">Maximal number of backup files per guest system.</param>
                    /// <param name="mode">Backup mode.
                    ///   Enum: snapshot,suspend,stop</param>
                    /// <param name="node">Only run if executed on this node.</param>
                    /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                    /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                    /// <param name="quiet">Be quiet.</param>
                    /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                    /// <param name="script">Use specified hook script.</param>
                    /// <param name="size">Unused, will be removed in a future release.</param>
                    /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                    /// <param name="stop">Stop running backup jobs on this host.</param>
                    /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                    /// <param name="storage">Store resulting file to this storage.</param>
                    /// <param name="tmpdir">Store temporary files to specified directory.</param>
                    /// <param name="vmid">The ID of the guest system you want to backup.</param>
                    /// <returns></returns>
                    public Result UpdateJob(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string delete = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, string pool = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null) => SetRest(starttime, all, bwlimit, compress, delete, dow, dumpdir, enabled, exclude, exclude_path, ionice, lockwait, mailnotification, mailto, maxfiles, mode, node, pigz, pool, quiet, remove, script, size, stdexcludes, stop, stopwait, storage, tmpdir, vmid);
                }
                /// <summary>
                /// List vzdump backup schedule.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/backup"); }

                /// <summary>
                /// List vzdump backup schedule.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
                /// <summary>
                /// Create new vzdump backup job.
                /// </summary>
                /// <param name="starttime">Job Start time.</param>
                /// <param name="all">Backup all known guest systems on this host.</param>
                /// <param name="bwlimit">Limit I/O bandwidth (KBytes per second).</param>
                /// <param name="compress">Compress dump file.
                ///   Enum: 0,1,gzip,lzo</param>
                /// <param name="dow">Day of week selection.</param>
                /// <param name="dumpdir">Store resulting files to specified directory.</param>
                /// <param name="enabled">Enable or disable the job.</param>
                /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                /// <param name="exclude_path">Exclude certain files/directories (shell globs).</param>
                /// <param name="ionice">Set CFQ ionice priority.</param>
                /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                /// <param name="mailnotification">Specify when to send an email
                ///   Enum: always,failure</param>
                /// <param name="mailto">Comma-separated list of email addresses that should receive email notifications.</param>
                /// <param name="maxfiles">Maximal number of backup files per guest system.</param>
                /// <param name="mode">Backup mode.
                ///   Enum: snapshot,suspend,stop</param>
                /// <param name="node">Only run if executed on this node.</param>
                /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                /// <param name="quiet">Be quiet.</param>
                /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                /// <param name="script">Use specified hook script.</param>
                /// <param name="size">Unused, will be removed in a future release.</param>
                /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                /// <param name="stop">Stop running backup jobs on this host.</param>
                /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                /// <param name="storage">Store resulting file to this storage.</param>
                /// <param name="tmpdir">Store temporary files to specified directory.</param>
                /// <param name="vmid">The ID of the guest system you want to backup.</param>
                /// <returns></returns>
                public Result CreateRest(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, string pool = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("starttime", starttime);
                    parameters.Add("all", all);
                    parameters.Add("bwlimit", bwlimit);
                    parameters.Add("compress", compress);
                    parameters.Add("dow", dow);
                    parameters.Add("dumpdir", dumpdir);
                    parameters.Add("enabled", enabled);
                    parameters.Add("exclude", exclude);
                    parameters.Add("exclude-path", exclude_path);
                    parameters.Add("ionice", ionice);
                    parameters.Add("lockwait", lockwait);
                    parameters.Add("mailnotification", mailnotification);
                    parameters.Add("mailto", mailto);
                    parameters.Add("maxfiles", maxfiles);
                    parameters.Add("mode", mode);
                    parameters.Add("node", node);
                    parameters.Add("pigz", pigz);
                    parameters.Add("pool", pool);
                    parameters.Add("quiet", quiet);
                    parameters.Add("remove", remove);
                    parameters.Add("script", script);
                    parameters.Add("size", size);
                    parameters.Add("stdexcludes", stdexcludes);
                    parameters.Add("stop", stop);
                    parameters.Add("stopwait", stopwait);
                    parameters.Add("storage", storage);
                    parameters.Add("tmpdir", tmpdir);
                    parameters.Add("vmid", vmid);
                    return _client.Create($"/cluster/backup", parameters);
                }

                /// <summary>
                /// Create new vzdump backup job.
                /// </summary>
                /// <param name="starttime">Job Start time.</param>
                /// <param name="all">Backup all known guest systems on this host.</param>
                /// <param name="bwlimit">Limit I/O bandwidth (KBytes per second).</param>
                /// <param name="compress">Compress dump file.
                ///   Enum: 0,1,gzip,lzo</param>
                /// <param name="dow">Day of week selection.</param>
                /// <param name="dumpdir">Store resulting files to specified directory.</param>
                /// <param name="enabled">Enable or disable the job.</param>
                /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                /// <param name="exclude_path">Exclude certain files/directories (shell globs).</param>
                /// <param name="ionice">Set CFQ ionice priority.</param>
                /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                /// <param name="mailnotification">Specify when to send an email
                ///   Enum: always,failure</param>
                /// <param name="mailto">Comma-separated list of email addresses that should receive email notifications.</param>
                /// <param name="maxfiles">Maximal number of backup files per guest system.</param>
                /// <param name="mode">Backup mode.
                ///   Enum: snapshot,suspend,stop</param>
                /// <param name="node">Only run if executed on this node.</param>
                /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                /// <param name="quiet">Be quiet.</param>
                /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                /// <param name="script">Use specified hook script.</param>
                /// <param name="size">Unused, will be removed in a future release.</param>
                /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                /// <param name="stop">Stop running backup jobs on this host.</param>
                /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                /// <param name="storage">Store resulting file to this storage.</param>
                /// <param name="tmpdir">Store temporary files to specified directory.</param>
                /// <param name="vmid">The ID of the guest system you want to backup.</param>
                /// <returns></returns>
                public Result CreateJob(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, string pool = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null) => CreateRest(starttime, all, bwlimit, compress, dow, dumpdir, enabled, exclude, exclude_path, ionice, lockwait, mailnotification, mailto, maxfiles, mode, node, pigz, pool, quiet, remove, script, size, stdexcludes, stop, stopwait, storage, tmpdir, vmid);
            }
            public class PVEHa
            {
                private readonly Client _client;

                internal PVEHa(Client client) { _client = client; }
                private PVEResources _resources;
                public PVEResources Resources => _resources ?? (_resources = new PVEResources(_client));
                private PVEGroups _groups;
                public PVEGroups Groups => _groups ?? (_groups = new PVEGroups(_client));
                private PVEStatus _status;
                public PVEStatus Status => _status ?? (_status = new PVEStatus(_client));
                public class PVEResources
                {
                    private readonly Client _client;

                    internal PVEResources(Client client) { _client = client; }
                    public PVEItemSid this[object sid] => new PVEItemSid(_client, sid);
                    public class PVEItemSid
                    {
                        private readonly Client _client;
                        private readonly object _sid;
                        internal PVEItemSid(Client client, object sid) { _client = client; _sid = sid; }
                        private PVEMigrate _migrate;
                        public PVEMigrate Migrate => _migrate ?? (_migrate = new PVEMigrate(_client, _sid));
                        private PVERelocate _relocate;
                        public PVERelocate Relocate => _relocate ?? (_relocate = new PVERelocate(_client, _sid));
                        public class PVEMigrate
                        {
                            private readonly Client _client;
                            private readonly object _sid;
                            internal PVEMigrate(Client client, object sid) { _client = client; _sid = sid; }
                            /// <summary>
                            /// Request resource migration (online) to another node.
                            /// </summary>
                            /// <param name="node">Target node.</param>
                            /// <returns></returns>
                            public Result CreateRest(string node)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("node", node);
                                return _client.Create($"/cluster/ha/resources/{_sid}/migrate", parameters);
                            }

                            /// <summary>
                            /// Request resource migration (online) to another node.
                            /// </summary>
                            /// <param name="node">Target node.</param>
                            /// <returns></returns>
                            public Result Migrate(string node) => CreateRest(node);
                        }
                        public class PVERelocate
                        {
                            private readonly Client _client;
                            private readonly object _sid;
                            internal PVERelocate(Client client, object sid) { _client = client; _sid = sid; }
                            /// <summary>
                            /// Request resource relocatzion to another node. This stops the service on the old node, and restarts it on the target node.
                            /// </summary>
                            /// <param name="node">Target node.</param>
                            /// <returns></returns>
                            public Result CreateRest(string node)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("node", node);
                                return _client.Create($"/cluster/ha/resources/{_sid}/relocate", parameters);
                            }

                            /// <summary>
                            /// Request resource relocatzion to another node. This stops the service on the old node, and restarts it on the target node.
                            /// </summary>
                            /// <param name="node">Target node.</param>
                            /// <returns></returns>
                            public Result Relocate(string node) => CreateRest(node);
                        }
                        /// <summary>
                        /// Delete resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/cluster/ha/resources/{_sid}"); }

                        /// <summary>
                        /// Delete resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Delete() => DeleteRest();
                        /// <summary>
                        /// Read resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/ha/resources/{_sid}"); }

                        /// <summary>
                        /// Read resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Read() => GetRest();
                        /// <summary>
                        /// Update resource configuration.
                        /// </summary>
                        /// <param name="comment">Description.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="group">The HA group identifier.</param>
                        /// <param name="max_relocate">Maximal number of service relocate tries when a service failes to start.</param>
                        /// <param name="max_restart">Maximal number of tries to restart the service on a node after its start failed.</param>
                        /// <param name="state">Requested resource state.
                        ///   Enum: started,stopped,enabled,disabled,ignored</param>
                        /// <returns></returns>
                        public Result SetRest(string comment = null, string delete = null, string digest = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("group", group);
                            parameters.Add("max_relocate", max_relocate);
                            parameters.Add("max_restart", max_restart);
                            parameters.Add("state", state);
                            return _client.Set($"/cluster/ha/resources/{_sid}", parameters);
                        }

                        /// <summary>
                        /// Update resource configuration.
                        /// </summary>
                        /// <param name="comment">Description.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="group">The HA group identifier.</param>
                        /// <param name="max_relocate">Maximal number of service relocate tries when a service failes to start.</param>
                        /// <param name="max_restart">Maximal number of tries to restart the service on a node after its start failed.</param>
                        /// <param name="state">Requested resource state.
                        ///   Enum: started,stopped,enabled,disabled,ignored</param>
                        /// <returns></returns>
                        public Result Update(string comment = null, string delete = null, string digest = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null) => SetRest(comment, delete, digest, group, max_relocate, max_restart, state);
                    }
                    /// <summary>
                    /// List HA resources.
                    /// </summary>
                    /// <param name="type">Only list resources of specific type
                    ///   Enum: ct,vm</param>
                    /// <returns></returns>
                    public Result GetRest(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return _client.Get($"/cluster/ha/resources", parameters);
                    }

                    /// <summary>
                    /// List HA resources.
                    /// </summary>
                    /// <param name="type">Only list resources of specific type
                    ///   Enum: ct,vm</param>
                    /// <returns></returns>
                    public Result Index(string type = null) => GetRest(type);
                    /// <summary>
                    /// Create a new HA resource.
                    /// </summary>
                    /// <param name="sid">HA resource ID. This consists of a resource type followed by a resource specific name, separated with colon (example: vm:100 / ct:100). For virtual machines and containers, you can simply use the VM or CT id as a shortcut (example: 100).</param>
                    /// <param name="comment">Description.</param>
                    /// <param name="group">The HA group identifier.</param>
                    /// <param name="max_relocate">Maximal number of service relocate tries when a service failes to start.</param>
                    /// <param name="max_restart">Maximal number of tries to restart the service on a node after its start failed.</param>
                    /// <param name="state">Requested resource state.
                    ///   Enum: started,stopped,enabled,disabled,ignored</param>
                    /// <param name="type">Resource type.
                    ///   Enum: ct,vm</param>
                    /// <returns></returns>
                    public Result CreateRest(string sid, string comment = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null, string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("sid", sid);
                        parameters.Add("comment", comment);
                        parameters.Add("group", group);
                        parameters.Add("max_relocate", max_relocate);
                        parameters.Add("max_restart", max_restart);
                        parameters.Add("state", state);
                        parameters.Add("type", type);
                        return _client.Create($"/cluster/ha/resources", parameters);
                    }

                    /// <summary>
                    /// Create a new HA resource.
                    /// </summary>
                    /// <param name="sid">HA resource ID. This consists of a resource type followed by a resource specific name, separated with colon (example: vm:100 / ct:100). For virtual machines and containers, you can simply use the VM or CT id as a shortcut (example: 100).</param>
                    /// <param name="comment">Description.</param>
                    /// <param name="group">The HA group identifier.</param>
                    /// <param name="max_relocate">Maximal number of service relocate tries when a service failes to start.</param>
                    /// <param name="max_restart">Maximal number of tries to restart the service on a node after its start failed.</param>
                    /// <param name="state">Requested resource state.
                    ///   Enum: started,stopped,enabled,disabled,ignored</param>
                    /// <param name="type">Resource type.
                    ///   Enum: ct,vm</param>
                    /// <returns></returns>
                    public Result Create(string sid, string comment = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null, string type = null) => CreateRest(sid, comment, group, max_relocate, max_restart, state, type);
                }
                public class PVEGroups
                {
                    private readonly Client _client;

                    internal PVEGroups(Client client) { _client = client; }
                    public PVEItemGroup this[object group] => new PVEItemGroup(_client, group);
                    public class PVEItemGroup
                    {
                        private readonly Client _client;
                        private readonly object _group;
                        internal PVEItemGroup(Client client, object group) { _client = client; _group = group; }
                        /// <summary>
                        /// Delete ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/cluster/ha/groups/{_group}"); }

                        /// <summary>
                        /// Delete ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Delete() => DeleteRest();
                        /// <summary>
                        /// Read ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/ha/groups/{_group}"); }

                        /// <summary>
                        /// Read ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Read() => GetRest();
                        /// <summary>
                        /// Update ha group configuration.
                        /// </summary>
                        /// <param name="comment">Description.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="nodes">List of cluster node names with optional priority.</param>
                        /// <param name="nofailback">The CRM tries to run services on the node with the highest priority. If a node with higher priority comes online, the CRM migrates the service to that node. Enabling nofailback prevents that behavior.</param>
                        /// <param name="restricted">Resources bound to restricted groups may only run on nodes defined by the group.</param>
                        /// <returns></returns>
                        public Result SetRest(string comment = null, string delete = null, string digest = null, string nodes = null, bool? nofailback = null, bool? restricted = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("nodes", nodes);
                            parameters.Add("nofailback", nofailback);
                            parameters.Add("restricted", restricted);
                            return _client.Set($"/cluster/ha/groups/{_group}", parameters);
                        }

                        /// <summary>
                        /// Update ha group configuration.
                        /// </summary>
                        /// <param name="comment">Description.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="nodes">List of cluster node names with optional priority.</param>
                        /// <param name="nofailback">The CRM tries to run services on the node with the highest priority. If a node with higher priority comes online, the CRM migrates the service to that node. Enabling nofailback prevents that behavior.</param>
                        /// <param name="restricted">Resources bound to restricted groups may only run on nodes defined by the group.</param>
                        /// <returns></returns>
                        public Result Update(string comment = null, string delete = null, string digest = null, string nodes = null, bool? nofailback = null, bool? restricted = null) => SetRest(comment, delete, digest, nodes, nofailback, restricted);
                    }
                    /// <summary>
                    /// Get HA groups.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/ha/groups"); }

                    /// <summary>
                    /// Get HA groups.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                    /// <summary>
                    /// Create a new HA group.
                    /// </summary>
                    /// <param name="group">The HA group identifier.</param>
                    /// <param name="nodes">List of cluster node names with optional priority.</param>
                    /// <param name="comment">Description.</param>
                    /// <param name="nofailback">The CRM tries to run services on the node with the highest priority. If a node with higher priority comes online, the CRM migrates the service to that node. Enabling nofailback prevents that behavior.</param>
                    /// <param name="restricted">Resources bound to restricted groups may only run on nodes defined by the group.</param>
                    /// <param name="type">Group type.
                    ///   Enum: group</param>
                    /// <returns></returns>
                    public Result CreateRest(string group, string nodes, string comment = null, bool? nofailback = null, bool? restricted = null, string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("group", group);
                        parameters.Add("nodes", nodes);
                        parameters.Add("comment", comment);
                        parameters.Add("nofailback", nofailback);
                        parameters.Add("restricted", restricted);
                        parameters.Add("type", type);
                        return _client.Create($"/cluster/ha/groups", parameters);
                    }

                    /// <summary>
                    /// Create a new HA group.
                    /// </summary>
                    /// <param name="group">The HA group identifier.</param>
                    /// <param name="nodes">List of cluster node names with optional priority.</param>
                    /// <param name="comment">Description.</param>
                    /// <param name="nofailback">The CRM tries to run services on the node with the highest priority. If a node with higher priority comes online, the CRM migrates the service to that node. Enabling nofailback prevents that behavior.</param>
                    /// <param name="restricted">Resources bound to restricted groups may only run on nodes defined by the group.</param>
                    /// <param name="type">Group type.
                    ///   Enum: group</param>
                    /// <returns></returns>
                    public Result Create(string group, string nodes, string comment = null, bool? nofailback = null, bool? restricted = null, string type = null) => CreateRest(group, nodes, comment, nofailback, restricted, type);
                }
                public class PVEStatus
                {
                    private readonly Client _client;

                    internal PVEStatus(Client client) { _client = client; }
                    private PVECurrent _current;
                    public PVECurrent Current => _current ?? (_current = new PVECurrent(_client));
                    private PVEManagerStatus _managerStatus;
                    public PVEManagerStatus ManagerStatus => _managerStatus ?? (_managerStatus = new PVEManagerStatus(_client));
                    public class PVECurrent
                    {
                        private readonly Client _client;

                        internal PVECurrent(Client client) { _client = client; }
                        /// <summary>
                        /// Get HA manger status.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/ha/status/current"); }

                        /// <summary>
                        /// Get HA manger status.
                        /// </summary>
                        /// <returns></returns>
                        public Result Status() => GetRest();
                    }
                    public class PVEManagerStatus
                    {
                        private readonly Client _client;

                        internal PVEManagerStatus(Client client) { _client = client; }
                        /// <summary>
                        /// Get full HA manger status, including LRM status.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/ha/status/manager_status"); }

                        /// <summary>
                        /// Get full HA manger status, including LRM status.
                        /// </summary>
                        /// <returns></returns>
                        public Result ManagerStatus() => GetRest();
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/ha/status"); }

                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/ha"); }

                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
            }
            public class PVEAcme
            {
                private readonly Client _client;

                internal PVEAcme(Client client) { _client = client; }
                private PVEAccount _account;
                public PVEAccount Account => _account ?? (_account = new PVEAccount(_client));
                private PVETos _tos;
                public PVETos Tos => _tos ?? (_tos = new PVETos(_client));
                private PVEDirectories _directories;
                public PVEDirectories Directories => _directories ?? (_directories = new PVEDirectories(_client));
                public class PVEAccount
                {
                    private readonly Client _client;

                    internal PVEAccount(Client client) { _client = client; }
                    public PVEItemName this[object name] => new PVEItemName(_client, name);
                    public class PVEItemName
                    {
                        private readonly Client _client;
                        private readonly object _name;
                        internal PVEItemName(Client client, object name) { _client = client; _name = name; }
                        /// <summary>
                        /// Deactivate existing ACME account at CA.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/cluster/acme/account/{_name}"); }

                        /// <summary>
                        /// Deactivate existing ACME account at CA.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeactivateAccount() => DeleteRest();
                        /// <summary>
                        /// Return existing ACME account information.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/cluster/acme/account/{_name}"); }

                        /// <summary>
                        /// Return existing ACME account information.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetAccount() => GetRest();
                        /// <summary>
                        /// Update existing ACME account information with CA. Note: not specifying any new account information triggers a refresh.
                        /// </summary>
                        /// <param name="contact">Contact email addresses.</param>
                        /// <returns></returns>
                        public Result SetRest(string contact = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("contact", contact);
                            return _client.Set($"/cluster/acme/account/{_name}", parameters);
                        }

                        /// <summary>
                        /// Update existing ACME account information with CA. Note: not specifying any new account information triggers a refresh.
                        /// </summary>
                        /// <param name="contact">Contact email addresses.</param>
                        /// <returns></returns>
                        public Result UpdateAccount(string contact = null) => SetRest(contact);
                    }
                    /// <summary>
                    /// ACMEAccount index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/acme/account"); }

                    /// <summary>
                    /// ACMEAccount index.
                    /// </summary>
                    /// <returns></returns>
                    public Result AccountIndex() => GetRest();
                    /// <summary>
                    /// Register a new ACME account with CA.
                    /// </summary>
                    /// <param name="contact">Contact email addresses.</param>
                    /// <param name="directory">URL of ACME CA directory endpoint.</param>
                    /// <param name="name">ACME account config file name.</param>
                    /// <param name="tos_url">URL of CA TermsOfService - setting this indicates agreement.</param>
                    /// <returns></returns>
                    public Result CreateRest(string contact, string directory = null, string name = null, string tos_url = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("contact", contact);
                        parameters.Add("directory", directory);
                        parameters.Add("name", name);
                        parameters.Add("tos_url", tos_url);
                        return _client.Create($"/cluster/acme/account", parameters);
                    }

                    /// <summary>
                    /// Register a new ACME account with CA.
                    /// </summary>
                    /// <param name="contact">Contact email addresses.</param>
                    /// <param name="directory">URL of ACME CA directory endpoint.</param>
                    /// <param name="name">ACME account config file name.</param>
                    /// <param name="tos_url">URL of CA TermsOfService - setting this indicates agreement.</param>
                    /// <returns></returns>
                    public Result RegisterAccount(string contact, string directory = null, string name = null, string tos_url = null) => CreateRest(contact, directory, name, tos_url);
                }
                public class PVETos
                {
                    private readonly Client _client;

                    internal PVETos(Client client) { _client = client; }
                    /// <summary>
                    /// Retrieve ACME TermsOfService URL from CA.
                    /// </summary>
                    /// <param name="directory">URL of ACME CA directory endpoint.</param>
                    /// <returns></returns>
                    public Result GetRest(string directory = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("directory", directory);
                        return _client.Get($"/cluster/acme/tos", parameters);
                    }

                    /// <summary>
                    /// Retrieve ACME TermsOfService URL from CA.
                    /// </summary>
                    /// <param name="directory">URL of ACME CA directory endpoint.</param>
                    /// <returns></returns>
                    public Result GetTos(string directory = null) => GetRest(directory);
                }
                public class PVEDirectories
                {
                    private readonly Client _client;

                    internal PVEDirectories(Client client) { _client = client; }
                    /// <summary>
                    /// Get named known ACME directory endpoints.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/acme/directories"); }

                    /// <summary>
                    /// Get named known ACME directory endpoints.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetDirectories() => GetRest();
                }
                /// <summary>
                /// ACMEAccount index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/acme"); }

                /// <summary>
                /// ACMEAccount index.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
            }
            public class PVELog
            {
                private readonly Client _client;

                internal PVELog(Client client) { _client = client; }
                /// <summary>
                /// Read cluster log
                /// </summary>
                /// <param name="max">Maximum number of entries.</param>
                /// <returns></returns>
                public Result GetRest(int? max = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("max", max);
                    return _client.Get($"/cluster/log", parameters);
                }

                /// <summary>
                /// Read cluster log
                /// </summary>
                /// <param name="max">Maximum number of entries.</param>
                /// <returns></returns>
                public Result Log(int? max = null) => GetRest(max);
            }
            public class PVEResources
            {
                private readonly Client _client;

                internal PVEResources(Client client) { _client = client; }
                /// <summary>
                /// Resources index (cluster wide).
                /// </summary>
                /// <param name="type">
                ///   Enum: vm,storage,node</param>
                /// <returns></returns>
                public Result GetRest(string type = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("type", type);
                    return _client.Get($"/cluster/resources", parameters);
                }

                /// <summary>
                /// Resources index (cluster wide).
                /// </summary>
                /// <param name="type">
                ///   Enum: vm,storage,node</param>
                /// <returns></returns>
                public Result Resources(string type = null) => GetRest(type);
            }
            public class PVETasks
            {
                private readonly Client _client;

                internal PVETasks(Client client) { _client = client; }
                /// <summary>
                /// List recent tasks (cluster wide).
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/tasks"); }

                /// <summary>
                /// List recent tasks (cluster wide).
                /// </summary>
                /// <returns></returns>
                public Result Tasks() => GetRest();
            }
            public class PVEOptions
            {
                private readonly Client _client;

                internal PVEOptions(Client client) { _client = client; }
                /// <summary>
                /// Get datacenter options.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/options"); }

                /// <summary>
                /// Get datacenter options.
                /// </summary>
                /// <returns></returns>
                public Result GetOptions() => GetRest();
                /// <summary>
                /// Set datacenter options.
                /// </summary>
                /// <param name="bwlimit">Set bandwidth/io limits various operations.</param>
                /// <param name="console">Select the default Console viewer. You can either use the builtin java applet (VNC; deprecated and maps to html5), an external virt-viewer comtatible application (SPICE), an HTML5 based vnc viewer (noVNC), or an HTML5 based console client (xtermjs). If the selected viewer is not available (e.g. SPICE not activated for the VM), the fallback is noVNC.
                ///   Enum: applet,vv,html5,xtermjs</param>
                /// <param name="delete">A list of settings you want to delete.</param>
                /// <param name="email_from">Specify email address to send notification from (default is root@$hostname)</param>
                /// <param name="fencing">Set the fencing mode of the HA cluster. Hardware mode needs a valid configuration of fence devices in /etc/pve/ha/fence.cfg. With both all two modes are used.  WARNING: 'hardware' and 'both' are EXPERIMENTAL &amp; WIP
                ///   Enum: watchdog,hardware,both</param>
                /// <param name="ha">Cluster wide HA settings.</param>
                /// <param name="http_proxy">Specify external http proxy which is used for downloads (example: 'http://username:password@host:port/')</param>
                /// <param name="keyboard">Default keybord layout for vnc server.
                ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                /// <param name="language">Default GUI language.
                ///   Enum: zh_CN,zh_TW,ca,en,eu,fr,de,it,es,ja,nb,nn,fa,pl,pt_BR,ru,sl,sv,tr</param>
                /// <param name="mac_prefix">Prefix for autogenerated MAC addresses.</param>
                /// <param name="max_workers">Defines how many workers (per node) are maximal started  on actions like 'stopall VMs' or task from the ha-manager.</param>
                /// <param name="migration">For cluster wide migration settings.</param>
                /// <param name="migration_unsecure">Migration is secure using SSH tunnel by default. For secure private networks you can disable it to speed up migration. Deprecated, use the 'migration' property instead!</param>
                /// <param name="u2f">u2f</param>
                /// <returns></returns>
                public Result SetRest(string bwlimit = null, string console = null, string delete = null, string email_from = null, string fencing = null, string ha = null, string http_proxy = null, string keyboard = null, string language = null, string mac_prefix = null, int? max_workers = null, string migration = null, bool? migration_unsecure = null, string u2f = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("bwlimit", bwlimit);
                    parameters.Add("console", console);
                    parameters.Add("delete", delete);
                    parameters.Add("email_from", email_from);
                    parameters.Add("fencing", fencing);
                    parameters.Add("ha", ha);
                    parameters.Add("http_proxy", http_proxy);
                    parameters.Add("keyboard", keyboard);
                    parameters.Add("language", language);
                    parameters.Add("mac_prefix", mac_prefix);
                    parameters.Add("max_workers", max_workers);
                    parameters.Add("migration", migration);
                    parameters.Add("migration_unsecure", migration_unsecure);
                    parameters.Add("u2f", u2f);
                    return _client.Set($"/cluster/options", parameters);
                }

                /// <summary>
                /// Set datacenter options.
                /// </summary>
                /// <param name="bwlimit">Set bandwidth/io limits various operations.</param>
                /// <param name="console">Select the default Console viewer. You can either use the builtin java applet (VNC; deprecated and maps to html5), an external virt-viewer comtatible application (SPICE), an HTML5 based vnc viewer (noVNC), or an HTML5 based console client (xtermjs). If the selected viewer is not available (e.g. SPICE not activated for the VM), the fallback is noVNC.
                ///   Enum: applet,vv,html5,xtermjs</param>
                /// <param name="delete">A list of settings you want to delete.</param>
                /// <param name="email_from">Specify email address to send notification from (default is root@$hostname)</param>
                /// <param name="fencing">Set the fencing mode of the HA cluster. Hardware mode needs a valid configuration of fence devices in /etc/pve/ha/fence.cfg. With both all two modes are used.  WARNING: 'hardware' and 'both' are EXPERIMENTAL &amp; WIP
                ///   Enum: watchdog,hardware,both</param>
                /// <param name="ha">Cluster wide HA settings.</param>
                /// <param name="http_proxy">Specify external http proxy which is used for downloads (example: 'http://username:password@host:port/')</param>
                /// <param name="keyboard">Default keybord layout for vnc server.
                ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                /// <param name="language">Default GUI language.
                ///   Enum: zh_CN,zh_TW,ca,en,eu,fr,de,it,es,ja,nb,nn,fa,pl,pt_BR,ru,sl,sv,tr</param>
                /// <param name="mac_prefix">Prefix for autogenerated MAC addresses.</param>
                /// <param name="max_workers">Defines how many workers (per node) are maximal started  on actions like 'stopall VMs' or task from the ha-manager.</param>
                /// <param name="migration">For cluster wide migration settings.</param>
                /// <param name="migration_unsecure">Migration is secure using SSH tunnel by default. For secure private networks you can disable it to speed up migration. Deprecated, use the 'migration' property instead!</param>
                /// <param name="u2f">u2f</param>
                /// <returns></returns>
                public Result SetOptions(string bwlimit = null, string console = null, string delete = null, string email_from = null, string fencing = null, string ha = null, string http_proxy = null, string keyboard = null, string language = null, string mac_prefix = null, int? max_workers = null, string migration = null, bool? migration_unsecure = null, string u2f = null) => SetRest(bwlimit, console, delete, email_from, fencing, ha, http_proxy, keyboard, language, mac_prefix, max_workers, migration, migration_unsecure, u2f);
            }
            public class PVEStatus
            {
                private readonly Client _client;

                internal PVEStatus(Client client) { _client = client; }
                /// <summary>
                /// Get cluster status information.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/status"); }

                /// <summary>
                /// Get cluster status information.
                /// </summary>
                /// <returns></returns>
                public Result GetStatus() => GetRest();
            }
            public class PVENextid
            {
                private readonly Client _client;

                internal PVENextid(Client client) { _client = client; }
                /// <summary>
                /// Get next free VMID. If you pass an VMID it will raise an error if the ID is already used.
                /// </summary>
                /// <param name="vmid">The (unique) ID of the VM.</param>
                /// <returns></returns>
                public Result GetRest(int? vmid = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("vmid", vmid);
                    return _client.Get($"/cluster/nextid", parameters);
                }

                /// <summary>
                /// Get next free VMID. If you pass an VMID it will raise an error if the ID is already used.
                /// </summary>
                /// <param name="vmid">The (unique) ID of the VM.</param>
                /// <returns></returns>
                public Result Nextid(int? vmid = null) => GetRest(vmid);
            }
            public class PVECeph
            {
                private readonly Client _client;

                internal PVECeph(Client client) { _client = client; }
                private PVEMetadata _metadata;
                public PVEMetadata Metadata => _metadata ?? (_metadata = new PVEMetadata(_client));
                private PVEStatus _status;
                public PVEStatus Status => _status ?? (_status = new PVEStatus(_client));
                public class PVEMetadata
                {
                    private readonly Client _client;

                    internal PVEMetadata(Client client) { _client = client; }
                    /// <summary>
                    /// Get ceph metadata.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/ceph/metadata"); }

                    /// <summary>
                    /// Get ceph metadata.
                    /// </summary>
                    /// <returns></returns>
                    public Result CephMetadata() => GetRest();
                }
                public class PVEStatus
                {
                    private readonly Client _client;

                    internal PVEStatus(Client client) { _client = client; }
                    /// <summary>
                    /// Get ceph status.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/cluster/ceph/status"); }

                    /// <summary>
                    /// Get ceph status.
                    /// </summary>
                    /// <returns></returns>
                    public Result Cephstatus() => GetRest();
                }
                /// <summary>
                /// Cluster ceph index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/cluster/ceph"); }

                /// <summary>
                /// Cluster ceph index.
                /// </summary>
                /// <returns></returns>
                public Result Cephindex() => GetRest();
            }
            /// <summary>
            /// Cluster index.
            /// </summary>
            /// <returns></returns>
            public Result GetRest() { return _client.Get($"/cluster"); }

            /// <summary>
            /// Cluster index.
            /// </summary>
            /// <returns></returns>
            public Result Index() => GetRest();
        }
        public class PVENodes
        {
            private readonly Client _client;

            internal PVENodes(Client client) { _client = client; }
            public PVEItemNode this[object node] => new PVEItemNode(_client, node);
            public class PVEItemNode
            {
                private readonly Client _client;
                private readonly object _node;
                internal PVEItemNode(Client client, object node) { _client = client; _node = node; }
                private PVEQemu _qemu;
                public PVEQemu Qemu => _qemu ?? (_qemu = new PVEQemu(_client, _node));
                private PVELxc _lxc;
                public PVELxc Lxc => _lxc ?? (_lxc = new PVELxc(_client, _node));
                private PVECeph _ceph;
                public PVECeph Ceph => _ceph ?? (_ceph = new PVECeph(_client, _node));
                private PVEVzdump _vzdump;
                public PVEVzdump Vzdump => _vzdump ?? (_vzdump = new PVEVzdump(_client, _node));
                private PVEServices _services;
                public PVEServices Services => _services ?? (_services = new PVEServices(_client, _node));
                private PVESubscription _subscription;
                public PVESubscription Subscription => _subscription ?? (_subscription = new PVESubscription(_client, _node));
                private PVENetwork _network;
                public PVENetwork Network => _network ?? (_network = new PVENetwork(_client, _node));
                private PVETasks _tasks;
                public PVETasks Tasks => _tasks ?? (_tasks = new PVETasks(_client, _node));
                private PVEScan _scan;
                public PVEScan Scan => _scan ?? (_scan = new PVEScan(_client, _node));
                private PVEHardware _hardware;
                public PVEHardware Hardware => _hardware ?? (_hardware = new PVEHardware(_client, _node));
                private PVEStorage _storage;
                public PVEStorage Storage => _storage ?? (_storage = new PVEStorage(_client, _node));
                private PVEDisks _disks;
                public PVEDisks Disks => _disks ?? (_disks = new PVEDisks(_client, _node));
                private PVEApt _apt;
                public PVEApt Apt => _apt ?? (_apt = new PVEApt(_client, _node));
                private PVEFirewall _firewall;
                public PVEFirewall Firewall => _firewall ?? (_firewall = new PVEFirewall(_client, _node));
                private PVEReplication _replication;
                public PVEReplication Replication => _replication ?? (_replication = new PVEReplication(_client, _node));
                private PVECertificates _certificates;
                public PVECertificates Certificates => _certificates ?? (_certificates = new PVECertificates(_client, _node));
                private PVEConfig _config;
                public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node));
                private PVEVersion _version;
                public PVEVersion Version => _version ?? (_version = new PVEVersion(_client, _node));
                private PVEStatus _status;
                public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node));
                private PVENetstat _netstat;
                public PVENetstat Netstat => _netstat ?? (_netstat = new PVENetstat(_client, _node));
                private PVEExecute _execute;
                public PVEExecute Execute => _execute ?? (_execute = new PVEExecute(_client, _node));
                private PVEWakeonlan _wakeonlan;
                public PVEWakeonlan Wakeonlan => _wakeonlan ?? (_wakeonlan = new PVEWakeonlan(_client, _node));
                private PVERrd _rrd;
                public PVERrd Rrd => _rrd ?? (_rrd = new PVERrd(_client, _node));
                private PVERrddata _rrddata;
                public PVERrddata Rrddata => _rrddata ?? (_rrddata = new PVERrddata(_client, _node));
                private PVESyslog _syslog;
                public PVESyslog Syslog => _syslog ?? (_syslog = new PVESyslog(_client, _node));
                private PVEJournal _journal;
                public PVEJournal Journal => _journal ?? (_journal = new PVEJournal(_client, _node));
                private PVEVncshell _vncshell;
                public PVEVncshell Vncshell => _vncshell ?? (_vncshell = new PVEVncshell(_client, _node));
                private PVETermproxy _termproxy;
                public PVETermproxy Termproxy => _termproxy ?? (_termproxy = new PVETermproxy(_client, _node));
                private PVEVncwebsocket _vncwebsocket;
                public PVEVncwebsocket Vncwebsocket => _vncwebsocket ?? (_vncwebsocket = new PVEVncwebsocket(_client, _node));
                private PVESpiceshell _spiceshell;
                public PVESpiceshell Spiceshell => _spiceshell ?? (_spiceshell = new PVESpiceshell(_client, _node));
                private PVEDns _dns;
                public PVEDns Dns => _dns ?? (_dns = new PVEDns(_client, _node));
                private PVETime _time;
                public PVETime Time => _time ?? (_time = new PVETime(_client, _node));
                private PVEAplinfo _aplinfo;
                public PVEAplinfo Aplinfo => _aplinfo ?? (_aplinfo = new PVEAplinfo(_client, _node));
                private PVEReport _report;
                public PVEReport Report => _report ?? (_report = new PVEReport(_client, _node));
                private PVEStartall _startall;
                public PVEStartall Startall => _startall ?? (_startall = new PVEStartall(_client, _node));
                private PVEStopall _stopall;
                public PVEStopall Stopall => _stopall ?? (_stopall = new PVEStopall(_client, _node));
                private PVEMigrateall _migrateall;
                public PVEMigrateall Migrateall => _migrateall ?? (_migrateall = new PVEMigrateall(_client, _node));
                private PVEHosts _hosts;
                public PVEHosts Hosts => _hosts ?? (_hosts = new PVEHosts(_client, _node));
                public class PVEQemu
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEQemu(Client client, object node) { _client = client; _node = node; }
                    public PVEItemVmid this[object vmid] => new PVEItemVmid(_client, _node, vmid);
                    public class PVEItemVmid
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        private readonly object _vmid;
                        internal PVEItemVmid(Client client, object node, object vmid)
                        {
                            _client = client; _node = node;
                            _vmid = vmid;
                        }
                        private PVEFirewall _firewall;
                        public PVEFirewall Firewall => _firewall ?? (_firewall = new PVEFirewall(_client, _node, _vmid));
                        private PVEAgent _agent;
                        public PVEAgent Agent => _agent ?? (_agent = new PVEAgent(_client, _node, _vmid));
                        private PVERrd _rrd;
                        public PVERrd Rrd => _rrd ?? (_rrd = new PVERrd(_client, _node, _vmid));
                        private PVERrddata _rrddata;
                        public PVERrddata Rrddata => _rrddata ?? (_rrddata = new PVERrddata(_client, _node, _vmid));
                        private PVEConfig _config;
                        public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node, _vmid));
                        private PVEPending _pending;
                        public PVEPending Pending => _pending ?? (_pending = new PVEPending(_client, _node, _vmid));
                        private PVEUnlink _unlink;
                        public PVEUnlink Unlink => _unlink ?? (_unlink = new PVEUnlink(_client, _node, _vmid));
                        private PVEVncproxy _vncproxy;
                        public PVEVncproxy Vncproxy => _vncproxy ?? (_vncproxy = new PVEVncproxy(_client, _node, _vmid));
                        private PVETermproxy _termproxy;
                        public PVETermproxy Termproxy => _termproxy ?? (_termproxy = new PVETermproxy(_client, _node, _vmid));
                        private PVEVncwebsocket _vncwebsocket;
                        public PVEVncwebsocket Vncwebsocket => _vncwebsocket ?? (_vncwebsocket = new PVEVncwebsocket(_client, _node, _vmid));
                        private PVESpiceproxy _spiceproxy;
                        public PVESpiceproxy Spiceproxy => _spiceproxy ?? (_spiceproxy = new PVESpiceproxy(_client, _node, _vmid));
                        private PVEStatus _status;
                        public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node, _vmid));
                        private PVESendkey _sendkey;
                        public PVESendkey Sendkey => _sendkey ?? (_sendkey = new PVESendkey(_client, _node, _vmid));
                        private PVEFeature _feature;
                        public PVEFeature Feature => _feature ?? (_feature = new PVEFeature(_client, _node, _vmid));
                        private PVEClone _clone;
                        public PVEClone Clone => _clone ?? (_clone = new PVEClone(_client, _node, _vmid));
                        private PVEMoveDisk _moveDisk;
                        public PVEMoveDisk MoveDisk => _moveDisk ?? (_moveDisk = new PVEMoveDisk(_client, _node, _vmid));
                        private PVEMigrate _migrate;
                        public PVEMigrate Migrate => _migrate ?? (_migrate = new PVEMigrate(_client, _node, _vmid));
                        private PVEMonitor _monitor;
                        public PVEMonitor Monitor => _monitor ?? (_monitor = new PVEMonitor(_client, _node, _vmid));
                        private PVEResize _resize;
                        public PVEResize Resize => _resize ?? (_resize = new PVEResize(_client, _node, _vmid));
                        private PVESnapshot _snapshot;
                        public PVESnapshot Snapshot => _snapshot ?? (_snapshot = new PVESnapshot(_client, _node, _vmid));
                        private PVETemplate _template;
                        public PVETemplate Template => _template ?? (_template = new PVETemplate(_client, _node, _vmid));
                        private PVECloudinit _cloudinit;
                        public PVECloudinit Cloudinit => _cloudinit ?? (_cloudinit = new PVECloudinit(_client, _node, _vmid));
                        public class PVEFirewall
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEFirewall(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PVERules _rules;
                            public PVERules Rules => _rules ?? (_rules = new PVERules(_client, _node, _vmid));
                            private PVEAliases _aliases;
                            public PVEAliases Aliases => _aliases ?? (_aliases = new PVEAliases(_client, _node, _vmid));
                            private PVEIpset _ipset;
                            public PVEIpset Ipset => _ipset ?? (_ipset = new PVEIpset(_client, _node, _vmid));
                            private PVEOptions _options;
                            public PVEOptions Options => _options ?? (_options = new PVEOptions(_client, _node, _vmid));
                            private PVELog _log;
                            public PVELog Log => _log ?? (_log = new PVELog(_client, _node, _vmid));
                            private PVERefs _refs;
                            public PVERefs Refs => _refs ?? (_refs = new PVERefs(_client, _node, _vmid));
                            public class PVERules
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVERules(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemPos this[object pos] => new PVEItemPos(_client, _node, _vmid, pos);
                                public class PVEItemPos
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _pos;
                                    internal PVEItemPos(Client client, object node, object vmid, object pos)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _pos = pos;
                                    }
                                    /// <summary>
                                    /// Delete rule.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public Result DeleteRest(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        return _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}", parameters);
                                    }

                                    /// <summary>
                                    /// Delete rule.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public Result DeleteRule(string digest = null) => DeleteRest(digest);
                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}"); }

                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRule() => GetRest();
                                    /// <summary>
                                    /// Modify rule data.
                                    /// </summary>
                                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                    /// <param name="comment">Descriptive comment.</param>
                                    /// <param name="delete">A list of settings you want to delete.</param>
                                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="enable">Flag to enable/disable a rule.</param>
                                    /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                    /// <param name="log">Log level for firewall rule.
                                    ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                    /// <param name="macro">Use predefined standard macro.</param>
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    /// <returns></returns>
                                    public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("action", action);
                                        parameters.Add("comment", comment);
                                        parameters.Add("delete", delete);
                                        parameters.Add("dest", dest);
                                        parameters.Add("digest", digest);
                                        parameters.Add("dport", dport);
                                        parameters.Add("enable", enable);
                                        parameters.Add("iface", iface);
                                        parameters.Add("log", log);
                                        parameters.Add("macro", macro);
                                        parameters.Add("moveto", moveto);
                                        parameters.Add("proto", proto);
                                        parameters.Add("source", source);
                                        parameters.Add("sport", sport);
                                        parameters.Add("type", type);
                                        return _client.Set($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}", parameters);
                                    }

                                    /// <summary>
                                    /// Modify rule data.
                                    /// </summary>
                                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                    /// <param name="comment">Descriptive comment.</param>
                                    /// <param name="delete">A list of settings you want to delete.</param>
                                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="enable">Flag to enable/disable a rule.</param>
                                    /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                    /// <param name="log">Log level for firewall rule.
                                    ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                    /// <param name="macro">Use predefined standard macro.</param>
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    /// <returns></returns>
                                    public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) => SetRest(action, comment, delete, dest, digest, dport, enable, iface, log, macro, moveto, proto, source, sport, type);
                                }
                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/rules"); }

                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRules() => GetRest();
                                /// <summary>
                                /// Create new rule.
                                /// </summary>
                                /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                /// <param name="type">Rule type.
                                ///   Enum: in,out,group</param>
                                /// <param name="comment">Descriptive comment.</param>
                                /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <param name="enable">Flag to enable/disable a rule.</param>
                                /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                /// <param name="log">Log level for firewall rule.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("action", action);
                                    parameters.Add("type", type);
                                    parameters.Add("comment", comment);
                                    parameters.Add("dest", dest);
                                    parameters.Add("digest", digest);
                                    parameters.Add("dport", dport);
                                    parameters.Add("enable", enable);
                                    parameters.Add("iface", iface);
                                    parameters.Add("log", log);
                                    parameters.Add("macro", macro);
                                    parameters.Add("pos", pos);
                                    parameters.Add("proto", proto);
                                    parameters.Add("source", source);
                                    parameters.Add("sport", sport);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/firewall/rules", parameters);
                                }

                                /// <summary>
                                /// Create new rule.
                                /// </summary>
                                /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                /// <param name="type">Rule type.
                                ///   Enum: in,out,group</param>
                                /// <param name="comment">Descriptive comment.</param>
                                /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <param name="enable">Flag to enable/disable a rule.</param>
                                /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                /// <param name="log">Log level for firewall rule.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) => CreateRest(action, type, comment, dest, digest, dport, enable, iface, log, macro, pos, proto, source, sport);
                            }
                            public class PVEAliases
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEAliases(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemName this[object name] => new PVEItemName(_client, _node, _vmid, name);
                                public class PVEItemName
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _name;
                                    internal PVEItemName(Client client, object node, object vmid, object name)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }
                                    /// <summary>
                                    /// Remove IP or Network alias.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public Result DeleteRest(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        return _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}", parameters);
                                    }

                                    /// <summary>
                                    /// Remove IP or Network alias.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public Result RemoveAlias(string digest = null) => DeleteRest(digest);
                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}"); }

                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result ReadAlias() => GetRest();
                                    /// <summary>
                                    /// Update IP or Network alias.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="rename">Rename an existing alias.</param>
                                    /// <returns></returns>
                                    public Result SetRest(string cidr, string comment = null, string digest = null, string rename = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("digest", digest);
                                        parameters.Add("rename", rename);
                                        return _client.Set($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}", parameters);
                                    }

                                    /// <summary>
                                    /// Update IP or Network alias.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="rename">Rename an existing alias.</param>
                                    /// <returns></returns>
                                    public Result UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null) => SetRest(cidr, comment, digest, rename);
                                }
                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases"); }

                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public Result GetAliases() => GetRest();
                                /// <summary>
                                /// Create IP or Network Alias.
                                /// </summary>
                                /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                /// <param name="name">Alias name.</param>
                                /// <param name="comment"></param>
                                /// <returns></returns>
                                public Result CreateRest(string cidr, string name, string comment = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("cidr", cidr);
                                    parameters.Add("name", name);
                                    parameters.Add("comment", comment);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases", parameters);
                                }

                                /// <summary>
                                /// Create IP or Network Alias.
                                /// </summary>
                                /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                /// <param name="name">Alias name.</param>
                                /// <param name="comment"></param>
                                /// <returns></returns>
                                public Result CreateAlias(string cidr, string name, string comment = null) => CreateRest(cidr, name, comment);
                            }
                            public class PVEIpset
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEIpset(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemName this[object name] => new PVEItemName(_client, _node, _vmid, name);
                                public class PVEItemName
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _name;
                                    internal PVEItemName(Client client, object node, object vmid, object name)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }
                                    public PVEItemCidr this[object cidr] => new PVEItemCidr(_client, _node, _vmid, _name, cidr);
                                    public class PVEItemCidr
                                    {
                                        private readonly Client _client;
                                        private readonly object _node;
                                        private readonly object _vmid;
                                        private readonly object _name;
                                        private readonly object _cidr;
                                        internal PVEItemCidr(Client client, object node, object vmid, object name, object cidr)
                                        {
                                            _client = client; _node = node;
                                            _vmid = vmid;
                                            _name = name;
                                            _cidr = cidr;
                                        }
                                        /// <summary>
                                        /// Remove IP or Network from IPSet.
                                        /// </summary>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <returns></returns>
                                        public Result DeleteRest(string digest = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("digest", digest);
                                            return _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}", parameters);
                                        }

                                        /// <summary>
                                        /// Remove IP or Network from IPSet.
                                        /// </summary>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <returns></returns>
                                        public Result RemoveIp(string digest = null) => DeleteRest(digest);
                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}"); }

                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public Result ReadIp() => GetRest();
                                        /// <summary>
                                        /// Update IP or Network settings
                                        /// </summary>
                                        /// <param name="comment"></param>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <param name="nomatch"></param>
                                        /// <returns></returns>
                                        public Result SetRest(string comment = null, string digest = null, bool? nomatch = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("comment", comment);
                                            parameters.Add("digest", digest);
                                            parameters.Add("nomatch", nomatch);
                                            return _client.Set($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}", parameters);
                                        }

                                        /// <summary>
                                        /// Update IP or Network settings
                                        /// </summary>
                                        /// <param name="comment"></param>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <param name="nomatch"></param>
                                        /// <returns></returns>
                                        public Result UpdateIp(string comment = null, string digest = null, bool? nomatch = null) => SetRest(comment, digest, nomatch);
                                    }
                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}"); }

                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result DeleteIpset() => DeleteRest();
                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}"); }

                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetIpset() => GetRest();
                                    /// <summary>
                                    /// Add IP or Network to IPSet.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="nomatch"></param>
                                    /// <returns></returns>
                                    public Result CreateRest(string cidr, string comment = null, bool? nomatch = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("nomatch", nomatch);
                                        return _client.Create($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}", parameters);
                                    }

                                    /// <summary>
                                    /// Add IP or Network to IPSet.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="nomatch"></param>
                                    /// <returns></returns>
                                    public Result CreateIp(string cidr, string comment = null, bool? nomatch = null) => CreateRest(cidr, comment, nomatch);
                                }
                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset"); }

                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public Result IpsetIndex() => GetRest();
                                /// <summary>
                                /// Create new IPSet
                                /// </summary>
                                /// <param name="name">IP set name.</param>
                                /// <param name="comment"></param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                                /// <returns></returns>
                                public Result CreateRest(string name, string comment = null, string digest = null, string rename = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("name", name);
                                    parameters.Add("comment", comment);
                                    parameters.Add("digest", digest);
                                    parameters.Add("rename", rename);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset", parameters);
                                }

                                /// <summary>
                                /// Create new IPSet
                                /// </summary>
                                /// <param name="name">IP set name.</param>
                                /// <param name="comment"></param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                                /// <returns></returns>
                                public Result CreateIpset(string name, string comment = null, string digest = null, string rename = null) => CreateRest(name, comment, digest, rename);
                            }
                            public class PVEOptions
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEOptions(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/options"); }

                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetOptions() => GetRest();
                                /// <summary>
                                /// Set Firewall options.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp">Enable DHCP.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="enable">Enable/disable firewall rules.</param>
                                /// <param name="ipfilter">Enable default IP filters. This is equivalent to adding an empty ipfilter-net&amp;lt;id&amp;gt; ipset for every interface. Such ipsets implicitly contain sane default restrictions such as restricting IPv6 link local addresses to the one derived from the interface's MAC address. For containers the configured IP addresses will be implicitly added.</param>
                                /// <param name="log_level_in">Log level for incoming traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="log_level_out">Log level for outgoing traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macfilter">Enable/disable MAC address filter.</param>
                                /// <param name="ndp">Enable NDP.</param>
                                /// <param name="policy_in">Input policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="policy_out">Output policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="radv">Allow sending Router Advertisement.</param>
                                /// <returns></returns>
                                public Result SetRest(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("delete", delete);
                                    parameters.Add("dhcp", dhcp);
                                    parameters.Add("digest", digest);
                                    parameters.Add("enable", enable);
                                    parameters.Add("ipfilter", ipfilter);
                                    parameters.Add("log_level_in", log_level_in);
                                    parameters.Add("log_level_out", log_level_out);
                                    parameters.Add("macfilter", macfilter);
                                    parameters.Add("ndp", ndp);
                                    parameters.Add("policy_in", policy_in);
                                    parameters.Add("policy_out", policy_out);
                                    parameters.Add("radv", radv);
                                    return _client.Set($"/nodes/{_node}/qemu/{_vmid}/firewall/options", parameters);
                                }

                                /// <summary>
                                /// Set Firewall options.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp">Enable DHCP.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="enable">Enable/disable firewall rules.</param>
                                /// <param name="ipfilter">Enable default IP filters. This is equivalent to adding an empty ipfilter-net&amp;lt;id&amp;gt; ipset for every interface. Such ipsets implicitly contain sane default restrictions such as restricting IPv6 link local addresses to the one derived from the interface's MAC address. For containers the configured IP addresses will be implicitly added.</param>
                                /// <param name="log_level_in">Log level for incoming traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="log_level_out">Log level for outgoing traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macfilter">Enable/disable MAC address filter.</param>
                                /// <param name="ndp">Enable NDP.</param>
                                /// <param name="policy_in">Input policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="policy_out">Output policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="radv">Allow sending Router Advertisement.</param>
                                /// <returns></returns>
                                public Result SetOptions(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null) => SetRest(delete, dhcp, digest, enable, ipfilter, log_level_in, log_level_out, macfilter, ndp, policy_in, policy_out, radv);
                            }
                            public class PVELog
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVELog(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Read firewall log
                                /// </summary>
                                /// <param name="limit"></param>
                                /// <param name="start"></param>
                                /// <returns></returns>
                                public Result GetRest(int? limit = null, int? start = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("limit", limit);
                                    parameters.Add("start", start);
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/log", parameters);
                                }

                                /// <summary>
                                /// Read firewall log
                                /// </summary>
                                /// <param name="limit"></param>
                                /// <param name="start"></param>
                                /// <returns></returns>
                                public Result Log(int? limit = null, int? start = null) => GetRest(limit, start);
                            }
                            public class PVERefs
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVERefs(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                                /// </summary>
                                /// <param name="type">Only list references of specified type.
                                ///   Enum: alias,ipset</param>
                                /// <returns></returns>
                                public Result GetRest(string type = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/refs", parameters);
                                }

                                /// <summary>
                                /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                                /// </summary>
                                /// <param name="type">Only list references of specified type.
                                ///   Enum: alias,ipset</param>
                                /// <returns></returns>
                                public Result Refs(string type = null) => GetRest(type);
                            }
                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall"); }

                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public Result Index() => GetRest();
                        }
                        public class PVEAgent
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEAgent(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PVEFsfreeze_Freeze _fsfreeze_Freeze;
                            public PVEFsfreeze_Freeze Fsfreeze_Freeze => _fsfreeze_Freeze ?? (_fsfreeze_Freeze = new PVEFsfreeze_Freeze(_client, _node, _vmid));
                            private PVEFsfreeze_Status _fsfreeze_Status;
                            public PVEFsfreeze_Status Fsfreeze_Status => _fsfreeze_Status ?? (_fsfreeze_Status = new PVEFsfreeze_Status(_client, _node, _vmid));
                            private PVEFsfreeze_Thaw _fsfreeze_Thaw;
                            public PVEFsfreeze_Thaw Fsfreeze_Thaw => _fsfreeze_Thaw ?? (_fsfreeze_Thaw = new PVEFsfreeze_Thaw(_client, _node, _vmid));
                            private PVEFstrim _fstrim;
                            public PVEFstrim Fstrim => _fstrim ?? (_fstrim = new PVEFstrim(_client, _node, _vmid));
                            private PVEGet_Fsinfo _get_Fsinfo;
                            public PVEGet_Fsinfo Get_Fsinfo => _get_Fsinfo ?? (_get_Fsinfo = new PVEGet_Fsinfo(_client, _node, _vmid));
                            private PVEGet_Host_Name _get_Host_Name;
                            public PVEGet_Host_Name Get_Host_Name => _get_Host_Name ?? (_get_Host_Name = new PVEGet_Host_Name(_client, _node, _vmid));
                            private PVEGet_Memory_Block_Info _get_Memory_Block_Info;
                            public PVEGet_Memory_Block_Info Get_Memory_Block_Info => _get_Memory_Block_Info ?? (_get_Memory_Block_Info = new PVEGet_Memory_Block_Info(_client, _node, _vmid));
                            private PVEGet_Memory_Blocks _get_Memory_Blocks;
                            public PVEGet_Memory_Blocks Get_Memory_Blocks => _get_Memory_Blocks ?? (_get_Memory_Blocks = new PVEGet_Memory_Blocks(_client, _node, _vmid));
                            private PVEGet_Osinfo _get_Osinfo;
                            public PVEGet_Osinfo Get_Osinfo => _get_Osinfo ?? (_get_Osinfo = new PVEGet_Osinfo(_client, _node, _vmid));
                            private PVEGet_Time _get_Time;
                            public PVEGet_Time Get_Time => _get_Time ?? (_get_Time = new PVEGet_Time(_client, _node, _vmid));
                            private PVEGet_Timezone _get_Timezone;
                            public PVEGet_Timezone Get_Timezone => _get_Timezone ?? (_get_Timezone = new PVEGet_Timezone(_client, _node, _vmid));
                            private PVEGet_Users _get_Users;
                            public PVEGet_Users Get_Users => _get_Users ?? (_get_Users = new PVEGet_Users(_client, _node, _vmid));
                            private PVEGet_Vcpus _get_Vcpus;
                            public PVEGet_Vcpus Get_Vcpus => _get_Vcpus ?? (_get_Vcpus = new PVEGet_Vcpus(_client, _node, _vmid));
                            private PVEInfo _info;
                            public PVEInfo Info => _info ?? (_info = new PVEInfo(_client, _node, _vmid));
                            private PVENetwork_Get_Interfaces _network_Get_Interfaces;
                            public PVENetwork_Get_Interfaces Network_Get_Interfaces => _network_Get_Interfaces ?? (_network_Get_Interfaces = new PVENetwork_Get_Interfaces(_client, _node, _vmid));
                            private PVEPing _ping;
                            public PVEPing Ping => _ping ?? (_ping = new PVEPing(_client, _node, _vmid));
                            private PVEShutdown _shutdown;
                            public PVEShutdown Shutdown => _shutdown ?? (_shutdown = new PVEShutdown(_client, _node, _vmid));
                            private PVESuspend_Disk _suspend_Disk;
                            public PVESuspend_Disk Suspend_Disk => _suspend_Disk ?? (_suspend_Disk = new PVESuspend_Disk(_client, _node, _vmid));
                            private PVESuspend_Hybrid _suspend_Hybrid;
                            public PVESuspend_Hybrid Suspend_Hybrid => _suspend_Hybrid ?? (_suspend_Hybrid = new PVESuspend_Hybrid(_client, _node, _vmid));
                            private PVESuspend_Ram _suspend_Ram;
                            public PVESuspend_Ram Suspend_Ram => _suspend_Ram ?? (_suspend_Ram = new PVESuspend_Ram(_client, _node, _vmid));
                            private PVESet_User_Password _set_User_Password;
                            public PVESet_User_Password Set_User_Password => _set_User_Password ?? (_set_User_Password = new PVESet_User_Password(_client, _node, _vmid));
                            private PVEExec _exec;
                            public PVEExec Exec => _exec ?? (_exec = new PVEExec(_client, _node, _vmid));
                            private PVEExec_Status _exec_Status;
                            public PVEExec_Status Exec_Status => _exec_Status ?? (_exec_Status = new PVEExec_Status(_client, _node, _vmid));
                            private PVEFile_Read _file_Read;
                            public PVEFile_Read File_Read => _file_Read ?? (_file_Read = new PVEFile_Read(_client, _node, _vmid));
                            private PVEFile_Write _file_Write;
                            public PVEFile_Write File_Write => _file_Write ?? (_file_Write = new PVEFile_Write(_client, _node, _vmid));
                            public class PVEFsfreeze_Freeze
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEFsfreeze_Freeze(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-freeze.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-freeze"); }

                                /// <summary>
                                /// Execute fsfreeze-freeze.
                                /// </summary>
                                /// <returns></returns>
                                public Result Fsfreeze_Freeze() => CreateRest();
                            }
                            public class PVEFsfreeze_Status
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEFsfreeze_Status(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-status.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-status"); }

                                /// <summary>
                                /// Execute fsfreeze-status.
                                /// </summary>
                                /// <returns></returns>
                                public Result Fsfreeze_Status() => CreateRest();
                            }
                            public class PVEFsfreeze_Thaw
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEFsfreeze_Thaw(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-thaw.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-thaw"); }

                                /// <summary>
                                /// Execute fsfreeze-thaw.
                                /// </summary>
                                /// <returns></returns>
                                public Result Fsfreeze_Thaw() => CreateRest();
                            }
                            public class PVEFstrim
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEFstrim(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fstrim.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fstrim"); }

                                /// <summary>
                                /// Execute fstrim.
                                /// </summary>
                                /// <returns></returns>
                                public Result Fstrim() => CreateRest();
                            }
                            public class PVEGet_Fsinfo
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Fsinfo(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-fsinfo.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-fsinfo"); }

                                /// <summary>
                                /// Execute get-fsinfo.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Fsinfo() => GetRest();
                            }
                            public class PVEGet_Host_Name
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Host_Name(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-host-name.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-host-name"); }

                                /// <summary>
                                /// Execute get-host-name.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Host_Name() => GetRest();
                            }
                            public class PVEGet_Memory_Block_Info
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Memory_Block_Info(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-memory-block-info.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-memory-block-info"); }

                                /// <summary>
                                /// Execute get-memory-block-info.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Memory_Block_Info() => GetRest();
                            }
                            public class PVEGet_Memory_Blocks
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Memory_Blocks(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-memory-blocks.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-memory-blocks"); }

                                /// <summary>
                                /// Execute get-memory-blocks.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Memory_Blocks() => GetRest();
                            }
                            public class PVEGet_Osinfo
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Osinfo(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-osinfo.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-osinfo"); }

                                /// <summary>
                                /// Execute get-osinfo.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Osinfo() => GetRest();
                            }
                            public class PVEGet_Time
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Time(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-time.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-time"); }

                                /// <summary>
                                /// Execute get-time.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Time() => GetRest();
                            }
                            public class PVEGet_Timezone
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Timezone(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-timezone.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-timezone"); }

                                /// <summary>
                                /// Execute get-timezone.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Timezone() => GetRest();
                            }
                            public class PVEGet_Users
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Users(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-users.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-users"); }

                                /// <summary>
                                /// Execute get-users.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Users() => GetRest();
                            }
                            public class PVEGet_Vcpus
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEGet_Vcpus(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-vcpus.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-vcpus"); }

                                /// <summary>
                                /// Execute get-vcpus.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Vcpus() => GetRest();
                            }
                            public class PVEInfo
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEInfo(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute info.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/info"); }

                                /// <summary>
                                /// Execute info.
                                /// </summary>
                                /// <returns></returns>
                                public Result Info() => GetRest();
                            }
                            public class PVENetwork_Get_Interfaces
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVENetwork_Get_Interfaces(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute network-get-interfaces.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/network-get-interfaces"); }

                                /// <summary>
                                /// Execute network-get-interfaces.
                                /// </summary>
                                /// <returns></returns>
                                public Result Network_Get_Interfaces() => GetRest();
                            }
                            public class PVEPing
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEPing(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute ping.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/ping"); }

                                /// <summary>
                                /// Execute ping.
                                /// </summary>
                                /// <returns></returns>
                                public Result Ping() => CreateRest();
                            }
                            public class PVEShutdown
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEShutdown(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute shutdown.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/shutdown"); }

                                /// <summary>
                                /// Execute shutdown.
                                /// </summary>
                                /// <returns></returns>
                                public Result Shutdown() => CreateRest();
                            }
                            public class PVESuspend_Disk
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVESuspend_Disk(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-disk.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-disk"); }

                                /// <summary>
                                /// Execute suspend-disk.
                                /// </summary>
                                /// <returns></returns>
                                public Result Suspend_Disk() => CreateRest();
                            }
                            public class PVESuspend_Hybrid
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVESuspend_Hybrid(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-hybrid.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-hybrid"); }

                                /// <summary>
                                /// Execute suspend-hybrid.
                                /// </summary>
                                /// <returns></returns>
                                public Result Suspend_Hybrid() => CreateRest();
                            }
                            public class PVESuspend_Ram
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVESuspend_Ram(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-ram.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-ram"); }

                                /// <summary>
                                /// Execute suspend-ram.
                                /// </summary>
                                /// <returns></returns>
                                public Result Suspend_Ram() => CreateRest();
                            }
                            public class PVESet_User_Password
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVESet_User_Password(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Sets the password for the given user to the given password
                                /// </summary>
                                /// <param name="password">The new password.</param>
                                /// <param name="username">The user to set the password for.</param>
                                /// <param name="crypted">set to 1 if the password has already been passed through crypt()</param>
                                /// <returns></returns>
                                public Result CreateRest(string password, string username, bool? crypted = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("password", password);
                                    parameters.Add("username", username);
                                    parameters.Add("crypted", crypted);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/set-user-password", parameters);
                                }

                                /// <summary>
                                /// Sets the password for the given user to the given password
                                /// </summary>
                                /// <param name="password">The new password.</param>
                                /// <param name="username">The user to set the password for.</param>
                                /// <param name="crypted">set to 1 if the password has already been passed through crypt()</param>
                                /// <returns></returns>
                                public Result Set_User_Password(string password, string username, bool? crypted = null) => CreateRest(password, username, crypted);
                            }
                            public class PVEExec
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEExec(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Executes the given command in the vm via the guest-agent and returns an object with the pid.
                                /// </summary>
                                /// <param name="command">The command as a list of program + arguments</param>
                                /// <returns></returns>
                                public Result CreateRest(string command)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("command", command);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/exec", parameters);
                                }

                                /// <summary>
                                /// Executes the given command in the vm via the guest-agent and returns an object with the pid.
                                /// </summary>
                                /// <param name="command">The command as a list of program + arguments</param>
                                /// <returns></returns>
                                public Result Exec(string command) => CreateRest(command);
                            }
                            public class PVEExec_Status
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEExec_Status(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Gets the status of the given pid started by the guest-agent
                                /// </summary>
                                /// <param name="pid">The PID to query</param>
                                /// <returns></returns>
                                public Result GetRest(int pid)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("pid", pid);
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/exec-status", parameters);
                                }

                                /// <summary>
                                /// Gets the status of the given pid started by the guest-agent
                                /// </summary>
                                /// <param name="pid">The PID to query</param>
                                /// <returns></returns>
                                public Result Exec_Status(int pid) => GetRest(pid);
                            }
                            public class PVEFile_Read
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEFile_Read(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Reads the given file via guest agent. Is limited to 16777216 bytes.
                                /// </summary>
                                /// <param name="file">The path to the file</param>
                                /// <returns></returns>
                                public Result GetRest(string file)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("file", file);
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/file-read", parameters);
                                }

                                /// <summary>
                                /// Reads the given file via guest agent. Is limited to 16777216 bytes.
                                /// </summary>
                                /// <param name="file">The path to the file</param>
                                /// <returns></returns>
                                public Result File_Read(string file) => GetRest(file);
                            }
                            public class PVEFile_Write
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEFile_Write(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Writes the given file via guest agent.
                                /// </summary>
                                /// <param name="content">The content to write into the file.</param>
                                /// <param name="file">The path to the file.</param>
                                /// <returns></returns>
                                public Result CreateRest(string content, string file)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("content", content);
                                    parameters.Add("file", file);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/file-write", parameters);
                                }

                                /// <summary>
                                /// Writes the given file via guest agent.
                                /// </summary>
                                /// <param name="content">The content to write into the file.</param>
                                /// <param name="file">The path to the file.</param>
                                /// <returns></returns>
                                public Result File_Write(string content, string file) => CreateRest(content, file);
                            }
                            /// <summary>
                            /// Qemu Agent command index.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent"); }

                            /// <summary>
                            /// Qemu Agent command index.
                            /// </summary>
                            /// <returns></returns>
                            public Result Index() => GetRest();
                            /// <summary>
                            /// Execute Qemu Guest Agent commands.
                            /// </summary>
                            /// <param name="command">The QGA command.
                            ///   Enum: fsfreeze-freeze,fsfreeze-status,fsfreeze-thaw,fstrim,get-fsinfo,get-host-name,get-memory-block-info,get-memory-blocks,get-osinfo,get-time,get-timezone,get-users,get-vcpus,info,network-get-interfaces,ping,shutdown,suspend-disk,suspend-hybrid,suspend-ram</param>
                            /// <returns></returns>
                            public Result CreateRest(string command)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("command", command);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent", parameters);
                            }

                            /// <summary>
                            /// Execute Qemu Guest Agent commands.
                            /// </summary>
                            /// <param name="command">The QGA command.
                            ///   Enum: fsfreeze-freeze,fsfreeze-status,fsfreeze-thaw,fstrim,get-fsinfo,get-host-name,get-memory-block-info,get-memory-blocks,get-osinfo,get-time,get-timezone,get-users,get-vcpus,info,network-get-interfaces,ping,shutdown,suspend-disk,suspend-hybrid,suspend-ram</param>
                            /// <returns></returns>
                            public Result Agent(string command) => CreateRest(command);
                        }
                        public class PVERrd
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVERrd(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Read VM RRD statistics (returns PNG)
                            /// </summary>
                            /// <param name="ds">The list of datasources you want to display.</param>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result GetRest(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/rrd", parameters);
                            }

                            /// <summary>
                            /// Read VM RRD statistics (returns PNG)
                            /// </summary>
                            /// <param name="ds">The list of datasources you want to display.</param>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result Rrd(string ds, string timeframe, string cf = null) => GetRest(ds, timeframe, cf);
                        }
                        public class PVERrddata
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVERrddata(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Read VM RRD statistics
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result GetRest(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/rrddata", parameters);
                            }

                            /// <summary>
                            /// Read VM RRD statistics
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result Rrddata(string timeframe, string cf = null) => GetRest(timeframe, cf);
                        }
                        public class PVEConfig
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEConfig(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get current virtual machine configuration. This does not include pending configuration changes (see 'pending' API).
                            /// </summary>
                            /// <param name="current">Get current values (instead of pending values).</param>
                            /// <param name="snapshot">Fetch config values from given snapshot.</param>
                            /// <returns></returns>
                            public Result GetRest(bool? current = null, string snapshot = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("current", current);
                                parameters.Add("snapshot", snapshot);
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/config", parameters);
                            }

                            /// <summary>
                            /// Get current virtual machine configuration. This does not include pending configuration changes (see 'pending' API).
                            /// </summary>
                            /// <param name="current">Get current values (instead of pending values).</param>
                            /// <param name="snapshot">Fetch config values from given snapshot.</param>
                            /// <returns></returns>
                            public Result VmConfig(bool? current = null, string snapshot = null) => GetRest(current, snapshot);
                            /// <summary>
                            /// Set virtual machine options (asynchrounous API).
                            /// </summary>
                            /// <param name="acpi">Enable/disable ACPI.</param>
                            /// <param name="agent">Enable/disable Qemu GuestAgent and its properties.</param>
                            /// <param name="arch">Virtual processor architecture. Defaults to the host.
                            ///   Enum: x86_64,aarch64</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="background_delay">Time to wait for the task to finish. We return 'null' if the task finish within that time.</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                            /// <param name="bootdisk">Enable booting from specified disk.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
                            /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                            /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                            /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                            ///   Enum: configdrive2,nocloud</param>
                            /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                            /// <param name="cores">The number of cores per socket.</param>
                            /// <param name="cpu">Emulated CPU type.</param>
                            /// <param name="cpulimit">Limit of CPU usage.</param>
                            /// <param name="cpuunits">CPU weight for a VM.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="efidisk0">Configure a Disk for storing EFI vars</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
                            ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                            /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                            /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                            /// <param name="machine">Specifies the Qemu machine type.</param>
                            /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                            /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                            /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                            /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                            /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="netN">Specify network devices.</param>
                            /// <param name="numa">Enable/disable NUMA.</param>
                            /// <param name="numaN">NUMA topology.</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">Specify guest operating system.
                            ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                            /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                            /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                            /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).</param>
                            /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).</param>
                            /// <param name="scsihw">SCSI controller model
                            ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                            /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                            /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                            /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                            /// <param name="sockets">The number of CPU sockets.</param>
                            /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                            /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="tablet">Enable/disable the USB tablet device.</param>
                            /// <param name="tdf">Enable/disable time drift fix.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <param name="usbN">Configure an USB device (n is 0 to 4).</param>
                            /// <param name="vcpus">Number of hotplugged vcpus.</param>
                            /// <param name="vga">Configure the VGA hardware.</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                            /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public Result CreateRest(bool? acpi = null, string agent = null, string arch = null, string args = null, bool? autostart = null, int? background_delay = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("acpi", acpi);
                                parameters.Add("agent", agent);
                                parameters.Add("arch", arch);
                                parameters.Add("args", args);
                                parameters.Add("autostart", autostart);
                                parameters.Add("background_delay", background_delay);
                                parameters.Add("balloon", balloon);
                                parameters.Add("bios", bios);
                                parameters.Add("boot", boot);
                                parameters.Add("bootdisk", bootdisk);
                                parameters.Add("cdrom", cdrom);
                                parameters.Add("cicustom", cicustom);
                                parameters.Add("cipassword", cipassword);
                                parameters.Add("citype", citype);
                                parameters.Add("ciuser", ciuser);
                                parameters.Add("cores", cores);
                                parameters.Add("cpu", cpu);
                                parameters.Add("cpulimit", cpulimit);
                                parameters.Add("cpuunits", cpuunits);
                                parameters.Add("delete", delete);
                                parameters.Add("description", description);
                                parameters.Add("digest", digest);
                                parameters.Add("efidisk0", efidisk0);
                                parameters.Add("force", force);
                                parameters.Add("freeze", freeze);
                                parameters.Add("hookscript", hookscript);
                                parameters.Add("hotplug", hotplug);
                                parameters.Add("hugepages", hugepages);
                                parameters.Add("ivshmem", ivshmem);
                                parameters.Add("keyboard", keyboard);
                                parameters.Add("kvm", kvm);
                                parameters.Add("localtime", localtime);
                                parameters.Add("lock", lock_);
                                parameters.Add("machine", machine);
                                parameters.Add("memory", memory);
                                parameters.Add("migrate_downtime", migrate_downtime);
                                parameters.Add("migrate_speed", migrate_speed);
                                parameters.Add("name", name);
                                parameters.Add("nameserver", nameserver);
                                parameters.Add("numa", numa);
                                parameters.Add("onboot", onboot);
                                parameters.Add("ostype", ostype);
                                parameters.Add("protection", protection);
                                parameters.Add("reboot", reboot);
                                parameters.Add("revert", revert);
                                parameters.Add("scsihw", scsihw);
                                parameters.Add("searchdomain", searchdomain);
                                parameters.Add("shares", shares);
                                parameters.Add("skiplock", skiplock);
                                parameters.Add("smbios1", smbios1);
                                parameters.Add("smp", smp);
                                parameters.Add("sockets", sockets);
                                parameters.Add("sshkeys", sshkeys);
                                parameters.Add("startdate", startdate);
                                parameters.Add("startup", startup);
                                parameters.Add("tablet", tablet);
                                parameters.Add("tdf", tdf);
                                parameters.Add("template", template);
                                parameters.Add("vcpus", vcpus);
                                parameters.Add("vga", vga);
                                parameters.Add("vmgenid", vmgenid);
                                parameters.Add("vmstatestorage", vmstatestorage);
                                parameters.Add("watchdog", watchdog);
                                AddIndexedParameter(parameters, "hostpci", hostpciN);
                                AddIndexedParameter(parameters, "ide", ideN);
                                AddIndexedParameter(parameters, "ipconfig", ipconfigN);
                                AddIndexedParameter(parameters, "net", netN);
                                AddIndexedParameter(parameters, "numa", numaN);
                                AddIndexedParameter(parameters, "parallel", parallelN);
                                AddIndexedParameter(parameters, "sata", sataN);
                                AddIndexedParameter(parameters, "scsi", scsiN);
                                AddIndexedParameter(parameters, "serial", serialN);
                                AddIndexedParameter(parameters, "unused", unusedN);
                                AddIndexedParameter(parameters, "usb", usbN);
                                AddIndexedParameter(parameters, "virtio", virtioN);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/config", parameters);
                            }

                            /// <summary>
                            /// Set virtual machine options (asynchrounous API).
                            /// </summary>
                            /// <param name="acpi">Enable/disable ACPI.</param>
                            /// <param name="agent">Enable/disable Qemu GuestAgent and its properties.</param>
                            /// <param name="arch">Virtual processor architecture. Defaults to the host.
                            ///   Enum: x86_64,aarch64</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="background_delay">Time to wait for the task to finish. We return 'null' if the task finish within that time.</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                            /// <param name="bootdisk">Enable booting from specified disk.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
                            /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                            /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                            /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                            ///   Enum: configdrive2,nocloud</param>
                            /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                            /// <param name="cores">The number of cores per socket.</param>
                            /// <param name="cpu">Emulated CPU type.</param>
                            /// <param name="cpulimit">Limit of CPU usage.</param>
                            /// <param name="cpuunits">CPU weight for a VM.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="efidisk0">Configure a Disk for storing EFI vars</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
                            ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                            /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                            /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                            /// <param name="machine">Specifies the Qemu machine type.</param>
                            /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                            /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                            /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                            /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                            /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="netN">Specify network devices.</param>
                            /// <param name="numa">Enable/disable NUMA.</param>
                            /// <param name="numaN">NUMA topology.</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">Specify guest operating system.
                            ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                            /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                            /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                            /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).</param>
                            /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).</param>
                            /// <param name="scsihw">SCSI controller model
                            ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                            /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                            /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                            /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                            /// <param name="sockets">The number of CPU sockets.</param>
                            /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                            /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="tablet">Enable/disable the USB tablet device.</param>
                            /// <param name="tdf">Enable/disable time drift fix.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <param name="usbN">Configure an USB device (n is 0 to 4).</param>
                            /// <param name="vcpus">Number of hotplugged vcpus.</param>
                            /// <param name="vga">Configure the VGA hardware.</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                            /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public Result UpdateVmAsync(bool? acpi = null, string agent = null, string arch = null, string args = null, bool? autostart = null, int? background_delay = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null) => CreateRest(acpi, agent, arch, args, autostart, background_delay, balloon, bios, boot, bootdisk, cdrom, cicustom, cipassword, citype, ciuser, cores, cpu, cpulimit, cpuunits, delete, description, digest, efidisk0, force, freeze, hookscript, hostpciN, hotplug, hugepages, ideN, ipconfigN, ivshmem, keyboard, kvm, localtime, lock_, machine, memory, migrate_downtime, migrate_speed, name, nameserver, netN, numa, numaN, onboot, ostype, parallelN, protection, reboot, revert, sataN, scsiN, scsihw, searchdomain, serialN, shares, skiplock, smbios1, smp, sockets, sshkeys, startdate, startup, tablet, tdf, template, unusedN, usbN, vcpus, vga, virtioN, vmgenid, vmstatestorage, watchdog);
                            /// <summary>
                            /// Set virtual machine options (synchrounous API) - You should consider using the POST method instead for any actions involving hotplug or storage allocation.
                            /// </summary>
                            /// <param name="acpi">Enable/disable ACPI.</param>
                            /// <param name="agent">Enable/disable Qemu GuestAgent and its properties.</param>
                            /// <param name="arch">Virtual processor architecture. Defaults to the host.
                            ///   Enum: x86_64,aarch64</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                            /// <param name="bootdisk">Enable booting from specified disk.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
                            /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                            /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                            /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                            ///   Enum: configdrive2,nocloud</param>
                            /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                            /// <param name="cores">The number of cores per socket.</param>
                            /// <param name="cpu">Emulated CPU type.</param>
                            /// <param name="cpulimit">Limit of CPU usage.</param>
                            /// <param name="cpuunits">CPU weight for a VM.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="efidisk0">Configure a Disk for storing EFI vars</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
                            ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                            /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                            /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                            /// <param name="machine">Specifies the Qemu machine type.</param>
                            /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                            /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                            /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                            /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                            /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="netN">Specify network devices.</param>
                            /// <param name="numa">Enable/disable NUMA.</param>
                            /// <param name="numaN">NUMA topology.</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">Specify guest operating system.
                            ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                            /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                            /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                            /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).</param>
                            /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).</param>
                            /// <param name="scsihw">SCSI controller model
                            ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                            /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                            /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                            /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                            /// <param name="sockets">The number of CPU sockets.</param>
                            /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                            /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="tablet">Enable/disable the USB tablet device.</param>
                            /// <param name="tdf">Enable/disable time drift fix.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <param name="usbN">Configure an USB device (n is 0 to 4).</param>
                            /// <param name="vcpus">Number of hotplugged vcpus.</param>
                            /// <param name="vga">Configure the VGA hardware.</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                            /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public Result SetRest(bool? acpi = null, string agent = null, string arch = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("acpi", acpi);
                                parameters.Add("agent", agent);
                                parameters.Add("arch", arch);
                                parameters.Add("args", args);
                                parameters.Add("autostart", autostart);
                                parameters.Add("balloon", balloon);
                                parameters.Add("bios", bios);
                                parameters.Add("boot", boot);
                                parameters.Add("bootdisk", bootdisk);
                                parameters.Add("cdrom", cdrom);
                                parameters.Add("cicustom", cicustom);
                                parameters.Add("cipassword", cipassword);
                                parameters.Add("citype", citype);
                                parameters.Add("ciuser", ciuser);
                                parameters.Add("cores", cores);
                                parameters.Add("cpu", cpu);
                                parameters.Add("cpulimit", cpulimit);
                                parameters.Add("cpuunits", cpuunits);
                                parameters.Add("delete", delete);
                                parameters.Add("description", description);
                                parameters.Add("digest", digest);
                                parameters.Add("efidisk0", efidisk0);
                                parameters.Add("force", force);
                                parameters.Add("freeze", freeze);
                                parameters.Add("hookscript", hookscript);
                                parameters.Add("hotplug", hotplug);
                                parameters.Add("hugepages", hugepages);
                                parameters.Add("ivshmem", ivshmem);
                                parameters.Add("keyboard", keyboard);
                                parameters.Add("kvm", kvm);
                                parameters.Add("localtime", localtime);
                                parameters.Add("lock", lock_);
                                parameters.Add("machine", machine);
                                parameters.Add("memory", memory);
                                parameters.Add("migrate_downtime", migrate_downtime);
                                parameters.Add("migrate_speed", migrate_speed);
                                parameters.Add("name", name);
                                parameters.Add("nameserver", nameserver);
                                parameters.Add("numa", numa);
                                parameters.Add("onboot", onboot);
                                parameters.Add("ostype", ostype);
                                parameters.Add("protection", protection);
                                parameters.Add("reboot", reboot);
                                parameters.Add("revert", revert);
                                parameters.Add("scsihw", scsihw);
                                parameters.Add("searchdomain", searchdomain);
                                parameters.Add("shares", shares);
                                parameters.Add("skiplock", skiplock);
                                parameters.Add("smbios1", smbios1);
                                parameters.Add("smp", smp);
                                parameters.Add("sockets", sockets);
                                parameters.Add("sshkeys", sshkeys);
                                parameters.Add("startdate", startdate);
                                parameters.Add("startup", startup);
                                parameters.Add("tablet", tablet);
                                parameters.Add("tdf", tdf);
                                parameters.Add("template", template);
                                parameters.Add("vcpus", vcpus);
                                parameters.Add("vga", vga);
                                parameters.Add("vmgenid", vmgenid);
                                parameters.Add("vmstatestorage", vmstatestorage);
                                parameters.Add("watchdog", watchdog);
                                AddIndexedParameter(parameters, "hostpci", hostpciN);
                                AddIndexedParameter(parameters, "ide", ideN);
                                AddIndexedParameter(parameters, "ipconfig", ipconfigN);
                                AddIndexedParameter(parameters, "net", netN);
                                AddIndexedParameter(parameters, "numa", numaN);
                                AddIndexedParameter(parameters, "parallel", parallelN);
                                AddIndexedParameter(parameters, "sata", sataN);
                                AddIndexedParameter(parameters, "scsi", scsiN);
                                AddIndexedParameter(parameters, "serial", serialN);
                                AddIndexedParameter(parameters, "unused", unusedN);
                                AddIndexedParameter(parameters, "usb", usbN);
                                AddIndexedParameter(parameters, "virtio", virtioN);
                                return _client.Set($"/nodes/{_node}/qemu/{_vmid}/config", parameters);
                            }

                            /// <summary>
                            /// Set virtual machine options (synchrounous API) - You should consider using the POST method instead for any actions involving hotplug or storage allocation.
                            /// </summary>
                            /// <param name="acpi">Enable/disable ACPI.</param>
                            /// <param name="agent">Enable/disable Qemu GuestAgent and its properties.</param>
                            /// <param name="arch">Virtual processor architecture. Defaults to the host.
                            ///   Enum: x86_64,aarch64</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                            /// <param name="bootdisk">Enable booting from specified disk.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
                            /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                            /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                            /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                            ///   Enum: configdrive2,nocloud</param>
                            /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                            /// <param name="cores">The number of cores per socket.</param>
                            /// <param name="cpu">Emulated CPU type.</param>
                            /// <param name="cpulimit">Limit of CPU usage.</param>
                            /// <param name="cpuunits">CPU weight for a VM.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="efidisk0">Configure a Disk for storing EFI vars</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
                            ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                            /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                            /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                            /// <param name="machine">Specifies the Qemu machine type.</param>
                            /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                            /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                            /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                            /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                            /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="netN">Specify network devices.</param>
                            /// <param name="numa">Enable/disable NUMA.</param>
                            /// <param name="numaN">NUMA topology.</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">Specify guest operating system.
                            ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                            /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                            /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                            /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).</param>
                            /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).</param>
                            /// <param name="scsihw">SCSI controller model
                            ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                            /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                            /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                            /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                            /// <param name="sockets">The number of CPU sockets.</param>
                            /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                            /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="tablet">Enable/disable the USB tablet device.</param>
                            /// <param name="tdf">Enable/disable time drift fix.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <param name="usbN">Configure an USB device (n is 0 to 4).</param>
                            /// <param name="vcpus">Number of hotplugged vcpus.</param>
                            /// <param name="vga">Configure the VGA hardware.</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                            /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public Result UpdateVm(bool? acpi = null, string agent = null, string arch = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null) => SetRest(acpi, agent, arch, args, autostart, balloon, bios, boot, bootdisk, cdrom, cicustom, cipassword, citype, ciuser, cores, cpu, cpulimit, cpuunits, delete, description, digest, efidisk0, force, freeze, hookscript, hostpciN, hotplug, hugepages, ideN, ipconfigN, ivshmem, keyboard, kvm, localtime, lock_, machine, memory, migrate_downtime, migrate_speed, name, nameserver, netN, numa, numaN, onboot, ostype, parallelN, protection, reboot, revert, sataN, scsiN, scsihw, searchdomain, serialN, shares, skiplock, smbios1, smp, sockets, sshkeys, startdate, startup, tablet, tdf, template, unusedN, usbN, vcpus, vga, virtioN, vmgenid, vmstatestorage, watchdog);
                        }
                        public class PVEPending
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEPending(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get virtual machine configuration, including pending changes.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/pending"); }

                            /// <summary>
                            /// Get virtual machine configuration, including pending changes.
                            /// </summary>
                            /// <returns></returns>
                            public Result VmPending() => GetRest();
                        }
                        public class PVEUnlink
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEUnlink(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Unlink/delete disk images.
                            /// </summary>
                            /// <param name="idlist">A list of disk IDs you want to delete.</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <returns></returns>
                            public Result SetRest(string idlist, bool? force = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("idlist", idlist);
                                parameters.Add("force", force);
                                return _client.Set($"/nodes/{_node}/qemu/{_vmid}/unlink", parameters);
                            }

                            /// <summary>
                            /// Unlink/delete disk images.
                            /// </summary>
                            /// <param name="idlist">A list of disk IDs you want to delete.</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <returns></returns>
                            public Result Unlink(string idlist, bool? force = null) => SetRest(idlist, force);
                        }
                        public class PVEVncproxy
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEVncproxy(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Creates a TCP VNC proxy connections.
                            /// </summary>
                            /// <param name="websocket">starts websockify instead of vncproxy</param>
                            /// <returns></returns>
                            public Result CreateRest(bool? websocket = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("websocket", websocket);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/vncproxy", parameters);
                            }

                            /// <summary>
                            /// Creates a TCP VNC proxy connections.
                            /// </summary>
                            /// <param name="websocket">starts websockify instead of vncproxy</param>
                            /// <returns></returns>
                            public Result Vncproxy(bool? websocket = null) => CreateRest(websocket);
                        }
                        public class PVETermproxy
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVETermproxy(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Creates a TCP proxy connections.
                            /// </summary>
                            /// <param name="serial">opens a serial terminal (defaults to display)
                            ///   Enum: serial0,serial1,serial2,serial3</param>
                            /// <returns></returns>
                            public Result CreateRest(string serial = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("serial", serial);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/termproxy", parameters);
                            }

                            /// <summary>
                            /// Creates a TCP proxy connections.
                            /// </summary>
                            /// <param name="serial">opens a serial terminal (defaults to display)
                            ///   Enum: serial0,serial1,serial2,serial3</param>
                            /// <returns></returns>
                            public Result Termproxy(string serial = null) => CreateRest(serial);
                        }
                        public class PVEVncwebsocket
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEVncwebsocket(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Opens a weksocket for VNC traffic.
                            /// </summary>
                            /// <param name="port">Port number returned by previous vncproxy call.</param>
                            /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                            /// <returns></returns>
                            public Result GetRest(int port, string vncticket)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("port", port);
                                parameters.Add("vncticket", vncticket);
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/vncwebsocket", parameters);
                            }

                            /// <summary>
                            /// Opens a weksocket for VNC traffic.
                            /// </summary>
                            /// <param name="port">Port number returned by previous vncproxy call.</param>
                            /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                            /// <returns></returns>
                            public Result Vncwebsocket(int port, string vncticket) => GetRest(port, vncticket);
                        }
                        public class PVESpiceproxy
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVESpiceproxy(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Returns a SPICE configuration to connect to the VM.
                            /// </summary>
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            /// <returns></returns>
                            public Result CreateRest(string proxy = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("proxy", proxy);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/spiceproxy", parameters);
                            }

                            /// <summary>
                            /// Returns a SPICE configuration to connect to the VM.
                            /// </summary>
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            /// <returns></returns>
                            public Result Spiceproxy(string proxy = null) => CreateRest(proxy);
                        }
                        public class PVEStatus
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEStatus(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PVECurrent _current;
                            public PVECurrent Current => _current ?? (_current = new PVECurrent(_client, _node, _vmid));
                            private PVEStart _start;
                            public PVEStart Start => _start ?? (_start = new PVEStart(_client, _node, _vmid));
                            private PVEStop _stop;
                            public PVEStop Stop => _stop ?? (_stop = new PVEStop(_client, _node, _vmid));
                            private PVEReset _reset;
                            public PVEReset Reset => _reset ?? (_reset = new PVEReset(_client, _node, _vmid));
                            private PVEShutdown _shutdown;
                            public PVEShutdown Shutdown => _shutdown ?? (_shutdown = new PVEShutdown(_client, _node, _vmid));
                            private PVESuspend _suspend;
                            public PVESuspend Suspend => _suspend ?? (_suspend = new PVESuspend(_client, _node, _vmid));
                            private PVEResume _resume;
                            public PVEResume Resume => _resume ?? (_resume = new PVEResume(_client, _node, _vmid));
                            public class PVECurrent
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVECurrent(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/status/current"); }

                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                /// <returns></returns>
                                public Result VmStatus() => GetRest();
                            }
                            public class PVEStart
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEStart(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Start virtual machine.
                                /// </summary>
                                /// <param name="machine">Specifies the Qemu machine type.</param>
                                /// <param name="migratedfrom">The cluster node name.</param>
                                /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                                /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                                ///   Enum: secure,insecure</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="stateuri">Some command save/restore state from this location.</param>
                                /// <param name="targetstorage">Target storage for the migration. (Can be '1' to use the same storage id as on the source node.)</param>
                                /// <returns></returns>
                                public Result CreateRest(string machine = null, string migratedfrom = null, string migration_network = null, string migration_type = null, bool? skiplock = null, string stateuri = null, string targetstorage = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("machine", machine);
                                    parameters.Add("migratedfrom", migratedfrom);
                                    parameters.Add("migration_network", migration_network);
                                    parameters.Add("migration_type", migration_type);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("stateuri", stateuri);
                                    parameters.Add("targetstorage", targetstorage);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/start", parameters);
                                }

                                /// <summary>
                                /// Start virtual machine.
                                /// </summary>
                                /// <param name="machine">Specifies the Qemu machine type.</param>
                                /// <param name="migratedfrom">The cluster node name.</param>
                                /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                                /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                                ///   Enum: secure,insecure</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="stateuri">Some command save/restore state from this location.</param>
                                /// <param name="targetstorage">Target storage for the migration. (Can be '1' to use the same storage id as on the source node.)</param>
                                /// <returns></returns>
                                public Result VmStart(string machine = null, string migratedfrom = null, string migration_network = null, string migration_type = null, bool? skiplock = null, string stateuri = null, string targetstorage = null) => CreateRest(machine, migratedfrom, migration_network, migration_type, skiplock, stateuri, targetstorage);
                            }
                            public class PVEStop
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEStop(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Stop virtual machine. The qemu process will exit immediately. Thisis akin to pulling the power plug of a running computer and may damage the VM data
                                /// </summary>
                                /// <param name="keepActive">Do not deactivate storage volumes.</param>
                                /// <param name="migratedfrom">The cluster node name.</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? keepActive = null, string migratedfrom = null, bool? skiplock = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("keepActive", keepActive);
                                    parameters.Add("migratedfrom", migratedfrom);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("timeout", timeout);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/stop", parameters);
                                }

                                /// <summary>
                                /// Stop virtual machine. The qemu process will exit immediately. Thisis akin to pulling the power plug of a running computer and may damage the VM data
                                /// </summary>
                                /// <param name="keepActive">Do not deactivate storage volumes.</param>
                                /// <param name="migratedfrom">The cluster node name.</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                /// <returns></returns>
                                public Result VmStop(bool? keepActive = null, string migratedfrom = null, bool? skiplock = null, int? timeout = null) => CreateRest(keepActive, migratedfrom, skiplock, timeout);
                            }
                            public class PVEReset
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEReset(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Reset virtual machine.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/reset", parameters);
                                }

                                /// <summary>
                                /// Reset virtual machine.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result VmReset(bool? skiplock = null) => CreateRest(skiplock);
                            }
                            public class PVEShutdown
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEShutdown(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Shutdown virtual machine. This is similar to pressing the power button on a physical machine.This will send an ACPI event for the guest OS, which should then proceed to a clean shutdown.
                                /// </summary>
                                /// <param name="forceStop">Make sure the VM stops.</param>
                                /// <param name="keepActive">Do not deactivate storage volumes.</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? forceStop = null, bool? keepActive = null, bool? skiplock = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("forceStop", forceStop);
                                    parameters.Add("keepActive", keepActive);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("timeout", timeout);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/shutdown", parameters);
                                }

                                /// <summary>
                                /// Shutdown virtual machine. This is similar to pressing the power button on a physical machine.This will send an ACPI event for the guest OS, which should then proceed to a clean shutdown.
                                /// </summary>
                                /// <param name="forceStop">Make sure the VM stops.</param>
                                /// <param name="keepActive">Do not deactivate storage volumes.</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                /// <returns></returns>
                                public Result VmShutdown(bool? forceStop = null, bool? keepActive = null, bool? skiplock = null, int? timeout = null) => CreateRest(forceStop, keepActive, skiplock, timeout);
                            }
                            public class PVESuspend
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVESuspend(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Suspend virtual machine.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="statestorage">The storage for the VM state</param>
                                /// <param name="todisk">If set, suspends the VM to disk. Will be resumed on next VM start.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? skiplock = null, string statestorage = null, bool? todisk = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("statestorage", statestorage);
                                    parameters.Add("todisk", todisk);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/suspend", parameters);
                                }

                                /// <summary>
                                /// Suspend virtual machine.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="statestorage">The storage for the VM state</param>
                                /// <param name="todisk">If set, suspends the VM to disk. Will be resumed on next VM start.</param>
                                /// <returns></returns>
                                public Result VmSuspend(bool? skiplock = null, string statestorage = null, bool? todisk = null) => CreateRest(skiplock, statestorage, todisk);
                            }
                            public class PVEResume
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEResume(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Resume virtual machine.
                                /// </summary>
                                /// <param name="nocheck"></param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? nocheck = null, bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("nocheck", nocheck);
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/resume", parameters);
                                }

                                /// <summary>
                                /// Resume virtual machine.
                                /// </summary>
                                /// <param name="nocheck"></param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result VmResume(bool? nocheck = null, bool? skiplock = null) => CreateRest(nocheck, skiplock);
                            }
                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/status"); }

                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public Result Vmcmdidx() => GetRest();
                        }
                        public class PVESendkey
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVESendkey(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Send key event to virtual machine.
                            /// </summary>
                            /// <param name="key">The key (qemu monitor encoding).</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <returns></returns>
                            public Result SetRest(string key, bool? skiplock = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("key", key);
                                parameters.Add("skiplock", skiplock);
                                return _client.Set($"/nodes/{_node}/qemu/{_vmid}/sendkey", parameters);
                            }

                            /// <summary>
                            /// Send key event to virtual machine.
                            /// </summary>
                            /// <param name="key">The key (qemu monitor encoding).</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <returns></returns>
                            public Result VmSendkey(string key, bool? skiplock = null) => SetRest(key, skiplock);
                        }
                        public class PVEFeature
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEFeature(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Check if feature for virtual machine is available.
                            /// </summary>
                            /// <param name="feature">Feature to check.
                            ///   Enum: snapshot,clone,copy</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <returns></returns>
                            public Result GetRest(string feature, string snapname = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("feature", feature);
                                parameters.Add("snapname", snapname);
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/feature", parameters);
                            }

                            /// <summary>
                            /// Check if feature for virtual machine is available.
                            /// </summary>
                            /// <param name="feature">Feature to check.
                            ///   Enum: snapshot,clone,copy</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <returns></returns>
                            public Result VmFeature(string feature, string snapname = null) => GetRest(feature, snapname);
                        }
                        public class PVEClone
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEClone(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Create a copy of virtual machine/template.
                            /// </summary>
                            /// <param name="newid">VMID for the clone.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="description">Description for the new VM.</param>
                            /// <param name="format">Target format for file storage. Only valid for full clone.
                            ///   Enum: raw,qcow2,vmdk</param>
                            /// <param name="full">Create a full copy of all disks. This is always done when you clone a normal VM. For VM templates, we try to create a linked clone by default.</param>
                            /// <param name="name">Set a name for the new VM.</param>
                            /// <param name="pool">Add the new VM to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            /// <param name="target">Target node. Only allowed if the original VM is on shared storage.</param>
                            /// <returns></returns>
                            public Result CreateRest(int newid, int? bwlimit = null, string description = null, string format = null, bool? full = null, string name = null, string pool = null, string snapname = null, string storage = null, string target = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("newid", newid);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("description", description);
                                parameters.Add("format", format);
                                parameters.Add("full", full);
                                parameters.Add("name", name);
                                parameters.Add("pool", pool);
                                parameters.Add("snapname", snapname);
                                parameters.Add("storage", storage);
                                parameters.Add("target", target);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/clone", parameters);
                            }

                            /// <summary>
                            /// Create a copy of virtual machine/template.
                            /// </summary>
                            /// <param name="newid">VMID for the clone.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="description">Description for the new VM.</param>
                            /// <param name="format">Target format for file storage. Only valid for full clone.
                            ///   Enum: raw,qcow2,vmdk</param>
                            /// <param name="full">Create a full copy of all disks. This is always done when you clone a normal VM. For VM templates, we try to create a linked clone by default.</param>
                            /// <param name="name">Set a name for the new VM.</param>
                            /// <param name="pool">Add the new VM to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            /// <param name="target">Target node. Only allowed if the original VM is on shared storage.</param>
                            /// <returns></returns>
                            public Result CloneVm(int newid, int? bwlimit = null, string description = null, string format = null, bool? full = null, string name = null, string pool = null, string snapname = null, string storage = null, string target = null) => CreateRest(newid, bwlimit, description, format, full, name, pool, snapname, storage, target);
                        }
                        public class PVEMoveDisk
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEMoveDisk(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Move volume to different storage.
                            /// </summary>
                            /// <param name="disk">The disk you want to move.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0</param>
                            /// <param name="storage">Target storage.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="delete">Delete the original disk after successful copy. By default the original disk is kept as unused disk.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="format">Target Format.
                            ///   Enum: raw,qcow2,vmdk</param>
                            /// <returns></returns>
                            public Result CreateRest(string disk, string storage, int? bwlimit = null, bool? delete = null, string digest = null, string format = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                parameters.Add("storage", storage);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                parameters.Add("format", format);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/move_disk", parameters);
                            }

                            /// <summary>
                            /// Move volume to different storage.
                            /// </summary>
                            /// <param name="disk">The disk you want to move.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0</param>
                            /// <param name="storage">Target storage.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="delete">Delete the original disk after successful copy. By default the original disk is kept as unused disk.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="format">Target Format.
                            ///   Enum: raw,qcow2,vmdk</param>
                            /// <returns></returns>
                            public Result MoveVmDisk(string disk, string storage, int? bwlimit = null, bool? delete = null, string digest = null, string format = null) => CreateRest(disk, storage, bwlimit, delete, digest, format);
                        }
                        public class PVEMigrate
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEMigrate(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get preconditions for migration.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <returns></returns>
                            public Result GetRest(string target = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target", target);
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/migrate", parameters);
                            }

                            /// <summary>
                            /// Get preconditions for migration.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <returns></returns>
                            public Result MigrateVmPrecondition(string target = null) => GetRest(target);
                            /// <summary>
                            /// Migrate virtual machine. Creates a new migration task.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="force">Allow to migrate VMs which use local devices. Only root may use this option.</param>
                            /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                            /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                            ///   Enum: secure,insecure</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="targetstorage">Default target storage.</param>
                            /// <param name="with_local_disks">Enable live storage migration for local disk</param>
                            /// <returns></returns>
                            public Result CreateRest(string target, int? bwlimit = null, bool? force = null, string migration_network = null, string migration_type = null, bool? online = null, string targetstorage = null, bool? with_local_disks = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target", target);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("force", force);
                                parameters.Add("migration_network", migration_network);
                                parameters.Add("migration_type", migration_type);
                                parameters.Add("online", online);
                                parameters.Add("targetstorage", targetstorage);
                                parameters.Add("with-local-disks", with_local_disks);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/migrate", parameters);
                            }

                            /// <summary>
                            /// Migrate virtual machine. Creates a new migration task.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="force">Allow to migrate VMs which use local devices. Only root may use this option.</param>
                            /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                            /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                            ///   Enum: secure,insecure</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="targetstorage">Default target storage.</param>
                            /// <param name="with_local_disks">Enable live storage migration for local disk</param>
                            /// <returns></returns>
                            public Result MigrateVm(string target, int? bwlimit = null, bool? force = null, string migration_network = null, string migration_type = null, bool? online = null, string targetstorage = null, bool? with_local_disks = null) => CreateRest(target, bwlimit, force, migration_network, migration_type, online, targetstorage, with_local_disks);
                        }
                        public class PVEMonitor
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEMonitor(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Execute Qemu monitor commands.
                            /// </summary>
                            /// <param name="command">The monitor command.</param>
                            /// <returns></returns>
                            public Result CreateRest(string command)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("command", command);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/monitor", parameters);
                            }

                            /// <summary>
                            /// Execute Qemu monitor commands.
                            /// </summary>
                            /// <param name="command">The monitor command.</param>
                            /// <returns></returns>
                            public Result Monitor(string command) => CreateRest(command);
                        }
                        public class PVEResize
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEResize(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Extend volume size.
                            /// </summary>
                            /// <param name="disk">The disk you want to resize.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0</param>
                            /// <param name="size">The new size. With the `+` sign the value is added to the actual size of the volume and without it, the value is taken as an absolute one. Shrinking disk size is not supported.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <returns></returns>
                            public Result SetRest(string disk, string size, string digest = null, bool? skiplock = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                parameters.Add("size", size);
                                parameters.Add("digest", digest);
                                parameters.Add("skiplock", skiplock);
                                return _client.Set($"/nodes/{_node}/qemu/{_vmid}/resize", parameters);
                            }

                            /// <summary>
                            /// Extend volume size.
                            /// </summary>
                            /// <param name="disk">The disk you want to resize.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0</param>
                            /// <param name="size">The new size. With the `+` sign the value is added to the actual size of the volume and without it, the value is taken as an absolute one. Shrinking disk size is not supported.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <returns></returns>
                            public Result ResizeVm(string disk, string size, string digest = null, bool? skiplock = null) => SetRest(disk, size, digest, skiplock);
                        }
                        public class PVESnapshot
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVESnapshot(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            public PVEItemSnapname this[object snapname] => new PVEItemSnapname(_client, _node, _vmid, snapname);
                            public class PVEItemSnapname
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                private readonly object _snapname;
                                internal PVEItemSnapname(Client client, object node, object vmid, object snapname)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                    _snapname = snapname;
                                }
                                private PVEConfig _config;
                                public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node, _vmid, _snapname));
                                private PVERollback _rollback;
                                public PVERollback Rollback => _rollback ?? (_rollback = new PVERollback(_client, _node, _vmid, _snapname));
                                public class PVEConfig
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _snapname;
                                    internal PVEConfig(Client client, object node, object vmid, object snapname)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }
                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/config"); }

                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetSnapshotConfig() => GetRest();
                                    /// <summary>
                                    /// Update snapshot metadata.
                                    /// </summary>
                                    /// <param name="description">A textual description or comment.</param>
                                    /// <returns></returns>
                                    public Result SetRest(string description = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("description", description);
                                        return _client.Set($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/config", parameters);
                                    }

                                    /// <summary>
                                    /// Update snapshot metadata.
                                    /// </summary>
                                    /// <param name="description">A textual description or comment.</param>
                                    /// <returns></returns>
                                    public Result UpdateSnapshotConfig(string description = null) => SetRest(description);
                                }
                                public class PVERollback
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _snapname;
                                    internal PVERollback(Client client, object node, object vmid, object snapname)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }
                                    /// <summary>
                                    /// Rollback VM state to specified snapshot.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result CreateRest() { return _client.Create($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/rollback"); }

                                    /// <summary>
                                    /// Rollback VM state to specified snapshot.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result Rollback() => CreateRest();
                                }
                                /// <summary>
                                /// Delete a VM snapshot.
                                /// </summary>
                                /// <param name="force">For removal from config file, even if removing disk snapshots fails.</param>
                                /// <returns></returns>
                                public Result DeleteRest(bool? force = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("force", force);
                                    return _client.Delete($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}", parameters);
                                }

                                /// <summary>
                                /// Delete a VM snapshot.
                                /// </summary>
                                /// <param name="force">For removal from config file, even if removing disk snapshots fails.</param>
                                /// <returns></returns>
                                public Result Delsnapshot(bool? force = null) => DeleteRest(force);
                                /// <summary>
                                /// 
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}"); }

                                /// <summary>
                                /// 
                                /// </summary>
                                /// <returns></returns>
                                public Result SnapshotCmdIdx() => GetRest();
                            }
                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot"); }

                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public Result SnapshotList() => GetRest();
                            /// <summary>
                            /// Snapshot a VM.
                            /// </summary>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="description">A textual description or comment.</param>
                            /// <param name="vmstate">Save the vmstate</param>
                            /// <returns></returns>
                            public Result CreateRest(string snapname, string description = null, bool? vmstate = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("snapname", snapname);
                                parameters.Add("description", description);
                                parameters.Add("vmstate", vmstate);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/snapshot", parameters);
                            }

                            /// <summary>
                            /// Snapshot a VM.
                            /// </summary>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="description">A textual description or comment.</param>
                            /// <param name="vmstate">Save the vmstate</param>
                            /// <returns></returns>
                            public Result Snapshot(string snapname, string description = null, bool? vmstate = null) => CreateRest(snapname, description, vmstate);
                        }
                        public class PVETemplate
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVETemplate(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <param name="disk">If you want to convert only 1 disk to base image.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0</param>
                            /// <returns></returns>
                            public Result CreateRest(string disk = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                return _client.Create($"/nodes/{_node}/qemu/{_vmid}/template", parameters);
                            }

                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <param name="disk">If you want to convert only 1 disk to base image.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0</param>
                            /// <returns></returns>
                            public Result Template(string disk = null) => CreateRest(disk);
                        }
                        public class PVECloudinit
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVECloudinit(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PVEDump _dump;
                            public PVEDump Dump => _dump ?? (_dump = new PVEDump(_client, _node, _vmid));
                            public class PVEDump
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEDump(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get automatically generated cloudinit config.
                                /// </summary>
                                /// <param name="type">Config type.
                                ///   Enum: user,network,meta</param>
                                /// <returns></returns>
                                public Result GetRest(string type)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/cloudinit/dump", parameters);
                                }

                                /// <summary>
                                /// Get automatically generated cloudinit config.
                                /// </summary>
                                /// <param name="type">Config type.
                                ///   Enum: user,network,meta</param>
                                /// <returns></returns>
                                public Result CloudinitGeneratedConfigDump(string type) => GetRest(type);
                            }
                        }
                        /// <summary>
                        /// Destroy the vm (also delete all used/owned volumes).
                        /// </summary>
                        /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                        /// <returns></returns>
                        public Result DeleteRest(bool? skiplock = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("skiplock", skiplock);
                            return _client.Delete($"/nodes/{_node}/qemu/{_vmid}", parameters);
                        }

                        /// <summary>
                        /// Destroy the vm (also delete all used/owned volumes).
                        /// </summary>
                        /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                        /// <returns></returns>
                        public Result DestroyVm(bool? skiplock = null) => DeleteRest(skiplock);
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/qemu/{_vmid}"); }

                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result Vmdiridx() => GetRest();
                    }
                    /// <summary>
                    /// Virtual machine index (per node).
                    /// </summary>
                    /// <param name="full">Determine the full status of active VMs.</param>
                    /// <returns></returns>
                    public Result GetRest(bool? full = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("full", full);
                        return _client.Get($"/nodes/{_node}/qemu", parameters);
                    }

                    /// <summary>
                    /// Virtual machine index (per node).
                    /// </summary>
                    /// <param name="full">Determine the full status of active VMs.</param>
                    /// <returns></returns>
                    public Result Vmlist(bool? full = null) => GetRest(full);
                    /// <summary>
                    /// Create or restore a virtual machine.
                    /// </summary>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="acpi">Enable/disable ACPI.</param>
                    /// <param name="agent">Enable/disable Qemu GuestAgent and its properties.</param>
                    /// <param name="arch">Virtual processor architecture. Defaults to the host.
                    ///   Enum: x86_64,aarch64</param>
                    /// <param name="archive">The backup file.</param>
                    /// <param name="args">Arbitrary arguments passed to kvm.</param>
                    /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                    /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                    /// <param name="bios">Select BIOS implementation.
                    ///   Enum: seabios,ovmf</param>
                    /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                    /// <param name="bootdisk">Enable booting from specified disk.</param>
                    /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                    /// <param name="cdrom">This is an alias for option -ide2</param>
                    /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                    /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                    /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                    ///   Enum: configdrive2,nocloud</param>
                    /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                    /// <param name="cores">The number of cores per socket.</param>
                    /// <param name="cpu">Emulated CPU type.</param>
                    /// <param name="cpulimit">Limit of CPU usage.</param>
                    /// <param name="cpuunits">CPU weight for a VM.</param>
                    /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                    /// <param name="efidisk0">Configure a Disk for storing EFI vars</param>
                    /// <param name="force">Allow to overwrite existing VM.</param>
                    /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                    /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                    /// <param name="hostpciN">Map host PCI devices into guest.</param>
                    /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                    /// <param name="hugepages">Enable/disable hugepages memory.
                    ///   Enum: any,2,1024</param>
                    /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                    /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                    /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                    /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
                    ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                    /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                    /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                    /// <param name="lock_">Lock/unlock the VM.
                    ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                    /// <param name="machine">Specifies the Qemu machine type.</param>
                    /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                    /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                    /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                    /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                    /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                    /// <param name="netN">Specify network devices.</param>
                    /// <param name="numa">Enable/disable NUMA.</param>
                    /// <param name="numaN">NUMA topology.</param>
                    /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                    /// <param name="ostype">Specify guest operating system.
                    ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                    /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                    /// <param name="pool">Add the VM to the specified pool.</param>
                    /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                    /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                    /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).</param>
                    /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).</param>
                    /// <param name="scsihw">SCSI controller model
                    ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                    /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                    /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                    /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                    /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                    /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                    /// <param name="sockets">The number of CPU sockets.</param>
                    /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                    /// <param name="start">Start VM after it was created successfully.</param>
                    /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                    /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                    /// <param name="storage">Default storage.</param>
                    /// <param name="tablet">Enable/disable the USB tablet device.</param>
                    /// <param name="tdf">Enable/disable time drift fix.</param>
                    /// <param name="template">Enable/disable Template.</param>
                    /// <param name="unique">Assign a unique random ethernet address.</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <param name="usbN">Configure an USB device (n is 0 to 4).</param>
                    /// <param name="vcpus">Number of hotplugged vcpus.</param>
                    /// <param name="vga">Configure the VGA hardware.</param>
                    /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                    /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                    /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                    /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                    /// <returns></returns>
                    public Result CreateRest(int vmid, bool? acpi = null, string agent = null, string arch = null, string archive = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, int? bwlimit = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string description = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, string pool = null, bool? protection = null, bool? reboot = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, bool? start = null, string startdate = null, string startup = null, string storage = null, bool? tablet = null, bool? tdf = null, bool? template = null, bool? unique = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("vmid", vmid);
                        parameters.Add("acpi", acpi);
                        parameters.Add("agent", agent);
                        parameters.Add("arch", arch);
                        parameters.Add("archive", archive);
                        parameters.Add("args", args);
                        parameters.Add("autostart", autostart);
                        parameters.Add("balloon", balloon);
                        parameters.Add("bios", bios);
                        parameters.Add("boot", boot);
                        parameters.Add("bootdisk", bootdisk);
                        parameters.Add("bwlimit", bwlimit);
                        parameters.Add("cdrom", cdrom);
                        parameters.Add("cicustom", cicustom);
                        parameters.Add("cipassword", cipassword);
                        parameters.Add("citype", citype);
                        parameters.Add("ciuser", ciuser);
                        parameters.Add("cores", cores);
                        parameters.Add("cpu", cpu);
                        parameters.Add("cpulimit", cpulimit);
                        parameters.Add("cpuunits", cpuunits);
                        parameters.Add("description", description);
                        parameters.Add("efidisk0", efidisk0);
                        parameters.Add("force", force);
                        parameters.Add("freeze", freeze);
                        parameters.Add("hookscript", hookscript);
                        parameters.Add("hotplug", hotplug);
                        parameters.Add("hugepages", hugepages);
                        parameters.Add("ivshmem", ivshmem);
                        parameters.Add("keyboard", keyboard);
                        parameters.Add("kvm", kvm);
                        parameters.Add("localtime", localtime);
                        parameters.Add("lock", lock_);
                        parameters.Add("machine", machine);
                        parameters.Add("memory", memory);
                        parameters.Add("migrate_downtime", migrate_downtime);
                        parameters.Add("migrate_speed", migrate_speed);
                        parameters.Add("name", name);
                        parameters.Add("nameserver", nameserver);
                        parameters.Add("numa", numa);
                        parameters.Add("onboot", onboot);
                        parameters.Add("ostype", ostype);
                        parameters.Add("pool", pool);
                        parameters.Add("protection", protection);
                        parameters.Add("reboot", reboot);
                        parameters.Add("scsihw", scsihw);
                        parameters.Add("searchdomain", searchdomain);
                        parameters.Add("shares", shares);
                        parameters.Add("smbios1", smbios1);
                        parameters.Add("smp", smp);
                        parameters.Add("sockets", sockets);
                        parameters.Add("sshkeys", sshkeys);
                        parameters.Add("start", start);
                        parameters.Add("startdate", startdate);
                        parameters.Add("startup", startup);
                        parameters.Add("storage", storage);
                        parameters.Add("tablet", tablet);
                        parameters.Add("tdf", tdf);
                        parameters.Add("template", template);
                        parameters.Add("unique", unique);
                        parameters.Add("vcpus", vcpus);
                        parameters.Add("vga", vga);
                        parameters.Add("vmgenid", vmgenid);
                        parameters.Add("vmstatestorage", vmstatestorage);
                        parameters.Add("watchdog", watchdog);
                        AddIndexedParameter(parameters, "hostpci", hostpciN);
                        AddIndexedParameter(parameters, "ide", ideN);
                        AddIndexedParameter(parameters, "ipconfig", ipconfigN);
                        AddIndexedParameter(parameters, "net", netN);
                        AddIndexedParameter(parameters, "numa", numaN);
                        AddIndexedParameter(parameters, "parallel", parallelN);
                        AddIndexedParameter(parameters, "sata", sataN);
                        AddIndexedParameter(parameters, "scsi", scsiN);
                        AddIndexedParameter(parameters, "serial", serialN);
                        AddIndexedParameter(parameters, "unused", unusedN);
                        AddIndexedParameter(parameters, "usb", usbN);
                        AddIndexedParameter(parameters, "virtio", virtioN);
                        return _client.Create($"/nodes/{_node}/qemu", parameters);
                    }

                    /// <summary>
                    /// Create or restore a virtual machine.
                    /// </summary>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="acpi">Enable/disable ACPI.</param>
                    /// <param name="agent">Enable/disable Qemu GuestAgent and its properties.</param>
                    /// <param name="arch">Virtual processor architecture. Defaults to the host.
                    ///   Enum: x86_64,aarch64</param>
                    /// <param name="archive">The backup file.</param>
                    /// <param name="args">Arbitrary arguments passed to kvm.</param>
                    /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                    /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                    /// <param name="bios">Select BIOS implementation.
                    ///   Enum: seabios,ovmf</param>
                    /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                    /// <param name="bootdisk">Enable booting from specified disk.</param>
                    /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                    /// <param name="cdrom">This is an alias for option -ide2</param>
                    /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                    /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                    /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                    ///   Enum: configdrive2,nocloud</param>
                    /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                    /// <param name="cores">The number of cores per socket.</param>
                    /// <param name="cpu">Emulated CPU type.</param>
                    /// <param name="cpulimit">Limit of CPU usage.</param>
                    /// <param name="cpuunits">CPU weight for a VM.</param>
                    /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                    /// <param name="efidisk0">Configure a Disk for storing EFI vars</param>
                    /// <param name="force">Allow to overwrite existing VM.</param>
                    /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                    /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                    /// <param name="hostpciN">Map host PCI devices into guest.</param>
                    /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                    /// <param name="hugepages">Enable/disable hugepages memory.
                    ///   Enum: any,2,1024</param>
                    /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                    /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                    /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                    /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
                    ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                    /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                    /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                    /// <param name="lock_">Lock/unlock the VM.
                    ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                    /// <param name="machine">Specifies the Qemu machine type.</param>
                    /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                    /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                    /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                    /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                    /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                    /// <param name="netN">Specify network devices.</param>
                    /// <param name="numa">Enable/disable NUMA.</param>
                    /// <param name="numaN">NUMA topology.</param>
                    /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                    /// <param name="ostype">Specify guest operating system.
                    ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                    /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                    /// <param name="pool">Add the VM to the specified pool.</param>
                    /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                    /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                    /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).</param>
                    /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).</param>
                    /// <param name="scsihw">SCSI controller model
                    ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                    /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                    /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                    /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                    /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                    /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                    /// <param name="sockets">The number of CPU sockets.</param>
                    /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                    /// <param name="start">Start VM after it was created successfully.</param>
                    /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                    /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                    /// <param name="storage">Default storage.</param>
                    /// <param name="tablet">Enable/disable the USB tablet device.</param>
                    /// <param name="tdf">Enable/disable time drift fix.</param>
                    /// <param name="template">Enable/disable Template.</param>
                    /// <param name="unique">Assign a unique random ethernet address.</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <param name="usbN">Configure an USB device (n is 0 to 4).</param>
                    /// <param name="vcpus">Number of hotplugged vcpus.</param>
                    /// <param name="vga">Configure the VGA hardware.</param>
                    /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                    /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                    /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                    /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                    /// <returns></returns>
                    public Result CreateVm(int vmid, bool? acpi = null, string agent = null, string arch = null, string archive = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, int? bwlimit = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string description = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, string pool = null, bool? protection = null, bool? reboot = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, bool? start = null, string startdate = null, string startup = null, string storage = null, bool? tablet = null, bool? tdf = null, bool? template = null, bool? unique = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null) => CreateRest(vmid, acpi, agent, arch, archive, args, autostart, balloon, bios, boot, bootdisk, bwlimit, cdrom, cicustom, cipassword, citype, ciuser, cores, cpu, cpulimit, cpuunits, description, efidisk0, force, freeze, hookscript, hostpciN, hotplug, hugepages, ideN, ipconfigN, ivshmem, keyboard, kvm, localtime, lock_, machine, memory, migrate_downtime, migrate_speed, name, nameserver, netN, numa, numaN, onboot, ostype, parallelN, pool, protection, reboot, sataN, scsiN, scsihw, searchdomain, serialN, shares, smbios1, smp, sockets, sshkeys, start, startdate, startup, storage, tablet, tdf, template, unique, unusedN, usbN, vcpus, vga, virtioN, vmgenid, vmstatestorage, watchdog);
                }
                public class PVELxc
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVELxc(Client client, object node) { _client = client; _node = node; }
                    public PVEItemVmid this[object vmid] => new PVEItemVmid(_client, _node, vmid);
                    public class PVEItemVmid
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        private readonly object _vmid;
                        internal PVEItemVmid(Client client, object node, object vmid)
                        {
                            _client = client; _node = node;
                            _vmid = vmid;
                        }
                        private PVEConfig _config;
                        public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node, _vmid));
                        private PVEStatus _status;
                        public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node, _vmid));
                        private PVESnapshot _snapshot;
                        public PVESnapshot Snapshot => _snapshot ?? (_snapshot = new PVESnapshot(_client, _node, _vmid));
                        private PVEFirewall _firewall;
                        public PVEFirewall Firewall => _firewall ?? (_firewall = new PVEFirewall(_client, _node, _vmid));
                        private PVERrd _rrd;
                        public PVERrd Rrd => _rrd ?? (_rrd = new PVERrd(_client, _node, _vmid));
                        private PVERrddata _rrddata;
                        public PVERrddata Rrddata => _rrddata ?? (_rrddata = new PVERrddata(_client, _node, _vmid));
                        private PVEVncproxy _vncproxy;
                        public PVEVncproxy Vncproxy => _vncproxy ?? (_vncproxy = new PVEVncproxy(_client, _node, _vmid));
                        private PVETermproxy _termproxy;
                        public PVETermproxy Termproxy => _termproxy ?? (_termproxy = new PVETermproxy(_client, _node, _vmid));
                        private PVEVncwebsocket _vncwebsocket;
                        public PVEVncwebsocket Vncwebsocket => _vncwebsocket ?? (_vncwebsocket = new PVEVncwebsocket(_client, _node, _vmid));
                        private PVESpiceproxy _spiceproxy;
                        public PVESpiceproxy Spiceproxy => _spiceproxy ?? (_spiceproxy = new PVESpiceproxy(_client, _node, _vmid));
                        private PVEMigrate _migrate;
                        public PVEMigrate Migrate => _migrate ?? (_migrate = new PVEMigrate(_client, _node, _vmid));
                        private PVEFeature _feature;
                        public PVEFeature Feature => _feature ?? (_feature = new PVEFeature(_client, _node, _vmid));
                        private PVETemplate _template;
                        public PVETemplate Template => _template ?? (_template = new PVETemplate(_client, _node, _vmid));
                        private PVEClone _clone;
                        public PVEClone Clone => _clone ?? (_clone = new PVEClone(_client, _node, _vmid));
                        private PVEResize _resize;
                        public PVEResize Resize => _resize ?? (_resize = new PVEResize(_client, _node, _vmid));
                        private PVEMoveVolume _moveVolume;
                        public PVEMoveVolume MoveVolume => _moveVolume ?? (_moveVolume = new PVEMoveVolume(_client, _node, _vmid));
                        public class PVEConfig
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEConfig(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get container configuration.
                            /// </summary>
                            /// <param name="snapshot">Fetch config values from given snapshot.</param>
                            /// <returns></returns>
                            public Result GetRest(string snapshot = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("snapshot", snapshot);
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/config", parameters);
                            }

                            /// <summary>
                            /// Get container configuration.
                            /// </summary>
                            /// <param name="snapshot">Fetch config values from given snapshot.</param>
                            /// <returns></returns>
                            public Result VmConfig(string snapshot = null) => GetRest(snapshot);
                            /// <summary>
                            /// Set container options.
                            /// </summary>
                            /// <param name="arch">OS architecture type.
                            ///   Enum: amd64,i386,arm64,armhf</param>
                            /// <param name="cmode">Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode to 'shell', it simply invokes a shell inside the container (no login).
                            ///   Enum: shell,console,tty</param>
                            /// <param name="console">Attach a console device (/dev/console) to the container.</param>
                            /// <param name="cores">The number of cores assigned to the container. A container can use all available cores by default.</param>
                            /// <param name="cpulimit">Limit of CPU usage.  NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.</param>
                            /// <param name="cpuunits">CPU weight for a VM. Argument is used in the kernel fair scheduler. The larger the number is, the more CPU time this VM gets. Number is relative to the weights of all the other running VMs.  NOTE: You can disable fair-scheduler configuration by setting this to 0.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Container description. Only used on the configuration web interface.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="features">Allow containers access to advanced features.</param>
                            /// <param name="hookscript">Script that will be exectued during various steps in the containers lifetime.</param>
                            /// <param name="hostname">Set a host name for the container.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: backup,create,disk,fstrim,migrate,mounted,rollback,snapshot,snapshot-delete</param>
                            /// <param name="memory">Amount of RAM for the VM in MB.</param>
                            /// <param name="mpN">Use volume as container mount point.</param>
                            /// <param name="nameserver">Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                            /// <param name="netN">Specifies network interfaces for the container.</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">OS type. This is used to setup configuration inside the container, and corresponds to lxc setup scripts in /usr/share/lxc/config/&amp;lt;ostype&amp;gt;.common.conf. Value 'unmanaged' can be used to skip and OS specific setup.
                            ///   Enum: debian,ubuntu,centos,fedora,opensuse,archlinux,alpine,gentoo,unmanaged</param>
                            /// <param name="protection">Sets the protection flag of the container. This will prevent the CT or CT's disk remove/update operation.</param>
                            /// <param name="rootfs">Use volume as container root.</param>
                            /// <param name="searchdomain">Sets DNS search domains for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="swap">Amount of SWAP for the VM in MB.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="tty">Specify the number of tty available to the container</param>
                            /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <returns></returns>
                            public Result SetRest(string arch = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string features = null, string hookscript = null, string hostname = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, bool? protection = null, string rootfs = null, string searchdomain = null, string startup = null, int? swap = null, bool? template = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("arch", arch);
                                parameters.Add("cmode", cmode);
                                parameters.Add("console", console);
                                parameters.Add("cores", cores);
                                parameters.Add("cpulimit", cpulimit);
                                parameters.Add("cpuunits", cpuunits);
                                parameters.Add("delete", delete);
                                parameters.Add("description", description);
                                parameters.Add("digest", digest);
                                parameters.Add("features", features);
                                parameters.Add("hookscript", hookscript);
                                parameters.Add("hostname", hostname);
                                parameters.Add("lock", lock_);
                                parameters.Add("memory", memory);
                                parameters.Add("nameserver", nameserver);
                                parameters.Add("onboot", onboot);
                                parameters.Add("ostype", ostype);
                                parameters.Add("protection", protection);
                                parameters.Add("rootfs", rootfs);
                                parameters.Add("searchdomain", searchdomain);
                                parameters.Add("startup", startup);
                                parameters.Add("swap", swap);
                                parameters.Add("template", template);
                                parameters.Add("tty", tty);
                                parameters.Add("unprivileged", unprivileged);
                                AddIndexedParameter(parameters, "mp", mpN);
                                AddIndexedParameter(parameters, "net", netN);
                                AddIndexedParameter(parameters, "unused", unusedN);
                                return _client.Set($"/nodes/{_node}/lxc/{_vmid}/config", parameters);
                            }

                            /// <summary>
                            /// Set container options.
                            /// </summary>
                            /// <param name="arch">OS architecture type.
                            ///   Enum: amd64,i386,arm64,armhf</param>
                            /// <param name="cmode">Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode to 'shell', it simply invokes a shell inside the container (no login).
                            ///   Enum: shell,console,tty</param>
                            /// <param name="console">Attach a console device (/dev/console) to the container.</param>
                            /// <param name="cores">The number of cores assigned to the container. A container can use all available cores by default.</param>
                            /// <param name="cpulimit">Limit of CPU usage.  NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.</param>
                            /// <param name="cpuunits">CPU weight for a VM. Argument is used in the kernel fair scheduler. The larger the number is, the more CPU time this VM gets. Number is relative to the weights of all the other running VMs.  NOTE: You can disable fair-scheduler configuration by setting this to 0.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Container description. Only used on the configuration web interface.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="features">Allow containers access to advanced features.</param>
                            /// <param name="hookscript">Script that will be exectued during various steps in the containers lifetime.</param>
                            /// <param name="hostname">Set a host name for the container.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: backup,create,disk,fstrim,migrate,mounted,rollback,snapshot,snapshot-delete</param>
                            /// <param name="memory">Amount of RAM for the VM in MB.</param>
                            /// <param name="mpN">Use volume as container mount point.</param>
                            /// <param name="nameserver">Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                            /// <param name="netN">Specifies network interfaces for the container.</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">OS type. This is used to setup configuration inside the container, and corresponds to lxc setup scripts in /usr/share/lxc/config/&amp;lt;ostype&amp;gt;.common.conf. Value 'unmanaged' can be used to skip and OS specific setup.
                            ///   Enum: debian,ubuntu,centos,fedora,opensuse,archlinux,alpine,gentoo,unmanaged</param>
                            /// <param name="protection">Sets the protection flag of the container. This will prevent the CT or CT's disk remove/update operation.</param>
                            /// <param name="rootfs">Use volume as container root.</param>
                            /// <param name="searchdomain">Sets DNS search domains for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="swap">Amount of SWAP for the VM in MB.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="tty">Specify the number of tty available to the container</param>
                            /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <returns></returns>
                            public Result UpdateVm(string arch = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string features = null, string hookscript = null, string hostname = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, bool? protection = null, string rootfs = null, string searchdomain = null, string startup = null, int? swap = null, bool? template = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null) => SetRest(arch, cmode, console, cores, cpulimit, cpuunits, delete, description, digest, features, hookscript, hostname, lock_, memory, mpN, nameserver, netN, onboot, ostype, protection, rootfs, searchdomain, startup, swap, template, tty, unprivileged, unusedN);
                        }
                        public class PVEStatus
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEStatus(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PVECurrent _current;
                            public PVECurrent Current => _current ?? (_current = new PVECurrent(_client, _node, _vmid));
                            private PVEStart _start;
                            public PVEStart Start => _start ?? (_start = new PVEStart(_client, _node, _vmid));
                            private PVEStop _stop;
                            public PVEStop Stop => _stop ?? (_stop = new PVEStop(_client, _node, _vmid));
                            private PVEShutdown _shutdown;
                            public PVEShutdown Shutdown => _shutdown ?? (_shutdown = new PVEShutdown(_client, _node, _vmid));
                            private PVESuspend _suspend;
                            public PVESuspend Suspend => _suspend ?? (_suspend = new PVESuspend(_client, _node, _vmid));
                            private PVEResume _resume;
                            public PVEResume Resume => _resume ?? (_resume = new PVEResume(_client, _node, _vmid));
                            public class PVECurrent
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVECurrent(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/status/current"); }

                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                /// <returns></returns>
                                public Result VmStatus() => GetRest();
                            }
                            public class PVEStart
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEStart(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Start the container.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/start", parameters);
                                }

                                /// <summary>
                                /// Start the container.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result VmStart(bool? skiplock = null) => CreateRest(skiplock);
                            }
                            public class PVEStop
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEStop(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Stop the container. This will abruptly stop all processes running in the container.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/stop", parameters);
                                }

                                /// <summary>
                                /// Stop the container. This will abruptly stop all processes running in the container.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result VmStop(bool? skiplock = null) => CreateRest(skiplock);
                            }
                            public class PVEShutdown
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEShutdown(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Shutdown the container. This will trigger a clean shutdown of the container, see lxc-stop(1) for details.
                                /// </summary>
                                /// <param name="forceStop">Make sure the Container stops.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? forceStop = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("forceStop", forceStop);
                                    parameters.Add("timeout", timeout);
                                    return _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/shutdown", parameters);
                                }

                                /// <summary>
                                /// Shutdown the container. This will trigger a clean shutdown of the container, see lxc-stop(1) for details.
                                /// </summary>
                                /// <param name="forceStop">Make sure the Container stops.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                /// <returns></returns>
                                public Result VmShutdown(bool? forceStop = null, int? timeout = null) => CreateRest(forceStop, timeout);
                            }
                            public class PVESuspend
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVESuspend(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Suspend the container.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/suspend"); }

                                /// <summary>
                                /// Suspend the container.
                                /// </summary>
                                /// <returns></returns>
                                public Result VmSuspend() => CreateRest();
                            }
                            public class PVEResume
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEResume(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Resume the container.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/resume"); }

                                /// <summary>
                                /// Resume the container.
                                /// </summary>
                                /// <returns></returns>
                                public Result VmResume() => CreateRest();
                            }
                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/status"); }

                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public Result Vmcmdidx() => GetRest();
                        }
                        public class PVESnapshot
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVESnapshot(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            public PVEItemSnapname this[object snapname] => new PVEItemSnapname(_client, _node, _vmid, snapname);
                            public class PVEItemSnapname
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                private readonly object _snapname;
                                internal PVEItemSnapname(Client client, object node, object vmid, object snapname)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                    _snapname = snapname;
                                }
                                private PVERollback _rollback;
                                public PVERollback Rollback => _rollback ?? (_rollback = new PVERollback(_client, _node, _vmid, _snapname));
                                private PVEConfig _config;
                                public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node, _vmid, _snapname));
                                public class PVERollback
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _snapname;
                                    internal PVERollback(Client client, object node, object vmid, object snapname)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }
                                    /// <summary>
                                    /// Rollback LXC state to specified snapshot.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result CreateRest() { return _client.Create($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/rollback"); }

                                    /// <summary>
                                    /// Rollback LXC state to specified snapshot.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result Rollback() => CreateRest();
                                }
                                public class PVEConfig
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _snapname;
                                    internal PVEConfig(Client client, object node, object vmid, object snapname)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }
                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/config"); }

                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetSnapshotConfig() => GetRest();
                                    /// <summary>
                                    /// Update snapshot metadata.
                                    /// </summary>
                                    /// <param name="description">A textual description or comment.</param>
                                    /// <returns></returns>
                                    public Result SetRest(string description = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("description", description);
                                        return _client.Set($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/config", parameters);
                                    }

                                    /// <summary>
                                    /// Update snapshot metadata.
                                    /// </summary>
                                    /// <param name="description">A textual description or comment.</param>
                                    /// <returns></returns>
                                    public Result UpdateSnapshotConfig(string description = null) => SetRest(description);
                                }
                                /// <summary>
                                /// Delete a LXC snapshot.
                                /// </summary>
                                /// <param name="force">For removal from config file, even if removing disk snapshots fails.</param>
                                /// <returns></returns>
                                public Result DeleteRest(bool? force = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("force", force);
                                    return _client.Delete($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}", parameters);
                                }

                                /// <summary>
                                /// Delete a LXC snapshot.
                                /// </summary>
                                /// <param name="force">For removal from config file, even if removing disk snapshots fails.</param>
                                /// <returns></returns>
                                public Result Delsnapshot(bool? force = null) => DeleteRest(force);
                                /// <summary>
                                /// 
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}"); }

                                /// <summary>
                                /// 
                                /// </summary>
                                /// <returns></returns>
                                public Result SnapshotCmdIdx() => GetRest();
                            }
                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot"); }

                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public Result List() => GetRest();
                            /// <summary>
                            /// Snapshot a container.
                            /// </summary>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="description">A textual description or comment.</param>
                            /// <returns></returns>
                            public Result CreateRest(string snapname, string description = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("snapname", snapname);
                                parameters.Add("description", description);
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/snapshot", parameters);
                            }

                            /// <summary>
                            /// Snapshot a container.
                            /// </summary>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="description">A textual description or comment.</param>
                            /// <returns></returns>
                            public Result Snapshot(string snapname, string description = null) => CreateRest(snapname, description);
                        }
                        public class PVEFirewall
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEFirewall(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PVERules _rules;
                            public PVERules Rules => _rules ?? (_rules = new PVERules(_client, _node, _vmid));
                            private PVEAliases _aliases;
                            public PVEAliases Aliases => _aliases ?? (_aliases = new PVEAliases(_client, _node, _vmid));
                            private PVEIpset _ipset;
                            public PVEIpset Ipset => _ipset ?? (_ipset = new PVEIpset(_client, _node, _vmid));
                            private PVEOptions _options;
                            public PVEOptions Options => _options ?? (_options = new PVEOptions(_client, _node, _vmid));
                            private PVELog _log;
                            public PVELog Log => _log ?? (_log = new PVELog(_client, _node, _vmid));
                            private PVERefs _refs;
                            public PVERefs Refs => _refs ?? (_refs = new PVERefs(_client, _node, _vmid));
                            public class PVERules
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVERules(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemPos this[object pos] => new PVEItemPos(_client, _node, _vmid, pos);
                                public class PVEItemPos
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _pos;
                                    internal PVEItemPos(Client client, object node, object vmid, object pos)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _pos = pos;
                                    }
                                    /// <summary>
                                    /// Delete rule.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public Result DeleteRest(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        return _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}", parameters);
                                    }

                                    /// <summary>
                                    /// Delete rule.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public Result DeleteRule(string digest = null) => DeleteRest(digest);
                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}"); }

                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRule() => GetRest();
                                    /// <summary>
                                    /// Modify rule data.
                                    /// </summary>
                                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                    /// <param name="comment">Descriptive comment.</param>
                                    /// <param name="delete">A list of settings you want to delete.</param>
                                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="enable">Flag to enable/disable a rule.</param>
                                    /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                    /// <param name="log">Log level for firewall rule.
                                    ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                    /// <param name="macro">Use predefined standard macro.</param>
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    /// <returns></returns>
                                    public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("action", action);
                                        parameters.Add("comment", comment);
                                        parameters.Add("delete", delete);
                                        parameters.Add("dest", dest);
                                        parameters.Add("digest", digest);
                                        parameters.Add("dport", dport);
                                        parameters.Add("enable", enable);
                                        parameters.Add("iface", iface);
                                        parameters.Add("log", log);
                                        parameters.Add("macro", macro);
                                        parameters.Add("moveto", moveto);
                                        parameters.Add("proto", proto);
                                        parameters.Add("source", source);
                                        parameters.Add("sport", sport);
                                        parameters.Add("type", type);
                                        return _client.Set($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}", parameters);
                                    }

                                    /// <summary>
                                    /// Modify rule data.
                                    /// </summary>
                                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                    /// <param name="comment">Descriptive comment.</param>
                                    /// <param name="delete">A list of settings you want to delete.</param>
                                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="enable">Flag to enable/disable a rule.</param>
                                    /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                    /// <param name="log">Log level for firewall rule.
                                    ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                    /// <param name="macro">Use predefined standard macro.</param>
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    /// <returns></returns>
                                    public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) => SetRest(action, comment, delete, dest, digest, dport, enable, iface, log, macro, moveto, proto, source, sport, type);
                                }
                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/rules"); }

                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRules() => GetRest();
                                /// <summary>
                                /// Create new rule.
                                /// </summary>
                                /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                /// <param name="type">Rule type.
                                ///   Enum: in,out,group</param>
                                /// <param name="comment">Descriptive comment.</param>
                                /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <param name="enable">Flag to enable/disable a rule.</param>
                                /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                /// <param name="log">Log level for firewall rule.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("action", action);
                                    parameters.Add("type", type);
                                    parameters.Add("comment", comment);
                                    parameters.Add("dest", dest);
                                    parameters.Add("digest", digest);
                                    parameters.Add("dport", dport);
                                    parameters.Add("enable", enable);
                                    parameters.Add("iface", iface);
                                    parameters.Add("log", log);
                                    parameters.Add("macro", macro);
                                    parameters.Add("pos", pos);
                                    parameters.Add("proto", proto);
                                    parameters.Add("source", source);
                                    parameters.Add("sport", sport);
                                    return _client.Create($"/nodes/{_node}/lxc/{_vmid}/firewall/rules", parameters);
                                }

                                /// <summary>
                                /// Create new rule.
                                /// </summary>
                                /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                /// <param name="type">Rule type.
                                ///   Enum: in,out,group</param>
                                /// <param name="comment">Descriptive comment.</param>
                                /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <param name="enable">Flag to enable/disable a rule.</param>
                                /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                /// <param name="log">Log level for firewall rule.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) => CreateRest(action, type, comment, dest, digest, dport, enable, iface, log, macro, pos, proto, source, sport);
                            }
                            public class PVEAliases
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEAliases(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemName this[object name] => new PVEItemName(_client, _node, _vmid, name);
                                public class PVEItemName
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _name;
                                    internal PVEItemName(Client client, object node, object vmid, object name)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }
                                    /// <summary>
                                    /// Remove IP or Network alias.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public Result DeleteRest(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        return _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}", parameters);
                                    }

                                    /// <summary>
                                    /// Remove IP or Network alias.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public Result RemoveAlias(string digest = null) => DeleteRest(digest);
                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}"); }

                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result ReadAlias() => GetRest();
                                    /// <summary>
                                    /// Update IP or Network alias.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="rename">Rename an existing alias.</param>
                                    /// <returns></returns>
                                    public Result SetRest(string cidr, string comment = null, string digest = null, string rename = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("digest", digest);
                                        parameters.Add("rename", rename);
                                        return _client.Set($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}", parameters);
                                    }

                                    /// <summary>
                                    /// Update IP or Network alias.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="rename">Rename an existing alias.</param>
                                    /// <returns></returns>
                                    public Result UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null) => SetRest(cidr, comment, digest, rename);
                                }
                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases"); }

                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public Result GetAliases() => GetRest();
                                /// <summary>
                                /// Create IP or Network Alias.
                                /// </summary>
                                /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                /// <param name="name">Alias name.</param>
                                /// <param name="comment"></param>
                                /// <returns></returns>
                                public Result CreateRest(string cidr, string name, string comment = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("cidr", cidr);
                                    parameters.Add("name", name);
                                    parameters.Add("comment", comment);
                                    return _client.Create($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases", parameters);
                                }

                                /// <summary>
                                /// Create IP or Network Alias.
                                /// </summary>
                                /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                /// <param name="name">Alias name.</param>
                                /// <param name="comment"></param>
                                /// <returns></returns>
                                public Result CreateAlias(string cidr, string name, string comment = null) => CreateRest(cidr, name, comment);
                            }
                            public class PVEIpset
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEIpset(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemName this[object name] => new PVEItemName(_client, _node, _vmid, name);
                                public class PVEItemName
                                {
                                    private readonly Client _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _name;
                                    internal PVEItemName(Client client, object node, object vmid, object name)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }
                                    public PVEItemCidr this[object cidr] => new PVEItemCidr(_client, _node, _vmid, _name, cidr);
                                    public class PVEItemCidr
                                    {
                                        private readonly Client _client;
                                        private readonly object _node;
                                        private readonly object _vmid;
                                        private readonly object _name;
                                        private readonly object _cidr;
                                        internal PVEItemCidr(Client client, object node, object vmid, object name, object cidr)
                                        {
                                            _client = client; _node = node;
                                            _vmid = vmid;
                                            _name = name;
                                            _cidr = cidr;
                                        }
                                        /// <summary>
                                        /// Remove IP or Network from IPSet.
                                        /// </summary>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <returns></returns>
                                        public Result DeleteRest(string digest = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("digest", digest);
                                            return _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}", parameters);
                                        }

                                        /// <summary>
                                        /// Remove IP or Network from IPSet.
                                        /// </summary>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <returns></returns>
                                        public Result RemoveIp(string digest = null) => DeleteRest(digest);
                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}"); }

                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public Result ReadIp() => GetRest();
                                        /// <summary>
                                        /// Update IP or Network settings
                                        /// </summary>
                                        /// <param name="comment"></param>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <param name="nomatch"></param>
                                        /// <returns></returns>
                                        public Result SetRest(string comment = null, string digest = null, bool? nomatch = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("comment", comment);
                                            parameters.Add("digest", digest);
                                            parameters.Add("nomatch", nomatch);
                                            return _client.Set($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}", parameters);
                                        }

                                        /// <summary>
                                        /// Update IP or Network settings
                                        /// </summary>
                                        /// <param name="comment"></param>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <param name="nomatch"></param>
                                        /// <returns></returns>
                                        public Result UpdateIp(string comment = null, string digest = null, bool? nomatch = null) => SetRest(comment, digest, nomatch);
                                    }
                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}"); }

                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result DeleteIpset() => DeleteRest();
                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}"); }

                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetIpset() => GetRest();
                                    /// <summary>
                                    /// Add IP or Network to IPSet.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="nomatch"></param>
                                    /// <returns></returns>
                                    public Result CreateRest(string cidr, string comment = null, bool? nomatch = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("nomatch", nomatch);
                                        return _client.Create($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}", parameters);
                                    }

                                    /// <summary>
                                    /// Add IP or Network to IPSet.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="nomatch"></param>
                                    /// <returns></returns>
                                    public Result CreateIp(string cidr, string comment = null, bool? nomatch = null) => CreateRest(cidr, comment, nomatch);
                                }
                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset"); }

                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public Result IpsetIndex() => GetRest();
                                /// <summary>
                                /// Create new IPSet
                                /// </summary>
                                /// <param name="name">IP set name.</param>
                                /// <param name="comment"></param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                                /// <returns></returns>
                                public Result CreateRest(string name, string comment = null, string digest = null, string rename = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("name", name);
                                    parameters.Add("comment", comment);
                                    parameters.Add("digest", digest);
                                    parameters.Add("rename", rename);
                                    return _client.Create($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset", parameters);
                                }

                                /// <summary>
                                /// Create new IPSet
                                /// </summary>
                                /// <param name="name">IP set name.</param>
                                /// <param name="comment"></param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                                /// <returns></returns>
                                public Result CreateIpset(string name, string comment = null, string digest = null, string rename = null) => CreateRest(name, comment, digest, rename);
                            }
                            public class PVEOptions
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVEOptions(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/options"); }

                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetOptions() => GetRest();
                                /// <summary>
                                /// Set Firewall options.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp">Enable DHCP.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="enable">Enable/disable firewall rules.</param>
                                /// <param name="ipfilter">Enable default IP filters. This is equivalent to adding an empty ipfilter-net&amp;lt;id&amp;gt; ipset for every interface. Such ipsets implicitly contain sane default restrictions such as restricting IPv6 link local addresses to the one derived from the interface's MAC address. For containers the configured IP addresses will be implicitly added.</param>
                                /// <param name="log_level_in">Log level for incoming traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="log_level_out">Log level for outgoing traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macfilter">Enable/disable MAC address filter.</param>
                                /// <param name="ndp">Enable NDP.</param>
                                /// <param name="policy_in">Input policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="policy_out">Output policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="radv">Allow sending Router Advertisement.</param>
                                /// <returns></returns>
                                public Result SetRest(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("delete", delete);
                                    parameters.Add("dhcp", dhcp);
                                    parameters.Add("digest", digest);
                                    parameters.Add("enable", enable);
                                    parameters.Add("ipfilter", ipfilter);
                                    parameters.Add("log_level_in", log_level_in);
                                    parameters.Add("log_level_out", log_level_out);
                                    parameters.Add("macfilter", macfilter);
                                    parameters.Add("ndp", ndp);
                                    parameters.Add("policy_in", policy_in);
                                    parameters.Add("policy_out", policy_out);
                                    parameters.Add("radv", radv);
                                    return _client.Set($"/nodes/{_node}/lxc/{_vmid}/firewall/options", parameters);
                                }

                                /// <summary>
                                /// Set Firewall options.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp">Enable DHCP.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="enable">Enable/disable firewall rules.</param>
                                /// <param name="ipfilter">Enable default IP filters. This is equivalent to adding an empty ipfilter-net&amp;lt;id&amp;gt; ipset for every interface. Such ipsets implicitly contain sane default restrictions such as restricting IPv6 link local addresses to the one derived from the interface's MAC address. For containers the configured IP addresses will be implicitly added.</param>
                                /// <param name="log_level_in">Log level for incoming traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="log_level_out">Log level for outgoing traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macfilter">Enable/disable MAC address filter.</param>
                                /// <param name="ndp">Enable NDP.</param>
                                /// <param name="policy_in">Input policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="policy_out">Output policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="radv">Allow sending Router Advertisement.</param>
                                /// <returns></returns>
                                public Result SetOptions(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null) => SetRest(delete, dhcp, digest, enable, ipfilter, log_level_in, log_level_out, macfilter, ndp, policy_in, policy_out, radv);
                            }
                            public class PVELog
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVELog(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Read firewall log
                                /// </summary>
                                /// <param name="limit"></param>
                                /// <param name="start"></param>
                                /// <returns></returns>
                                public Result GetRest(int? limit = null, int? start = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("limit", limit);
                                    parameters.Add("start", start);
                                    return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/log", parameters);
                                }

                                /// <summary>
                                /// Read firewall log
                                /// </summary>
                                /// <param name="limit"></param>
                                /// <param name="start"></param>
                                /// <returns></returns>
                                public Result Log(int? limit = null, int? start = null) => GetRest(limit, start);
                            }
                            public class PVERefs
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PVERefs(Client client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                                /// </summary>
                                /// <param name="type">Only list references of specified type.
                                ///   Enum: alias,ipset</param>
                                /// <returns></returns>
                                public Result GetRest(string type = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/refs", parameters);
                                }

                                /// <summary>
                                /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                                /// </summary>
                                /// <param name="type">Only list references of specified type.
                                ///   Enum: alias,ipset</param>
                                /// <returns></returns>
                                public Result Refs(string type = null) => GetRest(type);
                            }
                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall"); }

                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public Result Index() => GetRest();
                        }
                        public class PVERrd
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVERrd(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Read VM RRD statistics (returns PNG)
                            /// </summary>
                            /// <param name="ds">The list of datasources you want to display.</param>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result GetRest(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/rrd", parameters);
                            }

                            /// <summary>
                            /// Read VM RRD statistics (returns PNG)
                            /// </summary>
                            /// <param name="ds">The list of datasources you want to display.</param>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result Rrd(string ds, string timeframe, string cf = null) => GetRest(ds, timeframe, cf);
                        }
                        public class PVERrddata
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVERrddata(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Read VM RRD statistics
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result GetRest(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/rrddata", parameters);
                            }

                            /// <summary>
                            /// Read VM RRD statistics
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result Rrddata(string timeframe, string cf = null) => GetRest(timeframe, cf);
                        }
                        public class PVEVncproxy
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEVncproxy(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Creates a TCP VNC proxy connections.
                            /// </summary>
                            /// <param name="height">sets the height of the console in pixels.</param>
                            /// <param name="websocket">use websocket instead of standard VNC.</param>
                            /// <param name="width">sets the width of the console in pixels.</param>
                            /// <returns></returns>
                            public Result CreateRest(int? height = null, bool? websocket = null, int? width = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("height", height);
                                parameters.Add("websocket", websocket);
                                parameters.Add("width", width);
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/vncproxy", parameters);
                            }

                            /// <summary>
                            /// Creates a TCP VNC proxy connections.
                            /// </summary>
                            /// <param name="height">sets the height of the console in pixels.</param>
                            /// <param name="websocket">use websocket instead of standard VNC.</param>
                            /// <param name="width">sets the width of the console in pixels.</param>
                            /// <returns></returns>
                            public Result Vncproxy(int? height = null, bool? websocket = null, int? width = null) => CreateRest(height, websocket, width);
                        }
                        public class PVETermproxy
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVETermproxy(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Creates a TCP proxy connection.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/lxc/{_vmid}/termproxy"); }

                            /// <summary>
                            /// Creates a TCP proxy connection.
                            /// </summary>
                            /// <returns></returns>
                            public Result Termproxy() => CreateRest();
                        }
                        public class PVEVncwebsocket
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEVncwebsocket(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Opens a weksocket for VNC traffic.
                            /// </summary>
                            /// <param name="port">Port number returned by previous vncproxy call.</param>
                            /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                            /// <returns></returns>
                            public Result GetRest(int port, string vncticket)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("port", port);
                                parameters.Add("vncticket", vncticket);
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/vncwebsocket", parameters);
                            }

                            /// <summary>
                            /// Opens a weksocket for VNC traffic.
                            /// </summary>
                            /// <param name="port">Port number returned by previous vncproxy call.</param>
                            /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                            /// <returns></returns>
                            public Result Vncwebsocket(int port, string vncticket) => GetRest(port, vncticket);
                        }
                        public class PVESpiceproxy
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVESpiceproxy(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Returns a SPICE configuration to connect to the CT.
                            /// </summary>
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            /// <returns></returns>
                            public Result CreateRest(string proxy = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("proxy", proxy);
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/spiceproxy", parameters);
                            }

                            /// <summary>
                            /// Returns a SPICE configuration to connect to the CT.
                            /// </summary>
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            /// <returns></returns>
                            public Result Spiceproxy(string proxy = null) => CreateRest(proxy);
                        }
                        public class PVEMigrate
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEMigrate(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Migrate the container to another node. Creates a new migration task.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="force">Force migration despite local bind / device mounts. NOTE: deprecated, use 'shared' property of mount point instead.</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="restart">Use restart migration</param>
                            /// <param name="timeout">Timeout in seconds for shutdown for restart migration</param>
                            /// <returns></returns>
                            public Result CreateRest(string target, int? bwlimit = null, bool? force = null, bool? online = null, bool? restart = null, int? timeout = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target", target);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("force", force);
                                parameters.Add("online", online);
                                parameters.Add("restart", restart);
                                parameters.Add("timeout", timeout);
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/migrate", parameters);
                            }

                            /// <summary>
                            /// Migrate the container to another node. Creates a new migration task.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="force">Force migration despite local bind / device mounts. NOTE: deprecated, use 'shared' property of mount point instead.</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="restart">Use restart migration</param>
                            /// <param name="timeout">Timeout in seconds for shutdown for restart migration</param>
                            /// <returns></returns>
                            public Result MigrateVm(string target, int? bwlimit = null, bool? force = null, bool? online = null, bool? restart = null, int? timeout = null) => CreateRest(target, bwlimit, force, online, restart, timeout);
                        }
                        public class PVEFeature
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEFeature(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Check if feature for virtual machine is available.
                            /// </summary>
                            /// <param name="feature">Feature to check.
                            ///   Enum: snapshot,clone,copy</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <returns></returns>
                            public Result GetRest(string feature, string snapname = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("feature", feature);
                                parameters.Add("snapname", snapname);
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/feature", parameters);
                            }

                            /// <summary>
                            /// Check if feature for virtual machine is available.
                            /// </summary>
                            /// <param name="feature">Feature to check.
                            ///   Enum: snapshot,clone,copy</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <returns></returns>
                            public Result VmFeature(string feature, string snapname = null) => GetRest(feature, snapname);
                        }
                        public class PVETemplate
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVETemplate(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/lxc/{_vmid}/template"); }

                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <returns></returns>
                            public Result Template() => CreateRest();
                        }
                        public class PVEClone
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEClone(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Create a container clone/copy
                            /// </summary>
                            /// <param name="newid">VMID for the clone.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="description">Description for the new CT.</param>
                            /// <param name="full">Create a full copy of all disks. This is always done when you clone a normal CT. For CT templates, we try to create a linked clone by default.</param>
                            /// <param name="hostname">Set a hostname for the new CT.</param>
                            /// <param name="pool">Add the new CT to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            /// <param name="target">Target node. Only allowed if the original VM is on shared storage.</param>
                            /// <returns></returns>
                            public Result CreateRest(int newid, int? bwlimit = null, string description = null, bool? full = null, string hostname = null, string pool = null, string snapname = null, string storage = null, string target = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("newid", newid);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("description", description);
                                parameters.Add("full", full);
                                parameters.Add("hostname", hostname);
                                parameters.Add("pool", pool);
                                parameters.Add("snapname", snapname);
                                parameters.Add("storage", storage);
                                parameters.Add("target", target);
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/clone", parameters);
                            }

                            /// <summary>
                            /// Create a container clone/copy
                            /// </summary>
                            /// <param name="newid">VMID for the clone.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="description">Description for the new CT.</param>
                            /// <param name="full">Create a full copy of all disks. This is always done when you clone a normal CT. For CT templates, we try to create a linked clone by default.</param>
                            /// <param name="hostname">Set a hostname for the new CT.</param>
                            /// <param name="pool">Add the new CT to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            /// <param name="target">Target node. Only allowed if the original VM is on shared storage.</param>
                            /// <returns></returns>
                            public Result CloneVm(int newid, int? bwlimit = null, string description = null, bool? full = null, string hostname = null, string pool = null, string snapname = null, string storage = null, string target = null) => CreateRest(newid, bwlimit, description, full, hostname, pool, snapname, storage, target);
                        }
                        public class PVEResize
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEResize(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Resize a container mount point.
                            /// </summary>
                            /// <param name="disk">The disk you want to resize.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9,mp10,mp11,mp12,mp13,mp14,mp15,mp16,mp17,mp18,mp19,mp20,mp21,mp22,mp23,mp24,mp25,mp26,mp27,mp28,mp29,mp30,mp31,mp32,mp33,mp34,mp35,mp36,mp37,mp38,mp39,mp40,mp41,mp42,mp43,mp44,mp45,mp46,mp47,mp48,mp49,mp50,mp51,mp52,mp53,mp54,mp55,mp56,mp57,mp58,mp59,mp60,mp61,mp62,mp63,mp64,mp65,mp66,mp67,mp68,mp69,mp70,mp71,mp72,mp73,mp74,mp75,mp76,mp77,mp78,mp79,mp80,mp81,mp82,mp83,mp84,mp85,mp86,mp87,mp88,mp89,mp90,mp91,mp92,mp93,mp94,mp95,mp96,mp97,mp98,mp99,mp100,mp101,mp102,mp103,mp104,mp105,mp106,mp107,mp108,mp109,mp110,mp111,mp112,mp113,mp114,mp115,mp116,mp117,mp118,mp119,mp120,mp121,mp122,mp123,mp124,mp125,mp126,mp127,mp128,mp129,mp130,mp131,mp132,mp133,mp134,mp135,mp136,mp137,mp138,mp139,mp140,mp141,mp142,mp143,mp144,mp145,mp146,mp147,mp148,mp149,mp150,mp151,mp152,mp153,mp154,mp155,mp156,mp157,mp158,mp159,mp160,mp161,mp162,mp163,mp164,mp165,mp166,mp167,mp168,mp169,mp170,mp171,mp172,mp173,mp174,mp175,mp176,mp177,mp178,mp179,mp180,mp181,mp182,mp183,mp184,mp185,mp186,mp187,mp188,mp189,mp190,mp191,mp192,mp193,mp194,mp195,mp196,mp197,mp198,mp199,mp200,mp201,mp202,mp203,mp204,mp205,mp206,mp207,mp208,mp209,mp210,mp211,mp212,mp213,mp214,mp215,mp216,mp217,mp218,mp219,mp220,mp221,mp222,mp223,mp224,mp225,mp226,mp227,mp228,mp229,mp230,mp231,mp232,mp233,mp234,mp235,mp236,mp237,mp238,mp239,mp240,mp241,mp242,mp243,mp244,mp245,mp246,mp247,mp248,mp249,mp250,mp251,mp252,mp253,mp254,mp255</param>
                            /// <param name="size">The new size. With the '+' sign the value is added to the actual size of the volume and without it, the value is taken as an absolute one. Shrinking disk size is not supported.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result SetRest(string disk, string size, string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                parameters.Add("size", size);
                                parameters.Add("digest", digest);
                                return _client.Set($"/nodes/{_node}/lxc/{_vmid}/resize", parameters);
                            }

                            /// <summary>
                            /// Resize a container mount point.
                            /// </summary>
                            /// <param name="disk">The disk you want to resize.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9,mp10,mp11,mp12,mp13,mp14,mp15,mp16,mp17,mp18,mp19,mp20,mp21,mp22,mp23,mp24,mp25,mp26,mp27,mp28,mp29,mp30,mp31,mp32,mp33,mp34,mp35,mp36,mp37,mp38,mp39,mp40,mp41,mp42,mp43,mp44,mp45,mp46,mp47,mp48,mp49,mp50,mp51,mp52,mp53,mp54,mp55,mp56,mp57,mp58,mp59,mp60,mp61,mp62,mp63,mp64,mp65,mp66,mp67,mp68,mp69,mp70,mp71,mp72,mp73,mp74,mp75,mp76,mp77,mp78,mp79,mp80,mp81,mp82,mp83,mp84,mp85,mp86,mp87,mp88,mp89,mp90,mp91,mp92,mp93,mp94,mp95,mp96,mp97,mp98,mp99,mp100,mp101,mp102,mp103,mp104,mp105,mp106,mp107,mp108,mp109,mp110,mp111,mp112,mp113,mp114,mp115,mp116,mp117,mp118,mp119,mp120,mp121,mp122,mp123,mp124,mp125,mp126,mp127,mp128,mp129,mp130,mp131,mp132,mp133,mp134,mp135,mp136,mp137,mp138,mp139,mp140,mp141,mp142,mp143,mp144,mp145,mp146,mp147,mp148,mp149,mp150,mp151,mp152,mp153,mp154,mp155,mp156,mp157,mp158,mp159,mp160,mp161,mp162,mp163,mp164,mp165,mp166,mp167,mp168,mp169,mp170,mp171,mp172,mp173,mp174,mp175,mp176,mp177,mp178,mp179,mp180,mp181,mp182,mp183,mp184,mp185,mp186,mp187,mp188,mp189,mp190,mp191,mp192,mp193,mp194,mp195,mp196,mp197,mp198,mp199,mp200,mp201,mp202,mp203,mp204,mp205,mp206,mp207,mp208,mp209,mp210,mp211,mp212,mp213,mp214,mp215,mp216,mp217,mp218,mp219,mp220,mp221,mp222,mp223,mp224,mp225,mp226,mp227,mp228,mp229,mp230,mp231,mp232,mp233,mp234,mp235,mp236,mp237,mp238,mp239,mp240,mp241,mp242,mp243,mp244,mp245,mp246,mp247,mp248,mp249,mp250,mp251,mp252,mp253,mp254,mp255</param>
                            /// <param name="size">The new size. With the '+' sign the value is added to the actual size of the volume and without it, the value is taken as an absolute one. Shrinking disk size is not supported.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result ResizeVm(string disk, string size, string digest = null) => SetRest(disk, size, digest);
                        }
                        public class PVEMoveVolume
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PVEMoveVolume(Client client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Move a rootfs-/mp-volume to a different storage
                            /// </summary>
                            /// <param name="storage">Target Storage.</param>
                            /// <param name="volume">Volume which will be moved.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9,mp10,mp11,mp12,mp13,mp14,mp15,mp16,mp17,mp18,mp19,mp20,mp21,mp22,mp23,mp24,mp25,mp26,mp27,mp28,mp29,mp30,mp31,mp32,mp33,mp34,mp35,mp36,mp37,mp38,mp39,mp40,mp41,mp42,mp43,mp44,mp45,mp46,mp47,mp48,mp49,mp50,mp51,mp52,mp53,mp54,mp55,mp56,mp57,mp58,mp59,mp60,mp61,mp62,mp63,mp64,mp65,mp66,mp67,mp68,mp69,mp70,mp71,mp72,mp73,mp74,mp75,mp76,mp77,mp78,mp79,mp80,mp81,mp82,mp83,mp84,mp85,mp86,mp87,mp88,mp89,mp90,mp91,mp92,mp93,mp94,mp95,mp96,mp97,mp98,mp99,mp100,mp101,mp102,mp103,mp104,mp105,mp106,mp107,mp108,mp109,mp110,mp111,mp112,mp113,mp114,mp115,mp116,mp117,mp118,mp119,mp120,mp121,mp122,mp123,mp124,mp125,mp126,mp127,mp128,mp129,mp130,mp131,mp132,mp133,mp134,mp135,mp136,mp137,mp138,mp139,mp140,mp141,mp142,mp143,mp144,mp145,mp146,mp147,mp148,mp149,mp150,mp151,mp152,mp153,mp154,mp155,mp156,mp157,mp158,mp159,mp160,mp161,mp162,mp163,mp164,mp165,mp166,mp167,mp168,mp169,mp170,mp171,mp172,mp173,mp174,mp175,mp176,mp177,mp178,mp179,mp180,mp181,mp182,mp183,mp184,mp185,mp186,mp187,mp188,mp189,mp190,mp191,mp192,mp193,mp194,mp195,mp196,mp197,mp198,mp199,mp200,mp201,mp202,mp203,mp204,mp205,mp206,mp207,mp208,mp209,mp210,mp211,mp212,mp213,mp214,mp215,mp216,mp217,mp218,mp219,mp220,mp221,mp222,mp223,mp224,mp225,mp226,mp227,mp228,mp229,mp230,mp231,mp232,mp233,mp234,mp235,mp236,mp237,mp238,mp239,mp240,mp241,mp242,mp243,mp244,mp245,mp246,mp247,mp248,mp249,mp250,mp251,mp252,mp253,mp254,mp255</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="delete">Delete the original volume after successful copy. By default the original is kept as an unused volume entry.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result CreateRest(string storage, string volume, int? bwlimit = null, bool? delete = null, string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("storage", storage);
                                parameters.Add("volume", volume);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/move_volume", parameters);
                            }

                            /// <summary>
                            /// Move a rootfs-/mp-volume to a different storage
                            /// </summary>
                            /// <param name="storage">Target Storage.</param>
                            /// <param name="volume">Volume which will be moved.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9,mp10,mp11,mp12,mp13,mp14,mp15,mp16,mp17,mp18,mp19,mp20,mp21,mp22,mp23,mp24,mp25,mp26,mp27,mp28,mp29,mp30,mp31,mp32,mp33,mp34,mp35,mp36,mp37,mp38,mp39,mp40,mp41,mp42,mp43,mp44,mp45,mp46,mp47,mp48,mp49,mp50,mp51,mp52,mp53,mp54,mp55,mp56,mp57,mp58,mp59,mp60,mp61,mp62,mp63,mp64,mp65,mp66,mp67,mp68,mp69,mp70,mp71,mp72,mp73,mp74,mp75,mp76,mp77,mp78,mp79,mp80,mp81,mp82,mp83,mp84,mp85,mp86,mp87,mp88,mp89,mp90,mp91,mp92,mp93,mp94,mp95,mp96,mp97,mp98,mp99,mp100,mp101,mp102,mp103,mp104,mp105,mp106,mp107,mp108,mp109,mp110,mp111,mp112,mp113,mp114,mp115,mp116,mp117,mp118,mp119,mp120,mp121,mp122,mp123,mp124,mp125,mp126,mp127,mp128,mp129,mp130,mp131,mp132,mp133,mp134,mp135,mp136,mp137,mp138,mp139,mp140,mp141,mp142,mp143,mp144,mp145,mp146,mp147,mp148,mp149,mp150,mp151,mp152,mp153,mp154,mp155,mp156,mp157,mp158,mp159,mp160,mp161,mp162,mp163,mp164,mp165,mp166,mp167,mp168,mp169,mp170,mp171,mp172,mp173,mp174,mp175,mp176,mp177,mp178,mp179,mp180,mp181,mp182,mp183,mp184,mp185,mp186,mp187,mp188,mp189,mp190,mp191,mp192,mp193,mp194,mp195,mp196,mp197,mp198,mp199,mp200,mp201,mp202,mp203,mp204,mp205,mp206,mp207,mp208,mp209,mp210,mp211,mp212,mp213,mp214,mp215,mp216,mp217,mp218,mp219,mp220,mp221,mp222,mp223,mp224,mp225,mp226,mp227,mp228,mp229,mp230,mp231,mp232,mp233,mp234,mp235,mp236,mp237,mp238,mp239,mp240,mp241,mp242,mp243,mp244,mp245,mp246,mp247,mp248,mp249,mp250,mp251,mp252,mp253,mp254,mp255</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="delete">Delete the original volume after successful copy. By default the original is kept as an unused volume entry.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result MoveVolume(string storage, string volume, int? bwlimit = null, bool? delete = null, string digest = null) => CreateRest(storage, volume, bwlimit, delete, digest);
                        }
                        /// <summary>
                        /// Destroy the container (also delete all uses files).
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/lxc/{_vmid}"); }

                        /// <summary>
                        /// Destroy the container (also delete all uses files).
                        /// </summary>
                        /// <returns></returns>
                        public Result DestroyVm() => DeleteRest();
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc/{_vmid}"); }

                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result Vmdiridx() => GetRest();
                    }
                    /// <summary>
                    /// LXC container index (per node).
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/lxc"); }

                    /// <summary>
                    /// LXC container index (per node).
                    /// </summary>
                    /// <returns></returns>
                    public Result Vmlist() => GetRest();
                    /// <summary>
                    /// Create or restore a container.
                    /// </summary>
                    /// <param name="ostemplate">The OS template or backup file.</param>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="arch">OS architecture type.
                    ///   Enum: amd64,i386,arm64,armhf</param>
                    /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                    /// <param name="cmode">Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode to 'shell', it simply invokes a shell inside the container (no login).
                    ///   Enum: shell,console,tty</param>
                    /// <param name="console">Attach a console device (/dev/console) to the container.</param>
                    /// <param name="cores">The number of cores assigned to the container. A container can use all available cores by default.</param>
                    /// <param name="cpulimit">Limit of CPU usage.  NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.</param>
                    /// <param name="cpuunits">CPU weight for a VM. Argument is used in the kernel fair scheduler. The larger the number is, the more CPU time this VM gets. Number is relative to the weights of all the other running VMs.  NOTE: You can disable fair-scheduler configuration by setting this to 0.</param>
                    /// <param name="description">Container description. Only used on the configuration web interface.</param>
                    /// <param name="features">Allow containers access to advanced features.</param>
                    /// <param name="force">Allow to overwrite existing container.</param>
                    /// <param name="hookscript">Script that will be exectued during various steps in the containers lifetime.</param>
                    /// <param name="hostname">Set a host name for the container.</param>
                    /// <param name="ignore_unpack_errors">Ignore errors when extracting the template.</param>
                    /// <param name="lock_">Lock/unlock the VM.
                    ///   Enum: backup,create,disk,fstrim,migrate,mounted,rollback,snapshot,snapshot-delete</param>
                    /// <param name="memory">Amount of RAM for the VM in MB.</param>
                    /// <param name="mpN">Use volume as container mount point.</param>
                    /// <param name="nameserver">Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                    /// <param name="netN">Specifies network interfaces for the container.</param>
                    /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                    /// <param name="ostype">OS type. This is used to setup configuration inside the container, and corresponds to lxc setup scripts in /usr/share/lxc/config/&amp;lt;ostype&amp;gt;.common.conf. Value 'unmanaged' can be used to skip and OS specific setup.
                    ///   Enum: debian,ubuntu,centos,fedora,opensuse,archlinux,alpine,gentoo,unmanaged</param>
                    /// <param name="password">Sets root password inside container.</param>
                    /// <param name="pool">Add the VM to the specified pool.</param>
                    /// <param name="protection">Sets the protection flag of the container. This will prevent the CT or CT's disk remove/update operation.</param>
                    /// <param name="restore">Mark this as restore task.</param>
                    /// <param name="rootfs">Use volume as container root.</param>
                    /// <param name="searchdomain">Sets DNS search domains for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                    /// <param name="ssh_public_keys">Setup public SSH keys (one key per line, OpenSSH format).</param>
                    /// <param name="start">Start the CT after its creation finished successfully.</param>
                    /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                    /// <param name="storage">Default Storage.</param>
                    /// <param name="swap">Amount of SWAP for the VM in MB.</param>
                    /// <param name="template">Enable/disable Template.</param>
                    /// <param name="tty">Specify the number of tty available to the container</param>
                    /// <param name="unique">Assign a unique random ethernet address.</param>
                    /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <returns></returns>
                    public Result CreateRest(string ostemplate, int vmid, string arch = null, int? bwlimit = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string description = null, string features = null, bool? force = null, string hookscript = null, string hostname = null, bool? ignore_unpack_errors = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, string password = null, string pool = null, bool? protection = null, bool? restore = null, string rootfs = null, string searchdomain = null, string ssh_public_keys = null, bool? start = null, string startup = null, string storage = null, int? swap = null, bool? template = null, int? tty = null, bool? unique = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("ostemplate", ostemplate);
                        parameters.Add("vmid", vmid);
                        parameters.Add("arch", arch);
                        parameters.Add("bwlimit", bwlimit);
                        parameters.Add("cmode", cmode);
                        parameters.Add("console", console);
                        parameters.Add("cores", cores);
                        parameters.Add("cpulimit", cpulimit);
                        parameters.Add("cpuunits", cpuunits);
                        parameters.Add("description", description);
                        parameters.Add("features", features);
                        parameters.Add("force", force);
                        parameters.Add("hookscript", hookscript);
                        parameters.Add("hostname", hostname);
                        parameters.Add("ignore-unpack-errors", ignore_unpack_errors);
                        parameters.Add("lock", lock_);
                        parameters.Add("memory", memory);
                        parameters.Add("nameserver", nameserver);
                        parameters.Add("onboot", onboot);
                        parameters.Add("ostype", ostype);
                        parameters.Add("password", password);
                        parameters.Add("pool", pool);
                        parameters.Add("protection", protection);
                        parameters.Add("restore", restore);
                        parameters.Add("rootfs", rootfs);
                        parameters.Add("searchdomain", searchdomain);
                        parameters.Add("ssh-public-keys", ssh_public_keys);
                        parameters.Add("start", start);
                        parameters.Add("startup", startup);
                        parameters.Add("storage", storage);
                        parameters.Add("swap", swap);
                        parameters.Add("template", template);
                        parameters.Add("tty", tty);
                        parameters.Add("unique", unique);
                        parameters.Add("unprivileged", unprivileged);
                        AddIndexedParameter(parameters, "mp", mpN);
                        AddIndexedParameter(parameters, "net", netN);
                        AddIndexedParameter(parameters, "unused", unusedN);
                        return _client.Create($"/nodes/{_node}/lxc", parameters);
                    }

                    /// <summary>
                    /// Create or restore a container.
                    /// </summary>
                    /// <param name="ostemplate">The OS template or backup file.</param>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="arch">OS architecture type.
                    ///   Enum: amd64,i386,arm64,armhf</param>
                    /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                    /// <param name="cmode">Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode to 'shell', it simply invokes a shell inside the container (no login).
                    ///   Enum: shell,console,tty</param>
                    /// <param name="console">Attach a console device (/dev/console) to the container.</param>
                    /// <param name="cores">The number of cores assigned to the container. A container can use all available cores by default.</param>
                    /// <param name="cpulimit">Limit of CPU usage.  NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.</param>
                    /// <param name="cpuunits">CPU weight for a VM. Argument is used in the kernel fair scheduler. The larger the number is, the more CPU time this VM gets. Number is relative to the weights of all the other running VMs.  NOTE: You can disable fair-scheduler configuration by setting this to 0.</param>
                    /// <param name="description">Container description. Only used on the configuration web interface.</param>
                    /// <param name="features">Allow containers access to advanced features.</param>
                    /// <param name="force">Allow to overwrite existing container.</param>
                    /// <param name="hookscript">Script that will be exectued during various steps in the containers lifetime.</param>
                    /// <param name="hostname">Set a host name for the container.</param>
                    /// <param name="ignore_unpack_errors">Ignore errors when extracting the template.</param>
                    /// <param name="lock_">Lock/unlock the VM.
                    ///   Enum: backup,create,disk,fstrim,migrate,mounted,rollback,snapshot,snapshot-delete</param>
                    /// <param name="memory">Amount of RAM for the VM in MB.</param>
                    /// <param name="mpN">Use volume as container mount point.</param>
                    /// <param name="nameserver">Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                    /// <param name="netN">Specifies network interfaces for the container.</param>
                    /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                    /// <param name="ostype">OS type. This is used to setup configuration inside the container, and corresponds to lxc setup scripts in /usr/share/lxc/config/&amp;lt;ostype&amp;gt;.common.conf. Value 'unmanaged' can be used to skip and OS specific setup.
                    ///   Enum: debian,ubuntu,centos,fedora,opensuse,archlinux,alpine,gentoo,unmanaged</param>
                    /// <param name="password">Sets root password inside container.</param>
                    /// <param name="pool">Add the VM to the specified pool.</param>
                    /// <param name="protection">Sets the protection flag of the container. This will prevent the CT or CT's disk remove/update operation.</param>
                    /// <param name="restore">Mark this as restore task.</param>
                    /// <param name="rootfs">Use volume as container root.</param>
                    /// <param name="searchdomain">Sets DNS search domains for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                    /// <param name="ssh_public_keys">Setup public SSH keys (one key per line, OpenSSH format).</param>
                    /// <param name="start">Start the CT after its creation finished successfully.</param>
                    /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                    /// <param name="storage">Default Storage.</param>
                    /// <param name="swap">Amount of SWAP for the VM in MB.</param>
                    /// <param name="template">Enable/disable Template.</param>
                    /// <param name="tty">Specify the number of tty available to the container</param>
                    /// <param name="unique">Assign a unique random ethernet address.</param>
                    /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <returns></returns>
                    public Result CreateVm(string ostemplate, int vmid, string arch = null, int? bwlimit = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string description = null, string features = null, bool? force = null, string hookscript = null, string hostname = null, bool? ignore_unpack_errors = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, string password = null, string pool = null, bool? protection = null, bool? restore = null, string rootfs = null, string searchdomain = null, string ssh_public_keys = null, bool? start = null, string startup = null, string storage = null, int? swap = null, bool? template = null, int? tty = null, bool? unique = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null) => CreateRest(ostemplate, vmid, arch, bwlimit, cmode, console, cores, cpulimit, cpuunits, description, features, force, hookscript, hostname, ignore_unpack_errors, lock_, memory, mpN, nameserver, netN, onboot, ostype, password, pool, protection, restore, rootfs, searchdomain, ssh_public_keys, start, startup, storage, swap, template, tty, unique, unprivileged, unusedN);
                }
                public class PVECeph
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVECeph(Client client, object node) { _client = client; _node = node; }
                    private PVEOsd _osd;
                    public PVEOsd Osd => _osd ?? (_osd = new PVEOsd(_client, _node));
                    private PVEMds _mds;
                    public PVEMds Mds => _mds ?? (_mds = new PVEMds(_client, _node));
                    private PVEMgr _mgr;
                    public PVEMgr Mgr => _mgr ?? (_mgr = new PVEMgr(_client, _node));
                    private PVEMon _mon;
                    public PVEMon Mon => _mon ?? (_mon = new PVEMon(_client, _node));
                    private PVEFs _fs;
                    public PVEFs Fs => _fs ?? (_fs = new PVEFs(_client, _node));
                    private PVEDisks _disks;
                    public PVEDisks Disks => _disks ?? (_disks = new PVEDisks(_client, _node));
                    private PVEConfig _config;
                    public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node));
                    private PVEConfigdb _configdb;
                    public PVEConfigdb Configdb => _configdb ?? (_configdb = new PVEConfigdb(_client, _node));
                    private PVEInit _init;
                    public PVEInit Init => _init ?? (_init = new PVEInit(_client, _node));
                    private PVEStop _stop;
                    public PVEStop Stop => _stop ?? (_stop = new PVEStop(_client, _node));
                    private PVEStart _start;
                    public PVEStart Start => _start ?? (_start = new PVEStart(_client, _node));
                    private PVERestart _restart;
                    public PVERestart Restart => _restart ?? (_restart = new PVERestart(_client, _node));
                    private PVEStatus _status;
                    public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node));
                    private PVEPools _pools;
                    public PVEPools Pools => _pools ?? (_pools = new PVEPools(_client, _node));
                    private PVEFlags _flags;
                    public PVEFlags Flags => _flags ?? (_flags = new PVEFlags(_client, _node));
                    private PVECrush _crush;
                    public PVECrush Crush => _crush ?? (_crush = new PVECrush(_client, _node));
                    private PVELog _log;
                    public PVELog Log => _log ?? (_log = new PVELog(_client, _node));
                    private PVERules _rules;
                    public PVERules Rules => _rules ?? (_rules = new PVERules(_client, _node));
                    public class PVEOsd
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEOsd(Client client, object node) { _client = client; _node = node; }
                        public PVEItemOsdid this[object osdid] => new PVEItemOsdid(_client, _node, osdid);
                        public class PVEItemOsdid
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _osdid;
                            internal PVEItemOsdid(Client client, object node, object osdid)
                            {
                                _client = client; _node = node;
                                _osdid = osdid;
                            }
                            private PVEIn _in;
                            public PVEIn In => _in ?? (_in = new PVEIn(_client, _node, _osdid));
                            private PVEOut _out;
                            public PVEOut Out => _out ?? (_out = new PVEOut(_client, _node, _osdid));
                            private PVEScrub _scrub;
                            public PVEScrub Scrub => _scrub ?? (_scrub = new PVEScrub(_client, _node, _osdid));
                            public class PVEIn
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _osdid;
                                internal PVEIn(Client client, object node, object osdid)
                                {
                                    _client = client; _node = node;
                                    _osdid = osdid;
                                }
                                /// <summary>
                                /// ceph osd in
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/ceph/osd/{_osdid}/in"); }

                                /// <summary>
                                /// ceph osd in
                                /// </summary>
                                /// <returns></returns>
                                public Result In() => CreateRest();
                            }
                            public class PVEOut
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _osdid;
                                internal PVEOut(Client client, object node, object osdid)
                                {
                                    _client = client; _node = node;
                                    _osdid = osdid;
                                }
                                /// <summary>
                                /// ceph osd out
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest() { return _client.Create($"/nodes/{_node}/ceph/osd/{_osdid}/out"); }

                                /// <summary>
                                /// ceph osd out
                                /// </summary>
                                /// <returns></returns>
                                public Result Out() => CreateRest();
                            }
                            public class PVEScrub
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _osdid;
                                internal PVEScrub(Client client, object node, object osdid)
                                {
                                    _client = client; _node = node;
                                    _osdid = osdid;
                                }
                                /// <summary>
                                /// Instruct the OSD to scrub.
                                /// </summary>
                                /// <param name="deep">If set, instructs a deep scrub instead of a normal one.</param>
                                /// <returns></returns>
                                public Result CreateRest(bool? deep = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("deep", deep);
                                    return _client.Create($"/nodes/{_node}/ceph/osd/{_osdid}/scrub", parameters);
                                }

                                /// <summary>
                                /// Instruct the OSD to scrub.
                                /// </summary>
                                /// <param name="deep">If set, instructs a deep scrub instead of a normal one.</param>
                                /// <returns></returns>
                                public Result Scrub(bool? deep = null) => CreateRest(deep);
                            }
                            /// <summary>
                            /// Destroy OSD
                            /// </summary>
                            /// <param name="cleanup">If set, we remove partition table entries.</param>
                            /// <returns></returns>
                            public Result DeleteRest(bool? cleanup = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cleanup", cleanup);
                                return _client.Delete($"/nodes/{_node}/ceph/osd/{_osdid}", parameters);
                            }

                            /// <summary>
                            /// Destroy OSD
                            /// </summary>
                            /// <param name="cleanup">If set, we remove partition table entries.</param>
                            /// <returns></returns>
                            public Result Destroyosd(bool? cleanup = null) => DeleteRest(cleanup);
                        }
                        /// <summary>
                        /// Get Ceph osd list/tree.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/osd"); }

                        /// <summary>
                        /// Get Ceph osd list/tree.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                        /// <summary>
                        /// Create OSD
                        /// </summary>
                        /// <param name="dev">Block device name.</param>
                        /// <param name="db_dev">Block device name for block.db.</param>
                        /// <param name="db_size">Size in GiB for block.db.</param>
                        /// <param name="encrypted">Enables encryption of the OSD.</param>
                        /// <param name="wal_dev">Block device name for block.wal.</param>
                        /// <param name="wal_size">Size in GiB for block.wal.</param>
                        /// <returns></returns>
                        public Result CreateRest(string dev, string db_dev = null, int? db_size = null, bool? encrypted = null, string wal_dev = null, int? wal_size = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("dev", dev);
                            parameters.Add("db_dev", db_dev);
                            parameters.Add("db_size", db_size);
                            parameters.Add("encrypted", encrypted);
                            parameters.Add("wal_dev", wal_dev);
                            parameters.Add("wal_size", wal_size);
                            return _client.Create($"/nodes/{_node}/ceph/osd", parameters);
                        }

                        /// <summary>
                        /// Create OSD
                        /// </summary>
                        /// <param name="dev">Block device name.</param>
                        /// <param name="db_dev">Block device name for block.db.</param>
                        /// <param name="db_size">Size in GiB for block.db.</param>
                        /// <param name="encrypted">Enables encryption of the OSD.</param>
                        /// <param name="wal_dev">Block device name for block.wal.</param>
                        /// <param name="wal_size">Size in GiB for block.wal.</param>
                        /// <returns></returns>
                        public Result Createosd(string dev, string db_dev = null, int? db_size = null, bool? encrypted = null, string wal_dev = null, int? wal_size = null) => CreateRest(dev, db_dev, db_size, encrypted, wal_dev, wal_size);
                    }
                    public class PVEMds
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEMds(Client client, object node) { _client = client; _node = node; }
                        public PVEItemName this[object name] => new PVEItemName(_client, _node, name);
                        public class PVEItemName
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PVEItemName(Client client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Destroy Ceph Metadata Server
                            /// </summary>
                            /// <returns></returns>
                            public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/ceph/mds/{_name}"); }

                            /// <summary>
                            /// Destroy Ceph Metadata Server
                            /// </summary>
                            /// <returns></returns>
                            public Result Destroymds() => DeleteRest();
                            /// <summary>
                            /// Create Ceph Metadata Server (MDS)
                            /// </summary>
                            /// <param name="hotstandby">Determines whether a ceph-mds daemon should poll and replay the log of an active MDS. Faster switch on MDS failure, but needs more idle resources.</param>
                            /// <returns></returns>
                            public Result CreateRest(bool? hotstandby = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("hotstandby", hotstandby);
                                return _client.Create($"/nodes/{_node}/ceph/mds/{_name}", parameters);
                            }

                            /// <summary>
                            /// Create Ceph Metadata Server (MDS)
                            /// </summary>
                            /// <param name="hotstandby">Determines whether a ceph-mds daemon should poll and replay the log of an active MDS. Faster switch on MDS failure, but needs more idle resources.</param>
                            /// <returns></returns>
                            public Result Createmds(bool? hotstandby = null) => CreateRest(hotstandby);
                        }
                        /// <summary>
                        /// MDS directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/mds"); }

                        /// <summary>
                        /// MDS directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                    }
                    public class PVEMgr
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEMgr(Client client, object node) { _client = client; _node = node; }
                        public PVEItemId this[object id] => new PVEItemId(_client, _node, id);
                        public class PVEItemId
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _id;
                            internal PVEItemId(Client client, object node, object id)
                            {
                                _client = client; _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Destroy Ceph Manager.
                            /// </summary>
                            /// <returns></returns>
                            public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/ceph/mgr/{_id}"); }

                            /// <summary>
                            /// Destroy Ceph Manager.
                            /// </summary>
                            /// <returns></returns>
                            public Result Destroymgr() => DeleteRest();
                            /// <summary>
                            /// Create Ceph Manager
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/ceph/mgr/{_id}"); }

                            /// <summary>
                            /// Create Ceph Manager
                            /// </summary>
                            /// <returns></returns>
                            public Result Createmgr() => CreateRest();
                        }
                        /// <summary>
                        /// MGR directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/mgr"); }

                        /// <summary>
                        /// MGR directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                    }
                    public class PVEMon
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEMon(Client client, object node) { _client = client; _node = node; }
                        public PVEItemMonid this[object monid] => new PVEItemMonid(_client, _node, monid);
                        public class PVEItemMonid
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _monid;
                            internal PVEItemMonid(Client client, object node, object monid)
                            {
                                _client = client; _node = node;
                                _monid = monid;
                            }
                            /// <summary>
                            /// Destroy Ceph Monitor and Manager.
                            /// </summary>
                            /// <returns></returns>
                            public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/ceph/mon/{_monid}"); }

                            /// <summary>
                            /// Destroy Ceph Monitor and Manager.
                            /// </summary>
                            /// <returns></returns>
                            public Result Destroymon() => DeleteRest();
                            /// <summary>
                            /// Create Ceph Monitor and Manager
                            /// </summary>
                            /// <param name="mon_address">Overwrites autodetected monitor IP address. Must be in the public network of ceph.</param>
                            /// <returns></returns>
                            public Result CreateRest(string mon_address = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("mon-address", mon_address);
                                return _client.Create($"/nodes/{_node}/ceph/mon/{_monid}", parameters);
                            }

                            /// <summary>
                            /// Create Ceph Monitor and Manager
                            /// </summary>
                            /// <param name="mon_address">Overwrites autodetected monitor IP address. Must be in the public network of ceph.</param>
                            /// <returns></returns>
                            public Result Createmon(string mon_address = null) => CreateRest(mon_address);
                        }
                        /// <summary>
                        /// Get Ceph monitor list.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/mon"); }

                        /// <summary>
                        /// Get Ceph monitor list.
                        /// </summary>
                        /// <returns></returns>
                        public Result Listmon() => GetRest();
                    }
                    public class PVEFs
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEFs(Client client, object node) { _client = client; _node = node; }
                        public PVEItemName this[object name] => new PVEItemName(_client, _node, name);
                        public class PVEItemName
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PVEItemName(Client client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Create a Ceph filesystem
                            /// </summary>
                            /// <param name="add_storage">Configure the created CephFS as storage for this cluster.</param>
                            /// <param name="pg_num">Number of placement groups for the backing data pool. The metadata pool will use a quarter of this.</param>
                            /// <returns></returns>
                            public Result CreateRest(bool? add_storage = null, int? pg_num = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("add-storage", add_storage);
                                parameters.Add("pg_num", pg_num);
                                return _client.Create($"/nodes/{_node}/ceph/fs/{_name}", parameters);
                            }

                            /// <summary>
                            /// Create a Ceph filesystem
                            /// </summary>
                            /// <param name="add_storage">Configure the created CephFS as storage for this cluster.</param>
                            /// <param name="pg_num">Number of placement groups for the backing data pool. The metadata pool will use a quarter of this.</param>
                            /// <returns></returns>
                            public Result Createfs(bool? add_storage = null, int? pg_num = null) => CreateRest(add_storage, pg_num);
                        }
                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/fs"); }

                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                    }
                    public class PVEDisks
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEDisks(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        /// <param name="type">Only list specific types of disks.
                        ///   Enum: unused,journal_disks</param>
                        /// <returns></returns>
                        public Result GetRest(string type = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("type", type);
                            return _client.Get($"/nodes/{_node}/ceph/disks", parameters);
                        }

                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        /// <param name="type">Only list specific types of disks.
                        ///   Enum: unused,journal_disks</param>
                        /// <returns></returns>
                        public Result Disks(string type = null) => GetRest(type);
                    }
                    public class PVEConfig
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEConfig(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get Ceph configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/config"); }

                        /// <summary>
                        /// Get Ceph configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Config() => GetRest();
                    }
                    public class PVEConfigdb
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEConfigdb(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get Ceph configuration database.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/configdb"); }

                        /// <summary>
                        /// Get Ceph configuration database.
                        /// </summary>
                        /// <returns></returns>
                        public Result Configdb() => GetRest();
                    }
                    public class PVEInit
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEInit(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Create initial ceph default configuration and setup symlinks.
                        /// </summary>
                        /// <param name="cluster_network">Declare a separate cluster network, OSDs will routeheartbeat, object replication and recovery traffic over it</param>
                        /// <param name="disable_cephx">Disable cephx authentication.  WARNING: cephx is a security feature protecting against man-in-the-middle attacks. Only consider disabling cephx if your network is private!</param>
                        /// <param name="min_size">Minimum number of available replicas per object to allow I/O</param>
                        /// <param name="network">Use specific network for all ceph related traffic</param>
                        /// <param name="pg_bits">Placement group bits, used to specify the default number of placement groups.  NOTE: 'osd pool default pg num' does not work for default pools.</param>
                        /// <param name="size">Targeted number of replicas per object</param>
                        /// <returns></returns>
                        public Result CreateRest(string cluster_network = null, bool? disable_cephx = null, int? min_size = null, string network = null, int? pg_bits = null, int? size = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("cluster-network", cluster_network);
                            parameters.Add("disable_cephx", disable_cephx);
                            parameters.Add("min_size", min_size);
                            parameters.Add("network", network);
                            parameters.Add("pg_bits", pg_bits);
                            parameters.Add("size", size);
                            return _client.Create($"/nodes/{_node}/ceph/init", parameters);
                        }

                        /// <summary>
                        /// Create initial ceph default configuration and setup symlinks.
                        /// </summary>
                        /// <param name="cluster_network">Declare a separate cluster network, OSDs will routeheartbeat, object replication and recovery traffic over it</param>
                        /// <param name="disable_cephx">Disable cephx authentication.  WARNING: cephx is a security feature protecting against man-in-the-middle attacks. Only consider disabling cephx if your network is private!</param>
                        /// <param name="min_size">Minimum number of available replicas per object to allow I/O</param>
                        /// <param name="network">Use specific network for all ceph related traffic</param>
                        /// <param name="pg_bits">Placement group bits, used to specify the default number of placement groups.  NOTE: 'osd pool default pg num' does not work for default pools.</param>
                        /// <param name="size">Targeted number of replicas per object</param>
                        /// <returns></returns>
                        public Result Init(string cluster_network = null, bool? disable_cephx = null, int? min_size = null, string network = null, int? pg_bits = null, int? size = null) => CreateRest(cluster_network, disable_cephx, min_size, network, pg_bits, size);
                    }
                    public class PVEStop
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEStop(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Stop ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public Result CreateRest(string service = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("service", service);
                            return _client.Create($"/nodes/{_node}/ceph/stop", parameters);
                        }

                        /// <summary>
                        /// Stop ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public Result Stop(string service = null) => CreateRest(service);
                    }
                    public class PVEStart
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEStart(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Start ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public Result CreateRest(string service = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("service", service);
                            return _client.Create($"/nodes/{_node}/ceph/start", parameters);
                        }

                        /// <summary>
                        /// Start ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public Result Start(string service = null) => CreateRest(service);
                    }
                    public class PVERestart
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVERestart(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Restart ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public Result CreateRest(string service = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("service", service);
                            return _client.Create($"/nodes/{_node}/ceph/restart", parameters);
                        }

                        /// <summary>
                        /// Restart ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public Result Restart(string service = null) => CreateRest(service);
                    }
                    public class PVEStatus
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEStatus(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get ceph status.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/status"); }

                        /// <summary>
                        /// Get ceph status.
                        /// </summary>
                        /// <returns></returns>
                        public Result Status() => GetRest();
                    }
                    public class PVEPools
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEPools(Client client, object node) { _client = client; _node = node; }
                        public PVEItemName this[object name] => new PVEItemName(_client, _node, name);
                        public class PVEItemName
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PVEItemName(Client client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Destroy pool
                            /// </summary>
                            /// <param name="force">If true, destroys pool even if in use</param>
                            /// <param name="remove_storages">Remove all pveceph-managed storages configured for this pool</param>
                            /// <returns></returns>
                            public Result DeleteRest(bool? force = null, bool? remove_storages = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                parameters.Add("remove_storages", remove_storages);
                                return _client.Delete($"/nodes/{_node}/ceph/pools/{_name}", parameters);
                            }

                            /// <summary>
                            /// Destroy pool
                            /// </summary>
                            /// <param name="force">If true, destroys pool even if in use</param>
                            /// <param name="remove_storages">Remove all pveceph-managed storages configured for this pool</param>
                            /// <returns></returns>
                            public Result Destroypool(bool? force = null, bool? remove_storages = null) => DeleteRest(force, remove_storages);
                        }
                        /// <summary>
                        /// List all pools.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/pools"); }

                        /// <summary>
                        /// List all pools.
                        /// </summary>
                        /// <returns></returns>
                        public Result Lspools() => GetRest();
                        /// <summary>
                        /// Create POOL
                        /// </summary>
                        /// <param name="name">The name of the pool. It must be unique.</param>
                        /// <param name="add_storages">Configure VM and CT storage using the new pool.</param>
                        /// <param name="application">The application of the pool, 'rbd' by default.
                        ///   Enum: rbd,cephfs,rgw</param>
                        /// <param name="crush_rule">The rule to use for mapping object placement in the cluster.</param>
                        /// <param name="min_size">Minimum number of replicas per object</param>
                        /// <param name="pg_num">Number of placement groups.</param>
                        /// <param name="size">Number of replicas per object</param>
                        /// <returns></returns>
                        public Result CreateRest(string name, bool? add_storages = null, string application = null, string crush_rule = null, int? min_size = null, int? pg_num = null, int? size = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("name", name);
                            parameters.Add("add_storages", add_storages);
                            parameters.Add("application", application);
                            parameters.Add("crush_rule", crush_rule);
                            parameters.Add("min_size", min_size);
                            parameters.Add("pg_num", pg_num);
                            parameters.Add("size", size);
                            return _client.Create($"/nodes/{_node}/ceph/pools", parameters);
                        }

                        /// <summary>
                        /// Create POOL
                        /// </summary>
                        /// <param name="name">The name of the pool. It must be unique.</param>
                        /// <param name="add_storages">Configure VM and CT storage using the new pool.</param>
                        /// <param name="application">The application of the pool, 'rbd' by default.
                        ///   Enum: rbd,cephfs,rgw</param>
                        /// <param name="crush_rule">The rule to use for mapping object placement in the cluster.</param>
                        /// <param name="min_size">Minimum number of replicas per object</param>
                        /// <param name="pg_num">Number of placement groups.</param>
                        /// <param name="size">Number of replicas per object</param>
                        /// <returns></returns>
                        public Result Createpool(string name, bool? add_storages = null, string application = null, string crush_rule = null, int? min_size = null, int? pg_num = null, int? size = null) => CreateRest(name, add_storages, application, crush_rule, min_size, pg_num, size);
                    }
                    public class PVEFlags
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEFlags(Client client, object node) { _client = client; _node = node; }
                        public PVEItemFlag this[object flag] => new PVEItemFlag(_client, _node, flag);
                        public class PVEItemFlag
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _flag;
                            internal PVEItemFlag(Client client, object node, object flag)
                            {
                                _client = client; _node = node;
                                _flag = flag;
                            }
                            /// <summary>
                            /// Unset a ceph flag
                            /// </summary>
                            /// <returns></returns>
                            public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/ceph/flags/{_flag}"); }

                            /// <summary>
                            /// Unset a ceph flag
                            /// </summary>
                            /// <returns></returns>
                            public Result UnsetFlag() => DeleteRest();
                            /// <summary>
                            /// Set a ceph flag
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/ceph/flags/{_flag}"); }

                            /// <summary>
                            /// Set a ceph flag
                            /// </summary>
                            /// <returns></returns>
                            public Result SetFlag() => CreateRest();
                        }
                        /// <summary>
                        /// get all set ceph flags
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/flags"); }

                        /// <summary>
                        /// get all set ceph flags
                        /// </summary>
                        /// <returns></returns>
                        public Result GetFlags() => GetRest();
                    }
                    public class PVECrush
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVECrush(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get OSD crush map
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/crush"); }

                        /// <summary>
                        /// Get OSD crush map
                        /// </summary>
                        /// <returns></returns>
                        public Result Crush() => GetRest();
                    }
                    public class PVELog
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVELog(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Read ceph log
                        /// </summary>
                        /// <param name="limit"></param>
                        /// <param name="start"></param>
                        /// <returns></returns>
                        public Result GetRest(int? limit = null, int? start = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("limit", limit);
                            parameters.Add("start", start);
                            return _client.Get($"/nodes/{_node}/ceph/log", parameters);
                        }

                        /// <summary>
                        /// Read ceph log
                        /// </summary>
                        /// <param name="limit"></param>
                        /// <param name="start"></param>
                        /// <returns></returns>
                        public Result Log(int? limit = null, int? start = null) => GetRest(limit, start);
                    }
                    public class PVERules
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVERules(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List ceph rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph/rules"); }

                        /// <summary>
                        /// List ceph rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result Rules() => GetRest();
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/ceph"); }

                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                public class PVEVzdump
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEVzdump(Client client, object node) { _client = client; _node = node; }
                    private PVEExtractconfig _extractconfig;
                    public PVEExtractconfig Extractconfig => _extractconfig ?? (_extractconfig = new PVEExtractconfig(_client, _node));
                    public class PVEExtractconfig
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEExtractconfig(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Extract configuration from vzdump backup archive.
                        /// </summary>
                        /// <param name="volume">Volume identifier</param>
                        /// <returns></returns>
                        public Result GetRest(string volume)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("volume", volume);
                            return _client.Get($"/nodes/{_node}/vzdump/extractconfig", parameters);
                        }

                        /// <summary>
                        /// Extract configuration from vzdump backup archive.
                        /// </summary>
                        /// <param name="volume">Volume identifier</param>
                        /// <returns></returns>
                        public Result Extractconfig(string volume) => GetRest(volume);
                    }
                    /// <summary>
                    /// Create backup.
                    /// </summary>
                    /// <param name="all">Backup all known guest systems on this host.</param>
                    /// <param name="bwlimit">Limit I/O bandwidth (KBytes per second).</param>
                    /// <param name="compress">Compress dump file.
                    ///   Enum: 0,1,gzip,lzo</param>
                    /// <param name="dumpdir">Store resulting files to specified directory.</param>
                    /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                    /// <param name="exclude_path">Exclude certain files/directories (shell globs).</param>
                    /// <param name="ionice">Set CFQ ionice priority.</param>
                    /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                    /// <param name="mailnotification">Specify when to send an email
                    ///   Enum: always,failure</param>
                    /// <param name="mailto">Comma-separated list of email addresses that should receive email notifications.</param>
                    /// <param name="maxfiles">Maximal number of backup files per guest system.</param>
                    /// <param name="mode">Backup mode.
                    ///   Enum: snapshot,suspend,stop</param>
                    /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                    /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                    /// <param name="quiet">Be quiet.</param>
                    /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                    /// <param name="script">Use specified hook script.</param>
                    /// <param name="size">Unused, will be removed in a future release.</param>
                    /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                    /// <param name="stdout">Write tar to stdout, not to a file.</param>
                    /// <param name="stop">Stop running backup jobs on this host.</param>
                    /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                    /// <param name="storage">Store resulting file to this storage.</param>
                    /// <param name="tmpdir">Store temporary files to specified directory.</param>
                    /// <param name="vmid">The ID of the guest system you want to backup.</param>
                    /// <returns></returns>
                    public Result CreateRest(bool? all = null, int? bwlimit = null, string compress = null, string dumpdir = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, int? pigz = null, string pool = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stdout = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("all", all);
                        parameters.Add("bwlimit", bwlimit);
                        parameters.Add("compress", compress);
                        parameters.Add("dumpdir", dumpdir);
                        parameters.Add("exclude", exclude);
                        parameters.Add("exclude-path", exclude_path);
                        parameters.Add("ionice", ionice);
                        parameters.Add("lockwait", lockwait);
                        parameters.Add("mailnotification", mailnotification);
                        parameters.Add("mailto", mailto);
                        parameters.Add("maxfiles", maxfiles);
                        parameters.Add("mode", mode);
                        parameters.Add("pigz", pigz);
                        parameters.Add("pool", pool);
                        parameters.Add("quiet", quiet);
                        parameters.Add("remove", remove);
                        parameters.Add("script", script);
                        parameters.Add("size", size);
                        parameters.Add("stdexcludes", stdexcludes);
                        parameters.Add("stdout", stdout);
                        parameters.Add("stop", stop);
                        parameters.Add("stopwait", stopwait);
                        parameters.Add("storage", storage);
                        parameters.Add("tmpdir", tmpdir);
                        parameters.Add("vmid", vmid);
                        return _client.Create($"/nodes/{_node}/vzdump", parameters);
                    }

                    /// <summary>
                    /// Create backup.
                    /// </summary>
                    /// <param name="all">Backup all known guest systems on this host.</param>
                    /// <param name="bwlimit">Limit I/O bandwidth (KBytes per second).</param>
                    /// <param name="compress">Compress dump file.
                    ///   Enum: 0,1,gzip,lzo</param>
                    /// <param name="dumpdir">Store resulting files to specified directory.</param>
                    /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                    /// <param name="exclude_path">Exclude certain files/directories (shell globs).</param>
                    /// <param name="ionice">Set CFQ ionice priority.</param>
                    /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                    /// <param name="mailnotification">Specify when to send an email
                    ///   Enum: always,failure</param>
                    /// <param name="mailto">Comma-separated list of email addresses that should receive email notifications.</param>
                    /// <param name="maxfiles">Maximal number of backup files per guest system.</param>
                    /// <param name="mode">Backup mode.
                    ///   Enum: snapshot,suspend,stop</param>
                    /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                    /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                    /// <param name="quiet">Be quiet.</param>
                    /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                    /// <param name="script">Use specified hook script.</param>
                    /// <param name="size">Unused, will be removed in a future release.</param>
                    /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                    /// <param name="stdout">Write tar to stdout, not to a file.</param>
                    /// <param name="stop">Stop running backup jobs on this host.</param>
                    /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                    /// <param name="storage">Store resulting file to this storage.</param>
                    /// <param name="tmpdir">Store temporary files to specified directory.</param>
                    /// <param name="vmid">The ID of the guest system you want to backup.</param>
                    /// <returns></returns>
                    public Result Vzdump(bool? all = null, int? bwlimit = null, string compress = null, string dumpdir = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, int? pigz = null, string pool = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stdout = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null) => CreateRest(all, bwlimit, compress, dumpdir, exclude, exclude_path, ionice, lockwait, mailnotification, mailto, maxfiles, mode, pigz, pool, quiet, remove, script, size, stdexcludes, stdout, stop, stopwait, storage, tmpdir, vmid);
                }
                public class PVEServices
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEServices(Client client, object node) { _client = client; _node = node; }
                    public PVEItemService this[object service] => new PVEItemService(_client, _node, service);
                    public class PVEItemService
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        private readonly object _service;
                        internal PVEItemService(Client client, object node, object service)
                        {
                            _client = client; _node = node;
                            _service = service;
                        }
                        private PVEState _state;
                        public PVEState State => _state ?? (_state = new PVEState(_client, _node, _service));
                        private PVEStart _start;
                        public PVEStart Start => _start ?? (_start = new PVEStart(_client, _node, _service));
                        private PVEStop _stop;
                        public PVEStop Stop => _stop ?? (_stop = new PVEStop(_client, _node, _service));
                        private PVERestart _restart;
                        public PVERestart Restart => _restart ?? (_restart = new PVERestart(_client, _node, _service));
                        private PVEReload _reload;
                        public PVEReload Reload => _reload ?? (_reload = new PVEReload(_client, _node, _service));
                        public class PVEState
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PVEState(Client client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Read service properties
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/services/{_service}/state"); }

                            /// <summary>
                            /// Read service properties
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceState() => GetRest();
                        }
                        public class PVEStart
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PVEStart(Client client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Start service.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/services/{_service}/start"); }

                            /// <summary>
                            /// Start service.
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceStart() => CreateRest();
                        }
                        public class PVEStop
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PVEStop(Client client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Stop service.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/services/{_service}/stop"); }

                            /// <summary>
                            /// Stop service.
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceStop() => CreateRest();
                        }
                        public class PVERestart
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PVERestart(Client client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Restart service.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/services/{_service}/restart"); }

                            /// <summary>
                            /// Restart service.
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceRestart() => CreateRest();
                        }
                        public class PVEReload
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PVEReload(Client client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Reload service.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/services/{_service}/reload"); }

                            /// <summary>
                            /// Reload service.
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceReload() => CreateRest();
                        }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/services/{_service}"); }

                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result Srvcmdidx() => GetRest();
                    }
                    /// <summary>
                    /// Service list.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/services"); }

                    /// <summary>
                    /// Service list.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                public class PVESubscription
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVESubscription(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read subscription info.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/subscription"); }

                    /// <summary>
                    /// Read subscription info.
                    /// </summary>
                    /// <returns></returns>
                    public Result Get() => GetRest();
                    /// <summary>
                    /// Update subscription info.
                    /// </summary>
                    /// <param name="force">Always connect to server, even if we have up to date info inside local cache.</param>
                    /// <returns></returns>
                    public Result CreateRest(bool? force = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        return _client.Create($"/nodes/{_node}/subscription", parameters);
                    }

                    /// <summary>
                    /// Update subscription info.
                    /// </summary>
                    /// <param name="force">Always connect to server, even if we have up to date info inside local cache.</param>
                    /// <returns></returns>
                    public Result Update(bool? force = null) => CreateRest(force);
                    /// <summary>
                    /// Set subscription key.
                    /// </summary>
                    /// <param name="key">Proxmox VE subscription key</param>
                    /// <returns></returns>
                    public Result SetRest(string key)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("key", key);
                        return _client.Set($"/nodes/{_node}/subscription", parameters);
                    }

                    /// <summary>
                    /// Set subscription key.
                    /// </summary>
                    /// <param name="key">Proxmox VE subscription key</param>
                    /// <returns></returns>
                    public Result Set(string key) => SetRest(key);
                }
                public class PVENetwork
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVENetwork(Client client, object node) { _client = client; _node = node; }
                    public PVEItemIface this[object iface] => new PVEItemIface(_client, _node, iface);
                    public class PVEItemIface
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        private readonly object _iface;
                        internal PVEItemIface(Client client, object node, object iface)
                        {
                            _client = client; _node = node;
                            _iface = iface;
                        }
                        /// <summary>
                        /// Delete network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/network/{_iface}"); }

                        /// <summary>
                        /// Delete network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteNetwork() => DeleteRest();
                        /// <summary>
                        /// Read network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/network/{_iface}"); }

                        /// <summary>
                        /// Read network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public Result NetworkConfig() => GetRest();
                        /// <summary>
                        /// Update network device configuration
                        /// </summary>
                        /// <param name="type">Network interface type
                        ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,unknown</param>
                        /// <param name="address">IP address.</param>
                        /// <param name="address6">IP address.</param>
                        /// <param name="autostart">Automatically start interface on boot.</param>
                        /// <param name="bond_mode">Bonding mode.
                        ///   Enum: balance-rr,active-backup,balance-xor,broadcast,802.3ad,balance-tlb,balance-alb,balance-slb,lacp-balance-slb,lacp-balance-tcp</param>
                        /// <param name="bond_xmit_hash_policy">Selects the transmit hash policy to use for slave selection in balance-xor and 802.3ad modes.
                        ///   Enum: layer2,layer2+3,layer3+4</param>
                        /// <param name="bridge_ports">Specify the interfaces you want to add to your bridge.</param>
                        /// <param name="bridge_vlan_aware">Enable bridge vlan support.</param>
                        /// <param name="cidr">IPv4 CIDR.</param>
                        /// <param name="cidr6">IPv6 CIDR.</param>
                        /// <param name="comments">Comments</param>
                        /// <param name="comments6">Comments</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="gateway">Default gateway address.</param>
                        /// <param name="gateway6">Default ipv6 gateway address.</param>
                        /// <param name="netmask">Network mask.</param>
                        /// <param name="netmask6">Network mask.</param>
                        /// <param name="ovs_bonds">Specify the interfaces used by the bonding device.</param>
                        /// <param name="ovs_bridge">The OVS bridge associated with a OVS port. This is required when you create an OVS port.</param>
                        /// <param name="ovs_options">OVS interface options.</param>
                        /// <param name="ovs_ports">Specify the interfaces you want to add to your bridge.</param>
                        /// <param name="ovs_tag">Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)</param>
                        /// <param name="slaves">Specify the interfaces used by the bonding device.</param>
                        /// <returns></returns>
                        public Result SetRest(string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string cidr = null, string cidr6 = null, string comments = null, string comments6 = null, string delete = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("type", type);
                            parameters.Add("address", address);
                            parameters.Add("address6", address6);
                            parameters.Add("autostart", autostart);
                            parameters.Add("bond_mode", bond_mode);
                            parameters.Add("bond_xmit_hash_policy", bond_xmit_hash_policy);
                            parameters.Add("bridge_ports", bridge_ports);
                            parameters.Add("bridge_vlan_aware", bridge_vlan_aware);
                            parameters.Add("cidr", cidr);
                            parameters.Add("cidr6", cidr6);
                            parameters.Add("comments", comments);
                            parameters.Add("comments6", comments6);
                            parameters.Add("delete", delete);
                            parameters.Add("gateway", gateway);
                            parameters.Add("gateway6", gateway6);
                            parameters.Add("netmask", netmask);
                            parameters.Add("netmask6", netmask6);
                            parameters.Add("ovs_bonds", ovs_bonds);
                            parameters.Add("ovs_bridge", ovs_bridge);
                            parameters.Add("ovs_options", ovs_options);
                            parameters.Add("ovs_ports", ovs_ports);
                            parameters.Add("ovs_tag", ovs_tag);
                            parameters.Add("slaves", slaves);
                            return _client.Set($"/nodes/{_node}/network/{_iface}", parameters);
                        }

                        /// <summary>
                        /// Update network device configuration
                        /// </summary>
                        /// <param name="type">Network interface type
                        ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,unknown</param>
                        /// <param name="address">IP address.</param>
                        /// <param name="address6">IP address.</param>
                        /// <param name="autostart">Automatically start interface on boot.</param>
                        /// <param name="bond_mode">Bonding mode.
                        ///   Enum: balance-rr,active-backup,balance-xor,broadcast,802.3ad,balance-tlb,balance-alb,balance-slb,lacp-balance-slb,lacp-balance-tcp</param>
                        /// <param name="bond_xmit_hash_policy">Selects the transmit hash policy to use for slave selection in balance-xor and 802.3ad modes.
                        ///   Enum: layer2,layer2+3,layer3+4</param>
                        /// <param name="bridge_ports">Specify the interfaces you want to add to your bridge.</param>
                        /// <param name="bridge_vlan_aware">Enable bridge vlan support.</param>
                        /// <param name="cidr">IPv4 CIDR.</param>
                        /// <param name="cidr6">IPv6 CIDR.</param>
                        /// <param name="comments">Comments</param>
                        /// <param name="comments6">Comments</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="gateway">Default gateway address.</param>
                        /// <param name="gateway6">Default ipv6 gateway address.</param>
                        /// <param name="netmask">Network mask.</param>
                        /// <param name="netmask6">Network mask.</param>
                        /// <param name="ovs_bonds">Specify the interfaces used by the bonding device.</param>
                        /// <param name="ovs_bridge">The OVS bridge associated with a OVS port. This is required when you create an OVS port.</param>
                        /// <param name="ovs_options">OVS interface options.</param>
                        /// <param name="ovs_ports">Specify the interfaces you want to add to your bridge.</param>
                        /// <param name="ovs_tag">Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)</param>
                        /// <param name="slaves">Specify the interfaces used by the bonding device.</param>
                        /// <returns></returns>
                        public Result UpdateNetwork(string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string cidr = null, string cidr6 = null, string comments = null, string comments6 = null, string delete = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null) => SetRest(type, address, address6, autostart, bond_mode, bond_xmit_hash_policy, bridge_ports, bridge_vlan_aware, cidr, cidr6, comments, comments6, delete, gateway, gateway6, netmask, netmask6, ovs_bonds, ovs_bridge, ovs_options, ovs_ports, ovs_tag, slaves);
                    }
                    /// <summary>
                    /// Revert network configuration changes.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/network"); }

                    /// <summary>
                    /// Revert network configuration changes.
                    /// </summary>
                    /// <returns></returns>
                    public Result RevertNetworkChanges() => DeleteRest();
                    /// <summary>
                    /// List available networks
                    /// </summary>
                    /// <param name="type">Only list specific interface types.
                    ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,any_bridge</param>
                    /// <returns></returns>
                    public Result GetRest(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return _client.Get($"/nodes/{_node}/network", parameters);
                    }

                    /// <summary>
                    /// List available networks
                    /// </summary>
                    /// <param name="type">Only list specific interface types.
                    ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,any_bridge</param>
                    /// <returns></returns>
                    public Result Index(string type = null) => GetRest(type);
                    /// <summary>
                    /// Create network device configuration
                    /// </summary>
                    /// <param name="iface">Network interface name.</param>
                    /// <param name="type">Network interface type
                    ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,unknown</param>
                    /// <param name="address">IP address.</param>
                    /// <param name="address6">IP address.</param>
                    /// <param name="autostart">Automatically start interface on boot.</param>
                    /// <param name="bond_mode">Bonding mode.
                    ///   Enum: balance-rr,active-backup,balance-xor,broadcast,802.3ad,balance-tlb,balance-alb,balance-slb,lacp-balance-slb,lacp-balance-tcp</param>
                    /// <param name="bond_xmit_hash_policy">Selects the transmit hash policy to use for slave selection in balance-xor and 802.3ad modes.
                    ///   Enum: layer2,layer2+3,layer3+4</param>
                    /// <param name="bridge_ports">Specify the interfaces you want to add to your bridge.</param>
                    /// <param name="bridge_vlan_aware">Enable bridge vlan support.</param>
                    /// <param name="cidr">IPv4 CIDR.</param>
                    /// <param name="cidr6">IPv6 CIDR.</param>
                    /// <param name="comments">Comments</param>
                    /// <param name="comments6">Comments</param>
                    /// <param name="gateway">Default gateway address.</param>
                    /// <param name="gateway6">Default ipv6 gateway address.</param>
                    /// <param name="netmask">Network mask.</param>
                    /// <param name="netmask6">Network mask.</param>
                    /// <param name="ovs_bonds">Specify the interfaces used by the bonding device.</param>
                    /// <param name="ovs_bridge">The OVS bridge associated with a OVS port. This is required when you create an OVS port.</param>
                    /// <param name="ovs_options">OVS interface options.</param>
                    /// <param name="ovs_ports">Specify the interfaces you want to add to your bridge.</param>
                    /// <param name="ovs_tag">Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)</param>
                    /// <param name="slaves">Specify the interfaces used by the bonding device.</param>
                    /// <returns></returns>
                    public Result CreateRest(string iface, string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string cidr = null, string cidr6 = null, string comments = null, string comments6 = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("iface", iface);
                        parameters.Add("type", type);
                        parameters.Add("address", address);
                        parameters.Add("address6", address6);
                        parameters.Add("autostart", autostart);
                        parameters.Add("bond_mode", bond_mode);
                        parameters.Add("bond_xmit_hash_policy", bond_xmit_hash_policy);
                        parameters.Add("bridge_ports", bridge_ports);
                        parameters.Add("bridge_vlan_aware", bridge_vlan_aware);
                        parameters.Add("cidr", cidr);
                        parameters.Add("cidr6", cidr6);
                        parameters.Add("comments", comments);
                        parameters.Add("comments6", comments6);
                        parameters.Add("gateway", gateway);
                        parameters.Add("gateway6", gateway6);
                        parameters.Add("netmask", netmask);
                        parameters.Add("netmask6", netmask6);
                        parameters.Add("ovs_bonds", ovs_bonds);
                        parameters.Add("ovs_bridge", ovs_bridge);
                        parameters.Add("ovs_options", ovs_options);
                        parameters.Add("ovs_ports", ovs_ports);
                        parameters.Add("ovs_tag", ovs_tag);
                        parameters.Add("slaves", slaves);
                        return _client.Create($"/nodes/{_node}/network", parameters);
                    }

                    /// <summary>
                    /// Create network device configuration
                    /// </summary>
                    /// <param name="iface">Network interface name.</param>
                    /// <param name="type">Network interface type
                    ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,unknown</param>
                    /// <param name="address">IP address.</param>
                    /// <param name="address6">IP address.</param>
                    /// <param name="autostart">Automatically start interface on boot.</param>
                    /// <param name="bond_mode">Bonding mode.
                    ///   Enum: balance-rr,active-backup,balance-xor,broadcast,802.3ad,balance-tlb,balance-alb,balance-slb,lacp-balance-slb,lacp-balance-tcp</param>
                    /// <param name="bond_xmit_hash_policy">Selects the transmit hash policy to use for slave selection in balance-xor and 802.3ad modes.
                    ///   Enum: layer2,layer2+3,layer3+4</param>
                    /// <param name="bridge_ports">Specify the interfaces you want to add to your bridge.</param>
                    /// <param name="bridge_vlan_aware">Enable bridge vlan support.</param>
                    /// <param name="cidr">IPv4 CIDR.</param>
                    /// <param name="cidr6">IPv6 CIDR.</param>
                    /// <param name="comments">Comments</param>
                    /// <param name="comments6">Comments</param>
                    /// <param name="gateway">Default gateway address.</param>
                    /// <param name="gateway6">Default ipv6 gateway address.</param>
                    /// <param name="netmask">Network mask.</param>
                    /// <param name="netmask6">Network mask.</param>
                    /// <param name="ovs_bonds">Specify the interfaces used by the bonding device.</param>
                    /// <param name="ovs_bridge">The OVS bridge associated with a OVS port. This is required when you create an OVS port.</param>
                    /// <param name="ovs_options">OVS interface options.</param>
                    /// <param name="ovs_ports">Specify the interfaces you want to add to your bridge.</param>
                    /// <param name="ovs_tag">Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)</param>
                    /// <param name="slaves">Specify the interfaces used by the bonding device.</param>
                    /// <returns></returns>
                    public Result CreateNetwork(string iface, string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string cidr = null, string cidr6 = null, string comments = null, string comments6 = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null) => CreateRest(iface, type, address, address6, autostart, bond_mode, bond_xmit_hash_policy, bridge_ports, bridge_vlan_aware, cidr, cidr6, comments, comments6, gateway, gateway6, netmask, netmask6, ovs_bonds, ovs_bridge, ovs_options, ovs_ports, ovs_tag, slaves);
                    /// <summary>
                    /// Reload network configuration
                    /// </summary>
                    /// <returns></returns>
                    public Result SetRest() { return _client.Set($"/nodes/{_node}/network"); }

                    /// <summary>
                    /// Reload network configuration
                    /// </summary>
                    /// <returns></returns>
                    public Result ReloadNetworkConfig() => SetRest();
                }
                public class PVETasks
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVETasks(Client client, object node) { _client = client; _node = node; }
                    public PVEItemUpid this[object upid] => new PVEItemUpid(_client, _node, upid);
                    public class PVEItemUpid
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        private readonly object _upid;
                        internal PVEItemUpid(Client client, object node, object upid)
                        {
                            _client = client; _node = node;
                            _upid = upid;
                        }
                        private PVELog _log;
                        public PVELog Log => _log ?? (_log = new PVELog(_client, _node, _upid));
                        private PVEStatus _status;
                        public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node, _upid));
                        public class PVELog
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _upid;
                            internal PVELog(Client client, object node, object upid)
                            {
                                _client = client; _node = node;
                                _upid = upid;
                            }
                            /// <summary>
                            /// Read task log.
                            /// </summary>
                            /// <param name="limit"></param>
                            /// <param name="start"></param>
                            /// <returns></returns>
                            public Result GetRest(int? limit = null, int? start = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("limit", limit);
                                parameters.Add("start", start);
                                return _client.Get($"/nodes/{_node}/tasks/{_upid}/log", parameters);
                            }

                            /// <summary>
                            /// Read task log.
                            /// </summary>
                            /// <param name="limit"></param>
                            /// <param name="start"></param>
                            /// <returns></returns>
                            public Result ReadTaskLog(int? limit = null, int? start = null) => GetRest(limit, start);
                        }
                        public class PVEStatus
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _upid;
                            internal PVEStatus(Client client, object node, object upid)
                            {
                                _client = client; _node = node;
                                _upid = upid;
                            }
                            /// <summary>
                            /// Read task status.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/tasks/{_upid}/status"); }

                            /// <summary>
                            /// Read task status.
                            /// </summary>
                            /// <returns></returns>
                            public Result ReadTaskStatus() => GetRest();
                        }
                        /// <summary>
                        /// Stop a task.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/tasks/{_upid}"); }

                        /// <summary>
                        /// Stop a task.
                        /// </summary>
                        /// <returns></returns>
                        public Result StopTask() => DeleteRest();
                        /// <summary>
                        /// 
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/tasks/{_upid}"); }

                        /// <summary>
                        /// 
                        /// </summary>
                        /// <returns></returns>
                        public Result UpidIndex() => GetRest();
                    }
                    /// <summary>
                    /// Read task list for one node (finished tasks).
                    /// </summary>
                    /// <param name="errors"></param>
                    /// <param name="limit">Only list this amount of tasks.</param>
                    /// <param name="source">List archived, active or all tasks.
                    ///   Enum: archive,active,all</param>
                    /// <param name="start">List tasks beginning from this offset.</param>
                    /// <param name="typefilter">Only list tasks of this type (e.g., vzstart, vzdump).</param>
                    /// <param name="userfilter">Only list tasks from this user.</param>
                    /// <param name="vmid">Only list tasks for this VM.</param>
                    /// <returns></returns>
                    public Result GetRest(bool? errors = null, int? limit = null, string source = null, int? start = null, string typefilter = null, string userfilter = null, int? vmid = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("errors", errors);
                        parameters.Add("limit", limit);
                        parameters.Add("source", source);
                        parameters.Add("start", start);
                        parameters.Add("typefilter", typefilter);
                        parameters.Add("userfilter", userfilter);
                        parameters.Add("vmid", vmid);
                        return _client.Get($"/nodes/{_node}/tasks", parameters);
                    }

                    /// <summary>
                    /// Read task list for one node (finished tasks).
                    /// </summary>
                    /// <param name="errors"></param>
                    /// <param name="limit">Only list this amount of tasks.</param>
                    /// <param name="source">List archived, active or all tasks.
                    ///   Enum: archive,active,all</param>
                    /// <param name="start">List tasks beginning from this offset.</param>
                    /// <param name="typefilter">Only list tasks of this type (e.g., vzstart, vzdump).</param>
                    /// <param name="userfilter">Only list tasks from this user.</param>
                    /// <param name="vmid">Only list tasks for this VM.</param>
                    /// <returns></returns>
                    public Result NodeTasks(bool? errors = null, int? limit = null, string source = null, int? start = null, string typefilter = null, string userfilter = null, int? vmid = null) => GetRest(errors, limit, source, start, typefilter, userfilter, vmid);
                }
                public class PVEScan
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEScan(Client client, object node) { _client = client; _node = node; }
                    private PVEZfs _zfs;
                    public PVEZfs Zfs => _zfs ?? (_zfs = new PVEZfs(_client, _node));
                    private PVENfs _nfs;
                    public PVENfs Nfs => _nfs ?? (_nfs = new PVENfs(_client, _node));
                    private PVECifs _cifs;
                    public PVECifs Cifs => _cifs ?? (_cifs = new PVECifs(_client, _node));
                    private PVEGlusterfs _glusterfs;
                    public PVEGlusterfs Glusterfs => _glusterfs ?? (_glusterfs = new PVEGlusterfs(_client, _node));
                    private PVEIscsi _iscsi;
                    public PVEIscsi Iscsi => _iscsi ?? (_iscsi = new PVEIscsi(_client, _node));
                    private PVELvm _lvm;
                    public PVELvm Lvm => _lvm ?? (_lvm = new PVELvm(_client, _node));
                    private PVELvmthin _lvmthin;
                    public PVELvmthin Lvmthin => _lvmthin ?? (_lvmthin = new PVELvmthin(_client, _node));
                    private PVEUsb _usb;
                    public PVEUsb Usb => _usb ?? (_usb = new PVEUsb(_client, _node));
                    public class PVEZfs
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEZfs(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan zfs pool list on local node.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/scan/zfs"); }

                        /// <summary>
                        /// Scan zfs pool list on local node.
                        /// </summary>
                        /// <returns></returns>
                        public Result Zfsscan() => GetRest();
                    }
                    public class PVENfs
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVENfs(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote NFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <returns></returns>
                        public Result GetRest(string server)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("server", server);
                            return _client.Get($"/nodes/{_node}/scan/nfs", parameters);
                        }

                        /// <summary>
                        /// Scan remote NFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <returns></returns>
                        public Result Nfsscan(string server) => GetRest(server);
                    }
                    public class PVECifs
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVECifs(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote CIFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <param name="domain">SMB domain (Workgroup).</param>
                        /// <param name="password">User password.</param>
                        /// <param name="username">User name.</param>
                        /// <returns></returns>
                        public Result GetRest(string server, string domain = null, string password = null, string username = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("server", server);
                            parameters.Add("domain", domain);
                            parameters.Add("password", password);
                            parameters.Add("username", username);
                            return _client.Get($"/nodes/{_node}/scan/cifs", parameters);
                        }

                        /// <summary>
                        /// Scan remote CIFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <param name="domain">SMB domain (Workgroup).</param>
                        /// <param name="password">User password.</param>
                        /// <param name="username">User name.</param>
                        /// <returns></returns>
                        public Result Cifsscan(string server, string domain = null, string password = null, string username = null) => GetRest(server, domain, password, username);
                    }
                    public class PVEGlusterfs
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEGlusterfs(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote GlusterFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <returns></returns>
                        public Result GetRest(string server)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("server", server);
                            return _client.Get($"/nodes/{_node}/scan/glusterfs", parameters);
                        }

                        /// <summary>
                        /// Scan remote GlusterFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <returns></returns>
                        public Result Glusterfsscan(string server) => GetRest(server);
                    }
                    public class PVEIscsi
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEIscsi(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote iSCSI server.
                        /// </summary>
                        /// <param name="portal">The iSCSI portal (IP or DNS name with optional port).</param>
                        /// <returns></returns>
                        public Result GetRest(string portal)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("portal", portal);
                            return _client.Get($"/nodes/{_node}/scan/iscsi", parameters);
                        }

                        /// <summary>
                        /// Scan remote iSCSI server.
                        /// </summary>
                        /// <param name="portal">The iSCSI portal (IP or DNS name with optional port).</param>
                        /// <returns></returns>
                        public Result Iscsiscan(string portal) => GetRest(portal);
                    }
                    public class PVELvm
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVELvm(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local LVM volume groups.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/scan/lvm"); }

                        /// <summary>
                        /// List local LVM volume groups.
                        /// </summary>
                        /// <returns></returns>
                        public Result Lvmscan() => GetRest();
                    }
                    public class PVELvmthin
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVELvmthin(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local LVM Thin Pools.
                        /// </summary>
                        /// <param name="vg"></param>
                        /// <returns></returns>
                        public Result GetRest(string vg)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("vg", vg);
                            return _client.Get($"/nodes/{_node}/scan/lvmthin", parameters);
                        }

                        /// <summary>
                        /// List local LVM Thin Pools.
                        /// </summary>
                        /// <param name="vg"></param>
                        /// <returns></returns>
                        public Result Lvmthinscan(string vg) => GetRest(vg);
                    }
                    public class PVEUsb
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEUsb(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local USB devices.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/scan/usb"); }

                        /// <summary>
                        /// List local USB devices.
                        /// </summary>
                        /// <returns></returns>
                        public Result Usbscan() => GetRest();
                    }
                    /// <summary>
                    /// Index of available scan methods
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/scan"); }

                    /// <summary>
                    /// Index of available scan methods
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                public class PVEHardware
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEHardware(Client client, object node) { _client = client; _node = node; }
                    private PVEPci _pci;
                    public PVEPci Pci => _pci ?? (_pci = new PVEPci(_client, _node));
                    public class PVEPci
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEPci(Client client, object node) { _client = client; _node = node; }
                        public PVEItemPciid this[object pciid] => new PVEItemPciid(_client, _node, pciid);
                        public class PVEItemPciid
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _pciid;
                            internal PVEItemPciid(Client client, object node, object pciid)
                            {
                                _client = client; _node = node;
                                _pciid = pciid;
                            }
                            private PVEMdev _mdev;
                            public PVEMdev Mdev => _mdev ?? (_mdev = new PVEMdev(_client, _node, _pciid));
                            public class PVEMdev
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _pciid;
                                internal PVEMdev(Client client, object node, object pciid)
                                {
                                    _client = client; _node = node;
                                    _pciid = pciid;
                                }
                                /// <summary>
                                /// List mediated device types for given PCI device.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/hardware/pci/{_pciid}/mdev"); }

                                /// <summary>
                                /// List mediated device types for given PCI device.
                                /// </summary>
                                /// <returns></returns>
                                public Result Mdevscan() => GetRest();
                            }
                            /// <summary>
                            /// Index of available pci methods
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/hardware/pci/{_pciid}"); }

                            /// <summary>
                            /// Index of available pci methods
                            /// </summary>
                            /// <returns></returns>
                            public Result Pciindex() => GetRest();
                        }
                        /// <summary>
                        /// List local PCI devices.
                        /// </summary>
                        /// <param name="pci_class_blacklist">A list of blacklisted PCI classes, which will not be returned. Following are filtered by default: Memory Controller (05), Bridge (06), Generic System Peripheral (08) and Processor (0b).</param>
                        /// <param name="verbose">If disabled, does only print the PCI IDs. Otherwise, additional information like vendor and device will be returned.</param>
                        /// <returns></returns>
                        public Result GetRest(string pci_class_blacklist = null, bool? verbose = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("pci-class-blacklist", pci_class_blacklist);
                            parameters.Add("verbose", verbose);
                            return _client.Get($"/nodes/{_node}/hardware/pci", parameters);
                        }

                        /// <summary>
                        /// List local PCI devices.
                        /// </summary>
                        /// <param name="pci_class_blacklist">A list of blacklisted PCI classes, which will not be returned. Following are filtered by default: Memory Controller (05), Bridge (06), Generic System Peripheral (08) and Processor (0b).</param>
                        /// <param name="verbose">If disabled, does only print the PCI IDs. Otherwise, additional information like vendor and device will be returned.</param>
                        /// <returns></returns>
                        public Result Pciscan(string pci_class_blacklist = null, bool? verbose = null) => GetRest(pci_class_blacklist, verbose);
                    }
                    /// <summary>
                    /// Index of hardware types
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/hardware"); }

                    /// <summary>
                    /// Index of hardware types
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                public class PVEStorage
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEStorage(Client client, object node) { _client = client; _node = node; }
                    public PVEItemStorage this[object storage] => new PVEItemStorage(_client, _node, storage);
                    public class PVEItemStorage
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        private readonly object _storage;
                        internal PVEItemStorage(Client client, object node, object storage)
                        {
                            _client = client; _node = node;
                            _storage = storage;
                        }
                        private PVEContent _content;
                        public PVEContent Content => _content ?? (_content = new PVEContent(_client, _node, _storage));
                        private PVEStatus _status;
                        public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node, _storage));
                        private PVERrd _rrd;
                        public PVERrd Rrd => _rrd ?? (_rrd = new PVERrd(_client, _node, _storage));
                        private PVERrddata _rrddata;
                        public PVERrddata Rrddata => _rrddata ?? (_rrddata = new PVERrddata(_client, _node, _storage));
                        private PVEUpload _upload;
                        public PVEUpload Upload => _upload ?? (_upload = new PVEUpload(_client, _node, _storage));
                        public class PVEContent
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PVEContent(Client client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            public PVEItemVolume this[object volume] => new PVEItemVolume(_client, _node, _storage, volume);
                            public class PVEItemVolume
                            {
                                private readonly Client _client;
                                private readonly object _node;
                                private readonly object _storage;
                                private readonly object _volume;
                                internal PVEItemVolume(Client client, object node, object storage, object volume)
                                {
                                    _client = client; _node = node;
                                    _storage = storage;
                                    _volume = volume;
                                }
                                /// <summary>
                                /// Delete volume
                                /// </summary>
                                /// <returns></returns>
                                public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/storage/{_storage}/content/{_volume}"); }

                                /// <summary>
                                /// Delete volume
                                /// </summary>
                                /// <returns></returns>
                                public Result Delete() => DeleteRest();
                                /// <summary>
                                /// Get volume attributes
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest() { return _client.Get($"/nodes/{_node}/storage/{_storage}/content/{_volume}"); }

                                /// <summary>
                                /// Get volume attributes
                                /// </summary>
                                /// <returns></returns>
                                public Result Info() => GetRest();
                                /// <summary>
                                /// Copy a volume. This is experimental code - do not use.
                                /// </summary>
                                /// <param name="target">Target volume identifier</param>
                                /// <param name="target_node">Target node. Default is local node.</param>
                                /// <returns></returns>
                                public Result CreateRest(string target, string target_node = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("target", target);
                                    parameters.Add("target_node", target_node);
                                    return _client.Create($"/nodes/{_node}/storage/{_storage}/content/{_volume}", parameters);
                                }

                                /// <summary>
                                /// Copy a volume. This is experimental code - do not use.
                                /// </summary>
                                /// <param name="target">Target volume identifier</param>
                                /// <param name="target_node">Target node. Default is local node.</param>
                                /// <returns></returns>
                                public Result Copy(string target, string target_node = null) => CreateRest(target, target_node);
                            }
                            /// <summary>
                            /// List storage content.
                            /// </summary>
                            /// <param name="content">Only list content of this type.</param>
                            /// <param name="vmid">Only list images for this VM</param>
                            /// <returns></returns>
                            public Result GetRest(string content = null, int? vmid = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("content", content);
                                parameters.Add("vmid", vmid);
                                return _client.Get($"/nodes/{_node}/storage/{_storage}/content", parameters);
                            }

                            /// <summary>
                            /// List storage content.
                            /// </summary>
                            /// <param name="content">Only list content of this type.</param>
                            /// <param name="vmid">Only list images for this VM</param>
                            /// <returns></returns>
                            public Result Index(string content = null, int? vmid = null) => GetRest(content, vmid);
                            /// <summary>
                            /// Allocate disk images.
                            /// </summary>
                            /// <param name="filename">The name of the file to create.</param>
                            /// <param name="size">Size in kilobyte (1024 bytes). Optional suffixes 'M' (megabyte, 1024K) and 'G' (gigabyte, 1024M)</param>
                            /// <param name="vmid">Specify owner VM</param>
                            /// <param name="format">
                            ///   Enum: raw,qcow2,subvol</param>
                            /// <returns></returns>
                            public Result CreateRest(string filename, string size, int vmid, string format = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("filename", filename);
                                parameters.Add("size", size);
                                parameters.Add("vmid", vmid);
                                parameters.Add("format", format);
                                return _client.Create($"/nodes/{_node}/storage/{_storage}/content", parameters);
                            }

                            /// <summary>
                            /// Allocate disk images.
                            /// </summary>
                            /// <param name="filename">The name of the file to create.</param>
                            /// <param name="size">Size in kilobyte (1024 bytes). Optional suffixes 'M' (megabyte, 1024K) and 'G' (gigabyte, 1024M)</param>
                            /// <param name="vmid">Specify owner VM</param>
                            /// <param name="format">
                            ///   Enum: raw,qcow2,subvol</param>
                            /// <returns></returns>
                            public Result Create(string filename, string size, int vmid, string format = null) => CreateRest(filename, size, vmid, format);
                        }
                        public class PVEStatus
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PVEStatus(Client client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// Read storage status.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/storage/{_storage}/status"); }

                            /// <summary>
                            /// Read storage status.
                            /// </summary>
                            /// <returns></returns>
                            public Result ReadStatus() => GetRest();
                        }
                        public class PVERrd
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PVERrd(Client client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// Read storage RRD statistics (returns PNG).
                            /// </summary>
                            /// <param name="ds">The list of datasources you want to display.</param>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result GetRest(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return _client.Get($"/nodes/{_node}/storage/{_storage}/rrd", parameters);
                            }

                            /// <summary>
                            /// Read storage RRD statistics (returns PNG).
                            /// </summary>
                            /// <param name="ds">The list of datasources you want to display.</param>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result Rrd(string ds, string timeframe, string cf = null) => GetRest(ds, timeframe, cf);
                        }
                        public class PVERrddata
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PVERrddata(Client client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// Read storage RRD statistics.
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result GetRest(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return _client.Get($"/nodes/{_node}/storage/{_storage}/rrddata", parameters);
                            }

                            /// <summary>
                            /// Read storage RRD statistics.
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            /// <returns></returns>
                            public Result Rrddata(string timeframe, string cf = null) => GetRest(timeframe, cf);
                        }
                        public class PVEUpload
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PVEUpload(Client client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// Upload templates and ISO images.
                            /// </summary>
                            /// <param name="content">Content type.</param>
                            /// <param name="filename">The name of the file to create.</param>
                            /// <param name="tmpfilename">The source file name. This parameter is usually set by the REST handler. You can only overwrite it when connecting to the trusted port on localhost.</param>
                            /// <returns></returns>
                            public Result CreateRest(string content, string filename, string tmpfilename = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("content", content);
                                parameters.Add("filename", filename);
                                parameters.Add("tmpfilename", tmpfilename);
                                return _client.Create($"/nodes/{_node}/storage/{_storage}/upload", parameters);
                            }

                            /// <summary>
                            /// Upload templates and ISO images.
                            /// </summary>
                            /// <param name="content">Content type.</param>
                            /// <param name="filename">The name of the file to create.</param>
                            /// <param name="tmpfilename">The source file name. This parameter is usually set by the REST handler. You can only overwrite it when connecting to the trusted port on localhost.</param>
                            /// <returns></returns>
                            public Result Upload(string content, string filename, string tmpfilename = null) => CreateRest(content, filename, tmpfilename);
                        }
                        /// <summary>
                        /// 
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/storage/{_storage}"); }

                        /// <summary>
                        /// 
                        /// </summary>
                        /// <returns></returns>
                        public Result Diridx() => GetRest();
                    }
                    /// <summary>
                    /// Get status for all datastores.
                    /// </summary>
                    /// <param name="content">Only list stores which support this content type.</param>
                    /// <param name="enabled">Only list stores which are enabled (not disabled in config).</param>
                    /// <param name="format">Include information about formats</param>
                    /// <param name="storage">Only list status for  specified storage</param>
                    /// <param name="target">If target is different to 'node', we only lists shared storages which content is accessible on this 'node' and the specified 'target' node.</param>
                    /// <returns></returns>
                    public Result GetRest(string content = null, bool? enabled = null, bool? format = null, string storage = null, string target = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("content", content);
                        parameters.Add("enabled", enabled);
                        parameters.Add("format", format);
                        parameters.Add("storage", storage);
                        parameters.Add("target", target);
                        return _client.Get($"/nodes/{_node}/storage", parameters);
                    }

                    /// <summary>
                    /// Get status for all datastores.
                    /// </summary>
                    /// <param name="content">Only list stores which support this content type.</param>
                    /// <param name="enabled">Only list stores which are enabled (not disabled in config).</param>
                    /// <param name="format">Include information about formats</param>
                    /// <param name="storage">Only list status for  specified storage</param>
                    /// <param name="target">If target is different to 'node', we only lists shared storages which content is accessible on this 'node' and the specified 'target' node.</param>
                    /// <returns></returns>
                    public Result Index(string content = null, bool? enabled = null, bool? format = null, string storage = null, string target = null) => GetRest(content, enabled, format, storage, target);
                }
                public class PVEDisks
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEDisks(Client client, object node) { _client = client; _node = node; }
                    private PVELvm _lvm;
                    public PVELvm Lvm => _lvm ?? (_lvm = new PVELvm(_client, _node));
                    private PVELvmthin _lvmthin;
                    public PVELvmthin Lvmthin => _lvmthin ?? (_lvmthin = new PVELvmthin(_client, _node));
                    private PVEDirectory _directory;
                    public PVEDirectory Directory => _directory ?? (_directory = new PVEDirectory(_client, _node));
                    private PVEZfs _zfs;
                    public PVEZfs Zfs => _zfs ?? (_zfs = new PVEZfs(_client, _node));
                    private PVEList _list;
                    public PVEList List => _list ?? (_list = new PVEList(_client, _node));
                    private PVESmart _smart;
                    public PVESmart Smart => _smart ?? (_smart = new PVESmart(_client, _node));
                    private PVEInitgpt _initgpt;
                    public PVEInitgpt Initgpt => _initgpt ?? (_initgpt = new PVEInitgpt(_client, _node));
                    public class PVELvm
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVELvm(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List LVM Volume Groups
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/disks/lvm"); }

                        /// <summary>
                        /// List LVM Volume Groups
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                        /// <summary>
                        /// Create an LVM Volume Group
                        /// </summary>
                        /// <param name="device">The block device you want to create the volume group on</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the Volume Group</param>
                        /// <returns></returns>
                        public Result CreateRest(string device, string name, bool? add_storage = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("device", device);
                            parameters.Add("name", name);
                            parameters.Add("add_storage", add_storage);
                            return _client.Create($"/nodes/{_node}/disks/lvm", parameters);
                        }

                        /// <summary>
                        /// Create an LVM Volume Group
                        /// </summary>
                        /// <param name="device">The block device you want to create the volume group on</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the Volume Group</param>
                        /// <returns></returns>
                        public Result Create(string device, string name, bool? add_storage = null) => CreateRest(device, name, add_storage);
                    }
                    public class PVELvmthin
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVELvmthin(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List LVM thinpools
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/disks/lvmthin"); }

                        /// <summary>
                        /// List LVM thinpools
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                        /// <summary>
                        /// Create an LVM thinpool
                        /// </summary>
                        /// <param name="device">The block device you want to create the thinpool on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the thinpool.</param>
                        /// <returns></returns>
                        public Result CreateRest(string device, string name, bool? add_storage = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("device", device);
                            parameters.Add("name", name);
                            parameters.Add("add_storage", add_storage);
                            return _client.Create($"/nodes/{_node}/disks/lvmthin", parameters);
                        }

                        /// <summary>
                        /// Create an LVM thinpool
                        /// </summary>
                        /// <param name="device">The block device you want to create the thinpool on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the thinpool.</param>
                        /// <returns></returns>
                        public Result Create(string device, string name, bool? add_storage = null) => CreateRest(device, name, add_storage);
                    }
                    public class PVEDirectory
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEDirectory(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// PVE Managed Directory storages.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/disks/directory"); }

                        /// <summary>
                        /// PVE Managed Directory storages.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                        /// <summary>
                        /// Create a Filesystem on an unused disk. Will be mounted under '/mnt/pve/NAME'.
                        /// </summary>
                        /// <param name="device">The block device you want to create the filesystem on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the directory.</param>
                        /// <param name="filesystem">The desired filesystem.
                        ///   Enum: ext4,xfs</param>
                        /// <returns></returns>
                        public Result CreateRest(string device, string name, bool? add_storage = null, string filesystem = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("device", device);
                            parameters.Add("name", name);
                            parameters.Add("add_storage", add_storage);
                            parameters.Add("filesystem", filesystem);
                            return _client.Create($"/nodes/{_node}/disks/directory", parameters);
                        }

                        /// <summary>
                        /// Create a Filesystem on an unused disk. Will be mounted under '/mnt/pve/NAME'.
                        /// </summary>
                        /// <param name="device">The block device you want to create the filesystem on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the directory.</param>
                        /// <param name="filesystem">The desired filesystem.
                        ///   Enum: ext4,xfs</param>
                        /// <returns></returns>
                        public Result Create(string device, string name, bool? add_storage = null, string filesystem = null) => CreateRest(device, name, add_storage, filesystem);
                    }
                    public class PVEZfs
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEZfs(Client client, object node) { _client = client; _node = node; }
                        public PVEItemName this[object name] => new PVEItemName(_client, _node, name);
                        public class PVEItemName
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PVEItemName(Client client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Get details about a zpool.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/disks/zfs/{_name}"); }

                            /// <summary>
                            /// Get details about a zpool.
                            /// </summary>
                            /// <returns></returns>
                            public Result Detail() => GetRest();
                        }
                        /// <summary>
                        /// List Zpools.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/disks/zfs"); }

                        /// <summary>
                        /// List Zpools.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                        /// <summary>
                        /// Create a ZFS pool.
                        /// </summary>
                        /// <param name="devices">The block devices you want to create the zpool on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="raidlevel">The RAID level to use.
                        ///   Enum: single,mirror,raid10,raidz,raidz2,raidz3</param>
                        /// <param name="add_storage">Configure storage using the zpool.</param>
                        /// <param name="ashift">Pool sector size exponent.</param>
                        /// <param name="compression">The compression algorithm to use.
                        ///   Enum: on,off,gzip,lz4,lzjb,zle</param>
                        /// <returns></returns>
                        public Result CreateRest(string devices, string name, string raidlevel, bool? add_storage = null, int? ashift = null, string compression = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("devices", devices);
                            parameters.Add("name", name);
                            parameters.Add("raidlevel", raidlevel);
                            parameters.Add("add_storage", add_storage);
                            parameters.Add("ashift", ashift);
                            parameters.Add("compression", compression);
                            return _client.Create($"/nodes/{_node}/disks/zfs", parameters);
                        }

                        /// <summary>
                        /// Create a ZFS pool.
                        /// </summary>
                        /// <param name="devices">The block devices you want to create the zpool on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="raidlevel">The RAID level to use.
                        ///   Enum: single,mirror,raid10,raidz,raidz2,raidz3</param>
                        /// <param name="add_storage">Configure storage using the zpool.</param>
                        /// <param name="ashift">Pool sector size exponent.</param>
                        /// <param name="compression">The compression algorithm to use.
                        ///   Enum: on,off,gzip,lz4,lzjb,zle</param>
                        /// <returns></returns>
                        public Result Create(string devices, string name, string raidlevel, bool? add_storage = null, int? ashift = null, string compression = null) => CreateRest(devices, name, raidlevel, add_storage, ashift, compression);
                    }
                    public class PVEList
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEList(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        /// <param name="skipsmart">Skip smart checks.</param>
                        /// <param name="type">Only list specific types of disks.
                        ///   Enum: unused,journal_disks</param>
                        /// <returns></returns>
                        public Result GetRest(bool? skipsmart = null, string type = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("skipsmart", skipsmart);
                            parameters.Add("type", type);
                            return _client.Get($"/nodes/{_node}/disks/list", parameters);
                        }

                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        /// <param name="skipsmart">Skip smart checks.</param>
                        /// <param name="type">Only list specific types of disks.
                        ///   Enum: unused,journal_disks</param>
                        /// <returns></returns>
                        public Result List(bool? skipsmart = null, string type = null) => GetRest(skipsmart, type);
                    }
                    public class PVESmart
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVESmart(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get SMART Health of a disk.
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <param name="healthonly">If true returns only the health status</param>
                        /// <returns></returns>
                        public Result GetRest(string disk, bool? healthonly = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("disk", disk);
                            parameters.Add("healthonly", healthonly);
                            return _client.Get($"/nodes/{_node}/disks/smart", parameters);
                        }

                        /// <summary>
                        /// Get SMART Health of a disk.
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <param name="healthonly">If true returns only the health status</param>
                        /// <returns></returns>
                        public Result Smart(string disk, bool? healthonly = null) => GetRest(disk, healthonly);
                    }
                    public class PVEInitgpt
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEInitgpt(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Initialize Disk with GPT
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <param name="uuid">UUID for the GPT table</param>
                        /// <returns></returns>
                        public Result CreateRest(string disk, string uuid = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("disk", disk);
                            parameters.Add("uuid", uuid);
                            return _client.Create($"/nodes/{_node}/disks/initgpt", parameters);
                        }

                        /// <summary>
                        /// Initialize Disk with GPT
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <param name="uuid">UUID for the GPT table</param>
                        /// <returns></returns>
                        public Result Initgpt(string disk, string uuid = null) => CreateRest(disk, uuid);
                    }
                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/disks"); }

                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                public class PVEApt
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEApt(Client client, object node) { _client = client; _node = node; }
                    private PVEUpdate _update;
                    public PVEUpdate Update => _update ?? (_update = new PVEUpdate(_client, _node));
                    private PVEChangelog _changelog;
                    public PVEChangelog Changelog => _changelog ?? (_changelog = new PVEChangelog(_client, _node));
                    private PVEVersions _versions;
                    public PVEVersions Versions => _versions ?? (_versions = new PVEVersions(_client, _node));
                    public class PVEUpdate
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEUpdate(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List available updates.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/apt/update"); }

                        /// <summary>
                        /// List available updates.
                        /// </summary>
                        /// <returns></returns>
                        public Result ListUpdates() => GetRest();
                        /// <summary>
                        /// This is used to resynchronize the package index files from their sources (apt-get update).
                        /// </summary>
                        /// <param name="notify">Send notification mail about new packages (to email address specified for user 'root@pam').</param>
                        /// <param name="quiet">Only produces output suitable for logging, omitting progress indicators.</param>
                        /// <returns></returns>
                        public Result CreateRest(bool? notify = null, bool? quiet = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("notify", notify);
                            parameters.Add("quiet", quiet);
                            return _client.Create($"/nodes/{_node}/apt/update", parameters);
                        }

                        /// <summary>
                        /// This is used to resynchronize the package index files from their sources (apt-get update).
                        /// </summary>
                        /// <param name="notify">Send notification mail about new packages (to email address specified for user 'root@pam').</param>
                        /// <param name="quiet">Only produces output suitable for logging, omitting progress indicators.</param>
                        /// <returns></returns>
                        public Result UpdateDatabase(bool? notify = null, bool? quiet = null) => CreateRest(notify, quiet);
                    }
                    public class PVEChangelog
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEChangelog(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get package changelogs.
                        /// </summary>
                        /// <param name="name">Package name.</param>
                        /// <param name="version">Package version.</param>
                        /// <returns></returns>
                        public Result GetRest(string name, string version = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("name", name);
                            parameters.Add("version", version);
                            return _client.Get($"/nodes/{_node}/apt/changelog", parameters);
                        }

                        /// <summary>
                        /// Get package changelogs.
                        /// </summary>
                        /// <param name="name">Package name.</param>
                        /// <param name="version">Package version.</param>
                        /// <returns></returns>
                        public Result Changelog(string name, string version = null) => GetRest(name, version);
                    }
                    public class PVEVersions
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEVersions(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get package information for important Proxmox packages.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/apt/versions"); }

                        /// <summary>
                        /// Get package information for important Proxmox packages.
                        /// </summary>
                        /// <returns></returns>
                        public Result Versions() => GetRest();
                    }
                    /// <summary>
                    /// Directory index for apt (Advanced Package Tool).
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/apt"); }

                    /// <summary>
                    /// Directory index for apt (Advanced Package Tool).
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                public class PVEFirewall
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEFirewall(Client client, object node) { _client = client; _node = node; }
                    private PVERules _rules;
                    public PVERules Rules => _rules ?? (_rules = new PVERules(_client, _node));
                    private PVEOptions _options;
                    public PVEOptions Options => _options ?? (_options = new PVEOptions(_client, _node));
                    private PVELog _log;
                    public PVELog Log => _log ?? (_log = new PVELog(_client, _node));
                    public class PVERules
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVERules(Client client, object node) { _client = client; _node = node; }
                        public PVEItemPos this[object pos] => new PVEItemPos(_client, _node, pos);
                        public class PVEItemPos
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _pos;
                            internal PVEItemPos(Client client, object node, object pos)
                            {
                                _client = client; _node = node;
                                _pos = pos;
                            }
                            /// <summary>
                            /// Delete rule.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result DeleteRest(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                return _client.Delete($"/nodes/{_node}/firewall/rules/{_pos}", parameters);
                            }

                            /// <summary>
                            /// Delete rule.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result DeleteRule(string digest = null) => DeleteRest(digest);
                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/firewall/rules/{_pos}"); }

                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRule() => GetRest();
                            /// <summary>
                            /// Modify rule data.
                            /// </summary>
                            /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                            /// <param name="comment">Descriptive comment.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="enable">Flag to enable/disable a rule.</param>
                            /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                            /// <param name="log">Log level for firewall rule.
                            ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                            /// <param name="macro">Use predefined standard macro.</param>
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            /// <returns></returns>
                            public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("action", action);
                                parameters.Add("comment", comment);
                                parameters.Add("delete", delete);
                                parameters.Add("dest", dest);
                                parameters.Add("digest", digest);
                                parameters.Add("dport", dport);
                                parameters.Add("enable", enable);
                                parameters.Add("iface", iface);
                                parameters.Add("log", log);
                                parameters.Add("macro", macro);
                                parameters.Add("moveto", moveto);
                                parameters.Add("proto", proto);
                                parameters.Add("source", source);
                                parameters.Add("sport", sport);
                                parameters.Add("type", type);
                                return _client.Set($"/nodes/{_node}/firewall/rules/{_pos}", parameters);
                            }

                            /// <summary>
                            /// Modify rule data.
                            /// </summary>
                            /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                            /// <param name="comment">Descriptive comment.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="enable">Flag to enable/disable a rule.</param>
                            /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                            /// <param name="log">Log level for firewall rule.
                            ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                            /// <param name="macro">Use predefined standard macro.</param>
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            /// <returns></returns>
                            public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) => SetRest(action, comment, delete, dest, digest, dport, enable, iface, log, macro, moveto, proto, source, sport, type);
                        }
                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/firewall/rules"); }

                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRules() => GetRest();
                        /// <summary>
                        /// Create new rule.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                        /// <param name="log">Log level for firewall rule.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("type", type);
                            parameters.Add("comment", comment);
                            parameters.Add("dest", dest);
                            parameters.Add("digest", digest);
                            parameters.Add("dport", dport);
                            parameters.Add("enable", enable);
                            parameters.Add("iface", iface);
                            parameters.Add("log", log);
                            parameters.Add("macro", macro);
                            parameters.Add("pos", pos);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            return _client.Create($"/nodes/{_node}/firewall/rules", parameters);
                        }

                        /// <summary>
                        /// Create new rule.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                        /// <param name="log">Log level for firewall rule.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) => CreateRest(action, type, comment, dest, digest, dport, enable, iface, log, macro, pos, proto, source, sport);
                    }
                    public class PVEOptions
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEOptions(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get host firewall options.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/firewall/options"); }

                        /// <summary>
                        /// Get host firewall options.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetOptions() => GetRest();
                        /// <summary>
                        /// Set Firewall options.
                        /// </summary>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="enable">Enable host firewall rules.</param>
                        /// <param name="log_level_in">Log level for incoming traffic.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="log_level_out">Log level for outgoing traffic.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="log_nf_conntrack">Enable logging of conntrack information.</param>
                        /// <param name="ndp">Enable NDP.</param>
                        /// <param name="nf_conntrack_allow_invalid">Allow invalid packets on connection tracking.</param>
                        /// <param name="nf_conntrack_max">Maximum number of tracked connections.</param>
                        /// <param name="nf_conntrack_tcp_timeout_established">Conntrack established timeout.</param>
                        /// <param name="nosmurfs">Enable SMURFS filter.</param>
                        /// <param name="smurf_log_level">Log level for SMURFS filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcp_flags_log_level">Log level for illegal tcp flags filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcpflags">Filter illegal combinations of TCP flags.</param>
                        /// <returns></returns>
                        public Result SetRest(string delete = null, string digest = null, bool? enable = null, string log_level_in = null, string log_level_out = null, bool? log_nf_conntrack = null, bool? ndp = null, bool? nf_conntrack_allow_invalid = null, int? nf_conntrack_max = null, int? nf_conntrack_tcp_timeout_established = null, bool? nosmurfs = null, string smurf_log_level = null, string tcp_flags_log_level = null, bool? tcpflags = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("enable", enable);
                            parameters.Add("log_level_in", log_level_in);
                            parameters.Add("log_level_out", log_level_out);
                            parameters.Add("log_nf_conntrack", log_nf_conntrack);
                            parameters.Add("ndp", ndp);
                            parameters.Add("nf_conntrack_allow_invalid", nf_conntrack_allow_invalid);
                            parameters.Add("nf_conntrack_max", nf_conntrack_max);
                            parameters.Add("nf_conntrack_tcp_timeout_established", nf_conntrack_tcp_timeout_established);
                            parameters.Add("nosmurfs", nosmurfs);
                            parameters.Add("smurf_log_level", smurf_log_level);
                            parameters.Add("tcp_flags_log_level", tcp_flags_log_level);
                            parameters.Add("tcpflags", tcpflags);
                            return _client.Set($"/nodes/{_node}/firewall/options", parameters);
                        }

                        /// <summary>
                        /// Set Firewall options.
                        /// </summary>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="enable">Enable host firewall rules.</param>
                        /// <param name="log_level_in">Log level for incoming traffic.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="log_level_out">Log level for outgoing traffic.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="log_nf_conntrack">Enable logging of conntrack information.</param>
                        /// <param name="ndp">Enable NDP.</param>
                        /// <param name="nf_conntrack_allow_invalid">Allow invalid packets on connection tracking.</param>
                        /// <param name="nf_conntrack_max">Maximum number of tracked connections.</param>
                        /// <param name="nf_conntrack_tcp_timeout_established">Conntrack established timeout.</param>
                        /// <param name="nosmurfs">Enable SMURFS filter.</param>
                        /// <param name="smurf_log_level">Log level for SMURFS filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcp_flags_log_level">Log level for illegal tcp flags filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcpflags">Filter illegal combinations of TCP flags.</param>
                        /// <returns></returns>
                        public Result SetOptions(string delete = null, string digest = null, bool? enable = null, string log_level_in = null, string log_level_out = null, bool? log_nf_conntrack = null, bool? ndp = null, bool? nf_conntrack_allow_invalid = null, int? nf_conntrack_max = null, int? nf_conntrack_tcp_timeout_established = null, bool? nosmurfs = null, string smurf_log_level = null, string tcp_flags_log_level = null, bool? tcpflags = null) => SetRest(delete, digest, enable, log_level_in, log_level_out, log_nf_conntrack, ndp, nf_conntrack_allow_invalid, nf_conntrack_max, nf_conntrack_tcp_timeout_established, nosmurfs, smurf_log_level, tcp_flags_log_level, tcpflags);
                    }
                    public class PVELog
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVELog(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Read firewall log
                        /// </summary>
                        /// <param name="limit"></param>
                        /// <param name="start"></param>
                        /// <returns></returns>
                        public Result GetRest(int? limit = null, int? start = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("limit", limit);
                            parameters.Add("start", start);
                            return _client.Get($"/nodes/{_node}/firewall/log", parameters);
                        }

                        /// <summary>
                        /// Read firewall log
                        /// </summary>
                        /// <param name="limit"></param>
                        /// <param name="start"></param>
                        /// <returns></returns>
                        public Result Log(int? limit = null, int? start = null) => GetRest(limit, start);
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/firewall"); }

                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                public class PVEReplication
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEReplication(Client client, object node) { _client = client; _node = node; }
                    public PVEItemId this[object id] => new PVEItemId(_client, _node, id);
                    public class PVEItemId
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        private readonly object _id;
                        internal PVEItemId(Client client, object node, object id)
                        {
                            _client = client; _node = node;
                            _id = id;
                        }
                        private PVEStatus _status;
                        public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node, _id));
                        private PVELog _log;
                        public PVELog Log => _log ?? (_log = new PVELog(_client, _node, _id));
                        private PVEScheduleNow _scheduleNow;
                        public PVEScheduleNow ScheduleNow => _scheduleNow ?? (_scheduleNow = new PVEScheduleNow(_client, _node, _id));
                        public class PVEStatus
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _id;
                            internal PVEStatus(Client client, object node, object id)
                            {
                                _client = client; _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Get replication job status.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest() { return _client.Get($"/nodes/{_node}/replication/{_id}/status"); }

                            /// <summary>
                            /// Get replication job status.
                            /// </summary>
                            /// <returns></returns>
                            public Result JobStatus() => GetRest();
                        }
                        public class PVELog
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _id;
                            internal PVELog(Client client, object node, object id)
                            {
                                _client = client; _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Read replication job log.
                            /// </summary>
                            /// <param name="limit"></param>
                            /// <param name="start"></param>
                            /// <returns></returns>
                            public Result GetRest(int? limit = null, int? start = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("limit", limit);
                                parameters.Add("start", start);
                                return _client.Get($"/nodes/{_node}/replication/{_id}/log", parameters);
                            }

                            /// <summary>
                            /// Read replication job log.
                            /// </summary>
                            /// <param name="limit"></param>
                            /// <param name="start"></param>
                            /// <returns></returns>
                            public Result ReadJobLog(int? limit = null, int? start = null) => GetRest(limit, start);
                        }
                        public class PVEScheduleNow
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            private readonly object _id;
                            internal PVEScheduleNow(Client client, object node, object id)
                            {
                                _client = client; _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Schedule replication job to start as soon as possible.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest() { return _client.Create($"/nodes/{_node}/replication/{_id}/schedule_now"); }

                            /// <summary>
                            /// Schedule replication job to start as soon as possible.
                            /// </summary>
                            /// <returns></returns>
                            public Result ScheduleNow() => CreateRest();
                        }
                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/replication/{_id}"); }

                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                    }
                    /// <summary>
                    /// List status of all replication jobs on this node.
                    /// </summary>
                    /// <param name="guest">Only list replication jobs for this guest.</param>
                    /// <returns></returns>
                    public Result GetRest(int? guest = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("guest", guest);
                        return _client.Get($"/nodes/{_node}/replication", parameters);
                    }

                    /// <summary>
                    /// List status of all replication jobs on this node.
                    /// </summary>
                    /// <param name="guest">Only list replication jobs for this guest.</param>
                    /// <returns></returns>
                    public Result Status(int? guest = null) => GetRest(guest);
                }
                public class PVECertificates
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVECertificates(Client client, object node) { _client = client; _node = node; }
                    private PVEAcme _acme;
                    public PVEAcme Acme => _acme ?? (_acme = new PVEAcme(_client, _node));
                    private PVEInfo _info;
                    public PVEInfo Info => _info ?? (_info = new PVEInfo(_client, _node));
                    private PVECustom _custom;
                    public PVECustom Custom => _custom ?? (_custom = new PVECustom(_client, _node));
                    public class PVEAcme
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEAcme(Client client, object node) { _client = client; _node = node; }
                        private PVECertificate _certificate;
                        public PVECertificate Certificate => _certificate ?? (_certificate = new PVECertificate(_client, _node));
                        public class PVECertificate
                        {
                            private readonly Client _client;
                            private readonly object _node;
                            internal PVECertificate(Client client, object node) { _client = client; _node = node; }
                            /// <summary>
                            /// Revoke existing certificate from CA.
                            /// </summary>
                            /// <returns></returns>
                            public Result DeleteRest() { return _client.Delete($"/nodes/{_node}/certificates/acme/certificate"); }

                            /// <summary>
                            /// Revoke existing certificate from CA.
                            /// </summary>
                            /// <returns></returns>
                            public Result RevokeCertificate() => DeleteRest();
                            /// <summary>
                            /// Order a new certificate from ACME-compatible CA.
                            /// </summary>
                            /// <param name="force">Overwrite existing custom certificate.</param>
                            /// <returns></returns>
                            public Result CreateRest(bool? force = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                return _client.Create($"/nodes/{_node}/certificates/acme/certificate", parameters);
                            }

                            /// <summary>
                            /// Order a new certificate from ACME-compatible CA.
                            /// </summary>
                            /// <param name="force">Overwrite existing custom certificate.</param>
                            /// <returns></returns>
                            public Result NewCertificate(bool? force = null) => CreateRest(force);
                            /// <summary>
                            /// Renew existing certificate from CA.
                            /// </summary>
                            /// <param name="force">Force renewal even if expiry is more than 30 days away.</param>
                            /// <returns></returns>
                            public Result SetRest(bool? force = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                return _client.Set($"/nodes/{_node}/certificates/acme/certificate", parameters);
                            }

                            /// <summary>
                            /// Renew existing certificate from CA.
                            /// </summary>
                            /// <param name="force">Force renewal even if expiry is more than 30 days away.</param>
                            /// <returns></returns>
                            public Result RenewCertificate(bool? force = null) => SetRest(force);
                        }
                        /// <summary>
                        /// ACME index.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/certificates/acme"); }

                        /// <summary>
                        /// ACME index.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() => GetRest();
                    }
                    public class PVEInfo
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVEInfo(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get information about node's certificates.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/nodes/{_node}/certificates/info"); }

                        /// <summary>
                        /// Get information about node's certificates.
                        /// </summary>
                        /// <returns></returns>
                        public Result Info() => GetRest();
                    }
                    public class PVECustom
                    {
                        private readonly Client _client;
                        private readonly object _node;
                        internal PVECustom(Client client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// DELETE custom certificate chain and key.
                        /// </summary>
                        /// <param name="restart">Restart pveproxy.</param>
                        /// <returns></returns>
                        public Result DeleteRest(bool? restart = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("restart", restart);
                            return _client.Delete($"/nodes/{_node}/certificates/custom", parameters);
                        }

                        /// <summary>
                        /// DELETE custom certificate chain and key.
                        /// </summary>
                        /// <param name="restart">Restart pveproxy.</param>
                        /// <returns></returns>
                        public Result RemoveCustomCert(bool? restart = null) => DeleteRest(restart);
                        /// <summary>
                        /// Upload or update custom certificate chain and key.
                        /// </summary>
                        /// <param name="certificates">PEM encoded certificate (chain).</param>
                        /// <param name="force">Overwrite existing custom or ACME certificate files.</param>
                        /// <param name="key">PEM encoded private key.</param>
                        /// <param name="restart">Restart pveproxy.</param>
                        /// <returns></returns>
                        public Result CreateRest(string certificates, bool? force = null, string key = null, bool? restart = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("certificates", certificates);
                            parameters.Add("force", force);
                            parameters.Add("key", key);
                            parameters.Add("restart", restart);
                            return _client.Create($"/nodes/{_node}/certificates/custom", parameters);
                        }

                        /// <summary>
                        /// Upload or update custom certificate chain and key.
                        /// </summary>
                        /// <param name="certificates">PEM encoded certificate (chain).</param>
                        /// <param name="force">Overwrite existing custom or ACME certificate files.</param>
                        /// <param name="key">PEM encoded private key.</param>
                        /// <param name="restart">Restart pveproxy.</param>
                        /// <returns></returns>
                        public Result UploadCustomCert(string certificates, bool? force = null, string key = null, bool? restart = null) => CreateRest(certificates, force, key, restart);
                    }
                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/certificates"); }

                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() => GetRest();
                }
                public class PVEConfig
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEConfig(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Get node configuration options.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/config"); }

                    /// <summary>
                    /// Get node configuration options.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetConfig() => GetRest();
                    /// <summary>
                    /// Set node configuration options.
                    /// </summary>
                    /// <param name="acme">Node specific ACME settings.</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="description">Node description/comment.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="wakeonlan">MAC address for wake on LAN</param>
                    /// <returns></returns>
                    public Result SetRest(string acme = null, string delete = null, string description = null, string digest = null, string wakeonlan = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("acme", acme);
                        parameters.Add("delete", delete);
                        parameters.Add("description", description);
                        parameters.Add("digest", digest);
                        parameters.Add("wakeonlan", wakeonlan);
                        return _client.Set($"/nodes/{_node}/config", parameters);
                    }

                    /// <summary>
                    /// Set node configuration options.
                    /// </summary>
                    /// <param name="acme">Node specific ACME settings.</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="description">Node description/comment.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="wakeonlan">MAC address for wake on LAN</param>
                    /// <returns></returns>
                    public Result SetOptions(string acme = null, string delete = null, string description = null, string digest = null, string wakeonlan = null) => SetRest(acme, delete, description, digest, wakeonlan);
                }
                public class PVEVersion
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEVersion(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// API version details
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/version"); }

                    /// <summary>
                    /// API version details
                    /// </summary>
                    /// <returns></returns>
                    public Result Version() => GetRest();
                }
                public class PVEStatus
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEStatus(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read node status
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/status"); }

                    /// <summary>
                    /// Read node status
                    /// </summary>
                    /// <returns></returns>
                    public Result Status() => GetRest();
                    /// <summary>
                    /// Reboot or shutdown a node.
                    /// </summary>
                    /// <param name="command">Specify the command.
                    ///   Enum: reboot,shutdown</param>
                    /// <returns></returns>
                    public Result CreateRest(string command)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("command", command);
                        return _client.Create($"/nodes/{_node}/status", parameters);
                    }

                    /// <summary>
                    /// Reboot or shutdown a node.
                    /// </summary>
                    /// <param name="command">Specify the command.
                    ///   Enum: reboot,shutdown</param>
                    /// <returns></returns>
                    public Result NodeCmd(string command) => CreateRest(command);
                }
                public class PVENetstat
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVENetstat(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read tap/vm network device interface counters
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/netstat"); }

                    /// <summary>
                    /// Read tap/vm network device interface counters
                    /// </summary>
                    /// <returns></returns>
                    public Result Netstat() => GetRest();
                }
                public class PVEExecute
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEExecute(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Execute multiple commands in order.
                    /// </summary>
                    /// <param name="commands">JSON encoded array of commands.</param>
                    /// <returns></returns>
                    public Result CreateRest(string commands)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("commands", commands);
                        return _client.Create($"/nodes/{_node}/execute", parameters);
                    }

                    /// <summary>
                    /// Execute multiple commands in order.
                    /// </summary>
                    /// <param name="commands">JSON encoded array of commands.</param>
                    /// <returns></returns>
                    public Result Execute(string commands) => CreateRest(commands);
                }
                public class PVEWakeonlan
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEWakeonlan(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Try to wake a node via 'wake on LAN' network packet.
                    /// </summary>
                    /// <returns></returns>
                    public Result CreateRest() { return _client.Create($"/nodes/{_node}/wakeonlan"); }

                    /// <summary>
                    /// Try to wake a node via 'wake on LAN' network packet.
                    /// </summary>
                    /// <returns></returns>
                    public Result Wakeonlan() => CreateRest();
                }
                public class PVERrd
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVERrd(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read node RRD statistics (returns PNG)
                    /// </summary>
                    /// <param name="ds">The list of datasources you want to display.</param>
                    /// <param name="timeframe">Specify the time frame you are interested in.
                    ///   Enum: hour,day,week,month,year</param>
                    /// <param name="cf">The RRD consolidation function
                    ///   Enum: AVERAGE,MAX</param>
                    /// <returns></returns>
                    public Result GetRest(string ds, string timeframe, string cf = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("ds", ds);
                        parameters.Add("timeframe", timeframe);
                        parameters.Add("cf", cf);
                        return _client.Get($"/nodes/{_node}/rrd", parameters);
                    }

                    /// <summary>
                    /// Read node RRD statistics (returns PNG)
                    /// </summary>
                    /// <param name="ds">The list of datasources you want to display.</param>
                    /// <param name="timeframe">Specify the time frame you are interested in.
                    ///   Enum: hour,day,week,month,year</param>
                    /// <param name="cf">The RRD consolidation function
                    ///   Enum: AVERAGE,MAX</param>
                    /// <returns></returns>
                    public Result Rrd(string ds, string timeframe, string cf = null) => GetRest(ds, timeframe, cf);
                }
                public class PVERrddata
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVERrddata(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read node RRD statistics
                    /// </summary>
                    /// <param name="timeframe">Specify the time frame you are interested in.
                    ///   Enum: hour,day,week,month,year</param>
                    /// <param name="cf">The RRD consolidation function
                    ///   Enum: AVERAGE,MAX</param>
                    /// <returns></returns>
                    public Result GetRest(string timeframe, string cf = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("timeframe", timeframe);
                        parameters.Add("cf", cf);
                        return _client.Get($"/nodes/{_node}/rrddata", parameters);
                    }

                    /// <summary>
                    /// Read node RRD statistics
                    /// </summary>
                    /// <param name="timeframe">Specify the time frame you are interested in.
                    ///   Enum: hour,day,week,month,year</param>
                    /// <param name="cf">The RRD consolidation function
                    ///   Enum: AVERAGE,MAX</param>
                    /// <returns></returns>
                    public Result Rrddata(string timeframe, string cf = null) => GetRest(timeframe, cf);
                }
                public class PVESyslog
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVESyslog(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read system log
                    /// </summary>
                    /// <param name="limit"></param>
                    /// <param name="service">Service ID</param>
                    /// <param name="since">Display all log since this date-time string.</param>
                    /// <param name="start"></param>
                    /// <param name="until">Display all log until this date-time string.</param>
                    /// <returns></returns>
                    public Result GetRest(int? limit = null, string service = null, string since = null, int? start = null, string until = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("limit", limit);
                        parameters.Add("service", service);
                        parameters.Add("since", since);
                        parameters.Add("start", start);
                        parameters.Add("until", until);
                        return _client.Get($"/nodes/{_node}/syslog", parameters);
                    }

                    /// <summary>
                    /// Read system log
                    /// </summary>
                    /// <param name="limit"></param>
                    /// <param name="service">Service ID</param>
                    /// <param name="since">Display all log since this date-time string.</param>
                    /// <param name="start"></param>
                    /// <param name="until">Display all log until this date-time string.</param>
                    /// <returns></returns>
                    public Result Syslog(int? limit = null, string service = null, string since = null, int? start = null, string until = null) => GetRest(limit, service, since, start, until);
                }
                public class PVEJournal
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEJournal(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read Journal
                    /// </summary>
                    /// <param name="endcursor">End before the given Cursor. Conflicts with 'until'</param>
                    /// <param name="lastentries">Limit to the last X lines. Conflicts with a range.</param>
                    /// <param name="since">Display all log since this UNIX epoch. Conflicts with 'startcursor'.</param>
                    /// <param name="startcursor">Start after the given Cursor. Conflicts with 'since'</param>
                    /// <param name="until">Display all log until this UNIX epoch. Conflicts with 'endcursor'.</param>
                    /// <returns></returns>
                    public Result GetRest(string endcursor = null, int? lastentries = null, int? since = null, string startcursor = null, int? until = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("endcursor", endcursor);
                        parameters.Add("lastentries", lastentries);
                        parameters.Add("since", since);
                        parameters.Add("startcursor", startcursor);
                        parameters.Add("until", until);
                        return _client.Get($"/nodes/{_node}/journal", parameters);
                    }

                    /// <summary>
                    /// Read Journal
                    /// </summary>
                    /// <param name="endcursor">End before the given Cursor. Conflicts with 'until'</param>
                    /// <param name="lastentries">Limit to the last X lines. Conflicts with a range.</param>
                    /// <param name="since">Display all log since this UNIX epoch. Conflicts with 'startcursor'.</param>
                    /// <param name="startcursor">Start after the given Cursor. Conflicts with 'since'</param>
                    /// <param name="until">Display all log until this UNIX epoch. Conflicts with 'endcursor'.</param>
                    /// <returns></returns>
                    public Result Journal(string endcursor = null, int? lastentries = null, int? since = null, string startcursor = null, int? until = null) => GetRest(endcursor, lastentries, since, startcursor, until);
                }
                public class PVEVncshell
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEVncshell(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login.
                    ///   Enum: upgrade,ceph_install,login</param>
                    /// <param name="height">sets the height of the console in pixels.</param>
                    /// <param name="upgrade">Deprecated, use the 'cmd' property instead! Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <param name="websocket">use websocket instead of standard vnc.</param>
                    /// <param name="width">sets the width of the console in pixels.</param>
                    /// <returns></returns>
                    public Result CreateRest(string cmd = null, int? height = null, bool? upgrade = null, bool? websocket = null, int? width = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cmd", cmd);
                        parameters.Add("height", height);
                        parameters.Add("upgrade", upgrade);
                        parameters.Add("websocket", websocket);
                        parameters.Add("width", width);
                        return _client.Create($"/nodes/{_node}/vncshell", parameters);
                    }

                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login.
                    ///   Enum: upgrade,ceph_install,login</param>
                    /// <param name="height">sets the height of the console in pixels.</param>
                    /// <param name="upgrade">Deprecated, use the 'cmd' property instead! Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <param name="websocket">use websocket instead of standard vnc.</param>
                    /// <param name="width">sets the width of the console in pixels.</param>
                    /// <returns></returns>
                    public Result Vncshell(string cmd = null, int? height = null, bool? upgrade = null, bool? websocket = null, int? width = null) => CreateRest(cmd, height, upgrade, websocket, width);
                }
                public class PVETermproxy
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVETermproxy(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login.
                    ///   Enum: upgrade,ceph_install,login</param>
                    /// <param name="upgrade">Deprecated, use the 'cmd' property instead! Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <returns></returns>
                    public Result CreateRest(string cmd = null, bool? upgrade = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cmd", cmd);
                        parameters.Add("upgrade", upgrade);
                        return _client.Create($"/nodes/{_node}/termproxy", parameters);
                    }

                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login.
                    ///   Enum: upgrade,ceph_install,login</param>
                    /// <param name="upgrade">Deprecated, use the 'cmd' property instead! Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <returns></returns>
                    public Result Termproxy(string cmd = null, bool? upgrade = null) => CreateRest(cmd, upgrade);
                }
                public class PVEVncwebsocket
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEVncwebsocket(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Opens a weksocket for VNC traffic.
                    /// </summary>
                    /// <param name="port">Port number returned by previous vncproxy call.</param>
                    /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                    /// <returns></returns>
                    public Result GetRest(int port, string vncticket)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("port", port);
                        parameters.Add("vncticket", vncticket);
                        return _client.Get($"/nodes/{_node}/vncwebsocket", parameters);
                    }

                    /// <summary>
                    /// Opens a weksocket for VNC traffic.
                    /// </summary>
                    /// <param name="port">Port number returned by previous vncproxy call.</param>
                    /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                    /// <returns></returns>
                    public Result Vncwebsocket(int port, string vncticket) => GetRest(port, vncticket);
                }
                public class PVESpiceshell
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVESpiceshell(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Creates a SPICE shell.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login.
                    ///   Enum: upgrade,ceph_install,login</param>
                    /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                    /// <param name="upgrade">Deprecated, use the 'cmd' property instead! Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <returns></returns>
                    public Result CreateRest(string cmd = null, string proxy = null, bool? upgrade = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cmd", cmd);
                        parameters.Add("proxy", proxy);
                        parameters.Add("upgrade", upgrade);
                        return _client.Create($"/nodes/{_node}/spiceshell", parameters);
                    }

                    /// <summary>
                    /// Creates a SPICE shell.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login.
                    ///   Enum: upgrade,ceph_install,login</param>
                    /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                    /// <param name="upgrade">Deprecated, use the 'cmd' property instead! Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <returns></returns>
                    public Result Spiceshell(string cmd = null, string proxy = null, bool? upgrade = null) => CreateRest(cmd, proxy, upgrade);
                }
                public class PVEDns
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEDns(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read DNS settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/dns"); }

                    /// <summary>
                    /// Read DNS settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result Dns() => GetRest();
                    /// <summary>
                    /// Write DNS settings.
                    /// </summary>
                    /// <param name="search">Search domain for host-name lookup.</param>
                    /// <param name="dns1">First name server IP address.</param>
                    /// <param name="dns2">Second name server IP address.</param>
                    /// <param name="dns3">Third name server IP address.</param>
                    /// <returns></returns>
                    public Result SetRest(string search, string dns1 = null, string dns2 = null, string dns3 = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("search", search);
                        parameters.Add("dns1", dns1);
                        parameters.Add("dns2", dns2);
                        parameters.Add("dns3", dns3);
                        return _client.Set($"/nodes/{_node}/dns", parameters);
                    }

                    /// <summary>
                    /// Write DNS settings.
                    /// </summary>
                    /// <param name="search">Search domain for host-name lookup.</param>
                    /// <param name="dns1">First name server IP address.</param>
                    /// <param name="dns2">Second name server IP address.</param>
                    /// <param name="dns3">Third name server IP address.</param>
                    /// <returns></returns>
                    public Result UpdateDns(string search, string dns1 = null, string dns2 = null, string dns3 = null) => SetRest(search, dns1, dns2, dns3);
                }
                public class PVETime
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVETime(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read server time and time zone settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/time"); }

                    /// <summary>
                    /// Read server time and time zone settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result Time() => GetRest();
                    /// <summary>
                    /// Set time zone.
                    /// </summary>
                    /// <param name="timezone">Time zone. The file '/usr/share/zoneinfo/zone.tab' contains the list of valid names.</param>
                    /// <returns></returns>
                    public Result SetRest(string timezone)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("timezone", timezone);
                        return _client.Set($"/nodes/{_node}/time", parameters);
                    }

                    /// <summary>
                    /// Set time zone.
                    /// </summary>
                    /// <param name="timezone">Time zone. The file '/usr/share/zoneinfo/zone.tab' contains the list of valid names.</param>
                    /// <returns></returns>
                    public Result SetTimezone(string timezone) => SetRest(timezone);
                }
                public class PVEAplinfo
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEAplinfo(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Get list of appliances.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/aplinfo"); }

                    /// <summary>
                    /// Get list of appliances.
                    /// </summary>
                    /// <returns></returns>
                    public Result Aplinfo() => GetRest();
                    /// <summary>
                    /// Download appliance templates.
                    /// </summary>
                    /// <param name="storage">The storage where the template will be stored</param>
                    /// <param name="template">The template which will downloaded</param>
                    /// <returns></returns>
                    public Result CreateRest(string storage, string template)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("storage", storage);
                        parameters.Add("template", template);
                        return _client.Create($"/nodes/{_node}/aplinfo", parameters);
                    }

                    /// <summary>
                    /// Download appliance templates.
                    /// </summary>
                    /// <param name="storage">The storage where the template will be stored</param>
                    /// <param name="template">The template which will downloaded</param>
                    /// <returns></returns>
                    public Result AplDownload(string storage, string template) => CreateRest(storage, template);
                }
                public class PVEReport
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEReport(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Gather various systems information about a node
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/report"); }

                    /// <summary>
                    /// Gather various systems information about a node
                    /// </summary>
                    /// <returns></returns>
                    public Result Report() => GetRest();
                }
                public class PVEStartall
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEStartall(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Start all VMs and containers (when onboot=1).
                    /// </summary>
                    /// <param name="force">force if onboot=0.</param>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <returns></returns>
                    public Result CreateRest(bool? force = null, string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        parameters.Add("vms", vms);
                        return _client.Create($"/nodes/{_node}/startall", parameters);
                    }

                    /// <summary>
                    /// Start all VMs and containers (when onboot=1).
                    /// </summary>
                    /// <param name="force">force if onboot=0.</param>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <returns></returns>
                    public Result Startall(bool? force = null, string vms = null) => CreateRest(force, vms);
                }
                public class PVEStopall
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEStopall(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Stop all VMs and Containers.
                    /// </summary>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <returns></returns>
                    public Result CreateRest(string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("vms", vms);
                        return _client.Create($"/nodes/{_node}/stopall", parameters);
                    }

                    /// <summary>
                    /// Stop all VMs and Containers.
                    /// </summary>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <returns></returns>
                    public Result Stopall(string vms = null) => CreateRest(vms);
                }
                public class PVEMigrateall
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEMigrateall(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Migrate all VMs and Containers.
                    /// </summary>
                    /// <param name="target">Target node.</param>
                    /// <param name="maxworkers">Maximal number of parallel migration job. If not set use 'max_workers' from datacenter.cfg, one of both must be set!</param>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <returns></returns>
                    public Result CreateRest(string target, int? maxworkers = null, string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("target", target);
                        parameters.Add("maxworkers", maxworkers);
                        parameters.Add("vms", vms);
                        return _client.Create($"/nodes/{_node}/migrateall", parameters);
                    }

                    /// <summary>
                    /// Migrate all VMs and Containers.
                    /// </summary>
                    /// <param name="target">Target node.</param>
                    /// <param name="maxworkers">Maximal number of parallel migration job. If not set use 'max_workers' from datacenter.cfg, one of both must be set!</param>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <returns></returns>
                    public Result Migrateall(string target, int? maxworkers = null, string vms = null) => CreateRest(target, maxworkers, vms);
                }
                public class PVEHosts
                {
                    private readonly Client _client;
                    private readonly object _node;
                    internal PVEHosts(Client client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Get the content of /etc/hosts.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/nodes/{_node}/hosts"); }

                    /// <summary>
                    /// Get the content of /etc/hosts.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetEtcHosts() => GetRest();
                    /// <summary>
                    /// Write /etc/hosts.
                    /// </summary>
                    /// <param name="data">The target content of /etc/hosts.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <returns></returns>
                    public Result CreateRest(string data, string digest = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("data", data);
                        parameters.Add("digest", digest);
                        return _client.Create($"/nodes/{_node}/hosts", parameters);
                    }

                    /// <summary>
                    /// Write /etc/hosts.
                    /// </summary>
                    /// <param name="data">The target content of /etc/hosts.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <returns></returns>
                    public Result WriteEtcHosts(string data, string digest = null) => CreateRest(data, digest);
                }
                /// <summary>
                /// Node index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/nodes/{_node}"); }

                /// <summary>
                /// Node index.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
            }
            /// <summary>
            /// Cluster node index.
            /// </summary>
            /// <returns></returns>
            public Result GetRest() { return _client.Get($"/nodes"); }

            /// <summary>
            /// Cluster node index.
            /// </summary>
            /// <returns></returns>
            public Result Index() => GetRest();
        }
        public class PVEStorage
        {
            private readonly Client _client;

            internal PVEStorage(Client client) { _client = client; }
            public PVEItemStorage this[object storage] => new PVEItemStorage(_client, storage);
            public class PVEItemStorage
            {
                private readonly Client _client;
                private readonly object _storage;
                internal PVEItemStorage(Client client, object storage) { _client = client; _storage = storage; }
                /// <summary>
                /// Delete storage configuration.
                /// </summary>
                /// <returns></returns>
                public Result DeleteRest() { return _client.Delete($"/storage/{_storage}"); }

                /// <summary>
                /// Delete storage configuration.
                /// </summary>
                /// <returns></returns>
                public Result Delete() => DeleteRest();
                /// <summary>
                /// Read storage configuration.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/storage/{_storage}"); }

                /// <summary>
                /// Read storage configuration.
                /// </summary>
                /// <returns></returns>
                public Result Read() => GetRest();
                /// <summary>
                /// Update storage configuration.
                /// </summary>
                /// <param name="blocksize">block size</param>
                /// <param name="bwlimit">Set bandwidth/io limits various operations.</param>
                /// <param name="comstar_hg">host group for comstar views</param>
                /// <param name="comstar_tg">target group for comstar views</param>
                /// <param name="content">Allowed content types.  NOTE: the value 'rootdir' is used for Containers, and value 'images' for VMs. </param>
                /// <param name="delete">A list of settings you want to delete.</param>
                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                /// <param name="disable">Flag to disable the storage.</param>
                /// <param name="domain">CIFS domain.</param>
                /// <param name="format">Default image format.</param>
                /// <param name="fuse">Mount CephFS through FUSE.</param>
                /// <param name="is_mountpoint">Assume the given path is an externally managed mountpoint and consider the storage offline if it is not mounted. Using a boolean (yes/no) value serves as a shortcut to using the target path in this field.</param>
                /// <param name="krbd">Always access rbd through krbd kernel module.</param>
                /// <param name="lio_tpg">target portal group for Linux LIO targets</param>
                /// <param name="maxfiles">Maximal number of backup files per VM. Use '0' for unlimted.</param>
                /// <param name="mkdir">Create the directory if it doesn't exist.</param>
                /// <param name="monhost">IP addresses of monitors (for external clusters).</param>
                /// <param name="nodes">List of cluster node names.</param>
                /// <param name="nowritecache">disable write caching on the target</param>
                /// <param name="options">NFS mount options (see 'man nfs')</param>
                /// <param name="password">Password for CIFS share.</param>
                /// <param name="pool">Pool.</param>
                /// <param name="redundancy">The redundancy count specifies the number of nodes to which the resource should be deployed. It must be at least 1 and at most the number of nodes in the cluster.</param>
                /// <param name="saferemove">Zero-out data when removing LVs.</param>
                /// <param name="saferemove_throughput">Wipe throughput (cstream -t parameter value).</param>
                /// <param name="server">Server IP or DNS name.</param>
                /// <param name="server2">Backup volfile server IP or DNS name.</param>
                /// <param name="shared">Mark storage as shared.</param>
                /// <param name="smbversion">SMB protocol version
                ///   Enum: 2.0,2.1,3.0</param>
                /// <param name="sparse">use sparse volumes</param>
                /// <param name="subdir">Subdir to mount.</param>
                /// <param name="tagged_only">Only use logical volumes tagged with 'pve-vm-ID'.</param>
                /// <param name="transport">Gluster transport: tcp or rdma
                ///   Enum: tcp,rdma,unix</param>
                /// <param name="username">RBD Id.</param>
                /// <returns></returns>
                public Result SetRest(string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, string delete = null, string digest = null, bool? disable = null, string domain = null, string format = null, bool? fuse = null, string is_mountpoint = null, bool? krbd = null, string lio_tpg = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string pool = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string transport = null, string username = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("blocksize", blocksize);
                    parameters.Add("bwlimit", bwlimit);
                    parameters.Add("comstar_hg", comstar_hg);
                    parameters.Add("comstar_tg", comstar_tg);
                    parameters.Add("content", content);
                    parameters.Add("delete", delete);
                    parameters.Add("digest", digest);
                    parameters.Add("disable", disable);
                    parameters.Add("domain", domain);
                    parameters.Add("format", format);
                    parameters.Add("fuse", fuse);
                    parameters.Add("is_mountpoint", is_mountpoint);
                    parameters.Add("krbd", krbd);
                    parameters.Add("lio_tpg", lio_tpg);
                    parameters.Add("maxfiles", maxfiles);
                    parameters.Add("mkdir", mkdir);
                    parameters.Add("monhost", monhost);
                    parameters.Add("nodes", nodes);
                    parameters.Add("nowritecache", nowritecache);
                    parameters.Add("options", options);
                    parameters.Add("password", password);
                    parameters.Add("pool", pool);
                    parameters.Add("redundancy", redundancy);
                    parameters.Add("saferemove", saferemove);
                    parameters.Add("saferemove_throughput", saferemove_throughput);
                    parameters.Add("server", server);
                    parameters.Add("server2", server2);
                    parameters.Add("shared", shared);
                    parameters.Add("smbversion", smbversion);
                    parameters.Add("sparse", sparse);
                    parameters.Add("subdir", subdir);
                    parameters.Add("tagged_only", tagged_only);
                    parameters.Add("transport", transport);
                    parameters.Add("username", username);
                    return _client.Set($"/storage/{_storage}", parameters);
                }

                /// <summary>
                /// Update storage configuration.
                /// </summary>
                /// <param name="blocksize">block size</param>
                /// <param name="bwlimit">Set bandwidth/io limits various operations.</param>
                /// <param name="comstar_hg">host group for comstar views</param>
                /// <param name="comstar_tg">target group for comstar views</param>
                /// <param name="content">Allowed content types.  NOTE: the value 'rootdir' is used for Containers, and value 'images' for VMs. </param>
                /// <param name="delete">A list of settings you want to delete.</param>
                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                /// <param name="disable">Flag to disable the storage.</param>
                /// <param name="domain">CIFS domain.</param>
                /// <param name="format">Default image format.</param>
                /// <param name="fuse">Mount CephFS through FUSE.</param>
                /// <param name="is_mountpoint">Assume the given path is an externally managed mountpoint and consider the storage offline if it is not mounted. Using a boolean (yes/no) value serves as a shortcut to using the target path in this field.</param>
                /// <param name="krbd">Always access rbd through krbd kernel module.</param>
                /// <param name="lio_tpg">target portal group for Linux LIO targets</param>
                /// <param name="maxfiles">Maximal number of backup files per VM. Use '0' for unlimted.</param>
                /// <param name="mkdir">Create the directory if it doesn't exist.</param>
                /// <param name="monhost">IP addresses of monitors (for external clusters).</param>
                /// <param name="nodes">List of cluster node names.</param>
                /// <param name="nowritecache">disable write caching on the target</param>
                /// <param name="options">NFS mount options (see 'man nfs')</param>
                /// <param name="password">Password for CIFS share.</param>
                /// <param name="pool">Pool.</param>
                /// <param name="redundancy">The redundancy count specifies the number of nodes to which the resource should be deployed. It must be at least 1 and at most the number of nodes in the cluster.</param>
                /// <param name="saferemove">Zero-out data when removing LVs.</param>
                /// <param name="saferemove_throughput">Wipe throughput (cstream -t parameter value).</param>
                /// <param name="server">Server IP or DNS name.</param>
                /// <param name="server2">Backup volfile server IP or DNS name.</param>
                /// <param name="shared">Mark storage as shared.</param>
                /// <param name="smbversion">SMB protocol version
                ///   Enum: 2.0,2.1,3.0</param>
                /// <param name="sparse">use sparse volumes</param>
                /// <param name="subdir">Subdir to mount.</param>
                /// <param name="tagged_only">Only use logical volumes tagged with 'pve-vm-ID'.</param>
                /// <param name="transport">Gluster transport: tcp or rdma
                ///   Enum: tcp,rdma,unix</param>
                /// <param name="username">RBD Id.</param>
                /// <returns></returns>
                public Result Update(string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, string delete = null, string digest = null, bool? disable = null, string domain = null, string format = null, bool? fuse = null, string is_mountpoint = null, bool? krbd = null, string lio_tpg = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string pool = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string transport = null, string username = null) => SetRest(blocksize, bwlimit, comstar_hg, comstar_tg, content, delete, digest, disable, domain, format, fuse, is_mountpoint, krbd, lio_tpg, maxfiles, mkdir, monhost, nodes, nowritecache, options, password, pool, redundancy, saferemove, saferemove_throughput, server, server2, shared, smbversion, sparse, subdir, tagged_only, transport, username);
            }
            /// <summary>
            /// Storage index.
            /// </summary>
            /// <param name="type">Only list storage of specific type
            ///   Enum: cephfs,cifs,dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,zfs,zfspool</param>
            /// <returns></returns>
            public Result GetRest(string type = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("type", type);
                return _client.Get($"/storage", parameters);
            }

            /// <summary>
            /// Storage index.
            /// </summary>
            /// <param name="type">Only list storage of specific type
            ///   Enum: cephfs,cifs,dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,zfs,zfspool</param>
            /// <returns></returns>
            public Result Index(string type = null) => GetRest(type);
            /// <summary>
            /// Create a new storage.
            /// </summary>
            /// <param name="storage">The storage identifier.</param>
            /// <param name="type">Storage type.
            ///   Enum: cephfs,cifs,dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,zfs,zfspool</param>
            /// <param name="authsupported">Authsupported.</param>
            /// <param name="base_">Base volume. This volume is automatically activated.</param>
            /// <param name="blocksize">block size</param>
            /// <param name="bwlimit">Set bandwidth/io limits various operations.</param>
            /// <param name="comstar_hg">host group for comstar views</param>
            /// <param name="comstar_tg">target group for comstar views</param>
            /// <param name="content">Allowed content types.  NOTE: the value 'rootdir' is used for Containers, and value 'images' for VMs. </param>
            /// <param name="disable">Flag to disable the storage.</param>
            /// <param name="domain">CIFS domain.</param>
            /// <param name="export">NFS export path.</param>
            /// <param name="format">Default image format.</param>
            /// <param name="fuse">Mount CephFS through FUSE.</param>
            /// <param name="is_mountpoint">Assume the given path is an externally managed mountpoint and consider the storage offline if it is not mounted. Using a boolean (yes/no) value serves as a shortcut to using the target path in this field.</param>
            /// <param name="iscsiprovider">iscsi provider</param>
            /// <param name="krbd">Always access rbd through krbd kernel module.</param>
            /// <param name="lio_tpg">target portal group for Linux LIO targets</param>
            /// <param name="maxfiles">Maximal number of backup files per VM. Use '0' for unlimted.</param>
            /// <param name="mkdir">Create the directory if it doesn't exist.</param>
            /// <param name="monhost">IP addresses of monitors (for external clusters).</param>
            /// <param name="nodes">List of cluster node names.</param>
            /// <param name="nowritecache">disable write caching on the target</param>
            /// <param name="options">NFS mount options (see 'man nfs')</param>
            /// <param name="password">Password for CIFS share.</param>
            /// <param name="path">File system path.</param>
            /// <param name="pool">Pool.</param>
            /// <param name="portal">iSCSI portal (IP or DNS name with optional port).</param>
            /// <param name="redundancy">The redundancy count specifies the number of nodes to which the resource should be deployed. It must be at least 1 and at most the number of nodes in the cluster.</param>
            /// <param name="saferemove">Zero-out data when removing LVs.</param>
            /// <param name="saferemove_throughput">Wipe throughput (cstream -t parameter value).</param>
            /// <param name="server">Server IP or DNS name.</param>
            /// <param name="server2">Backup volfile server IP or DNS name.</param>
            /// <param name="share">CIFS share.</param>
            /// <param name="shared">Mark storage as shared.</param>
            /// <param name="smbversion">SMB protocol version
            ///   Enum: 2.0,2.1,3.0</param>
            /// <param name="sparse">use sparse volumes</param>
            /// <param name="subdir">Subdir to mount.</param>
            /// <param name="tagged_only">Only use logical volumes tagged with 'pve-vm-ID'.</param>
            /// <param name="target">iSCSI target.</param>
            /// <param name="thinpool">LVM thin pool LV name.</param>
            /// <param name="transport">Gluster transport: tcp or rdma
            ///   Enum: tcp,rdma,unix</param>
            /// <param name="username">RBD Id.</param>
            /// <param name="vgname">Volume group name.</param>
            /// <param name="volume">Glusterfs Volume.</param>
            /// <returns></returns>
            public Result CreateRest(string storage, string type, string authsupported = null, string base_ = null, string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, bool? disable = null, string domain = null, string export = null, string format = null, bool? fuse = null, string is_mountpoint = null, string iscsiprovider = null, bool? krbd = null, string lio_tpg = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string path = null, string pool = null, string portal = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, string share = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string target = null, string thinpool = null, string transport = null, string username = null, string vgname = null, string volume = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("storage", storage);
                parameters.Add("type", type);
                parameters.Add("authsupported", authsupported);
                parameters.Add("base", base_);
                parameters.Add("blocksize", blocksize);
                parameters.Add("bwlimit", bwlimit);
                parameters.Add("comstar_hg", comstar_hg);
                parameters.Add("comstar_tg", comstar_tg);
                parameters.Add("content", content);
                parameters.Add("disable", disable);
                parameters.Add("domain", domain);
                parameters.Add("export", export);
                parameters.Add("format", format);
                parameters.Add("fuse", fuse);
                parameters.Add("is_mountpoint", is_mountpoint);
                parameters.Add("iscsiprovider", iscsiprovider);
                parameters.Add("krbd", krbd);
                parameters.Add("lio_tpg", lio_tpg);
                parameters.Add("maxfiles", maxfiles);
                parameters.Add("mkdir", mkdir);
                parameters.Add("monhost", monhost);
                parameters.Add("nodes", nodes);
                parameters.Add("nowritecache", nowritecache);
                parameters.Add("options", options);
                parameters.Add("password", password);
                parameters.Add("path", path);
                parameters.Add("pool", pool);
                parameters.Add("portal", portal);
                parameters.Add("redundancy", redundancy);
                parameters.Add("saferemove", saferemove);
                parameters.Add("saferemove_throughput", saferemove_throughput);
                parameters.Add("server", server);
                parameters.Add("server2", server2);
                parameters.Add("share", share);
                parameters.Add("shared", shared);
                parameters.Add("smbversion", smbversion);
                parameters.Add("sparse", sparse);
                parameters.Add("subdir", subdir);
                parameters.Add("tagged_only", tagged_only);
                parameters.Add("target", target);
                parameters.Add("thinpool", thinpool);
                parameters.Add("transport", transport);
                parameters.Add("username", username);
                parameters.Add("vgname", vgname);
                parameters.Add("volume", volume);
                return _client.Create($"/storage", parameters);
            }

            /// <summary>
            /// Create a new storage.
            /// </summary>
            /// <param name="storage">The storage identifier.</param>
            /// <param name="type">Storage type.
            ///   Enum: cephfs,cifs,dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,zfs,zfspool</param>
            /// <param name="authsupported">Authsupported.</param>
            /// <param name="base_">Base volume. This volume is automatically activated.</param>
            /// <param name="blocksize">block size</param>
            /// <param name="bwlimit">Set bandwidth/io limits various operations.</param>
            /// <param name="comstar_hg">host group for comstar views</param>
            /// <param name="comstar_tg">target group for comstar views</param>
            /// <param name="content">Allowed content types.  NOTE: the value 'rootdir' is used for Containers, and value 'images' for VMs. </param>
            /// <param name="disable">Flag to disable the storage.</param>
            /// <param name="domain">CIFS domain.</param>
            /// <param name="export">NFS export path.</param>
            /// <param name="format">Default image format.</param>
            /// <param name="fuse">Mount CephFS through FUSE.</param>
            /// <param name="is_mountpoint">Assume the given path is an externally managed mountpoint and consider the storage offline if it is not mounted. Using a boolean (yes/no) value serves as a shortcut to using the target path in this field.</param>
            /// <param name="iscsiprovider">iscsi provider</param>
            /// <param name="krbd">Always access rbd through krbd kernel module.</param>
            /// <param name="lio_tpg">target portal group for Linux LIO targets</param>
            /// <param name="maxfiles">Maximal number of backup files per VM. Use '0' for unlimted.</param>
            /// <param name="mkdir">Create the directory if it doesn't exist.</param>
            /// <param name="monhost">IP addresses of monitors (for external clusters).</param>
            /// <param name="nodes">List of cluster node names.</param>
            /// <param name="nowritecache">disable write caching on the target</param>
            /// <param name="options">NFS mount options (see 'man nfs')</param>
            /// <param name="password">Password for CIFS share.</param>
            /// <param name="path">File system path.</param>
            /// <param name="pool">Pool.</param>
            /// <param name="portal">iSCSI portal (IP or DNS name with optional port).</param>
            /// <param name="redundancy">The redundancy count specifies the number of nodes to which the resource should be deployed. It must be at least 1 and at most the number of nodes in the cluster.</param>
            /// <param name="saferemove">Zero-out data when removing LVs.</param>
            /// <param name="saferemove_throughput">Wipe throughput (cstream -t parameter value).</param>
            /// <param name="server">Server IP or DNS name.</param>
            /// <param name="server2">Backup volfile server IP or DNS name.</param>
            /// <param name="share">CIFS share.</param>
            /// <param name="shared">Mark storage as shared.</param>
            /// <param name="smbversion">SMB protocol version
            ///   Enum: 2.0,2.1,3.0</param>
            /// <param name="sparse">use sparse volumes</param>
            /// <param name="subdir">Subdir to mount.</param>
            /// <param name="tagged_only">Only use logical volumes tagged with 'pve-vm-ID'.</param>
            /// <param name="target">iSCSI target.</param>
            /// <param name="thinpool">LVM thin pool LV name.</param>
            /// <param name="transport">Gluster transport: tcp or rdma
            ///   Enum: tcp,rdma,unix</param>
            /// <param name="username">RBD Id.</param>
            /// <param name="vgname">Volume group name.</param>
            /// <param name="volume">Glusterfs Volume.</param>
            /// <returns></returns>
            public Result Create(string storage, string type, string authsupported = null, string base_ = null, string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, bool? disable = null, string domain = null, string export = null, string format = null, bool? fuse = null, string is_mountpoint = null, string iscsiprovider = null, bool? krbd = null, string lio_tpg = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string path = null, string pool = null, string portal = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, string share = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string target = null, string thinpool = null, string transport = null, string username = null, string vgname = null, string volume = null) => CreateRest(storage, type, authsupported, base_, blocksize, bwlimit, comstar_hg, comstar_tg, content, disable, domain, export, format, fuse, is_mountpoint, iscsiprovider, krbd, lio_tpg, maxfiles, mkdir, monhost, nodes, nowritecache, options, password, path, pool, portal, redundancy, saferemove, saferemove_throughput, server, server2, share, shared, smbversion, sparse, subdir, tagged_only, target, thinpool, transport, username, vgname, volume);
        }
        public class PVEAccess
        {
            private readonly Client _client;

            internal PVEAccess(Client client) { _client = client; }
            private PVEUsers _users;
            public PVEUsers Users => _users ?? (_users = new PVEUsers(_client));
            private PVEGroups _groups;
            public PVEGroups Groups => _groups ?? (_groups = new PVEGroups(_client));
            private PVERoles _roles;
            public PVERoles Roles => _roles ?? (_roles = new PVERoles(_client));
            private PVEAcl _acl;
            public PVEAcl Acl => _acl ?? (_acl = new PVEAcl(_client));
            private PVEDomains _domains;
            public PVEDomains Domains => _domains ?? (_domains = new PVEDomains(_client));
            private PVETicket _ticket;
            public PVETicket Ticket => _ticket ?? (_ticket = new PVETicket(_client));
            private PVEPassword _password;
            public PVEPassword Password => _password ?? (_password = new PVEPassword(_client));
            private PVETfa _tfa;
            public PVETfa Tfa => _tfa ?? (_tfa = new PVETfa(_client));
            public class PVEUsers
            {
                private readonly Client _client;

                internal PVEUsers(Client client) { _client = client; }
                public PVEItemUserid this[object userid] => new PVEItemUserid(_client, userid);
                public class PVEItemUserid
                {
                    private readonly Client _client;
                    private readonly object _userid;
                    internal PVEItemUserid(Client client, object userid) { _client = client; _userid = userid; }
                    private PVETfa _tfa;
                    public PVETfa Tfa => _tfa ?? (_tfa = new PVETfa(_client, _userid));
                    public class PVETfa
                    {
                        private readonly Client _client;
                        private readonly object _userid;
                        internal PVETfa(Client client, object userid) { _client = client; _userid = userid; }
                        /// <summary>
                        /// Get user TFA types (Personal and Realm).
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest() { return _client.Get($"/access/users/{_userid}/tfa"); }

                        /// <summary>
                        /// Get user TFA types (Personal and Realm).
                        /// </summary>
                        /// <returns></returns>
                        public Result ReadUserTfaType() => GetRest();
                    }
                    /// <summary>
                    /// Delete user.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest() { return _client.Delete($"/access/users/{_userid}"); }

                    /// <summary>
                    /// Delete user.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteUser() => DeleteRest();
                    /// <summary>
                    /// Get user configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/access/users/{_userid}"); }

                    /// <summary>
                    /// Get user configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result ReadUser() => GetRest();
                    /// <summary>
                    /// Update user configuration.
                    /// </summary>
                    /// <param name="append"></param>
                    /// <param name="comment"></param>
                    /// <param name="email"></param>
                    /// <param name="enable">Enable the account (default). You can set this to '0' to disable the account</param>
                    /// <param name="expire">Account expiration date (seconds since epoch). '0' means no expiration date.</param>
                    /// <param name="firstname"></param>
                    /// <param name="groups"></param>
                    /// <param name="keys">Keys for two factor auth (yubico).</param>
                    /// <param name="lastname"></param>
                    /// <returns></returns>
                    public Result SetRest(bool? append = null, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("append", append);
                        parameters.Add("comment", comment);
                        parameters.Add("email", email);
                        parameters.Add("enable", enable);
                        parameters.Add("expire", expire);
                        parameters.Add("firstname", firstname);
                        parameters.Add("groups", groups);
                        parameters.Add("keys", keys);
                        parameters.Add("lastname", lastname);
                        return _client.Set($"/access/users/{_userid}", parameters);
                    }

                    /// <summary>
                    /// Update user configuration.
                    /// </summary>
                    /// <param name="append"></param>
                    /// <param name="comment"></param>
                    /// <param name="email"></param>
                    /// <param name="enable">Enable the account (default). You can set this to '0' to disable the account</param>
                    /// <param name="expire">Account expiration date (seconds since epoch). '0' means no expiration date.</param>
                    /// <param name="firstname"></param>
                    /// <param name="groups"></param>
                    /// <param name="keys">Keys for two factor auth (yubico).</param>
                    /// <param name="lastname"></param>
                    /// <returns></returns>
                    public Result UpdateUser(bool? append = null, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null) => SetRest(append, comment, email, enable, expire, firstname, groups, keys, lastname);
                }
                /// <summary>
                /// User index.
                /// </summary>
                /// <param name="enabled">Optional filter for enable property.</param>
                /// <returns></returns>
                public Result GetRest(bool? enabled = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("enabled", enabled);
                    return _client.Get($"/access/users", parameters);
                }

                /// <summary>
                /// User index.
                /// </summary>
                /// <param name="enabled">Optional filter for enable property.</param>
                /// <returns></returns>
                public Result Index(bool? enabled = null) => GetRest(enabled);
                /// <summary>
                /// Create new user.
                /// </summary>
                /// <param name="userid">User ID</param>
                /// <param name="comment"></param>
                /// <param name="email"></param>
                /// <param name="enable">Enable the account (default). You can set this to '0' to disable the account</param>
                /// <param name="expire">Account expiration date (seconds since epoch). '0' means no expiration date.</param>
                /// <param name="firstname"></param>
                /// <param name="groups"></param>
                /// <param name="keys">Keys for two factor auth (yubico).</param>
                /// <param name="lastname"></param>
                /// <param name="password">Initial password.</param>
                /// <returns></returns>
                public Result CreateRest(string userid, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null, string password = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("userid", userid);
                    parameters.Add("comment", comment);
                    parameters.Add("email", email);
                    parameters.Add("enable", enable);
                    parameters.Add("expire", expire);
                    parameters.Add("firstname", firstname);
                    parameters.Add("groups", groups);
                    parameters.Add("keys", keys);
                    parameters.Add("lastname", lastname);
                    parameters.Add("password", password);
                    return _client.Create($"/access/users", parameters);
                }

                /// <summary>
                /// Create new user.
                /// </summary>
                /// <param name="userid">User ID</param>
                /// <param name="comment"></param>
                /// <param name="email"></param>
                /// <param name="enable">Enable the account (default). You can set this to '0' to disable the account</param>
                /// <param name="expire">Account expiration date (seconds since epoch). '0' means no expiration date.</param>
                /// <param name="firstname"></param>
                /// <param name="groups"></param>
                /// <param name="keys">Keys for two factor auth (yubico).</param>
                /// <param name="lastname"></param>
                /// <param name="password">Initial password.</param>
                /// <returns></returns>
                public Result CreateUser(string userid, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null, string password = null) => CreateRest(userid, comment, email, enable, expire, firstname, groups, keys, lastname, password);
            }
            public class PVEGroups
            {
                private readonly Client _client;

                internal PVEGroups(Client client) { _client = client; }
                public PVEItemGroupid this[object groupid] => new PVEItemGroupid(_client, groupid);
                public class PVEItemGroupid
                {
                    private readonly Client _client;
                    private readonly object _groupid;
                    internal PVEItemGroupid(Client client, object groupid) { _client = client; _groupid = groupid; }
                    /// <summary>
                    /// Delete group.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest() { return _client.Delete($"/access/groups/{_groupid}"); }

                    /// <summary>
                    /// Delete group.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteGroup() => DeleteRest();
                    /// <summary>
                    /// Get group configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/access/groups/{_groupid}"); }

                    /// <summary>
                    /// Get group configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result ReadGroup() => GetRest();
                    /// <summary>
                    /// Update group data.
                    /// </summary>
                    /// <param name="comment"></param>
                    /// <returns></returns>
                    public Result SetRest(string comment = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        return _client.Set($"/access/groups/{_groupid}", parameters);
                    }

                    /// <summary>
                    /// Update group data.
                    /// </summary>
                    /// <param name="comment"></param>
                    /// <returns></returns>
                    public Result UpdateGroup(string comment = null) => SetRest(comment);
                }
                /// <summary>
                /// Group index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/access/groups"); }

                /// <summary>
                /// Group index.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
                /// <summary>
                /// Create new group.
                /// </summary>
                /// <param name="groupid"></param>
                /// <param name="comment"></param>
                /// <returns></returns>
                public Result CreateRest(string groupid, string comment = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("groupid", groupid);
                    parameters.Add("comment", comment);
                    return _client.Create($"/access/groups", parameters);
                }

                /// <summary>
                /// Create new group.
                /// </summary>
                /// <param name="groupid"></param>
                /// <param name="comment"></param>
                /// <returns></returns>
                public Result CreateGroup(string groupid, string comment = null) => CreateRest(groupid, comment);
            }
            public class PVERoles
            {
                private readonly Client _client;

                internal PVERoles(Client client) { _client = client; }
                public PVEItemRoleid this[object roleid] => new PVEItemRoleid(_client, roleid);
                public class PVEItemRoleid
                {
                    private readonly Client _client;
                    private readonly object _roleid;
                    internal PVEItemRoleid(Client client, object roleid) { _client = client; _roleid = roleid; }
                    /// <summary>
                    /// Delete role.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest() { return _client.Delete($"/access/roles/{_roleid}"); }

                    /// <summary>
                    /// Delete role.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRole() => DeleteRest();
                    /// <summary>
                    /// Get role configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/access/roles/{_roleid}"); }

                    /// <summary>
                    /// Get role configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result ReadRole() => GetRest();
                    /// <summary>
                    /// Update an existing role.
                    /// </summary>
                    /// <param name="append"></param>
                    /// <param name="privs"></param>
                    /// <returns></returns>
                    public Result SetRest(bool? append = null, string privs = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("append", append);
                        parameters.Add("privs", privs);
                        return _client.Set($"/access/roles/{_roleid}", parameters);
                    }

                    /// <summary>
                    /// Update an existing role.
                    /// </summary>
                    /// <param name="append"></param>
                    /// <param name="privs"></param>
                    /// <returns></returns>
                    public Result UpdateRole(bool? append = null, string privs = null) => SetRest(append, privs);
                }
                /// <summary>
                /// Role index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/access/roles"); }

                /// <summary>
                /// Role index.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
                /// <summary>
                /// Create new role.
                /// </summary>
                /// <param name="roleid"></param>
                /// <param name="privs"></param>
                /// <returns></returns>
                public Result CreateRest(string roleid, string privs = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("roleid", roleid);
                    parameters.Add("privs", privs);
                    return _client.Create($"/access/roles", parameters);
                }

                /// <summary>
                /// Create new role.
                /// </summary>
                /// <param name="roleid"></param>
                /// <param name="privs"></param>
                /// <returns></returns>
                public Result CreateRole(string roleid, string privs = null) => CreateRest(roleid, privs);
            }
            public class PVEAcl
            {
                private readonly Client _client;

                internal PVEAcl(Client client) { _client = client; }
                /// <summary>
                /// Get Access Control List (ACLs).
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/access/acl"); }

                /// <summary>
                /// Get Access Control List (ACLs).
                /// </summary>
                /// <returns></returns>
                public Result ReadAcl() => GetRest();
                /// <summary>
                /// Update Access Control List (add or remove permissions).
                /// </summary>
                /// <param name="path">Access control path</param>
                /// <param name="roles">List of roles.</param>
                /// <param name="delete">Remove permissions (instead of adding it).</param>
                /// <param name="groups">List of groups.</param>
                /// <param name="propagate">Allow to propagate (inherit) permissions.</param>
                /// <param name="users">List of users.</param>
                /// <returns></returns>
                public Result SetRest(string path, string roles, bool? delete = null, string groups = null, bool? propagate = null, string users = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("path", path);
                    parameters.Add("roles", roles);
                    parameters.Add("delete", delete);
                    parameters.Add("groups", groups);
                    parameters.Add("propagate", propagate);
                    parameters.Add("users", users);
                    return _client.Set($"/access/acl", parameters);
                }

                /// <summary>
                /// Update Access Control List (add or remove permissions).
                /// </summary>
                /// <param name="path">Access control path</param>
                /// <param name="roles">List of roles.</param>
                /// <param name="delete">Remove permissions (instead of adding it).</param>
                /// <param name="groups">List of groups.</param>
                /// <param name="propagate">Allow to propagate (inherit) permissions.</param>
                /// <param name="users">List of users.</param>
                /// <returns></returns>
                public Result UpdateAcl(string path, string roles, bool? delete = null, string groups = null, bool? propagate = null, string users = null) => SetRest(path, roles, delete, groups, propagate, users);
            }
            public class PVEDomains
            {
                private readonly Client _client;

                internal PVEDomains(Client client) { _client = client; }
                public PVEItemRealm this[object realm] => new PVEItemRealm(_client, realm);
                public class PVEItemRealm
                {
                    private readonly Client _client;
                    private readonly object _realm;
                    internal PVEItemRealm(Client client, object realm) { _client = client; _realm = realm; }
                    /// <summary>
                    /// Delete an authentication server.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest() { return _client.Delete($"/access/domains/{_realm}"); }

                    /// <summary>
                    /// Delete an authentication server.
                    /// </summary>
                    /// <returns></returns>
                    public Result Delete() => DeleteRest();
                    /// <summary>
                    /// Get auth server configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest() { return _client.Get($"/access/domains/{_realm}"); }

                    /// <summary>
                    /// Get auth server configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result Read() => GetRest();
                    /// <summary>
                    /// Update authentication server settings.
                    /// </summary>
                    /// <param name="base_dn">LDAP base domain name</param>
                    /// <param name="bind_dn">LDAP bind domain name</param>
                    /// <param name="capath">Path to the CA certificate store</param>
                    /// <param name="cert">Path to the client certificate</param>
                    /// <param name="certkey">Path to the client certificate key</param>
                    /// <param name="comment">Description.</param>
                    /// <param name="default_">Use this as default realm</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="domain">AD domain name</param>
                    /// <param name="port">Server port.</param>
                    /// <param name="secure">Use secure LDAPS protocol.</param>
                    /// <param name="server1">Server IP address (or DNS name)</param>
                    /// <param name="server2">Fallback Server IP address (or DNS name)</param>
                    /// <param name="tfa">Use Two-factor authentication.</param>
                    /// <param name="user_attr">LDAP user attribute name</param>
                    /// <param name="verify">Verify the server's SSL certificate</param>
                    /// <returns></returns>
                    public Result SetRest(string base_dn = null, string bind_dn = null, string capath = null, string cert = null, string certkey = null, string comment = null, bool? default_ = null, string delete = null, string digest = null, string domain = null, int? port = null, bool? secure = null, string server1 = null, string server2 = null, string tfa = null, string user_attr = null, bool? verify = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("base_dn", base_dn);
                        parameters.Add("bind_dn", bind_dn);
                        parameters.Add("capath", capath);
                        parameters.Add("cert", cert);
                        parameters.Add("certkey", certkey);
                        parameters.Add("comment", comment);
                        parameters.Add("default", default_);
                        parameters.Add("delete", delete);
                        parameters.Add("digest", digest);
                        parameters.Add("domain", domain);
                        parameters.Add("port", port);
                        parameters.Add("secure", secure);
                        parameters.Add("server1", server1);
                        parameters.Add("server2", server2);
                        parameters.Add("tfa", tfa);
                        parameters.Add("user_attr", user_attr);
                        parameters.Add("verify", verify);
                        return _client.Set($"/access/domains/{_realm}", parameters);
                    }

                    /// <summary>
                    /// Update authentication server settings.
                    /// </summary>
                    /// <param name="base_dn">LDAP base domain name</param>
                    /// <param name="bind_dn">LDAP bind domain name</param>
                    /// <param name="capath">Path to the CA certificate store</param>
                    /// <param name="cert">Path to the client certificate</param>
                    /// <param name="certkey">Path to the client certificate key</param>
                    /// <param name="comment">Description.</param>
                    /// <param name="default_">Use this as default realm</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="domain">AD domain name</param>
                    /// <param name="port">Server port.</param>
                    /// <param name="secure">Use secure LDAPS protocol.</param>
                    /// <param name="server1">Server IP address (or DNS name)</param>
                    /// <param name="server2">Fallback Server IP address (or DNS name)</param>
                    /// <param name="tfa">Use Two-factor authentication.</param>
                    /// <param name="user_attr">LDAP user attribute name</param>
                    /// <param name="verify">Verify the server's SSL certificate</param>
                    /// <returns></returns>
                    public Result Update(string base_dn = null, string bind_dn = null, string capath = null, string cert = null, string certkey = null, string comment = null, bool? default_ = null, string delete = null, string digest = null, string domain = null, int? port = null, bool? secure = null, string server1 = null, string server2 = null, string tfa = null, string user_attr = null, bool? verify = null) => SetRest(base_dn, bind_dn, capath, cert, certkey, comment, default_, delete, digest, domain, port, secure, server1, server2, tfa, user_attr, verify);
                }
                /// <summary>
                /// Authentication domain index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/access/domains"); }

                /// <summary>
                /// Authentication domain index.
                /// </summary>
                /// <returns></returns>
                public Result Index() => GetRest();
                /// <summary>
                /// Add an authentication server.
                /// </summary>
                /// <param name="realm">Authentication domain ID</param>
                /// <param name="type">Realm type.
                ///   Enum: ad,ldap,pam,pve</param>
                /// <param name="base_dn">LDAP base domain name</param>
                /// <param name="bind_dn">LDAP bind domain name</param>
                /// <param name="capath">Path to the CA certificate store</param>
                /// <param name="cert">Path to the client certificate</param>
                /// <param name="certkey">Path to the client certificate key</param>
                /// <param name="comment">Description.</param>
                /// <param name="default_">Use this as default realm</param>
                /// <param name="domain">AD domain name</param>
                /// <param name="port">Server port.</param>
                /// <param name="secure">Use secure LDAPS protocol.</param>
                /// <param name="server1">Server IP address (or DNS name)</param>
                /// <param name="server2">Fallback Server IP address (or DNS name)</param>
                /// <param name="tfa">Use Two-factor authentication.</param>
                /// <param name="user_attr">LDAP user attribute name</param>
                /// <param name="verify">Verify the server's SSL certificate</param>
                /// <returns></returns>
                public Result CreateRest(string realm, string type, string base_dn = null, string bind_dn = null, string capath = null, string cert = null, string certkey = null, string comment = null, bool? default_ = null, string domain = null, int? port = null, bool? secure = null, string server1 = null, string server2 = null, string tfa = null, string user_attr = null, bool? verify = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("realm", realm);
                    parameters.Add("type", type);
                    parameters.Add("base_dn", base_dn);
                    parameters.Add("bind_dn", bind_dn);
                    parameters.Add("capath", capath);
                    parameters.Add("cert", cert);
                    parameters.Add("certkey", certkey);
                    parameters.Add("comment", comment);
                    parameters.Add("default", default_);
                    parameters.Add("domain", domain);
                    parameters.Add("port", port);
                    parameters.Add("secure", secure);
                    parameters.Add("server1", server1);
                    parameters.Add("server2", server2);
                    parameters.Add("tfa", tfa);
                    parameters.Add("user_attr", user_attr);
                    parameters.Add("verify", verify);
                    return _client.Create($"/access/domains", parameters);
                }

                /// <summary>
                /// Add an authentication server.
                /// </summary>
                /// <param name="realm">Authentication domain ID</param>
                /// <param name="type">Realm type.
                ///   Enum: ad,ldap,pam,pve</param>
                /// <param name="base_dn">LDAP base domain name</param>
                /// <param name="bind_dn">LDAP bind domain name</param>
                /// <param name="capath">Path to the CA certificate store</param>
                /// <param name="cert">Path to the client certificate</param>
                /// <param name="certkey">Path to the client certificate key</param>
                /// <param name="comment">Description.</param>
                /// <param name="default_">Use this as default realm</param>
                /// <param name="domain">AD domain name</param>
                /// <param name="port">Server port.</param>
                /// <param name="secure">Use secure LDAPS protocol.</param>
                /// <param name="server1">Server IP address (or DNS name)</param>
                /// <param name="server2">Fallback Server IP address (or DNS name)</param>
                /// <param name="tfa">Use Two-factor authentication.</param>
                /// <param name="user_attr">LDAP user attribute name</param>
                /// <param name="verify">Verify the server's SSL certificate</param>
                /// <returns></returns>
                public Result Create(string realm, string type, string base_dn = null, string bind_dn = null, string capath = null, string cert = null, string certkey = null, string comment = null, bool? default_ = null, string domain = null, int? port = null, bool? secure = null, string server1 = null, string server2 = null, string tfa = null, string user_attr = null, bool? verify = null) => CreateRest(realm, type, base_dn, bind_dn, capath, cert, certkey, comment, default_, domain, port, secure, server1, server2, tfa, user_attr, verify);
            }
            public class PVETicket
            {
                private readonly Client _client;

                internal PVETicket(Client client) { _client = client; }
                /// <summary>
                /// Dummy. Useful for formatters which want to provide a login page.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/access/ticket"); }

                /// <summary>
                /// Dummy. Useful for formatters which want to provide a login page.
                /// </summary>
                /// <returns></returns>
                public Result GetTicket() => GetRest();
                /// <summary>
                /// Create or verify authentication ticket.
                /// </summary>
                /// <param name="password">The secret password. This can also be a valid ticket.</param>
                /// <param name="username">User name</param>
                /// <param name="otp">One-time password for Two-factor authentication.</param>
                /// <param name="path">Verify ticket, and check if user have access 'privs' on 'path'</param>
                /// <param name="privs">Verify ticket, and check if user have access 'privs' on 'path'</param>
                /// <param name="realm">You can optionally pass the realm using this parameter. Normally the realm is simply added to the username &amp;lt;username&amp;gt;@&amp;lt;relam&amp;gt;.</param>
                /// <returns></returns>
                public Result CreateRest(string password, string username, string otp = null, string path = null, string privs = null, string realm = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("password", password);
                    parameters.Add("username", username);
                    parameters.Add("otp", otp);
                    parameters.Add("path", path);
                    parameters.Add("privs", privs);
                    parameters.Add("realm", realm);
                    return _client.Create($"/access/ticket", parameters);
                }

                /// <summary>
                /// Create or verify authentication ticket.
                /// </summary>
                /// <param name="password">The secret password. This can also be a valid ticket.</param>
                /// <param name="username">User name</param>
                /// <param name="otp">One-time password for Two-factor authentication.</param>
                /// <param name="path">Verify ticket, and check if user have access 'privs' on 'path'</param>
                /// <param name="privs">Verify ticket, and check if user have access 'privs' on 'path'</param>
                /// <param name="realm">You can optionally pass the realm using this parameter. Normally the realm is simply added to the username &amp;lt;username&amp;gt;@&amp;lt;relam&amp;gt;.</param>
                /// <returns></returns>
                public Result CreateTicket(string password, string username, string otp = null, string path = null, string privs = null, string realm = null) => CreateRest(password, username, otp, path, privs, realm);
            }
            public class PVEPassword
            {
                private readonly Client _client;

                internal PVEPassword(Client client) { _client = client; }
                /// <summary>
                /// Change user password.
                /// </summary>
                /// <param name="password">The new password.</param>
                /// <param name="userid">User ID</param>
                /// <returns></returns>
                public Result SetRest(string password, string userid)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("password", password);
                    parameters.Add("userid", userid);
                    return _client.Set($"/access/password", parameters);
                }

                /// <summary>
                /// Change user password.
                /// </summary>
                /// <param name="password">The new password.</param>
                /// <param name="userid">User ID</param>
                /// <returns></returns>
                public Result ChangePassword(string password, string userid) => SetRest(password, userid);
            }
            public class PVETfa
            {
                private readonly Client _client;

                internal PVETfa(Client client) { _client = client; }
                /// <summary>
                /// Finish a u2f challenge.
                /// </summary>
                /// <param name="response">The response to the current authentication challenge.</param>
                /// <returns></returns>
                public Result CreateRest(string response)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("response", response);
                    return _client.Create($"/access/tfa", parameters);
                }

                /// <summary>
                /// Finish a u2f challenge.
                /// </summary>
                /// <param name="response">The response to the current authentication challenge.</param>
                /// <returns></returns>
                public Result VerifyTfa(string response) => CreateRest(response);
                /// <summary>
                /// Change user u2f authentication.
                /// </summary>
                /// <param name="action">The action to perform
                ///   Enum: delete,new,confirm</param>
                /// <param name="userid">User ID</param>
                /// <param name="config">A TFA configuration. This must currently be of type TOTP of not set at all.</param>
                /// <param name="key">When adding TOTP, the shared secret value.</param>
                /// <param name="password">The current password.</param>
                /// <param name="response">Either the the response to the current u2f registration challenge, or, when adding TOTP, the currently valid TOTP value.</param>
                /// <returns></returns>
                public Result SetRest(string action, string userid, string config = null, string key = null, string password = null, string response = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("action", action);
                    parameters.Add("userid", userid);
                    parameters.Add("config", config);
                    parameters.Add("key", key);
                    parameters.Add("password", password);
                    parameters.Add("response", response);
                    return _client.Set($"/access/tfa", parameters);
                }

                /// <summary>
                /// Change user u2f authentication.
                /// </summary>
                /// <param name="action">The action to perform
                ///   Enum: delete,new,confirm</param>
                /// <param name="userid">User ID</param>
                /// <param name="config">A TFA configuration. This must currently be of type TOTP of not set at all.</param>
                /// <param name="key">When adding TOTP, the shared secret value.</param>
                /// <param name="password">The current password.</param>
                /// <param name="response">Either the the response to the current u2f registration challenge, or, when adding TOTP, the currently valid TOTP value.</param>
                /// <returns></returns>
                public Result ChangeTfa(string action, string userid, string config = null, string key = null, string password = null, string response = null) => SetRest(action, userid, config, key, password, response);
            }
            /// <summary>
            /// Directory index.
            /// </summary>
            /// <returns></returns>
            public Result GetRest() { return _client.Get($"/access"); }

            /// <summary>
            /// Directory index.
            /// </summary>
            /// <returns></returns>
            public Result Index() => GetRest();
        }
        public class PVEPools
        {
            private readonly Client _client;

            internal PVEPools(Client client) { _client = client; }
            public PVEItemPoolid this[object poolid] => new PVEItemPoolid(_client, poolid);
            public class PVEItemPoolid
            {
                private readonly Client _client;
                private readonly object _poolid;
                internal PVEItemPoolid(Client client, object poolid) { _client = client; _poolid = poolid; }
                /// <summary>
                /// Delete pool.
                /// </summary>
                /// <returns></returns>
                public Result DeleteRest() { return _client.Delete($"/pools/{_poolid}"); }

                /// <summary>
                /// Delete pool.
                /// </summary>
                /// <returns></returns>
                public Result DeletePool() => DeleteRest();
                /// <summary>
                /// Get pool configuration.
                /// </summary>
                /// <returns></returns>
                public Result GetRest() { return _client.Get($"/pools/{_poolid}"); }

                /// <summary>
                /// Get pool configuration.
                /// </summary>
                /// <returns></returns>
                public Result ReadPool() => GetRest();
                /// <summary>
                /// Update pool data.
                /// </summary>
                /// <param name="comment"></param>
                /// <param name="delete">Remove vms/storage (instead of adding it).</param>
                /// <param name="storage">List of storage IDs.</param>
                /// <param name="vms">List of virtual machines.</param>
                /// <returns></returns>
                public Result SetRest(string comment = null, bool? delete = null, string storage = null, string vms = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("comment", comment);
                    parameters.Add("delete", delete);
                    parameters.Add("storage", storage);
                    parameters.Add("vms", vms);
                    return _client.Set($"/pools/{_poolid}", parameters);
                }

                /// <summary>
                /// Update pool data.
                /// </summary>
                /// <param name="comment"></param>
                /// <param name="delete">Remove vms/storage (instead of adding it).</param>
                /// <param name="storage">List of storage IDs.</param>
                /// <param name="vms">List of virtual machines.</param>
                /// <returns></returns>
                public Result UpdatePool(string comment = null, bool? delete = null, string storage = null, string vms = null) => SetRest(comment, delete, storage, vms);
            }
            /// <summary>
            /// Pool index.
            /// </summary>
            /// <returns></returns>
            public Result GetRest() { return _client.Get($"/pools"); }

            /// <summary>
            /// Pool index.
            /// </summary>
            /// <returns></returns>
            public Result Index() => GetRest();
            /// <summary>
            /// Create new pool.
            /// </summary>
            /// <param name="poolid"></param>
            /// <param name="comment"></param>
            /// <returns></returns>
            public Result CreateRest(string poolid, string comment = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("poolid", poolid);
                parameters.Add("comment", comment);
                return _client.Create($"/pools", parameters);
            }

            /// <summary>
            /// Create new pool.
            /// </summary>
            /// <param name="poolid"></param>
            /// <param name="comment"></param>
            /// <returns></returns>
            public Result CreatePool(string poolid, string comment = null) => CreateRest(poolid, comment);
        }
        public class PVEVersion
        {
            private readonly Client _client;

            internal PVEVersion(Client client) { _client = client; }
            /// <summary>
            /// API version details. The result also includes the global datacenter confguration.
            /// </summary>
            /// <returns></returns>
            public Result GetRest() { return _client.Get($"/version"); }

            /// <summary>
            /// API version details. The result also includes the global datacenter confguration.
            /// </summary>
            /// <returns></returns>
            public Result Version() => GetRest();
        }

    }
}