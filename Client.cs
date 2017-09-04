using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Dynamic;
using System.Web;

namespace EnterpriseVE.ProxmoxVE.Api
{
#pragma warning disable 1591

    /// <summary>
    /// ProxmocVE Client
    /// </summary>
    public class Client
    {
        private string _ticketCSRFPreventionToken;
        private string _ticketPVEAuthCookie;
        private Client _client;
        private string _baseUrl;

        public Client(string hostName, int port = 8006)
        {
            _client = this;
            HostName = hostName;
            Port = port;
            _baseUrl = "https://" + hostName + ":" + port + "/api2/json";
        }

        public string HostName { get; private set; }

        public int Port { get; private set; }

        public bool ThrowExceptionNoSuccess { get; set; } = false;

        /// <summary>
        /// Convert object to JSON.
        /// </summary>
        /// <param name="obj"></param>
        public static string ObjectToJson(object obj) { return JsonConvert.SerializeObject(obj, Formatting.Indented); }

        /// <summary>
        /// Creation ticket from login.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="realm"></param>
        public bool Login(string userName, string password, string realm = "pam")
        {
            dynamic ticket = Access.Ticket.CreateTicket(username: userName, password: password, realm: realm);
            _ticketCSRFPreventionToken = ticket.data.CSRFPreventionToken;
            _ticketPVEAuthCookie = ticket.data.ticket;
            return ticket != null;
        }

        private ExpandoObject Execute(string resource, HttpMethod method, IDictionary<string, object> parameters = null)
        {
            using (var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            })
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(_baseUrl);

                //load parameters
                var parms = new Dictionary<string, string>();
                if (parameters != null)
                {
                    foreach (var parameter in parameters.Where(a => a.Value != null))
                    {
                        var value = parameter.Value;
                        if (value is bool) { value = ((bool)value) ? 1 : 0; }
                        parms.Add(parameter.Key, HttpUtility.UrlEncode(value.ToString()));
                    }
                }

                var uriString = _baseUrl + resource;
                if (method == HttpMethod.Get && parms.Count > 0)
                {
                    uriString += "?" + string.Join("&", (from a in parms
                                                         select $"{a.Key}={a.Value}"));
                }

                var request = new HttpRequestMessage(method, new Uri(uriString));
                if (method != HttpMethod.Get) { request.Content = new FormUrlEncodedContent(parms); }

                //tiket login
                if (_ticketCSRFPreventionToken != null)
                {
                    handler.CookieContainer.Add(request.RequestUri, new Cookie("PVEAuthCookie", _ticketPVEAuthCookie));
                    request.Headers.Add("CSRFPreventionToken", _ticketCSRFPreventionToken);
                }

                var response = client.SendAsync(request).Result;
                if (ThrowExceptionNoSuccess && !response.IsSuccessStatusCode) { throw new Exception(response.ReasonPhrase); }

                var stringContent = response.Content.ReadAsStringAsync().Result;
                dynamic result = JsonConvert.DeserializeObject<ExpandoObject>(stringContent);

                //check in error
                result.InError = ((IDictionary<String, object>)result).ContainsKey("errors");

