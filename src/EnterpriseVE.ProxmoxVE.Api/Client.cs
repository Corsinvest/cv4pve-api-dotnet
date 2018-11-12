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
    public abstract class Base
    {
        protected Client _client;
    }
    /// <summary>
    /// Result request API
    /// </summary>
    public class Result
    {
        internal Result(dynamic response, HttpStatusCode statusCode, string reasonPhrase, bool isSuccessStatusCode)
        {
            Response = response;
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            IsSuccessStatusCode = isSuccessStatusCode;
        }
        /// <summary>
        /// Get if response Proxmox VE contain errors
        /// </summary>
        /// <returns></returns>
        public bool ResponseInError { get { return ((IDictionary<String, object>)Response).ContainsKey("errors"); } }
        /// <summary>
        /// Proxmox VE response.
        /// </summary>
        /// <returns></returns>    
        public dynamic Response { get; private set; }
        /// <summary>
        /// Contains the values of status codes defined for HTTP.
        /// </summary>
        /// <returns></returns>    
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// Gets the reason phrase which typically is sent by servers together with the status code.
        /// </summary>
        /// <returns></returns>
        public string ReasonPhrase { get; private set; }
        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// </summary>
        /// <returns></returns>
        public bool IsSuccessStatusCode { get; private set; }
        /// <summary>
        /// Get error
        /// </summary>
        /// <returns></returns>
        public string GetError()
        {
            var ret = "";
            if (ResponseInError)
            {
                foreach (var item in (IDictionary<string, object>)Response.errors)
                {
                    if (!string.IsNullOrWhiteSpace(ret)) { ret += Environment.NewLine; }
                    ret += $"{item.Key} : {item.Value}";
                }
            }
            return ret;
        }
    }
    /// <summary>
    /// Proxmox VE Client
    /// </summary>
    public class Client : Base
    {
        private string _ticketCSRFPreventionToken;
        private string _ticketPVEAuthCookie;
        public Client(string hostname, int port = 8006)
        {
            _client = this;
            Hostname = hostname;
            Port = port;
        }
        /// <summary>
        /// Get hostname configured. 
        /// </summary>
        public string Hostname { get; private set; }
        /// <summary>
        /// Get port configured. 
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// Get/Set the response type that is going to be returned when doing requests (json, png). 
        /// </summary>
        public string ResponseType { get; set; } = "json";
        /// <summary>
        /// Returns the base URL used to interact with the Proxmox VE API. 
        /// </summary>
        public string GetApiUrl() { return $"https://{Hostname}:{Port}/api2/{ResponseType}"; }
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
            var ticket = Access.Ticket.CreateRest(username: userName, password: password, realm: realm);
            if (ticket.IsSuccessStatusCode)
            {
                _ticketCSRFPreventionToken = ticket.Response.data.CSRFPreventionToken;
                _ticketPVEAuthCookie = ticket.Response.data.ticket;
            }
            return ticket.IsSuccessStatusCode;
        }
        /// <summary>
        /// Creation ticket from login split username &lt;username&gt;@&lt;realm&gt;.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public bool Login(string userName, string password)
        {
            var realm = "pam";
            //check username
            var uData = userName.Split('@');
            if (uData.Length > 1)
            {
                userName = uData[0];
                realm = uData[1];
            }
            return Login(userName, password, realm);
        }
        /// <summary>
        /// Execute Execute method GET
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public Result Get(string resource, IDictionary<string, object> parameters = null)
        {
            return ExecuteAction(resource, HttpMethod.Get, parameters);
        }
        /// <summary>
        /// Execute Execute method POST
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public Result Create(string resource, IDictionary<string, object> parameters = null)
        {
            return ExecuteAction(resource, HttpMethod.Post, parameters);
        }
        /// <summary>
        /// Execute Execute method PUT
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public Result Set(string resource, IDictionary<string, object> parameters = null)
        {
            return ExecuteAction(resource, HttpMethod.Put, parameters);
        }
        /// <summary>
        /// Execute Execute method DELETE
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public Result Delete(string resource, IDictionary<string, object> parameters = null)
        {
            return ExecuteAction(resource, HttpMethod.Delete, parameters);
        }
        private Result ExecuteAction(string resource, HttpMethod method, IDictionary<string, object> parameters = null)
        {
            using (var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            })
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(GetApiUrl());
                //load parameters
                var @params = new Dictionary<string, string>();
                if (parameters != null)
                {
                    foreach (var parameter in parameters.Where(a => a.Value != null))
                    {
                        var value = parameter.Value;
                        if (value is bool) { value = ((bool)value) ? 1 : 0; }
                        @params.Add(parameter.Key, HttpUtility.UrlEncode(value.ToString()));
                    }
                }
                var uriString = GetApiUrl() + resource;
                if (method == HttpMethod.Get && @params.Count > 0)
                {
                    uriString += "?" + string.Join("&", (from a in @params
                                                         select $"{a.Key}={a.Value}"));
                }
                var request = new HttpRequestMessage(method, new Uri(uriString));
                if (method != HttpMethod.Get) { request.Content = new FormUrlEncodedContent(@params); }
                //tiket login
                if (_ticketCSRFPreventionToken != null)
                {
                    handler.CookieContainer.Add(request.RequestUri, new Cookie("PVEAuthCookie", _ticketPVEAuthCookie));
                    request.Headers.Add("CSRFPreventionToken", _ticketCSRFPreventionToken);
                }

                var response = client.SendAsync(request).Result;
                dynamic result = null;
                if (ResponseType == "json")
                {
                    var stringContent = response.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<ExpandoObject>(stringContent);
                }
                else if (ResponseType == "png")
                {
                    result = "data:image/png;base64," +
                             Convert.ToBase64String(response.Content.ReadAsByteArrayAsync().Result);
                }
                if (result == null) { result = new ExpandoObject(); }
                return new Result(result,
                                response.StatusCode,
                                response.ReasonPhrase,
                                response.IsSuccessStatusCode);
            }
        }
        private static void AddIndexedParameter(Dictionary<string, object> parameters, string name, IDictionary<int, string> value)
        {
            if (value == null) { return; }
            foreach (var item in value) { parameters.Add(name + item.Key, item.Value); }
        }
        /// <summary>
        /// Wait for task to finish
        /// </summary>
        /// <param name="node">Node identifier</param>
        /// <param name="task">Task identifier</param>
        /// <param name="wait">Millisecond wait next check</param>
        /// <param name="timeOut">Millisecond timeout</param>
        /// <return>O Success</return>
        public int WaitForTaskToFinish(string node, string task, long wait = 500, long timeOut = 10000)
        {
            var isRunning = true;
            if (wait <= 0) { wait = 500; }
            if (timeOut < wait) { timeOut = wait + 5000; }
            var timeStart = DateTime.Now;
            var waitTime = DateTime.Now;
            while (isRunning && (timeStart - DateTime.Now).Milliseconds < timeOut)
            {
                if ((DateTime.Now - waitTime).TotalMilliseconds >= wait)
                {
                    waitTime = DateTime.Now;
                    var status = Nodes[node].Tasks[task].Status.GetRest().Response.data.status;
                    isRunning = status == "running";
                }
            }
            //check timeout
            return (timeStart - DateTime.Now).Milliseconds < timeOut ? 0 : 1;
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

        public class PVECluster : Base
        {
            internal PVECluster(Client client)
            {
                _client = client;
            }
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
            public class PVEReplication : Base
            {
                internal PVEReplication(Client client)
                {
                    _client = client;
                }
                public PVEItemId this[object id] => new PVEItemId(_client, id);
                public class PVEItemId : Base
                {
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
                    public Result Delete(bool? force = null, bool? keep = null) { return DeleteRest(force, keep); }
                    /// <summary>
                    /// Read replication job configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/replication/{_id}");
                    }
                    /// <summary>
                    /// Read replication job configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result Read() { return GetRest(); }
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
                    public Result Update(string comment = null, string delete = null, string digest = null, bool? disable = null, int? rate = null, string remove_job = null, string schedule = null, string source = null) { return SetRest(comment, delete, digest, disable, rate, remove_job, schedule, source); }
                }
                /// <summary>
                /// List replication jobs.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/replication");
                }
                /// <summary>
                /// List replication jobs.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
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
                public Result Create(string id, string target, string type, string comment = null, bool? disable = null, int? rate = null, string remove_job = null, string schedule = null, string source = null) { return CreateRest(id, target, type, comment, disable, rate, remove_job, schedule, source); }
            }
            public class PVEConfig : Base
            {
                internal PVEConfig(Client client)
                {
                    _client = client;
                }
                private PVENodes _nodes;
                public PVENodes Nodes => _nodes ?? (_nodes = new PVENodes(_client));
                private PVEJoin _join;
                public PVEJoin Join => _join ?? (_join = new PVEJoin(_client));
                private PVETotem _totem;
                public PVETotem Totem => _totem ?? (_totem = new PVETotem(_client));
                public class PVENodes : Base
                {
                    internal PVENodes(Client client)
                    {
                        _client = client;
                    }
                    public PVEItemNode this[object node] => new PVEItemNode(_client, node);
                    public class PVEItemNode : Base
                    {
                        private object _node;
                        internal PVEItemNode(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Removes a node from the cluster configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/cluster/config/nodes/{_node}");
                        }
                        /// <summary>
                        /// Removes a node from the cluster configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Delnode() { return DeleteRest(); }
                        /// <summary>
                        /// Adds a node to the cluster configuration.
                        /// </summary>
                        /// <param name="force">Do not throw error if node already exists.</param>
                        /// <param name="nodeid">Node id for this node.</param>
                        /// <param name="ring0_addr">Hostname (or IP) of the corosync ring0 address of this node.</param>
                        /// <param name="ring1_addr">Hostname (or IP) of the corosync ring1 address of this node. Requires a valid configured ring 1 (bindnet1_addr) in the cluster.</param>
                        /// <param name="votes">Number of votes for this node</param>
                        /// <returns></returns>
                        public Result CreateRest(bool? force = null, int? nodeid = null, string ring0_addr = null, string ring1_addr = null, int? votes = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("force", force);
                            parameters.Add("nodeid", nodeid);
                            parameters.Add("ring0_addr", ring0_addr);
                            parameters.Add("ring1_addr", ring1_addr);
                            parameters.Add("votes", votes);
                            return _client.Create($"/cluster/config/nodes/{_node}", parameters);
                        }
                        /// <summary>
                        /// Adds a node to the cluster configuration.
                        /// </summary>
                        /// <param name="force">Do not throw error if node already exists.</param>
                        /// <param name="nodeid">Node id for this node.</param>
                        /// <param name="ring0_addr">Hostname (or IP) of the corosync ring0 address of this node.</param>
                        /// <param name="ring1_addr">Hostname (or IP) of the corosync ring1 address of this node. Requires a valid configured ring 1 (bindnet1_addr) in the cluster.</param>
                        /// <param name="votes">Number of votes for this node</param>
                        /// <returns></returns>
                        public Result Addnode(bool? force = null, int? nodeid = null, string ring0_addr = null, string ring1_addr = null, int? votes = null) { return CreateRest(force, nodeid, ring0_addr, ring1_addr, votes); }
                    }
                    /// <summary>
                    /// Corosync node list.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/config/nodes");
                    }
                    /// <summary>
                    /// Corosync node list.
                    /// </summary>
                    /// <returns></returns>
                    public Result Nodes() { return GetRest(); }
                }
                public class PVEJoin : Base
                {
                    internal PVEJoin(Client client)
                    {
                        _client = client;
                    }
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
                    public Result JoinInfo(string node = null) { return GetRest(node); }
                    /// <summary>
                    /// Joins this node into an existing cluster.
                    /// </summary>
                    /// <param name="fingerprint">Certificate SHA 256 fingerprint.</param>
                    /// <param name="hostname">Hostname (or IP) of an existing cluster member.</param>
                    /// <param name="password">Superuser (root) password of peer node.</param>
                    /// <param name="force">Do not throw error if node already exists.</param>
                    /// <param name="nodeid">Node id for this node.</param>
                    /// <param name="ring0_addr">Hostname (or IP) of the corosync ring0 address of this node.</param>
                    /// <param name="ring1_addr">Hostname (or IP) of the corosync ring1 address of this node. Requires a valid configured ring 1 (bindnet1_addr) in the cluster.</param>
                    /// <param name="votes">Number of votes for this node</param>
                    /// <returns></returns>
                    public Result CreateRest(string fingerprint, string hostname, string password, bool? force = null, int? nodeid = null, string ring0_addr = null, string ring1_addr = null, int? votes = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("fingerprint", fingerprint);
                        parameters.Add("hostname", hostname);
                        parameters.Add("password", password);
                        parameters.Add("force", force);
                        parameters.Add("nodeid", nodeid);
                        parameters.Add("ring0_addr", ring0_addr);
                        parameters.Add("ring1_addr", ring1_addr);
                        parameters.Add("votes", votes);
                        return _client.Create($"/cluster/config/join", parameters);
                    }
                    /// <summary>
                    /// Joins this node into an existing cluster.
                    /// </summary>
                    /// <param name="fingerprint">Certificate SHA 256 fingerprint.</param>
                    /// <param name="hostname">Hostname (or IP) of an existing cluster member.</param>
                    /// <param name="password">Superuser (root) password of peer node.</param>
                    /// <param name="force">Do not throw error if node already exists.</param>
                    /// <param name="nodeid">Node id for this node.</param>
                    /// <param name="ring0_addr">Hostname (or IP) of the corosync ring0 address of this node.</param>
                    /// <param name="ring1_addr">Hostname (or IP) of the corosync ring1 address of this node. Requires a valid configured ring 1 (bindnet1_addr) in the cluster.</param>
                    /// <param name="votes">Number of votes for this node</param>
                    /// <returns></returns>
                    public Result Join(string fingerprint, string hostname, string password, bool? force = null, int? nodeid = null, string ring0_addr = null, string ring1_addr = null, int? votes = null) { return CreateRest(fingerprint, hostname, password, force, nodeid, ring0_addr, ring1_addr, votes); }
                }
                public class PVETotem : Base
                {
                    internal PVETotem(Client client)
                    {
                        _client = client;
                    }
                    /// <summary>
                    /// Get corosync totem protocol settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/config/totem");
                    }
                    /// <summary>
                    /// Get corosync totem protocol settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result Totem() { return GetRest(); }
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/config");
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
                /// <summary>
                /// Generate new cluster configuration.
                /// </summary>
                /// <param name="clustername">The name of the cluster.</param>
                /// <param name="bindnet0_addr">This specifies the network address the corosync ring 0 executive should bind to and defaults to the local IP address of the node.</param>
                /// <param name="bindnet1_addr">This specifies the network address the corosync ring 1 executive should bind to and is optional.</param>
                /// <param name="nodeid">Node id for this node.</param>
                /// <param name="ring0_addr">Hostname (or IP) of the corosync ring0 address of this node.</param>
                /// <param name="ring1_addr">Hostname (or IP) of the corosync ring1 address of this node. Requires a valid configured ring 1 (bindnet1_addr) in the cluster.</param>
                /// <param name="votes">Number of votes for this node.</param>
                /// <returns></returns>
                public Result CreateRest(string clustername, string bindnet0_addr = null, string bindnet1_addr = null, int? nodeid = null, string ring0_addr = null, string ring1_addr = null, int? votes = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("clustername", clustername);
                    parameters.Add("bindnet0_addr", bindnet0_addr);
                    parameters.Add("bindnet1_addr", bindnet1_addr);
                    parameters.Add("nodeid", nodeid);
                    parameters.Add("ring0_addr", ring0_addr);
                    parameters.Add("ring1_addr", ring1_addr);
                    parameters.Add("votes", votes);
                    return _client.Create($"/cluster/config", parameters);
                }
                /// <summary>
                /// Generate new cluster configuration.
                /// </summary>
                /// <param name="clustername">The name of the cluster.</param>
                /// <param name="bindnet0_addr">This specifies the network address the corosync ring 0 executive should bind to and defaults to the local IP address of the node.</param>
                /// <param name="bindnet1_addr">This specifies the network address the corosync ring 1 executive should bind to and is optional.</param>
                /// <param name="nodeid">Node id for this node.</param>
                /// <param name="ring0_addr">Hostname (or IP) of the corosync ring0 address of this node.</param>
                /// <param name="ring1_addr">Hostname (or IP) of the corosync ring1 address of this node. Requires a valid configured ring 1 (bindnet1_addr) in the cluster.</param>
                /// <param name="votes">Number of votes for this node.</param>
                /// <returns></returns>
                public Result Create(string clustername, string bindnet0_addr = null, string bindnet1_addr = null, int? nodeid = null, string ring0_addr = null, string ring1_addr = null, int? votes = null) { return CreateRest(clustername, bindnet0_addr, bindnet1_addr, nodeid, ring0_addr, ring1_addr, votes); }
            }
            public class PVEFirewall : Base
            {
                internal PVEFirewall(Client client)
                {
                    _client = client;
                }
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
                public class PVEGroups : Base
                {
                    internal PVEGroups(Client client)
                    {
                        _client = client;
                    }
                    public PVEItemGroup this[object group] => new PVEItemGroup(_client, group);
                    public class PVEItemGroup : Base
                    {
                        private object _group;
                        internal PVEItemGroup(Client client, object group)
                        {
                            _client = client;
                            _group = group;
                        }
                        public PVEItemPos this[object pos] => new PVEItemPos(_client, _group, pos);
                        public class PVEItemPos : Base
                        {
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
                            public Result DeleteRule(string digest = null) { return DeleteRest(digest); }
                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/cluster/firewall/groups/{_group}/{_pos}");
                            }
                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRule() { return GetRest(); }
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
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            /// <returns></returns>
                            public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                            /// <param name="macro">Use predefined standard macro.</param>
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            /// <returns></returns>
                            public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) { return SetRest(action, comment, delete, dest, digest, dport, enable, iface, macro, moveto, proto, source, sport, type); }
                        }
                        /// <summary>
                        /// Delete security group.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/cluster/firewall/groups/{_group}");
                        }
                        /// <summary>
                        /// Delete security group.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteSecurityGroup() { return DeleteRest(); }
                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/firewall/groups/{_group}");
                        }
                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRules() { return GetRest(); }
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
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
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
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) { return CreateRest(action, type, comment, dest, digest, dport, enable, iface, macro, pos, proto, source, sport); }
                    }
                    /// <summary>
                    /// List security groups.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/firewall/groups");
                    }
                    /// <summary>
                    /// List security groups.
                    /// </summary>
                    /// <returns></returns>
                    public Result ListSecurityGroups() { return GetRest(); }
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
                    public Result CreateSecurityGroup(string group, string comment = null, string digest = null, string rename = null) { return CreateRest(group, comment, digest, rename); }
                }
                public class PVERules : Base
                {
                    internal PVERules(Client client)
                    {
                        _client = client;
                    }
                    public PVEItemPos this[object pos] => new PVEItemPos(_client, pos);
                    public class PVEItemPos : Base
                    {
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
                        public Result DeleteRule(string digest = null) { return DeleteRest(digest); }
                        /// <summary>
                        /// Get single rule data.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/firewall/rules/{_pos}");
                        }
                        /// <summary>
                        /// Get single rule data.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRule() { return GetRest(); }
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
                        /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <returns></returns>
                        public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <returns></returns>
                        public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) { return SetRest(action, comment, delete, dest, digest, dport, enable, iface, macro, moveto, proto, source, sport, type); }
                    }
                    /// <summary>
                    /// List rules.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/firewall/rules");
                    }
                    /// <summary>
                    /// List rules.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRules() { return GetRest(); }
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
                    /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    /// <returns></returns>
                    public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
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
                    /// <param name="macro">Use predefined standard macro.</param>
                    /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    /// <returns></returns>
                    public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) { return CreateRest(action, type, comment, dest, digest, dport, enable, iface, macro, pos, proto, source, sport); }
                }
                public class PVEIpset : Base
                {
                    internal PVEIpset(Client client)
                    {
                        _client = client;
                    }
                    public PVEItemName this[object name] => new PVEItemName(_client, name);
                    public class PVEItemName : Base
                    {
                        private object _name;
                        internal PVEItemName(Client client, object name)
                        {
                            _client = client;
                            _name = name;
                        }
                        public PVEItemCidr this[object cidr] => new PVEItemCidr(_client, _name, cidr);
                        public class PVEItemCidr : Base
                        {
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
                            public Result RemoveIp(string digest = null) { return DeleteRest(digest); }
                            /// <summary>
                            /// Read IP or Network settings from IPSet.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/cluster/firewall/ipset/{_name}/{_cidr}");
                            }
                            /// <summary>
                            /// Read IP or Network settings from IPSet.
                            /// </summary>
                            /// <returns></returns>
                            public Result ReadIp() { return GetRest(); }
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
                            public Result UpdateIp(string comment = null, string digest = null, bool? nomatch = null) { return SetRest(comment, digest, nomatch); }
                        }
                        /// <summary>
                        /// Delete IPSet
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/cluster/firewall/ipset/{_name}");
                        }
                        /// <summary>
                        /// Delete IPSet
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteIpset() { return DeleteRest(); }
                        /// <summary>
                        /// List IPSet content
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/firewall/ipset/{_name}");
                        }
                        /// <summary>
                        /// List IPSet content
                        /// </summary>
                        /// <returns></returns>
                        public Result GetIpset() { return GetRest(); }
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
                        public Result CreateIp(string cidr, string comment = null, bool? nomatch = null) { return CreateRest(cidr, comment, nomatch); }
                    }
                    /// <summary>
                    /// List IPSets
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/firewall/ipset");
                    }
                    /// <summary>
                    /// List IPSets
                    /// </summary>
                    /// <returns></returns>
                    public Result IpsetIndex() { return GetRest(); }
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
                    public Result CreateIpset(string name, string comment = null, string digest = null, string rename = null) { return CreateRest(name, comment, digest, rename); }
                }
                public class PVEAliases : Base
                {
                    internal PVEAliases(Client client)
                    {
                        _client = client;
                    }
                    public PVEItemName this[object name] => new PVEItemName(_client, name);
                    public class PVEItemName : Base
                    {
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
                        public Result RemoveAlias(string digest = null) { return DeleteRest(digest); }
                        /// <summary>
                        /// Read alias.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/firewall/aliases/{_name}");
                        }
                        /// <summary>
                        /// Read alias.
                        /// </summary>
                        /// <returns></returns>
                        public Result ReadAlias() { return GetRest(); }
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
                        public Result UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null) { return SetRest(cidr, comment, digest, rename); }
                    }
                    /// <summary>
                    /// List aliases
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/firewall/aliases");
                    }
                    /// <summary>
                    /// List aliases
                    /// </summary>
                    /// <returns></returns>
                    public Result GetAliases() { return GetRest(); }
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
                    public Result CreateAlias(string cidr, string name, string comment = null) { return CreateRest(cidr, name, comment); }
                }
                public class PVEOptions : Base
                {
                    internal PVEOptions(Client client)
                    {
                        _client = client;
                    }
                    /// <summary>
                    /// Get Firewall options.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/firewall/options");
                    }
                    /// <summary>
                    /// Get Firewall options.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetOptions() { return GetRest(); }
                    /// <summary>
                    /// Set Firewall options.
                    /// </summary>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="ebtables">Enable ebtables rules cluster wide.</param>
                    /// <param name="enable">Enable or disable the firewall cluster wide.</param>
                    /// <param name="policy_in">Input policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <param name="policy_out">Output policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <returns></returns>
                    public Result SetRest(string delete = null, string digest = null, bool? ebtables = null, int? enable = null, string policy_in = null, string policy_out = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("delete", delete);
                        parameters.Add("digest", digest);
                        parameters.Add("ebtables", ebtables);
                        parameters.Add("enable", enable);
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
                    /// <param name="policy_in">Input policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <param name="policy_out">Output policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <returns></returns>
                    public Result SetOptions(string delete = null, string digest = null, bool? ebtables = null, int? enable = null, string policy_in = null, string policy_out = null) { return SetRest(delete, digest, ebtables, enable, policy_in, policy_out); }
                }
                public class PVEMacros : Base
                {
                    internal PVEMacros(Client client)
                    {
                        _client = client;
                    }
                    /// <summary>
                    /// List available macros
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/firewall/macros");
                    }
                    /// <summary>
                    /// List available macros
                    /// </summary>
                    /// <returns></returns>
                    public Result GetMacros() { return GetRest(); }
                }
                public class PVERefs : Base
                {
                    internal PVERefs(Client client)
                    {
                        _client = client;
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
                        return _client.Get($"/cluster/firewall/refs", parameters);
                    }
                    /// <summary>
                    /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                    /// </summary>
                    /// <param name="type">Only list references of specified type.
                    ///   Enum: alias,ipset</param>
                    /// <returns></returns>
                    public Result Refs(string type = null) { return GetRest(type); }
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/firewall");
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
            }
            public class PVEBackup : Base
            {
                internal PVEBackup(Client client)
                {
                    _client = client;
                }
                public PVEItemId this[object id] => new PVEItemId(_client, id);
                public class PVEItemId : Base
                {
                    private object _id;
                    internal PVEItemId(Client client, object id)
                    {
                        _client = client;
                        _id = id;
                    }
                    /// <summary>
                    /// Delete vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest()
                    {
                        return _client.Delete($"/cluster/backup/{_id}");
                    }
                    /// <summary>
                    /// Delete vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteJob() { return DeleteRest(); }
                    /// <summary>
                    /// Read vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/backup/{_id}");
                    }
                    /// <summary>
                    /// Read vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public Result ReadJob() { return GetRest(); }
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
                    /// <returns></returns>
                    public Result SetRest(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string delete = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
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
                    /// <returns></returns>
                    public Result UpdateJob(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string delete = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null) { return SetRest(starttime, all, bwlimit, compress, delete, dow, dumpdir, enabled, exclude, exclude_path, ionice, lockwait, mailnotification, mailto, maxfiles, mode, node, pigz, quiet, remove, script, size, stdexcludes, stop, stopwait, storage, tmpdir, vmid); }
                }
                /// <summary>
                /// List vzdump backup schedule.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/backup");
                }
                /// <summary>
                /// List vzdump backup schedule.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
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
                /// <returns></returns>
                public Result CreateRest(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
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
                /// <returns></returns>
                public Result CreateJob(string starttime, bool? all = null, int? bwlimit = null, string compress = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null) { return CreateRest(starttime, all, bwlimit, compress, dow, dumpdir, enabled, exclude, exclude_path, ionice, lockwait, mailnotification, mailto, maxfiles, mode, node, pigz, quiet, remove, script, size, stdexcludes, stop, stopwait, storage, tmpdir, vmid); }
            }
            public class PVEHa : Base
            {
                internal PVEHa(Client client)
                {
                    _client = client;
                }
                private PVEResources _resources;
                public PVEResources Resources => _resources ?? (_resources = new PVEResources(_client));
                private PVEGroups _groups;
                public PVEGroups Groups => _groups ?? (_groups = new PVEGroups(_client));
                private PVEStatus _status;
                public PVEStatus Status => _status ?? (_status = new PVEStatus(_client));
                public class PVEResources : Base
                {
                    internal PVEResources(Client client)
                    {
                        _client = client;
                    }
                    public PVEItemSid this[object sid] => new PVEItemSid(_client, sid);
                    public class PVEItemSid : Base
                    {
                        private object _sid;
                        internal PVEItemSid(Client client, object sid)
                        {
                            _client = client;
                            _sid = sid;
                        }
                        private PVEMigrate _migrate;
                        public PVEMigrate Migrate => _migrate ?? (_migrate = new PVEMigrate(_client, _sid));
                        private PVERelocate _relocate;
                        public PVERelocate Relocate => _relocate ?? (_relocate = new PVERelocate(_client, _sid));
                        public class PVEMigrate : Base
                        {
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
                            /// <param name="node">The cluster node name.</param>
                            /// <returns></returns>
                            public Result Migrate(string node) { return CreateRest(node); }
                        }
                        public class PVERelocate : Base
                        {
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
                            /// <param name="node">The cluster node name.</param>
                            /// <returns></returns>
                            public Result Relocate(string node) { return CreateRest(node); }
                        }
                        /// <summary>
                        /// Delete resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/cluster/ha/resources/{_sid}");
                        }
                        /// <summary>
                        /// Delete resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Delete() { return DeleteRest(); }
                        /// <summary>
                        /// Read resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/ha/resources/{_sid}");
                        }
                        /// <summary>
                        /// Read resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Read() { return GetRest(); }
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
                        public Result Update(string comment = null, string delete = null, string digest = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null) { return SetRest(comment, delete, digest, group, max_relocate, max_restart, state); }
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
                    public Result Index(string type = null) { return GetRest(type); }
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
                    public Result Create(string sid, string comment = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null, string type = null) { return CreateRest(sid, comment, group, max_relocate, max_restart, state, type); }
                }
                public class PVEGroups : Base
                {
                    internal PVEGroups(Client client)
                    {
                        _client = client;
                    }
                    public PVEItemGroup this[object group] => new PVEItemGroup(_client, group);
                    public class PVEItemGroup : Base
                    {
                        private object _group;
                        internal PVEItemGroup(Client client, object group)
                        {
                            _client = client;
                            _group = group;
                        }
                        /// <summary>
                        /// Delete ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/cluster/ha/groups/{_group}");
                        }
                        /// <summary>
                        /// Delete ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Delete() { return DeleteRest(); }
                        /// <summary>
                        /// Read ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/ha/groups/{_group}");
                        }
                        /// <summary>
                        /// Read ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Read() { return GetRest(); }
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
                        public Result Update(string comment = null, string delete = null, string digest = null, string nodes = null, bool? nofailback = null, bool? restricted = null) { return SetRest(comment, delete, digest, nodes, nofailback, restricted); }
                    }
                    /// <summary>
                    /// Get HA groups.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/ha/groups");
                    }
                    /// <summary>
                    /// Get HA groups.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
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
                    public Result Create(string group, string nodes, string comment = null, bool? nofailback = null, bool? restricted = null, string type = null) { return CreateRest(group, nodes, comment, nofailback, restricted, type); }
                }
                public class PVEStatus : Base
                {
                    internal PVEStatus(Client client)
                    {
                        _client = client;
                    }
                    private PVECurrent _current;
                    public PVECurrent Current => _current ?? (_current = new PVECurrent(_client));
                    private PVEManagerStatus _managerStatus;
                    public PVEManagerStatus ManagerStatus => _managerStatus ?? (_managerStatus = new PVEManagerStatus(_client));
                    public class PVECurrent : Base
                    {
                        internal PVECurrent(Client client)
                        {
                            _client = client;
                        }
                        /// <summary>
                        /// Get HA manger status.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/ha/status/current");
                        }
                        /// <summary>
                        /// Get HA manger status.
                        /// </summary>
                        /// <returns></returns>
                        public Result Status() { return GetRest(); }
                    }
                    public class PVEManagerStatus : Base
                    {
                        internal PVEManagerStatus(Client client)
                        {
                            _client = client;
                        }
                        /// <summary>
                        /// Get full HA manger status, including LRM status.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/ha/status/manager_status");
                        }
                        /// <summary>
                        /// Get full HA manger status, including LRM status.
                        /// </summary>
                        /// <returns></returns>
                        public Result ManagerStatus() { return GetRest(); }
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/ha/status");
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/ha");
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
            }
            public class PVEAcme : Base
            {
                internal PVEAcme(Client client)
                {
                    _client = client;
                }
                private PVEAccount _account;
                public PVEAccount Account => _account ?? (_account = new PVEAccount(_client));
                private PVETos _tos;
                public PVETos Tos => _tos ?? (_tos = new PVETos(_client));
                private PVEDirectories _directories;
                public PVEDirectories Directories => _directories ?? (_directories = new PVEDirectories(_client));
                public class PVEAccount : Base
                {
                    internal PVEAccount(Client client)
                    {
                        _client = client;
                    }
                    public PVEItemName this[object name] => new PVEItemName(_client, name);
                    public class PVEItemName : Base
                    {
                        private object _name;
                        internal PVEItemName(Client client, object name)
                        {
                            _client = client;
                            _name = name;
                        }
                        /// <summary>
                        /// Deactivate existing ACME account at CA.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/cluster/acme/account/{_name}");
                        }
                        /// <summary>
                        /// Deactivate existing ACME account at CA.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeactivateAccount() { return DeleteRest(); }
                        /// <summary>
                        /// Return existing ACME account information.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/cluster/acme/account/{_name}");
                        }
                        /// <summary>
                        /// Return existing ACME account information.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetAccount() { return GetRest(); }
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
                        public Result UpdateAccount(string contact = null) { return SetRest(contact); }
                    }
                    /// <summary>
                    /// ACMEAccount index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/acme/account");
                    }
                    /// <summary>
                    /// ACMEAccount index.
                    /// </summary>
                    /// <returns></returns>
                    public Result AccountIndex() { return GetRest(); }
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
                    public Result RegisterAccount(string contact, string directory = null, string name = null, string tos_url = null) { return CreateRest(contact, directory, name, tos_url); }
                }
                public class PVETos : Base
                {
                    internal PVETos(Client client)
                    {
                        _client = client;
                    }
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
                    public Result GetTos(string directory = null) { return GetRest(directory); }
                }
                public class PVEDirectories : Base
                {
                    internal PVEDirectories(Client client)
                    {
                        _client = client;
                    }
                    /// <summary>
                    /// Get named known ACME directory endpoints.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/cluster/acme/directories");
                    }
                    /// <summary>
                    /// Get named known ACME directory endpoints.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetDirectories() { return GetRest(); }
                }
                /// <summary>
                /// ACMEAccount index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/acme");
                }
                /// <summary>
                /// ACMEAccount index.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
            }
            public class PVELog : Base
            {
                internal PVELog(Client client)
                {
                    _client = client;
                }
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
                public Result Log(int? max = null) { return GetRest(max); }
            }
            public class PVEResources : Base
            {
                internal PVEResources(Client client)
                {
                    _client = client;
                }
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
                public Result Resources(string type = null) { return GetRest(type); }
            }
            public class PVETasks : Base
            {
                internal PVETasks(Client client)
                {
                    _client = client;
                }
                /// <summary>
                /// List recent tasks (cluster wide).
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/tasks");
                }
                /// <summary>
                /// List recent tasks (cluster wide).
                /// </summary>
                /// <returns></returns>
                public Result Tasks() { return GetRest(); }
            }
            public class PVEOptions : Base
            {
                internal PVEOptions(Client client)
                {
                    _client = client;
                }
                /// <summary>
                /// Get datacenter options.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/options");
                }
                /// <summary>
                /// Get datacenter options.
                /// </summary>
                /// <returns></returns>
                public Result GetOptions() { return GetRest(); }
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
                /// <param name="http_proxy">Specify external http proxy which is used for downloads (example: 'http://username:password@host:port/')</param>
                /// <param name="keyboard">Default keybord layout for vnc server.
                ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                /// <param name="language">Default GUI language.
                ///   Enum: en,de</param>
                /// <param name="mac_prefix">Prefix for autogenerated MAC addresses.</param>
                /// <param name="max_workers">Defines how many workers (per node) are maximal started  on actions like 'stopall VMs' or task from the ha-manager.</param>
                /// <param name="migration">For cluster wide migration settings.</param>
                /// <param name="migration_unsecure">Migration is secure using SSH tunnel by default. For secure private networks you can disable it to speed up migration. Deprecated, use the 'migration' property instead!</param>
                /// <returns></returns>
                public Result SetRest(string bwlimit = null, string console = null, string delete = null, string email_from = null, string fencing = null, string http_proxy = null, string keyboard = null, string language = null, string mac_prefix = null, int? max_workers = null, string migration = null, bool? migration_unsecure = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("bwlimit", bwlimit);
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
                /// <param name="http_proxy">Specify external http proxy which is used for downloads (example: 'http://username:password@host:port/')</param>
                /// <param name="keyboard">Default keybord layout for vnc server.
                ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                /// <param name="language">Default GUI language.
                ///   Enum: en,de</param>
                /// <param name="mac_prefix">Prefix for autogenerated MAC addresses.</param>
                /// <param name="max_workers">Defines how many workers (per node) are maximal started  on actions like 'stopall VMs' or task from the ha-manager.</param>
                /// <param name="migration">For cluster wide migration settings.</param>
                /// <param name="migration_unsecure">Migration is secure using SSH tunnel by default. For secure private networks you can disable it to speed up migration. Deprecated, use the 'migration' property instead!</param>
                /// <returns></returns>
                public Result SetOptions(string bwlimit = null, string console = null, string delete = null, string email_from = null, string fencing = null, string http_proxy = null, string keyboard = null, string language = null, string mac_prefix = null, int? max_workers = null, string migration = null, bool? migration_unsecure = null) { return SetRest(bwlimit, console, delete, email_from, fencing, http_proxy, keyboard, language, mac_prefix, max_workers, migration, migration_unsecure); }
            }
            public class PVEStatus : Base
            {
                internal PVEStatus(Client client)
                {
                    _client = client;
                }
                /// <summary>
                /// Get cluster status informations.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/cluster/status");
                }
                /// <summary>
                /// Get cluster status informations.
                /// </summary>
                /// <returns></returns>
                public Result GetStatus() { return GetRest(); }
            }
            public class PVENextid : Base
            {
                internal PVENextid(Client client)
                {
                    _client = client;
                }
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
                public Result Nextid(int? vmid = null) { return GetRest(vmid); }
            }
            /// <summary>
            /// Cluster index.
            /// </summary>
            /// <returns></returns>
            public Result GetRest()
            {
                return _client.Get($"/cluster");
            }
            /// <summary>
            /// Cluster index.
            /// </summary>
            /// <returns></returns>
            public Result Index() { return GetRest(); }
        }
        public class PVENodes : Base
        {
            internal PVENodes(Client client)
            {
                _client = client;
            }
            public PVEItemNode this[object node] => new PVEItemNode(_client, node);
            public class PVEItemNode : Base
            {
                private object _node;
                internal PVEItemNode(Client client, object node)
                {
                    _client = client;
                    _node = node;
                }
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
                private PVERrd _rrd;
                public PVERrd Rrd => _rrd ?? (_rrd = new PVERrd(_client, _node));
                private PVERrddata _rrddata;
                public PVERrddata Rrddata => _rrddata ?? (_rrddata = new PVERrddata(_client, _node));
                private PVESyslog _syslog;
                public PVESyslog Syslog => _syslog ?? (_syslog = new PVESyslog(_client, _node));
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
                public class PVEQemu : Base
                {
                    private object _node;
                    internal PVEQemu(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    public PVEItemVmid this[object vmid] => new PVEItemVmid(_client, _node, vmid);
                    public class PVEItemVmid : Base
                    {
                        private object _node;
                        private object _vmid;
                        internal PVEItemVmid(Client client, object node, object vmid)
                        {
                            _client = client;
                            _node = node;
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
                        public class PVEFirewall : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVEFirewall(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public class PVERules : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVERules(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemPos this[object pos] => new PVEItemPos(_client, _node, _vmid, pos);
                                public class PVEItemPos : Base
                                {
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
                                    public Result DeleteRule(string digest = null) { return DeleteRest(digest); }
                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest()
                                    {
                                        return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}");
                                    }
                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRule() { return GetRest(); }
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
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    /// <returns></returns>
                                    public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                                    /// <param name="macro">Use predefined standard macro.</param>
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    /// <returns></returns>
                                    public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) { return SetRest(action, comment, delete, dest, digest, dport, enable, iface, macro, moveto, proto, source, sport, type); }
                                }
                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/rules");
                                }
                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRules() { return GetRest(); }
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
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
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
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) { return CreateRest(action, type, comment, dest, digest, dport, enable, iface, macro, pos, proto, source, sport); }
                            }
                            public class PVEAliases : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEAliases(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemName this[object name] => new PVEItemName(_client, _node, _vmid, name);
                                public class PVEItemName : Base
                                {
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
                                    public Result RemoveAlias(string digest = null) { return DeleteRest(digest); }
                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest()
                                    {
                                        return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}");
                                    }
                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result ReadAlias() { return GetRest(); }
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
                                    public Result UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null) { return SetRest(cidr, comment, digest, rename); }
                                }
                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases");
                                }
                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public Result GetAliases() { return GetRest(); }
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
                                public Result CreateAlias(string cidr, string name, string comment = null) { return CreateRest(cidr, name, comment); }
                            }
                            public class PVEIpset : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEIpset(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemName this[object name] => new PVEItemName(_client, _node, _vmid, name);
                                public class PVEItemName : Base
                                {
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
                                    public PVEItemCidr this[object cidr] => new PVEItemCidr(_client, _node, _vmid, _name, cidr);
                                    public class PVEItemCidr : Base
                                    {
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
                                        public Result RemoveIp(string digest = null) { return DeleteRest(digest); }
                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public Result GetRest()
                                        {
                                            return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}");
                                        }
                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public Result ReadIp() { return GetRest(); }
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
                                        public Result UpdateIp(string comment = null, string digest = null, bool? nomatch = null) { return SetRest(comment, digest, nomatch); }
                                    }
                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result DeleteRest()
                                    {
                                        return _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}");
                                    }
                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result DeleteIpset() { return DeleteRest(); }
                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest()
                                    {
                                        return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}");
                                    }
                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetIpset() { return GetRest(); }
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
                                    public Result CreateIp(string cidr, string comment = null, bool? nomatch = null) { return CreateRest(cidr, comment, nomatch); }
                                }
                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset");
                                }
                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public Result IpsetIndex() { return GetRest(); }
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
                                public Result CreateIpset(string name, string comment = null, string digest = null, string rename = null) { return CreateRest(name, comment, digest, rename); }
                            }
                            public class PVEOptions : Base
                            {
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
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/options");
                                }
                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetOptions() { return GetRest(); }
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
                                public Result SetOptions(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null) { return SetRest(delete, dhcp, digest, enable, ipfilter, log_level_in, log_level_out, macfilter, ndp, policy_in, policy_out, radv); }
                            }
                            public class PVELog : Base
                            {
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
                                public Result Log(int? limit = null, int? start = null) { return GetRest(limit, start); }
                            }
                            public class PVERefs : Base
                            {
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
                                public Result Refs(string type = null) { return GetRest(type); }
                            }
                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall");
                            }
                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public Result Index() { return GetRest(); }
                        }
                        public class PVEAgent : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVEAgent(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public class PVEFsfreeze_Freeze : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEFsfreeze_Freeze(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-freeze.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-freeze");
                                }
                                /// <summary>
                                /// Execute fsfreeze-freeze.
                                /// </summary>
                                /// <returns></returns>
                                public Result Fsfreeze_Freeze() { return CreateRest(); }
                            }
                            public class PVEFsfreeze_Status : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEFsfreeze_Status(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-status.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-status");
                                }
                                /// <summary>
                                /// Execute fsfreeze-status.
                                /// </summary>
                                /// <returns></returns>
                                public Result Fsfreeze_Status() { return CreateRest(); }
                            }
                            public class PVEFsfreeze_Thaw : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEFsfreeze_Thaw(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-thaw.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-thaw");
                                }
                                /// <summary>
                                /// Execute fsfreeze-thaw.
                                /// </summary>
                                /// <returns></returns>
                                public Result Fsfreeze_Thaw() { return CreateRest(); }
                            }
                            public class PVEFstrim : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEFstrim(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fstrim.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fstrim");
                                }
                                /// <summary>
                                /// Execute fstrim.
                                /// </summary>
                                /// <returns></returns>
                                public Result Fstrim() { return CreateRest(); }
                            }
                            public class PVEGet_Fsinfo : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Fsinfo(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-fsinfo.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-fsinfo");
                                }
                                /// <summary>
                                /// Execute get-fsinfo.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Fsinfo() { return GetRest(); }
                            }
                            public class PVEGet_Host_Name : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Host_Name(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-host-name.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-host-name");
                                }
                                /// <summary>
                                /// Execute get-host-name.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Host_Name() { return GetRest(); }
                            }
                            public class PVEGet_Memory_Block_Info : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Memory_Block_Info(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-memory-block-info.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-memory-block-info");
                                }
                                /// <summary>
                                /// Execute get-memory-block-info.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Memory_Block_Info() { return GetRest(); }
                            }
                            public class PVEGet_Memory_Blocks : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Memory_Blocks(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-memory-blocks.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-memory-blocks");
                                }
                                /// <summary>
                                /// Execute get-memory-blocks.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Memory_Blocks() { return GetRest(); }
                            }
                            public class PVEGet_Osinfo : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Osinfo(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-osinfo.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-osinfo");
                                }
                                /// <summary>
                                /// Execute get-osinfo.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Osinfo() { return GetRest(); }
                            }
                            public class PVEGet_Time : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Time(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-time.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-time");
                                }
                                /// <summary>
                                /// Execute get-time.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Time() { return GetRest(); }
                            }
                            public class PVEGet_Timezone : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Timezone(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-timezone.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-timezone");
                                }
                                /// <summary>
                                /// Execute get-timezone.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Timezone() { return GetRest(); }
                            }
                            public class PVEGet_Users : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Users(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-users.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-users");
                                }
                                /// <summary>
                                /// Execute get-users.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Users() { return GetRest(); }
                            }
                            public class PVEGet_Vcpus : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEGet_Vcpus(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-vcpus.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-vcpus");
                                }
                                /// <summary>
                                /// Execute get-vcpus.
                                /// </summary>
                                /// <returns></returns>
                                public Result Get_Vcpus() { return GetRest(); }
                            }
                            public class PVEInfo : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEInfo(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute info.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/info");
                                }
                                /// <summary>
                                /// Execute info.
                                /// </summary>
                                /// <returns></returns>
                                public Result Info() { return GetRest(); }
                            }
                            public class PVENetwork_Get_Interfaces : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVENetwork_Get_Interfaces(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute network-get-interfaces.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/network-get-interfaces");
                                }
                                /// <summary>
                                /// Execute network-get-interfaces.
                                /// </summary>
                                /// <returns></returns>
                                public Result Network_Get_Interfaces() { return GetRest(); }
                            }
                            public class PVEPing : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEPing(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute ping.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/ping");
                                }
                                /// <summary>
                                /// Execute ping.
                                /// </summary>
                                /// <returns></returns>
                                public Result Ping() { return CreateRest(); }
                            }
                            public class PVEShutdown : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEShutdown(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute shutdown.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/shutdown");
                                }
                                /// <summary>
                                /// Execute shutdown.
                                /// </summary>
                                /// <returns></returns>
                                public Result Shutdown() { return CreateRest(); }
                            }
                            public class PVESuspend_Disk : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVESuspend_Disk(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-disk.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-disk");
                                }
                                /// <summary>
                                /// Execute suspend-disk.
                                /// </summary>
                                /// <returns></returns>
                                public Result Suspend_Disk() { return CreateRest(); }
                            }
                            public class PVESuspend_Hybrid : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVESuspend_Hybrid(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-hybrid.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-hybrid");
                                }
                                /// <summary>
                                /// Execute suspend-hybrid.
                                /// </summary>
                                /// <returns></returns>
                                public Result Suspend_Hybrid() { return CreateRest(); }
                            }
                            public class PVESuspend_Ram : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVESuspend_Ram(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-ram.
                                /// </summary>
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-ram");
                                }
                                /// <summary>
                                /// Execute suspend-ram.
                                /// </summary>
                                /// <returns></returns>
                                public Result Suspend_Ram() { return CreateRest(); }
                            }
                            /// <summary>
                            /// Qemu Agent command index.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent");
                            }
                            /// <summary>
                            /// Qemu Agent command index.
                            /// </summary>
                            /// <returns></returns>
                            public Result Index() { return GetRest(); }
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
                            public Result Agent(string command) { return CreateRest(command); }
                        }
                        public class PVERrd : Base
                        {
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
                            public Result Rrd(string ds, string timeframe, string cf = null) { return GetRest(ds, timeframe, cf); }
                        }
                        public class PVERrddata : Base
                        {
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
                            public Result Rrddata(string timeframe, string cf = null) { return GetRest(timeframe, cf); }
                        }
                        public class PVEConfig : Base
                        {
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
                            /// <returns></returns>
                            public Result GetRest(bool? current = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("current", current);
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/config", parameters);
                            }
                            /// <summary>
                            /// Get current virtual machine configuration. This does not include pending configuration changes (see 'pending' API).
                            /// </summary>
                            /// <param name="current">Get current values (instead of pending values).</param>
                            /// <returns></returns>
                            public Result VmConfig(bool? current = null) { return GetRest(current); }
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
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
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
                            /// <param name="vga">Select the VGA type.
                            ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public Result CreateRest(bool? acpi = null, bool? agent = null, string args = null, bool? autostart = null, int? background_delay = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmstatestorage = null, string watchdog = null)
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
                                parameters.Add("force", force);
                                parameters.Add("freeze", freeze);
                                parameters.Add("hotplug", hotplug);
                                parameters.Add("hugepages", hugepages);
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
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
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
                            /// <param name="vga">Select the VGA type.
                            ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public Result UpdateVmAsync(bool? acpi = null, bool? agent = null, string args = null, bool? autostart = null, int? background_delay = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmstatestorage = null, string watchdog = null) { return CreateRest(acpi, agent, args, autostart, background_delay, balloon, bios, boot, bootdisk, cdrom, cipassword, citype, ciuser, cores, cpu, cpulimit, cpuunits, delete, description, digest, force, freeze, hostpciN, hotplug, hugepages, ideN, ipconfigN, keyboard, kvm, localtime, lock_, machine, memory, migrate_downtime, migrate_speed, name, nameserver, netN, numa, numaN, onboot, ostype, parallelN, protection, reboot, revert, sataN, scsiN, scsihw, searchdomain, serialN, shares, skiplock, smbios1, smp, sockets, sshkeys, startdate, startup, tablet, tdf, template, unusedN, usbN, vcpus, vga, virtioN, vmstatestorage, watchdog); }
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
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
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
                            /// <param name="vga">Select the VGA type.
                            ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public Result SetRest(bool? acpi = null, bool? agent = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmstatestorage = null, string watchdog = null)
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
                                parameters.Add("force", force);
                                parameters.Add("freeze", freeze);
                                parameters.Add("hotplug", hotplug);
                                parameters.Add("hugepages", hugepages);
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
                            /// <param name="agent">Enable/disable Qemu GuestAgent.</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                            /// <param name="bootdisk">Enable booting from specified disk.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
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
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
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
                            /// <param name="vga">Select the VGA type.
                            ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public Result UpdateVm(bool? acpi = null, bool? agent = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, bool? tdf = null, bool? template = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmstatestorage = null, string watchdog = null) { return SetRest(acpi, agent, args, autostart, balloon, bios, boot, bootdisk, cdrom, cipassword, citype, ciuser, cores, cpu, cpulimit, cpuunits, delete, description, digest, force, freeze, hostpciN, hotplug, hugepages, ideN, ipconfigN, keyboard, kvm, localtime, lock_, machine, memory, migrate_downtime, migrate_speed, name, nameserver, netN, numa, numaN, onboot, ostype, parallelN, protection, reboot, revert, sataN, scsiN, scsihw, searchdomain, serialN, shares, skiplock, smbios1, smp, sockets, sshkeys, startdate, startup, tablet, tdf, template, unusedN, usbN, vcpus, vga, virtioN, vmstatestorage, watchdog); }
                        }
                        public class PVEPending : Base
                        {
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
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/pending");
                            }
                            /// <summary>
                            /// Get virtual machine configuration, including pending changes.
                            /// </summary>
                            /// <returns></returns>
                            public Result VmPending() { return GetRest(); }
                        }
                        public class PVEUnlink : Base
                        {
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
                            public Result Unlink(string idlist, bool? force = null) { return SetRest(idlist, force); }
                        }
                        public class PVEVncproxy : Base
                        {
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
                            public Result Vncproxy(bool? websocket = null) { return CreateRest(websocket); }
                        }
                        public class PVETermproxy : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVETermproxy(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public Result Termproxy(string serial = null) { return CreateRest(serial); }
                        }
                        public class PVEVncwebsocket : Base
                        {
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
                            public Result Vncwebsocket(int port, string vncticket) { return GetRest(port, vncticket); }
                        }
                        public class PVESpiceproxy : Base
                        {
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
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As resonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            /// <returns></returns>
                            public Result Spiceproxy(string proxy = null) { return CreateRest(proxy); }
                        }
                        public class PVEStatus : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVEStatus(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public class PVECurrent : Base
                            {
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
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/status/current");
                                }
                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                /// <returns></returns>
                                public Result VmStatus() { return GetRest(); }
                            }
                            public class PVEStart : Base
                            {
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
                                /// <param name="machine">Specific the Qemu machine type.</param>
                                /// <param name="migratedfrom">The cluster node name.</param>
                                /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                                /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                                ///   Enum: secure,insecure</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="stateuri">Some command save/restore state from this location.</param>
                                /// <param name="targetstorage">Target storage for the migration. (Can be '1' to use the same storage id as on the source node.)</param>
                                /// <returns></returns>
                                public Result VmStart(string machine = null, string migratedfrom = null, string migration_network = null, string migration_type = null, bool? skiplock = null, string stateuri = null, string targetstorage = null) { return CreateRest(machine, migratedfrom, migration_network, migration_type, skiplock, stateuri, targetstorage); }
                            }
                            public class PVEStop : Base
                            {
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
                                public Result VmStop(bool? keepActive = null, string migratedfrom = null, bool? skiplock = null, int? timeout = null) { return CreateRest(keepActive, migratedfrom, skiplock, timeout); }
                            }
                            public class PVEReset : Base
                            {
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
                                public Result VmReset(bool? skiplock = null) { return CreateRest(skiplock); }
                            }
                            public class PVEShutdown : Base
                            {
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
                                public Result VmShutdown(bool? forceStop = null, bool? keepActive = null, bool? skiplock = null, int? timeout = null) { return CreateRest(forceStop, keepActive, skiplock, timeout); }
                            }
                            public class PVESuspend : Base
                            {
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
                                /// <returns></returns>
                                public Result CreateRest(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/suspend", parameters);
                                }
                                /// <summary>
                                /// Suspend virtual machine.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public Result VmSuspend(bool? skiplock = null) { return CreateRest(skiplock); }
                            }
                            public class PVEResume : Base
                            {
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
                                public Result VmResume(bool? nocheck = null, bool? skiplock = null) { return CreateRest(nocheck, skiplock); }
                            }
                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/status");
                            }
                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public Result Vmcmdidx() { return GetRest(); }
                        }
                        public class PVESendkey : Base
                        {
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
                            public Result VmSendkey(string key, bool? skiplock = null) { return SetRest(key, skiplock); }
                        }
                        public class PVEFeature : Base
                        {
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
                            public Result VmFeature(string feature, string snapname = null) { return GetRest(feature, snapname); }
                        }
                        public class PVEClone : Base
                        {
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
                            /// <param name="format">Target format for file storage. Only valid for full clone.
                            ///   Enum: raw,qcow2,vmdk</param>
                            /// <param name="full">Create a full copy of all disks. This is always done when you clone a normal VM. For VM templates, we try to create a linked clone by default.</param>
                            /// <param name="name">Set a name for the new VM.</param>
                            /// <param name="pool">Add the new VM to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            /// <param name="target">Target node. Only allowed if the original VM is on shared storage.</param>
                            /// <returns></returns>
                            public Result CreateRest(int newid, string description = null, string format = null, bool? full = null, string name = null, string pool = null, string snapname = null, string storage = null, string target = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("newid", newid);
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
                            public Result CloneVm(int newid, string description = null, string format = null, bool? full = null, string name = null, string pool = null, string snapname = null, string storage = null, string target = null) { return CreateRest(newid, description, format, full, name, pool, snapname, storage, target); }
                        }
                        public class PVEMoveDisk : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVEMoveDisk(Client client, object node, object vmid)
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
                            /// <returns></returns>
                            public Result CreateRest(string disk, string storage, bool? delete = null, string digest = null, string format = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                parameters.Add("storage", storage);
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
                            /// <param name="delete">Delete the original disk after successful copy. By default the original disk is kept as unused disk.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="format">Target Format.
                            ///   Enum: raw,qcow2,vmdk</param>
                            /// <returns></returns>
                            public Result MoveVmDisk(string disk, string storage, bool? delete = null, string digest = null, string format = null) { return CreateRest(disk, storage, delete, digest, format); }
                        }
                        public class PVEMigrate : Base
                        {
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
                            /// <returns></returns>
                            public Result CreateRest(string target, bool? force = null, string migration_network = null, string migration_type = null, bool? online = null, string targetstorage = null, bool? with_local_disks = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target", target);
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
                            /// <param name="force">Allow to migrate VMs which use local devices. Only root may use this option.</param>
                            /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                            /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                            ///   Enum: secure,insecure</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="targetstorage">Default target storage.</param>
                            /// <param name="with_local_disks">Enable live storage migration for local disk</param>
                            /// <returns></returns>
                            public Result MigrateVm(string target, bool? force = null, string migration_network = null, string migration_type = null, bool? online = null, string targetstorage = null, bool? with_local_disks = null) { return CreateRest(target, force, migration_network, migration_type, online, targetstorage, with_local_disks); }
                        }
                        public class PVEMonitor : Base
                        {
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
                            public Result Monitor(string command) { return CreateRest(command); }
                        }
                        public class PVEResize : Base
                        {
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
                            public Result ResizeVm(string disk, string size, string digest = null, bool? skiplock = null) { return SetRest(disk, size, digest, skiplock); }
                        }
                        public class PVESnapshot : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVESnapshot(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }
                            public PVEItemSnapname this[object snapname] => new PVEItemSnapname(_client, _node, _vmid, snapname);
                            public class PVEItemSnapname : Base
                            {
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
                                private PVEConfig _config;
                                public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node, _vmid, _snapname));
                                private PVERollback _rollback;
                                public PVERollback Rollback => _rollback ?? (_rollback = new PVERollback(_client, _node, _vmid, _snapname));
                                public class PVEConfig : Base
                                {
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
                                    /// <returns></returns>
                                    public Result GetRest()
                                    {
                                        return _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/config");
                                    }
                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetSnapshotConfig() { return GetRest(); }
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
                                    public Result UpdateSnapshotConfig(string description = null) { return SetRest(description); }
                                }
                                public class PVERollback : Base
                                {
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
                                    /// <returns></returns>
                                    public Result CreateRest()
                                    {
                                        return _client.Create($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/rollback");
                                    }
                                    /// <summary>
                                    /// Rollback VM state to specified snapshot.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result Rollback() { return CreateRest(); }
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
                                public Result Delsnapshot(bool? force = null) { return DeleteRest(force); }
                                /// <summary>
                                /// 
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}");
                                }
                                /// <summary>
                                /// 
                                /// </summary>
                                /// <returns></returns>
                                public Result SnapshotCmdIdx() { return GetRest(); }
                            }
                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot");
                            }
                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public Result SnapshotList() { return GetRest(); }
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
                            public Result Snapshot(string snapname, string description = null, bool? vmstate = null) { return CreateRest(snapname, description, vmstate); }
                        }
                        public class PVETemplate : Base
                        {
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
                            public Result Template(string disk = null) { return CreateRest(disk); }
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
                        public Result DestroyVm(bool? skiplock = null) { return DeleteRest(skiplock); }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/qemu/{_vmid}");
                        }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result Vmdiridx() { return GetRest(); }
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
                    public Result Vmlist(bool? full = null) { return GetRest(full); }
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
                    /// <param name="bwlimit">Override i/o bandwidth limit (in KiB/s).</param>
                    /// <param name="cdrom">This is an alias for option -ide2</param>
                    /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                    /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                    ///   Enum: configdrive2,nocloud</param>
                    /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                    /// <param name="cores">The number of cores per socket.</param>
                    /// <param name="cpu">Emulated CPU type.</param>
                    /// <param name="cpulimit">Limit of CPU usage.</param>
                    /// <param name="cpuunits">CPU weight for a VM.</param>
                    /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                    /// <param name="force">Allow to overwrite existing VM.</param>
                    /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                    /// <param name="hostpciN">Map host PCI devices into guest.</param>
                    /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                    /// <param name="hugepages">Enable/disable hugepages memory.
                    ///   Enum: any,2,1024</param>
                    /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                    /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                    /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
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
                    /// <param name="vga">Select the VGA type.
                    ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                    /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                    /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                    /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                    /// <returns></returns>
                    public Result CreateRest(int vmid, bool? acpi = null, bool? agent = null, string archive = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, int? bwlimit = null, string cdrom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string description = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, string pool = null, bool? protection = null, bool? reboot = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, bool? start = null, string startdate = null, string startup = null, string storage = null, bool? tablet = null, bool? tdf = null, bool? template = null, bool? unique = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmstatestorage = null, string watchdog = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("vmid", vmid);
                        parameters.Add("acpi", acpi);
                        parameters.Add("agent", agent);
                        parameters.Add("archive", archive);
                        parameters.Add("args", args);
                        parameters.Add("autostart", autostart);
                        parameters.Add("balloon", balloon);
                        parameters.Add("bios", bios);
                        parameters.Add("boot", boot);
                        parameters.Add("bootdisk", bootdisk);
                        parameters.Add("bwlimit", bwlimit);
                        parameters.Add("cdrom", cdrom);
                        parameters.Add("cipassword", cipassword);
                        parameters.Add("citype", citype);
                        parameters.Add("ciuser", ciuser);
                        parameters.Add("cores", cores);
                        parameters.Add("cpu", cpu);
                        parameters.Add("cpulimit", cpulimit);
                        parameters.Add("cpuunits", cpuunits);
                        parameters.Add("description", description);
                        parameters.Add("force", force);
                        parameters.Add("freeze", freeze);
                        parameters.Add("hotplug", hotplug);
                        parameters.Add("hugepages", hugepages);
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
                    /// <param name="agent">Enable/disable Qemu GuestAgent.</param>
                    /// <param name="archive">The backup file.</param>
                    /// <param name="args">Arbitrary arguments passed to kvm.</param>
                    /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                    /// <param name="balloon">Amount of target RAM for the VM in MB. Using zero disables the ballon driver.</param>
                    /// <param name="bios">Select BIOS implementation.
                    ///   Enum: seabios,ovmf</param>
                    /// <param name="boot">Boot on floppy (a), hard disk (c), CD-ROM (d), or network (n).</param>
                    /// <param name="bootdisk">Enable booting from specified disk.</param>
                    /// <param name="bwlimit">Override i/o bandwidth limit (in KiB/s).</param>
                    /// <param name="cdrom">This is an alias for option -ide2</param>
                    /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                    /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                    ///   Enum: configdrive2,nocloud</param>
                    /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                    /// <param name="cores">The number of cores per socket.</param>
                    /// <param name="cpu">Emulated CPU type.</param>
                    /// <param name="cpulimit">Limit of CPU usage.</param>
                    /// <param name="cpuunits">CPU weight for a VM.</param>
                    /// <param name="description">Description for the VM. Only used on the configuration web interface. This is saved as comment inside the configuration file.</param>
                    /// <param name="force">Allow to overwrite existing VM.</param>
                    /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                    /// <param name="hostpciN">Map host PCI devices into guest.</param>
                    /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory' and 'usb'. Use '0' to disable hotplug completely. Value '1' is an alias for the default 'network,disk,usb'.</param>
                    /// <param name="hugepages">Enable/disable hugepages memory.
                    ///   Enum: any,2,1024</param>
                    /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3).</param>
                    /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                    /// <param name="keyboard">Keybord layout for vnc server. Default is read from the '/etc/pve/datacenter.cfg' configuration file.It should not be necessary to set it.
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
                    /// <param name="vga">Select the VGA type.
                    ///   Enum: std,cirrus,vmware,qxl,serial0,serial1,serial2,serial3,qxl2,qxl3,qxl4</param>
                    /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15).</param>
                    /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                    /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                    /// <returns></returns>
                    public Result CreateVm(int vmid, bool? acpi = null, bool? agent = null, string archive = null, string args = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, int? bwlimit = null, string cdrom = null, string cipassword = null, string citype = null, string ciuser = null, int? cores = null, string cpu = null, int? cpulimit = null, int? cpuunits = null, string description = null, bool? force = null, bool? freeze = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, int? memory = null, int? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, string pool = null, bool? protection = null, bool? reboot = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, string smbios1 = null, int? smp = null, int? sockets = null, string sshkeys = null, bool? start = null, string startdate = null, string startup = null, string storage = null, bool? tablet = null, bool? tdf = null, bool? template = null, bool? unique = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmstatestorage = null, string watchdog = null) { return CreateRest(vmid, acpi, agent, archive, args, autostart, balloon, bios, boot, bootdisk, bwlimit, cdrom, cipassword, citype, ciuser, cores, cpu, cpulimit, cpuunits, description, force, freeze, hostpciN, hotplug, hugepages, ideN, ipconfigN, keyboard, kvm, localtime, lock_, machine, memory, migrate_downtime, migrate_speed, name, nameserver, netN, numa, numaN, onboot, ostype, parallelN, pool, protection, reboot, sataN, scsiN, scsihw, searchdomain, serialN, shares, smbios1, smp, sockets, sshkeys, start, startdate, startup, storage, tablet, tdf, template, unique, unusedN, usbN, vcpus, vga, virtioN, vmstatestorage, watchdog); }
                }
                public class PVELxc : Base
                {
                    private object _node;
                    internal PVELxc(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    public PVEItemVmid this[object vmid] => new PVEItemVmid(_client, _node, vmid);
                    public class PVEItemVmid : Base
                    {
                        private object _node;
                        private object _vmid;
                        internal PVEItemVmid(Client client, object node, object vmid)
                        {
                            _client = client;
                            _node = node;
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
                        public class PVEConfig : Base
                        {
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
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/config");
                            }
                            /// <summary>
                            /// Get container configuration.
                            /// </summary>
                            /// <returns></returns>
                            public Result VmConfig() { return GetRest(); }
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
                            public Result SetRest(string arch = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string hostname = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, bool? protection = null, string rootfs = null, string searchdomain = null, string startup = null, int? swap = null, bool? template = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null)
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
                            public Result UpdateVm(string arch = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string hostname = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, bool? protection = null, string rootfs = null, string searchdomain = null, string startup = null, int? swap = null, bool? template = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null) { return SetRest(arch, cmode, console, cores, cpulimit, cpuunits, delete, description, digest, hostname, lock_, memory, mpN, nameserver, netN, onboot, ostype, protection, rootfs, searchdomain, startup, swap, template, tty, unprivileged, unusedN); }
                        }
                        public class PVEStatus : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVEStatus(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public class PVECurrent : Base
                            {
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
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/lxc/{_vmid}/status/current");
                                }
                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                /// <returns></returns>
                                public Result VmStatus() { return GetRest(); }
                            }
                            public class PVEStart : Base
                            {
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
                                public Result VmStart(bool? skiplock = null) { return CreateRest(skiplock); }
                            }
                            public class PVEStop : Base
                            {
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
                                public Result VmStop(bool? skiplock = null) { return CreateRest(skiplock); }
                            }
                            public class PVEShutdown : Base
                            {
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
                                public Result VmShutdown(bool? forceStop = null, int? timeout = null) { return CreateRest(forceStop, timeout); }
                            }
                            public class PVESuspend : Base
                            {
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
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/suspend");
                                }
                                /// <summary>
                                /// Suspend the container.
                                /// </summary>
                                /// <returns></returns>
                                public Result VmSuspend() { return CreateRest(); }
                            }
                            public class PVEResume : Base
                            {
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
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/resume");
                                }
                                /// <summary>
                                /// Resume the container.
                                /// </summary>
                                /// <returns></returns>
                                public Result VmResume() { return CreateRest(); }
                            }
                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/status");
                            }
                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public Result Vmcmdidx() { return GetRest(); }
                        }
                        public class PVESnapshot : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVESnapshot(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }
                            public PVEItemSnapname this[object snapname] => new PVEItemSnapname(_client, _node, _vmid, snapname);
                            public class PVEItemSnapname : Base
                            {
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
                                private PVERollback _rollback;
                                public PVERollback Rollback => _rollback ?? (_rollback = new PVERollback(_client, _node, _vmid, _snapname));
                                private PVEConfig _config;
                                public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node, _vmid, _snapname));
                                public class PVERollback : Base
                                {
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
                                    /// <returns></returns>
                                    public Result CreateRest()
                                    {
                                        return _client.Create($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/rollback");
                                    }
                                    /// <summary>
                                    /// Rollback LXC state to specified snapshot.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result Rollback() { return CreateRest(); }
                                }
                                public class PVEConfig : Base
                                {
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
                                    /// <returns></returns>
                                    public Result GetRest()
                                    {
                                        return _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/config");
                                    }
                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetSnapshotConfig() { return GetRest(); }
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
                                    public Result UpdateSnapshotConfig(string description = null) { return SetRest(description); }
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
                                public Result Delsnapshot(bool? force = null) { return DeleteRest(force); }
                                /// <summary>
                                /// 
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}");
                                }
                                /// <summary>
                                /// 
                                /// </summary>
                                /// <returns></returns>
                                public Result SnapshotCmdIdx() { return GetRest(); }
                            }
                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot");
                            }
                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public Result List() { return GetRest(); }
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
                            public Result Snapshot(string snapname, string description = null) { return CreateRest(snapname, description); }
                        }
                        public class PVEFirewall : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVEFirewall(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
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
                            public class PVERules : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVERules(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemPos this[object pos] => new PVEItemPos(_client, _node, _vmid, pos);
                                public class PVEItemPos : Base
                                {
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
                                    public Result DeleteRule(string digest = null) { return DeleteRest(digest); }
                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest()
                                    {
                                        return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}");
                                    }
                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRule() { return GetRest(); }
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
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    /// <returns></returns>
                                    public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                                    /// <param name="macro">Use predefined standard macro.</param>
                                    /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="type">Rule type.
                                    ///   Enum: in,out,group</param>
                                    /// <returns></returns>
                                    public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) { return SetRest(action, comment, delete, dest, digest, dport, enable, iface, macro, moveto, proto, source, sport, type); }
                                }
                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/rules");
                                }
                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRules() { return GetRest(); }
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
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
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
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) { return CreateRest(action, type, comment, dest, digest, dport, enable, iface, macro, pos, proto, source, sport); }
                            }
                            public class PVEAliases : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEAliases(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemName this[object name] => new PVEItemName(_client, _node, _vmid, name);
                                public class PVEItemName : Base
                                {
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
                                    public Result RemoveAlias(string digest = null) { return DeleteRest(digest); }
                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest()
                                    {
                                        return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}");
                                    }
                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result ReadAlias() { return GetRest(); }
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
                                    public Result UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null) { return SetRest(cidr, comment, digest, rename); }
                                }
                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases");
                                }
                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public Result GetAliases() { return GetRest(); }
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
                                public Result CreateAlias(string cidr, string name, string comment = null) { return CreateRest(cidr, name, comment); }
                            }
                            public class PVEIpset : Base
                            {
                                private object _node;
                                private object _vmid;
                                internal PVEIpset(Client client, object node, object vmid)
                                {
                                    _client = client;
                                    _node = node;
                                    _vmid = vmid;
                                }
                                public PVEItemName this[object name] => new PVEItemName(_client, _node, _vmid, name);
                                public class PVEItemName : Base
                                {
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
                                    public PVEItemCidr this[object cidr] => new PVEItemCidr(_client, _node, _vmid, _name, cidr);
                                    public class PVEItemCidr : Base
                                    {
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
                                        public Result RemoveIp(string digest = null) { return DeleteRest(digest); }
                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public Result GetRest()
                                        {
                                            return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}");
                                        }
                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public Result ReadIp() { return GetRest(); }
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
                                        public Result UpdateIp(string comment = null, string digest = null, bool? nomatch = null) { return SetRest(comment, digest, nomatch); }
                                    }
                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result DeleteRest()
                                    {
                                        return _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}");
                                    }
                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result DeleteIpset() { return DeleteRest(); }
                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetRest()
                                    {
                                        return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}");
                                    }
                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public Result GetIpset() { return GetRest(); }
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
                                    public Result CreateIp(string cidr, string comment = null, bool? nomatch = null) { return CreateRest(cidr, comment, nomatch); }
                                }
                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset");
                                }
                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public Result IpsetIndex() { return GetRest(); }
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
                                public Result CreateIpset(string name, string comment = null, string digest = null, string rename = null) { return CreateRest(name, comment, digest, rename); }
                            }
                            public class PVEOptions : Base
                            {
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
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/options");
                                }
                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                /// <returns></returns>
                                public Result GetOptions() { return GetRest(); }
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
                                public Result SetOptions(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null) { return SetRest(delete, dhcp, digest, enable, ipfilter, log_level_in, log_level_out, macfilter, ndp, policy_in, policy_out, radv); }
                            }
                            public class PVELog : Base
                            {
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
                                public Result Log(int? limit = null, int? start = null) { return GetRest(limit, start); }
                            }
                            public class PVERefs : Base
                            {
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
                                public Result Refs(string type = null) { return GetRest(type); }
                            }
                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall");
                            }
                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public Result Index() { return GetRest(); }
                        }
                        public class PVERrd : Base
                        {
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
                            public Result Rrd(string ds, string timeframe, string cf = null) { return GetRest(ds, timeframe, cf); }
                        }
                        public class PVERrddata : Base
                        {
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
                            public Result Rrddata(string timeframe, string cf = null) { return GetRest(timeframe, cf); }
                        }
                        public class PVEVncproxy : Base
                        {
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
                            public Result Vncproxy(int? height = null, bool? websocket = null, int? width = null) { return CreateRest(height, websocket, width); }
                        }
                        public class PVETermproxy : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVETermproxy(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Creates a TCP proxy connection.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest()
                            {
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/termproxy");
                            }
                            /// <summary>
                            /// Creates a TCP proxy connection.
                            /// </summary>
                            /// <returns></returns>
                            public Result Termproxy() { return CreateRest(); }
                        }
                        public class PVEVncwebsocket : Base
                        {
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
                            public Result Vncwebsocket(int port, string vncticket) { return GetRest(port, vncticket); }
                        }
                        public class PVESpiceproxy : Base
                        {
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
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As resonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            /// <returns></returns>
                            public Result Spiceproxy(string proxy = null) { return CreateRest(proxy); }
                        }
                        public class PVEMigrate : Base
                        {
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
                            /// <returns></returns>
                            public Result CreateRest(string target, bool? force = null, bool? online = null, bool? restart = null, int? timeout = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target", target);
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
                            /// <param name="force">Force migration despite local bind / device mounts. NOTE: deprecated, use 'shared' property of mount point instead.</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="restart">Use restart migration</param>
                            /// <param name="timeout">Timeout in seconds for shutdown for restart migration</param>
                            /// <returns></returns>
                            public Result MigrateVm(string target, bool? force = null, bool? online = null, bool? restart = null, int? timeout = null) { return CreateRest(target, force, online, restart, timeout); }
                        }
                        public class PVEFeature : Base
                        {
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
                            public Result VmFeature(string feature, string snapname = null) { return GetRest(feature, snapname); }
                        }
                        public class PVETemplate : Base
                        {
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
                            /// <returns></returns>
                            public Result CreateRest()
                            {
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/template");
                            }
                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <returns></returns>
                            public Result Template() { return CreateRest(); }
                        }
                        public class PVEClone : Base
                        {
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
                            /// <param name="newid">VMID for the clone.</param>
                            /// <param name="description">Description for the new CT.</param>
                            /// <param name="full">Create a full copy of all disks. This is always done when you clone a normal CT. For CT templates, we try to create a linked clone by default.</param>
                            /// <param name="hostname">Set a hostname for the new CT.</param>
                            /// <param name="pool">Add the new CT to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            /// <param name="target">Target node. Only allowed if the original VM is on shared storage.</param>
                            /// <returns></returns>
                            public Result CreateRest(int newid, string description = null, bool? full = null, string hostname = null, string pool = null, string snapname = null, string storage = null, string target = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("newid", newid);
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
                            /// <param name="description">Description for the new CT.</param>
                            /// <param name="full">Create a full copy of all disks. This is always done when you clone a normal CT. For CT templates, we try to create a linked clone by default.</param>
                            /// <param name="hostname">Set a hostname for the new CT.</param>
                            /// <param name="pool">Add the new CT to the specified pool.</param>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="storage">Target storage for full clone.</param>
                            /// <param name="target">Target node. Only allowed if the original VM is on shared storage.</param>
                            /// <returns></returns>
                            public Result CloneVm(int newid, string description = null, bool? full = null, string hostname = null, string pool = null, string snapname = null, string storage = null, string target = null) { return CreateRest(newid, description, full, hostname, pool, snapname, storage, target); }
                        }
                        public class PVEResize : Base
                        {
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
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9</param>
                            /// <param name="size">The new size. With the '+' sign the value is added to the actual size of the volume and without it, the value is taken as an absolute one. Shrinking disk size is not supported.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result ResizeVm(string disk, string size, string digest = null) { return SetRest(disk, size, digest); }
                        }
                        public class PVEMoveVolume : Base
                        {
                            private object _node;
                            private object _vmid;
                            internal PVEMoveVolume(Client client, object node, object vmid)
                            {
                                _client = client;
                                _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Move a rootfs-/mp-volume to a different storage
                            /// </summary>
                            /// <param name="storage">Target Storage.</param>
                            /// <param name="volume">Volume which will be moved.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9</param>
                            /// <param name="delete">Delete the original volume after successful copy. By default the original is kept as an unused volume entry.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result CreateRest(string storage, string volume, bool? delete = null, string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("storage", storage);
                                parameters.Add("volume", volume);
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                return _client.Create($"/nodes/{_node}/lxc/{_vmid}/move_volume", parameters);
                            }
                            /// <summary>
                            /// Move a rootfs-/mp-volume to a different storage
                            /// </summary>
                            /// <param name="storage">Target Storage.</param>
                            /// <param name="volume">Volume which will be moved.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9</param>
                            /// <param name="delete">Delete the original volume after successful copy. By default the original is kept as an unused volume entry.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public Result MoveVolume(string storage, string volume, bool? delete = null, string digest = null) { return CreateRest(storage, volume, delete, digest); }
                        }
                        /// <summary>
                        /// Destroy the container (also delete all uses files).
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/nodes/{_node}/lxc/{_vmid}");
                        }
                        /// <summary>
                        /// Destroy the container (also delete all uses files).
                        /// </summary>
                        /// <returns></returns>
                        public Result DestroyVm() { return DeleteRest(); }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/lxc/{_vmid}");
                        }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result Vmdiridx() { return GetRest(); }
                    }
                    /// <summary>
                    /// LXC container index (per node).
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/lxc");
                    }
                    /// <summary>
                    /// LXC container index (per node).
                    /// </summary>
                    /// <returns></returns>
                    public Result Vmlist() { return GetRest(); }
                    /// <summary>
                    /// Create or restore a container.
                    /// </summary>
                    /// <param name="ostemplate">The OS template or backup file.</param>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="arch">OS architecture type.
                    ///   Enum: amd64,i386</param>
                    /// <param name="bwlimit">Override i/o bandwidth limit (in KiB/s).</param>
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
                    /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <returns></returns>
                    public Result CreateRest(string ostemplate, int vmid, string arch = null, int? bwlimit = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string description = null, bool? force = null, string hostname = null, bool? ignore_unpack_errors = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, string password = null, string pool = null, bool? protection = null, bool? restore = null, string rootfs = null, string searchdomain = null, string ssh_public_keys = null, bool? start = null, string startup = null, string storage = null, int? swap = null, bool? template = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null)
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
                        parameters.Add("force", force);
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
                    ///   Enum: amd64,i386</param>
                    /// <param name="bwlimit">Override i/o bandwidth limit (in KiB/s).</param>
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
                    /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <returns></returns>
                    public Result CreateVm(string ostemplate, int vmid, string arch = null, int? bwlimit = null, string cmode = null, bool? console = null, int? cores = null, int? cpulimit = null, int? cpuunits = null, string description = null, bool? force = null, string hostname = null, bool? ignore_unpack_errors = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, string password = null, string pool = null, bool? protection = null, bool? restore = null, string rootfs = null, string searchdomain = null, string ssh_public_keys = null, bool? start = null, string startup = null, string storage = null, int? swap = null, bool? template = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null) { return CreateRest(ostemplate, vmid, arch, bwlimit, cmode, console, cores, cpulimit, cpuunits, description, force, hostname, ignore_unpack_errors, lock_, memory, mpN, nameserver, netN, onboot, ostype, password, pool, protection, restore, rootfs, searchdomain, ssh_public_keys, start, startup, storage, swap, template, tty, unprivileged, unusedN); }
                }
                public class PVECeph : Base
                {
                    private object _node;
                    internal PVECeph(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    private PVEOsd _osd;
                    public PVEOsd Osd => _osd ?? (_osd = new PVEOsd(_client, _node));
                    private PVEDisks _disks;
                    public PVEDisks Disks => _disks ?? (_disks = new PVEDisks(_client, _node));
                    private PVEConfig _config;
                    public PVEConfig Config => _config ?? (_config = new PVEConfig(_client, _node));
                    private PVEMon _mon;
                    public PVEMon Mon => _mon ?? (_mon = new PVEMon(_client, _node));
                    private PVEInit _init;
                    public PVEInit Init => _init ?? (_init = new PVEInit(_client, _node));
                    private PVEMgr _mgr;
                    public PVEMgr Mgr => _mgr ?? (_mgr = new PVEMgr(_client, _node));
                    private PVEStop _stop;
                    public PVEStop Stop => _stop ?? (_stop = new PVEStop(_client, _node));
                    private PVEStart _start;
                    public PVEStart Start => _start ?? (_start = new PVEStart(_client, _node));
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
                    public class PVEOsd : Base
                    {
                        private object _node;
                        internal PVEOsd(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        public PVEItemOsdid this[object osdid] => new PVEItemOsdid(_client, _node, osdid);
                        public class PVEItemOsdid : Base
                        {
                            private object _node;
                            private object _osdid;
                            internal PVEItemOsdid(Client client, object node, object osdid)
                            {
                                _client = client;
                                _node = node;
                                _osdid = osdid;
                            }
                            private PVEIn _in;
                            public PVEIn In => _in ?? (_in = new PVEIn(_client, _node, _osdid));
                            private PVEOut _out;
                            public PVEOut Out => _out ?? (_out = new PVEOut(_client, _node, _osdid));
                            public class PVEIn : Base
                            {
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
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/ceph/osd/{_osdid}/in");
                                }
                                /// <summary>
                                /// ceph osd in
                                /// </summary>
                                /// <returns></returns>
                                public Result In() { return CreateRest(); }
                            }
                            public class PVEOut : Base
                            {
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
                                /// <returns></returns>
                                public Result CreateRest()
                                {
                                    return _client.Create($"/nodes/{_node}/ceph/osd/{_osdid}/out");
                                }
                                /// <summary>
                                /// ceph osd out
                                /// </summary>
                                /// <returns></returns>
                                public Result Out() { return CreateRest(); }
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
                            public Result Destroyosd(bool? cleanup = null) { return DeleteRest(cleanup); }
                        }
                        /// <summary>
                        /// Get Ceph osd list/tree.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/ceph/osd");
                        }
                        /// <summary>
                        /// Get Ceph osd list/tree.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() { return GetRest(); }
                        /// <summary>
                        /// Create OSD
                        /// </summary>
                        /// <param name="dev">Block device name.</param>
                        /// <param name="bluestore">Use bluestore instead of filestore. This is the default.</param>
                        /// <param name="fstype">File system type (filestore only).
                        ///   Enum: xfs,ext4,btrfs</param>
                        /// <param name="journal_dev">Block device name for journal (filestore) or block.db (bluestore).</param>
                        /// <param name="wal_dev">Block device name for block.wal (bluestore only).</param>
                        /// <returns></returns>
                        public Result CreateRest(string dev, bool? bluestore = null, string fstype = null, string journal_dev = null, string wal_dev = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("dev", dev);
                            parameters.Add("bluestore", bluestore);
                            parameters.Add("fstype", fstype);
                            parameters.Add("journal_dev", journal_dev);
                            parameters.Add("wal_dev", wal_dev);
                            return _client.Create($"/nodes/{_node}/ceph/osd", parameters);
                        }
                        /// <summary>
                        /// Create OSD
                        /// </summary>
                        /// <param name="dev">Block device name.</param>
                        /// <param name="bluestore">Use bluestore instead of filestore. This is the default.</param>
                        /// <param name="fstype">File system type (filestore only).
                        ///   Enum: xfs,ext4,btrfs</param>
                        /// <param name="journal_dev">Block device name for journal (filestore) or block.db (bluestore).</param>
                        /// <param name="wal_dev">Block device name for block.wal (bluestore only).</param>
                        /// <returns></returns>
                        public Result Createosd(string dev, bool? bluestore = null, string fstype = null, string journal_dev = null, string wal_dev = null) { return CreateRest(dev, bluestore, fstype, journal_dev, wal_dev); }
                    }
                    public class PVEDisks : Base
                    {
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
                        public Result Disks(string type = null) { return GetRest(type); }
                    }
                    public class PVEConfig : Base
                    {
                        private object _node;
                        internal PVEConfig(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Get Ceph configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/ceph/config");
                        }
                        /// <summary>
                        /// Get Ceph configuration.
                        /// </summary>
                        /// <returns></returns>
                        public Result Config() { return GetRest(); }
                    }
                    public class PVEMon : Base
                    {
                        private object _node;
                        internal PVEMon(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        public PVEItemMonid this[object monid] => new PVEItemMonid(_client, _node, monid);
                        public class PVEItemMonid : Base
                        {
                            private object _node;
                            private object _monid;
                            internal PVEItemMonid(Client client, object node, object monid)
                            {
                                _client = client;
                                _node = node;
                                _monid = monid;
                            }
                            /// <summary>
                            /// Destroy Ceph Monitor and Manager.
                            /// </summary>
                            /// <param name="exclude_manager">When set, removes only the monitor, not the manager</param>
                            /// <returns></returns>
                            public Result DeleteRest(bool? exclude_manager = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("exclude-manager", exclude_manager);
                                return _client.Delete($"/nodes/{_node}/ceph/mon/{_monid}", parameters);
                            }
                            /// <summary>
                            /// Destroy Ceph Monitor and Manager.
                            /// </summary>
                            /// <param name="exclude_manager">When set, removes only the monitor, not the manager</param>
                            /// <returns></returns>
                            public Result Destroymon(bool? exclude_manager = null) { return DeleteRest(exclude_manager); }
                        }
                        /// <summary>
                        /// Get Ceph monitor list.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/ceph/mon");
                        }
                        /// <summary>
                        /// Get Ceph monitor list.
                        /// </summary>
                        /// <returns></returns>
                        public Result Listmon() { return GetRest(); }
                        /// <summary>
                        /// Create Ceph Monitor and Manager
                        /// </summary>
                        /// <param name="exclude_manager">When set, only a monitor will be created.</param>
                        /// <param name="id">The ID for the monitor, when omitted the same as the nodename</param>
                        /// <param name="mon_address">Overwrites autodetected monitor IP address. Must be in the public network of ceph.</param>
                        /// <returns></returns>
                        public Result CreateRest(bool? exclude_manager = null, string id = null, string mon_address = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("exclude-manager", exclude_manager);
                            parameters.Add("id", id);
                            parameters.Add("mon-address", mon_address);
                            return _client.Create($"/nodes/{_node}/ceph/mon", parameters);
                        }
                        /// <summary>
                        /// Create Ceph Monitor and Manager
                        /// </summary>
                        /// <param name="exclude_manager">When set, only a monitor will be created.</param>
                        /// <param name="id">The ID for the monitor, when omitted the same as the nodename</param>
                        /// <param name="mon_address">Overwrites autodetected monitor IP address. Must be in the public network of ceph.</param>
                        /// <returns></returns>
                        public Result Createmon(bool? exclude_manager = null, string id = null, string mon_address = null) { return CreateRest(exclude_manager, id, mon_address); }
                    }
                    public class PVEInit : Base
                    {
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
                        /// <returns></returns>
                        public Result CreateRest(bool? disable_cephx = null, int? min_size = null, string network = null, int? pg_bits = null, int? size = null)
                        {
                            var parameters = new Dictionary<string, object>();
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
                        /// <param name="disable_cephx">Disable cephx authentification.  WARNING: cephx is a security feature protecting against man-in-the-middle attacks. Only consider disabling cephx if your network is private!</param>
                        /// <param name="min_size">Minimum number of available replicas per object to allow I/O</param>
                        /// <param name="network">Use specific network for all ceph related traffic</param>
                        /// <param name="pg_bits">Placement group bits, used to specify the default number of placement groups.  NOTE: 'osd pool default pg num' does not work for default pools.</param>
                        /// <param name="size">Targeted number of replicas per object</param>
                        /// <returns></returns>
                        public Result Init(bool? disable_cephx = null, int? min_size = null, string network = null, int? pg_bits = null, int? size = null) { return CreateRest(disable_cephx, min_size, network, pg_bits, size); }
                    }
                    public class PVEMgr : Base
                    {
                        private object _node;
                        internal PVEMgr(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        public PVEItemId this[object id] => new PVEItemId(_client, _node, id);
                        public class PVEItemId : Base
                        {
                            private object _node;
                            private object _id;
                            internal PVEItemId(Client client, object node, object id)
                            {
                                _client = client;
                                _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Destroy Ceph Manager.
                            /// </summary>
                            /// <returns></returns>
                            public Result DeleteRest()
                            {
                                return _client.Delete($"/nodes/{_node}/ceph/mgr/{_id}");
                            }
                            /// <summary>
                            /// Destroy Ceph Manager.
                            /// </summary>
                            /// <returns></returns>
                            public Result Destroymgr() { return DeleteRest(); }
                        }
                        /// <summary>
                        /// Create Ceph Manager
                        /// </summary>
                        /// <param name="id">The ID for the manager, when omitted the same as the nodename</param>
                        /// <returns></returns>
                        public Result CreateRest(string id = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("id", id);
                            return _client.Create($"/nodes/{_node}/ceph/mgr", parameters);
                        }
                        /// <summary>
                        /// Create Ceph Manager
                        /// </summary>
                        /// <param name="id">The ID for the manager, when omitted the same as the nodename</param>
                        /// <returns></returns>
                        public Result Createmgr(string id = null) { return CreateRest(id); }
                    }
                    public class PVEStop : Base
                    {
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
                        public Result Stop(string service = null) { return CreateRest(service); }
                    }
                    public class PVEStart : Base
                    {
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
                        public Result Start(string service = null) { return CreateRest(service); }
                    }
                    public class PVEStatus : Base
                    {
                        private object _node;
                        internal PVEStatus(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Get ceph status.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/ceph/status");
                        }
                        /// <summary>
                        /// Get ceph status.
                        /// </summary>
                        /// <returns></returns>
                        public Result Status() { return GetRest(); }
                    }
                    public class PVEPools : Base
                    {
                        private object _node;
                        internal PVEPools(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        public PVEItemName this[object name] => new PVEItemName(_client, _node, name);
                        public class PVEItemName : Base
                        {
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
                            public Result Destroypool(bool? force = null, bool? remove_storages = null) { return DeleteRest(force, remove_storages); }
                        }
                        /// <summary>
                        /// List all pools.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/ceph/pools");
                        }
                        /// <summary>
                        /// List all pools.
                        /// </summary>
                        /// <returns></returns>
                        public Result Lspools() { return GetRest(); }
                        /// <summary>
                        /// Create POOL
                        /// </summary>
                        /// <param name="name">The name of the pool. It must be unique.</param>
                        /// <param name="add_storages">Configure VM and CT storages using the new pool.</param>
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
                        /// <param name="add_storages">Configure VM and CT storages using the new pool.</param>
                        /// <param name="application">The application of the pool, 'rbd' by default.
                        ///   Enum: rbd,cephfs,rgw</param>
                        /// <param name="crush_rule">The rule to use for mapping object placement in the cluster.</param>
                        /// <param name="min_size">Minimum number of replicas per object</param>
                        /// <param name="pg_num">Number of placement groups.</param>
                        /// <param name="size">Number of replicas per object</param>
                        /// <returns></returns>
                        public Result Createpool(string name, bool? add_storages = null, string application = null, string crush_rule = null, int? min_size = null, int? pg_num = null, int? size = null) { return CreateRest(name, add_storages, application, crush_rule, min_size, pg_num, size); }
                    }
                    public class PVEFlags : Base
                    {
                        private object _node;
                        internal PVEFlags(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        public PVEItemFlag this[object flag] => new PVEItemFlag(_client, _node, flag);
                        public class PVEItemFlag : Base
                        {
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
                            /// <returns></returns>
                            public Result DeleteRest()
                            {
                                return _client.Delete($"/nodes/{_node}/ceph/flags/{_flag}");
                            }
                            /// <summary>
                            /// Unset a ceph flag
                            /// </summary>
                            /// <returns></returns>
                            public Result UnsetFlag() { return DeleteRest(); }
                            /// <summary>
                            /// Set a ceph flag
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest()
                            {
                                return _client.Create($"/nodes/{_node}/ceph/flags/{_flag}");
                            }
                            /// <summary>
                            /// Set a ceph flag
                            /// </summary>
                            /// <returns></returns>
                            public Result SetFlag() { return CreateRest(); }
                        }
                        /// <summary>
                        /// get all set ceph flags
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/ceph/flags");
                        }
                        /// <summary>
                        /// get all set ceph flags
                        /// </summary>
                        /// <returns></returns>
                        public Result GetFlags() { return GetRest(); }
                    }
                    public class PVECrush : Base
                    {
                        private object _node;
                        internal PVECrush(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Get OSD crush map
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/ceph/crush");
                        }
                        /// <summary>
                        /// Get OSD crush map
                        /// </summary>
                        /// <returns></returns>
                        public Result Crush() { return GetRest(); }
                    }
                    public class PVELog : Base
                    {
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
                        public Result Log(int? limit = null, int? start = null) { return GetRest(limit, start); }
                    }
                    public class PVERules : Base
                    {
                        private object _node;
                        internal PVERules(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// List ceph rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/ceph/rules");
                        }
                        /// <summary>
                        /// List ceph rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result Rules() { return GetRest(); }
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/ceph");
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
                }
                public class PVEVzdump : Base
                {
                    private object _node;
                    internal PVEVzdump(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    private PVEExtractconfig _extractconfig;
                    public PVEExtractconfig Extractconfig => _extractconfig ?? (_extractconfig = new PVEExtractconfig(_client, _node));
                    public class PVEExtractconfig : Base
                    {
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
                        public Result Extractconfig(string volume) { return GetRest(volume); }
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
                    /// <returns></returns>
                    public Result CreateRest(bool? all = null, int? bwlimit = null, string compress = null, string dumpdir = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stdout = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null)
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
                    /// <returns></returns>
                    public Result Vzdump(bool? all = null, int? bwlimit = null, string compress = null, string dumpdir = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, int? pigz = null, bool? quiet = null, bool? remove = null, string script = null, int? size = null, bool? stdexcludes = null, bool? stdout = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null) { return CreateRest(all, bwlimit, compress, dumpdir, exclude, exclude_path, ionice, lockwait, mailnotification, mailto, maxfiles, mode, pigz, quiet, remove, script, size, stdexcludes, stdout, stop, stopwait, storage, tmpdir, vmid); }
                }
                public class PVEServices : Base
                {
                    private object _node;
                    internal PVEServices(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    public PVEItemService this[object service] => new PVEItemService(_client, _node, service);
                    public class PVEItemService : Base
                    {
                        private object _node;
                        private object _service;
                        internal PVEItemService(Client client, object node, object service)
                        {
                            _client = client;
                            _node = node;
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
                        public class PVEState : Base
                        {
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
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/services/{_service}/state");
                            }
                            /// <summary>
                            /// Read service properties
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceState() { return GetRest(); }
                        }
                        public class PVEStart : Base
                        {
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
                            /// <returns></returns>
                            public Result CreateRest()
                            {
                                return _client.Create($"/nodes/{_node}/services/{_service}/start");
                            }
                            /// <summary>
                            /// Start service.
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceStart() { return CreateRest(); }
                        }
                        public class PVEStop : Base
                        {
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
                            /// <returns></returns>
                            public Result CreateRest()
                            {
                                return _client.Create($"/nodes/{_node}/services/{_service}/stop");
                            }
                            /// <summary>
                            /// Stop service.
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceStop() { return CreateRest(); }
                        }
                        public class PVERestart : Base
                        {
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
                            /// <returns></returns>
                            public Result CreateRest()
                            {
                                return _client.Create($"/nodes/{_node}/services/{_service}/restart");
                            }
                            /// <summary>
                            /// Restart service.
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceRestart() { return CreateRest(); }
                        }
                        public class PVEReload : Base
                        {
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
                            /// <returns></returns>
                            public Result CreateRest()
                            {
                                return _client.Create($"/nodes/{_node}/services/{_service}/reload");
                            }
                            /// <summary>
                            /// Reload service.
                            /// </summary>
                            /// <returns></returns>
                            public Result ServiceReload() { return CreateRest(); }
                        }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/services/{_service}");
                        }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public Result Srvcmdidx() { return GetRest(); }
                    }
                    /// <summary>
                    /// Service list.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/services");
                    }
                    /// <summary>
                    /// Service list.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
                }
                public class PVESubscription : Base
                {
                    private object _node;
                    internal PVESubscription(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Read subscription info.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/subscription");
                    }
                    /// <summary>
                    /// Read subscription info.
                    /// </summary>
                    /// <returns></returns>
                    public Result Get() { return GetRest(); }
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
                    public Result Update(bool? force = null) { return CreateRest(force); }
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
                    public Result Set(string key) { return SetRest(key); }
                }
                public class PVENetwork : Base
                {
                    private object _node;
                    internal PVENetwork(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    public PVEItemIface this[object iface] => new PVEItemIface(_client, _node, iface);
                    public class PVEItemIface : Base
                    {
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
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/nodes/{_node}/network/{_iface}");
                        }
                        /// <summary>
                        /// Delete network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteNetwork() { return DeleteRest(); }
                        /// <summary>
                        /// Read network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/network/{_iface}");
                        }
                        /// <summary>
                        /// Read network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public Result NetworkConfig() { return GetRest(); }
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
                        /// <returns></returns>
                        public Result SetRest(string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string comments = null, string comments6 = null, string delete = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null)
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
                        /// <returns></returns>
                        public Result UpdateNetwork(string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string comments = null, string comments6 = null, string delete = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null) { return SetRest(type, address, address6, autostart, bond_mode, bond_xmit_hash_policy, bridge_ports, bridge_vlan_aware, comments, comments6, delete, gateway, gateway6, netmask, netmask6, ovs_bonds, ovs_bridge, ovs_options, ovs_ports, ovs_tag, slaves); }
                    }
                    /// <summary>
                    /// Revert network configuration changes.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest()
                    {
                        return _client.Delete($"/nodes/{_node}/network");
                    }
                    /// <summary>
                    /// Revert network configuration changes.
                    /// </summary>
                    /// <returns></returns>
                    public Result RevertNetworkChanges() { return DeleteRest(); }
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
                    public Result Index(string type = null) { return GetRest(type); }
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
                    /// <returns></returns>
                    public Result CreateRest(string iface, string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string comments = null, string comments6 = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null)
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
                    /// <returns></returns>
                    public Result CreateNetwork(string iface, string type, string address = null, string address6 = null, bool? autostart = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string comments = null, string comments6 = null, string gateway = null, string gateway6 = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null) { return CreateRest(iface, type, address, address6, autostart, bond_mode, bond_xmit_hash_policy, bridge_ports, bridge_vlan_aware, comments, comments6, gateway, gateway6, netmask, netmask6, ovs_bonds, ovs_bridge, ovs_options, ovs_ports, ovs_tag, slaves); }
                }
                public class PVETasks : Base
                {
                    private object _node;
                    internal PVETasks(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    public PVEItemUpid this[object upid] => new PVEItemUpid(_client, _node, upid);
                    public class PVEItemUpid : Base
                    {
                        private object _node;
                        private object _upid;
                        internal PVEItemUpid(Client client, object node, object upid)
                        {
                            _client = client;
                            _node = node;
                            _upid = upid;
                        }
                        private PVELog _log;
                        public PVELog Log => _log ?? (_log = new PVELog(_client, _node, _upid));
                        private PVEStatus _status;
                        public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node, _upid));
                        public class PVELog : Base
                        {
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
                            public Result ReadTaskLog(int? limit = null, int? start = null) { return GetRest(limit, start); }
                        }
                        public class PVEStatus : Base
                        {
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
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/tasks/{_upid}/status");
                            }
                            /// <summary>
                            /// Read task status.
                            /// </summary>
                            /// <returns></returns>
                            public Result ReadTaskStatus() { return GetRest(); }
                        }
                        /// <summary>
                        /// Stop a task.
                        /// </summary>
                        /// <returns></returns>
                        public Result DeleteRest()
                        {
                            return _client.Delete($"/nodes/{_node}/tasks/{_upid}");
                        }
                        /// <summary>
                        /// Stop a task.
                        /// </summary>
                        /// <returns></returns>
                        public Result StopTask() { return DeleteRest(); }
                        /// <summary>
                        /// 
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/tasks/{_upid}");
                        }
                        /// <summary>
                        /// 
                        /// </summary>
                        /// <returns></returns>
                        public Result UpidIndex() { return GetRest(); }
                    }
                    /// <summary>
                    /// Read task list for one node (finished tasks).
                    /// </summary>
                    /// <param name="errors"></param>
                    /// <param name="limit"></param>
                    /// <param name="start"></param>
                    /// <param name="userfilter"></param>
                    /// <param name="vmid">Only list tasks for this VM.</param>
                    /// <returns></returns>
                    public Result GetRest(bool? errors = null, int? limit = null, int? start = null, string userfilter = null, int? vmid = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("errors", errors);
                        parameters.Add("limit", limit);
                        parameters.Add("start", start);
                        parameters.Add("userfilter", userfilter);
                        parameters.Add("vmid", vmid);
                        return _client.Get($"/nodes/{_node}/tasks", parameters);
                    }
                    /// <summary>
                    /// Read task list for one node (finished tasks).
                    /// </summary>
                    /// <param name="errors"></param>
                    /// <param name="limit"></param>
                    /// <param name="start"></param>
                    /// <param name="userfilter"></param>
                    /// <param name="vmid">Only list tasks for this VM.</param>
                    /// <returns></returns>
                    public Result NodeTasks(bool? errors = null, int? limit = null, int? start = null, string userfilter = null, int? vmid = null) { return GetRest(errors, limit, start, userfilter, vmid); }
                }
                public class PVEScan : Base
                {
                    private object _node;
                    internal PVEScan(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
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
                    public class PVEZfs : Base
                    {
                        private object _node;
                        internal PVEZfs(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Scan zfs pool list on local node.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/scan/zfs");
                        }
                        /// <summary>
                        /// Scan zfs pool list on local node.
                        /// </summary>
                        /// <returns></returns>
                        public Result Zfsscan() { return GetRest(); }
                    }
                    public class PVENfs : Base
                    {
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
                        /// <param name="server"></param>
                        /// <returns></returns>
                        public Result Nfsscan(string server) { return GetRest(server); }
                    }
                    public class PVECifs : Base
                    {
                        private object _node;
                        internal PVECifs(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Scan remote CIFS server.
                        /// </summary>
                        /// <param name="server"></param>
                        /// <param name="domain"></param>
                        /// <param name="password"></param>
                        /// <param name="username"></param>
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
                        /// <param name="server"></param>
                        /// <param name="domain"></param>
                        /// <param name="password"></param>
                        /// <param name="username"></param>
                        /// <returns></returns>
                        public Result Cifsscan(string server, string domain = null, string password = null, string username = null) { return GetRest(server, domain, password, username); }
                    }
                    public class PVEGlusterfs : Base
                    {
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
                        /// <param name="server"></param>
                        /// <returns></returns>
                        public Result Glusterfsscan(string server) { return GetRest(server); }
                    }
                    public class PVEIscsi : Base
                    {
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
                        /// <param name="portal"></param>
                        /// <returns></returns>
                        public Result Iscsiscan(string portal) { return GetRest(portal); }
                    }
                    public class PVELvm : Base
                    {
                        private object _node;
                        internal PVELvm(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// List local LVM volume groups.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/scan/lvm");
                        }
                        /// <summary>
                        /// List local LVM volume groups.
                        /// </summary>
                        /// <returns></returns>
                        public Result Lvmscan() { return GetRest(); }
                    }
                    public class PVELvmthin : Base
                    {
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
                        public Result Lvmthinscan(string vg) { return GetRest(vg); }
                    }
                    public class PVEUsb : Base
                    {
                        private object _node;
                        internal PVEUsb(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// List local USB devices.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/scan/usb");
                        }
                        /// <summary>
                        /// List local USB devices.
                        /// </summary>
                        /// <returns></returns>
                        public Result Usbscan() { return GetRest(); }
                    }
                    /// <summary>
                    /// Index of available scan methods
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/scan");
                    }
                    /// <summary>
                    /// Index of available scan methods
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
                }
                public class PVEStorage : Base
                {
                    private object _node;
                    internal PVEStorage(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    public PVEItemStorage this[object storage] => new PVEItemStorage(_client, _node, storage);
                    public class PVEItemStorage : Base
                    {
                        private object _node;
                        private object _storage;
                        internal PVEItemStorage(Client client, object node, object storage)
                        {
                            _client = client;
                            _node = node;
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
                        public class PVEContent : Base
                        {
                            private object _node;
                            private object _storage;
                            internal PVEContent(Client client, object node, object storage)
                            {
                                _client = client;
                                _node = node;
                                _storage = storage;
                            }
                            public PVEItemVolume this[object volume] => new PVEItemVolume(_client, _node, _storage, volume);
                            public class PVEItemVolume : Base
                            {
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
                                /// <returns></returns>
                                public Result DeleteRest()
                                {
                                    return _client.Delete($"/nodes/{_node}/storage/{_storage}/content/{_volume}");
                                }
                                /// <summary>
                                /// Delete volume
                                /// </summary>
                                /// <returns></returns>
                                public Result Delete() { return DeleteRest(); }
                                /// <summary>
                                /// Get volume attributes
                                /// </summary>
                                /// <returns></returns>
                                public Result GetRest()
                                {
                                    return _client.Get($"/nodes/{_node}/storage/{_storage}/content/{_volume}");
                                }
                                /// <summary>
                                /// Get volume attributes
                                /// </summary>
                                /// <returns></returns>
                                public Result Info() { return GetRest(); }
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
                                public Result Copy(string target, string target_node = null) { return CreateRest(target, target_node); }
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
                            public Result Index(string content = null, int? vmid = null) { return GetRest(content, vmid); }
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
                            public Result Create(string filename, string size, int vmid, string format = null) { return CreateRest(filename, size, vmid, format); }
                        }
                        public class PVEStatus : Base
                        {
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
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/storage/{_storage}/status");
                            }
                            /// <summary>
                            /// Read storage status.
                            /// </summary>
                            /// <returns></returns>
                            public Result ReadStatus() { return GetRest(); }
                        }
                        public class PVERrd : Base
                        {
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
                            public Result Rrd(string ds, string timeframe, string cf = null) { return GetRest(ds, timeframe, cf); }
                        }
                        public class PVERrddata : Base
                        {
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
                            public Result Rrddata(string timeframe, string cf = null) { return GetRest(timeframe, cf); }
                        }
                        public class PVEUpload : Base
                        {
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
                            /// <param name="tmpfilename">The source file name. This parameter is usually set by the REST handler. You can only overwrite it when connecting to the trustet port on localhost.</param>
                            /// <returns></returns>
                            public Result Upload(string content, string filename, string tmpfilename = null) { return CreateRest(content, filename, tmpfilename); }
                        }
                        /// <summary>
                        /// 
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/storage/{_storage}");
                        }
                        /// <summary>
                        /// 
                        /// </summary>
                        /// <returns></returns>
                        public Result Diridx() { return GetRest(); }
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
                    public Result Index(string content = null, bool? enabled = null, bool? format = null, string storage = null, string target = null) { return GetRest(content, enabled, format, storage, target); }
                }
                public class PVEDisks : Base
                {
                    private object _node;
                    internal PVEDisks(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    private PVEList _list;
                    public PVEList List => _list ?? (_list = new PVEList(_client, _node));
                    private PVESmart _smart;
                    public PVESmart Smart => _smart ?? (_smart = new PVESmart(_client, _node));
                    private PVEInitgpt _initgpt;
                    public PVEInitgpt Initgpt => _initgpt ?? (_initgpt = new PVEInitgpt(_client, _node));
                    public class PVEList : Base
                    {
                        private object _node;
                        internal PVEList(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/disks/list");
                        }
                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        /// <returns></returns>
                        public Result List() { return GetRest(); }
                    }
                    public class PVESmart : Base
                    {
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
                        public Result Smart(string disk, bool? healthonly = null) { return GetRest(disk, healthonly); }
                    }
                    public class PVEInitgpt : Base
                    {
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
                        public Result Initgpt(string disk, string uuid = null) { return CreateRest(disk, uuid); }
                    }
                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/disks");
                    }
                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
                }
                public class PVEApt : Base
                {
                    private object _node;
                    internal PVEApt(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    private PVEUpdate _update;
                    public PVEUpdate Update => _update ?? (_update = new PVEUpdate(_client, _node));
                    private PVEChangelog _changelog;
                    public PVEChangelog Changelog => _changelog ?? (_changelog = new PVEChangelog(_client, _node));
                    private PVEVersions _versions;
                    public PVEVersions Versions => _versions ?? (_versions = new PVEVersions(_client, _node));
                    public class PVEUpdate : Base
                    {
                        private object _node;
                        internal PVEUpdate(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// List available updates.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/apt/update");
                        }
                        /// <summary>
                        /// List available updates.
                        /// </summary>
                        /// <returns></returns>
                        public Result ListUpdates() { return GetRest(); }
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
                        public Result UpdateDatabase(bool? notify = null, bool? quiet = null) { return CreateRest(notify, quiet); }
                    }
                    public class PVEChangelog : Base
                    {
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
                        public Result Changelog(string name, string version = null) { return GetRest(name, version); }
                    }
                    public class PVEVersions : Base
                    {
                        private object _node;
                        internal PVEVersions(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Get package information for important Proxmox packages.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/apt/versions");
                        }
                        /// <summary>
                        /// Get package information for important Proxmox packages.
                        /// </summary>
                        /// <returns></returns>
                        public Result Versions() { return GetRest(); }
                    }
                    /// <summary>
                    /// Directory index for apt (Advanced Package Tool).
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/apt");
                    }
                    /// <summary>
                    /// Directory index for apt (Advanced Package Tool).
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
                }
                public class PVEFirewall : Base
                {
                    private object _node;
                    internal PVEFirewall(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    private PVERules _rules;
                    public PVERules Rules => _rules ?? (_rules = new PVERules(_client, _node));
                    private PVEOptions _options;
                    public PVEOptions Options => _options ?? (_options = new PVEOptions(_client, _node));
                    private PVELog _log;
                    public PVELog Log => _log ?? (_log = new PVELog(_client, _node));
                    public class PVERules : Base
                    {
                        private object _node;
                        internal PVERules(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        public PVEItemPos this[object pos] => new PVEItemPos(_client, _node, pos);
                        public class PVEItemPos : Base
                        {
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
                            public Result DeleteRule(string digest = null) { return DeleteRest(digest); }
                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/firewall/rules/{_pos}");
                            }
                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public Result GetRule() { return GetRest(); }
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
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            /// <returns></returns>
                            public Result SetRest(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
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
                            /// <param name="macro">Use predefined standard macro.</param>
                            /// <param name="moveto">Move rule to new position &amp;lt;moveto&amp;gt;. Other arguments are ignored.</param>
                            /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                            /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="type">Rule type.
                            ///   Enum: in,out,group</param>
                            /// <returns></returns>
                            public Result UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null) { return SetRest(action, comment, delete, dest, digest, dport, enable, iface, macro, moveto, proto, source, sport, type); }
                        }
                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/firewall/rules");
                        }
                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRules() { return GetRest(); }
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
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public Result CreateRest(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
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
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public Result CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string iface = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null) { return CreateRest(action, type, comment, dest, digest, dport, enable, iface, macro, pos, proto, source, sport); }
                    }
                    public class PVEOptions : Base
                    {
                        private object _node;
                        internal PVEOptions(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Get host firewall options.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/firewall/options");
                        }
                        /// <summary>
                        /// Get host firewall options.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetOptions() { return GetRest(); }
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
                        /// <returns></returns>
                        public Result SetRest(string delete = null, string digest = null, bool? enable = null, string log_level_in = null, string log_level_out = null, bool? ndp = null, int? nf_conntrack_max = null, int? nf_conntrack_tcp_timeout_established = null, bool? nosmurfs = null, string smurf_log_level = null, string tcp_flags_log_level = null, bool? tcpflags = null)
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
                        /// <param name="ndp">Enable NDP.</param>
                        /// <param name="nf_conntrack_max">Maximum number of tracked connections.</param>
                        /// <param name="nf_conntrack_tcp_timeout_established">Conntrack established timeout.</param>
                        /// <param name="nosmurfs">Enable SMURFS filter.</param>
                        /// <param name="smurf_log_level">Log level for SMURFS filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcp_flags_log_level">Log level for illegal tcp flags filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcpflags">Filter illegal combinations of TCP flags.</param>
                        /// <returns></returns>
                        public Result SetOptions(string delete = null, string digest = null, bool? enable = null, string log_level_in = null, string log_level_out = null, bool? ndp = null, int? nf_conntrack_max = null, int? nf_conntrack_tcp_timeout_established = null, bool? nosmurfs = null, string smurf_log_level = null, string tcp_flags_log_level = null, bool? tcpflags = null) { return SetRest(delete, digest, enable, log_level_in, log_level_out, ndp, nf_conntrack_max, nf_conntrack_tcp_timeout_established, nosmurfs, smurf_log_level, tcp_flags_log_level, tcpflags); }
                    }
                    public class PVELog : Base
                    {
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
                        public Result Log(int? limit = null, int? start = null) { return GetRest(limit, start); }
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/firewall");
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
                }
                public class PVEReplication : Base
                {
                    private object _node;
                    internal PVEReplication(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    public PVEItemId this[object id] => new PVEItemId(_client, _node, id);
                    public class PVEItemId : Base
                    {
                        private object _node;
                        private object _id;
                        internal PVEItemId(Client client, object node, object id)
                        {
                            _client = client;
                            _node = node;
                            _id = id;
                        }
                        private PVEStatus _status;
                        public PVEStatus Status => _status ?? (_status = new PVEStatus(_client, _node, _id));
                        private PVELog _log;
                        public PVELog Log => _log ?? (_log = new PVELog(_client, _node, _id));
                        private PVEScheduleNow _scheduleNow;
                        public PVEScheduleNow ScheduleNow => _scheduleNow ?? (_scheduleNow = new PVEScheduleNow(_client, _node, _id));
                        public class PVEStatus : Base
                        {
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
                            /// <returns></returns>
                            public Result GetRest()
                            {
                                return _client.Get($"/nodes/{_node}/replication/{_id}/status");
                            }
                            /// <summary>
                            /// Get replication job status.
                            /// </summary>
                            /// <returns></returns>
                            public Result JobStatus() { return GetRest(); }
                        }
                        public class PVELog : Base
                        {
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
                            public Result ReadJobLog(int? limit = null, int? start = null) { return GetRest(limit, start); }
                        }
                        public class PVEScheduleNow : Base
                        {
                            private object _node;
                            private object _id;
                            internal PVEScheduleNow(Client client, object node, object id)
                            {
                                _client = client;
                                _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Schedule replication job to start as soon as possible.
                            /// </summary>
                            /// <returns></returns>
                            public Result CreateRest()
                            {
                                return _client.Create($"/nodes/{_node}/replication/{_id}/schedule_now");
                            }
                            /// <summary>
                            /// Schedule replication job to start as soon as possible.
                            /// </summary>
                            /// <returns></returns>
                            public Result ScheduleNow() { return CreateRest(); }
                        }
                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/replication/{_id}");
                        }
                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() { return GetRest(); }
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
                    public Result Status(int? guest = null) { return GetRest(guest); }
                }
                public class PVECertificates : Base
                {
                    private object _node;
                    internal PVECertificates(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    private PVEAcme _acme;
                    public PVEAcme Acme => _acme ?? (_acme = new PVEAcme(_client, _node));
                    private PVEInfo _info;
                    public PVEInfo Info => _info ?? (_info = new PVEInfo(_client, _node));
                    private PVECustom _custom;
                    public PVECustom Custom => _custom ?? (_custom = new PVECustom(_client, _node));
                    public class PVEAcme : Base
                    {
                        private object _node;
                        internal PVEAcme(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        private PVECertificate _certificate;
                        public PVECertificate Certificate => _certificate ?? (_certificate = new PVECertificate(_client, _node));
                        public class PVECertificate : Base
                        {
                            private object _node;
                            internal PVECertificate(Client client, object node)
                            {
                                _client = client;
                                _node = node;
                            }
                            /// <summary>
                            /// Revoke existing certificate from CA.
                            /// </summary>
                            /// <returns></returns>
                            public Result DeleteRest()
                            {
                                return _client.Delete($"/nodes/{_node}/certificates/acme/certificate");
                            }
                            /// <summary>
                            /// Revoke existing certificate from CA.
                            /// </summary>
                            /// <returns></returns>
                            public Result RevokeCertificate() { return DeleteRest(); }
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
                            public Result NewCertificate(bool? force = null) { return CreateRest(force); }
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
                            public Result RenewCertificate(bool? force = null) { return SetRest(force); }
                        }
                        /// <summary>
                        /// ACME index.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/certificates/acme");
                        }
                        /// <summary>
                        /// ACME index.
                        /// </summary>
                        /// <returns></returns>
                        public Result Index() { return GetRest(); }
                    }
                    public class PVEInfo : Base
                    {
                        private object _node;
                        internal PVEInfo(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
                        /// <summary>
                        /// Get information about node's certificates.
                        /// </summary>
                        /// <returns></returns>
                        public Result GetRest()
                        {
                            return _client.Get($"/nodes/{_node}/certificates/info");
                        }
                        /// <summary>
                        /// Get information about node's certificates.
                        /// </summary>
                        /// <returns></returns>
                        public Result Info() { return GetRest(); }
                    }
                    public class PVECustom : Base
                    {
                        private object _node;
                        internal PVECustom(Client client, object node)
                        {
                            _client = client;
                            _node = node;
                        }
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
                        public Result RemoveCustomCert(bool? restart = null) { return DeleteRest(restart); }
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
                        public Result UploadCustomCert(string certificates, bool? force = null, string key = null, bool? restart = null) { return CreateRest(certificates, force, key, restart); }
                    }
                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/certificates");
                    }
                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public Result Index() { return GetRest(); }
                }
                public class PVEConfig : Base
                {
                    private object _node;
                    internal PVEConfig(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Get node configuration options.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/config");
                    }
                    /// <summary>
                    /// Get node configuration options.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetConfig() { return GetRest(); }
                    /// <summary>
                    /// Set node configuration options.
                    /// </summary>
                    /// <param name="acme">Node specific ACME settings.</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="description">Node description/comment.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <returns></returns>
                    public Result SetRest(string acme = null, string delete = null, string description = null, string digest = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("acme", acme);
                        parameters.Add("delete", delete);
                        parameters.Add("description", description);
                        parameters.Add("digest", digest);
                        return _client.Set($"/nodes/{_node}/config", parameters);
                    }
                    /// <summary>
                    /// Set node configuration options.
                    /// </summary>
                    /// <param name="acme">Node specific ACME settings.</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="description">Node description/comment.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <returns></returns>
                    public Result SetOptions(string acme = null, string delete = null, string description = null, string digest = null) { return SetRest(acme, delete, description, digest); }
                }
                public class PVEVersion : Base
                {
                    private object _node;
                    internal PVEVersion(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// API version details
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/version");
                    }
                    /// <summary>
                    /// API version details
                    /// </summary>
                    /// <returns></returns>
                    public Result Version() { return GetRest(); }
                }
                public class PVEStatus : Base
                {
                    private object _node;
                    internal PVEStatus(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Read node status
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/status");
                    }
                    /// <summary>
                    /// Read node status
                    /// </summary>
                    /// <returns></returns>
                    public Result Status() { return GetRest(); }
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
                    public Result NodeCmd(string command) { return CreateRest(command); }
                }
                public class PVENetstat : Base
                {
                    private object _node;
                    internal PVENetstat(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Read tap/vm network device interface counters
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/netstat");
                    }
                    /// <summary>
                    /// Read tap/vm network device interface counters
                    /// </summary>
                    /// <returns></returns>
                    public Result Netstat() { return GetRest(); }
                }
                public class PVEExecute : Base
                {
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
                    public Result Execute(string commands) { return CreateRest(commands); }
                }
                public class PVERrd : Base
                {
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
                    public Result Rrd(string ds, string timeframe, string cf = null) { return GetRest(ds, timeframe, cf); }
                }
                public class PVERrddata : Base
                {
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
                    public Result Rrddata(string timeframe, string cf = null) { return GetRest(timeframe, cf); }
                }
                public class PVESyslog : Base
                {
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
                    public Result Syslog(int? limit = null, string service = null, string since = null, int? start = null, string until = null) { return GetRest(limit, service, since, start, until); }
                }
                public class PVEVncshell : Base
                {
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
                    /// <returns></returns>
                    public Result CreateRest(int? height = null, bool? upgrade = null, bool? websocket = null, int? width = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("height", height);
                        parameters.Add("upgrade", upgrade);
                        parameters.Add("websocket", websocket);
                        parameters.Add("width", width);
                        return _client.Create($"/nodes/{_node}/vncshell", parameters);
                    }
                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="height">sets the height of the console in pixels.</param>
                    /// <param name="upgrade">Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <param name="websocket">use websocket instead of standard vnc.</param>
                    /// <param name="width">sets the width of the console in pixels.</param>
                    /// <returns></returns>
                    public Result Vncshell(int? height = null, bool? upgrade = null, bool? websocket = null, int? width = null) { return CreateRest(height, upgrade, websocket, width); }
                }
                public class PVETermproxy : Base
                {
                    private object _node;
                    internal PVETermproxy(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="upgrade">Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <returns></returns>
                    public Result CreateRest(bool? upgrade = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("upgrade", upgrade);
                        return _client.Create($"/nodes/{_node}/termproxy", parameters);
                    }
                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="upgrade">Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <returns></returns>
                    public Result Termproxy(bool? upgrade = null) { return CreateRest(upgrade); }
                }
                public class PVEVncwebsocket : Base
                {
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
                    public Result Vncwebsocket(int port, string vncticket) { return GetRest(port, vncticket); }
                }
                public class PVESpiceshell : Base
                {
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
                    /// <returns></returns>
                    public Result CreateRest(string proxy = null, bool? upgrade = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("proxy", proxy);
                        parameters.Add("upgrade", upgrade);
                        return _client.Create($"/nodes/{_node}/spiceshell", parameters);
                    }
                    /// <summary>
                    /// Creates a SPICE shell.
                    /// </summary>
                    /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As resonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                    /// <param name="upgrade">Run 'apt-get dist-upgrade' instead of normal shell.</param>
                    /// <returns></returns>
                    public Result Spiceshell(string proxy = null, bool? upgrade = null) { return CreateRest(proxy, upgrade); }
                }
                public class PVEDns : Base
                {
                    private object _node;
                    internal PVEDns(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Read DNS settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/dns");
                    }
                    /// <summary>
                    /// Read DNS settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result Dns() { return GetRest(); }
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
                    public Result UpdateDns(string search, string dns1 = null, string dns2 = null, string dns3 = null) { return SetRest(search, dns1, dns2, dns3); }
                }
                public class PVETime : Base
                {
                    private object _node;
                    internal PVETime(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Read server time and time zone settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/time");
                    }
                    /// <summary>
                    /// Read server time and time zone settings.
                    /// </summary>
                    /// <returns></returns>
                    public Result Time() { return GetRest(); }
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
                    public Result SetTimezone(string timezone) { return SetRest(timezone); }
                }
                public class PVEAplinfo : Base
                {
                    private object _node;
                    internal PVEAplinfo(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Get list of appliances.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/aplinfo");
                    }
                    /// <summary>
                    /// Get list of appliances.
                    /// </summary>
                    /// <returns></returns>
                    public Result Aplinfo() { return GetRest(); }
                    /// <summary>
                    /// Download appliance templates.
                    /// </summary>
                    /// <param name="storage">The storage where the template will be stored</param>
                    /// <param name="template">The template wich will downloaded</param>
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
                    /// <param name="template">The template wich will downloaded</param>
                    /// <returns></returns>
                    public Result AplDownload(string storage, string template) { return CreateRest(storage, template); }
                }
                public class PVEReport : Base
                {
                    private object _node;
                    internal PVEReport(Client client, object node)
                    {
                        _client = client;
                        _node = node;
                    }
                    /// <summary>
                    /// Gather various systems information about a node
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/nodes/{_node}/report");
                    }
                    /// <summary>
                    /// Gather various systems information about a node
                    /// </summary>
                    /// <returns></returns>
                    public Result Report() { return GetRest(); }
                }
                public class PVEStartall : Base
                {
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
                    public Result Startall(bool? force = null, string vms = null) { return CreateRest(force, vms); }
                }
                public class PVEStopall : Base
                {
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
                    public Result Stopall(string vms = null) { return CreateRest(vms); }
                }
                public class PVEMigrateall : Base
                {
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
                    public Result Migrateall(string target, int? maxworkers = null, string vms = null) { return CreateRest(target, maxworkers, vms); }
                }
                /// <summary>
                /// Node index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/nodes/{_node}");
                }
                /// <summary>
                /// Node index.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
            }
            /// <summary>
            /// Cluster node index.
            /// </summary>
            /// <returns></returns>
            public Result GetRest()
            {
                return _client.Get($"/nodes");
            }
            /// <summary>
            /// Cluster node index.
            /// </summary>
            /// <returns></returns>
            public Result Index() { return GetRest(); }
        }
        public class PVEStorage : Base
        {
            internal PVEStorage(Client client)
            {
                _client = client;
            }
            public PVEItemStorage this[object storage] => new PVEItemStorage(_client, storage);
            public class PVEItemStorage : Base
            {
                private object _storage;
                internal PVEItemStorage(Client client, object storage)
                {
                    _client = client;
                    _storage = storage;
                }
                /// <summary>
                /// Delete storage configuration.
                /// </summary>
                /// <returns></returns>
                public Result DeleteRest()
                {
                    return _client.Delete($"/storage/{_storage}");
                }
                /// <summary>
                /// Delete storage configuration.
                /// </summary>
                /// <returns></returns>
                public Result Delete() { return DeleteRest(); }
                /// <summary>
                /// Read storage configuration.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/storage/{_storage}");
                }
                /// <summary>
                /// Read storage configuration.
                /// </summary>
                /// <returns></returns>
                public Result Read() { return GetRest(); }
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
                /// <param name="krbd">Access rbd through krbd kernel module.</param>
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
                public Result SetRest(string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, string delete = null, string digest = null, bool? disable = null, string domain = null, string format = null, bool? fuse = null, string is_mountpoint = null, bool? krbd = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string pool = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string transport = null, string username = null)
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
                /// <param name="krbd">Access rbd through krbd kernel module.</param>
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
                public Result Update(string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, string delete = null, string digest = null, bool? disable = null, string domain = null, string format = null, bool? fuse = null, string is_mountpoint = null, bool? krbd = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string pool = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string transport = null, string username = null) { return SetRest(blocksize, bwlimit, comstar_hg, comstar_tg, content, delete, digest, disable, domain, format, fuse, is_mountpoint, krbd, maxfiles, mkdir, monhost, nodes, nowritecache, options, password, pool, redundancy, saferemove, saferemove_throughput, server, server2, shared, smbversion, sparse, subdir, tagged_only, transport, username); }
            }
            /// <summary>
            /// Storage index.
            /// </summary>
            /// <param name="type">Only list storage of specific type
            ///   Enum: cephfs,cifs,dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,sheepdog,zfs,zfspool</param>
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
            ///   Enum: cephfs,cifs,dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,sheepdog,zfs,zfspool</param>
            /// <returns></returns>
            public Result Index(string type = null) { return GetRest(type); }
            /// <summary>
            /// Create a new storage.
            /// </summary>
            /// <param name="storage">The storage identifier.</param>
            /// <param name="type">Storage type.
            ///   Enum: cephfs,cifs,dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,sheepdog,zfs,zfspool</param>
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
            /// <param name="krbd">Access rbd through krbd kernel module.</param>
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
            public Result CreateRest(string storage, string type, string authsupported = null, string base_ = null, string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, bool? disable = null, string domain = null, string export = null, string format = null, bool? fuse = null, string is_mountpoint = null, string iscsiprovider = null, bool? krbd = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string path = null, string pool = null, string portal = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, string share = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string target = null, string thinpool = null, string transport = null, string username = null, string vgname = null, string volume = null)
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
            ///   Enum: cephfs,cifs,dir,drbd,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,rbd,sheepdog,zfs,zfspool</param>
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
            /// <param name="krbd">Access rbd through krbd kernel module.</param>
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
            public Result Create(string storage, string type, string authsupported = null, string base_ = null, string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, bool? disable = null, string domain = null, string export = null, string format = null, bool? fuse = null, string is_mountpoint = null, string iscsiprovider = null, bool? krbd = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string path = null, string pool = null, string portal = null, int? redundancy = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, string share = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string target = null, string thinpool = null, string transport = null, string username = null, string vgname = null, string volume = null) { return CreateRest(storage, type, authsupported, base_, blocksize, bwlimit, comstar_hg, comstar_tg, content, disable, domain, export, format, fuse, is_mountpoint, iscsiprovider, krbd, maxfiles, mkdir, monhost, nodes, nowritecache, options, password, path, pool, portal, redundancy, saferemove, saferemove_throughput, server, server2, share, shared, smbversion, sparse, subdir, tagged_only, target, thinpool, transport, username, vgname, volume); }
        }
        public class PVEAccess : Base
        {
            internal PVEAccess(Client client)
            {
                _client = client;
            }
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
            public class PVEUsers : Base
            {
                internal PVEUsers(Client client)
                {
                    _client = client;
                }
                public PVEItemUserid this[object userid] => new PVEItemUserid(_client, userid);
                public class PVEItemUserid : Base
                {
                    private object _userid;
                    internal PVEItemUserid(Client client, object userid)
                    {
                        _client = client;
                        _userid = userid;
                    }
                    /// <summary>
                    /// Delete user.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest()
                    {
                        return _client.Delete($"/access/users/{_userid}");
                    }
                    /// <summary>
                    /// Delete user.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteUser() { return DeleteRest(); }
                    /// <summary>
                    /// Get user configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/access/users/{_userid}");
                    }
                    /// <summary>
                    /// Get user configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result ReadUser() { return GetRest(); }
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
                    /// <param name="enable">Enable/disable the account.</param>
                    /// <param name="expire">Account expiration date (seconds since epoch). '0' means no expiration date.</param>
                    /// <param name="firstname"></param>
                    /// <param name="groups"></param>
                    /// <param name="keys">Keys for two factor auth (yubico).</param>
                    /// <param name="lastname"></param>
                    /// <returns></returns>
                    public Result UpdateUser(bool? append = null, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null) { return SetRest(append, comment, email, enable, expire, firstname, groups, keys, lastname); }
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
                public Result Index(bool? enabled = null) { return GetRest(enabled); }
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
                /// <param name="enable">Enable the account (default). You can set this to '0' to disable the accout</param>
                /// <param name="expire">Account expiration date (seconds since epoch). '0' means no expiration date.</param>
                /// <param name="firstname"></param>
                /// <param name="groups"></param>
                /// <param name="keys">Keys for two factor auth (yubico).</param>
                /// <param name="lastname"></param>
                /// <param name="password">Initial password.</param>
                /// <returns></returns>
                public Result CreateUser(string userid, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null, string password = null) { return CreateRest(userid, comment, email, enable, expire, firstname, groups, keys, lastname, password); }
            }
            public class PVEGroups : Base
            {
                internal PVEGroups(Client client)
                {
                    _client = client;
                }
                public PVEItemGroupid this[object groupid] => new PVEItemGroupid(_client, groupid);
                public class PVEItemGroupid : Base
                {
                    private object _groupid;
                    internal PVEItemGroupid(Client client, object groupid)
                    {
                        _client = client;
                        _groupid = groupid;
                    }
                    /// <summary>
                    /// Delete group.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest()
                    {
                        return _client.Delete($"/access/groups/{_groupid}");
                    }
                    /// <summary>
                    /// Delete group.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteGroup() { return DeleteRest(); }
                    /// <summary>
                    /// Get group configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/access/groups/{_groupid}");
                    }
                    /// <summary>
                    /// Get group configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result ReadGroup() { return GetRest(); }
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
                    public Result UpdateGroup(string comment = null) { return SetRest(comment); }
                }
                /// <summary>
                /// Group index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/access/groups");
                }
                /// <summary>
                /// Group index.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
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
                public Result CreateGroup(string groupid, string comment = null) { return CreateRest(groupid, comment); }
            }
            public class PVERoles : Base
            {
                internal PVERoles(Client client)
                {
                    _client = client;
                }
                public PVEItemRoleid this[object roleid] => new PVEItemRoleid(_client, roleid);
                public class PVEItemRoleid : Base
                {
                    private object _roleid;
                    internal PVEItemRoleid(Client client, object roleid)
                    {
                        _client = client;
                        _roleid = roleid;
                    }
                    /// <summary>
                    /// Delete role.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest()
                    {
                        return _client.Delete($"/access/roles/{_roleid}");
                    }
                    /// <summary>
                    /// Delete role.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRole() { return DeleteRest(); }
                    /// <summary>
                    /// Get role configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/access/roles/{_roleid}");
                    }
                    /// <summary>
                    /// Get role configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result ReadRole() { return GetRest(); }
                    /// <summary>
                    /// Create new role.
                    /// </summary>
                    /// <param name="privs"></param>
                    /// <param name="append"></param>
                    /// <returns></returns>
                    public Result SetRest(string privs, bool? append = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("privs", privs);
                        parameters.Add("append", append);
                        return _client.Set($"/access/roles/{_roleid}", parameters);
                    }
                    /// <summary>
                    /// Create new role.
                    /// </summary>
                    /// <param name="privs"></param>
                    /// <param name="append"></param>
                    /// <returns></returns>
                    public Result UpdateRole(string privs, bool? append = null) { return SetRest(privs, append); }
                }
                /// <summary>
                /// Role index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/access/roles");
                }
                /// <summary>
                /// Role index.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
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
                public Result CreateRole(string roleid, string privs = null) { return CreateRest(roleid, privs); }
            }
            public class PVEAcl : Base
            {
                internal PVEAcl(Client client)
                {
                    _client = client;
                }
                /// <summary>
                /// Get Access Control List (ACLs).
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/access/acl");
                }
                /// <summary>
                /// Get Access Control List (ACLs).
                /// </summary>
                /// <returns></returns>
                public Result ReadAcl() { return GetRest(); }
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
                public Result UpdateAcl(string path, string roles, bool? delete = null, string groups = null, bool? propagate = null, string users = null) { return SetRest(path, roles, delete, groups, propagate, users); }
            }
            public class PVEDomains : Base
            {
                internal PVEDomains(Client client)
                {
                    _client = client;
                }
                public PVEItemRealm this[object realm] => new PVEItemRealm(_client, realm);
                public class PVEItemRealm : Base
                {
                    private object _realm;
                    internal PVEItemRealm(Client client, object realm)
                    {
                        _client = client;
                        _realm = realm;
                    }
                    /// <summary>
                    /// Delete an authentication server.
                    /// </summary>
                    /// <returns></returns>
                    public Result DeleteRest()
                    {
                        return _client.Delete($"/access/domains/{_realm}");
                    }
                    /// <summary>
                    /// Delete an authentication server.
                    /// </summary>
                    /// <returns></returns>
                    public Result Delete() { return DeleteRest(); }
                    /// <summary>
                    /// Get auth server configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result GetRest()
                    {
                        return _client.Get($"/access/domains/{_realm}");
                    }
                    /// <summary>
                    /// Get auth server configuration.
                    /// </summary>
                    /// <returns></returns>
                    public Result Read() { return GetRest(); }
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
                    public Result Update(string base_dn = null, string bind_dn = null, string capath = null, string cert = null, string certkey = null, string comment = null, bool? default_ = null, string delete = null, string digest = null, string domain = null, int? port = null, bool? secure = null, string server1 = null, string server2 = null, string tfa = null, string user_attr = null, bool? verify = null) { return SetRest(base_dn, bind_dn, capath, cert, certkey, comment, default_, delete, digest, domain, port, secure, server1, server2, tfa, user_attr, verify); }
                }
                /// <summary>
                /// Authentication domain index.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/access/domains");
                }
                /// <summary>
                /// Authentication domain index.
                /// </summary>
                /// <returns></returns>
                public Result Index() { return GetRest(); }
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
                public Result Create(string realm, string type, string base_dn = null, string bind_dn = null, string capath = null, string cert = null, string certkey = null, string comment = null, bool? default_ = null, string domain = null, int? port = null, bool? secure = null, string server1 = null, string server2 = null, string tfa = null, string user_attr = null, bool? verify = null) { return CreateRest(realm, type, base_dn, bind_dn, capath, cert, certkey, comment, default_, domain, port, secure, server1, server2, tfa, user_attr, verify); }
            }
            public class PVETicket : Base
            {
                internal PVETicket(Client client)
                {
                    _client = client;
                }
                /// <summary>
                /// Dummy. Useful for formatters which want to provide a login page.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/access/ticket");
                }
                /// <summary>
                /// Dummy. Useful for formatters which want to provide a login page.
                /// </summary>
                /// <returns></returns>
                public Result GetTicket() { return GetRest(); }
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
                public Result CreateTicket(string password, string username, string otp = null, string path = null, string privs = null, string realm = null) { return CreateRest(password, username, otp, path, privs, realm); }
            }
            public class PVEPassword : Base
            {
                internal PVEPassword(Client client)
                {
                    _client = client;
                }
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
                public Result ChangePasssword(string password, string userid) { return SetRest(password, userid); }
            }
            /// <summary>
            /// Directory index.
            /// </summary>
            /// <returns></returns>
            public Result GetRest()
            {
                return _client.Get($"/access");
            }
            /// <summary>
            /// Directory index.
            /// </summary>
            /// <returns></returns>
            public Result Index() { return GetRest(); }
        }
        public class PVEPools : Base
        {
            internal PVEPools(Client client)
            {
                _client = client;
            }
            public PVEItemPoolid this[object poolid] => new PVEItemPoolid(_client, poolid);
            public class PVEItemPoolid : Base
            {
                private object _poolid;
                internal PVEItemPoolid(Client client, object poolid)
                {
                    _client = client;
                    _poolid = poolid;
                }
                /// <summary>
                /// Delete pool.
                /// </summary>
                /// <returns></returns>
                public Result DeleteRest()
                {
                    return _client.Delete($"/pools/{_poolid}");
                }
                /// <summary>
                /// Delete pool.
                /// </summary>
                /// <returns></returns>
                public Result DeletePool() { return DeleteRest(); }
                /// <summary>
                /// Get pool configuration.
                /// </summary>
                /// <returns></returns>
                public Result GetRest()
                {
                    return _client.Get($"/pools/{_poolid}");
                }
                /// <summary>
                /// Get pool configuration.
                /// </summary>
                /// <returns></returns>
                public Result ReadPool() { return GetRest(); }
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
                public Result UpdatePool(string comment = null, bool? delete = null, string storage = null, string vms = null) { return SetRest(comment, delete, storage, vms); }
            }
            /// <summary>
            /// Pool index.
            /// </summary>
            /// <returns></returns>
            public Result GetRest()
            {
                return _client.Get($"/pools");
            }
            /// <summary>
            /// Pool index.
            /// </summary>
            /// <returns></returns>
            public Result Index() { return GetRest(); }
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
            public Result CreatePool(string poolid, string comment = null) { return CreateRest(poolid, comment); }
        }
        public class PVEVersion : Base
        {
            internal PVEVersion(Client client)
            {
                _client = client;
            }
            /// <summary>
            /// API version details. The result also includes the global datacenter confguration.
            /// </summary>
            /// <returns></returns>
            public Result GetRest()
            {
                return _client.Get($"/version");
            }
            /// <summary>
            /// API version details. The result also includes the global datacenter confguration.
            /// </summary>
            /// <returns></returns>
            public Result Version() { return GetRest(); }
        }
    }
}