                return result;
            }
        }

        private static void AddComplexParmeterToDictionary(Dictionary<string, object> parameters, string name, IDictionary<int, string> value)
        {
            if (value == null) { return; }
            foreach (var item in value) { parameters.Add(name + item.Key, item.Value); }
        }

        private PVECluster _cluster;
        public PVECluster Cluster { get { return _cluster ?? (_cluster = new PVECluster(_client)); } }

        public class PVECluster
        {
            private Client _client;
            internal PVECluster(Client client) { _client = client; }

            /// <summary>
            /// Cluster index.
            /// </summary>
            public ExpandoObject Index() { return _client.Execute($"/cluster", HttpMethod.Get); }

            private PVEReplication _replication;
            public PVEReplication Replication { get { return _replication ?? (_replication = new PVEReplication(_client)); } }

            public class PVEReplication
            {
                private Client _client;
                internal PVEReplication(Client client) { _client = client; }

                /// <summary>
                /// List replication jobs.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/cluster/replication", HttpMethod.Get); }

                /// <summary>
                /// Create a new replication job
                /// </summary>
                /// <param name="id">Replication Job ID. The ID is composed of a Guest ID and a job number, separated by a hyphen, i.e. '&amp;lt;GUEST>-&amp;lt;JOBNUM>'.</param>
                /// <param name="target">Target node.</param>
                /// <param name="type">Section type.
                ///   Enum: local</param>
                /// <param name="comment">Description.</param>
                /// <param name="disable">Flag to disable/deactivate the entry.</param>
                /// <param name="rate">Rate limit in mbps (megabytes per second) as floating point number.</param>
                /// <param name="remove_job">Mark the replication job for removal. The job will remove all local replication snapshots. When set to 'full', it also tries to remove replicated volumes on the target. The job then removes itself from the configuration file.
                ///   Enum: local,full</param>
                /// <param name="schedule">Storage replication schedule. The format is a subset of `systemd` calender events.</param>
                public void Create(string id, string target, string type, string comment = null, bool? disable = null, int? rate = null, string remove_job = null, string schedule = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("comment", comment);
                    parameters.Add("disable", disable);
                    parameters.Add("id", id);
                    parameters.Add("rate", rate);
                    parameters.Add("remove_job", remove_job);
                    parameters.Add("schedule", schedule);
                    parameters.Add("target", target);
                    parameters.Add("type", type);
                    _client.Execute($"/cluster/replication", HttpMethod.Post, parameters);
                }

                public PVEItemId this[object id] { get { return new PVEItemId(_client, id: id); } }

                public class PVEItemId
                {
                    private Client _client;
                    private object _id;
                    internal PVEItemId(Client client, object id)
                    {
                        _client = client;
                        _id = id;
                    }

                    /// <summary>
                    /// Mark replication job for removal.
                    /// </summary>
                    /// <param name="force">Will remove the jobconfig entry, but will not cleanup.</param>
                    /// <param name="keep">Keep replicated data at target (do not remove).</param>
                    public void Delete(bool? force = null, bool? keep = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        parameters.Add("keep", keep);
                        _client.Execute($"/cluster/replication/{_id}", HttpMethod.Delete, parameters);
                    }

                    /// <summary>
                    /// Read replication job configuration.
                    /// </summary>
                    public ExpandoObject Read() { return _client.Execute($"/cluster/replication/{_id}", HttpMethod.Get); }

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
                    public void Update(string comment = null, string delete = null, string digest = null, bool? disable = null, int? rate = null, string remove_job = null, string schedule = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        parameters.Add("delete", delete);
                        parameters.Add("digest", digest);
                        parameters.Add("disable", disable);
                        parameters.Add("rate", rate);
                        parameters.Add("remove_job", remove_job);
                        parameters.Add("schedule", schedule);
                        _client.Execute($"/cluster/replication/{_id}", HttpMethod.Put, parameters);
                    }
                }
            }

            private PVEConfig _config;
            public PVEConfig Config { get { return _config ?? (_config = new PVEConfig(_client)); } }

            public class PVEConfig
            {
                private Client _client;
                internal PVEConfig(Client client) { _client = client; }

                /// <summary>
                /// Directory index.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/cluster/config", HttpMethod.Get); }

                private PVENodes _nodes;
                public PVENodes Nodes { get { return _nodes ?? (_nodes = new PVENodes(_client)); } }

                public class PVENodes
                {
                    private Client _client;
                    internal PVENodes(Client client) { _client = client; }

                    /// <summary>
                    /// Corosync node list.
                    /// </summary>
                    public ExpandoObject Nodes() { return _client.Execute($"/cluster/config/nodes", HttpMethod.Get); }
                }

                private PVETotem _totem;
                public PVETotem Totem { get { return _totem ?? (_totem = new PVETotem(_client)); } }

                public class PVETotem
                {
                    private Client _client;
                    internal PVETotem(Client client) { _client = client; }

                    /// <summary>
                    /// Get corosync totem protocol settings.
                    /// </summary>
                    public ExpandoObject Totem() { return _client.Execute($"/cluster/config/totem", HttpMethod.Get); }
                }
            }

            private PVEFirewall _firewall;
            public PVEFirewall Firewall { get { return _firewall ?? (_firewall = new PVEFirewall(_client)); } }

            public class PVEFirewall
            {
                private Client _client;
                internal PVEFirewall(Client client) { _client = client; }

                /// <summary>
                /// Directory index.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/cluster/firewall", HttpMethod.Get); }

                private PVEGroups _groups;
                public PVEGroups Groups { get { return _groups ?? (_groups = new PVEGroups(_client)); } }

                public class PVEGroups
                {
                    private Client _client;
                    internal PVEGroups(Client client) { _client = client; }

                    /// <summary>
                    /// List security groups.
                    /// </summary>
                    public ExpandoObject ListSecurityGroups() { return _client.Execute($"/cluster/firewall/groups", HttpMethod.Get); }

                    /// <summary>
                    /// Create new security group.
                    /// </summary>
                    /// <param name="group">Security Group name.</param>
                    /// <param name="comment"></param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="rename">Rename/update an existing security group. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing group.</param>
                    public void CreateSecurityGroup(string group, string comment = null, string digest = null, string rename = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        parameters.Add("digest", digest);
                        parameters.Add("group", group);
                        parameters.Add("rename", rename);
                        _client.Execute($"/cluster/firewall/groups", HttpMethod.Post, parameters);
                    }

                    public PVEItemGroup this[object group] { get { return new PVEItemGroup(_client, group: group); } }

                    public class PVEItemGroup
                    {
                        private Client _client;
                        private object _group;
                        internal PVEItemGroup(Client client, object group)
                        {
                            _client = client;
                            _group = group;
                        }

                        /// <summary>
                        /// Delete security group.
                        /// </summary>
                        public void DeleteSecurityGroup() { _client.Execute($"/cluster/firewall/groups/{_group}", HttpMethod.Delete); }

                        /// <summary>
                        /// List rules.
                        /// </summary>
                        public ExpandoObject GetRules() { return _client.Execute($"/cluster/firewall/groups/{_group}", HttpMethod.Get); }

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
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos>.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        public void CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("comment", comment);
                            parameters.Add("dest", dest);
                            parameters.Add("digest", digest);
                            parameters.Add("dport", dport);
                            parameters.Add("enable", enable);
                            parameters.Add("iface", iface);
                            parameters.Add("macro", macro);
                            parameters.Add("pos", pos);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            parameters.Add("type", type);
                            _client.Execute($"/cluster/firewall/groups/{_group}", HttpMethod.Post, parameters);
                        }

                        public PVEItemPos this[object pos] { get { return new PVEItemPos(_client, group: _group, pos: pos); } }

                        public class PVEItemPos
                        {
                            private Client _client;
                            private object _group;
                            private object _pos;
                            internal PVEItemPos(Client client, object group, object pos)
                            {
                                _client = client;
                                _group = group;
                                _pos = pos;
                            }

                            /// <summary>
                            /// Delete rule.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            public void DeleteRule(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                _client.Execute($"/cluster/firewall/groups/{_group}/{_pos}", HttpMethod.Delete, parameters);
                            }

                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            public ExpandoObject GetRule() { return _client.Execute($"/cluster/firewall/groups/{_group}/{_pos}", HttpMethod.Get); }

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
                            /// <param name="macro">Use predefined standard macro.</param>
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto>. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            public void UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                                parameters.Add("macro", macro);
                                parameters.Add("moveto", moveto);
                                parameters.Add("proto", proto);
                                parameters.Add("source", source);
                                parameters.Add("sport", sport);
                                parameters.Add("type", type);
                                _client.Execute($"/cluster/firewall/groups/{_group}/{_pos}", HttpMethod.Put, parameters);
                            }
                        }
                    }
                }

                private PVERules _rules;
                public PVERules Rules { get { return _rules ?? (_rules = new PVERules(_client)); } }

                public class PVERules
                {
                    private Client _client;
                    internal PVERules(Client client) { _client = client; }

                    /// <summary>
                    /// List rules.
                    /// </summary>
                    public ExpandoObject GetRules() { return _client.Execute($"/cluster/firewall/rules", HttpMethod.Get); }

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
                    /// <param name="macro">Use predefined standard macro.</param>
                    /// <param name="pos">Update rule at position &amp;lt;pos>.</param>
                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    public void CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("action", action);
                        parameters.Add("comment", comment);
                        parameters.Add("dest", dest);
                        parameters.Add("digest", digest);
                        parameters.Add("dport", dport);
                        parameters.Add("enable", enable);
                        parameters.Add("iface", iface);
                        parameters.Add("macro", macro);
                        parameters.Add("pos", pos);
                        parameters.Add("proto", proto);
                        parameters.Add("source", source);
                        parameters.Add("sport", sport);
                        parameters.Add("type", type);
                        _client.Execute($"/cluster/firewall/rules", HttpMethod.Post, parameters);
                    }

                    public PVEItemPos this[object pos] { get { return new PVEItemPos(_client, pos: pos); } }

                    public class PVEItemPos
                    {
                        private Client _client;
                        private object _pos;
                        internal PVEItemPos(Client client, object pos)
                        {
                            _client = client;
                            _pos = pos;
                        }

                        /// <summary>
                        /// Delete rule.
                        /// </summary>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        public void DeleteRule(string digest = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("digest", digest);
                            _client.Execute($"/cluster/firewall/rules/{_pos}", HttpMethod.Delete, parameters);
                        }

                        /// <summary>
                        /// Get single rule data.
                        /// </summary>
                        public ExpandoObject GetRule() { return _client.Execute($"/cluster/firewall/rules/{_pos}", HttpMethod.Get); }

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
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="moveto">Move rule to new position &amp;lt;moveto>. Other arguments are ignored.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        public void UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                            parameters.Add("macro", macro);
                            parameters.Add("moveto", moveto);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            parameters.Add("type", type);
                            _client.Execute($"/cluster/firewall/rules/{_pos}", HttpMethod.Put, parameters);
                        }
                    }
                }

                private PVEIpset _ipset;
                public PVEIpset Ipset { get { return _ipset ?? (_ipset = new PVEIpset(_client)); } }

                public class PVEIpset
                {
                    private Client _client;
                    internal PVEIpset(Client client) { _client = client; }

                    /// <summary>
                    /// List IPSets
                    /// </summary>
                    public ExpandoObject IpsetIndex() { return _client.Execute($"/cluster/firewall/ipset", HttpMethod.Get); }

                    /// <summary>
                    /// Create new IPSet
                    /// </summary>
                    /// <param name="name">IP set name.</param>
                    /// <param name="comment"></param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                    public void CreateIpset(string name, string comment = null, string digest = null, string rename = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        parameters.Add("digest", digest);
                        parameters.Add("name", name);
                        parameters.Add("rename", rename);
                        _client.Execute($"/cluster/firewall/ipset", HttpMethod.Post, parameters);
                    }

                    public PVEItemName this[object name] { get { return new PVEItemName(_client, name: name); } }

                    public class PVEItemName
                    {
                        private Client _client;
                        private object _name;
                        internal PVEItemName(Client client, object name)
                        {
                            _client = client;
                            _name = name;
                        }

                        /// <summary>
                        /// Delete IPSet
                        /// </summary>
                        public void DeleteIpset() { _client.Execute($"/cluster/firewall/ipset/{_name}", HttpMethod.Delete); }

                        /// <summary>
                        /// List IPSet content
                        /// </summary>
                        public ExpandoObject GetIpset() { return _client.Execute($"/cluster/firewall/ipset/{_name}", HttpMethod.Get); }

                        /// <summary>
                        /// Add IP or Network to IPSet.
                        /// </summary>
                        /// <param name="cidr">Network/IP specification in CIDR format.</param>
                        /// <param name="comment"></param>
                        /// <param name="nomatch"></param>
                        public void CreateIp(string cidr, string comment = null, bool? nomatch = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("cidr", cidr);
                            parameters.Add("comment", comment);
                            parameters.Add("nomatch", nomatch);
                            _client.Execute($"/cluster/firewall/ipset/{_name}", HttpMethod.Post, parameters);
                        }

                        public PVEItemCidr this[object cidr] { get { return new PVEItemCidr(_client, name: _name, cidr: cidr); } }

                        public class PVEItemCidr
                        {
                            private Client _client;
                            private object _name;
                            private object _cidr;
                            internal PVEItemCidr(Client client, object name, object cidr)
                            {
                                _client = client;
                                _name = name;
                                _cidr = cidr;
                            }

                            /// <summary>
                            /// Remove IP or Network from IPSet.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            public void RemoveIp(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                _client.Execute($"/cluster/firewall/ipset/{_name}/{_cidr}", HttpMethod.Delete, parameters);
                            }

                            /// <summary>
                            /// Read IP or Network settings from IPSet.
                            /// </summary>
                            public ExpandoObject ReadIp() { return _client.Execute($"/cluster/firewall/ipset/{_name}/{_cidr}", HttpMethod.Get); }

                            /// <summary>
                            /// Update IP or Network settings
                            /// </summary>
                            /// <param name="comment"></param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="nomatch"></param>
                            public void UpdateIp(string comment = null, string digest = null, bool? nomatch = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("comment", comment);
                                parameters.Add("digest", digest);
                                parameters.Add("nomatch", nomatch);
                                _client.Execute($"/cluster/firewall/ipset/{_name}/{_cidr}", HttpMethod.Put, parameters);
                            }
                        }
                    }
                }

                private PVEAliases _aliases;
                public PVEAliases Aliases { get { return _aliases ?? (_aliases = new PVEAliases(_client)); } }

                public class PVEAliases
                {
                    private Client _client;
                    internal PVEAliases(Client client) { _client = client; }

                    /// <summary>
                    /// List aliases
                    /// </summary>
                    public ExpandoObject GetAliases() { return _client.Execute($"/cluster/firewall/aliases", HttpMethod.Get); }

                    /// <summary>
                    /// Create IP or Network Alias.
                    /// </summary>
                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                    /// <param name="name">Alias name.</param>
                    /// <param name="comment"></param>
                    public void CreateAlias(string cidr, string name, string comment = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cidr", cidr);
                        parameters.Add("comment", comment);
                        parameters.Add("name", name);
                        _client.Execute($"/cluster/firewall/aliases", HttpMethod.Post, parameters);
                    }

                    public PVEItemName this[object name] { get { return new PVEItemName(_client, name: name); } }

                    public class PVEItemName
                    {
                        private Client _client;
                        private object _name;
                        internal PVEItemName(Client client, object name)
                        {
                            _client = client;
                            _name = name;
                        }

                        /// <summary>
                        /// Remove IP or Network alias.
                        /// </summary>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        public void RemoveAlias(string digest = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("digest", digest);
                            _client.Execute($"/cluster/firewall/aliases/{_name}", HttpMethod.Delete, parameters);
                        }

                        /// <summary>
                        /// Read alias.
                        /// </summary>
                        public ExpandoObject ReadAlias() { return _client.Execute($"/cluster/firewall/aliases/{_name}", HttpMethod.Get); }

                        /// <summary>
                        /// Update IP or Network alias.
                        /// </summary>
                        /// <param name="cidr">Network/IP specification in CIDR format.</param>
                        /// <param name="comment"></param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="rename">Rename an existing alias.</param>
                        public void UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("cidr", cidr);
                            parameters.Add("comment", comment);
                            parameters.Add("digest", digest);
                            parameters.Add("rename", rename);
                            _client.Execute($"/cluster/firewall/aliases/{_name}", HttpMethod.Put, parameters);
                        }
                    }
                }

                private PVEOptions _options;
                public PVEOptions Options { get { return _options ?? (_options = new PVEOptions(_client)); } }

                public class PVEOptions
                {
                    private Client _client;
                    internal PVEOptions(Client client) { _client = client; }

                    /// <summary>
                    /// Get Firewall options.
                    /// </summary>
                    public ExpandoObject GetOptions() { return _client.Execute($"/cluster/firewall/options", HttpMethod.Get); }

                    /// <summary>
                    /// Set Firewall options.
                    /// </summary>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="enable">Enable or disable the firewall cluster wide.</param>
                    /// <param name="policy_in">Input policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <param name="policy_out">Output policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    public void SetOptions(string delete = null, string digest = null, int? enable = null, string policy_in = null, string policy_out = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("delete", delete);
                        parameters.Add("digest", digest);
                        parameters.Add("enable", enable);
                        parameters.Add("policy_in", policy_in);
                        parameters.Add("policy_out", policy_out);
                        _client.Execute($"/cluster/firewall/options", HttpMethod.Put, parameters);
                    }
                }

                private PVEMacros _macros;
                public PVEMacros Macros { get { return _macros ?? (_macros = new PVEMacros(_client)); } }

                public class PVEMacros
                {
                    private Client _client;
                    internal PVEMacros(Client client) { _client = client; }

                    /// <summary>
                    /// List available macros
                    /// </summary>
                    public ExpandoObject GetMacros() { return _client.Execute($"/cluster/firewall/macros", HttpMethod.Get); }
                }

                private PVERefs _refs;
                public PVERefs Refs { get { return _refs ?? (_refs = new PVERefs(_client)); } }

                public class PVERefs
                {
                    private Client _client;
                    internal PVERefs(Client client) { _client = client; }

                    /// <summary>
                    /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                    /// </summary>
                    /// <param name="type">Only list references of specified type.
                    ///   Enum: alias,ipset</param>
                    public ExpandoObject Refs(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return _client.Execute($"/cluster/firewall/refs", HttpMethod.Get, parameters);
                    }
                }
            }

            private PVEBackup _backup;
            public PVEBackup Backup { get { return _backup ?? (_backup = new PVEBackup(_client)); } }

            public class PVEBackup
            {
                private Client _client;
                internal PVEBackup(Client client) { _client = client; }

                /// <summary>
                /// List vzdump backup schedule.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/cluster/backup", HttpMethod.Get); }

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
                /// <param name="pigz">Use pigz instead of gzip when N>0. N=1 uses half of cores, N>1 uses N as thread count.</param>
                /// <param name="quiet">Be quiet.</param>
                /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                /// <param name="script">Use specified hook script.</param>
                /// <param name="size">Unused, will be removed in a future release.</param>
                /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                /// <param name="stop">Stop runnig backup jobs on this host.</param>
                /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                /// <param name="storage">Store resulting file to this storage.</param>
                /// <param name="tmpdir">Store temporary files to specified directory.</param>
                /// <param name="vmid">The ID of the guest system you want to backup.</param>
                public void CreateJob(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
                {
                    var parameters = new Dictionary<string, object>();
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
                    parameters.Add("quiet", quiet);
                    parameters.Add("remove", remove);
                    parameters.Add("script", script);
                    parameters.Add("size", size);
                    parameters.Add("starttime", starttime);
                    parameters.Add("stdexcludes", stdexcludes);
                    parameters.Add("stop", stop);
                    parameters.Add("stopwait", stopwait);
                    parameters.Add("storage", storage);
                    parameters.Add("tmpdir", tmpdir);
                    parameters.Add("vmid", vmid);
                    _client.Execute($"/cluster/backup", HttpMethod.Post, parameters);
                }

                public PVEItemId this[object id] { get { return new PVEItemId(_client, id: id); } }

                public class PVEItemId
                {
                    private Client _client;
                    private object _id;
                    internal PVEItemId(Client client, object id)
                    {
                        _client = client;
                        _id = id;
                    }

                    /// <summary>
                    /// Delete vzdump backup job definition.
                    /// </summary>
                    public void DeleteJob() { _client.Execute($"/cluster/backup/{_id}", HttpMethod.Delete); }

                    /// <summary>
                    /// Read vzdump backup job definition.
                    /// </summary>
                    public ExpandoObject ReadJob() { return _client.Execute($"/cluster/backup/{_id}", HttpMethod.Get); }

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
                    /// <param name="pigz">Use pigz instead of gzip when N>0. N=1 uses half of cores, N>1 uses N as thread count.</param>
                    /// <param name="quiet">Be quiet.</param>
                    /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                    /// <param name="script">Use specified hook script.</param>
                    /// <param name="size">Unused, will be removed in a future release.</param>
                    /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                    /// <param name="stop">Stop runnig backup jobs on this host.</param>
                    /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                    /// <param name="storage">Store resulting file to this storage.</param>
                    /// <param name="tmpdir">Store temporary files to specified directory.</param>
                    /// <param name="vmid">The ID of the guest system you want to backup.</param>
                    public void UpdateJob(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string delete = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
                    {
                        var parameters = new Dictionary<string, object>();
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
                        parameters.Add("quiet", quiet);
                        parameters.Add("remove", remove);
                        parameters.Add("script", script);
                        parameters.Add("size", size);
                        parameters.Add("starttime", starttime);
                        parameters.Add("stdexcludes", stdexcludes);
                        parameters.Add("stop", stop);
                        parameters.Add("stopwait", stopwait);
                        parameters.Add("storage", storage);
                        parameters.Add("tmpdir", tmpdir);
                        parameters.Add("vmid", vmid);
                        _client.Execute($"/cluster/backup/{_id}", HttpMethod.Put, parameters);
                    }
                }
            }

            private PVEHa _ha;
            public PVEHa Ha { get { return _ha ?? (_ha = new PVEHa(_client)); } }

            public class PVEHa
            {
                private Client _client;
                internal PVEHa(Client client) { _client = client; }

                /// <summary>
                /// Directory index.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/cluster/ha", HttpMethod.Get); }

                private PVEResources _resources;
                public PVEResources Resources { get { return _resources ?? (_resources = new PVEResources(_client)); } }

                public class PVEResources
                {
                    private Client _client;
                    internal PVEResources(Client client) { _client = client; }

                    /// <summary>
                    /// List HA resources.
                    /// </summary>
                    /// <param name="type">Only list resources of specific type
                    ///   Enum: ct,vm</param>
                    public ExpandoObject Index(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return _client.Execute($"/cluster/ha/resources", HttpMethod.Get, parameters);
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
                    ///   Enum: started,stopped,enabled,disabled</param>
                    /// <param name="type">Resource type.
                    ///   Enum: ct,vm</param>
                    public void Create(string sid, string comment = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null, string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        parameters.Add("group", group);
                        parameters.Add("max_relocate", max_relocate);
                        parameters.Add("max_restart", max_restart);
                        parameters.Add("sid", sid);
                        parameters.Add("state", state);
                        parameters.Add("type", type);
                        _client.Execute($"/cluster/ha/resources", HttpMethod.Post, parameters);
                    }

                    public PVEItemSid this[object sid] { get { return new PVEItemSid(_client, sid: sid); } }

                    public class PVEItemSid
                    {
                        private Client _client;
                        private object _sid;
                        internal PVEItemSid(Client client, object sid)
                        {
                            _client = client;
                            _sid = sid;
                        }

                        /// <summary>
                        /// Delete resource configuration.
                        /// </summary>
                        public void Delete() { _client.Execute($"/cluster/ha/resources/{_sid}", HttpMethod.Delete); }

                        /// <summary>
                        /// Read resource configuration.
                        /// </summary>
                        public ExpandoObject Read() { return _client.Execute($"/cluster/ha/resources/{_sid}", HttpMethod.Get); }

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
                        ///   Enum: started,stopped,enabled,disabled</param>
                        public void Update(string comment = null, string delete = null, string digest = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("group", group);
                            parameters.Add("max_relocate", max_relocate);
                            parameters.Add("max_restart", max_restart);
                            parameters.Add("state", state);
                            _client.Execute($"/cluster/ha/resources/{_sid}", HttpMethod.Put, parameters);
                        }

                        private PVEMigrate _migrate;
                        public PVEMigrate Migrate { get { return _migrate ?? (_migrate = new PVEMigrate(_client, sid: _sid)); } }

                        public class PVEMigrate
                        {
                            private Client _client;
                            private object _sid;
                            internal PVEMigrate(Client client, object sid)
                            {
                                _client = client;
                                _sid = sid;
                            }

                            /// <summary>
                            /// Request resource migration (online) to another node.
                            /// </summary>
                            /// <param name="node">The cluster node name.</param>
                            public void Migrate(string node)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("node", node);
                                _client.Execute($"/cluster/ha/resources/{_sid}/migrate", HttpMethod.Post, parameters);
                            }
                        }

                        private PVERelocate _relocate;
                        public PVERelocate Relocate { get { return _relocate ?? (_relocate = new PVERelocate(_client, sid: _sid)); } }

                        public class PVERelocate
                        {
                            private Client _client;
                            private object _sid;
                            internal PVERelocate(Client client, object sid)
                            {
                                _client = client;
                                _sid = sid;
                            }

                            /// <summary>
                            /// Request resource relocatzion to another node. This stops the service on the old node, and restarts it on the target node.
                            /// </summary>
                            /// <param name="node">The cluster node name.</param>
                            public void Relocate(string node)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("node", node);
                                _client.Execute($"/cluster/ha/resources/{_sid}/relocate", HttpMethod.Post, parameters);
                            }
                        }
                    }
                }

                private PVEGroups _groups;
                public PVEGroups Groups { get { return _groups ?? (_groups = new PVEGroups(_client)); } }

                public class PVEGroups
                {
                    private Client _client;
                    internal PVEGroups(Client client) { _client = client; }

                    /// <summary>
                    /// Get HA groups.
                    /// </summary>
                    public ExpandoObject Index() { return _client.Execute($"/cluster/ha/groups", HttpMethod.Get); }

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
                    public void Create(string group, string nodes, string comment = null, bool? nofailback = null, bool? restricted = null, string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        parameters.Add("group", group);
                        parameters.Add("nodes", nodes);
                        parameters.Add("nofailback", nofailback);
                        parameters.Add("restricted", restricted);
                        parameters.Add("type", type);
                        _client.Execute($"/cluster/ha/groups", HttpMethod.Post, parameters);
                    }

                    public PVEItemGroup this[object group] { get { return new PVEItemGroup(_client, group: group); } }

                    public class PVEItemGroup
                    {
                        private Client _client;
                        private object _group;
                        internal PVEItemGroup(Client client, object group)
                        {
                            _client = client;
                            _group = group;
                        }

                        /// <summary>
                        /// Delete ha group configuration.
                        /// </summary>
                        public void Delete() { _client.Execute($"/cluster/ha/groups/{_group}", HttpMethod.Delete); }

                        /// <summary>
                        /// Read ha group configuration.
                        /// </summary>
                        public ExpandoObject Read() { return _client.Execute($"/cluster/ha/groups/{_group}", HttpMethod.Get); }

                        /// <summary>
                        /// Update ha group configuration.
                        /// </summary>
                        /// <param name="comment">Description.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="nodes">List of cluster node names with optional priority.</param>
                        /// <param name="nofailback">The CRM tries to run services on the node with the highest priority. If a node with higher priority comes online, the CRM migrates the service to that node. Enabling nofailback prevents that behavior.</param>
                        /// <param name="restricted">Resources bound to restricted groups may only run on nodes defined by the group.</param>
                        public void Update(string comment = null, string delete = null, string digest = null, string nodes = null, bool? nofailback = null, bool? restricted = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("nodes", nodes);
                            parameters.Add("nofailback", nofailback);
                            parameters.Add("restricted", restricted);
                            _client.Execute($"/cluster/ha/groups/{_group}", HttpMethod.Put, parameters);
                        }
                    }
                }

                private PVEStatus _status;
                public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client)); } }

                public class PVEStatus
                {
                    private Client _client;
                    internal PVEStatus(Client client) { _client = client; }

                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    public ExpandoObject Index() { return _client.Execute($"/cluster/ha/status", HttpMethod.Get); }

                    private PVECurrent _current;
                    public PVECurrent Current { get { return _current ?? (_current = new PVECurrent(_client)); } }

                    public class PVECurrent
                    {
                        private Client _client;
                        internal PVECurrent(Client client) { _client = client; }

                        /// <summary>
                        /// Get HA manger status.
                        /// </summary>
                        public ExpandoObject Status() { return _client.Execute($"/cluster/ha/status/current", HttpMethod.Get); }
                    }

                    private PVEManager_Status _manager_status;
                    public PVEManager_Status Manager_Status { get { return _manager_status ?? (_manager_status = new PVEManager_Status(_client)); } }

                    public class PVEManager_Status
                    {
                        private Client _client;
                        internal PVEManager_Status(Client client) { _client = client; }

                        /// <summary>
                        /// Get full HA manger status, including LRM status.
                        /// </summary>
                        public ExpandoObject ManagerStatus() { return _client.Execute($"/cluster/ha/status/manager_status", HttpMethod.Get); }
                    }
                }
            }

            private PVELog _log;
            public PVELog Log { get { return _log ?? (_log = new PVELog(_client)); } }

            public class PVELog
            {
                private Client _client;
                internal PVELog(Client client) { _client = client; }

                /// <summary>
                /// Read cluster log
                /// </summary>
                /// <param name="max">Maximum number of entries.</param>
                public ExpandoObject Log(int? max = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("max", max);
                    return _client.Execute($"/cluster/log", HttpMethod.Get, parameters);
                }
            }

            private PVEResources _resources;
            public PVEResources Resources { get { return _resources ?? (_resources = new PVEResources(_client)); } }

            public class PVEResources
            {
                private Client _client;
                internal PVEResources(Client client) { _client = client; }

                /// <summary>
                /// Resources index (cluster wide).
                /// </summary>
                /// <param name="type">
                ///   Enum: vm,storage,node</param>
                public ExpandoObject Resources(string type = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("type", type);
                    return _client.Execute($"/cluster/resources", HttpMethod.Get, parameters);
                }
            }

            private PVETasks _tasks;
            public PVETasks Tasks { get { return _tasks ?? (_tasks = new PVETasks(_client)); } }

            public class PVETasks
            {
                private Client _client;
                internal PVETasks(Client client) { _client = client; }

                /// <summary>
                /// List recent tasks (cluster wide).
                /// </summary>
                public ExpandoObject Tasks() { return _client.Execute($"/cluster/tasks", HttpMethod.Get); }
            }

            private PVEOptions _options;
            public PVEOptions Options { get { return _options ?? (_options = new PVEOptions(_client)); } }

            public class PVEOptions
            {
                private Client _client;
                internal PVEOptions(Client client) { _client = client; }

                /// <summary>
                /// Get datacenter options.
                /// </summary>
                public ExpandoObject GetOptions() { return _client.Execute($"/cluster/options", HttpMethod.Get); }

                /// <summary>
                /// Set datacenter options.
                /// </summary>
                /// <param name="console">Select the default Console viewer. You can either use the builtin java applet (VNC), an external virt-viewer comtatible application (SPICE), or an HTML5 based viewer (noVNC).
                ///   Enum: applet,vv,html5</param>
                /// <param name="delete">A list of settings you want to delete.</param>
                /// <param name="email_from">Specify email address to send notification from (default is root@$hostname)</param>
                /// <param name="fencing">Set the fencing mode of the HA cluster. Hardware mode needs a valid configuration of fence devices in /etc/pve/ha/fence.cfg. With both all two modes are used.  WARNING: 'hardware' and 'both' are EXPERIMENTAL &amp; WIP
                ///   Enum: watchdog,hardware,both</param>
                /// <param name="http_proxy">Specify external http proxy which is used for downloads (example: 'http://username:password@host:port/')</param>
                /// <param name="keyboard">Default keybord layout for vnc server.
                ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                /// <param name="language">Default GUI language.
                ///   Enum: en,de</param>
                /// <param name="mac_prefix">Prefix for autogenerated MAC addresses.</param>
                /// <param name="max_workers">Defines how many workers (per node) are maximal started  on actions like 'stopall VMs' or task from the ha-manager.</param>
                /// <param name="migration">For cluster wide migration settings.
                /// network CIDR of the (sub) network that is used for migration.
                /// type Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                ///   Enum: secure,insecure///</param>
                /// <param name="migration_unsecure">Migration is secure using SSH tunnel by default. For secure private networks you can disable it to speed up migration. Deprecated, use the 'migration' property instead!</param>
                public void SetOptions(string console = null, string delete = null, string email_from = null, string fencing = null, string http_proxy = null, string keyboard = null, string language = null, string mac_prefix = null, int? max_workers = null, string migration = null, bool? migration_unsecure = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("console", console);
                    parameters.Add("delete", delete);
                    parameters.Add("email_from", email_from);
                    parameters.Add("fencing", fencing);
                    parameters.Add("http_proxy", http_proxy);
                    parameters.Add("keyboard", keyboard);
                    parameters.Add("language", language);
                    parameters.Add("mac_prefix", mac_prefix);
                    parameters.Add("max_workers", max_workers);
                    parameters.Add("migration", migration);
                    parameters.Add("migration_unsecure", migration_unsecure);
                    _client.Execute($"/cluster/options", HttpMethod.Put, parameters);
                }
            }

            private PVEStatus _status;
            public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client)); } }

            public class PVEStatus
            {
                private Client _client;
                internal PVEStatus(Client client) { _client = client; }

                /// <summary>
                /// Get cluster status informations.
                /// </summary>
                public ExpandoObject GetStatus() { return _client.Execute($"/cluster/status", HttpMethod.Get); }
            }

            private PVENextid _nextid;
            public PVENextid Nextid { get { return _nextid ?? (_nextid = new PVENextid(_client)); } }

            public class PVENextid
            {
                private Client _client;
                internal PVENextid(Client client) { _client = client; }

                /// <summary>
                /// Get next free VMID. If you pass an VMID it will raise an error if the ID is already used.
                /// </summary>
                /// <param name="vmid">The (unique) ID of the VM.</param>
                public ExpandoObject Nextid(int? vmid = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("vmid", vmid);
                    return _client.Execute($"/cluster/nextid", HttpMethod.Get, parameters);
                }
            }
        }

        private PVENodes _nodes;
        public PVENodes Nodes { get { return _nodes ?? (_nodes = new PVENodes(_client)); } }

        public class PVENodes
        {
            private Client _client;
            internal PVENodes(Client client) { _client = client; }

            /// <summary>
            /// Cluster node index.
            /// </summary>
            public ExpandoObject Index() { return _client.Execute($"/nodes", HttpMethod.Get); }

            public PVEItemNode this[object node] { get { return new PVEItemNode(_client, node: node); } }

            public class PVEItemNode
            {
                private Client _client;
                private object _node;
                internal PVEItemNode(Client client, object node)
                {
                    _client = client;
                    _node = node;
                }

                /// <summary>
                /// Node index.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}", HttpMethod.Get); }

                private PVEQemu _qemu;
                public PVEQemu Qemu { get { return _qemu ?? (_qemu = new PVEQemu(_client, node: _node)); } }

                public class PVEQemu
                {
                    private Client _client;
                    private object _node;
                    internal PVEQemu(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Virtual machine index (per node).
                    /// </summary>
                    /// <param name="full">Determine the full status of active VMs.</param>
                    public ExpandoObject Vmlist(bool? full = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("full", full);
                        return _client.Execute($"/nodes/{_node}/qemu", HttpMethod.Get, parameters);
                    }

                    /// <summary>
                    /// Create or restore a virtual machine.
                    /// </summary>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="acpi">Enable/disable ACPI.</param>
                    /// <param name="agent">Enable/disable Qemu GuestAgent.</param>
                    /// <param name="archive">The backup file.</param>
                    /// <param name="args">Arbitrary arguments passed to kvm.</param>
                    /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                    /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                    /// <param name="bios">Select BIOS implementation.
                    ///   Enum: seabios,ovmf</param>
                    /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                    /// <param name="bootdisk">Enable booting from specified disk.</param>
                    /// <param name="cdrom">This is an alias for option -ide2</param>
                    /// <param name="cores">The number of cores per socket.</param>
                    /// <param name="cpu">Emulated CPU type.
                    /// cputype Emulated CPU type.
                    ///   Enum: 486,athlon,Broadwell,Broadwell-noTSX,Conroe,core2duo,coreduo,Haswell,Haswell-noTSX,host,IvyBridge,kvm32,kvm64,Nehalem,Opteron_G1,Opteron_G2,Opteron_G3,Opteron_G4,Opteron_G5,Penryn,pentium,pentium2,pentium3,phenom,qemu32,qemu64,SandyBridge,Skylake-Client,Westmere
                    /// hidden Do not identify as a KVM virtual machine.///</param>
                    /// <param name="cpulimit">Limit of CPU usage.</param>
                    /// <param name="cpuunits">CPU weight for a VM.</param>
                    /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                    /// <param name="force">Allow to overwrite existing VM.</param>
                    /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                    /// <param name="hostpciN">Map host PCI devices into guest.</param>
                    /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                    /// <param name="hugepages">Enable/disable hugepages memory.
                    ///   Enum: any,2,1024</param>
                    /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).
                    /// aio AIO type to use.
                    ///   Enum: native,threads
                    /// backup Whether the drive should be included when making backups.
                    /// bps Maximum r/w speed in bytes per second.
                    /// bps_max_length Maximum length of I/O bursts in seconds.
                    /// bps_rd Maximum read speed in bytes per second.
                    /// bps_rd_length 
                    /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                    /// bps_wr Maximum write speed in bytes per second.
                    /// bps_wr_length 
                    /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                    /// cache The drive's cache mode
                    ///   Enum: none,writethrough,writeback,unsafe,directsync
                    /// cyls Force the drive's physical geometry to have a specific cylinder count.
                    /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                    /// discard Controls whether to pass discard/trim requests to the underlying storage.
                    ///   Enum: ignore,on
                    /// file The drive's backing volume.
                    /// format The drive's backing file's data format.
                    ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                    /// heads Force the drive's physical geometry to have a specific head count.
                    /// iops Maximum r/w I/O in operations per second.
                    /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                    /// iops_max_length Maximum length of I/O bursts in seconds.
                    /// iops_rd Maximum read I/O in operations per second.
                    /// iops_rd_length 
                    /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                    /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                    /// iops_wr Maximum write I/O in operations per second.
                    /// iops_wr_length 
                    /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                    /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                    /// mbps Maximum r/w speed in megabytes per second.
                    /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                    /// mbps_rd Maximum read speed in megabytes per second.
                    /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                    /// mbps_wr Maximum write speed in megabytes per second.
                    /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                    /// media The drive's media type.
                    ///   Enum: cdrom,disk
                    /// model The drive's reported model name, url-encoded, up to 40 bytes long.
                    /// replicate Whether the drive should considered for replication jobs.
                    /// rerror Read error action.
                    ///   Enum: ignore,report,stop
                    /// secs Force the drive's physical geometry to have a specific sector count.
                    /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                    /// size Disk size. This is purely informational and has no effect.
                    /// snapshot Whether the drive should be included when making snapshots.
                    /// trans Force disk geometry bios translation mode.
                    ///   Enum: none,lba,auto
                    /// volume 
                    /// werror Write error action.
                    ///   Enum: enospc,ignore,report,stop///</param>
                    /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.conf' configuration file.
                    ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                    /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                    /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                    /// <param name="lock_">Lock/unlock the VM.
                    ///   Enum: migrate,backup,snapshot,rollback</param>
                    /// <param name="machine">Specific the Qemu machine type.</param>
                    /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                    /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                    /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                    /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                    /// <param name="netN">Specify network devices.
                    /// bridge Bridge to attach the network device to. The Proxmox VE standard bridge is called 'vmbr0'.  If you do not specify a bridge, we create a kvm user (NATed) network device, which provides DHCP and DNS services. The following addresses are used:   10.0.2.2   Gateway  10.0.2.3   DNS Server  10.0.2.4   SMB Server  The DHCP server assign addresses to the guest starting from 10.0.2.15. 
                    /// e1000 
                    /// e1000_82540em 
                    /// e1000_82544gc 
                    /// e1000_82545em 
                    /// firewall Whether this interface should be protected by the firewall.
                    /// i82551 
                    /// i82557b 
                    /// i82559er 
                    /// link_down Whether this interface should be disconnected (like pulling the plug).
                    /// macaddr MAC address. That address must be unique withing your network. This is automatically generated if not specified.
                    /// model Network Card Model. The 'virtio' model provides the best performance with very low CPU overhead. If your guest does not support this driver, it is usually best to use 'e1000'.
                    ///   Enum: rtl8139,ne2k_pci,e1000,pcnet,virtio,ne2k_isa,i82551,i82557b,i82559er,vmxnet3,e1000-82540em,e1000-82544gc,e1000-82545em
                    /// ne2k_isa 
                    /// ne2k_pci 
                    /// pcnet 
                    /// queues Number of packet queues to be used on the device.
                    /// rate Rate limit in mbps (megabytes per second) as floating point number.
                    /// rtl8139 
                    /// tag VLAN tag to apply to packets on this interface.
                    /// trunks VLAN trunks to pass through this interface.
                    /// virtio 
                    /// vmxnet3 ///</param>
                    /// <param name="numa">Enable/disable NUMA.</param>
                    /// <param name="numaN">NUMA topology.
                    /// cpus CPUs accessing this NUMA node.
                    /// hostnodes Host NUMA nodes to use.
                    /// memory Amount of memory this NUMA node provides.
                    /// policy NUMA allocation policy.
                    ///   Enum: preferred,bind,interleave///</param>
                    /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                    /// <param name="ostype">Specify guest operating system.
                    ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                    /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                    /// <param name="pool">Add the VM to the specified pool.</param>
                    /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                    /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                    /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).
                    /// aio AIO type to use.
                    ///   Enum: native,threads
                    /// backup Whether the drive should be included when making backups.
                    /// bps Maximum r/w speed in bytes per second.
                    /// bps_max_length Maximum length of I/O bursts in seconds.
                    /// bps_rd Maximum read speed in bytes per second.
                    /// bps_rd_length 
                    /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                    /// bps_wr Maximum write speed in bytes per second.
                    /// bps_wr_length 
                    /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                    /// cache The drive's cache mode
                    ///   Enum: none,writethrough,writeback,unsafe,directsync
                    /// cyls Force the drive's physical geometry to have a specific cylinder count.
                    /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                    /// discard Controls whether to pass discard/trim requests to the underlying storage.
                    ///   Enum: ignore,on
                    /// file The drive's backing volume.
                    /// format The drive's backing file's data format.
                    ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                    /// heads Force the drive's physical geometry to have a specific head count.
                    /// iops Maximum r/w I/O in operations per second.
                    /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                    /// iops_max_length Maximum length of I/O bursts in seconds.
                    /// iops_rd Maximum read I/O in operations per second.
                    /// iops_rd_length 
                    /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                    /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                    /// iops_wr Maximum write I/O in operations per second.
                    /// iops_wr_length 
                    /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                    /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                    /// mbps Maximum r/w speed in megabytes per second.
                    /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                    /// mbps_rd Maximum read speed in megabytes per second.
                    /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                    /// mbps_wr Maximum write speed in megabytes per second.
                    /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                    /// media The drive's media type.
                    ///   Enum: cdrom,disk
                    /// replicate Whether the drive should considered for replication jobs.
                    /// rerror Read error action.
                    ///   Enum: ignore,report,stop
                    /// secs Force the drive's physical geometry to have a specific sector count.
                    /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                    /// size Disk size. This is purely informational and has no effect.
                    /// snapshot Whether the drive should be included when making snapshots.
                    /// trans Force disk geometry bios translation mode.
                    ///   Enum: none,lba,auto
                    /// volume 
                    /// werror Write error action.
                    ///   Enum: enospc,ignore,report,stop///</param>
                    /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).
                    /// aio AIO type to use.
                    ///   Enum: native,threads
                    /// backup Whether the drive should be included when making backups.
                    /// bps Maximum r/w speed in bytes per second.
                    /// bps_max_length Maximum length of I/O bursts in seconds.
                    /// bps_rd Maximum read speed in bytes per second.
                    /// bps_rd_length 
                    /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                    /// bps_wr Maximum write speed in bytes per second.
                    /// bps_wr_length 
                    /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                    /// cache The drive's cache mode
                    ///   Enum: none,writethrough,writeback,unsafe,directsync
                    /// cyls Force the drive's physical geometry to have a specific cylinder count.
                    /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                    /// discard Controls whether to pass discard/trim requests to the underlying storage.
                    ///   Enum: ignore,on
                    /// file The drive's backing volume.
                    /// format The drive's backing file's data format.
                    ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                    /// heads Force the drive's physical geometry to have a specific head count.
                    /// iops Maximum r/w I/O in operations per second.
                    /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                    /// iops_max_length Maximum length of I/O bursts in seconds.
                    /// iops_rd Maximum read I/O in operations per second.
                    /// iops_rd_length 
                    /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                    /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                    /// iops_wr Maximum write I/O in operations per second.
                    /// iops_wr_length 
                    /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                    /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                    /// iothread Whether to use iothreads for this drive
                    /// mbps Maximum r/w speed in megabytes per second.
                    /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                    /// mbps_rd Maximum read speed in megabytes per second.
                    /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                    /// mbps_wr Maximum write speed in megabytes per second.
                    /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                    /// media The drive's media type.
                    ///   Enum: cdrom,disk
                    /// queues Number of queues.
                    /// replicate Whether the drive should considered for replication jobs.
                    /// rerror Read error action.
                    ///   Enum: ignore,report,stop
                    /// scsiblock whether to use scsi-block for full passthrough of host block device  WARNING: can lead to I/O errors in combination with low memory or high memory fragmentation on host
                    /// secs Force the drive's physical geometry to have a specific sector count.
                    /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                    /// size Disk size. This is purely informational and has no effect.
                    /// snapshot Whether the drive should be included when making snapshots.
                    /// trans Force disk geometry bios translation mode.
                    ///   Enum: none,lba,auto
                    /// volume 
                    /// werror Write error action.
                    ///   Enum: enospc,ignore,report,stop///</param>
                    /// <param name="scsihw">SCSI controller model
                    ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                    /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                    /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning</param>
                    /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                    /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                    /// <param name="sockets">The number of CPU sockets.</param>
                    /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                    /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                    /// <param name="storage">Default storage.</param>
                    /// <param name="tablet">Enable/disable the USB tablet device.</param>
                    /// <param name="tdf">Enable/disable time drift fix.</param>
                    /// <param name="template">Enable/disable Template.</param>
                    /// <param name="unique">Assign a unique random ethernet address.</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <param name="usbN">Configure an USB device (n is 0 to 4).
                    /// host The Host USB device or port or the value 'spice'. HOSTUSBDEVICE syntax is:   'bus-port(.port)*' (decimal numbers) or  'vendor_id:product_id' (hexadeciaml numbers) or  'spice'  You can use the 'lsusb -t' command to list existing usb devices.  NOTE: This option allows direct access to host hardware. So it is no longer possible to migrate such machines - use with special care.  The value 'spice' can be used to add a usb redirection devices for spice. 
                    /// usb3 Specifies whether if given host option is a USB3 device or port (this does currently not work reliably with spice redirection and is then ignored).///</param>
                    /// <param name="vcpus">Number of hotplugged vcpus.</param>
                    /// <param name="vga">Select the VGA type.
                    ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                    /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).
                    /// aio AIO type to use.
                    ///   Enum: native,threads
                    /// backup Whether the drive should be included when making backups.
                    /// bps Maximum r/w speed in bytes per second.
                    /// bps_max_length Maximum length of I/O bursts in seconds.
                    /// bps_rd Maximum read speed in bytes per second.
                    /// bps_rd_length 
                    /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                    /// bps_wr Maximum write speed in bytes per second.
                    /// bps_wr_length 
                    /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                    /// cache The drive's cache mode
                    ///   Enum: none,writethrough,writeback,unsafe,directsync
                    /// cyls Force the drive's physical geometry to have a specific cylinder count.
                    /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                    /// discard Controls whether to pass discard/trim requests to the underlying storage.
                    ///   Enum: ignore,on
                    /// file The drive's backing volume.
                    /// format The drive's backing file's data format.
                    ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                    /// heads Force the drive's physical geometry to have a specific head count.
                    /// iops Maximum r/w I/O in operations per second.
                    /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                    /// iops_max_length Maximum length of I/O bursts in seconds.
                    /// iops_rd Maximum read I/O in operations per second.
                    /// iops_rd_length 
                    /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                    /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                    /// iops_wr Maximum write I/O in operations per second.
                    /// iops_wr_length 
                    /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                    /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                    /// iothread Whether to use iothreads for this drive
                    /// mbps Maximum r/w speed in megabytes per second.
                    /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                    /// mbps_rd Maximum read speed in megabytes per second.
                    /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                    /// mbps_wr Maximum write speed in megabytes per second.
                    /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                    /// media The drive's media type.
                    ///   Enum: cdrom,disk
                    /// replicate Whether the drive should considered for replication jobs.
                    /// rerror Read error action.
                    ///   Enum: ignore,report,stop
                    /// secs Force the drive's physical geometry to have a specific sector count.
                    /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                    /// size Disk size. This is purely informational and has no effect.
                    /// snapshot Whether the drive should be included when making snapshots.
                    /// trans Force disk geometry bios translation mode.
                    ///   Enum: none,lba,auto
                    /// volume 
                    /// werror Write error action.
                    ///   Enum: enospc,ignore,report,stop///</param>
                    /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                    public ExpandoObject CreateVm(int vmid, bool? acpi = null, bool? agent = null, string archive = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string description = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, string pool = null, bool? protection = null, bool? reboot = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, IDictionary<int, string> serialN = null, int? shares = null, string smbios1 = null, int? smp = null, int? sockets = null, string startdate = null, string startup = null, string storage = null, bool? tablet = null, bool? tdf = null, bool? template = null, bool? unique = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string watchdog = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("acpi", acpi);
                        parameters.Add("agent", agent);
                        parameters.Add("archive", archive);
                        parameters.Add("args", args);
                        parameters.Add("autostart", autostart);
                        parameters.Add("balloon", balloon);
                        parameters.Add("bios", bios);
                        parameters.Add("boot", boot);
                        parameters.Add("bootdisk", bootdisk);
                        parameters.Add("cdrom", cdrom);
                        parameters.Add("cores", cores);
                        parameters.Add("cpu", cpu);
                        parameters.Add("cpulimit", cpulimit);
                        parameters.Add("cpuunits", cpuunits);
                        parameters.Add("description", description);
                        parameters.Add("force", force);
                        parameters.Add("freeze", freeze);
                        AddComplexParmeterToDictionary(parameters, "hostpci", hostpciN);
                        parameters.Add("hotplug", hotplug);
                        parameters.Add("hugepages", hugepages);
                        AddComplexParmeterToDictionary(parameters, "ide", ideN);
                        parameters.Add("keyboard", keyboard);
                        parameters.Add("kvm", kvm);
                        parameters.Add("localtime", localtime);
                        parameters.Add("lock", lock_);
                        parameters.Add("machine", machine);
                        parameters.Add("memory", memory);
                        parameters.Add("migrate_downtime", migrate_downtime);
                        parameters.Add("migrate_speed", migrate_speed);
                        parameters.Add("name", name);
                        AddComplexParmeterToDictionary(parameters, "net", netN);
                        parameters.Add("numa", numa);
                        AddComplexParmeterToDictionary(parameters, "numa", numaN);
                        parameters.Add("onboot", onboot);
                        parameters.Add("ostype", ostype);
                        AddComplexParmeterToDictionary(parameters, "parallel", parallelN);
                        parameters.Add("pool", pool);
                        parameters.Add("protection", protection);
                        parameters.Add("reboot", reboot);
                        AddComplexParmeterToDictionary(parameters, "sata", sataN);
                        AddComplexParmeterToDictionary(parameters, "scsi", scsiN);
                        parameters.Add("scsihw", scsihw);
                        AddComplexParmeterToDictionary(parameters, "serial", serialN);
                        parameters.Add("shares", shares);
                        parameters.Add("smbios1", smbios1);
                        parameters.Add("smp", smp);
                        parameters.Add("sockets", sockets);
                        parameters.Add("startdate", startdate);
                        parameters.Add("startup", startup);
                        parameters.Add("storage", storage);
                        parameters.Add("tablet", tablet);
                        parameters.Add("tdf", tdf);
                        parameters.Add("template", template);
                        parameters.Add("unique", unique);
                        AddComplexParmeterToDictionary(parameters, "unused", unusedN);
                        AddComplexParmeterToDictionary(parameters, "usb", usbN);
                        parameters.Add("vcpus", vcpus);
                        parameters.Add("vga", vga);
                        AddComplexParmeterToDictionary(parameters, "virtio", virtioN);
                        parameters.Add("vmid", vmid);
                        parameters.Add("watchdog", watchdog);
                        return _client.Execute($"/nodes/{_node}/qemu", HttpMethod.Post, parameters);
                    }

                    public PVEItemVmid this[object vmid] { get { return new PVEItemVmid(_client, node: _node, vmid: vmid); } }

                    public class PVEItemVmid
                    {
                        private Client _client;
                        private object _node;
                        private object _vmid;
                        internal PVEItemVmid(Client client, object node, object vmid)
                        {
                            _client = client;
                            _node = node;
                            _vmid = vmid;
                        }

                        /// <summary>
                        /// Destroy the vm (also delete all used/owned volumes).
                        /// </summary>
                        /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                        public ExpandoObject DestroyVm(bool? skiplock = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("skiplock", skiplock);
                            return _client.Execute($"/nodes/{_node}/qemu/{_vmid}", HttpMethod.Delete, parameters);
                        }

                        /// <summary>
                        /// Directory index
                        /// </summary>
                        public ExpandoObject Vmdiridx() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}", HttpMethod.Get); }

                        private PVEFirewall _firewall;
                        public PVEFirewall Firewall { get { return _firewall ?? (_firewall = new PVEFirewall(_client, node: _node, vmid: _vmid)); } }

                        public class PVEFirewall
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEFirewall(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall", HttpMethod.Get); }

                            private PVERules _rules;
                            public PVERules Rules { get { return _rules ?? (_rules = new PVERules(_client, node: _node, vmid: _vmid)); } }

                            public class PVERules
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVERules(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// List rules.
                                /// </summary>
                                public ExpandoObject GetRules() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/rules", HttpMethod.Get); }

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
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos>.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                public void CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("action", action);
                                    parameters.Add("comment", comment);
                                    parameters.Add("dest", dest);
                                    parameters.Add("digest", digest);
                                    parameters.Add("dport", dport);
                                    parameters.Add("enable", enable);
                                    parameters.Add("iface", iface);
                                    parameters.Add("macro", macro);
                                    parameters.Add("pos", pos);
                                    parameters.Add("proto", proto);
                                    parameters.Add("source", source);
                                    parameters.Add("sport", sport);
                                    parameters.Add("type", type);
                                    _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/rules", HttpMethod.Post, parameters);
                                }

                                public PVEItemPos this[object pos] { get { return new PVEItemPos(_client, node: _node, vmid: _vmid, pos: pos); } }

                                public class PVEItemPos
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _pos;
                                    internal PVEItemPos(Client client, object node, object vmid, object pos)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _pos = pos;
                                    }

                                    /// <summary>
                                    /// Delete rule.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    public void DeleteRule(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}", HttpMethod.Delete, parameters);
                                    }

                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    public ExpandoObject GetRule() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}", HttpMethod.Get); }

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
                                    /// <param name="macro">Use predefined standard macro.</param>
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto>. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    public void UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                                        parameters.Add("macro", macro);
                                        parameters.Add("moveto", moveto);
                                        parameters.Add("proto", proto);
                                        parameters.Add("source", source);
                                        parameters.Add("sport", sport);
                                        parameters.Add("type", type);
                                        _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}", HttpMethod.Put, parameters);
                                    }
                                }
                            }

                            private PVEAliases _aliases;
                            public PVEAliases Aliases { get { return _aliases ?? (_aliases = new PVEAliases(_client, node: _node, vmid: _vmid)); } }

                            public class PVEAliases
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEAliases(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// List aliases
                                /// </summary>
                                public ExpandoObject GetAliases() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases", HttpMethod.Get); }

                                /// <summary>
                                /// Create IP or Network Alias.
                                /// </summary>
                                /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                /// <param name="name">Alias name.</param>
                                /// <param name="comment"></param>
                                public void CreateAlias(string cidr, string name, string comment = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("cidr", cidr);
                                    parameters.Add("comment", comment);
                                    parameters.Add("name", name);
                                    _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases", HttpMethod.Post, parameters);
                                }

                                public PVEItemName this[object name] { get { return new PVEItemName(_client, node: _node, vmid: _vmid, name: name); } }

                                public class PVEItemName
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _name;
                                    internal PVEItemName(Client client, object node, object vmid, object name)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }

                                    /// <summary>
                                    /// Remove IP or Network alias.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    public void RemoveAlias(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}", HttpMethod.Delete, parameters);
                                    }

                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    public ExpandoObject ReadAlias() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}", HttpMethod.Get); }

                                    /// <summary>
                                    /// Update IP or Network alias.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="rename">Rename an existing alias.</param>
                                    public void UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("digest", digest);
                                        parameters.Add("rename", rename);
                                        _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}", HttpMethod.Put, parameters);
                                    }
                                }
                            }

                            private PVEIpset _ipset;
                            public PVEIpset Ipset { get { return _ipset ?? (_ipset = new PVEIpset(_client, node: _node, vmid: _vmid)); } }

                            public class PVEIpset
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEIpset(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                public ExpandoObject IpsetIndex() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset", HttpMethod.Get); }

                                /// <summary>
                                /// Create new IPSet
                                /// </summary>
                                /// <param name="name">IP set name.</param>
                                /// <param name="comment"></param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                                public void CreateIpset(string name, string comment = null, string digest = null, string rename = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("comment", comment);
                                    parameters.Add("digest", digest);
                                    parameters.Add("name", name);
                                    parameters.Add("rename", rename);
                                    _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset", HttpMethod.Post, parameters);
                                }

                                public PVEItemName this[object name] { get { return new PVEItemName(_client, node: _node, vmid: _vmid, name: name); } }

                                public class PVEItemName
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _name;
                                    internal PVEItemName(Client client, object node, object vmid, object name)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }

                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    public void DeleteIpset() { _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}", HttpMethod.Delete); }

                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    public ExpandoObject GetIpset() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}", HttpMethod.Get); }

                                    /// <summary>
                                    /// Add IP or Network to IPSet.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="nomatch"></param>
                                    public void CreateIp(string cidr, string comment = null, bool? nomatch = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("nomatch", nomatch);
                                        _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}", HttpMethod.Post, parameters);
                                    }

                                    public PVEItemCidr this[object cidr] { get { return new PVEItemCidr(_client, node: _node, vmid: _vmid, name: _name, cidr: cidr); } }

                                    public class PVEItemCidr
                                    {
                                        private Client _client;
                                        private object _node;
                                        private object _vmid;
                                        private object _name;
                                        private object _cidr;
                                        internal PVEItemCidr(Client client, object node, object vmid, object name, object cidr)
                                        {
                                            _client = client;
                                            _node = node;
                                            _vmid = vmid;
                                            _name = name;
                                            _cidr = cidr;
                                        }

                                        /// <summary>
                                        /// Remove IP or Network from IPSet.
                                        /// </summary>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        public void RemoveIp(string digest = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("digest", digest);
                                            _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}", HttpMethod.Delete, parameters);
                                        }

                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        public ExpandoObject ReadIp() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}", HttpMethod.Get); }

                                        /// <summary>
                                        /// Update IP or Network settings
                                        /// </summary>
                                        /// <param name="comment"></param>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <param name="nomatch"></param>
                                        public void UpdateIp(string comment = null, string digest = null, bool? nomatch = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("comment", comment);
                                            parameters.Add("digest", digest);
                                            parameters.Add("nomatch", nomatch);
                                            _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}", HttpMethod.Put, parameters);
                                        }
                                    }
                                }
                            }

                            private PVEOptions _options;
                            public PVEOptions Options { get { return _options ?? (_options = new PVEOptions(_client, node: _node, vmid: _vmid)); } }

                            public class PVEOptions
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEOptions(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                public ExpandoObject GetOptions() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/options", HttpMethod.Get); }

                                /// <summary>
                                /// Set Firewall options.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp">Enable DHCP.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="enable">Enable/disable firewall rules.</param>
                                /// <param name="ipfilter">Enable default IP filters. This is equivalent to adding an empty ipfilter-net&amp;lt;id> ipset for every interface. Such ipsets implicitly contain sane default restrictions such as restricting IPv6 link local addresses to the one derived from the interface's MAC address. For containers the configured IP addresses will be implicitly added.</param>
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
                                public void SetOptions(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null)
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
                                    _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/options", HttpMethod.Put, parameters);
                                }
                            }

                            private PVELog _log;
                            public PVELog Log { get { return _log ?? (_log = new PVELog(_client, node: _node, vmid: _vmid)); } }

                            public class PVELog
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVELog(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Read firewall log
                                /// </summary>
                                /// <param name="limit"></param>
                                /// <param name="start"></param>
                                public ExpandoObject Log(int? limit = null, int? start = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("limit", limit);
                                    parameters.Add("start", start);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/log", HttpMethod.Get, parameters);
                                }
                            }

                            private PVERefs _refs;
                            public PVERefs Refs { get { return _refs ?? (_refs = new PVERefs(_client, node: _node, vmid: _vmid)); } }

                            public class PVERefs
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVERefs(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                                /// </summary>
                                /// <param name="type">Only list references of specified type.
                                ///   Enum: alias,ipset</param>
                                public ExpandoObject Refs(string type = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/firewall/refs", HttpMethod.Get, parameters);
                                }
                            }
                        }

                        private PVERrd _rrd;
                        public PVERrd Rrd { get { return _rrd ?? (_rrd = new PVERrd(_client, node: _node, vmid: _vmid)); } }

                        public class PVERrd
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVERrd(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public ExpandoObject Rrd(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cf", cf);
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/rrd", HttpMethod.Get, parameters);
                            }
                        }

                        private PVERrddata _rrddata;
                        public PVERrddata Rrddata { get { return _rrddata ?? (_rrddata = new PVERrddata(_client, node: _node, vmid: _vmid)); } }

                        public class PVERrddata
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVERrddata(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Read VM RRD statistics
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            public ExpandoObject Rrddata(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cf", cf);
                                parameters.Add("timeframe", timeframe);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/rrddata", HttpMethod.Get, parameters);
                            }
                        }

                        private PVEConfig _config;
                        public PVEConfig Config { get { return _config ?? (_config = new PVEConfig(_client, node: _node, vmid: _vmid)); } }

                        public class PVEConfig
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEConfig(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Get current virtual machine configuration. This does not include pending configuration changes (see 'pending' API).
                            /// </summary>
                            /// <param name="current">Get current values (instead of pending values).</param>
                            public ExpandoObject VmConfig(bool? current = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("current", current);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/config", HttpMethod.Get, parameters);
                            }

                            /// <summary>
                            /// Set virtual machine options (asynchrounous API).
                            /// </summary>
                            /// <param name="acpi">Enable/disable ACPI.</param>
                            /// <param name="agent">Enable/disable Qemu GuestAgent.</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="background_delay">Time to wait for the task to finish. We return 'null' if the task finish within that time.</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                            /// <param name="bootdisk">Enable booting from specified disk.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
                            /// <param name="cores">The number of cores per socket.</param>
                            /// <param name="cpu">Emulated CPU type.
                            /// cputype Emulated CPU type.
                            ///   Enum: 486,athlon,Broadwell,Broadwell-noTSX,Conroe,core2duo,coreduo,Haswell,Haswell-noTSX,host,IvyBridge,kvm32,kvm64,Nehalem,Opteron_G1,Opteron_G2,Opteron_G3,Opteron_G4,Opteron_G5,Penryn,pentium,pentium2,pentium3,phenom,qemu32,qemu64,SandyBridge,Skylake-Client,Westmere
                            /// hidden Do not identify as a KVM virtual machine.///</param>
                            /// <param name="cpulimit">Limit of CPU usage.</param>
                            /// <param name="cpuunits">CPU weight for a VM.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).
                            /// aio AIO type to use.
                            ///   Enum: native,threads
                            /// backup Whether the drive should be included when making backups.
                            /// bps Maximum r/w speed in bytes per second.
                            /// bps_max_length Maximum length of I/O bursts in seconds.
                            /// bps_rd Maximum read speed in bytes per second.
                            /// bps_rd_length 
                            /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// bps_wr Maximum write speed in bytes per second.
                            /// bps_wr_length 
                            /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// cache The drive's cache mode
                            ///   Enum: none,writethrough,writeback,unsafe,directsync
                            /// cyls Force the drive's physical geometry to have a specific cylinder count.
                            /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                            /// discard Controls whether to pass discard/trim requests to the underlying storage.
                            ///   Enum: ignore,on
                            /// file The drive's backing volume.
                            /// format The drive's backing file's data format.
                            ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                            /// heads Force the drive's physical geometry to have a specific head count.
                            /// iops Maximum r/w I/O in operations per second.
                            /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                            /// iops_max_length Maximum length of I/O bursts in seconds.
                            /// iops_rd Maximum read I/O in operations per second.
                            /// iops_rd_length 
                            /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                            /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// iops_wr Maximum write I/O in operations per second.
                            /// iops_wr_length 
                            /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                            /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// mbps Maximum r/w speed in megabytes per second.
                            /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                            /// mbps_rd Maximum read speed in megabytes per second.
                            /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                            /// mbps_wr Maximum write speed in megabytes per second.
                            /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                            /// media The drive's media type.
                            ///   Enum: cdrom,disk
                            /// model The drive's reported model name, url-encoded, up to 40 bytes long.
                            /// replicate Whether the drive should considered for replication jobs.
                            /// rerror Read error action.
                            ///   Enum: ignore,report,stop
                            /// secs Force the drive's physical geometry to have a specific sector count.
                            /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                            /// size Disk size. This is purely informational and has no effect.
                            /// snapshot Whether the drive should be included when making snapshots.
                            /// trans Force disk geometry bios translation mode.
                            ///   Enum: none,lba,auto
                            /// volume 
                            /// werror Write error action.
                            ///   Enum: enospc,ignore,report,stop///</param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.conf' configuration file.
                            ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                            /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                            /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: migrate,backup,snapshot,rollback</param>
                            /// <param name="machine">Specific the Qemu machine type.</param>
                            /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                            /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                            /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                            /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                            /// <param name="netN">Specify network devices.
                            /// bridge Bridge to attach the network device to. The Proxmox VE standard bridge is called 'vmbr0'.  If you do not specify a bridge, we create a kvm user (NATed) network device, which provides DHCP and DNS services. The following addresses are used:   10.0.2.2   Gateway  10.0.2.3   DNS Server  10.0.2.4   SMB Server  The DHCP server assign addresses to the guest starting from 10.0.2.15. 
                            /// e1000 
                            /// e1000_82540em 
                            /// e1000_82544gc 
                            /// e1000_82545em 
                            /// firewall Whether this interface should be protected by the firewall.
                            /// i82551 
                            /// i82557b 
                            /// i82559er 
                            /// link_down Whether this interface should be disconnected (like pulling the plug).
                            /// macaddr MAC address. That address must be unique withing your network. This is automatically generated if not specified.
                            /// model Network Card Model. The 'virtio' model provides the best performance with very low CPU overhead. If your guest does not support this driver, it is usually best to use 'e1000'.
                            ///   Enum: rtl8139,ne2k_pci,e1000,pcnet,virtio,ne2k_isa,i82551,i82557b,i82559er,vmxnet3,e1000-82540em,e1000-82544gc,e1000-82545em
                            /// ne2k_isa 
                            /// ne2k_pci 
                            /// pcnet 
                            /// queues Number of packet queues to be used on the device.
                            /// rate Rate limit in mbps (megabytes per second) as floating point number.
                            /// rtl8139 
                            /// tag VLAN tag to apply to packets on this interface.
                            /// trunks VLAN trunks to pass through this interface.
                            /// virtio 
                            /// vmxnet3 ///</param>
                            /// <param name="numa">Enable/disable NUMA.</param>
                            /// <param name="numaN">NUMA topology.
                            /// cpus CPUs accessing this NUMA node.
                            /// hostnodes Host NUMA nodes to use.
                            /// memory Amount of memory this NUMA node provides.
                            /// policy NUMA allocation policy.
                            ///   Enum: preferred,bind,interleave///</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">Specify guest operating system.
                            ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                            /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                            /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                            /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).
                            /// aio AIO type to use.
                            ///   Enum: native,threads
                            /// backup Whether the drive should be included when making backups.
                            /// bps Maximum r/w speed in bytes per second.
                            /// bps_max_length Maximum length of I/O bursts in seconds.
                            /// bps_rd Maximum read speed in bytes per second.
                            /// bps_rd_length 
                            /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// bps_wr Maximum write speed in bytes per second.
                            /// bps_wr_length 
                            /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// cache The drive's cache mode
                            ///   Enum: none,writethrough,writeback,unsafe,directsync
                            /// cyls Force the drive's physical geometry to have a specific cylinder count.
                            /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                            /// discard Controls whether to pass discard/trim requests to the underlying storage.
                            ///   Enum: ignore,on
                            /// file The drive's backing volume.
                            /// format The drive's backing file's data format.
                            ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                            /// heads Force the drive's physical geometry to have a specific head count.
                            /// iops Maximum r/w I/O in operations per second.
                            /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                            /// iops_max_length Maximum length of I/O bursts in seconds.
                            /// iops_rd Maximum read I/O in operations per second.
                            /// iops_rd_length 
                            /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                            /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// iops_wr Maximum write I/O in operations per second.
                            /// iops_wr_length 
                            /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                            /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// mbps Maximum r/w speed in megabytes per second.
                            /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                            /// mbps_rd Maximum read speed in megabytes per second.
                            /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                            /// mbps_wr Maximum write speed in megabytes per second.
                            /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                            /// media The drive's media type.
                            ///   Enum: cdrom,disk
                            /// replicate Whether the drive should considered for replication jobs.
                            /// rerror Read error action.
                            ///   Enum: ignore,report,stop
                            /// secs Force the drive's physical geometry to have a specific sector count.
                            /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                            /// size Disk size. This is purely informational and has no effect.
                            /// snapshot Whether the drive should be included when making snapshots.
                            /// trans Force disk geometry bios translation mode.
                            ///   Enum: none,lba,auto
                            /// volume 
                            /// werror Write error action.
                            ///   Enum: enospc,ignore,report,stop///</param>
                            /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).
                            /// aio AIO type to use.
                            ///   Enum: native,threads
                            /// backup Whether the drive should be included when making backups.
                            /// bps Maximum r/w speed in bytes per second.
                            /// bps_max_length Maximum length of I/O bursts in seconds.
                            /// bps_rd Maximum read speed in bytes per second.
                            /// bps_rd_length 
                            /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// bps_wr Maximum write speed in bytes per second.
                            /// bps_wr_length 
                            /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// cache The drive's cache mode
                            ///   Enum: none,writethrough,writeback,unsafe,directsync
                            /// cyls Force the drive's physical geometry to have a specific cylinder count.
                            /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                            /// discard Controls whether to pass discard/trim requests to the underlying storage.
                            ///   Enum: ignore,on
                            /// file The drive's backing volume.
                            /// format The drive's backing file's data format.
                            ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                            /// heads Force the drive's physical geometry to have a specific head count.
                            /// iops Maximum r/w I/O in operations per second.
                            /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                            /// iops_max_length Maximum length of I/O bursts in seconds.
                            /// iops_rd Maximum read I/O in operations per second.
                            /// iops_rd_length 
                            /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                            /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// iops_wr Maximum write I/O in operations per second.
                            /// iops_wr_length 
                            /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                            /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// iothread Whether to use iothreads for this drive
                            /// mbps Maximum r/w speed in megabytes per second.
                            /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                            /// mbps_rd Maximum read speed in megabytes per second.
                            /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                            /// mbps_wr Maximum write speed in megabytes per second.
                            /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                            /// media The drive's media type.
                            ///   Enum: cdrom,disk
                            /// queues Number of queues.
                            /// replicate Whether the drive should considered for replication jobs.
                            /// rerror Read error action.
                            ///   Enum: ignore,report,stop
                            /// scsiblock whether to use scsi-block for full passthrough of host block device  WARNING: can lead to I/O errors in combination with low memory or high memory fragmentation on host
                            /// secs Force the drive's physical geometry to have a specific sector count.
                            /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                            /// size Disk size. This is purely informational and has no effect.
                            /// snapshot Whether the drive should be included when making snapshots.
                            /// trans Force disk geometry bios translation mode.
                            ///   Enum: none,lba,auto
                            /// volume 
                            /// werror Write error action.
                            ///   Enum: enospc,ignore,report,stop///</param>
                            /// <param name="scsihw">SCSI controller model
                            ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                            /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                            /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                            /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                            /// <param name="sockets">The number of CPU sockets.</param>
                            /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="tablet">Enable/disable the USB tablet device.</param>
                            /// <param name="tdf">Enable/disable time drift fix.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <param name="usbN">Configure an USB device (n is 0 to 4).
                            /// host The Host USB device or port or the value 'spice'. HOSTUSBDEVICE syntax is:   'bus-port(.port)*' (decimal numbers) or  'vendor_id:product_id' (hexadeciaml numbers) or  'spice'  You can use the 'lsusb -t' command to list existing usb devices.  NOTE: This option allows direct access to host hardware. So it is no longer possible to migrate such machines - use with special care.  The value 'spice' can be used to add a usb redirection devices for spice. 
                            /// usb3 Specifies whether if given host option is a USB3 device or port (this does currently not work reliably with spice redirection and is then ignored).///</param>
                            /// <param name="vcpus">Number of hotplugged vcpus.</param>
                            /// <param name="vga">Select the VGA type.
                            ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).
                            /// aio AIO type to use.
                            ///   Enum: native,threads
                            /// backup Whether the drive should be included when making backups.
                            /// bps Maximum r/w speed in bytes per second.
                            /// bps_max_length Maximum length of I/O bursts in seconds.
                            /// bps_rd Maximum read speed in bytes per second.
                            /// bps_rd_length 
                            /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// bps_wr Maximum write speed in bytes per second.
                            /// bps_wr_length 
                            /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// cache The drive's cache mode
                            ///   Enum: none,writethrough,writeback,unsafe,directsync
                            /// cyls Force the drive's physical geometry to have a specific cylinder count.
                            /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                            /// discard Controls whether to pass discard/trim requests to the underlying storage.
                            ///   Enum: ignore,on
                            /// file The drive's backing volume.
                            /// format The drive's backing file's data format.
                            ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                            /// heads Force the drive's physical geometry to have a specific head count.
                            /// iops Maximum r/w I/O in operations per second.
                            /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                            /// iops_max_length Maximum length of I/O bursts in seconds.
                            /// iops_rd Maximum read I/O in operations per second.
                            /// iops_rd_length 
                            /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                            /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// iops_wr Maximum write I/O in operations per second.
                            /// iops_wr_length 
                            /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                            /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// iothread Whether to use iothreads for this drive
                            /// mbps Maximum r/w speed in megabytes per second.
                            /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                            /// mbps_rd Maximum read speed in megabytes per second.
                            /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                            /// mbps_wr Maximum write speed in megabytes per second.
                            /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                            /// media The drive's media type.
                            ///   Enum: cdrom,disk
                            /// replicate Whether the drive should considered for replication jobs.
                            /// rerror Read error action.
                            ///   Enum: ignore,report,stop
                            /// secs Force the drive's physical geometry to have a specific sector count.
                            /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                            /// size Disk size. This is purely informational and has no effect.
                            /// snapshot Whether the drive should be included when making snapshots.
                            /// trans Force disk geometry bios translation mode.
                            ///   Enum: none,lba,auto
                            /// volume 
                            /// werror Write error action.
                            ///   Enum: enospc,ignore,report,stop///</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            public ExpandoObject UpdateVmAsync(bool? acpi = null, bool? agent = null, string args = null, bool? autostart = null, int? background_delay = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string watchdog = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("acpi", acpi);
                                parameters.Add("agent", agent);
                                parameters.Add("args", args);
                                parameters.Add("autostart", autostart);
                                parameters.Add("background_delay", background_delay);
                                parameters.Add("balloon", balloon);
                                parameters.Add("bios", bios);
                                parameters.Add("boot", boot);
                                parameters.Add("bootdisk", bootdisk);
                                parameters.Add("cdrom", cdrom);
                                parameters.Add("cores", cores);
                                parameters.Add("cpu", cpu);
                                parameters.Add("cpulimit", cpulimit);
                                parameters.Add("cpuunits", cpuunits);
                                parameters.Add("delete", delete);
                                parameters.Add("description", description);
                                parameters.Add("digest", digest);
                                parameters.Add("force", force);
                                parameters.Add("freeze", freeze);
                                AddComplexParmeterToDictionary(parameters, "hostpci", hostpciN);
                                parameters.Add("hotplug", hotplug);
                                parameters.Add("hugepages", hugepages);
                                AddComplexParmeterToDictionary(parameters, "ide", ideN);
                                parameters.Add("keyboard", keyboard);
                                parameters.Add("kvm", kvm);
                                parameters.Add("localtime", localtime);
                                parameters.Add("lock", lock_);
                                parameters.Add("machine", machine);
                                parameters.Add("memory", memory);
                                parameters.Add("migrate_downtime", migrate_downtime);
                                parameters.Add("migrate_speed", migrate_speed);
                                parameters.Add("name", name);
                                AddComplexParmeterToDictionary(parameters, "net", netN);
                                parameters.Add("numa", numa);
                                AddComplexParmeterToDictionary(parameters, "numa", numaN);
                                parameters.Add("onboot", onboot);
                                parameters.Add("ostype", ostype);
                                AddComplexParmeterToDictionary(parameters, "parallel", parallelN);
                                parameters.Add("protection", protection);
                                parameters.Add("reboot", reboot);
                                parameters.Add("revert", revert);
                                AddComplexParmeterToDictionary(parameters, "sata", sataN);
                                AddComplexParmeterToDictionary(parameters, "scsi", scsiN);
                                parameters.Add("scsihw", scsihw);
                                AddComplexParmeterToDictionary(parameters, "serial", serialN);
                                parameters.Add("shares", shares);
                                parameters.Add("skiplock", skiplock);
                                parameters.Add("smbios1", smbios1);
                                parameters.Add("smp", smp);
                                parameters.Add("sockets", sockets);
                                parameters.Add("startdate", startdate);
                                parameters.Add("startup", startup);
                                parameters.Add("tablet", tablet);
                                parameters.Add("tdf", tdf);
                                parameters.Add("template", template);
                                AddComplexParmeterToDictionary(parameters, "unused", unusedN);
                                AddComplexParmeterToDictionary(parameters, "usb", usbN);
                                parameters.Add("vcpus", vcpus);
                                parameters.Add("vga", vga);
                                AddComplexParmeterToDictionary(parameters, "virtio", virtioN);
                                parameters.Add("watchdog", watchdog);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/config", HttpMethod.Post, parameters);
                            }

                            /// <summary>
                            /// Set virtual machine options (synchrounous API) - You should consider using the POST method instead for any actions involving hotplug or storage allocation.
                            /// </summary>
                            /// <param name="acpi">Enable/disable ACPI.</param>
                            /// <param name="agent">Enable/disable Qemu GuestAgent.</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                            /// <param name="bootdisk">Enable booting from specified disk.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
                            /// <param name="cores">The number of cores per socket.</param>
                            /// <param name="cpu">Emulated CPU type.
                            /// cputype Emulated CPU type.
                            ///   Enum: 486,athlon,Broadwell,Broadwell-noTSX,Conroe,core2duo,coreduo,Haswell,Haswell-noTSX,host,IvyBridge,kvm32,kvm64,Nehalem,Opteron_G1,Opteron_G2,Opteron_G3,Opteron_G4,Opteron_G5,Penryn,pentium,pentium2,pentium3,phenom,qemu32,qemu64,SandyBridge,Skylake-Client,Westmere
                            /// hidden Do not identify as a KVM virtual machine.///</param>
                            /// <param name="cpulimit">Limit of CPU usage.</param>
                            /// <param name="cpuunits">CPU weight for a VM.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).
                            /// aio AIO type to use.
                            ///   Enum: native,threads
                            /// backup Whether the drive should be included when making backups.
                            /// bps Maximum r/w speed in bytes per second.
                            /// bps_max_length Maximum length of I/O bursts in seconds.
                            /// bps_rd Maximum read speed in bytes per second.
                            /// bps_rd_length 
                            /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// bps_wr Maximum write speed in bytes per second.
                            /// bps_wr_length 
                            /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// cache The drive's cache mode
                            ///   Enum: none,writethrough,writeback,unsafe,directsync
                            /// cyls Force the drive's physical geometry to have a specific cylinder count.
                            /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                            /// discard Controls whether to pass discard/trim requests to the underlying storage.
                            ///   Enum: ignore,on
                            /// file The drive's backing volume.
                            /// format The drive's backing file's data format.
                            ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                            /// heads Force the drive's physical geometry to have a specific head count.
                            /// iops Maximum r/w I/O in operations per second.
                            /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                            /// iops_max_length Maximum length of I/O bursts in seconds.
                            /// iops_rd Maximum read I/O in operations per second.
                            /// iops_rd_length 
                            /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                            /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// iops_wr Maximum write I/O in operations per second.
                            /// iops_wr_length 
                            /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                            /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// mbps Maximum r/w speed in megabytes per second.
                            /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                            /// mbps_rd Maximum read speed in megabytes per second.
                            /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                            /// mbps_wr Maximum write speed in megabytes per second.
                            /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                            /// media The drive's media type.
                            ///   Enum: cdrom,disk
                            /// model The drive's reported model name, url-encoded, up to 40 bytes long.
                            /// replicate Whether the drive should considered for replication jobs.
                            /// rerror Read error action.
                            ///   Enum: ignore,report,stop
                            /// secs Force the drive's physical geometry to have a specific sector count.
                            /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                            /// size Disk size. This is purely informational and has no effect.
                            /// snapshot Whether the drive should be included when making snapshots.
                            /// trans Force disk geometry bios translation mode.
                            ///   Enum: none,lba,auto
                            /// volume 
                            /// werror Write error action.
                            ///   Enum: enospc,ignore,report,stop///</param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.conf' configuration file.
                            ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                            /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                            /// <param name="localtime">Set the real time clock to local time. This is enabled by default if ostype indicates a Microsoft OS.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: migrate,backup,snapshot,rollback</param>
                            /// <param name="machine">Specific the Qemu machine type.</param>
                            /// <param name="memory">Amount of RAM for the VM in MB. This is the maximum available memory when you use the balloon device.</param>
                            /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                            /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                            /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                            /// <param name="netN">Specify network devices.
                            /// bridge Bridge to attach the network device to. The Proxmox VE standard bridge is called 'vmbr0'.  If you do not specify a bridge, we create a kvm user (NATed) network device, which provides DHCP and DNS services. The following addresses are used:   10.0.2.2   Gateway  10.0.2.3   DNS Server  10.0.2.4   SMB Server  The DHCP server assign addresses to the guest starting from 10.0.2.15. 
                            /// e1000 
                            /// e1000_82540em 
                            /// e1000_82544gc 
                            /// e1000_82545em 
                            /// firewall Whether this interface should be protected by the firewall.
                            /// i82551 
                            /// i82557b 
                            /// i82559er 
                            /// link_down Whether this interface should be disconnected (like pulling the plug).
                            /// macaddr MAC address. That address must be unique withing your network. This is automatically generated if not specified.
                            /// model Network Card Model. The 'virtio' model provides the best performance with very low CPU overhead. If your guest does not support this driver, it is usually best to use 'e1000'.
                            ///   Enum: rtl8139,ne2k_pci,e1000,pcnet,virtio,ne2k_isa,i82551,i82557b,i82559er,vmxnet3,e1000-82540em,e1000-82544gc,e1000-82545em
                            /// ne2k_isa 
                            /// ne2k_pci 
                            /// pcnet 
                            /// queues Number of packet queues to be used on the device.
                            /// rate Rate limit in mbps (megabytes per second) as floating point number.
                            /// rtl8139 
                            /// tag VLAN tag to apply to packets on this interface.
                            /// trunks VLAN trunks to pass through this interface.
                            /// virtio 
                            /// vmxnet3 ///</param>
                            /// <param name="numa">Enable/disable NUMA.</param>
                            /// <param name="numaN">NUMA topology.
                            /// cpus CPUs accessing this NUMA node.
                            /// hostnodes Host NUMA nodes to use.
                            /// memory Amount of memory this NUMA node provides.
                            /// policy NUMA allocation policy.
                            ///   Enum: preferred,bind,interleave///</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">Specify guest operating system.
                            ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,l24,l26,solaris</param>
                            /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                            /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                            /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5).
                            /// aio AIO type to use.
                            ///   Enum: native,threads
                            /// backup Whether the drive should be included when making backups.
                            /// bps Maximum r/w speed in bytes per second.
                            /// bps_max_length Maximum length of I/O bursts in seconds.
                            /// bps_rd Maximum read speed in bytes per second.
                            /// bps_rd_length 
                            /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// bps_wr Maximum write speed in bytes per second.
                            /// bps_wr_length 
                            /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// cache The drive's cache mode
                            ///   Enum: none,writethrough,writeback,unsafe,directsync
                            /// cyls Force the drive's physical geometry to have a specific cylinder count.
                            /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                            /// discard Controls whether to pass discard/trim requests to the underlying storage.
                            ///   Enum: ignore,on
                            /// file The drive's backing volume.
                            /// format The drive's backing file's data format.
                            ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                            /// heads Force the drive's physical geometry to have a specific head count.
                            /// iops Maximum r/w I/O in operations per second.
                            /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                            /// iops_max_length Maximum length of I/O bursts in seconds.
                            /// iops_rd Maximum read I/O in operations per second.
                            /// iops_rd_length 
                            /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                            /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// iops_wr Maximum write I/O in operations per second.
                            /// iops_wr_length 
                            /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                            /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// mbps Maximum r/w speed in megabytes per second.
                            /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                            /// mbps_rd Maximum read speed in megabytes per second.
                            /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                            /// mbps_wr Maximum write speed in megabytes per second.
                            /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                            /// media The drive's media type.
                            ///   Enum: cdrom,disk
                            /// replicate Whether the drive should considered for replication jobs.
                            /// rerror Read error action.
                            ///   Enum: ignore,report,stop
                            /// secs Force the drive's physical geometry to have a specific sector count.
                            /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                            /// size Disk size. This is purely informational and has no effect.
                            /// snapshot Whether the drive should be included when making snapshots.
                            /// trans Force disk geometry bios translation mode.
                            ///   Enum: none,lba,auto
                            /// volume 
                            /// werror Write error action.
                            ///   Enum: enospc,ignore,report,stop///</param>
                            /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 13).
                            /// aio AIO type to use.
                            ///   Enum: native,threads
                            /// backup Whether the drive should be included when making backups.
                            /// bps Maximum r/w speed in bytes per second.
                            /// bps_max_length Maximum length of I/O bursts in seconds.
                            /// bps_rd Maximum read speed in bytes per second.
                            /// bps_rd_length 
                            /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// bps_wr Maximum write speed in bytes per second.
                            /// bps_wr_length 
                            /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// cache The drive's cache mode
                            ///   Enum: none,writethrough,writeback,unsafe,directsync
                            /// cyls Force the drive's physical geometry to have a specific cylinder count.
                            /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                            /// discard Controls whether to pass discard/trim requests to the underlying storage.
                            ///   Enum: ignore,on
                            /// file The drive's backing volume.
                            /// format The drive's backing file's data format.
                            ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                            /// heads Force the drive's physical geometry to have a specific head count.
                            /// iops Maximum r/w I/O in operations per second.
                            /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                            /// iops_max_length Maximum length of I/O bursts in seconds.
                            /// iops_rd Maximum read I/O in operations per second.
                            /// iops_rd_length 
                            /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                            /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// iops_wr Maximum write I/O in operations per second.
                            /// iops_wr_length 
                            /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                            /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// iothread Whether to use iothreads for this drive
                            /// mbps Maximum r/w speed in megabytes per second.
                            /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                            /// mbps_rd Maximum read speed in megabytes per second.
                            /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                            /// mbps_wr Maximum write speed in megabytes per second.
                            /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                            /// media The drive's media type.
                            ///   Enum: cdrom,disk
                            /// queues Number of queues.
                            /// replicate Whether the drive should considered for replication jobs.
                            /// rerror Read error action.
                            ///   Enum: ignore,report,stop
                            /// scsiblock whether to use scsi-block for full passthrough of host block device  WARNING: can lead to I/O errors in combination with low memory or high memory fragmentation on host
                            /// secs Force the drive's physical geometry to have a specific sector count.
                            /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                            /// size Disk size. This is purely informational and has no effect.
                            /// snapshot Whether the drive should be included when making snapshots.
                            /// trans Force disk geometry bios translation mode.
                            ///   Enum: none,lba,auto
                            /// volume 
                            /// werror Write error action.
                            ///   Enum: enospc,ignore,report,stop///</param>
                            /// <param name="scsihw">SCSI controller model
                            ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                            /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                            /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                            /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                            /// <param name="sockets">The number of CPU sockets.</param>
                            /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are: 'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="tablet">Enable/disable the USB tablet device.</param>
                            /// <param name="tdf">Enable/disable time drift fix.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <param name="usbN">Configure an USB device (n is 0 to 4).
                            /// host The Host USB device or port or the value 'spice'. HOSTUSBDEVICE syntax is:   'bus-port(.port)*' (decimal numbers) or  'vendor_id:product_id' (hexadeciaml numbers) or  'spice'  You can use the 'lsusb -t' command to list existing usb devices.  NOTE: This option allows direct access to host hardware. So it is no longer possible to migrate such machines - use with special care.  The value 'spice' can be used to add a usb redirection devices for spice. 
                            /// usb3 Specifies whether if given host option is a USB3 device or port (this does currently not work reliably with spice redirection and is then ignored).///</param>
                            /// <param name="vcpus">Number of hotplugged vcpus.</param>
                            /// <param name="vga">Select the VGA type.
                            ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).
                            /// aio AIO type to use.
                            ///   Enum: native,threads
                            /// backup Whether the drive should be included when making backups.
                            /// bps Maximum r/w speed in bytes per second.
                            /// bps_max_length Maximum length of I/O bursts in seconds.
                            /// bps_rd Maximum read speed in bytes per second.
                            /// bps_rd_length 
                            /// bps_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// bps_wr Maximum write speed in bytes per second.
                            /// bps_wr_length 
                            /// bps_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// cache The drive's cache mode
                            ///   Enum: none,writethrough,writeback,unsafe,directsync
                            /// cyls Force the drive's physical geometry to have a specific cylinder count.
                            /// detect_zeroes Controls whether to detect and try to optimize writes of zeroes.
                            /// discard Controls whether to pass discard/trim requests to the underlying storage.
                            ///   Enum: ignore,on
                            /// file The drive's backing volume.
                            /// format The drive's backing file's data format.
                            ///   Enum: raw,cow,qcow,qed,qcow2,vmdk,cloop
                            /// heads Force the drive's physical geometry to have a specific head count.
                            /// iops Maximum r/w I/O in operations per second.
                            /// iops_max Maximum unthrottled r/w I/O pool in operations per second.
                            /// iops_max_length Maximum length of I/O bursts in seconds.
                            /// iops_rd Maximum read I/O in operations per second.
                            /// iops_rd_length 
                            /// iops_rd_max Maximum unthrottled read I/O pool in operations per second.
                            /// iops_rd_max_length Maximum length of read I/O bursts in seconds.
                            /// iops_wr Maximum write I/O in operations per second.
                            /// iops_wr_length 
                            /// iops_wr_max Maximum unthrottled write I/O pool in operations per second.
                            /// iops_wr_max_length Maximum length of write I/O bursts in seconds.
                            /// iothread Whether to use iothreads for this drive
                            /// mbps Maximum r/w speed in megabytes per second.
                            /// mbps_max Maximum unthrottled r/w pool in megabytes per second.
                            /// mbps_rd Maximum read speed in megabytes per second.
                            /// mbps_rd_max Maximum unthrottled read pool in megabytes per second.
                            /// mbps_wr Maximum write speed in megabytes per second.
                            /// mbps_wr_max Maximum unthrottled write pool in megabytes per second.
                            /// media The drive's media type.
                            ///   Enum: cdrom,disk
                            /// replicate Whether the drive should considered for replication jobs.
                            /// rerror Read error action.
                            ///   Enum: ignore,report,stop
                            /// secs Force the drive's physical geometry to have a specific sector count.
                            /// serial The drive's reported serial number, url-encoded, up to 20 bytes long.
                            /// size Disk size. This is purely informational and has no effect.
                            /// snapshot Whether the drive should be included when making snapshots.
                            /// trans Force disk geometry bios translation mode.
                            ///   Enum: none,lba,auto
                            /// volume 
                            /// werror Write error action.
                            ///   Enum: enospc,ignore,report,stop///</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            public void UpdateVm(bool? acpi = null, bool? agent = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string watchdog = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("acpi", acpi);
                                parameters.Add("agent", agent);
                                parameters.Add("args", args);
                                parameters.Add("autostart", autostart);
                                parameters.Add("balloon", balloon);
                                parameters.Add("bios", bios);
                                parameters.Add("boot", boot);
                                parameters.Add("bootdisk", bootdisk);
                                parameters.Add("cdrom", cdrom);
                                parameters.Add("cores", cores);
                                parameters.Add("cpu", cpu);
                                parameters.Add("cpulimit", cpulimit);
                                parameters.Add("cpuunits", cpuunits);
                                parameters.Add("delete", delete);
                                parameters.Add("description", description);
                                parameters.Add("digest", digest);
                                parameters.Add("force", force);
                                parameters.Add("freeze", freeze);
                                AddComplexParmeterToDictionary(parameters, "hostpci", hostpciN);
                                parameters.Add("hotplug", hotplug);
                                parameters.Add("hugepages", hugepages);
                                AddComplexParmeterToDictionary(parameters, "ide", ideN);
                                parameters.Add("keyboard", keyboard);
                                parameters.Add("kvm", kvm);
                                parameters.Add("localtime", localtime);
                                parameters.Add("lock", lock_);
                                parameters.Add("machine", machine);
                                parameters.Add("memory", memory);
                                parameters.Add("migrate_downtime", migrate_downtime);
                                parameters.Add("migrate_speed", migrate_speed);
                                parameters.Add("name", name);
                                AddComplexParmeterToDictionary(parameters, "net", netN);
                                parameters.Add("numa", numa);
                                AddComplexParmeterToDictionary(parameters, "numa", numaN);
                                parameters.Add("onboot", onboot);
                                parameters.Add("ostype", ostype);
                                AddComplexParmeterToDictionary(parameters, "parallel", parallelN);
                                parameters.Add("protection", protection);
                                parameters.Add("reboot", reboot);
                                parameters.Add("revert", revert);
                                AddComplexParmeterToDictionary(parameters, "sata", sataN);
                                AddComplexParmeterToDictionary(parameters, "scsi", scsiN);
                                parameters.Add("scsihw", scsihw);
                                AddComplexParmeterToDictionary(parameters, "serial", serialN);
                                parameters.Add("shares", shares);
                                parameters.Add("skiplock", skiplock);
                                parameters.Add("smbios1", smbios1);
                                parameters.Add("smp", smp);
                                parameters.Add("sockets", sockets);
                                parameters.Add("startdate", startdate);
                                parameters.Add("startup", startup);
                                parameters.Add("tablet", tablet);
                                parameters.Add("tdf", tdf);
                                parameters.Add("template", template);
                                AddComplexParmeterToDictionary(parameters, "unused", unusedN);
                                AddComplexParmeterToDictionary(parameters, "usb", usbN);
                                parameters.Add("vcpus", vcpus);
                                parameters.Add("vga", vga);
                                AddComplexParmeterToDictionary(parameters, "virtio", virtioN);
                                parameters.Add("watchdog", watchdog);
                                _client.Execute($"/nodes/{_node}/qemu/{_vmid}/config", HttpMethod.Put, parameters);
                            }
                        }

                        private PVEPending _pending;
                        public PVEPending Pending { get { return _pending ?? (_pending = new PVEPending(_client, node: _node, vmid: _vmid)); } }

                        public class PVEPending
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEPending(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Get virtual machine configuration, including pending changes.
                            /// </summary>
                            public ExpandoObject VmPending() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/pending", HttpMethod.Get); }
                        }

                        private PVEUnlink _unlink;
                        public PVEUnlink Unlink { get { return _unlink ?? (_unlink = new PVEUnlink(_client, node: _node, vmid: _vmid)); } }

                        public class PVEUnlink
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEUnlink(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Unlink/delete disk images.
                            /// </summary>
                            /// <param name="idlist">A list of disk IDs you want to delete.</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            public void Unlink(string idlist, bool? force = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                parameters.Add("idlist", idlist);
                                _client.Execute($"/nodes/{_node}/qemu/{_vmid}/unlink", HttpMethod.Put, parameters);
                            }
                        }

                        private PVEVncproxy _vncproxy;
                        public PVEVncproxy Vncproxy { get { return _vncproxy ?? (_vncproxy = new PVEVncproxy(_client, node: _node, vmid: _vmid)); } }

                        public class PVEVncproxy
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEVncproxy(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Creates a TCP VNC proxy connections.
                            /// </summary>
                            /// <param name="websocket">starts websockify instead of vncproxy</param>
                            public ExpandoObject Vncproxy(bool? websocket = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("websocket", websocket);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/vncproxy", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEVncwebsocket _vncwebsocket;
                        public PVEVncwebsocket Vncwebsocket { get { return _vncwebsocket ?? (_vncwebsocket = new PVEVncwebsocket(_client, node: _node, vmid: _vmid)); } }

                        public class PVEVncwebsocket
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEVncwebsocket(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Opens a weksocket for VNC traffic.
                            /// </summary>
                            /// <param name="port">Port number returned by previous vncproxy call.</param>
                            /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                            public ExpandoObject Vncwebsocket(int port, string vncticket)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("port", port);
                                parameters.Add("vncticket", vncticket);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/vncwebsocket", HttpMethod.Get, parameters);
                            }
                        }

                        private PVESpiceproxy _spiceproxy;
                        public PVESpiceproxy Spiceproxy { get { return _spiceproxy ?? (_spiceproxy = new PVESpiceproxy(_client, node: _node, vmid: _vmid)); } }

                        public class PVESpiceproxy
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVESpiceproxy(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Returns a SPICE configuration to connect to the VM.
                            /// </summary>
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As resonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            public ExpandoObject Spiceproxy(string proxy = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("proxy", proxy);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/spiceproxy", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEStatus _status;
                        public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client, node: _node, vmid: _vmid)); } }

                        public class PVEStatus
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEStatus(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Directory index
                            /// </summary>
                            public ExpandoObject Vmcmdidx() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/status", HttpMethod.Get); }

                            private PVECurrent _current;
                            public PVECurrent Current { get { return _current ?? (_current = new PVECurrent(_client, node: _node, vmid: _vmid)); } }

                            public class PVECurrent
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVECurrent(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                public ExpandoObject VmStatus() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/status/current", HttpMethod.Get); }
                            }

                            private PVEStart _start;
                            public PVEStart Start { get { return _start ?? (_start = new PVEStart(_client, node: _node, vmid: _vmid)); } }

                            public class PVEStart
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEStart(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Start virtual machine.
                                /// </summary>
                                /// <param name="machine">Specific the Qemu machine type.</param>
                                /// <param name="migratedfrom">The cluster node name.</param>
                                /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                                /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                                ///   Enum: secure,insecure</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="stateuri">Some command save/restore state from this location.</param>
                                /// <param name="targetstorage">Target storage for the migration. (Can be '1' to use the same storage id as on the source node.)</param>
                                public ExpandoObject VmStart(string machine = null, string migratedfrom = null, string migration_network = null, string migration_type = null, bool? skiplock = null, string stateuri = null, string targetstorage = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("machine", machine);
                                    parameters.Add("migratedfrom", migratedfrom);
                                    parameters.Add("migration_network", migration_network);
                                    parameters.Add("migration_type", migration_type);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("stateuri", stateuri);
                                    parameters.Add("targetstorage", targetstorage);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/status/start", HttpMethod.Post, parameters);
                                }
                            }

                            private PVEStop _stop;
                            public PVEStop Stop { get { return _stop ?? (_stop = new PVEStop(_client, node: _node, vmid: _vmid)); } }

                            public class PVEStop
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEStop(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Stop virtual machine. The qemu process will exit immediately. Thisis akin to pulling the power plug of a running computer and may damage the VM data
                                /// </summary>
                                /// <param name="keepActive">Do not deactivate storage volumes.</param>
                                /// <param name="migratedfrom">The cluster node name.</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                public ExpandoObject VmStop(bool? keepActive = null, string migratedfrom = null, bool? skiplock = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("keepActive", keepActive);
                                    parameters.Add("migratedfrom", migratedfrom);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("timeout", timeout);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/status/stop", HttpMethod.Post, parameters);
                                }
                            }

                            private PVEReset _reset;
                            public PVEReset Reset { get { return _reset ?? (_reset = new PVEReset(_client, node: _node, vmid: _vmid)); } }

                            public class PVEReset
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEReset(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Reset virtual machine.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                public ExpandoObject VmReset(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/status/reset", HttpMethod.Post, parameters);
                                }
                            }

                            private PVEShutdown _shutdown;
                            public PVEShutdown Shutdown { get { return _shutdown ?? (_shutdown = new PVEShutdown(_client, node: _node, vmid: _vmid)); } }

                            public class PVEShutdown
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEShutdown(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Shutdown virtual machine. This is similar to pressing the power button on a physical machine.This will send an ACPI event for the guest OS, which should then proceed to a clean shutdown.
                                /// </summary>
                                /// <param name="forceStop">Make sure the VM stops.</param>
                                /// <param name="keepActive">Do not deactivate storage volumes.</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                public ExpandoObject VmShutdown(bool? forceStop = null, bool? keepActive = null, bool? skiplock = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("forceStop", forceStop);
                                    parameters.Add("keepActive", keepActive);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("timeout", timeout);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/status/shutdown", HttpMethod.Post, parameters);
                                }
                            }

                            private PVESuspend _suspend;
                            public PVESuspend Suspend { get { return _suspend ?? (_suspend = new PVESuspend(_client, node: _node, vmid: _vmid)); } }

                            public class PVESuspend
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVESuspend(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Suspend virtual machine.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                public ExpandoObject VmSuspend(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/status/suspend", HttpMethod.Post, parameters);
                                }
                            }

                            private PVEResume _resume;
                            public PVEResume Resume { get { return _resume ?? (_resume = new PVEResume(_client, node: _node, vmid: _vmid)); } }

                            public class PVEResume
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEResume(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Resume virtual machine.
                                /// </summary>
                                /// <param name="nocheck"></param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                public ExpandoObject VmResume(bool? nocheck = null, bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("nocheck", nocheck);
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/status/resume", HttpMethod.Post, parameters);
                                }
                            }
                        }

                        private PVESendkey _sendkey;
                        public PVESendkey Sendkey { get { return _sendkey ?? (_sendkey = new PVESendkey(_client, node: _node, vmid: _vmid)); } }

                        public class PVESendkey
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVESendkey(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Send key event to virtual machine.
                            /// </summary>
                            /// <param name="key">The key (qemu monitor encoding).</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            public void VmSendkey(string key, bool? skiplock = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("key", key);
                                parameters.Add("skiplock", skiplock);
                                _client.Execute($"/nodes/{_node}/qemu/{_vmid}/sendkey", HttpMethod.Put, parameters);
                            }
                        }

                        private PVEFeature _feature;
                        public PVEFeature Feature { get { return _feature ?? (_feature = new PVEFeature(_client, node: _node, vmid: _vmid)); } }

                        public class PVEFeature
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEFeature(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Check if feature for virtual machine is available.
                            /// </summary>
                            /// <param name="feature">Feature to check.
                            ///   Enum: snapshot,clone,copy</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            public ExpandoObject VmFeature(string feature, string snapname = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("feature", feature);
                                parameters.Add("snapname", snapname);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/feature", HttpMethod.Get, parameters);
                            }
                        }

                        private PVEClone _clone;
                        public PVEClone Clone { get { return _clone ?? (_clone = new PVEClone(_client, node: _node, vmid: _vmid)); } }

                        public class PVEClone
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEClone(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Create a copy of virtual machine/template.
                            /// </summary>
                            /// <param name="newid">VMID for the clone.</param>
                            /// <param name="description">Description for the new VM.</param>
                            /// <param name="format">Target format for file storage.
                            ///   Enum: raw,qcow2,vmdk</param>
                            /// <param name="full">Create a full copy of all disk. This is always done when you clone a normal VM. For VM templates, we try to create a linked clone by default.</param>
                            /// <param name="name">Set a name for the new VM.</param>
                            /// <param name="pool">Add the new VM to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            /// <param name="target">Target node. Only allowed if the original VM is on shared storage.</param>
                            public ExpandoObject CloneVm(int newid, string description = null, string format = null, bool? full = null, string name = null, string pool = null, string snapname = null, string storage = null, string target = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("description", description);
                                parameters.Add("format", format);
                                parameters.Add("full", full);
                                parameters.Add("name", name);
                                parameters.Add("newid", newid);
                                parameters.Add("pool", pool);
                                parameters.Add("snapname", snapname);
                                parameters.Add("storage", storage);
                                parameters.Add("target", target);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/clone", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEMove_Disk _move_disk;
                        public PVEMove_Disk Move_Disk { get { return _move_disk ?? (_move_disk = new PVEMove_Disk(_client, node: _node, vmid: _vmid)); } }

                        public class PVEMove_Disk
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEMove_Disk(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Move volume to different storage.
                            /// </summary>
                            /// <param name="disk">The disk you want to move.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0</param>
                            /// <param name="storage">Target storage.</param>
                            /// <param name="delete">Delete the original disk after successful copy. By default the original disk is kept as unused disk.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="format">Target Format.
                            ///   Enum: raw,qcow2,vmdk</param>
                            public ExpandoObject MoveVmDisk(string disk, string storage, bool? delete = null, string digest = null, string format = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                parameters.Add("disk", disk);
                                parameters.Add("format", format);
                                parameters.Add("storage", storage);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/move_disk", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEMigrate _migrate;
                        public PVEMigrate Migrate { get { return _migrate ?? (_migrate = new PVEMigrate(_client, node: _node, vmid: _vmid)); } }

                        public class PVEMigrate
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEMigrate(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Migrate virtual machine. Creates a new migration task.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <param name="force">Allow to migrate VMs which use local devices. Only root may use this option.</param>
                            /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                            /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                            ///   Enum: secure,insecure</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="targetstorage">Default target storage.</param>
                            /// <param name="with_local_disks">Enable live storage migration for local disk</param>
                            public ExpandoObject MigrateVm(string target, bool? force = null, string migration_network = null, string migration_type = null, bool? online = null, string targetstorage = null, bool? with_local_disks = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                parameters.Add("migration_network", migration_network);
                                parameters.Add("migration_type", migration_type);
                                parameters.Add("online", online);
                                parameters.Add("target", target);
                                parameters.Add("targetstorage", targetstorage);
                                parameters.Add("with-local-disks", with_local_disks);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/migrate", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEMonitor _monitor;
                        public PVEMonitor Monitor { get { return _monitor ?? (_monitor = new PVEMonitor(_client, node: _node, vmid: _vmid)); } }

                        public class PVEMonitor
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEMonitor(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Execute Qemu monitor commands.
                            /// </summary>
                            /// <param name="command">The monitor command.</param>
                            public ExpandoObject Monitor(string command)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("command", command);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/monitor", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEAgent _agent;
                        public PVEAgent Agent { get { return _agent ?? (_agent = new PVEAgent(_client, node: _node, vmid: _vmid)); } }

                        public class PVEAgent
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEAgent(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Execute Qemu Guest Agent commands.
                            /// </summary>
                            /// <param name="command">The QGA command.
                            ///   Enum: ping,get-time,info,fsfreeze-status,fsfreeze-freeze,fsfreeze-thaw,fstrim,network-get-interfaces,get-vcpus,get-fsinfo,get-memory-blocks,get-memory-block-info,suspend-hybrid,suspend-ram,suspend-disk,shutdown</param>
                            public ExpandoObject Agent(string command)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("command", command);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/agent", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEResize _resize;
                        public PVEResize Resize { get { return _resize ?? (_resize = new PVEResize(_client, node: _node, vmid: _vmid)); } }

                        public class PVEResize
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEResize(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public void ResizeVm(string disk, string size, string digest = null, bool? skiplock = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                parameters.Add("disk", disk);
                                parameters.Add("size", size);
                                parameters.Add("skiplock", skiplock);
                                _client.Execute($"/nodes/{_node}/qemu/{_vmid}/resize", HttpMethod.Put, parameters);
                            }
                        }

                        private PVESnapshot _snapshot;
                        public PVESnapshot Snapshot { get { return _snapshot ?? (_snapshot = new PVESnapshot(_client, node: _node, vmid: _vmid)); } }

                        public class PVESnapshot
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVESnapshot(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            public ExpandoObject SnapshotList() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/snapshot", HttpMethod.Get); }

                            /// <summary>
                            /// Snapshot a VM.
                            /// </summary>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="description">A textual description or comment.</param>
                            /// <param name="vmstate">Save the vmstate</param>
                            public ExpandoObject Snapshot(string snapname, string description = null, bool? vmstate = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("description", description);
                                parameters.Add("snapname", snapname);
                                parameters.Add("vmstate", vmstate);
                                return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/snapshot", HttpMethod.Post, parameters);
                            }

                            public PVEItemSnapname this[object snapname] { get { return new PVEItemSnapname(_client, node: _node, vmid: _vmid, snapname: snapname); } }

                            public class PVEItemSnapname
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                private object _snapname;
                                internal PVEItemSnapname(Client client, object node, object vmid, object snapname)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                    _snapname = snapname;
                                }

                                /// <summary>
                                /// Delete a VM snapshot.
                                /// </summary>
                                /// <param name="force">For removal from config file, even if removing disk snapshots fails.</param>
                                public ExpandoObject Delsnapshot(bool? force = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("force", force);
                                    return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}", HttpMethod.Delete, parameters);
                                }

                                /// <summary>
                                /// 
                                /// </summary>
                                public ExpandoObject SnapshotCmdIdx() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}", HttpMethod.Get); }

                                private PVEConfig _config;
                                public PVEConfig Config { get { return _config ?? (_config = new PVEConfig(_client, node: _node, vmid: _vmid, snapname: _snapname)); } }

                                public class PVEConfig
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _snapname;
                                    internal PVEConfig(Client client, object node, object vmid, object snapname)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }

                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    public ExpandoObject GetSnapshotConfig() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/config", HttpMethod.Get); }

                                    /// <summary>
                                    /// Update snapshot metadata.
                                    /// </summary>
                                    /// <param name="description">A textual description or comment.</param>
                                    public void UpdateSnapshotConfig(string description = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("description", description);
                                        _client.Execute($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/config", HttpMethod.Put, parameters);
                                    }
                                }

                                private PVERollback _rollback;
                                public PVERollback Rollback { get { return _rollback ?? (_rollback = new PVERollback(_client, node: _node, vmid: _vmid, snapname: _snapname)); } }

                                public class PVERollback
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _snapname;
                                    internal PVERollback(Client client, object node, object vmid, object snapname)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }

                                    /// <summary>
                                    /// Rollback VM state to specified snapshot.
                                    /// </summary>
                                    public ExpandoObject Rollback() { return _client.Execute($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/rollback", HttpMethod.Post); }
                                }
                            }
                        }

                        private PVETemplate _template;
                        public PVETemplate Template { get { return _template ?? (_template = new PVETemplate(_client, node: _node, vmid: _vmid)); } }

                        public class PVETemplate
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVETemplate(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <param name="disk">If you want to convert only 1 disk to base image.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0</param>
                            public void Template(string disk = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                _client.Execute($"/nodes/{_node}/qemu/{_vmid}/template", HttpMethod.Post, parameters);
                            }
                        }
                    }
                }

                private PVELxc _lxc;
                public PVELxc Lxc { get { return _lxc ?? (_lxc = new PVELxc(_client, node: _node)); } }

                public class PVELxc
                {
                    private Client _client;
                    private object _node;
                    internal PVELxc(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// LXC container index (per node).
                    /// </summary>
                    public ExpandoObject Vmlist() { return _client.Execute($"/nodes/{_node}/lxc", HttpMethod.Get); }

                    /// <summary>
                    /// Create or restore a container.
                    /// </summary>
                    /// <param name="ostemplate">The OS template or backup file.</param>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="arch">OS architecture type.
                    ///   Enum: amd64,i386</param>
                    /// <param name="cmode">Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode to 'shell', it simply invokes a shell inside the container (no login).
                    ///   Enum: shell,console,tty</param>
                    /// <param name="console">Attach a console device (/dev/console) to the container.</param>
                    /// <param name="cores">The number of cores assigned to the container. A container can use all available cores by default.</param>
                    /// <param name="cpulimit">Limit of CPU usage.  NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.</param>
                    /// <param name="cpuunits">CPU weight for a VM. Argument is used in the kernel fair scheduler. The larger the number is, the more CPU time this VM gets. Number is relative to the weights of all the other running VMs.  NOTE: You can disable fair-scheduler configuration by setting this to 0.</param>
                    /// <param name="description">Container description. Only used on the configuration web interface.</param>
                    /// <param name="force">Allow to overwrite existing container.</param>
                    /// <param name="hostname">Set a host name for the container.</param>
                    /// <param name="ignore_unpack_errors">Ignore errors when extracting the template.</param>
                    /// <param name="lock_">Lock/unlock the VM.
                    ///   Enum: migrate,backup,snapshot,rollback</param>
                    /// <param name="memory">Amount of RAM for the VM in MB.</param>
                    /// <param name="mpN">Use volume as container mount point.
                    /// acl Explicitly enable or disable ACL support.
                    /// backup Whether to include the mount point in backups.
                    /// mp Path to the mount point as seen from inside the container (must not contain symlinks).
                    /// quota Enable user quotas inside the container (not supported with zfs subvolumes)
                    /// replicate Will include this volume to a storage replica job.
                    /// ro Read-only mount point
                    /// shared Mark this non-volume mount point as available on multiple nodes (see 'nodes')
                    /// size Volume size (read only value).
                    /// volume Volume, device or directory to mount into the container.///</param>
                    /// <param name="nameserver">Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                    /// <param name="netN">Specifies network interfaces for the container.
                    /// bridge Bridge to attach the network device to.
                    /// firewall Controls whether this interface's firewall rules should be used.
                    /// gw Default gateway for IPv4 traffic.
                    /// gw6 Default gateway for IPv6 traffic.
                    /// hwaddr The interface MAC address. This is dynamically allocated by default, but you can set that statically if needed, for example to always have the same link-local IPv6 address. (lxc.network.hwaddr)
                    /// ip IPv4 address in CIDR format.
                    /// ip6 IPv6 address in CIDR format.
                    /// mtu Maximum transfer unit of the interface. (lxc.network.mtu)
                    /// name Name of the network device as seen from inside the container. (lxc.network.name)
                    /// rate Apply rate limiting to the interface
                    /// tag VLAN tag for this interface.
                    /// trunks VLAN ids to pass through the interface
                    /// type Network interface type.
                    ///   Enum: veth///</param>
                    /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                    /// <param name="ostype">OS type. This is used to setup configuration inside the container, and corresponds to lxc setup scripts in /usr/share/lxc/config/&amp;lt;ostype>.common.conf. Value 'unmanaged' can be used to skip and OS specific setup.
                    ///   Enum: debian,ubuntu,centos,fedora,opensuse,archlinux,alpine,gentoo,unmanaged</param>
                    /// <param name="password">Sets root password inside container.</param>
                    /// <param name="pool">Add the VM to the specified pool.</param>
                    /// <param name="protection">Sets the protection flag of the container. This will prevent the CT or CT's disk remove/update operation.</param>
                    /// <param name="restore">Mark this as restore task.</param>
                    /// <param name="rootfs">Use volume as container root.
                    /// acl Explicitly enable or disable ACL support.
                    /// quota Enable user quotas inside the container (not supported with zfs subvolumes)
                    /// replicate Will include this volume to a storage replica job.
                    /// ro Read-only mount point
                    /// shared Mark this non-volume mount point as available on multiple nodes (see 'nodes')
                    /// size Volume size (read only value).
                    /// volume Volume, device or directory to mount into the container.///</param>
                    /// <param name="searchdomain">Sets DNS search domains for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                    /// <param name="ssh_public_keys">Setup public SSH keys (one key per line, OpenSSH format).</param>
                    /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                    /// <param name="storage">Default Storage.</param>
                    /// <param name="swap">Amount of SWAP for the VM in MB.</param>
                    /// <param name="template">Enable/disable Template.</param>
                    /// <param name="tty">Specify the number of tty available to the container</param>
                    /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    public ExpandoObject CreateVm(string ostemplate, int vmid, string arch = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string description = null, bool? force = null, string hostname = null, bool? ignore_unpack_errors = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, string password = null, string pool = null, bool? protection = null, bool? restore = null, string rootfs = null, string searchdomain = null, string ssh_public_keys = null, string startup = null, string storage = null, int? swap = null, bool? template = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("arch", arch);
                        parameters.Add("cmode", cmode);
                        parameters.Add("console", console);
                        parameters.Add("cores", cores);
                        parameters.Add("cpulimit", cpulimit);
                        parameters.Add("cpuunits", cpuunits);
                        parameters.Add("description", description);
                        parameters.Add("force", force);
                        parameters.Add("hostname", hostname);
                        parameters.Add("ignore-unpack-errors", ignore_unpack_errors);
                        parameters.Add("lock", lock_);
                        parameters.Add("memory", memory);
                        AddComplexParmeterToDictionary(parameters, "mp", mpN);
                        parameters.Add("nameserver", nameserver);
                        AddComplexParmeterToDictionary(parameters, "net", netN);
                        parameters.Add("onboot", onboot);
                        parameters.Add("ostemplate", ostemplate);
                        parameters.Add("ostype", ostype);
                        parameters.Add("password", password);
                        parameters.Add("pool", pool);
                        parameters.Add("protection", protection);
                        parameters.Add("restore", restore);
                        parameters.Add("rootfs", rootfs);
                        parameters.Add("searchdomain", searchdomain);
                        parameters.Add("ssh-public-keys", ssh_public_keys);
                        parameters.Add("startup", startup);
                        parameters.Add("storage", storage);
                        parameters.Add("swap", swap);
                        parameters.Add("template", template);
                        parameters.Add("tty", tty);
                        parameters.Add("unprivileged", unprivileged);
                        AddComplexParmeterToDictionary(parameters, "unused", unusedN);
                        parameters.Add("vmid", vmid);
                        return _client.Execute($"/nodes/{_node}/lxc", HttpMethod.Post, parameters);
                    }

                    public PVEItemVmid this[object vmid] { get { return new PVEItemVmid(_client, node: _node, vmid: vmid); } }

                    public class PVEItemVmid
                    {
                        private Client _client;
                        private object _node;
                        private object _vmid;
                        internal PVEItemVmid(Client client, object node, object vmid)
                        {
                            _client = client;
                            _node = node;
                            _vmid = vmid;
                        }

                        /// <summary>
                        /// Destroy the container (also delete all uses files).
                        /// </summary>
                        public ExpandoObject DestroyVm() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}", HttpMethod.Delete); }

                        /// <summary>
                        /// Directory index
                        /// </summary>
                        public ExpandoObject Vmdiridx() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}", HttpMethod.Get); }

                        private PVEConfig _config;
                        public PVEConfig Config { get { return _config ?? (_config = new PVEConfig(_client, node: _node, vmid: _vmid)); } }

                        public class PVEConfig
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEConfig(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Get container configuration.
                            /// </summary>
                            public ExpandoObject VmConfig() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/config", HttpMethod.Get); }

                            /// <summary>
                            /// Set container options.
                            /// </summary>
                            /// <param name="arch">OS architecture type.
                            ///   Enum: amd64,i386</param>
                            /// <param name="cmode">Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode to 'shell', it simply invokes a shell inside the container (no login).
                            ///   Enum: shell,console,tty</param>
                            /// <param name="console">Attach a console device (/dev/console) to the container.</param>
                            /// <param name="cores">The number of cores assigned to the container. A container can use all available cores by default.</param>
                            /// <param name="cpulimit">Limit of CPU usage.  NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.</param>
                            /// <param name="cpuunits">CPU weight for a VM. Argument is used in the kernel fair scheduler. The larger the number is, the more CPU time this VM gets. Number is relative to the weights of all the other running VMs.  NOTE: You can disable fair-scheduler configuration by setting this to 0.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Container description. Only used on the configuration web interface.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="hostname">Set a host name for the container.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: migrate,backup,snapshot,rollback</param>
                            /// <param name="memory">Amount of RAM for the VM in MB.</param>
                            /// <param name="mpN">Use volume as container mount point.
                            /// acl Explicitly enable or disable ACL support.
                            /// backup Whether to include the mount point in backups.
                            /// mp Path to the mount point as seen from inside the container (must not contain symlinks).
                            /// quota Enable user quotas inside the container (not supported with zfs subvolumes)
                            /// replicate Will include this volume to a storage replica job.
                            /// ro Read-only mount point
                            /// shared Mark this non-volume mount point as available on multiple nodes (see 'nodes')
                            /// size Volume size (read only value).
                            /// volume Volume, device or directory to mount into the container.///</param>
                            /// <param name="nameserver">Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                            /// <param name="netN">Specifies network interfaces for the container.
                            /// bridge Bridge to attach the network device to.
                            /// firewall Controls whether this interface's firewall rules should be used.
                            /// gw Default gateway for IPv4 traffic.
                            /// gw6 Default gateway for IPv6 traffic.
                            /// hwaddr The interface MAC address. This is dynamically allocated by default, but you can set that statically if needed, for example to always have the same link-local IPv6 address. (lxc.network.hwaddr)
                            /// ip IPv4 address in CIDR format.
                            /// ip6 IPv6 address in CIDR format.
                            /// mtu Maximum transfer unit of the interface. (lxc.network.mtu)
                            /// name Name of the network device as seen from inside the container. (lxc.network.name)
                            /// rate Apply rate limiting to the interface
                            /// tag VLAN tag for this interface.
                            /// trunks VLAN ids to pass through the interface
                            /// type Network interface type.
                            ///   Enum: veth///</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">OS type. This is used to setup configuration inside the container, and corresponds to lxc setup scripts in /usr/share/lxc/config/&amp;lt;ostype>.common.conf. Value 'unmanaged' can be used to skip and OS specific setup.
                            ///   Enum: debian,ubuntu,centos,fedora,opensuse,archlinux,alpine,gentoo,unmanaged</param>
                            /// <param name="protection">Sets the protection flag of the container. This will prevent the CT or CT's disk remove/update operation.</param>
                            /// <param name="rootfs">Use volume as container root.
                            /// acl Explicitly enable or disable ACL support.
                            /// quota Enable user quotas inside the container (not supported with zfs subvolumes)
                            /// replicate Will include this volume to a storage replica job.
                            /// ro Read-only mount point
                            /// shared Mark this non-volume mount point as available on multiple nodes (see 'nodes')
                            /// size Volume size (read only value).
                            /// volume Volume, device or directory to mount into the container.///</param>
                            /// <param name="searchdomain">Sets DNS search domains for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="swap">Amount of SWAP for the VM in MB.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="tty">Specify the number of tty available to the container</param>
                            /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            public void UpdateVm(string arch = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string hostname = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, bool? protection = null, string rootfs = null, string searchdomain = null, string startup = null, int? swap = null, bool? template = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null)
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
                                parameters.Add("hostname", hostname);
                                parameters.Add("lock", lock_);
                                parameters.Add("memory", memory);
                                AddComplexParmeterToDictionary(parameters, "mp", mpN);
                                parameters.Add("nameserver", nameserver);
                                AddComplexParmeterToDictionary(parameters, "net", netN);
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
                                AddComplexParmeterToDictionary(parameters, "unused", unusedN);
                                _client.Execute($"/nodes/{_node}/lxc/{_vmid}/config", HttpMethod.Put, parameters);
                            }
                        }

                        private PVEStatus _status;
                        public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client, node: _node, vmid: _vmid)); } }

                        public class PVEStatus
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEStatus(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Directory index
                            /// </summary>
                            public ExpandoObject Vmcmdidx() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/status", HttpMethod.Get); }

                            private PVECurrent _current;
                            public PVECurrent Current { get { return _current ?? (_current = new PVECurrent(_client, node: _node, vmid: _vmid)); } }

                            public class PVECurrent
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVECurrent(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                public ExpandoObject VmStatus() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/status/current", HttpMethod.Get); }
                            }

                            private PVEStart _start;
                            public PVEStart Start { get { return _start ?? (_start = new PVEStart(_client, node: _node, vmid: _vmid)); } }

                            public class PVEStart
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEStart(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Start the container.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                public ExpandoObject VmStart(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/status/start", HttpMethod.Post, parameters);
                                }
                            }

                            private PVEStop _stop;
                            public PVEStop Stop { get { return _stop ?? (_stop = new PVEStop(_client, node: _node, vmid: _vmid)); } }

                            public class PVEStop
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEStop(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Stop the container. This will abruptly stop all processes running in the container.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                public ExpandoObject VmStop(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/status/stop", HttpMethod.Post, parameters);
                                }
                            }

                            private PVEShutdown _shutdown;
                            public PVEShutdown Shutdown { get { return _shutdown ?? (_shutdown = new PVEShutdown(_client, node: _node, vmid: _vmid)); } }

                            public class PVEShutdown
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEShutdown(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Shutdown the container. This will trigger a clean shutdown of the container, see lxc-stop(1) for details.
                                /// </summary>
                                /// <param name="forceStop">Make sure the Container stops.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                public ExpandoObject VmShutdown(bool? forceStop = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("forceStop", forceStop);
                                    parameters.Add("timeout", timeout);
                                    return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/status/shutdown", HttpMethod.Post, parameters);
                                }
                            }

                            private PVESuspend _suspend;
                            public PVESuspend Suspend { get { return _suspend ?? (_suspend = new PVESuspend(_client, node: _node, vmid: _vmid)); } }

                            public class PVESuspend
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVESuspend(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Suspend the container.
                                /// </summary>
                                public ExpandoObject VmSuspend() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/status/suspend", HttpMethod.Post); }
                            }

                            private PVEResume _resume;
                            public PVEResume Resume { get { return _resume ?? (_resume = new PVEResume(_client, node: _node, vmid: _vmid)); } }

                            public class PVEResume
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEResume(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Resume the container.
                                /// </summary>
                                public ExpandoObject VmResume() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/status/resume", HttpMethod.Post); }
                            }
                        }

                        private PVESnapshot _snapshot;
                        public PVESnapshot Snapshot { get { return _snapshot ?? (_snapshot = new PVESnapshot(_client, node: _node, vmid: _vmid)); } }

                        public class PVESnapshot
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVESnapshot(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            public ExpandoObject List() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/snapshot", HttpMethod.Get); }

                            /// <summary>
                            /// Snapshot a container.
                            /// </summary>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="description">A textual description or comment.</param>
                            public ExpandoObject Snapshot(string snapname, string description = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("description", description);
                                parameters.Add("snapname", snapname);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/snapshot", HttpMethod.Post, parameters);
                            }

                            public PVEItemSnapname this[object snapname] { get { return new PVEItemSnapname(_client, node: _node, vmid: _vmid, snapname: snapname); } }

                            public class PVEItemSnapname
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                private object _snapname;
                                internal PVEItemSnapname(Client client, object node, object vmid, object snapname)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                    _snapname = snapname;
                                }

                                /// <summary>
                                /// Delete a LXC snapshot.
                                /// </summary>
                                /// <param name="force">For removal from config file, even if removing disk snapshots fails.</param>
                                public ExpandoObject Delsnapshot(bool? force = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("force", force);
                                    return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}", HttpMethod.Delete, parameters);
                                }

                                /// <summary>
                                /// 
                                /// </summary>
                                public ExpandoObject SnapshotCmdIdx() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}", HttpMethod.Get); }

                                private PVERollback _rollback;
                                public PVERollback Rollback { get { return _rollback ?? (_rollback = new PVERollback(_client, node: _node, vmid: _vmid, snapname: _snapname)); } }

                                public class PVERollback
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _snapname;
                                    internal PVERollback(Client client, object node, object vmid, object snapname)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }

                                    /// <summary>
                                    /// Rollback LXC state to specified snapshot.
                                    /// </summary>
                                    public ExpandoObject Rollback() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/rollback", HttpMethod.Post); }
                                }

                                private PVEConfig _config;
                                public PVEConfig Config { get { return _config ?? (_config = new PVEConfig(_client, node: _node, vmid: _vmid, snapname: _snapname)); } }

                                public class PVEConfig
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _snapname;
                                    internal PVEConfig(Client client, object node, object vmid, object snapname)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }

                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    public ExpandoObject GetSnapshotConfig() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/config", HttpMethod.Get); }

                                    /// <summary>
                                    /// Update snapshot metadata.
                                    /// </summary>
                                    /// <param name="description">A textual description or comment.</param>
                                    public void UpdateSnapshotConfig(string description = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("description", description);
                                        _client.Execute($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/config", HttpMethod.Put, parameters);
                                    }
                                }
                            }
                        }

                        private PVEFirewall _firewall;
                        public PVEFirewall Firewall { get { return _firewall ?? (_firewall = new PVEFirewall(_client, node: _node, vmid: _vmid)); } }

                        public class PVEFirewall
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEFirewall(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall", HttpMethod.Get); }

                            private PVERules _rules;
                            public PVERules Rules { get { return _rules ?? (_rules = new PVERules(_client, node: _node, vmid: _vmid)); } }

                            public class PVERules
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVERules(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// List rules.
                                /// </summary>
                                public ExpandoObject GetRules() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/rules", HttpMethod.Get); }

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
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos>.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                public void CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("action", action);
                                    parameters.Add("comment", comment);
                                    parameters.Add("dest", dest);
                                    parameters.Add("digest", digest);
                                    parameters.Add("dport", dport);
                                    parameters.Add("enable", enable);
                                    parameters.Add("iface", iface);
                                    parameters.Add("macro", macro);
                                    parameters.Add("pos", pos);
                                    parameters.Add("proto", proto);
                                    parameters.Add("source", source);
                                    parameters.Add("sport", sport);
                                    parameters.Add("type", type);
                                    _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/rules", HttpMethod.Post, parameters);
                                }

                                public PVEItemPos this[object pos] { get { return new PVEItemPos(_client, node: _node, vmid: _vmid, pos: pos); } }

                                public class PVEItemPos
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _pos;
                                    internal PVEItemPos(Client client, object node, object vmid, object pos)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _pos = pos;
                                    }

                                    /// <summary>
                                    /// Delete rule.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    public void DeleteRule(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}", HttpMethod.Delete, parameters);
                                    }

                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    public ExpandoObject GetRule() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}", HttpMethod.Get); }

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
                                    /// <param name="macro">Use predefined standard macro.</param>
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto>. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    public void UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                                        parameters.Add("macro", macro);
                                        parameters.Add("moveto", moveto);
                                        parameters.Add("proto", proto);
                                        parameters.Add("source", source);
                                        parameters.Add("sport", sport);
                                        parameters.Add("type", type);
                                        _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}", HttpMethod.Put, parameters);
                                    }
                                }
                            }

                            private PVEAliases _aliases;
                            public PVEAliases Aliases { get { return _aliases ?? (_aliases = new PVEAliases(_client, node: _node, vmid: _vmid)); } }

                            public class PVEAliases
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEAliases(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// List aliases
                                /// </summary>
                                public ExpandoObject GetAliases() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases", HttpMethod.Get); }

                                /// <summary>
                                /// Create IP or Network Alias.
                                /// </summary>
                                /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                /// <param name="name">Alias name.</param>
                                /// <param name="comment"></param>
                                public void CreateAlias(string cidr, string name, string comment = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("cidr", cidr);
                                    parameters.Add("comment", comment);
                                    parameters.Add("name", name);
                                    _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases", HttpMethod.Post, parameters);
                                }

                                public PVEItemName this[object name] { get { return new PVEItemName(_client, node: _node, vmid: _vmid, name: name); } }

                                public class PVEItemName
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _name;
                                    internal PVEItemName(Client client, object node, object vmid, object name)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }

                                    /// <summary>
                                    /// Remove IP or Network alias.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    public void RemoveAlias(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}", HttpMethod.Delete, parameters);
                                    }

                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    public ExpandoObject ReadAlias() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}", HttpMethod.Get); }

                                    /// <summary>
                                    /// Update IP or Network alias.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="rename">Rename an existing alias.</param>
                                    public void UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("digest", digest);
                                        parameters.Add("rename", rename);
                                        _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}", HttpMethod.Put, parameters);
                                    }
                                }
                            }

                            private PVEIpset _ipset;
                            public PVEIpset Ipset { get { return _ipset ?? (_ipset = new PVEIpset(_client, node: _node, vmid: _vmid)); } }

                            public class PVEIpset
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEIpset(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                public ExpandoObject IpsetIndex() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset", HttpMethod.Get); }

                                /// <summary>
                                /// Create new IPSet
                                /// </summary>
                                /// <param name="name">IP set name.</param>
                                /// <param name="comment"></param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                                public void CreateIpset(string name, string comment = null, string digest = null, string rename = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("comment", comment);
                                    parameters.Add("digest", digest);
                                    parameters.Add("name", name);
                                    parameters.Add("rename", rename);
                                    _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset", HttpMethod.Post, parameters);
                                }

                                public PVEItemName this[object name] { get { return new PVEItemName(_client, node: _node, vmid: _vmid, name: name); } }

                                public class PVEItemName
                                {
                                    private Client _client;
                                    private object _node;
                                    private object _vmid;
                                    private object _name;
                                    internal PVEItemName(Client client, object node, object vmid, object name)
                                    {
                                        _client = client;
                                        _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }

                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    public void DeleteIpset() { _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}", HttpMethod.Delete); }

                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    public ExpandoObject GetIpset() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}", HttpMethod.Get); }

                                    /// <summary>
                                    /// Add IP or Network to IPSet.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="nomatch"></param>
                                    public void CreateIp(string cidr, string comment = null, bool? nomatch = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("nomatch", nomatch);
                                        _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}", HttpMethod.Post, parameters);
                                    }

                                    public PVEItemCidr this[object cidr] { get { return new PVEItemCidr(_client, node: _node, vmid: _vmid, name: _name, cidr: cidr); } }

                                    public class PVEItemCidr
                                    {
                                        private Client _client;
                                        private object _node;
                                        private object _vmid;
                                        private object _name;
                                        private object _cidr;
                                        internal PVEItemCidr(Client client, object node, object vmid, object name, object cidr)
                                        {
                                            _client = client;
                                            _node = node;
                                            _vmid = vmid;
                                            _name = name;
                                            _cidr = cidr;
                                        }

                                        /// <summary>
                                        /// Remove IP or Network from IPSet.
                                        /// </summary>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        public void RemoveIp(string digest = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("digest", digest);
                                            _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}", HttpMethod.Delete, parameters);
                                        }

                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        public ExpandoObject ReadIp() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}", HttpMethod.Get); }

                                        /// <summary>
                                        /// Update IP or Network settings
                                        /// </summary>
                                        /// <param name="comment"></param>
                                        /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                        /// <param name="nomatch"></param>
                                        public void UpdateIp(string comment = null, string digest = null, bool? nomatch = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("comment", comment);
                                            parameters.Add("digest", digest);
                                            parameters.Add("nomatch", nomatch);
                                            _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}", HttpMethod.Put, parameters);
                                        }
                                    }
                                }
                            }

                            private PVEOptions _options;
                            public PVEOptions Options { get { return _options ?? (_options = new PVEOptions(_client, node: _node, vmid: _vmid)); } }

                            public class PVEOptions
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVEOptions(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                public ExpandoObject GetOptions() { return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/options", HttpMethod.Get); }

                                /// <summary>
                                /// Set Firewall options.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp">Enable DHCP.</param>
                                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="enable">Enable/disable firewall rules.</param>
                                /// <param name="ipfilter">Enable default IP filters. This is equivalent to adding an empty ipfilter-net&amp;lt;id> ipset for every interface. Such ipsets implicitly contain sane default restrictions such as restricting IPv6 link local addresses to the one derived from the interface's MAC address. For containers the configured IP addresses will be implicitly added.</param>
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
                                public void SetOptions(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null)
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
                                    _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/options", HttpMethod.Put, parameters);
                                }
                            }

                            private PVELog _log;
                            public PVELog Log { get { return _log ?? (_log = new PVELog(_client, node: _node, vmid: _vmid)); } }

                            public class PVELog
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVELog(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Read firewall log
                                /// </summary>
                                /// <param name="limit"></param>
                                /// <param name="start"></param>
                                public ExpandoObject Log(int? limit = null, int? start = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("limit", limit);
                                    parameters.Add("start", start);
                                    return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/log", HttpMethod.Get, parameters);
                                }
                            }

                            private PVERefs _refs;
                            public PVERefs Refs { get { return _refs ?? (_refs = new PVERefs(_client, node: _node, vmid: _vmid)); } }

                            public class PVERefs
                            {
                                private Client _client;
                                private object _node;
                                private object _vmid;
                                internal PVERefs(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }

                                /// <summary>
                                /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                                /// </summary>
                                /// <param name="type">Only list references of specified type.
                                ///   Enum: alias,ipset</param>
                                public ExpandoObject Refs(string type = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/firewall/refs", HttpMethod.Get, parameters);
                                }
                            }
                        }

                        private PVERrd _rrd;
                        public PVERrd Rrd { get { return _rrd ?? (_rrd = new PVERrd(_client, node: _node, vmid: _vmid)); } }

                        public class PVERrd
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVERrd(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public ExpandoObject Rrd(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cf", cf);
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/rrd", HttpMethod.Get, parameters);
                            }
                        }

                        private PVERrddata _rrddata;
                        public PVERrddata Rrddata { get { return _rrddata ?? (_rrddata = new PVERrddata(_client, node: _node, vmid: _vmid)); } }

                        public class PVERrddata
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVERrddata(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Read VM RRD statistics
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            public ExpandoObject Rrddata(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cf", cf);
                                parameters.Add("timeframe", timeframe);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/rrddata", HttpMethod.Get, parameters);
                            }
                        }

                        private PVEVncproxy _vncproxy;
                        public PVEVncproxy Vncproxy { get { return _vncproxy ?? (_vncproxy = new PVEVncproxy(_client, node: _node, vmid: _vmid)); } }

                        public class PVEVncproxy
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEVncproxy(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Creates a TCP VNC proxy connections.
                            /// </summary>
                            /// <param name="height">sets the height of the console in pixels.</param>
                            /// <param name="websocket">use websocket instead of standard VNC.</param>
                            /// <param name="width">sets the width of the console in pixels.</param>
                            public ExpandoObject Vncproxy(int? height = null, bool? websocket = null, int? width = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("height", height);
                                parameters.Add("websocket", websocket);
                                parameters.Add("width", width);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/vncproxy", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEVncwebsocket _vncwebsocket;
                        public PVEVncwebsocket Vncwebsocket { get { return _vncwebsocket ?? (_vncwebsocket = new PVEVncwebsocket(_client, node: _node, vmid: _vmid)); } }

                        public class PVEVncwebsocket
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEVncwebsocket(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Opens a weksocket for VNC traffic.
                            /// </summary>
                            /// <param name="port">Port number returned by previous vncproxy call.</param>
                            /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                            public ExpandoObject Vncwebsocket(int port, string vncticket)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("port", port);
                                parameters.Add("vncticket", vncticket);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/vncwebsocket", HttpMethod.Get, parameters);
                            }
                        }

                        private PVESpiceproxy _spiceproxy;
                        public PVESpiceproxy Spiceproxy { get { return _spiceproxy ?? (_spiceproxy = new PVESpiceproxy(_client, node: _node, vmid: _vmid)); } }

                        public class PVESpiceproxy
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVESpiceproxy(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Returns a SPICE configuration to connect to the CT.
                            /// </summary>
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As resonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            public ExpandoObject Spiceproxy(string proxy = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("proxy", proxy);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/spiceproxy", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEMigrate _migrate;
                        public PVEMigrate Migrate { get { return _migrate ?? (_migrate = new PVEMigrate(_client, node: _node, vmid: _vmid)); } }

                        public class PVEMigrate
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEMigrate(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Migrate the container to another node. Creates a new migration task.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <param name="force">Force migration despite local bind / device mounts. NOTE: deprecated, use 'shared' property of mount point instead.</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="restart">Use restart migration</param>
                            /// <param name="timeout">Timeout in seconds for shutdown for restart migration</param>
                            public ExpandoObject MigrateVm(string target, bool? force = null, bool? online = null, bool? restart = null, int? timeout = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                parameters.Add("online", online);
                                parameters.Add("restart", restart);
                                parameters.Add("target", target);
                                parameters.Add("timeout", timeout);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/migrate", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEFeature _feature;
                        public PVEFeature Feature { get { return _feature ?? (_feature = new PVEFeature(_client, node: _node, vmid: _vmid)); } }

                        public class PVEFeature
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEFeature(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Check if feature for virtual machine is available.
                            /// </summary>
                            /// <param name="feature">Feature to check.
                            ///   Enum: snapshot</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            public ExpandoObject VmFeature(string feature, string snapname = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("feature", feature);
                                parameters.Add("snapname", snapname);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/feature", HttpMethod.Get, parameters);
                            }
                        }

                        private PVETemplate _template;
                        public PVETemplate Template { get { return _template ?? (_template = new PVETemplate(_client, node: _node, vmid: _vmid)); } }

                        public class PVETemplate
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVETemplate(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <param name="experimental">The template feature is experimental, set this flag if you know what you are doing.</param>
                            public void Template(bool experimental)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("experimental", experimental);
                                _client.Execute($"/nodes/{_node}/lxc/{_vmid}/template", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEClone _clone;
                        public PVEClone Clone { get { return _clone ?? (_clone = new PVEClone(_client, node: _node, vmid: _vmid)); } }

                        public class PVEClone
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEClone(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Create a container clone/copy
                            /// </summary>
                            /// <param name="experimental">The clone feature is experimental, set this flag if you know what you are doing.</param>
                            /// <param name="newid">VMID for the clone.</param>
                            /// <param name="description">Description for the new CT.</param>
                            /// <param name="full">Create a full copy of all disk. This is always done when you clone a normal CT. For CT templates, we try to create a linked clone by default.</param>
                            /// <param name="hostname">Set a hostname for the new CT.</param>
                            /// <param name="pool">Add the new CT to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            public ExpandoObject CloneVm(bool experimental, int newid, string description = null, bool? full = null, string hostname = null, string pool = null, string snapname = null, string storage = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("description", description);
                                parameters.Add("experimental", experimental);
                                parameters.Add("full", full);
                                parameters.Add("hostname", hostname);
                                parameters.Add("newid", newid);
                                parameters.Add("pool", pool);
                                parameters.Add("snapname", snapname);
                                parameters.Add("storage", storage);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/clone", HttpMethod.Post, parameters);
                            }
                        }

                        private PVEResize _resize;
                        public PVEResize Resize { get { return _resize ?? (_resize = new PVEResize(_client, node: _node, vmid: _vmid)); } }

                        public class PVEResize
                        {
                            private Client _client;
                            private object _node;
                            private object _vmid;
                            internal PVEResize(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }

                            /// <summary>
                            /// Resize a container mount point.
                            /// </summary>
                            /// <param name="disk">The disk you want to resize.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9</param>
                            /// <param name="size">The new size. With the '+' sign the value is added to the actual size of the volume and without it, the value is taken as an absolute one. Shrinking disk size is not supported.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            public ExpandoObject ResizeVm(string disk, string size, string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                parameters.Add("disk", disk);
                                parameters.Add("size", size);
                                return _client.Execute($"/nodes/{_node}/lxc/{_vmid}/resize", HttpMethod.Put, parameters);
                            }
                        }
                    }
                }

                private PVECeph _ceph;
                public PVECeph Ceph { get { return _ceph ?? (_ceph = new PVECeph(_client, node: _node)); } }

                public class PVECeph
                {
                    private Client _client;
                    private object _node;
                    internal PVECeph(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/ceph", HttpMethod.Get); }

                    private PVEOsd _osd;
                    public PVEOsd Osd { get { return _osd ?? (_osd = new PVEOsd(_client, node: _node)); } }

                    public class PVEOsd
                    {
                        private Client _client;
                        private object _node;
                        internal PVEOsd(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get Ceph osd list/tree.
                        /// </summary>
                        public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/ceph/osd", HttpMethod.Get); }

                        /// <summary>
                        /// Create OSD
                        /// </summary>
                        /// <param name="dev">Block device name.</param>
                        /// <param name="bluestore">Use bluestore instead of filestore.</param>
                        /// <param name="fstype">File system type (filestore only).
                        ///   Enum: xfs,ext4,btrfs</param>
                        /// <param name="journal_dev">Block device name for journal.</param>
                        public ExpandoObject Createosd(string dev, bool? bluestore = null, string fstype = null, string journal_dev = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("bluestore", bluestore);
                            parameters.Add("dev", dev);
                            parameters.Add("fstype", fstype);
                            parameters.Add("journal_dev", journal_dev);
                            return _client.Execute($"/nodes/{_node}/ceph/osd", HttpMethod.Post, parameters);
                        }

                        public PVEItemOsdid this[object osdid] { get { return new PVEItemOsdid(_client, node: _node, osdid: osdid); } }

                        public class PVEItemOsdid
                        {
                            private Client _client;
                            private object _node;
                            private object _osdid;
                            internal PVEItemOsdid(Client client, object node, object osdid)
                            {
                                _client = client;
                                _node = node;
                                _osdid = osdid;
                            }

                            /// <summary>
                            /// Destroy OSD
                            /// </summary>
                            /// <param name="cleanup">If set, we remove partition table entries.</param>
                            public ExpandoObject Destroyosd(bool? cleanup = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cleanup", cleanup);
                                return _client.Execute($"/nodes/{_node}/ceph/osd/{_osdid}", HttpMethod.Delete, parameters);
                            }

                            private PVEIn _in;
                            public PVEIn In { get { return _in ?? (_in = new PVEIn(_client, node: _node, osdid: _osdid)); } }

                            public class PVEIn
                            {
                                private Client _client;
                                private object _node;
                                private object _osdid;
                                internal PVEIn(Client client, object node, object osdid)
                                {
                                    _client = client;
                                    _node = node;
                                    _osdid = osdid;
                                }

                                /// <summary>
                                /// ceph osd in
                                /// </summary>
                                public void In() { _client.Execute($"/nodes/{_node}/ceph/osd/{_osdid}/in", HttpMethod.Post); }
                            }

                            private PVEOut _out;
                            public PVEOut Out { get { return _out ?? (_out = new PVEOut(_client, node: _node, osdid: _osdid)); } }

                            public class PVEOut
                            {
                                private Client _client;
                                private object _node;
                                private object _osdid;
                                internal PVEOut(Client client, object node, object osdid)
                                {
                                    _client = client;
                                    _node = node;
                                    _osdid = osdid;
                                }

                                /// <summary>
                                /// ceph osd out
                                /// </summary>
                                public void Out() { _client.Execute($"/nodes/{_node}/ceph/osd/{_osdid}/out", HttpMethod.Post); }
                            }
                        }
                    }

                    private PVEDisks _disks;
                    public PVEDisks Disks { get { return _disks ?? (_disks = new PVEDisks(_client, node: _node)); } }

                    public class PVEDisks
                    {
                        private Client _client;
                        private object _node;
                        internal PVEDisks(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        /// <param name="type">Only list specific types of disks.
                        ///   Enum: unused,journal_disks</param>
                        public ExpandoObject Disks(string type = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("type", type);
                            return _client.Execute($"/nodes/{_node}/ceph/disks", HttpMethod.Get, parameters);
                        }
                    }

                    private PVEConfig _config;
                    public PVEConfig Config { get { return _config ?? (_config = new PVEConfig(_client, node: _node)); } }

                    public class PVEConfig
                    {
                        private Client _client;
                        private object _node;
                        internal PVEConfig(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get Ceph configuration.
                        /// </summary>
                        public ExpandoObject Config() { return _client.Execute($"/nodes/{_node}/ceph/config", HttpMethod.Get); }
                    }

                    private PVEMon _mon;
                    public PVEMon Mon { get { return _mon ?? (_mon = new PVEMon(_client, node: _node)); } }

                    public class PVEMon
                    {
                        private Client _client;
                        private object _node;
                        internal PVEMon(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get Ceph monitor list.
                        /// </summary>
                        public ExpandoObject Listmon() { return _client.Execute($"/nodes/{_node}/ceph/mon", HttpMethod.Get); }

                        /// <summary>
                        /// Create Ceph Monitor
                        /// </summary>
                        public ExpandoObject Createmon() { return _client.Execute($"/nodes/{_node}/ceph/mon", HttpMethod.Post); }

                        public PVEItemMonid this[object monid] { get { return new PVEItemMonid(_client, node: _node, monid: monid); } }

                        public class PVEItemMonid
                        {
                            private Client _client;
                            private object _node;
                            private object _monid;
                            internal PVEItemMonid(Client client, object node, object monid)
                            {
                                _client = client;
                                _node = node;
                                _monid = monid;
                            }

                            /// <summary>
                            /// Destroy Ceph monitor.
                            /// </summary>
                            public ExpandoObject Destroymon() { return _client.Execute($"/nodes/{_node}/ceph/mon/{_monid}", HttpMethod.Delete); }
                        }
                    }

                    private PVEInit _init;
                    public PVEInit Init { get { return _init ?? (_init = new PVEInit(_client, node: _node)); } }

                    public class PVEInit
                    {
                        private Client _client;
                        private object _node;
                        internal PVEInit(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Create initial ceph default configuration and setup symlinks.
                        /// </summary>
                        /// <param name="disable_cephx">Disable cephx authentification.  WARNING: cephx is a security feature protecting against man-in-the-middle attacks. Only consider disabling cephx if your network is private!</param>
                        /// <param name="min_size">Minimum number of available replicas per object to allow I/O</param>
                        /// <param name="network">Use specific network for all ceph related traffic</param>
                        /// <param name="pg_bits">Placement group bits, used to specify the default number of placement groups.  NOTE: 'osd pool default pg num' does not work for default pools.</param>
                        /// <param name="size">Targeted number of replicas per object</param>
                        public void Init(bool? disable_cephx = null, int? min_size = null, string network = null, int? pg_bits = null, int? size = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("disable_cephx", disable_cephx);
                            parameters.Add("min_size", min_size);
                            parameters.Add("network", network);
                            parameters.Add("pg_bits", pg_bits);
                            parameters.Add("size", size);
                            _client.Execute($"/nodes/{_node}/ceph/init", HttpMethod.Post, parameters);
                        }
                    }

                    private PVEStop _stop;
                    public PVEStop Stop { get { return _stop ?? (_stop = new PVEStop(_client, node: _node)); } }

                    public class PVEStop
                    {
                        private Client _client;
                        private object _node;
                        internal PVEStop(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Stop ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        public ExpandoObject Stop(string service = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("service", service);
                            return _client.Execute($"/nodes/{_node}/ceph/stop", HttpMethod.Post, parameters);
                        }
                    }

                    private PVEStart _start;
                    public PVEStart Start { get { return _start ?? (_start = new PVEStart(_client, node: _node)); } }

                    public class PVEStart
                    {
                        private Client _client;
                        private object _node;
                        internal PVEStart(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Start ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        public ExpandoObject Start(string service = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("service", service);
                            return _client.Execute($"/nodes/{_node}/ceph/start", HttpMethod.Post, parameters);
                        }
                    }

                    private PVEStatus _status;
                    public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client, node: _node)); } }

                    public class PVEStatus
                    {
                        private Client _client;
                        private object _node;
                        internal PVEStatus(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get ceph status.
                        /// </summary>
                        public ExpandoObject Status() { return _client.Execute($"/nodes/{_node}/ceph/status", HttpMethod.Get); }
                    }

                    private PVEPools _pools;
                    public PVEPools Pools { get { return _pools ?? (_pools = new PVEPools(_client, node: _node)); } }

                    public class PVEPools
                    {
                        private Client _client;
                        private object _node;
                        internal PVEPools(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// List all pools.
                        /// </summary>
                        public ExpandoObject Lspools() { return _client.Execute($"/nodes/{_node}/ceph/pools", HttpMethod.Get); }

                        /// <summary>
                        /// Create POOL
                        /// </summary>
                        /// <param name="name">The name of the pool. It must be unique.</param>
                        /// <param name="crush_ruleset">The ruleset to use for mapping object placement in the cluster.</param>
                        /// <param name="min_size">Minimum number of replicas per object</param>
                        /// <param name="pg_num">Number of placement groups.</param>
                        /// <param name="size">Number of replicas per object</param>
                        public void Createpool(string name, int? crush_ruleset = null, int? min_size = null, int? pg_num = null, int? size = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("crush_ruleset", crush_ruleset);
                            parameters.Add("min_size", min_size);
                            parameters.Add("name", name);
                            parameters.Add("pg_num", pg_num);
                            parameters.Add("size", size);
                            _client.Execute($"/nodes/{_node}/ceph/pools", HttpMethod.Post, parameters);
                        }

                        public PVEItemName this[object name] { get { return new PVEItemName(_client, node: _node, name: name); } }

                        public class PVEItemName
                        {
                            private Client _client;
                            private object _node;
                            private object _name;
                            internal PVEItemName(Client client, object node, object name)
                            {
                                _client = client;
                                _node = node;
                                _name = name;
                            }

                            /// <summary>
                            /// Destroy pool
                            /// </summary>
                            /// <param name="force">If true, destroys pool even if in use</param>
                            public void Destroypool(bool? force = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                _client.Execute($"/nodes/{_node}/ceph/pools/{_name}", HttpMethod.Delete, parameters);
                            }
                        }
                    }

                    private PVEFlags _flags;
                    public PVEFlags Flags { get { return _flags ?? (_flags = new PVEFlags(_client, node: _node)); } }

                    public class PVEFlags
                    {
                        private Client _client;
                        private object _node;
                        internal PVEFlags(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// get all set ceph flags
                        /// </summary>
                        public ExpandoObject GetFlags() { return _client.Execute($"/nodes/{_node}/ceph/flags", HttpMethod.Get); }

                        public PVEItemFlag this[object flag] { get { return new PVEItemFlag(_client, node: _node, flag: flag); } }

                        public class PVEItemFlag
                        {
                            private Client _client;
                            private object _node;
                            private object _flag;
                            internal PVEItemFlag(Client client, object node, object flag)
                            {
                                _client = client;
                                _node = node;
                                _flag = flag;
                            }

                            /// <summary>
                            /// Unset a ceph flag
                            /// </summary>
                            public void UnsetFlag() { _client.Execute($"/nodes/{_node}/ceph/flags/{_flag}", HttpMethod.Delete); }

                            /// <summary>
                            /// Set a ceph flag
                            /// </summary>
                            public void SetFlag() { _client.Execute($"/nodes/{_node}/ceph/flags/{_flag}", HttpMethod.Post); }
                        }
                    }

                    private PVECrush _crush;
                    public PVECrush Crush { get { return _crush ?? (_crush = new PVECrush(_client, node: _node)); } }

                    public class PVECrush
                    {
                        private Client _client;
                        private object _node;
                        internal PVECrush(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get OSD crush map
                        /// </summary>
                        public ExpandoObject Crush() { return _client.Execute($"/nodes/{_node}/ceph/crush", HttpMethod.Get); }
                    }

                    private PVELog _log;
                    public PVELog Log { get { return _log ?? (_log = new PVELog(_client, node: _node)); } }

                    public class PVELog
                    {
                        private Client _client;
                        private object _node;
                        internal PVELog(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Read ceph log
                        /// </summary>
                        /// <param name="limit"></param>
                        /// <param name="start"></param>
                        public ExpandoObject Log(int? limit = null, int? start = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("limit", limit);
                            parameters.Add("start", start);
                            return _client.Execute($"/nodes/{_node}/ceph/log", HttpMethod.Get, parameters);
                        }
                    }
                }

                private PVEVzdump _vzdump;
                public PVEVzdump Vzdump { get { return _vzdump ?? (_vzdump = new PVEVzdump(_client, node: _node)); } }

                public class PVEVzdump
                {
                    private Client _client;
                    private object _node;
                    internal PVEVzdump(Client client, object node)
                    {
                        _client = client;
                        _node = node;
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
                    /// <param name="pigz">Use pigz instead of gzip when N>0. N=1 uses half of cores, N>1 uses N as thread count.</param>
                    /// <param name="quiet">Be quiet.</param>
                    /// <param name="remove">Remove old backup files if there are more than 'maxfiles' backup files.</param>
                    /// <param name="script">Use specified hook script.</param>
                    /// <param name="size">Unused, will be removed in a future release.</param>
                    /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                    /// <param name="stdout">Write tar to stdout, not to a file.</param>
                    /// <param name="stop">Stop runnig backup jobs on this host.</param>
                    /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                    /// <param name="storage">Store resulting file to this storage.</param>
                    /// <param name="tmpdir">Store temporary files to specified directory.</param>
                    /// <param name="vmid">The ID of the guest system you want to backup.</param>
                    public ExpandoObject Vzdump(bool? all = null, int? bwlimit = null, string compress = null, string dumpdir = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stdout = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
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
                        return _client.Execute($"/nodes/{_node}/vzdump", HttpMethod.Post, parameters);
                    }

                    private PVEExtractconfig _extractconfig;
                    public PVEExtractconfig Extractconfig { get { return _extractconfig ?? (_extractconfig = new PVEExtractconfig(_client, node: _node)); } }

                    public class PVEExtractconfig
                    {
                        private Client _client;
                        private object _node;
                        internal PVEExtractconfig(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Extract configuration from vzdump backup archive.
                        /// </summary>
                        /// <param name="volume">Volume identifier</param>
                        public ExpandoObject Extractconfig(string volume)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("volume", volume);
                            return _client.Execute($"/nodes/{_node}/vzdump/extractconfig", HttpMethod.Get, parameters);
                        }
                    }
                }

                private PVEServices _services;
                public PVEServices Services { get { return _services ?? (_services = new PVEServices(_client, node: _node)); } }

                public class PVEServices
                {
                    private Client _client;
                    private object _node;
                    internal PVEServices(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Service list.
                    /// </summary>
                    public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/services", HttpMethod.Get); }

                    public PVEItemService this[object service] { get { return new PVEItemService(_client, node: _node, service: service); } }

                    public class PVEItemService
                    {
                        private Client _client;
                        private object _node;
                        private object _service;
                        internal PVEItemService(Client client, object node, object service)
                        {
                            _client = client;
                            _node = node;
                            _service = service;
                        }

                        /// <summary>
                        /// Directory index
                        /// </summary>
                        public ExpandoObject Srvcmdidx() { return _client.Execute($"/nodes/{_node}/services/{_service}", HttpMethod.Get); }

                        private PVEState _state;
                        public PVEState State { get { return _state ?? (_state = new PVEState(_client, node: _node, service: _service)); } }

                        public class PVEState
                        {
                            private Client _client;
                            private object _node;
                            private object _service;
                            internal PVEState(Client client, object node, object service)
                            {
                                _client = client;
                                _node = node;
                                _service = service;
                            }

                            /// <summary>
                            /// Read service properties
                            /// </summary>
                            public ExpandoObject ServiceState() { return _client.Execute($"/nodes/{_node}/services/{_service}/state", HttpMethod.Get); }
                        }

                        private PVEStart _start;
                        public PVEStart Start { get { return _start ?? (_start = new PVEStart(_client, node: _node, service: _service)); } }

                        public class PVEStart
                        {
                            private Client _client;
                            private object _node;
                            private object _service;
                            internal PVEStart(Client client, object node, object service)
                            {
                                _client = client;
                                _node = node;
                                _service = service;
                            }

                            /// <summary>
                            /// Start service.
                            /// </summary>
                            public ExpandoObject ServiceStart() { return _client.Execute($"/nodes/{_node}/services/{_service}/start", HttpMethod.Post); }
                        }

                        private PVEStop _stop;
                        public PVEStop Stop { get { return _stop ?? (_stop = new PVEStop(_client, node: _node, service: _service)); } }

                        public class PVEStop
                        {
                            private Client _client;
                            private object _node;
                            private object _service;
                            internal PVEStop(Client client, object node, object service)
                            {
                                _client = client;
                                _node = node;
                                _service = service;
                            }

                            /// <summary>
                            /// Stop service.
                            /// </summary>
                            public ExpandoObject ServiceStop() { return _client.Execute($"/nodes/{_node}/services/{_service}/stop", HttpMethod.Post); }
                        }

                        private PVERestart _restart;
                        public PVERestart Restart { get { return _restart ?? (_restart = new PVERestart(_client, node: _node, service: _service)); } }

                        public class PVERestart
                        {
                            private Client _client;
                            private object _node;
                            private object _service;
                            internal PVERestart(Client client, object node, object service)
                            {
                                _client = client;
                                _node = node;
                                _service = service;
                            }

                            /// <summary>
                            /// Restart service.
                            /// </summary>
                            public ExpandoObject ServiceRestart() { return _client.Execute($"/nodes/{_node}/services/{_service}/restart", HttpMethod.Post); }
                        }

                        private PVEReload _reload;
                        public PVEReload Reload { get { return _reload ?? (_reload = new PVEReload(_client, node: _node, service: _service)); } }

                        public class PVEReload
                        {
                            private Client _client;
                            private object _node;
                            private object _service;
                            internal PVEReload(Client client, object node, object service)
                            {
                                _client = client;
                                _node = node;
                                _service = service;
                            }

                            /// <summary>
                            /// Reload service.
                            /// </summary>
                            public ExpandoObject ServiceReload() { return _client.Execute($"/nodes/{_node}/services/{_service}/reload", HttpMethod.Post); }
                        }
                    }
                }

                private PVESubscription _subscription;
                public PVESubscription Subscription { get { return _subscription ?? (_subscription = new PVESubscription(_client, node: _node)); } }

                public class PVESubscription
                {
                    private Client _client;
                    private object _node;
                    internal PVESubscription(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read subscription info.
                    /// </summary>
                    public ExpandoObject Get() { return _client.Execute($"/nodes/{_node}/subscription", HttpMethod.Get); }

                    /// <summary>
                    /// Update subscription info.
                    /// </summary>
                    /// <param name="force">Always connect to server, even if we have up to date info inside local cache.</param>
                    public void Update(bool? force = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        _client.Execute($"/nodes/{_node}/subscription", HttpMethod.Post, parameters);
                    }

                    /// <summary>
                    /// Set subscription key.
                    /// </summary>
                    /// <param name="key">Proxmox VE subscription key</param>
                    public void Set(string key)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("key", key);
                        _client.Execute($"/nodes/{_node}/subscription", HttpMethod.Put, parameters);
                    }
                }

                private PVENetwork _network;
                public PVENetwork Network { get { return _network ?? (_network = new PVENetwork(_client, node: _node)); } }

                public class PVENetwork
                {
                    private Client _client;
                    private object _node;
                    internal PVENetwork(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Revert network configuration changes.
                    /// </summary>
                    public void RevertNetworkChanges() { _client.Execute($"/nodes/{_node}/network", HttpMethod.Delete); }

                    /// <summary>
                    /// List available networks
                    /// </summary>
                    /// <param name="type">Only list specific interface types.
                    ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,any_bridge</param>
                    public ExpandoObject Index(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return _client.Execute($"/nodes/{_node}/network", HttpMethod.Get, parameters);
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
                    /// <param name="bridge_ports">Specify the iterfaces you want to add to your bridge.</param>
                    /// <param name="bridge_vlan_aware">Enable bridge vlan support.</param>
                    /// <param name="comments">Comments</param>
                    /// <param name="comments6">Comments</param>
                    /// <param name="gateway">Default gateway address.</param>
                    /// <param name="gateway6">Default ipv6 gateway address.</param>
                    /// <param name="netmask">Network mask.</param>
                    /// <param name="netmask6">Network mask.</param>
                    /// <param name="ovs_bonds">Specify the interfaces used by the bonding device.</param>
                    /// <param name="ovs_bridge">The OVS bridge associated with a OVS port. This is required when you create an OVS port.</param>
                    /// <param name="ovs_options">OVS interface options.</param>
                    /// <param name="ovs_ports">Specify the iterfaces you want to add to your bridge.</param>
                    /// <param name="ovs_tag">Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)</param>
                    /// <param name="slaves">Specify the interfaces used by the bonding device.</param>
                    public void CreateNetwork(string iface, string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string comments = null, string comments6 = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("address", address);
                        parameters.Add("address6", address6);
                        parameters.Add("autostart", autostart);
                        parameters.Add("bond_mode", bond_mode);
                        parameters.Add("bond_xmit_hash_policy", bond_xmit_hash_policy);
                        parameters.Add("bridge_ports", bridge_ports);
                        parameters.Add("bridge_vlan_aware", bridge_vlan_aware);
                        parameters.Add("comments", comments);
                        parameters.Add("comments6", comments6);
                        parameters.Add("gateway", gateway);
                        parameters.Add("gateway6", gateway6);
                        parameters.Add("iface", iface);
                        parameters.Add("netmask", netmask);
                        parameters.Add("netmask6", netmask6);
                        parameters.Add("ovs_bonds", ovs_bonds);
                        parameters.Add("ovs_bridge", ovs_bridge);
                        parameters.Add("ovs_options", ovs_options);
                        parameters.Add("ovs_ports", ovs_ports);
                        parameters.Add("ovs_tag", ovs_tag);
                        parameters.Add("slaves", slaves);
                        parameters.Add("type", type);
                        _client.Execute($"/nodes/{_node}/network", HttpMethod.Post, parameters);
                    }

                    public PVEItemIface this[object iface] { get { return new PVEItemIface(_client, node: _node, iface: iface); } }

                    public class PVEItemIface
                    {
                        private Client _client;
                        private object _node;
                        private object _iface;
                        internal PVEItemIface(Client client, object node, object iface)
                        {
                            _client = client;
                            _node = node;
                            _iface = iface;
                        }

                        /// <summary>
                        /// Delete network device configuration
                        /// </summary>
                        public void DeleteNetwork() { _client.Execute($"/nodes/{_node}/network/{_iface}", HttpMethod.Delete); }

                        /// <summary>
                        /// Read network device configuration
                        /// </summary>
                        public ExpandoObject NetworkConfig() { return _client.Execute($"/nodes/{_node}/network/{_iface}", HttpMethod.Get); }

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
                        /// <param name="bridge_ports">Specify the iterfaces you want to add to your bridge.</param>
                        /// <param name="bridge_vlan_aware">Enable bridge vlan support.</param>
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
                        /// <param name="ovs_ports">Specify the iterfaces you want to add to your bridge.</param>
                        /// <param name="ovs_tag">Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)</param>
                        /// <param name="slaves">Specify the interfaces used by the bonding device.</param>
                        public void UpdateNetwork(string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string comments = null, string comments6 = null, string delete = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("address", address);
                            parameters.Add("address6", address6);
                            parameters.Add("autostart", autostart);
                            parameters.Add("bond_mode", bond_mode);
                            parameters.Add("bond_xmit_hash_policy", bond_xmit_hash_policy);
                            parameters.Add("bridge_ports", bridge_ports);
                            parameters.Add("bridge_vlan_aware", bridge_vlan_aware);
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
                            parameters.Add("type", type);
                            _client.Execute($"/nodes/{_node}/network/{_iface}", HttpMethod.Put, parameters);
                        }
                    }
                }

                private PVETasks _tasks;
                public PVETasks Tasks { get { return _tasks ?? (_tasks = new PVETasks(_client, node: _node)); } }

                public class PVETasks
                {
                    private Client _client;
                    private object _node;
                    internal PVETasks(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read task list for one node (finished tasks).
                    /// </summary>
                    /// <param name="errors"></param>
                    /// <param name="limit"></param>
                    /// <param name="start"></param>
                    /// <param name="userfilter"></param>
                    /// <param name="vmid">Only list tasks for this VM.</param>
                    public ExpandoObject NodeTasks(bool? errors = null, int? limit = null, int? start = null, string userfilter = null, int? vmid = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("errors", errors);
                        parameters.Add("limit", limit);
                        parameters.Add("start", start);
                        parameters.Add("userfilter", userfilter);
                        parameters.Add("vmid", vmid);
                        return _client.Execute($"/nodes/{_node}/tasks", HttpMethod.Get, parameters);
                    }

                    public PVEItemUpid this[object upid] { get { return new PVEItemUpid(_client, node: _node, upid: upid); } }

                    public class PVEItemUpid
                    {
                        private Client _client;
                        private object _node;
                        private object _upid;
                        internal PVEItemUpid(Client client, object node, object upid)
                        {
                            _client = client;
                            _node = node;
                            _upid = upid;
                        }

                        /// <summary>
                        /// Stop a task.
                        /// </summary>
                        public void StopTask() { _client.Execute($"/nodes/{_node}/tasks/{_upid}", HttpMethod.Delete); }

                        /// <summary>
                        /// 
                        /// </summary>
                        public ExpandoObject UpidIndex() { return _client.Execute($"/nodes/{_node}/tasks/{_upid}", HttpMethod.Get); }

                        private PVELog _log;
                        public PVELog Log { get { return _log ?? (_log = new PVELog(_client, node: _node, upid: _upid)); } }

                        public class PVELog
                        {
                            private Client _client;
                            private object _node;
                            private object _upid;
                            internal PVELog(Client client, object node, object upid)
                            {
                                _client = client;
                                _node = node;
                                _upid = upid;
                            }

                            /// <summary>
                            /// Read task log.
                            /// </summary>
                            /// <param name="limit"></param>
                            /// <param name="start"></param>
                            public ExpandoObject ReadTaskLog(int? limit = null, int? start = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("limit", limit);
                                parameters.Add("start", start);
                                return _client.Execute($"/nodes/{_node}/tasks/{_upid}/log", HttpMethod.Get, parameters);
                            }
                        }

                        private PVEStatus _status;
                        public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client, node: _node, upid: _upid)); } }

                        public class PVEStatus
                        {
                            private Client _client;
                            private object _node;
                            private object _upid;
                            internal PVEStatus(Client client, object node, object upid)
                            {
                                _client = client;
                                _node = node;
                                _upid = upid;
                            }

                            /// <summary>
                            /// Read task status.
                            /// </summary>
                            public ExpandoObject ReadTaskStatus() { return _client.Execute($"/nodes/{_node}/tasks/{_upid}/status", HttpMethod.Get); }
                        }
                    }
                }

                private PVEScan _scan;
                public PVEScan Scan { get { return _scan ?? (_scan = new PVEScan(_client, node: _node)); } }

                public class PVEScan
                {
                    private Client _client;
                    private object _node;
                    internal PVEScan(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Index of available scan methods
                    /// </summary>
                    public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/scan", HttpMethod.Get); }

                    private PVEZfs _zfs;
                    public PVEZfs Zfs { get { return _zfs ?? (_zfs = new PVEZfs(_client, node: _node)); } }

                    public class PVEZfs
                    {
                        private Client _client;
                        private object _node;
                        internal PVEZfs(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Scan zfs pool list on local node.
                        /// </summary>
                        public ExpandoObject Zfsscan() { return _client.Execute($"/nodes/{_node}/scan/zfs", HttpMethod.Get); }
                    }

                    private PVENfs _nfs;
                    public PVENfs Nfs { get { return _nfs ?? (_nfs = new PVENfs(_client, node: _node)); } }

                    public class PVENfs
                    {
                        private Client _client;
                        private object _node;
                        internal PVENfs(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Scan remote NFS server.
                        /// </summary>
                        /// <param name="server"></param>
                        public ExpandoObject Nfsscan(string server)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("server", server);
                            return _client.Execute($"/nodes/{_node}/scan/nfs", HttpMethod.Get, parameters);
                        }
                    }

                    private PVEGlusterfs _glusterfs;
                    public PVEGlusterfs Glusterfs { get { return _glusterfs ?? (_glusterfs = new PVEGlusterfs(_client, node: _node)); } }

                    public class PVEGlusterfs
                    {
                        private Client _client;
                        private object _node;
                        internal PVEGlusterfs(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Scan remote GlusterFS server.
                        /// </summary>
                        /// <param name="server"></param>
                        public ExpandoObject Glusterfsscan(string server)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("server", server);
                            return _client.Execute($"/nodes/{_node}/scan/glusterfs", HttpMethod.Get, parameters);
                        }
                    }

                    private PVEIscsi _iscsi;
                    public PVEIscsi Iscsi { get { return _iscsi ?? (_iscsi = new PVEIscsi(_client, node: _node)); } }

                    public class PVEIscsi
                    {
                        private Client _client;
                        private object _node;
                        internal PVEIscsi(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Scan remote iSCSI server.
                        /// </summary>
                        /// <param name="portal"></param>
                        public ExpandoObject Iscsiscan(string portal)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("portal", portal);
                            return _client.Execute($"/nodes/{_node}/scan/iscsi", HttpMethod.Get, parameters);
                        }
                    }

                    private PVELvm _lvm;
                    public PVELvm Lvm { get { return _lvm ?? (_lvm = new PVELvm(_client, node: _node)); } }

                    public class PVELvm
                    {
                        private Client _client;
                        private object _node;
                        internal PVELvm(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// List local LVM volume groups.
                        /// </summary>
                        public ExpandoObject Lvmscan() { return _client.Execute($"/nodes/{_node}/scan/lvm", HttpMethod.Get); }
                    }

                    private PVELvmthin _lvmthin;
                    public PVELvmthin Lvmthin { get { return _lvmthin ?? (_lvmthin = new PVELvmthin(_client, node: _node)); } }

                    public class PVELvmthin
                    {
                        private Client _client;
                        private object _node;
                        internal PVELvmthin(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// List local LVM Thin Pools.
                        /// </summary>
                        /// <param name="vg"></param>
                        public ExpandoObject Lvmthinscan(string vg)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("vg", vg);
                            return _client.Execute($"/nodes/{_node}/scan/lvmthin", HttpMethod.Get, parameters);
                        }
                    }

                    private PVEUsb _usb;
                    public PVEUsb Usb { get { return _usb ?? (_usb = new PVEUsb(_client, node: _node)); } }

                    public class PVEUsb
                    {
                        private Client _client;
                        private object _node;
                        internal PVEUsb(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// List local USB devices.
                        /// </summary>
                        public ExpandoObject Usbscan() { return _client.Execute($"/nodes/{_node}/scan/usb", HttpMethod.Get); }
                    }
                }

                private PVEStorage _storage;
                public PVEStorage Storage { get { return _storage ?? (_storage = new PVEStorage(_client, node: _node)); } }

                public class PVEStorage
                {
                    private Client _client;
                    private object _node;
                    internal PVEStorage(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Get status for all datastores.
                    /// </summary>
                    /// <param name="content">Only list stores which support this content type.</param>
                    /// <param name="enabled">Only list stores which are enabled (not disabled in config).</param>
                    /// <param name="storage">Only list status for  specified storage</param>
                    /// <param name="target">If target is different to 'node', we only lists shared storages which content is accessible on this 'node' and the specified 'target' node.</param>
                    public ExpandoObject Index(string content = null, bool? enabled = null, string storage = null, string target = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("content", content);
                        parameters.Add("enabled", enabled);
                        parameters.Add("storage", storage);
                        parameters.Add("target", target);
                        return _client.Execute($"/nodes/{_node}/storage", HttpMethod.Get, parameters);
                    }

                    public PVEItemStorage this[object storage] { get { return new PVEItemStorage(_client, node: _node, storage: storage); } }

                    public class PVEItemStorage
                    {
                        private Client _client;
                        private object _node;
                        private object _storage;
                        internal PVEItemStorage(Client client, object node, object storage)
                        {
                            _client = client;
                            _node = node;
                            _storage = storage;
                        }

                        /// <summary>
                        /// 
                        /// </summary>
                        public ExpandoObject Diridx() { return _client.Execute($"/nodes/{_node}/storage/{_storage}", HttpMethod.Get); }

                        private PVEContent _content;
                        public PVEContent Content { get { return _content ?? (_content = new PVEContent(_client, node: _node, storage: _storage)); } }

                        public class PVEContent
                        {
                            private Client _client;
                            private object _node;
                            private object _storage;
                            internal PVEContent(Client client, object node, object storage)
                            {
                                _client = client;
                                _node = node;
                                _storage = storage;
                            }

                            /// <summary>
                            /// List storage content.
                            /// </summary>
                            /// <param name="content">Only list content of this type.</param>
                            /// <param name="vmid">Only list images for this VM</param>
                            public ExpandoObject Index(string content = null, int? vmid = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("content", content);
                                parameters.Add("vmid", vmid);
                                return _client.Execute($"/nodes/{_node}/storage/{_storage}/content", HttpMethod.Get, parameters);
                            }

                            /// <summary>
                            /// Allocate disk images.
                            /// </summary>
                            /// <param name="filename">The name of the file to create.</param>
                            /// <param name="size">Size in kilobyte (1024 bytes). Optional suffixes 'M' (megabyte, 1024K) and 'G' (gigabyte, 1024M)</param>
                            /// <param name="vmid">Specify owner VM</param>
                            /// <param name="format">
                            ///   Enum: raw,qcow2,subvol</param>
                            public ExpandoObject Create(string filename, string size, int vmid, string format = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("filename", filename);
                                parameters.Add("format", format);
                                parameters.Add("size", size);
                                parameters.Add("vmid", vmid);
                                return _client.Execute($"/nodes/{_node}/storage/{_storage}/content", HttpMethod.Post, parameters);
                            }

                            public PVEItemVolume this[object volume] { get { return new PVEItemVolume(_client, node: _node, storage: _storage, volume: volume); } }

                            public class PVEItemVolume
                            {
                                private Client _client;
                                private object _node;
                                private object _storage;
                                private object _volume;
                                internal PVEItemVolume(Client client, object node, object storage, object volume)
                                {
                                    _client = client;
                                    _node = node;
                                    _storage = storage;
                                    _volume = volume;
                                }

                                /// <summary>
                                /// Delete volume
                                /// </summary>
                                public void Delete() { _client.Execute($"/nodes/{_node}/storage/{_storage}/content/{_volume}", HttpMethod.Delete); }

                                /// <summary>
                                /// Get volume attributes
                                /// </summary>
                                public ExpandoObject Info() { return _client.Execute($"/nodes/{_node}/storage/{_storage}/content/{_volume}", HttpMethod.Get); }

                                /// <summary>
                                /// Copy a volume. This is experimental code - do not use.
                                /// </summary>
                                /// <param name="target">Target volume identifier</param>
                                /// <param name="target_node">Target node. Default is local node.</param>
                                public ExpandoObject Copy(string target, string target_node = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("target", target);
                                    parameters.Add("target_node", target_node);
                                    return _client.Execute($"/nodes/{_node}/storage/{_storage}/content/{_volume}", HttpMethod.Post, parameters);
                                }
                            }
                        }

                        private PVEStatus _status;
                        public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client, node: _node, storage: _storage)); } }

                        public class PVEStatus
                        {
                            private Client _client;
                            private object _node;
                            private object _storage;
                            internal PVEStatus(Client client, object node, object storage)
                            {
                                _client = client;
                                _node = node;
                                _storage = storage;
                            }

                            /// <summary>
                            /// Read storage status.
                            /// </summary>
                            public ExpandoObject ReadStatus() { return _client.Execute($"/nodes/{_node}/storage/{_storage}/status", HttpMethod.Get); }
                        }

                        private PVERrd _rrd;
                        public PVERrd Rrd { get { return _rrd ?? (_rrd = new PVERrd(_client, node: _node, storage: _storage)); } }

                        public class PVERrd
                        {
                            private Client _client;
                            private object _node;
                            private object _storage;
                            internal PVERrd(Client client, object node, object storage)
                            {
                                _client = client;
                                _node = node;
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
                            public ExpandoObject Rrd(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cf", cf);
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                return _client.Execute($"/nodes/{_node}/storage/{_storage}/rrd", HttpMethod.Get, parameters);
                            }
                        }

                        private PVERrddata _rrddata;
                        public PVERrddata Rrddata { get { return _rrddata ?? (_rrddata = new PVERrddata(_client, node: _node, storage: _storage)); } }

                        public class PVERrddata
                        {
                            private Client _client;
                            private object _node;
                            private object _storage;
                            internal PVERrddata(Client client, object node, object storage)
                            {
                                _client = client;
                                _node = node;
                                _storage = storage;
                            }

                            /// <summary>
                            /// Read storage RRD statistics.
                            /// </summary>
                            /// <param name="timeframe">Specify the time frame you are interested in.
                            ///   Enum: hour,day,week,month,year</param>
                            /// <param name="cf">The RRD consolidation function
                            ///   Enum: AVERAGE,MAX</param>
                            public ExpandoObject Rrddata(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cf", cf);
                                parameters.Add("timeframe", timeframe);
                                return _client.Execute($"/nodes/{_node}/storage/{_storage}/rrddata", HttpMethod.Get, parameters);
                            }
                        }

                        private PVEUpload _upload;
                        public PVEUpload Upload { get { return _upload ?? (_upload = new PVEUpload(_client, node: _node, storage: _storage)); } }

                        public class PVEUpload
                        {
                            private Client _client;
                            private object _node;
                            private object _storage;
                            internal PVEUpload(Client client, object node, object storage)
                            {
                                _client = client;
                                _node = node;
                                _storage = storage;
                            }

                            /// <summary>
                            /// Upload templates and ISO images.
                            /// </summary>
                            /// <param name="content">Content type.</param>
                            /// <param name="filename">The name of the file to create.</param>
                            /// <param name="tmpfilename">The source file name. This parameter is usually set by the REST handler. You can only overwrite it when connecting to the trustet port on localhost.</param>
                            public ExpandoObject Upload(string content, string filename, string tmpfilename = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("content", content);
                                parameters.Add("filename", filename);
                                parameters.Add("tmpfilename", tmpfilename);
                                return _client.Execute($"/nodes/{_node}/storage/{_storage}/upload", HttpMethod.Post, parameters);
                            }
                        }
                    }
                }

                private PVEDisks _disks;
                public PVEDisks Disks { get { return _disks ?? (_disks = new PVEDisks(_client, node: _node)); } }

                public class PVEDisks
                {
                    private Client _client;
                    private object _node;
                    internal PVEDisks(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Node index.
                    /// </summary>
                    public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/disks", HttpMethod.Get); }

                    private PVEList _list;
                    public PVEList List { get { return _list ?? (_list = new PVEList(_client, node: _node)); } }

                    public class PVEList
                    {
                        private Client _client;
                        private object _node;
                        internal PVEList(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        public ExpandoObject List() { return _client.Execute($"/nodes/{_node}/disks/list", HttpMethod.Get); }
                    }

                    private PVESmart _smart;
                    public PVESmart Smart { get { return _smart ?? (_smart = new PVESmart(_client, node: _node)); } }

                    public class PVESmart
                    {
                        private Client _client;
                        private object _node;
                        internal PVESmart(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get SMART Health of a disk.
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <param name="healthonly">If true returns only the health status</param>
                        public ExpandoObject Smart(string disk, bool? healthonly = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("disk", disk);
                            parameters.Add("healthonly", healthonly);
                            return _client.Execute($"/nodes/{_node}/disks/smart", HttpMethod.Get, parameters);
                        }
                    }

                    private PVEInitgpt _initgpt;
                    public PVEInitgpt Initgpt { get { return _initgpt ?? (_initgpt = new PVEInitgpt(_client, node: _node)); } }

                    public class PVEInitgpt
                    {
                        private Client _client;
                        private object _node;
                        internal PVEInitgpt(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Initialize Disk with GPT
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <param name="uuid">UUID for the GPT table</param>
                        public ExpandoObject Initgpt(string disk, string uuid = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("disk", disk);
                            parameters.Add("uuid", uuid);
                            return _client.Execute($"/nodes/{_node}/disks/initgpt", HttpMethod.Post, parameters);
                        }
                    }
                }

                private PVEApt _apt;
                public PVEApt Apt { get { return _apt ?? (_apt = new PVEApt(_client, node: _node)); } }

                public class PVEApt
                {
                    private Client _client;
                    private object _node;
                    internal PVEApt(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Directory index for apt (Advanced Package Tool).
                    /// </summary>
                    public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/apt", HttpMethod.Get); }

                    private PVEUpdate _update;
                    public PVEUpdate Update { get { return _update ?? (_update = new PVEUpdate(_client, node: _node)); } }

                    public class PVEUpdate
                    {
                        private Client _client;
                        private object _node;
                        internal PVEUpdate(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// List available updates.
                        /// </summary>
                        public ExpandoObject ListUpdates() { return _client.Execute($"/nodes/{_node}/apt/update", HttpMethod.Get); }

                        /// <summary>
                        /// This is used to resynchronize the package index files from their sources (apt-get update).
                        /// </summary>
                        /// <param name="notify">Send notification mail about new packages (to email address specified for user 'root@pam').</param>
                        /// <param name="quiet">Only produces output suitable for logging, omitting progress indicators.</param>
                        public ExpandoObject UpdateDatabase(bool? notify = null, bool? quiet = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("notify", notify);
                            parameters.Add("quiet", quiet);
                            return _client.Execute($"/nodes/{_node}/apt/update", HttpMethod.Post, parameters);
                        }
                    }

                    private PVEChangelog _changelog;
                    public PVEChangelog Changelog { get { return _changelog ?? (_changelog = new PVEChangelog(_client, node: _node)); } }

                    public class PVEChangelog
                    {
                        private Client _client;
                        private object _node;
                        internal PVEChangelog(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get package changelogs.
                        /// </summary>
                        /// <param name="name">Package name.</param>
                        /// <param name="version">Package version.</param>
                        public ExpandoObject Changelog(string name, string version = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("name", name);
                            parameters.Add("version", version);
                            return _client.Execute($"/nodes/{_node}/apt/changelog", HttpMethod.Get, parameters);
                        }
                    }

                    private PVEVersions _versions;
                    public PVEVersions Versions { get { return _versions ?? (_versions = new PVEVersions(_client, node: _node)); } }

                    public class PVEVersions
                    {
                        private Client _client;
                        private object _node;
                        internal PVEVersions(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get package information for important Proxmox packages.
                        /// </summary>
                        public ExpandoObject Versions() { return _client.Execute($"/nodes/{_node}/apt/versions", HttpMethod.Get); }
                    }
                }

                private PVEFirewall _firewall;
                public PVEFirewall Firewall { get { return _firewall ?? (_firewall = new PVEFirewall(_client, node: _node)); } }

                public class PVEFirewall
                {
                    private Client _client;
                    private object _node;
                    internal PVEFirewall(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/firewall", HttpMethod.Get); }

                    private PVERules _rules;
                    public PVERules Rules { get { return _rules ?? (_rules = new PVERules(_client, node: _node)); } }

                    public class PVERules
                    {
                        private Client _client;
                        private object _node;
                        internal PVERules(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// List rules.
                        /// </summary>
                        public ExpandoObject GetRules() { return _client.Execute($"/nodes/{_node}/firewall/rules", HttpMethod.Get); }

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
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos>.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        public void CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("comment", comment);
                            parameters.Add("dest", dest);
                            parameters.Add("digest", digest);
                            parameters.Add("dport", dport);
                            parameters.Add("enable", enable);
                            parameters.Add("iface", iface);
                            parameters.Add("macro", macro);
                            parameters.Add("pos", pos);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            parameters.Add("type", type);
                            _client.Execute($"/nodes/{_node}/firewall/rules", HttpMethod.Post, parameters);
                        }

                        public PVEItemPos this[object pos] { get { return new PVEItemPos(_client, node: _node, pos: pos); } }

                        public class PVEItemPos
                        {
                            private Client _client;
                            private object _node;
                            private object _pos;
                            internal PVEItemPos(Client client, object node, object pos)
                            {
                                _client = client;
                                _node = node;
                                _pos = pos;
                            }

                            /// <summary>
                            /// Delete rule.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            public void DeleteRule(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                _client.Execute($"/nodes/{_node}/firewall/rules/{_pos}", HttpMethod.Delete, parameters);
                            }

                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            public ExpandoObject GetRule() { return _client.Execute($"/nodes/{_node}/firewall/rules/{_pos}", HttpMethod.Get); }

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
                            /// <param name="macro">Use predefined standard macro.</param>
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto>. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            public void UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                                parameters.Add("macro", macro);
                                parameters.Add("moveto", moveto);
                                parameters.Add("proto", proto);
                                parameters.Add("source", source);
                                parameters.Add("sport", sport);
                                parameters.Add("type", type);
                                _client.Execute($"/nodes/{_node}/firewall/rules/{_pos}", HttpMethod.Put, parameters);
                            }
                        }
                    }

                    private PVEOptions _options;
                    public PVEOptions Options { get { return _options ?? (_options = new PVEOptions(_client, node: _node)); } }

                    public class PVEOptions
                    {
                        private Client _client;
                        private object _node;
                        internal PVEOptions(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Get host firewall options.
                        /// </summary>
                        public ExpandoObject GetOptions() { return _client.Execute($"/nodes/{_node}/firewall/options", HttpMethod.Get); }

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
                        /// <param name="ndp">Enable NDP.</param>
                        /// <param name="nf_conntrack_max">Maximum number of tracked connections.</param>
                        /// <param name="nf_conntrack_tcp_timeout_established">Conntrack established timeout.</param>
                        /// <param name="nosmurfs">Enable SMURFS filter.</param>
                        /// <param name="smurf_log_level">Log level for SMURFS filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcp_flags_log_level">Log level for illegal tcp flags filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcpflags">Filter illegal combinations of TCP flags.</param>
                        public void SetOptions(string delete = null, string digest = null, bool? enable = null, string log_level_in = null, string log_level_out = null, bool? ndp = null, int? nf_conntrack_max = null, int? nf_conntrack_tcp_timeout_established = null, bool? nosmurfs = null, string smurf_log_level = null, string tcp_flags_log_level = null, bool? tcpflags = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("enable", enable);
                            parameters.Add("log_level_in", log_level_in);
                            parameters.Add("log_level_out", log_level_out);
                            parameters.Add("ndp", ndp);
                            parameters.Add("nf_conntrack_max", nf_conntrack_max);
                            parameters.Add("nf_conntrack_tcp_timeout_established", nf_conntrack_tcp_timeout_established);
                            parameters.Add("nosmurfs", nosmurfs);
                            parameters.Add("smurf_log_level", smurf_log_level);
                            parameters.Add("tcp_flags_log_level", tcp_flags_log_level);
                            parameters.Add("tcpflags", tcpflags);
                            _client.Execute($"/nodes/{_node}/firewall/options", HttpMethod.Put, parameters);
                        }
                    }

                    private PVELog _log;
                    public PVELog Log { get { return _log ?? (_log = new PVELog(_client, node: _node)); } }

                    public class PVELog
                    {
                        private Client _client;
                        private object _node;
                        internal PVELog(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }

                        /// <summary>
                        /// Read firewall log
                        /// </summary>
                        /// <param name="limit"></param>
                        /// <param name="start"></param>
                        public ExpandoObject Log(int? limit = null, int? start = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("limit", limit);
                            parameters.Add("start", start);
                            return _client.Execute($"/nodes/{_node}/firewall/log", HttpMethod.Get, parameters);
                        }
                    }
                }

                private PVEReplication _replication;
                public PVEReplication Replication { get { return _replication ?? (_replication = new PVEReplication(_client, node: _node)); } }

                public class PVEReplication
                {
                    private Client _client;
                    private object _node;
                    internal PVEReplication(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// List status of all replication jobs on this node.
                    /// </summary>
                    /// <param name="guest">Only list replication jobs for this guest.</param>
                    public ExpandoObject Status(int? guest = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("guest", guest);
                        return _client.Execute($"/nodes/{_node}/replication", HttpMethod.Get, parameters);
                    }

                    public PVEItemId this[object id] { get { return new PVEItemId(_client, node: _node, id: id); } }

                    public class PVEItemId
                    {
                        private Client _client;
                        private object _node;
                        private object _id;
                        internal PVEItemId(Client client, object node, object id)
                        {
                            _client = client;
                            _node = node;
                            _id = id;
                        }

                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        public ExpandoObject Index() { return _client.Execute($"/nodes/{_node}/replication/{_id}", HttpMethod.Get); }

                        private PVEStatus _status;
                        public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client, node: _node, id: _id)); } }

                        public class PVEStatus
                        {
                            private Client _client;
                            private object _node;
                            private object _id;
                            internal PVEStatus(Client client, object node, object id)
                            {
                                _client = client;
                                _node = node;
                                _id = id;
                            }

                            /// <summary>
                            /// Get replication job status.
                            /// </summary>
                            public ExpandoObject JobStatus() { return _client.Execute($"/nodes/{_node}/replication/{_id}/status", HttpMethod.Get); }
                        }

                        private PVELog _log;
                        public PVELog Log { get { return _log ?? (_log = new PVELog(_client, node: _node, id: _id)); } }

                        public class PVELog
                        {
                            private Client _client;
                            private object _node;
                            private object _id;
                            internal PVELog(Client client, object node, object id)
                            {
                                _client = client;
                                _node = node;
                                _id = id;
                            }

                            /// <summary>
                            /// Read replication job log.
                            /// </summary>
                            /// <param name="limit"></param>
                            /// <param name="start"></param>
                            public ExpandoObject ReadJobLog(int? limit = null, int? start = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("limit", limit);
                                parameters.Add("start", start);
                                return _client.Execute($"/nodes/{_node}/replication/{_id}/log", HttpMethod.Get, parameters);
                            }
                        }

                        private PVESchedule_Now _schedule_now;
                        public PVESchedule_Now Schedule_Now { get { return _schedule_now ?? (_schedule_now = new PVESchedule_Now(_client, node: _node, id: _id)); } }

                        public class PVESchedule_Now
                        {
                            private Client _client;
                            private object _node;
                            private object _id;
                            internal PVESchedule_Now(Client client, object node, object id)
                            {
                                _client = client;
                                _node = node;
                                _id = id;
                            }

                            /// <summary>
                            /// Schedule replication job to start as soon as possible.
                            /// </summary>
                            public ExpandoObject ScheduleNow() { return _client.Execute($"/nodes/{_node}/replication/{_id}/schedule_now", HttpMethod.Post); }
                        }
                    }
                }

                private PVEVersion _version;
                public PVEVersion Version { get { return _version ?? (_version = new PVEVersion(_client, node: _node)); } }

                public class PVEVersion
                {
                    private Client _client;
                    private object _node;
                    internal PVEVersion(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// API version details
                    /// </summary>
                    public ExpandoObject Version() { return _client.Execute($"/nodes/{_node}/version", HttpMethod.Get); }
                }

                private PVEStatus _status;
                public PVEStatus Status { get { return _status ?? (_status = new PVEStatus(_client, node: _node)); } }

                public class PVEStatus
                {
                    private Client _client;
                    private object _node;
                    internal PVEStatus(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read node status
                    /// </summary>
                    public ExpandoObject Status() { return _client.Execute($"/nodes/{_node}/status", HttpMethod.Get); }

                    /// <summary>
                    /// Reboot or shutdown a node.
                    /// </summary>
                    /// <param name="command">Specify the command.
                    ///   Enum: reboot,shutdown</param>
                    public void NodeCmd(string command)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("command", command);
                        _client.Execute($"/nodes/{_node}/status", HttpMethod.Post, parameters);
                    }
                }

                private PVENetstat _netstat;
                public PVENetstat Netstat { get { return _netstat ?? (_netstat = new PVENetstat(_client, node: _node)); } }

                public class PVENetstat
                {
                    private Client _client;
                    private object _node;
                    internal PVENetstat(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read tap/vm network device interface counters
                    /// </summary>
                    public ExpandoObject Netstat() { return _client.Execute($"/nodes/{_node}/netstat", HttpMethod.Get); }
                }

                private PVEExecute _execute;
                public PVEExecute Execute { get { return _execute ?? (_execute = new PVEExecute(_client, node: _node)); } }

                public class PVEExecute
                {
                    private Client _client;
                    private object _node;
                    internal PVEExecute(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Execute multiple commands in order.
                    /// </summary>
                    /// <param name="commands">JSON encoded array of commands.</param>
                    public ExpandoObject Execute(string commands)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("commands", commands);
                        return _client.Execute($"/nodes/{_node}/execute", HttpMethod.Post, parameters);
                    }
                }

                private PVERrd _rrd;
                public PVERrd Rrd { get { return _rrd ?? (_rrd = new PVERrd(_client, node: _node)); } }

                public class PVERrd
                {
                    private Client _client;
                    private object _node;
                    internal PVERrd(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read node RRD statistics (returns PNG)
                    /// </summary>
                    /// <param name="ds">The list of datasources you want to display.</param>
                    /// <param name="timeframe">Specify the time frame you are interested in.
                    ///   Enum: hour,day,week,month,year</param>
                    /// <param name="cf">The RRD consolidation function
                    ///   Enum: AVERAGE,MAX</param>
                    public ExpandoObject Rrd(string ds, string timeframe, string cf = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cf", cf);
                        parameters.Add("ds", ds);
                        parameters.Add("timeframe", timeframe);
                        return _client.Execute($"/nodes/{_node}/rrd", HttpMethod.Get, parameters);
                    }
                }

                private PVERrddata _rrddata;
                public PVERrddata Rrddata { get { return _rrddata ?? (_rrddata = new PVERrddata(_client, node: _node)); } }

                public class PVERrddata
                {
                    private Client _client;
                    private object _node;
                    internal PVERrddata(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read node RRD statistics
                    /// </summary>
                    /// <param name="timeframe">Specify the time frame you are interested in.
                    ///   Enum: hour,day,week,month,year</param>
                    /// <param name="cf">The RRD consolidation function
                    ///   Enum: AVERAGE,MAX</param>
                    public ExpandoObject Rrddata(string timeframe, string cf = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cf", cf);
                        parameters.Add("timeframe", timeframe);
                        return _client.Execute($"/nodes/{_node}/rrddata", HttpMethod.Get, parameters);
                    }
                }

                private PVESyslog _syslog;
                public PVESyslog Syslog { get { return _syslog ?? (_syslog = new PVESyslog(_client, node: _node)); } }

                public class PVESyslog
                {
                    private Client _client;
                    private object _node;
                    internal PVESyslog(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read system log
                    /// </summary>
                    /// <param name="limit"></param>
                    /// <param name="since">Display all log since this date-time string.</param>
                    /// <param name="start"></param>
                    /// <param name="until">Display all log until this date-time string.</param>
                    public ExpandoObject Syslog(int? limit = null, string since = null, int? start = null, string until = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("limit", limit);
                        parameters.Add("since", since);
                        parameters.Add("start", start);
                        parameters.Add("until", until);
                        return _client.Execute($"/nodes/{_node}/syslog", HttpMethod.Get, parameters);
                    }
                }

                private PVEVncshell _vncshell;
                public PVEVncshell Vncshell { get { return _vncshell ?? (_vncshell = new PVEVncshell(_client, node: _node)); } }

                public class PVEVncshell
                {
                    private Client _client;
                    private object _node;
                    internal PVEVncshell(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="height">sets the height of the console in pixels.</param>
                    /// <param name="upgrade">Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <param name="websocket">use websocket instead of standard vnc.</param>
                    /// <param name="width">sets the width of the console in pixels.</param>
                    public ExpandoObject Vncshell(int? height = null, bool? upgrade = null, bool? websocket = null, int? width = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("height", height);
                        parameters.Add("upgrade", upgrade);
                        parameters.Add("websocket", websocket);
                        parameters.Add("width", width);
                        return _client.Execute($"/nodes/{_node}/vncshell", HttpMethod.Post, parameters);
                    }
                }

                private PVEVncwebsocket _vncwebsocket;
                public PVEVncwebsocket Vncwebsocket { get { return _vncwebsocket ?? (_vncwebsocket = new PVEVncwebsocket(_client, node: _node)); } }

                public class PVEVncwebsocket
                {
                    private Client _client;
                    private object _node;
                    internal PVEVncwebsocket(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Opens a weksocket for VNC traffic.
                    /// </summary>
                    /// <param name="port">Port number returned by previous vncproxy call.</param>
                    /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                    public ExpandoObject Vncwebsocket(int port, string vncticket)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("port", port);
                        parameters.Add("vncticket", vncticket);
                        return _client.Execute($"/nodes/{_node}/vncwebsocket", HttpMethod.Get, parameters);
                    }
                }

                private PVESpiceshell _spiceshell;
                public PVESpiceshell Spiceshell { get { return _spiceshell ?? (_spiceshell = new PVESpiceshell(_client, node: _node)); } }

                public class PVESpiceshell
                {
                    private Client _client;
                    private object _node;
                    internal PVESpiceshell(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Creates a SPICE shell.
                    /// </summary>
                    /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As resonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                    /// <param name="upgrade">Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    public ExpandoObject Spiceshell(string proxy = null, bool? upgrade = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("proxy", proxy);
                        parameters.Add("upgrade", upgrade);
                        return _client.Execute($"/nodes/{_node}/spiceshell", HttpMethod.Post, parameters);
                    }
                }

                private PVEDns _dns;
                public PVEDns Dns { get { return _dns ?? (_dns = new PVEDns(_client, node: _node)); } }

                public class PVEDns
                {
                    private Client _client;
                    private object _node;
                    internal PVEDns(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read DNS settings.
                    /// </summary>
                    public ExpandoObject Dns() { return _client.Execute($"/nodes/{_node}/dns", HttpMethod.Get); }

                    /// <summary>
                    /// Write DNS settings.
                    /// </summary>
                    /// <param name="search">Search domain for host-name lookup.</param>
                    /// <param name="dns1">First name server IP address.</param>
                    /// <param name="dns2">Second name server IP address.</param>
                    /// <param name="dns3">Third name server IP address.</param>
                    public void UpdateDns(string search, string dns1 = null, string dns2 = null, string dns3 = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("dns1", dns1);
                        parameters.Add("dns2", dns2);
                        parameters.Add("dns3", dns3);
                        parameters.Add("search", search);
                        _client.Execute($"/nodes/{_node}/dns", HttpMethod.Put, parameters);
                    }
                }

                private PVETime _time;
                public PVETime Time { get { return _time ?? (_time = new PVETime(_client, node: _node)); } }

                public class PVETime
                {
                    private Client _client;
                    private object _node;
                    internal PVETime(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Read server time and time zone settings.
                    /// </summary>
                    public ExpandoObject Time() { return _client.Execute($"/nodes/{_node}/time", HttpMethod.Get); }

                    /// <summary>
                    /// Set time zone.
                    /// </summary>
                    /// <param name="timezone">Time zone. The file '/usr/share/zoneinfo/zone.tab' contains the list of valid names.</param>
                    public void SetTimezone(string timezone)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("timezone", timezone);
                        _client.Execute($"/nodes/{_node}/time", HttpMethod.Put, parameters);
                    }
                }

                private PVEAplinfo _aplinfo;
                public PVEAplinfo Aplinfo { get { return _aplinfo ?? (_aplinfo = new PVEAplinfo(_client, node: _node)); } }

                public class PVEAplinfo
                {
                    private Client _client;
                    private object _node;
                    internal PVEAplinfo(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Get list of appliances.
                    /// </summary>
                    public ExpandoObject Aplinfo() { return _client.Execute($"/nodes/{_node}/aplinfo", HttpMethod.Get); }

                    /// <summary>
                    /// Download appliance templates.
                    /// </summary>
                    /// <param name="storage">The storage where the template will be stored</param>
                    /// <param name="template">The template wich will downloaded</param>
                    public ExpandoObject AplDownload(string storage, string template)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("storage", storage);
                        parameters.Add("template", template);
                        return _client.Execute($"/nodes/{_node}/aplinfo", HttpMethod.Post, parameters);
                    }
                }

                private PVEReport _report;
                public PVEReport Report { get { return _report ?? (_report = new PVEReport(_client, node: _node)); } }

                public class PVEReport
                {
                    private Client _client;
                    private object _node;
                    internal PVEReport(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Gather various systems information about a node
                    /// </summary>
                    public ExpandoObject Report() { return _client.Execute($"/nodes/{_node}/report", HttpMethod.Get); }
                }

                private PVEStartall _startall;
                public PVEStartall Startall { get { return _startall ?? (_startall = new PVEStartall(_client, node: _node)); } }

                public class PVEStartall
                {
                    private Client _client;
                    private object _node;
                    internal PVEStartall(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Start all VMs and containers (when onboot=1).
                    /// </summary>
                    /// <param name="force">force if onboot=0.</param>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    public ExpandoObject Startall(bool? force = null, string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        parameters.Add("vms", vms);
                        return _client.Execute($"/nodes/{_node}/startall", HttpMethod.Post, parameters);
                    }
                }

                private PVEStopall _stopall;
                public PVEStopall Stopall { get { return _stopall ?? (_stopall = new PVEStopall(_client, node: _node)); } }

                public class PVEStopall
                {
                    private Client _client;
                    private object _node;
                    internal PVEStopall(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Stop all VMs and Containers.
                    /// </summary>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    public ExpandoObject Stopall(string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("vms", vms);
                        return _client.Execute($"/nodes/{_node}/stopall", HttpMethod.Post, parameters);
                    }
                }

                private PVEMigrateall _migrateall;
                public PVEMigrateall Migrateall { get { return _migrateall ?? (_migrateall = new PVEMigrateall(_client, node: _node)); } }

                public class PVEMigrateall
                {
                    private Client _client;
                    private object _node;
                    internal PVEMigrateall(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }

                    /// <summary>
                    /// Migrate all VMs and Containers.
                    /// </summary>
                    /// <param name="target">Target node.</param>
                    /// <param name="maxworkers">Maximal number of parallel migration job. If not set use 'max_workers' from datacenter.cfg, one of both must be set!</param>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    public ExpandoObject Migrateall(string target, int? maxworkers = null, string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("maxworkers", maxworkers);
                        parameters.Add("target", target);
                        parameters.Add("vms", vms);
                        return _client.Execute($"/nodes/{_node}/migrateall", HttpMethod.Post, parameters);
                    }
                }
            }
        }

        private PVEStorage _storage;
        public PVEStorage Storage { get { return _storage ?? (_storage = new PVEStorage(_client)); } }

        public class PVEStorage
        {
            private Client _client;
            internal PVEStorage(Client client) { _client = client; }

            /// <summary>
            /// Storage index.
            /// </summary>
            /// <param name="type">Only list storage of specific type
            ///   Enum: dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,sheepdog,zfs,zfspool</param>
            public ExpandoObject Index(string type = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("type", type);
                return _client.Execute($"/storage", HttpMethod.Get, parameters);
            }

            /// <summary>
            /// Create a new storage.
            /// </summary>
            /// <param name="storage">The storage identifier.</param>
            /// <param name="type">Storage type.
            ///   Enum: dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,sheepdog,zfs,zfspool</param>
            /// <param name="authsupported">Authsupported.</param>
            /// <param name="base_">Base volume. This volume is automatically activated.</param>
            /// <param name="blocksize">block size</param>
            /// <param name="comstar_hg">host group for comstar views</param>
            /// <param name="comstar_tg">target group for comstar views</param>
            /// <param name="content">Allowed content types.  NOTE: the value 'rootdir' is used for Containers, and value 'images' for VMs. </param>
            /// <param name="disable">Flag to disable the storage.</param>
            /// <param name="export">NFS export path.</param>
            /// <param name="format">Default image format.</param>
            /// <param name="is_mountpoint">Assume the directory is an externally managed mountpoint. If nothing is mounted the storage will be considered offline.</param>
            /// <param name="iscsiprovider">iscsi provider</param>
            /// <param name="krbd">Access rbd through krbd kernel module.</param>
            /// <param name="maxfiles">Maximal number of backup files per VM. Use '0' for unlimted.</param>
            /// <param name="mkdir">Create the directory if it doesn't exist.</param>
            /// <param name="monhost">Monitors daemon ips.</param>
            /// <param name="nodes">List of cluster node names.</param>
            /// <param name="nowritecache">disable write caching on the target</param>
            /// <param name="options">NFS mount options (see 'man nfs')</param>
            /// <param name="path">File system path.</param>
            /// <param name="pool">Pool.</param>
            /// <param name="portal">iSCSI portal (IP or DNS name with optional port).</param>
            /// <param name="redundancy">The redundancy count specifies the number of nodes to which the resource should be deployed. It must be at least 1 and at most the number of nodes in the cluster.</param>
            /// <param name="saferemove">Zero-out data when removing LVs.</param>
            /// <param name="saferemove_throughput">Wipe throughput (cstream -t parameter value).</param>
            /// <param name="server">Server IP or DNS name.</param>
            /// <param name="server2">Backup volfile server IP or DNS name.</param>
            /// <param name="shared">Mark storage as shared.</param>
            /// <param name="sparse">use sparse volumes</param>
            /// <param name="tagged_only">Only use logical volumes tagged with 'pve-vm-ID'.</param>
            /// <param name="target">iSCSI target.</param>
            /// <param name="thinpool">LVM thin pool LV name.</param>
            /// <param name="transport">Gluster transport: tcp or rdma
            ///   Enum: tcp,rdma,unix</param>
            /// <param name="username">RBD Id.</param>
            /// <param name="vgname">Volume group name.</param>
            /// <param name="volume">Glusterfs Volume.</param>
            public void Create(string storage, string type, string authsupported = null, string base_ = null, string blocksize = null, string comstar_hg = null, string comstar_tg = null, string content = null, bool? disable = null, string export = null, string format = null, bool? is_mountpoint = null, string iscsiprovider = null, bool? krbd = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string path = null, string pool = null, string portal = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, bool? shared = null, bool? sparse = null, bool? tagged_only = null, string target = null, string thinpool = null, string transport = null, string username = null, string vgname = null, string volume = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("authsupported", authsupported);
                parameters.Add("base", base_);
                parameters.Add("blocksize", blocksize);
                parameters.Add("comstar_hg", comstar_hg);
                parameters.Add("comstar_tg", comstar_tg);
                parameters.Add("content", content);
                parameters.Add("disable", disable);
                parameters.Add("export", export);
                parameters.Add("format", format);
                parameters.Add("is_mountpoint", is_mountpoint);
                parameters.Add("iscsiprovider", iscsiprovider);
                parameters.Add("krbd", krbd);
                parameters.Add("maxfiles", maxfiles);
                parameters.Add("mkdir", mkdir);
                parameters.Add("monhost", monhost);
                parameters.Add("nodes", nodes);
                parameters.Add("nowritecache", nowritecache);
                parameters.Add("options", options);
                parameters.Add("path", path);
                parameters.Add("pool", pool);
                parameters.Add("portal", portal);
                parameters.Add("redundancy", redundancy);
                parameters.Add("saferemove", saferemove);
                parameters.Add("saferemove_throughput", saferemove_throughput);
                parameters.Add("server", server);
                parameters.Add("server2", server2);
                parameters.Add("shared", shared);
                parameters.Add("sparse", sparse);
                parameters.Add("storage", storage);
                parameters.Add("tagged_only", tagged_only);
                parameters.Add("target", target);
                parameters.Add("thinpool", thinpool);
                parameters.Add("transport", transport);
                parameters.Add("type", type);
                parameters.Add("username", username);
                parameters.Add("vgname", vgname);
                parameters.Add("volume", volume);
                _client.Execute($"/storage", HttpMethod.Post, parameters);
            }

            public PVEItemStorage this[object storage] { get { return new PVEItemStorage(_client, storage: storage); } }

            public class PVEItemStorage
            {
                private Client _client;
                private object _storage;
                internal PVEItemStorage(Client client, object storage)
                {
                    _client = client;
                    _storage = storage;
                }

                /// <summary>
                /// Delete storage configuration.
                /// </summary>
                public void Delete() { _client.Execute($"/storage/{_storage}", HttpMethod.Delete); }

                /// <summary>
                /// Read storage configuration.
                /// </summary>
                public ExpandoObject Read() { return _client.Execute($"/storage/{_storage}", HttpMethod.Get); }

                /// <summary>
                /// Update storage configuration.
                /// </summary>
                /// <param name="blocksize">block size</param>
                /// <param name="comstar_hg">host group for comstar views</param>
                /// <param name="comstar_tg">target group for comstar views</param>
                /// <param name="content">Allowed content types.  NOTE: the value 'rootdir' is used for Containers, and value 'images' for VMs. </param>
                /// <param name="delete">A list of settings you want to delete.</param>
                /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                /// <param name="disable">Flag to disable the storage.</param>
                /// <param name="format">Default image format.</param>
                /// <param name="is_mountpoint">Assume the directory is an externally managed mountpoint. If nothing is mounted the storage will be considered offline.</param>
                /// <param name="krbd">Access rbd through krbd kernel module.</param>
                /// <param name="maxfiles">Maximal number of backup files per VM. Use '0' for unlimted.</param>
                /// <param name="mkdir">Create the directory if it doesn't exist.</param>
                /// <param name="nodes">List of cluster node names.</param>
                /// <param name="nowritecache">disable write caching on the target</param>
                /// <param name="options">NFS mount options (see 'man nfs')</param>
                /// <param name="pool">Pool.</param>
                /// <param name="redundancy">The redundancy count specifies the number of nodes to which the resource should be deployed. It must be at least 1 and at most the number of nodes in the cluster.</param>
                /// <param name="saferemove">Zero-out data when removing LVs.</param>
                /// <param name="saferemove_throughput">Wipe throughput (cstream -t parameter value).</param>
                /// <param name="server">Server IP or DNS name.</param>
                /// <param name="server2">Backup volfile server IP or DNS name.</param>
                /// <param name="shared">Mark storage as shared.</param>
                /// <param name="sparse">use sparse volumes</param>
                /// <param name="tagged_only">Only use logical volumes tagged with 'pve-vm-ID'.</param>
                /// <param name="transport">Gluster transport: tcp or rdma
                ///   Enum: tcp,rdma,unix</param>
                /// <param name="username">RBD Id.</param>
                public void Update(string blocksize = null, string comstar_hg = null, string comstar_tg = null, string content = null, string delete = null, string digest = null, bool? disable = null, string format = null, bool? is_mountpoint = null, bool? krbd = null, int? maxfiles = null, bool? mkdir = null, string nodes = null, bool? nowritecache = null, string options = null, string pool = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, bool? shared = null, bool? sparse = null, bool? tagged_only = null, string transport = null, string username = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("blocksize", blocksize);
                    parameters.Add("comstar_hg", comstar_hg);
                    parameters.Add("comstar_tg", comstar_tg);
                    parameters.Add("content", content);
                    parameters.Add("delete", delete);
                    parameters.Add("digest", digest);
                    parameters.Add("disable", disable);
                    parameters.Add("format", format);
                    parameters.Add("is_mountpoint", is_mountpoint);
                    parameters.Add("krbd", krbd);
                    parameters.Add("maxfiles", maxfiles);
                    parameters.Add("mkdir", mkdir);
                    parameters.Add("nodes", nodes);
                    parameters.Add("nowritecache", nowritecache);
                    parameters.Add("options", options);
                    parameters.Add("pool", pool);
                    parameters.Add("redundancy", redundancy);
                    parameters.Add("saferemove", saferemove);
                    parameters.Add("saferemove_throughput", saferemove_throughput);
                    parameters.Add("server", server);
                    parameters.Add("server2", server2);
                    parameters.Add("shared", shared);
                    parameters.Add("sparse", sparse);
                    parameters.Add("tagged_only", tagged_only);
                    parameters.Add("transport", transport);
                    parameters.Add("username", username);
                    _client.Execute($"/storage/{_storage}", HttpMethod.Put, parameters);
                }
            }
        }

        private PVEAccess _access;
        public PVEAccess Access { get { return _access ?? (_access = new PVEAccess(_client)); } }

        public class PVEAccess
        {
            private Client _client;
            internal PVEAccess(Client client) { _client = client; }

            /// <summary>
            /// Directory index.
            /// </summary>
            public ExpandoObject Index() { return _client.Execute($"/access", HttpMethod.Get); }

            private PVEUsers _users;
            public PVEUsers Users { get { return _users ?? (_users = new PVEUsers(_client)); } }

            public class PVEUsers
            {
                private Client _client;
                internal PVEUsers(Client client) { _client = client; }

                /// <summary>
                /// User index.
                /// </summary>
                /// <param name="enabled">Optional filter for enable property.</param>
                public ExpandoObject Index(bool? enabled = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("enabled", enabled);
                    return _client.Execute($"/access/users", HttpMethod.Get, parameters);
                }

                /// <summary>
                /// Create new user.
                /// </summary>
                /// <param name="userid">User ID</param>
                /// <param name="comment"></param>
                /// <param name="email"></param>
                /// <param name="enable">Enable the account (default). You can set this to '0' to disable the accout</param>
                /// <param name="expire">Account expiration date (seconds since epoch). '0' means no expiration date.</param>
                /// <param name="firstname"></param>
                /// <param name="groups"></param>
                /// <param name="keys">Keys for two factor auth (yubico).</param>
                /// <param name="lastname"></param>
                /// <param name="password">Initial password.</param>
                public void CreateUser(string userid, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null, string password = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("comment", comment);
                    parameters.Add("email", email);
                    parameters.Add("enable", enable);
                    parameters.Add("expire", expire);
                    parameters.Add("firstname", firstname);
                    parameters.Add("groups", groups);
                    parameters.Add("keys", keys);
                    parameters.Add("lastname", lastname);
                    parameters.Add("password", password);
                    parameters.Add("userid", userid);
                    _client.Execute($"/access/users", HttpMethod.Post, parameters);
                }

                public PVEItemUserid this[object userid] { get { return new PVEItemUserid(_client, userid: userid); } }

                public class PVEItemUserid
                {
                    private Client _client;
                    private object _userid;
                    internal PVEItemUserid(Client client, object userid)
                    {
                        _client = client;
                        _userid = userid;
                    }

                    /// <summary>
                    /// Delete user.
                    /// </summary>
                    public void DeleteUser() { _client.Execute($"/access/users/{_userid}", HttpMethod.Delete); }

                    /// <summary>
                    /// Get user configuration.
                    /// </summary>
                    public ExpandoObject ReadUser() { return _client.Execute($"/access/users/{_userid}", HttpMethod.Get); }

                    /// <summary>
                    /// Update user configuration.
                    /// </summary>
                    /// <param name="append"></param>
                    /// <param name="comment"></param>
                    /// <param name="email"></param>
                    /// <param name="enable">Enable/disable the account.</param>
                    /// <param name="expire">Account expiration date (seconds since epoch). '0' means no expiration date.</param>
                    /// <param name="firstname"></param>
                    /// <param name="groups"></param>
                    /// <param name="keys">Keys for two factor auth (yubico).</param>
                    /// <param name="lastname"></param>
                    public void UpdateUser(bool? append = null, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null)
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
                        _client.Execute($"/access/users/{_userid}", HttpMethod.Put, parameters);
                    }
                }
            }

            private PVEGroups _groups;
            public PVEGroups Groups { get { return _groups ?? (_groups = new PVEGroups(_client)); } }

            public class PVEGroups
            {
                private Client _client;
                internal PVEGroups(Client client) { _client = client; }

                /// <summary>
                /// Group index.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/access/groups", HttpMethod.Get); }

                /// <summary>
                /// Create new group.
                /// </summary>
                /// <param name="groupid"></param>
                /// <param name="comment"></param>
                public void CreateGroup(string groupid, string comment = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("comment", comment);
                    parameters.Add("groupid", groupid);
                    _client.Execute($"/access/groups", HttpMethod.Post, parameters);
                }

                public PVEItemGroupid this[object groupid] { get { return new PVEItemGroupid(_client, groupid: groupid); } }

                public class PVEItemGroupid
                {
                    private Client _client;
                    private object _groupid;
                    internal PVEItemGroupid(Client client, object groupid)
                    {
                        _client = client;
                        _groupid = groupid;
                    }

                    /// <summary>
                    /// Delete group.
                    /// </summary>
                    public void DeleteGroup() { _client.Execute($"/access/groups/{_groupid}", HttpMethod.Delete); }

                    /// <summary>
                    /// Get group configuration.
                    /// </summary>
                    public ExpandoObject ReadGroup() { return _client.Execute($"/access/groups/{_groupid}", HttpMethod.Get); }

                    /// <summary>
                    /// Update group data.
                    /// </summary>
                    /// <param name="comment"></param>
                    public void UpdateGroup(string comment = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        _client.Execute($"/access/groups/{_groupid}", HttpMethod.Put, parameters);
                    }
                }
            }

            private PVERoles _roles;
            public PVERoles Roles { get { return _roles ?? (_roles = new PVERoles(_client)); } }

            public class PVERoles
            {
                private Client _client;
                internal PVERoles(Client client) { _client = client; }

                /// <summary>
                /// Role index.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/access/roles", HttpMethod.Get); }

                /// <summary>
                /// Create new role.
                /// </summary>
                /// <param name="roleid"></param>
                /// <param name="privs"></param>
                public void CreateRole(string roleid, string privs = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("privs", privs);
                    parameters.Add("roleid", roleid);
                    _client.Execute($"/access/roles", HttpMethod.Post, parameters);
                }

                public PVEItemRoleid this[object roleid] { get { return new PVEItemRoleid(_client, roleid: roleid); } }

                public class PVEItemRoleid
                {
                    private Client _client;
                    private object _roleid;
                    internal PVEItemRoleid(Client client, object roleid)
                    {
                        _client = client;
                        _roleid = roleid;
                    }

                    /// <summary>
                    /// Delete role.
                    /// </summary>
                    public void DeleteRole() { _client.Execute($"/access/roles/{_roleid}", HttpMethod.Delete); }

                    /// <summary>
                    /// Get role configuration.
                    /// </summary>
                    public ExpandoObject ReadRole() { return _client.Execute($"/access/roles/{_roleid}", HttpMethod.Get); }

                    /// <summary>
                    /// Create new role.
                    /// </summary>
                    /// <param name="privs"></param>
                    /// <param name="append"></param>
                    public void UpdateRole(string privs, bool? append = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("append", append);
                        parameters.Add("privs", privs);
                        _client.Execute($"/access/roles/{_roleid}", HttpMethod.Put, parameters);
                    }
                }
            }

            private PVEAcl _acl;
            public PVEAcl Acl { get { return _acl ?? (_acl = new PVEAcl(_client)); } }

            public class PVEAcl
            {
                private Client _client;
                internal PVEAcl(Client client) { _client = client; }

                /// <summary>
                /// Get Access Control List (ACLs).
                /// </summary>
                public ExpandoObject ReadAcl() { return _client.Execute($"/access/acl", HttpMethod.Get); }

                /// <summary>
                /// Update Access Control List (add or remove permissions).
                /// </summary>
                /// <param name="path">Access control path</param>
                /// <param name="roles">List of roles.</param>
                /// <param name="delete">Remove permissions (instead of adding it).</param>
                /// <param name="groups">List of groups.</param>
                /// <param name="propagate">Allow to propagate (inherit) permissions.</param>
                /// <param name="users">List of users.</param>
                public void UpdateAcl(string path, string roles, bool? delete = null, string groups = null, bool? propagate = null, string users = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("delete", delete);
                    parameters.Add("groups", groups);
                    parameters.Add("path", path);
                    parameters.Add("propagate", propagate);
                    parameters.Add("roles", roles);
                    parameters.Add("users", users);
                    _client.Execute($"/access/acl", HttpMethod.Put, parameters);
                }
            }

            private PVEDomains _domains;
            public PVEDomains Domains { get { return _domains ?? (_domains = new PVEDomains(_client)); } }

            public class PVEDomains
            {
                private Client _client;
                internal PVEDomains(Client client) { _client = client; }

                /// <summary>
                /// Authentication domain index.
                /// </summary>
                public ExpandoObject Index() { return _client.Execute($"/access/domains", HttpMethod.Get); }

                /// <summary>
                /// Add an authentication server.
                /// </summary>
                /// <param name="realm">Authentication domain ID</param>
                /// <param name="type">Realm type.
                ///   Enum: ad,ldap,pam,pve</param>
                /// <param name="base_dn">LDAP base domain name</param>
                /// <param name="bind_dn">LDAP bind domain name</param>
                /// <param name="comment">Description.</param>
                /// <param name="default_">Use this as default realm</param>
                /// <param name="domain">AD domain name</param>
                /// <param name="port">Server port.</param>
                /// <param name="secure">Use secure LDAPS protocol.</param>
                /// <param name="server1">Server IP address (or DNS name)</param>
                /// <param name="server2">Fallback Server IP address (or DNS name)</param>
                /// <param name="tfa">Use Two-factor authentication.</param>
                /// <param name="user_attr">LDAP user attribute name</param>
                public void Create(string realm, string type, string base_dn = null, string bind_dn = null, string comment = null, bool? default_ = null, string domain = null, int? port = null, bool? secure = null, string server1 = null, string server2 = null, string tfa = null, string user_attr = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("base_dn", base_dn);
                    parameters.Add("bind_dn", bind_dn);
                    parameters.Add("comment", comment);
                    parameters.Add("default", default_);
                    parameters.Add("domain", domain);
                    parameters.Add("port", port);
                    parameters.Add("realm", realm);
                    parameters.Add("secure", secure);
                    parameters.Add("server1", server1);
                    parameters.Add("server2", server2);
                    parameters.Add("tfa", tfa);
                    parameters.Add("type", type);
                    parameters.Add("user_attr", user_attr);
                    _client.Execute($"/access/domains", HttpMethod.Post, parameters);
                }

                public PVEItemRealm this[object realm] { get { return new PVEItemRealm(_client, realm: realm); } }

                public class PVEItemRealm
                {
                    private Client _client;
                    private object _realm;
                    internal PVEItemRealm(Client client, object realm)
                    {
                        _client = client;
                        _realm = realm;
                    }

                    /// <summary>
                    /// Delete an authentication server.
                    /// </summary>
                    public void Delete() { _client.Execute($"/access/domains/{_realm}", HttpMethod.Delete); }

                    /// <summary>
                    /// Get auth server configuration.
                    /// </summary>
                    public ExpandoObject Read() { return _client.Execute($"/access/domains/{_realm}", HttpMethod.Get); }

                    /// <summary>
                    /// Update authentication server settings.
                    /// </summary>
                    /// <param name="base_dn">LDAP base domain name</param>
                    /// <param name="bind_dn">LDAP bind domain name</param>
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
                    public void Update(string base_dn = null, string bind_dn = null, string comment = null, bool? default_ = null, string delete = null, string digest = null, string domain = null, int? port = null, bool? secure = null, string server1 = null, string server2 = null, string tfa = null, string user_attr = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("base_dn", base_dn);
                        parameters.Add("bind_dn", bind_dn);
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
                        _client.Execute($"/access/domains/{_realm}", HttpMethod.Put, parameters);
                    }
                }
            }

            private PVETicket _ticket;
            public PVETicket Ticket { get { return _ticket ?? (_ticket = new PVETicket(_client)); } }

            public class PVETicket
            {
                private Client _client;
                internal PVETicket(Client client) { _client = client; }

                /// <summary>
                /// Dummy. Useful for formaters which want to priovde a login page.
                /// </summary>
                public void GetTicket() { _client.Execute($"/access/ticket", HttpMethod.Get); }

                /// <summary>
                /// Create or verify authentication ticket.
                /// </summary>
                /// <param name="password">The secret password. This can also be a valid ticket.</param>
                /// <param name="username">User name</param>
                /// <param name="otp">One-time password for Two-factor authentication.</param>
                /// <param name="path">Verify ticket, and check if user have access 'privs' on 'path'</param>
                /// <param name="privs">Verify ticket, and check if user have access 'privs' on 'path'</param>
                /// <param name="realm">You can optionally pass the realm using this parameter. Normally the realm is simply added to the username &amp;lt;username>@&amp;lt;relam>.</param>
                public ExpandoObject CreateTicket(string password, string username, string otp = null, string path = null, string privs = null, string realm = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("otp", otp);
                    parameters.Add("password", password);
                    parameters.Add("path", path);
                    parameters.Add("privs", privs);
                    parameters.Add("realm", realm);
                    parameters.Add("username", username);
                    return _client.Execute($"/access/ticket", HttpMethod.Post, parameters);
                }
            }

            private PVEPassword _password;
            public PVEPassword Password { get { return _password ?? (_password = new PVEPassword(_client)); } }

            public class PVEPassword
            {
                private Client _client;
                internal PVEPassword(Client client) { _client = client; }

                /// <summary>
                /// Change user password.
                /// </summary>
                /// <param name="password">The new password.</param>
                /// <param name="userid">User ID</param>
                public void ChangePasssword(string password, string userid)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("password", password);
                    parameters.Add("userid", userid);
                    _client.Execute($"/access/password", HttpMethod.Put, parameters);
                }
            }
        }

        private PVEPools _pools;
        public PVEPools Pools { get { return _pools ?? (_pools = new PVEPools(_client)); } }

        public class PVEPools
        {
            private Client _client;
            internal PVEPools(Client client) { _client = client; }

            /// <summary>
            /// Pool index.
            /// </summary>
            public ExpandoObject Index() { return _client.Execute($"/pools", HttpMethod.Get); }

            /// <summary>
            /// Create new pool.
            /// </summary>
            /// <param name="poolid"></param>
            /// <param name="comment"></param>
            public void CreatePool(string poolid, string comment = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("comment", comment);
                parameters.Add("poolid", poolid);
                _client.Execute($"/pools", HttpMethod.Post, parameters);
            }

            public PVEItemPoolid this[object poolid] { get { return new PVEItemPoolid(_client, poolid: poolid); } }

            public class PVEItemPoolid
            {
                private Client _client;
                private object _poolid;
                internal PVEItemPoolid(Client client, object poolid)
                {
                    _client = client;
                    _poolid = poolid;
                }

                /// <summary>
                /// Delete pool.
                /// </summary>
                public void DeletePool() { _client.Execute($"/pools/{_poolid}", HttpMethod.Delete); }

                /// <summary>
                /// Get pool configuration.
                /// </summary>
                public ExpandoObject ReadPool() { return _client.Execute($"/pools/{_poolid}", HttpMethod.Get); }

                /// <summary>
                /// Update pool data.
                /// </summary>
                /// <param name="comment"></param>
                /// <param name="delete">Remove vms/storage (instead of adding it).</param>
                /// <param name="storage">List of storage IDs.</param>
                /// <param name="vms">List of virtual machines.</param>
                public void UpdatePool(string comment = null, bool? delete = null, string storage = null, string vms = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("comment", comment);
                    parameters.Add("delete", delete);
                    parameters.Add("storage", storage);
                    parameters.Add("vms", vms);
                    _client.Execute($"/pools/{_poolid}", HttpMethod.Put, parameters);
                }
            }
        }

        private PVEVersion _version;
        public PVEVersion Version { get { return _version ?? (_version = new PVEVersion(_client)); } }

        public class PVEVersion
        {
            private Client _client;
            internal PVEVersion(Client client) { _client = client; }

            /// <summary>
            /// API version details. The result also includes the global datacenter confguration.
            /// </summary>
            public ExpandoObject Version() { return _client.Execute($"/version", HttpMethod.Get); }
        }
    }
}