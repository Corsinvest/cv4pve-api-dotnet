/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api
{
    /// <summary>
    /// Proxmox VE Client
    /// </summary>
    public class PveClient : PveClientBase
    {
#pragma warning disable 1591
        private readonly PveClient _client;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public PveClient(string host, int port = 8006) : base(host, port) => _client = this;

        private PveCluster _cluster;
        /// <summary>
        /// Cluster
        /// </summary>
        public PveCluster Cluster => _cluster ??= new(_client);
        private PveNodes _nodes;
        /// <summary>
        /// Nodes
        /// </summary>
        public PveNodes Nodes => _nodes ??= new(_client);
        private PveStorage _storage;
        /// <summary>
        /// Storage
        /// </summary>
        public PveStorage Storage => _storage ??= new(_client);
        private PveAccess _access;
        /// <summary>
        /// Access
        /// </summary>
        public PveAccess Access => _access ??= new(_client);
        private PvePools _pools;
        /// <summary>
        /// Pools
        /// </summary>
        public PvePools Pools => _pools ??= new(_client);
        private PveVersion _version;
        /// <summary>
        /// Version
        /// </summary>
        public PveVersion Version => _version ??= new(_client);
        /// <summary>
        /// Cluster
        /// </summary>
        public class PveCluster
        {
            private readonly PveClient _client;

            internal PveCluster(PveClient client) { _client = client; }
            private PveReplication _replication;
            /// <summary>
            /// Replication
            /// </summary>
            public PveReplication Replication => _replication ??= new(_client);
            private PveMetrics _metrics;
            /// <summary>
            /// Metrics
            /// </summary>
            public PveMetrics Metrics => _metrics ??= new(_client);
            private PveNotifications _notifications;
            /// <summary>
            /// Notifications
            /// </summary>
            public PveNotifications Notifications => _notifications ??= new(_client);
            private PveConfig _config;
            /// <summary>
            /// Config
            /// </summary>
            public PveConfig Config => _config ??= new(_client);
            private PveFirewall _firewall;
            /// <summary>
            /// Firewall
            /// </summary>
            public PveFirewall Firewall => _firewall ??= new(_client);
            private PveBackup _backup;
            /// <summary>
            /// Backup
            /// </summary>
            public PveBackup Backup => _backup ??= new(_client);
            private PveBackupInfo _backupInfo;
            /// <summary>
            /// BackupInfo
            /// </summary>
            public PveBackupInfo BackupInfo => _backupInfo ??= new(_client);
            private PveHa _ha;
            /// <summary>
            /// Ha
            /// </summary>
            public PveHa Ha => _ha ??= new(_client);
            private PveAcme _acme;
            /// <summary>
            /// Acme
            /// </summary>
            public PveAcme Acme => _acme ??= new(_client);
            private PveCeph _ceph;
            /// <summary>
            /// Ceph
            /// </summary>
            public PveCeph Ceph => _ceph ??= new(_client);
            private PveJobs _jobs;
            /// <summary>
            /// Jobs
            /// </summary>
            public PveJobs Jobs => _jobs ??= new(_client);
            private PveMapping _mapping;
            /// <summary>
            /// Mapping
            /// </summary>
            public PveMapping Mapping => _mapping ??= new(_client);
            private PveSdn _sdn;
            /// <summary>
            /// Sdn
            /// </summary>
            public PveSdn Sdn => _sdn ??= new(_client);
            private PveLog _log;
            /// <summary>
            /// Log
            /// </summary>
            public PveLog Log => _log ??= new(_client);
            private PveResources _resources;
            /// <summary>
            /// Resources
            /// </summary>
            public PveResources Resources => _resources ??= new(_client);
            private PveTasks _tasks;
            /// <summary>
            /// Tasks
            /// </summary>
            public PveTasks Tasks => _tasks ??= new(_client);
            private PveOptions _options;
            /// <summary>
            /// Options
            /// </summary>
            public PveOptions Options => _options ??= new(_client);
            private PveStatus _status;
            /// <summary>
            /// Status
            /// </summary>
            public PveStatus Status => _status ??= new(_client);
            private PveNextid _nextid;
            /// <summary>
            /// Nextid
            /// </summary>
            public PveNextid Nextid => _nextid ??= new(_client);
            /// <summary>
            /// Replication
            /// </summary>
            public class PveReplication
            {
                private readonly PveClient _client;

                internal PveReplication(PveClient client) { _client = client; }
                /// <summary>
                /// IdItem
                /// </summary>
                public PveIdItem this[object id] => new(_client, id);
                /// <summary>
                /// IdItem
                /// </summary>
                public class PveIdItem
                {
                    private readonly PveClient _client;
                    private readonly object _id;
                    internal PveIdItem(PveClient client, object id) { _client = client; _id = id; }
                    /// <summary>
                    /// Mark replication job for removal.
                    /// </summary>
                    /// <param name="force">Will remove the jobconfig entry, but will not cleanup.</param>
                    /// <param name="keep">Keep replicated data at target (do not remove).</param>
                    /// <returns></returns>
                    public async Task<Result> Delete(bool? force = null, bool? keep = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        parameters.Add("keep", keep);
                        return await _client.Delete($"/cluster/replication/{_id}", parameters);
                    }
                    /// <summary>
                    /// Read replication job configuration.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Read() { return await _client.Get($"/cluster/replication/{_id}"); }
                    /// <summary>
                    /// Update replication job configuration.
                    /// </summary>
                    /// <param name="comment">Description.</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="disable">Flag to disable/deactivate the entry.</param>
                    /// <param name="rate">Rate limit in mbps (megabytes per second) as floating point number.</param>
                    /// <param name="remove_job">Mark the replication job for removal. The job will remove all local replication snapshots. When set to 'full', it also tries to remove replicated volumes on the target. The job then removes itself from the configuration file.
                    ///   Enum: local,full</param>
                    /// <param name="schedule">Storage replication schedule. The format is a subset of `systemd` calendar events.</param>
                    /// <param name="source">For internal use, to detect if the guest was stolen.</param>
                    /// <returns></returns>
                    public async Task<Result> Update(string comment = null, string delete = null, string digest = null, bool? disable = null, float? rate = null, string remove_job = null, string schedule = null, string source = null)
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
                        return await _client.Set($"/cluster/replication/{_id}", parameters);
                    }
                }
                /// <summary>
                /// List replication jobs.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/replication"); }
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
                /// <param name="schedule">Storage replication schedule. The format is a subset of `systemd` calendar events.</param>
                /// <param name="source">For internal use, to detect if the guest was stolen.</param>
                /// <returns></returns>
                public async Task<Result> Create(string id, string target, string type, string comment = null, bool? disable = null, float? rate = null, string remove_job = null, string schedule = null, string source = null)
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
                    return await _client.Create($"/cluster/replication", parameters);
                }
            }
            /// <summary>
            /// Metrics
            /// </summary>
            public class PveMetrics
            {
                private readonly PveClient _client;

                internal PveMetrics(PveClient client) { _client = client; }
                private PveServer _server;
                /// <summary>
                /// Server
                /// </summary>
                public PveServer Server => _server ??= new(_client);
                /// <summary>
                /// Server
                /// </summary>
                public class PveServer
                {
                    private readonly PveClient _client;

                    internal PveServer(PveClient client) { _client = client; }
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public PveIdItem this[object id] => new(_client, id);
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public class PveIdItem
                    {
                        private readonly PveClient _client;
                        private readonly object _id;
                        internal PveIdItem(PveClient client, object id) { _client = client; _id = id; }
                        /// <summary>
                        /// Remove Metric server.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/metrics/server/{_id}"); }
                        /// <summary>
                        /// Read metric server configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Read() { return await _client.Get($"/cluster/metrics/server/{_id}"); }
                        /// <summary>
                        /// Create a new external metric server config
                        /// </summary>
                        /// <param name="port">server network port</param>
                        /// <param name="server">server dns name or IP address</param>
                        /// <param name="type">Plugin type.
                        ///   Enum: graphite,influxdb</param>
                        /// <param name="api_path_prefix">An API path prefix inserted between '&amp;lt;host&amp;gt;:&amp;lt;port&amp;gt;/' and '/api2/'. Can be useful if the InfluxDB service runs behind a reverse proxy.</param>
                        /// <param name="bucket">The InfluxDB bucket/db. Only necessary when using the http v2 api.</param>
                        /// <param name="disable">Flag to disable the plugin.</param>
                        /// <param name="influxdbproto">
                        ///   Enum: udp,http,https</param>
                        /// <param name="max_body_size">InfluxDB max-body-size in bytes. Requests are batched up to this size.</param>
                        /// <param name="mtu">MTU for metrics transmission over UDP</param>
                        /// <param name="organization">The InfluxDB organization. Only necessary when using the http v2 api. Has no meaning when using v2 compatibility api.</param>
                        /// <param name="path">root graphite path (ex: proxmox.mycluster.mykey)</param>
                        /// <param name="proto">Protocol to send graphite data. TCP or UDP (default)
                        ///   Enum: udp,tcp</param>
                        /// <param name="timeout">graphite TCP socket timeout (default=1)</param>
                        /// <param name="token">The InfluxDB access token. Only necessary when using the http v2 api. If the v2 compatibility api is used, use 'user:password' instead.</param>
                        /// <param name="verify_certificate">Set to 0 to disable certificate verification for https endpoints.</param>
                        /// <returns></returns>
                        public async Task<Result> Create(int port, string server, string type, string api_path_prefix = null, string bucket = null, bool? disable = null, string influxdbproto = null, int? max_body_size = null, int? mtu = null, string organization = null, string path = null, string proto = null, int? timeout = null, string token = null, bool? verify_certificate = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("port", port);
                            parameters.Add("server", server);
                            parameters.Add("type", type);
                            parameters.Add("api-path-prefix", api_path_prefix);
                            parameters.Add("bucket", bucket);
                            parameters.Add("disable", disable);
                            parameters.Add("influxdbproto", influxdbproto);
                            parameters.Add("max-body-size", max_body_size);
                            parameters.Add("mtu", mtu);
                            parameters.Add("organization", organization);
                            parameters.Add("path", path);
                            parameters.Add("proto", proto);
                            parameters.Add("timeout", timeout);
                            parameters.Add("token", token);
                            parameters.Add("verify-certificate", verify_certificate);
                            return await _client.Create($"/cluster/metrics/server/{_id}", parameters);
                        }
                        /// <summary>
                        /// Update metric server configuration.
                        /// </summary>
                        /// <param name="port">server network port</param>
                        /// <param name="server">server dns name or IP address</param>
                        /// <param name="api_path_prefix">An API path prefix inserted between '&amp;lt;host&amp;gt;:&amp;lt;port&amp;gt;/' and '/api2/'. Can be useful if the InfluxDB service runs behind a reverse proxy.</param>
                        /// <param name="bucket">The InfluxDB bucket/db. Only necessary when using the http v2 api.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="disable">Flag to disable the plugin.</param>
                        /// <param name="influxdbproto">
                        ///   Enum: udp,http,https</param>
                        /// <param name="max_body_size">InfluxDB max-body-size in bytes. Requests are batched up to this size.</param>
                        /// <param name="mtu">MTU for metrics transmission over UDP</param>
                        /// <param name="organization">The InfluxDB organization. Only necessary when using the http v2 api. Has no meaning when using v2 compatibility api.</param>
                        /// <param name="path">root graphite path (ex: proxmox.mycluster.mykey)</param>
                        /// <param name="proto">Protocol to send graphite data. TCP or UDP (default)
                        ///   Enum: udp,tcp</param>
                        /// <param name="timeout">graphite TCP socket timeout (default=1)</param>
                        /// <param name="token">The InfluxDB access token. Only necessary when using the http v2 api. If the v2 compatibility api is used, use 'user:password' instead.</param>
                        /// <param name="verify_certificate">Set to 0 to disable certificate verification for https endpoints.</param>
                        /// <returns></returns>
                        public async Task<Result> Update(int port, string server, string api_path_prefix = null, string bucket = null, string delete = null, string digest = null, bool? disable = null, string influxdbproto = null, int? max_body_size = null, int? mtu = null, string organization = null, string path = null, string proto = null, int? timeout = null, string token = null, bool? verify_certificate = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("port", port);
                            parameters.Add("server", server);
                            parameters.Add("api-path-prefix", api_path_prefix);
                            parameters.Add("bucket", bucket);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("disable", disable);
                            parameters.Add("influxdbproto", influxdbproto);
                            parameters.Add("max-body-size", max_body_size);
                            parameters.Add("mtu", mtu);
                            parameters.Add("organization", organization);
                            parameters.Add("path", path);
                            parameters.Add("proto", proto);
                            parameters.Add("timeout", timeout);
                            parameters.Add("token", token);
                            parameters.Add("verify-certificate", verify_certificate);
                            return await _client.Set($"/cluster/metrics/server/{_id}", parameters);
                        }
                    }
                    /// <summary>
                    /// List configured metric servers.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> ServerIndex() { return await _client.Get($"/cluster/metrics/server"); }
                }
                /// <summary>
                /// Metrics index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/metrics"); }
            }
            /// <summary>
            /// Notifications
            /// </summary>
            public class PveNotifications
            {
                private readonly PveClient _client;

                internal PveNotifications(PveClient client) { _client = client; }
                private PveEndpoints _endpoints;
                /// <summary>
                /// Endpoints
                /// </summary>
                public PveEndpoints Endpoints => _endpoints ??= new(_client);
                private PveTargets _targets;
                /// <summary>
                /// Targets
                /// </summary>
                public PveTargets Targets => _targets ??= new(_client);
                private PveMatchers _matchers;
                /// <summary>
                /// Matchers
                /// </summary>
                public PveMatchers Matchers => _matchers ??= new(_client);
                /// <summary>
                /// Endpoints
                /// </summary>
                public class PveEndpoints
                {
                    private readonly PveClient _client;

                    internal PveEndpoints(PveClient client) { _client = client; }
                    private PveSendmail _sendmail;
                    /// <summary>
                    /// Sendmail
                    /// </summary>
                    public PveSendmail Sendmail => _sendmail ??= new(_client);
                    private PveGotify _gotify;
                    /// <summary>
                    /// Gotify
                    /// </summary>
                    public PveGotify Gotify => _gotify ??= new(_client);
                    private PveSmtp _smtp;
                    /// <summary>
                    /// Smtp
                    /// </summary>
                    public PveSmtp Smtp => _smtp ??= new(_client);
                    /// <summary>
                    /// Sendmail
                    /// </summary>
                    public class PveSendmail
                    {
                        private readonly PveClient _client;

                        internal PveSendmail(PveClient client) { _client = client; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object name) { _client = client; _name = name; }
                            /// <summary>
                            /// Remove sendmail endpoint
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> DeleteSendmailEndpoint() { return await _client.Delete($"/cluster/notifications/endpoints/sendmail/{_name}"); }
                            /// <summary>
                            /// Return a specific sendmail endpoint
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> GetSendmailEndpoint() { return await _client.Get($"/cluster/notifications/endpoints/sendmail/{_name}"); }
                            /// <summary>
                            /// Update existing sendmail endpoint
                            /// </summary>
                            /// <param name="author">Author of the mail</param>
                            /// <param name="comment">Comment</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="disable">Disable this target</param>
                            /// <param name="from_address">`From` address for the mail</param>
                            /// <param name="mailto">List of email recipients</param>
                            /// <param name="mailto_user">List of users</param>
                            /// <returns></returns>
                            public async Task<Result> UpdateSendmailEndpoint(string author = null, string comment = null, string delete = null, string digest = null, bool? disable = null, string from_address = null, string mailto = null, string mailto_user = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("author", author);
                                parameters.Add("comment", comment);
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                parameters.Add("disable", disable);
                                parameters.Add("from-address", from_address);
                                parameters.Add("mailto", mailto);
                                parameters.Add("mailto-user", mailto_user);
                                return await _client.Set($"/cluster/notifications/endpoints/sendmail/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// Returns a list of all sendmail endpoints
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetSendmailEndpoints() { return await _client.Get($"/cluster/notifications/endpoints/sendmail"); }
                        /// <summary>
                        /// Create a new sendmail endpoint
                        /// </summary>
                        /// <param name="name">The name of the endpoint.</param>
                        /// <param name="author">Author of the mail</param>
                        /// <param name="comment">Comment</param>
                        /// <param name="disable">Disable this target</param>
                        /// <param name="from_address">`From` address for the mail</param>
                        /// <param name="mailto">List of email recipients</param>
                        /// <param name="mailto_user">List of users</param>
                        /// <returns></returns>
                        public async Task<Result> CreateSendmailEndpoint(string name, string author = null, string comment = null, bool? disable = null, string from_address = null, string mailto = null, string mailto_user = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("name", name);
                            parameters.Add("author", author);
                            parameters.Add("comment", comment);
                            parameters.Add("disable", disable);
                            parameters.Add("from-address", from_address);
                            parameters.Add("mailto", mailto);
                            parameters.Add("mailto-user", mailto_user);
                            return await _client.Create($"/cluster/notifications/endpoints/sendmail", parameters);
                        }
                    }
                    /// <summary>
                    /// Gotify
                    /// </summary>
                    public class PveGotify
                    {
                        private readonly PveClient _client;

                        internal PveGotify(PveClient client) { _client = client; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object name) { _client = client; _name = name; }
                            /// <summary>
                            /// Remove gotify endpoint
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> DeleteGotifyEndpoint() { return await _client.Delete($"/cluster/notifications/endpoints/gotify/{_name}"); }
                            /// <summary>
                            /// Return a specific gotify endpoint
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> GetGotifyEndpoint() { return await _client.Get($"/cluster/notifications/endpoints/gotify/{_name}"); }
                            /// <summary>
                            /// Update existing gotify endpoint
                            /// </summary>
                            /// <param name="comment">Comment</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="disable">Disable this target</param>
                            /// <param name="server">Server URL</param>
                            /// <param name="token">Secret token</param>
                            /// <returns></returns>
                            public async Task<Result> UpdateGotifyEndpoint(string comment = null, string delete = null, string digest = null, bool? disable = null, string server = null, string token = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("comment", comment);
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                parameters.Add("disable", disable);
                                parameters.Add("server", server);
                                parameters.Add("token", token);
                                return await _client.Set($"/cluster/notifications/endpoints/gotify/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// Returns a list of all gotify endpoints
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetGotifyEndpoints() { return await _client.Get($"/cluster/notifications/endpoints/gotify"); }
                        /// <summary>
                        /// Create a new gotify endpoint
                        /// </summary>
                        /// <param name="name">The name of the endpoint.</param>
                        /// <param name="server">Server URL</param>
                        /// <param name="token">Secret token</param>
                        /// <param name="comment">Comment</param>
                        /// <param name="disable">Disable this target</param>
                        /// <returns></returns>
                        public async Task<Result> CreateGotifyEndpoint(string name, string server, string token, string comment = null, bool? disable = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("name", name);
                            parameters.Add("server", server);
                            parameters.Add("token", token);
                            parameters.Add("comment", comment);
                            parameters.Add("disable", disable);
                            return await _client.Create($"/cluster/notifications/endpoints/gotify", parameters);
                        }
                    }
                    /// <summary>
                    /// Smtp
                    /// </summary>
                    public class PveSmtp
                    {
                        private readonly PveClient _client;

                        internal PveSmtp(PveClient client) { _client = client; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object name) { _client = client; _name = name; }
                            /// <summary>
                            /// Remove smtp endpoint
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> DeleteSmtpEndpoint() { return await _client.Delete($"/cluster/notifications/endpoints/smtp/{_name}"); }
                            /// <summary>
                            /// Return a specific smtp endpoint
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> GetSmtpEndpoint() { return await _client.Get($"/cluster/notifications/endpoints/smtp/{_name}"); }
                            /// <summary>
                            /// Update existing smtp endpoint
                            /// </summary>
                            /// <param name="author">Author of the mail. Defaults to 'Proxmox VE'.</param>
                            /// <param name="comment">Comment</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="disable">Disable this target</param>
                            /// <param name="from_address">`From` address for the mail</param>
                            /// <param name="mailto">List of email recipients</param>
                            /// <param name="mailto_user">List of users</param>
                            /// <param name="mode">Determine which encryption method shall be used for the connection.
                            ///   Enum: insecure,starttls,tls</param>
                            /// <param name="password">Password for SMTP authentication</param>
                            /// <param name="port">The port to be used. Defaults to 465 for TLS based connections, 587 for STARTTLS based connections and port 25 for insecure plain-text connections.</param>
                            /// <param name="server">The address of the SMTP server.</param>
                            /// <param name="username">Username for SMTP authentication</param>
                            /// <returns></returns>
                            public async Task<Result> UpdateSmtpEndpoint(string author = null, string comment = null, string delete = null, string digest = null, bool? disable = null, string from_address = null, string mailto = null, string mailto_user = null, string mode = null, string password = null, int? port = null, string server = null, string username = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("author", author);
                                parameters.Add("comment", comment);
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                parameters.Add("disable", disable);
                                parameters.Add("from-address", from_address);
                                parameters.Add("mailto", mailto);
                                parameters.Add("mailto-user", mailto_user);
                                parameters.Add("mode", mode);
                                parameters.Add("password", password);
                                parameters.Add("port", port);
                                parameters.Add("server", server);
                                parameters.Add("username", username);
                                return await _client.Set($"/cluster/notifications/endpoints/smtp/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// Returns a list of all smtp endpoints
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetSmtpEndpoints() { return await _client.Get($"/cluster/notifications/endpoints/smtp"); }
                        /// <summary>
                        /// Create a new smtp endpoint
                        /// </summary>
                        /// <param name="from_address">`From` address for the mail</param>
                        /// <param name="name">The name of the endpoint.</param>
                        /// <param name="server">The address of the SMTP server.</param>
                        /// <param name="author">Author of the mail. Defaults to 'Proxmox VE'.</param>
                        /// <param name="comment">Comment</param>
                        /// <param name="disable">Disable this target</param>
                        /// <param name="mailto">List of email recipients</param>
                        /// <param name="mailto_user">List of users</param>
                        /// <param name="mode">Determine which encryption method shall be used for the connection.
                        ///   Enum: insecure,starttls,tls</param>
                        /// <param name="password">Password for SMTP authentication</param>
                        /// <param name="port">The port to be used. Defaults to 465 for TLS based connections, 587 for STARTTLS based connections and port 25 for insecure plain-text connections.</param>
                        /// <param name="username">Username for SMTP authentication</param>
                        /// <returns></returns>
                        public async Task<Result> CreateSmtpEndpoint(string from_address, string name, string server, string author = null, string comment = null, bool? disable = null, string mailto = null, string mailto_user = null, string mode = null, string password = null, int? port = null, string username = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("from-address", from_address);
                            parameters.Add("name", name);
                            parameters.Add("server", server);
                            parameters.Add("author", author);
                            parameters.Add("comment", comment);
                            parameters.Add("disable", disable);
                            parameters.Add("mailto", mailto);
                            parameters.Add("mailto-user", mailto_user);
                            parameters.Add("mode", mode);
                            parameters.Add("password", password);
                            parameters.Add("port", port);
                            parameters.Add("username", username);
                            return await _client.Create($"/cluster/notifications/endpoints/smtp", parameters);
                        }
                    }
                    /// <summary>
                    /// Index for all available endpoint types.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> EndpointsIndex() { return await _client.Get($"/cluster/notifications/endpoints"); }
                }
                /// <summary>
                /// Targets
                /// </summary>
                public class PveTargets
                {
                    private readonly PveClient _client;

                    internal PveTargets(PveClient client) { _client = client; }
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public PveNameItem this[object name] => new(_client, name);
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public class PveNameItem
                    {
                        private readonly PveClient _client;
                        private readonly object _name;
                        internal PveNameItem(PveClient client, object name) { _client = client; _name = name; }
                        private PveTest _test;
                        /// <summary>
                        /// Test
                        /// </summary>
                        public PveTest Test => _test ??= new(_client, _name);
                        /// <summary>
                        /// Test
                        /// </summary>
                        public class PveTest
                        {
                            private readonly PveClient _client;
                            private readonly object _name;
                            internal PveTest(PveClient client, object name) { _client = client; _name = name; }
                            /// <summary>
                            /// Send a test notification to a provided target.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> TestTarget() { return await _client.Create($"/cluster/notifications/targets/{_name}/test"); }
                        }
                    }
                    /// <summary>
                    /// Returns a list of all entities that can be used as notification targets.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetAllTargets() { return await _client.Get($"/cluster/notifications/targets"); }
                }
                /// <summary>
                /// Matchers
                /// </summary>
                public class PveMatchers
                {
                    private readonly PveClient _client;

                    internal PveMatchers(PveClient client) { _client = client; }
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public PveNameItem this[object name] => new(_client, name);
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public class PveNameItem
                    {
                        private readonly PveClient _client;
                        private readonly object _name;
                        internal PveNameItem(PveClient client, object name) { _client = client; _name = name; }
                        /// <summary>
                        /// Remove matcher
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> DeleteMatcher() { return await _client.Delete($"/cluster/notifications/matchers/{_name}"); }
                        /// <summary>
                        /// Return a specific matcher
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetMatcher() { return await _client.Get($"/cluster/notifications/matchers/{_name}"); }
                        /// <summary>
                        /// Update existing matcher
                        /// </summary>
                        /// <param name="comment">Comment</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="disable">Disable this matcher</param>
                        /// <param name="invert_match">Invert match of the whole matcher</param>
                        /// <param name="match_calendar">Match notification timestamp</param>
                        /// <param name="match_field">Metadata fields to match (regex or exact match). Must be in the form (regex|exact):&amp;lt;field&amp;gt;=&amp;lt;value&amp;gt;</param>
                        /// <param name="match_severity">Notification severities to match</param>
                        /// <param name="mode">Choose between 'all' and 'any' for when multiple properties are specified
                        ///   Enum: all,any</param>
                        /// <param name="target">Targets to notify on match</param>
                        /// <returns></returns>
                        public async Task<Result> UpdateMatcher(string comment = null, string delete = null, string digest = null, bool? disable = null, bool? invert_match = null, string match_calendar = null, string match_field = null, string match_severity = null, string mode = null, string target = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("disable", disable);
                            parameters.Add("invert-match", invert_match);
                            parameters.Add("match-calendar", match_calendar);
                            parameters.Add("match-field", match_field);
                            parameters.Add("match-severity", match_severity);
                            parameters.Add("mode", mode);
                            parameters.Add("target", target);
                            return await _client.Set($"/cluster/notifications/matchers/{_name}", parameters);
                        }
                    }
                    /// <summary>
                    /// Returns a list of all matchers
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetMatchers() { return await _client.Get($"/cluster/notifications/matchers"); }
                    /// <summary>
                    /// Create a new matcher
                    /// </summary>
                    /// <param name="name">Name of the matcher.</param>
                    /// <param name="comment">Comment</param>
                    /// <param name="disable">Disable this matcher</param>
                    /// <param name="invert_match">Invert match of the whole matcher</param>
                    /// <param name="match_calendar">Match notification timestamp</param>
                    /// <param name="match_field">Metadata fields to match (regex or exact match). Must be in the form (regex|exact):&amp;lt;field&amp;gt;=&amp;lt;value&amp;gt;</param>
                    /// <param name="match_severity">Notification severities to match</param>
                    /// <param name="mode">Choose between 'all' and 'any' for when multiple properties are specified
                    ///   Enum: all,any</param>
                    /// <param name="target">Targets to notify on match</param>
                    /// <returns></returns>
                    public async Task<Result> CreateMatcher(string name, string comment = null, bool? disable = null, bool? invert_match = null, string match_calendar = null, string match_field = null, string match_severity = null, string mode = null, string target = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("name", name);
                        parameters.Add("comment", comment);
                        parameters.Add("disable", disable);
                        parameters.Add("invert-match", invert_match);
                        parameters.Add("match-calendar", match_calendar);
                        parameters.Add("match-field", match_field);
                        parameters.Add("match-severity", match_severity);
                        parameters.Add("mode", mode);
                        parameters.Add("target", target);
                        return await _client.Create($"/cluster/notifications/matchers", parameters);
                    }
                }
                /// <summary>
                /// Index for notification-related API endpoints.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/notifications"); }
            }
            /// <summary>
            /// Config
            /// </summary>
            public class PveConfig
            {
                private readonly PveClient _client;

                internal PveConfig(PveClient client) { _client = client; }
                private PveApiversion _apiversion;
                /// <summary>
                /// Apiversion
                /// </summary>
                public PveApiversion Apiversion => _apiversion ??= new(_client);
                private PveNodes _nodes;
                /// <summary>
                /// Nodes
                /// </summary>
                public PveNodes Nodes => _nodes ??= new(_client);
                private PveJoin _join;
                /// <summary>
                /// Join
                /// </summary>
                public PveJoin Join => _join ??= new(_client);
                private PveTotem _totem;
                /// <summary>
                /// Totem
                /// </summary>
                public PveTotem Totem => _totem ??= new(_client);
                private PveQdevice _qdevice;
                /// <summary>
                /// Qdevice
                /// </summary>
                public PveQdevice Qdevice => _qdevice ??= new(_client);
                /// <summary>
                /// Apiversion
                /// </summary>
                public class PveApiversion
                {
                    private readonly PveClient _client;

                    internal PveApiversion(PveClient client) { _client = client; }
                    /// <summary>
                    /// Return the version of the cluster join API available on this node.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> JoinApiVersion() { return await _client.Get($"/cluster/config/apiversion"); }
                }
                /// <summary>
                /// Nodes
                /// </summary>
                public class PveNodes
                {
                    private readonly PveClient _client;

                    internal PveNodes(PveClient client) { _client = client; }
                    /// <summary>
                    /// NodeItem
                    /// </summary>
                    public PveNodeItem this[object node] => new(_client, node);
                    /// <summary>
                    /// NodeItem
                    /// </summary>
                    public class PveNodeItem
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveNodeItem(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Removes a node from the cluster configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delnode() { return await _client.Delete($"/cluster/config/nodes/{_node}"); }
                        /// <summary>
                        /// Adds a node to the cluster configuration. This call is for internal use.
                        /// </summary>
                        /// <param name="apiversion">The JOIN_API_VERSION of the new node.</param>
                        /// <param name="force">Do not throw error if node already exists.</param>
                        /// <param name="linkN">Address and priority information of a single corosync link. (up to 8 links supported; link0..link7)</param>
                        /// <param name="new_node_ip">IP Address of node to add. Used as fallback if no links are given.</param>
                        /// <param name="nodeid">Node id for this node.</param>
                        /// <param name="votes">Number of votes for this node</param>
                        /// <returns></returns>
                        public async Task<Result> Addnode(int? apiversion = null, bool? force = null, IDictionary<int, string> linkN = null, string new_node_ip = null, int? nodeid = null, int? votes = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("apiversion", apiversion);
                            parameters.Add("force", force);
                            parameters.Add("new_node_ip", new_node_ip);
                            parameters.Add("nodeid", nodeid);
                            parameters.Add("votes", votes);
                            AddIndexedParameter(parameters, "link", linkN);
                            return await _client.Create($"/cluster/config/nodes/{_node}", parameters);
                        }
                    }
                    /// <summary>
                    /// Corosync node list.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Nodes() { return await _client.Get($"/cluster/config/nodes"); }
                }
                /// <summary>
                /// Join
                /// </summary>
                public class PveJoin
                {
                    private readonly PveClient _client;

                    internal PveJoin(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get information needed to join this cluster over the connected node.
                    /// </summary>
                    /// <param name="node">The node for which the joinee gets the nodeinfo. </param>
                    /// <returns></returns>
                    public async Task<Result> JoinInfo(string node = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("node", node);
                        return await _client.Get($"/cluster/config/join", parameters);
                    }
                    /// <summary>
                    /// Joins this node into an existing cluster. If no links are given, default to IP resolved by node's hostname on single link (fallback fails for clusters with multiple links).
                    /// </summary>
                    /// <param name="fingerprint">Certificate SHA 256 fingerprint.</param>
                    /// <param name="hostname">Hostname (or IP) of an existing cluster member.</param>
                    /// <param name="password">Superuser (root) password of peer node.</param>
                    /// <param name="force">Do not throw error if node already exists.</param>
                    /// <param name="linkN">Address and priority information of a single corosync link. (up to 8 links supported; link0..link7)</param>
                    /// <param name="nodeid">Node id for this node.</param>
                    /// <param name="votes">Number of votes for this node</param>
                    /// <returns></returns>
                    public async Task<Result> Join(string fingerprint, string hostname, string password, bool? force = null, IDictionary<int, string> linkN = null, int? nodeid = null, int? votes = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("fingerprint", fingerprint);
                        parameters.Add("hostname", hostname);
                        parameters.Add("password", password);
                        parameters.Add("force", force);
                        parameters.Add("nodeid", nodeid);
                        parameters.Add("votes", votes);
                        AddIndexedParameter(parameters, "link", linkN);
                        return await _client.Create($"/cluster/config/join", parameters);
                    }
                }
                /// <summary>
                /// Totem
                /// </summary>
                public class PveTotem
                {
                    private readonly PveClient _client;

                    internal PveTotem(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get corosync totem protocol settings.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Totem() { return await _client.Get($"/cluster/config/totem"); }
                }
                /// <summary>
                /// Qdevice
                /// </summary>
                public class PveQdevice
                {
                    private readonly PveClient _client;

                    internal PveQdevice(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get QDevice status
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Status() { return await _client.Get($"/cluster/config/qdevice"); }
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/config"); }
                /// <summary>
                /// Generate new cluster configuration. If no links given, default to local IP address as link0.
                /// </summary>
                /// <param name="clustername">The name of the cluster.</param>
                /// <param name="linkN">Address and priority information of a single corosync link. (up to 8 links supported; link0..link7)</param>
                /// <param name="nodeid">Node id for this node.</param>
                /// <param name="votes">Number of votes for this node.</param>
                /// <returns></returns>
                public async Task<Result> Create(string clustername, IDictionary<int, string> linkN = null, int? nodeid = null, int? votes = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("clustername", clustername);
                    parameters.Add("nodeid", nodeid);
                    parameters.Add("votes", votes);
                    AddIndexedParameter(parameters, "link", linkN);
                    return await _client.Create($"/cluster/config", parameters);
                }
            }
            /// <summary>
            /// Firewall
            /// </summary>
            public class PveFirewall
            {
                private readonly PveClient _client;

                internal PveFirewall(PveClient client) { _client = client; }
                private PveGroups _groups;
                /// <summary>
                /// Groups
                /// </summary>
                public PveGroups Groups => _groups ??= new(_client);
                private PveRules _rules;
                /// <summary>
                /// Rules
                /// </summary>
                public PveRules Rules => _rules ??= new(_client);
                private PveIpset _ipset;
                /// <summary>
                /// Ipset
                /// </summary>
                public PveIpset Ipset => _ipset ??= new(_client);
                private PveAliases _aliases;
                /// <summary>
                /// Aliases
                /// </summary>
                public PveAliases Aliases => _aliases ??= new(_client);
                private PveOptions _options;
                /// <summary>
                /// Options
                /// </summary>
                public PveOptions Options => _options ??= new(_client);
                private PveMacros _macros;
                /// <summary>
                /// Macros
                /// </summary>
                public PveMacros Macros => _macros ??= new(_client);
                private PveRefs _refs;
                /// <summary>
                /// Refs
                /// </summary>
                public PveRefs Refs => _refs ??= new(_client);
                /// <summary>
                /// Groups
                /// </summary>
                public class PveGroups
                {
                    private readonly PveClient _client;

                    internal PveGroups(PveClient client) { _client = client; }
                    /// <summary>
                    /// GroupItem
                    /// </summary>
                    public PveGroupItem this[object group] => new(_client, group);
                    /// <summary>
                    /// GroupItem
                    /// </summary>
                    public class PveGroupItem
                    {
                        private readonly PveClient _client;
                        private readonly object _group;
                        internal PveGroupItem(PveClient client, object group) { _client = client; _group = group; }
                        /// <summary>
                        /// PosItem
                        /// </summary>
                        public PvePosItem this[object pos] => new(_client, _group, pos);
                        /// <summary>
                        /// PosItem
                        /// </summary>
                        public class PvePosItem
                        {
                            private readonly PveClient _client;
                            private readonly object _group;
                            private readonly object _pos;
                            internal PvePosItem(PveClient client, object group, object pos)
                            {
                                _client = client; _group = group;
                                _pos = pos;
                            }
                            /// <summary>
                            /// Delete rule.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public async Task<Result> DeleteRule(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                return await _client.Delete($"/cluster/firewall/groups/{_group}/{_pos}", parameters);
                            }
                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> GetRule() { return await _client.Get($"/cluster/firewall/groups/{_group}/{_pos}"); }
                            /// <summary>
                            /// Modify rule data.
                            /// </summary>
                            /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                            /// <param name="comment">Descriptive comment.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="enable">Flag to enable/disable a rule.</param>
                            /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
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
                            public async Task<Result> UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("action", action);
                                parameters.Add("comment", comment);
                                parameters.Add("delete", delete);
                                parameters.Add("dest", dest);
                                parameters.Add("digest", digest);
                                parameters.Add("dport", dport);
                                parameters.Add("enable", enable);
                                parameters.Add("icmp-type", icmp_type);
                                parameters.Add("iface", iface);
                                parameters.Add("log", log);
                                parameters.Add("macro", macro);
                                parameters.Add("moveto", moveto);
                                parameters.Add("proto", proto);
                                parameters.Add("source", source);
                                parameters.Add("sport", sport);
                                parameters.Add("type", type);
                                return await _client.Set($"/cluster/firewall/groups/{_group}/{_pos}", parameters);
                            }
                        }
                        /// <summary>
                        /// Delete security group.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> DeleteSecurityGroup() { return await _client.Delete($"/cluster/firewall/groups/{_group}"); }
                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetRules() { return await _client.Get($"/cluster/firewall/groups/{_group}"); }
                        /// <summary>
                        /// Create new rule.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
                        /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                        /// <param name="log">Log level for firewall rule.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public async Task<Result> CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("type", type);
                            parameters.Add("comment", comment);
                            parameters.Add("dest", dest);
                            parameters.Add("digest", digest);
                            parameters.Add("dport", dport);
                            parameters.Add("enable", enable);
                            parameters.Add("icmp-type", icmp_type);
                            parameters.Add("iface", iface);
                            parameters.Add("log", log);
                            parameters.Add("macro", macro);
                            parameters.Add("pos", pos);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            return await _client.Create($"/cluster/firewall/groups/{_group}", parameters);
                        }
                    }
                    /// <summary>
                    /// List security groups.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> ListSecurityGroups() { return await _client.Get($"/cluster/firewall/groups"); }
                    /// <summary>
                    /// Create new security group.
                    /// </summary>
                    /// <param name="group">Security Group name.</param>
                    /// <param name="comment"></param>
                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="rename">Rename/update an existing security group. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing group.</param>
                    /// <returns></returns>
                    public async Task<Result> CreateSecurityGroup(string group, string comment = null, string digest = null, string rename = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("group", group);
                        parameters.Add("comment", comment);
                        parameters.Add("digest", digest);
                        parameters.Add("rename", rename);
                        return await _client.Create($"/cluster/firewall/groups", parameters);
                    }
                }
                /// <summary>
                /// Rules
                /// </summary>
                public class PveRules
                {
                    private readonly PveClient _client;

                    internal PveRules(PveClient client) { _client = client; }
                    /// <summary>
                    /// PosItem
                    /// </summary>
                    public PvePosItem this[object pos] => new(_client, pos);
                    /// <summary>
                    /// PosItem
                    /// </summary>
                    public class PvePosItem
                    {
                        private readonly PveClient _client;
                        private readonly object _pos;
                        internal PvePosItem(PveClient client, object pos) { _client = client; _pos = pos; }
                        /// <summary>
                        /// Delete rule.
                        /// </summary>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <returns></returns>
                        public async Task<Result> DeleteRule(string digest = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("digest", digest);
                            return await _client.Delete($"/cluster/firewall/rules/{_pos}", parameters);
                        }
                        /// <summary>
                        /// Get single rule data.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetRule() { return await _client.Get($"/cluster/firewall/rules/{_pos}"); }
                        /// <summary>
                        /// Modify rule data.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
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
                        public async Task<Result> UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("dest", dest);
                            parameters.Add("digest", digest);
                            parameters.Add("dport", dport);
                            parameters.Add("enable", enable);
                            parameters.Add("icmp-type", icmp_type);
                            parameters.Add("iface", iface);
                            parameters.Add("log", log);
                            parameters.Add("macro", macro);
                            parameters.Add("moveto", moveto);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            parameters.Add("type", type);
                            return await _client.Set($"/cluster/firewall/rules/{_pos}", parameters);
                        }
                    }
                    /// <summary>
                    /// List rules.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetRules() { return await _client.Get($"/cluster/firewall/rules"); }
                    /// <summary>
                    /// Create new rule.
                    /// </summary>
                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                    /// <param name="type">Rule type.
                    ///   Enum: in,out,group</param>
                    /// <param name="comment">Descriptive comment.</param>
                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    /// <param name="enable">Flag to enable/disable a rule.</param>
                    /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
                    /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                    /// <param name="log">Log level for firewall rule.
                    ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                    /// <param name="macro">Use predefined standard macro.</param>
                    /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                    /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                    /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                    /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                    /// <returns></returns>
                    public async Task<Result> CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("action", action);
                        parameters.Add("type", type);
                        parameters.Add("comment", comment);
                        parameters.Add("dest", dest);
                        parameters.Add("digest", digest);
                        parameters.Add("dport", dport);
                        parameters.Add("enable", enable);
                        parameters.Add("icmp-type", icmp_type);
                        parameters.Add("iface", iface);
                        parameters.Add("log", log);
                        parameters.Add("macro", macro);
                        parameters.Add("pos", pos);
                        parameters.Add("proto", proto);
                        parameters.Add("source", source);
                        parameters.Add("sport", sport);
                        return await _client.Create($"/cluster/firewall/rules", parameters);
                    }
                }
                /// <summary>
                /// Ipset
                /// </summary>
                public class PveIpset
                {
                    private readonly PveClient _client;

                    internal PveIpset(PveClient client) { _client = client; }
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public PveNameItem this[object name] => new(_client, name);
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public class PveNameItem
                    {
                        private readonly PveClient _client;
                        private readonly object _name;
                        internal PveNameItem(PveClient client, object name) { _client = client; _name = name; }
                        /// <summary>
                        /// CidrItem
                        /// </summary>
                        public PveCidrItem this[object cidr] => new(_client, _name, cidr);
                        /// <summary>
                        /// CidrItem
                        /// </summary>
                        public class PveCidrItem
                        {
                            private readonly PveClient _client;
                            private readonly object _name;
                            private readonly object _cidr;
                            internal PveCidrItem(PveClient client, object name, object cidr)
                            {
                                _client = client; _name = name;
                                _cidr = cidr;
                            }
                            /// <summary>
                            /// Remove IP or Network from IPSet.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public async Task<Result> RemoveIp(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                return await _client.Delete($"/cluster/firewall/ipset/{_name}/{_cidr}", parameters);
                            }
                            /// <summary>
                            /// Read IP or Network settings from IPSet.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ReadIp() { return await _client.Get($"/cluster/firewall/ipset/{_name}/{_cidr}"); }
                            /// <summary>
                            /// Update IP or Network settings
                            /// </summary>
                            /// <param name="comment"></param>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="nomatch"></param>
                            /// <returns></returns>
                            public async Task<Result> UpdateIp(string comment = null, string digest = null, bool? nomatch = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("comment", comment);
                                parameters.Add("digest", digest);
                                parameters.Add("nomatch", nomatch);
                                return await _client.Set($"/cluster/firewall/ipset/{_name}/{_cidr}", parameters);
                            }
                        }
                        /// <summary>
                        /// Delete IPSet
                        /// </summary>
                        /// <param name="force">Delete all members of the IPSet, if there are any.</param>
                        /// <returns></returns>
                        public async Task<Result> DeleteIpset(bool? force = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("force", force);
                            return await _client.Delete($"/cluster/firewall/ipset/{_name}", parameters);
                        }
                        /// <summary>
                        /// List IPSet content
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetIpset() { return await _client.Get($"/cluster/firewall/ipset/{_name}"); }
                        /// <summary>
                        /// Add IP or Network to IPSet.
                        /// </summary>
                        /// <param name="cidr">Network/IP specification in CIDR format.</param>
                        /// <param name="comment"></param>
                        /// <param name="nomatch"></param>
                        /// <returns></returns>
                        public async Task<Result> CreateIp(string cidr, string comment = null, bool? nomatch = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("cidr", cidr);
                            parameters.Add("comment", comment);
                            parameters.Add("nomatch", nomatch);
                            return await _client.Create($"/cluster/firewall/ipset/{_name}", parameters);
                        }
                    }
                    /// <summary>
                    /// List IPSets
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> IpsetIndex() { return await _client.Get($"/cluster/firewall/ipset"); }
                    /// <summary>
                    /// Create new IPSet
                    /// </summary>
                    /// <param name="name">IP set name.</param>
                    /// <param name="comment"></param>
                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                    /// <returns></returns>
                    public async Task<Result> CreateIpset(string name, string comment = null, string digest = null, string rename = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("name", name);
                        parameters.Add("comment", comment);
                        parameters.Add("digest", digest);
                        parameters.Add("rename", rename);
                        return await _client.Create($"/cluster/firewall/ipset", parameters);
                    }
                }
                /// <summary>
                /// Aliases
                /// </summary>
                public class PveAliases
                {
                    private readonly PveClient _client;

                    internal PveAliases(PveClient client) { _client = client; }
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public PveNameItem this[object name] => new(_client, name);
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public class PveNameItem
                    {
                        private readonly PveClient _client;
                        private readonly object _name;
                        internal PveNameItem(PveClient client, object name) { _client = client; _name = name; }
                        /// <summary>
                        /// Remove IP or Network alias.
                        /// </summary>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <returns></returns>
                        public async Task<Result> RemoveAlias(string digest = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("digest", digest);
                            return await _client.Delete($"/cluster/firewall/aliases/{_name}", parameters);
                        }
                        /// <summary>
                        /// Read alias.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> ReadAlias() { return await _client.Get($"/cluster/firewall/aliases/{_name}"); }
                        /// <summary>
                        /// Update IP or Network alias.
                        /// </summary>
                        /// <param name="cidr">Network/IP specification in CIDR format.</param>
                        /// <param name="comment"></param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="rename">Rename an existing alias.</param>
                        /// <returns></returns>
                        public async Task<Result> UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("cidr", cidr);
                            parameters.Add("comment", comment);
                            parameters.Add("digest", digest);
                            parameters.Add("rename", rename);
                            return await _client.Set($"/cluster/firewall/aliases/{_name}", parameters);
                        }
                    }
                    /// <summary>
                    /// List aliases
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetAliases() { return await _client.Get($"/cluster/firewall/aliases"); }
                    /// <summary>
                    /// Create IP or Network Alias.
                    /// </summary>
                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                    /// <param name="name">Alias name.</param>
                    /// <param name="comment"></param>
                    /// <returns></returns>
                    public async Task<Result> CreateAlias(string cidr, string name, string comment = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cidr", cidr);
                        parameters.Add("name", name);
                        parameters.Add("comment", comment);
                        return await _client.Create($"/cluster/firewall/aliases", parameters);
                    }
                }
                /// <summary>
                /// Options
                /// </summary>
                public class PveOptions
                {
                    private readonly PveClient _client;

                    internal PveOptions(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get Firewall options.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetOptions() { return await _client.Get($"/cluster/firewall/options"); }
                    /// <summary>
                    /// Set Firewall options.
                    /// </summary>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="ebtables">Enable ebtables rules cluster wide.</param>
                    /// <param name="enable">Enable or disable the firewall cluster wide.</param>
                    /// <param name="log_ratelimit">Log ratelimiting settings</param>
                    /// <param name="policy_in">Input policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <param name="policy_out">Output policy.
                    ///   Enum: ACCEPT,REJECT,DROP</param>
                    /// <returns></returns>
                    public async Task<Result> SetOptions(string delete = null, string digest = null, bool? ebtables = null, int? enable = null, string log_ratelimit = null, string policy_in = null, string policy_out = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("delete", delete);
                        parameters.Add("digest", digest);
                        parameters.Add("ebtables", ebtables);
                        parameters.Add("enable", enable);
                        parameters.Add("log_ratelimit", log_ratelimit);
                        parameters.Add("policy_in", policy_in);
                        parameters.Add("policy_out", policy_out);
                        return await _client.Set($"/cluster/firewall/options", parameters);
                    }
                }
                /// <summary>
                /// Macros
                /// </summary>
                public class PveMacros
                {
                    private readonly PveClient _client;

                    internal PveMacros(PveClient client) { _client = client; }
                    /// <summary>
                    /// List available macros
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetMacros() { return await _client.Get($"/cluster/firewall/macros"); }
                }
                /// <summary>
                /// Refs
                /// </summary>
                public class PveRefs
                {
                    private readonly PveClient _client;

                    internal PveRefs(PveClient client) { _client = client; }
                    /// <summary>
                    /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
                    /// </summary>
                    /// <param name="type">Only list references of specified type.
                    ///   Enum: alias,ipset</param>
                    /// <returns></returns>
                    public async Task<Result> Refs(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return await _client.Get($"/cluster/firewall/refs", parameters);
                    }
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/firewall"); }
            }
            /// <summary>
            /// Backup
            /// </summary>
            public class PveBackup
            {
                private readonly PveClient _client;

                internal PveBackup(PveClient client) { _client = client; }
                /// <summary>
                /// IdItem
                /// </summary>
                public PveIdItem this[object id] => new(_client, id);
                /// <summary>
                /// IdItem
                /// </summary>
                public class PveIdItem
                {
                    private readonly PveClient _client;
                    private readonly object _id;
                    internal PveIdItem(PveClient client, object id) { _client = client; _id = id; }
                    private PveIncludedVolumes _includedVolumes;
                    /// <summary>
                    /// IncludedVolumes
                    /// </summary>
                    public PveIncludedVolumes IncludedVolumes => _includedVolumes ??= new(_client, _id);
                    /// <summary>
                    /// IncludedVolumes
                    /// </summary>
                    public class PveIncludedVolumes
                    {
                        private readonly PveClient _client;
                        private readonly object _id;
                        internal PveIncludedVolumes(PveClient client, object id) { _client = client; _id = id; }
                        /// <summary>
                        /// Returns included guests and the backup status of their disks. Optimized to be used in ExtJS tree views.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetVolumeBackupIncluded() { return await _client.Get($"/cluster/backup/{_id}/included_volumes"); }
                    }
                    /// <summary>
                    /// Delete vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> DeleteJob() { return await _client.Delete($"/cluster/backup/{_id}"); }
                    /// <summary>
                    /// Read vzdump backup job definition.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> ReadJob() { return await _client.Get($"/cluster/backup/{_id}"); }
                    /// <summary>
                    /// Update vzdump backup job definition.
                    /// </summary>
                    /// <param name="all">Backup all known guest systems on this host.</param>
                    /// <param name="bwlimit">Limit I/O bandwidth (in KiB/s).</param>
                    /// <param name="comment">Description for the Job.</param>
                    /// <param name="compress">Compress dump file.
                    ///   Enum: 0,1,gzip,lzo,zstd</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="dow">Day of week selection.</param>
                    /// <param name="dumpdir">Store resulting files to specified directory.</param>
                    /// <param name="enabled">Enable or disable the job.</param>
                    /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                    /// <param name="exclude_path">Exclude certain files/directories (shell globs). Paths starting with '/' are anchored to the container's root,  other paths match relative to each subdirectory.</param>
                    /// <param name="ionice">Set IO priority when using the BFQ scheduler. For snapshot and suspend mode backups of VMs, this only affects the compressor. A value of 8 means the idle priority is used, otherwise the best-effort priority is used with the specified value.</param>
                    /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                    /// <param name="mailnotification">Deprecated: use 'notification-policy' instead.
                    ///   Enum: always,failure</param>
                    /// <param name="mailto">Comma-separated list of email addresses or users that should receive email notifications. Has no effect if the 'notification-target' option  is set at the same time.</param>
                    /// <param name="maxfiles">Deprecated: use 'prune-backups' instead. Maximal number of backup files per guest system.</param>
                    /// <param name="mode">Backup mode.
                    ///   Enum: snapshot,suspend,stop</param>
                    /// <param name="node">Only run if executed on this node.</param>
                    /// <param name="notes_template">Template string for generating notes for the backup(s). It can contain variables which will be replaced by their values. Currently supported are {{cluster}}, {{guestname}}, {{node}}, and {{vmid}}, but more might be added in the future. Needs to be a single line, newline and backslash need to be escaped as '\n' and '\\' respectively.</param>
                    /// <param name="notification_policy">Specify when to send a notification
                    ///   Enum: always,failure,never</param>
                    /// <param name="notification_target">Determine the target to which notifications should be sent. Can either be a notification endpoint or a notification group. This option takes precedence over 'mailto', meaning that if both are  set, the 'mailto' option will be ignored.</param>
                    /// <param name="performance">Other performance-related settings.</param>
                    /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                    /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                    /// <param name="protected_">If true, mark backup(s) as protected.</param>
                    /// <param name="prune_backups">Use these retention options instead of those from the storage configuration.</param>
                    /// <param name="quiet">Be quiet.</param>
                    /// <param name="remove">Prune older backups according to 'prune-backups'.</param>
                    /// <param name="repeat_missed">If true, the job will be run as soon as possible if it was missed while the scheduler was not running.</param>
                    /// <param name="schedule">Backup schedule. The format is a subset of `systemd` calendar events.</param>
                    /// <param name="script">Use specified hook script.</param>
                    /// <param name="starttime">Job Start time.</param>
                    /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                    /// <param name="stop">Stop running backup jobs on this host.</param>
                    /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                    /// <param name="storage">Store resulting file to this storage.</param>
                    /// <param name="tmpdir">Store temporary files to specified directory.</param>
                    /// <param name="vmid">The ID of the guest system you want to backup.</param>
                    /// <param name="zstd">Zstd threads. N=0 uses half of the available cores, N&amp;gt;0 uses N as thread count.</param>
                    /// <returns></returns>
                    public async Task<Result> UpdateJob(bool? all = null, int? bwlimit = null, string comment = null, string compress = null, string delete = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, string notes_template = null, string notification_policy = null, string notification_target = null, string performance = null, int? pigz = null, string pool = null, bool? protected_ = null, string prune_backups = null, bool? quiet = null, bool? remove = null, bool? repeat_missed = null, string schedule = null, string script = null, string starttime = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null, int? zstd = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("all", all);
                        parameters.Add("bwlimit", bwlimit);
                        parameters.Add("comment", comment);
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
                        parameters.Add("notes-template", notes_template);
                        parameters.Add("notification-policy", notification_policy);
                        parameters.Add("notification-target", notification_target);
                        parameters.Add("performance", performance);
                        parameters.Add("pigz", pigz);
                        parameters.Add("pool", pool);
                        parameters.Add("protected", protected_);
                        parameters.Add("prune-backups", prune_backups);
                        parameters.Add("quiet", quiet);
                        parameters.Add("remove", remove);
                        parameters.Add("repeat-missed", repeat_missed);
                        parameters.Add("schedule", schedule);
                        parameters.Add("script", script);
                        parameters.Add("starttime", starttime);
                        parameters.Add("stdexcludes", stdexcludes);
                        parameters.Add("stop", stop);
                        parameters.Add("stopwait", stopwait);
                        parameters.Add("storage", storage);
                        parameters.Add("tmpdir", tmpdir);
                        parameters.Add("vmid", vmid);
                        parameters.Add("zstd", zstd);
                        return await _client.Set($"/cluster/backup/{_id}", parameters);
                    }
                }
                /// <summary>
                /// List vzdump backup schedule.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/backup"); }
                /// <summary>
                /// Create new vzdump backup job.
                /// </summary>
                /// <param name="all">Backup all known guest systems on this host.</param>
                /// <param name="bwlimit">Limit I/O bandwidth (in KiB/s).</param>
                /// <param name="comment">Description for the Job.</param>
                /// <param name="compress">Compress dump file.
                ///   Enum: 0,1,gzip,lzo,zstd</param>
                /// <param name="dow">Day of week selection.</param>
                /// <param name="dumpdir">Store resulting files to specified directory.</param>
                /// <param name="enabled">Enable or disable the job.</param>
                /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                /// <param name="exclude_path">Exclude certain files/directories (shell globs). Paths starting with '/' are anchored to the container's root,  other paths match relative to each subdirectory.</param>
                /// <param name="id">Job ID (will be autogenerated).</param>
                /// <param name="ionice">Set IO priority when using the BFQ scheduler. For snapshot and suspend mode backups of VMs, this only affects the compressor. A value of 8 means the idle priority is used, otherwise the best-effort priority is used with the specified value.</param>
                /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                /// <param name="mailnotification">Deprecated: use 'notification-policy' instead.
                ///   Enum: always,failure</param>
                /// <param name="mailto">Comma-separated list of email addresses or users that should receive email notifications. Has no effect if the 'notification-target' option  is set at the same time.</param>
                /// <param name="maxfiles">Deprecated: use 'prune-backups' instead. Maximal number of backup files per guest system.</param>
                /// <param name="mode">Backup mode.
                ///   Enum: snapshot,suspend,stop</param>
                /// <param name="node">Only run if executed on this node.</param>
                /// <param name="notes_template">Template string for generating notes for the backup(s). It can contain variables which will be replaced by their values. Currently supported are {{cluster}}, {{guestname}}, {{node}}, and {{vmid}}, but more might be added in the future. Needs to be a single line, newline and backslash need to be escaped as '\n' and '\\' respectively.</param>
                /// <param name="notification_policy">Specify when to send a notification
                ///   Enum: always,failure,never</param>
                /// <param name="notification_target">Determine the target to which notifications should be sent. Can either be a notification endpoint or a notification group. This option takes precedence over 'mailto', meaning that if both are  set, the 'mailto' option will be ignored.</param>
                /// <param name="performance">Other performance-related settings.</param>
                /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                /// <param name="protected_">If true, mark backup(s) as protected.</param>
                /// <param name="prune_backups">Use these retention options instead of those from the storage configuration.</param>
                /// <param name="quiet">Be quiet.</param>
                /// <param name="remove">Prune older backups according to 'prune-backups'.</param>
                /// <param name="repeat_missed">If true, the job will be run as soon as possible if it was missed while the scheduler was not running.</param>
                /// <param name="schedule">Backup schedule. The format is a subset of `systemd` calendar events.</param>
                /// <param name="script">Use specified hook script.</param>
                /// <param name="starttime">Job Start time.</param>
                /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                /// <param name="stop">Stop running backup jobs on this host.</param>
                /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                /// <param name="storage">Store resulting file to this storage.</param>
                /// <param name="tmpdir">Store temporary files to specified directory.</param>
                /// <param name="vmid">The ID of the guest system you want to backup.</param>
                /// <param name="zstd">Zstd threads. N=0 uses half of the available cores, N&amp;gt;0 uses N as thread count.</param>
                /// <returns></returns>
                public async Task<Result> CreateJob(bool? all = null, int? bwlimit = null, string comment = null, string compress = null, string dow = null, string dumpdir = null, bool? enabled = null, string exclude = null, string exclude_path = null, string id = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string node = null, string notes_template = null, string notification_policy = null, string notification_target = null, string performance = null, int? pigz = null, string pool = null, bool? protected_ = null, string prune_backups = null, bool? quiet = null, bool? remove = null, bool? repeat_missed = null, string schedule = null, string script = null, string starttime = null, bool? stdexcludes = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null, int? zstd = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("all", all);
                    parameters.Add("bwlimit", bwlimit);
                    parameters.Add("comment", comment);
                    parameters.Add("compress", compress);
                    parameters.Add("dow", dow);
                    parameters.Add("dumpdir", dumpdir);
                    parameters.Add("enabled", enabled);
                    parameters.Add("exclude", exclude);
                    parameters.Add("exclude-path", exclude_path);
                    parameters.Add("id", id);
                    parameters.Add("ionice", ionice);
                    parameters.Add("lockwait", lockwait);
                    parameters.Add("mailnotification", mailnotification);
                    parameters.Add("mailto", mailto);
                    parameters.Add("maxfiles", maxfiles);
                    parameters.Add("mode", mode);
                    parameters.Add("node", node);
                    parameters.Add("notes-template", notes_template);
                    parameters.Add("notification-policy", notification_policy);
                    parameters.Add("notification-target", notification_target);
                    parameters.Add("performance", performance);
                    parameters.Add("pigz", pigz);
                    parameters.Add("pool", pool);
                    parameters.Add("protected", protected_);
                    parameters.Add("prune-backups", prune_backups);
                    parameters.Add("quiet", quiet);
                    parameters.Add("remove", remove);
                    parameters.Add("repeat-missed", repeat_missed);
                    parameters.Add("schedule", schedule);
                    parameters.Add("script", script);
                    parameters.Add("starttime", starttime);
                    parameters.Add("stdexcludes", stdexcludes);
                    parameters.Add("stop", stop);
                    parameters.Add("stopwait", stopwait);
                    parameters.Add("storage", storage);
                    parameters.Add("tmpdir", tmpdir);
                    parameters.Add("vmid", vmid);
                    parameters.Add("zstd", zstd);
                    return await _client.Create($"/cluster/backup", parameters);
                }
            }
            /// <summary>
            /// BackupInfo
            /// </summary>
            public class PveBackupInfo
            {
                private readonly PveClient _client;

                internal PveBackupInfo(PveClient client) { _client = client; }
                private PveNotBackedUp _notBackedUp;
                /// <summary>
                /// NotBackedUp
                /// </summary>
                public PveNotBackedUp NotBackedUp => _notBackedUp ??= new(_client);
                /// <summary>
                /// NotBackedUp
                /// </summary>
                public class PveNotBackedUp
                {
                    private readonly PveClient _client;

                    internal PveNotBackedUp(PveClient client) { _client = client; }
                    /// <summary>
                    /// Shows all guests which are not covered by any backup job.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetGuestsNotInBackup() { return await _client.Get($"/cluster/backup-info/not-backed-up"); }
                }
                /// <summary>
                /// Index for backup info related endpoints
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/backup-info"); }
            }
            /// <summary>
            /// Ha
            /// </summary>
            public class PveHa
            {
                private readonly PveClient _client;

                internal PveHa(PveClient client) { _client = client; }
                private PveResources _resources;
                /// <summary>
                /// Resources
                /// </summary>
                public PveResources Resources => _resources ??= new(_client);
                private PveGroups _groups;
                /// <summary>
                /// Groups
                /// </summary>
                public PveGroups Groups => _groups ??= new(_client);
                private PveStatus _status;
                /// <summary>
                /// Status
                /// </summary>
                public PveStatus Status => _status ??= new(_client);
                /// <summary>
                /// Resources
                /// </summary>
                public class PveResources
                {
                    private readonly PveClient _client;

                    internal PveResources(PveClient client) { _client = client; }
                    /// <summary>
                    /// SidItem
                    /// </summary>
                    public PveSidItem this[object sid] => new(_client, sid);
                    /// <summary>
                    /// SidItem
                    /// </summary>
                    public class PveSidItem
                    {
                        private readonly PveClient _client;
                        private readonly object _sid;
                        internal PveSidItem(PveClient client, object sid) { _client = client; _sid = sid; }
                        private PveMigrate _migrate;
                        /// <summary>
                        /// Migrate
                        /// </summary>
                        public PveMigrate Migrate => _migrate ??= new(_client, _sid);
                        private PveRelocate _relocate;
                        /// <summary>
                        /// Relocate
                        /// </summary>
                        public PveRelocate Relocate => _relocate ??= new(_client, _sid);
                        /// <summary>
                        /// Migrate
                        /// </summary>
                        public class PveMigrate
                        {
                            private readonly PveClient _client;
                            private readonly object _sid;
                            internal PveMigrate(PveClient client, object sid) { _client = client; _sid = sid; }
                            /// <summary>
                            /// Request resource migration (online) to another node.
                            /// </summary>
                            /// <param name="node">Target node.</param>
                            /// <returns></returns>
                            public async Task<Result> Migrate(string node)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("node", node);
                                return await _client.Create($"/cluster/ha/resources/{_sid}/migrate", parameters);
                            }
                        }
                        /// <summary>
                        /// Relocate
                        /// </summary>
                        public class PveRelocate
                        {
                            private readonly PveClient _client;
                            private readonly object _sid;
                            internal PveRelocate(PveClient client, object sid) { _client = client; _sid = sid; }
                            /// <summary>
                            /// Request resource relocatzion to another node. This stops the service on the old node, and restarts it on the target node.
                            /// </summary>
                            /// <param name="node">Target node.</param>
                            /// <returns></returns>
                            public async Task<Result> Relocate(string node)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("node", node);
                                return await _client.Create($"/cluster/ha/resources/{_sid}/relocate", parameters);
                            }
                        }
                        /// <summary>
                        /// Delete resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/ha/resources/{_sid}"); }
                        /// <summary>
                        /// Read resource configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Read() { return await _client.Get($"/cluster/ha/resources/{_sid}"); }
                        /// <summary>
                        /// Update resource configuration.
                        /// </summary>
                        /// <param name="comment">Description.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="group">The HA group identifier.</param>
                        /// <param name="max_relocate">Maximal number of service relocate tries when a service failes to start.</param>
                        /// <param name="max_restart">Maximal number of tries to restart the service on a node after its start failed.</param>
                        /// <param name="state">Requested resource state.
                        ///   Enum: started,stopped,enabled,disabled,ignored</param>
                        /// <returns></returns>
                        public async Task<Result> Update(string comment = null, string delete = null, string digest = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("group", group);
                            parameters.Add("max_relocate", max_relocate);
                            parameters.Add("max_restart", max_restart);
                            parameters.Add("state", state);
                            return await _client.Set($"/cluster/ha/resources/{_sid}", parameters);
                        }
                    }
                    /// <summary>
                    /// List HA resources.
                    /// </summary>
                    /// <param name="type">Only list resources of specific type
                    ///   Enum: ct,vm</param>
                    /// <returns></returns>
                    public async Task<Result> Index(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return await _client.Get($"/cluster/ha/resources", parameters);
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
                    public async Task<Result> Create(string sid, string comment = null, string group = null, int? max_relocate = null, int? max_restart = null, string state = null, string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("sid", sid);
                        parameters.Add("comment", comment);
                        parameters.Add("group", group);
                        parameters.Add("max_relocate", max_relocate);
                        parameters.Add("max_restart", max_restart);
                        parameters.Add("state", state);
                        parameters.Add("type", type);
                        return await _client.Create($"/cluster/ha/resources", parameters);
                    }
                }
                /// <summary>
                /// Groups
                /// </summary>
                public class PveGroups
                {
                    private readonly PveClient _client;

                    internal PveGroups(PveClient client) { _client = client; }
                    /// <summary>
                    /// GroupItem
                    /// </summary>
                    public PveGroupItem this[object group] => new(_client, group);
                    /// <summary>
                    /// GroupItem
                    /// </summary>
                    public class PveGroupItem
                    {
                        private readonly PveClient _client;
                        private readonly object _group;
                        internal PveGroupItem(PveClient client, object group) { _client = client; _group = group; }
                        /// <summary>
                        /// Delete ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/ha/groups/{_group}"); }
                        /// <summary>
                        /// Read ha group configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Read() { return await _client.Get($"/cluster/ha/groups/{_group}"); }
                        /// <summary>
                        /// Update ha group configuration.
                        /// </summary>
                        /// <param name="comment">Description.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="nodes">List of cluster node names with optional priority.</param>
                        /// <param name="nofailback">The CRM tries to run services on the node with the highest priority. If a node with higher priority comes online, the CRM migrates the service to that node. Enabling nofailback prevents that behavior.</param>
                        /// <param name="restricted">Resources bound to restricted groups may only run on nodes defined by the group.</param>
                        /// <returns></returns>
                        public async Task<Result> Update(string comment = null, string delete = null, string digest = null, string nodes = null, bool? nofailback = null, bool? restricted = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("nodes", nodes);
                            parameters.Add("nofailback", nofailback);
                            parameters.Add("restricted", restricted);
                            return await _client.Set($"/cluster/ha/groups/{_group}", parameters);
                        }
                    }
                    /// <summary>
                    /// Get HA groups.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/cluster/ha/groups"); }
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
                    public async Task<Result> Create(string group, string nodes, string comment = null, bool? nofailback = null, bool? restricted = null, string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("group", group);
                        parameters.Add("nodes", nodes);
                        parameters.Add("comment", comment);
                        parameters.Add("nofailback", nofailback);
                        parameters.Add("restricted", restricted);
                        parameters.Add("type", type);
                        return await _client.Create($"/cluster/ha/groups", parameters);
                    }
                }
                /// <summary>
                /// Status
                /// </summary>
                public class PveStatus
                {
                    private readonly PveClient _client;

                    internal PveStatus(PveClient client) { _client = client; }
                    private PveCurrent _current;
                    /// <summary>
                    /// Current
                    /// </summary>
                    public PveCurrent Current => _current ??= new(_client);
                    private PveManagerStatus _managerStatus;
                    /// <summary>
                    /// ManagerStatus
                    /// </summary>
                    public PveManagerStatus ManagerStatus => _managerStatus ??= new(_client);
                    /// <summary>
                    /// Current
                    /// </summary>
                    public class PveCurrent
                    {
                        private readonly PveClient _client;

                        internal PveCurrent(PveClient client) { _client = client; }
                        /// <summary>
                        /// Get HA manger status.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Status() { return await _client.Get($"/cluster/ha/status/current"); }
                    }
                    /// <summary>
                    /// ManagerStatus
                    /// </summary>
                    public class PveManagerStatus
                    {
                        private readonly PveClient _client;

                        internal PveManagerStatus(PveClient client) { _client = client; }
                        /// <summary>
                        /// Get full HA manger status, including LRM status.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> ManagerStatus() { return await _client.Get($"/cluster/ha/status/manager_status"); }
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/cluster/ha/status"); }
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/ha"); }
            }
            /// <summary>
            /// Acme
            /// </summary>
            public class PveAcme
            {
                private readonly PveClient _client;

                internal PveAcme(PveClient client) { _client = client; }
                private PvePlugins _plugins;
                /// <summary>
                /// Plugins
                /// </summary>
                public PvePlugins Plugins => _plugins ??= new(_client);
                private PveAccount _account;
                /// <summary>
                /// Account
                /// </summary>
                public PveAccount Account => _account ??= new(_client);
                private PveTos _tos;
                /// <summary>
                /// Tos
                /// </summary>
                public PveTos Tos => _tos ??= new(_client);
                private PveMeta _meta;
                /// <summary>
                /// Meta
                /// </summary>
                public PveMeta Meta => _meta ??= new(_client);
                private PveDirectories _directories;
                /// <summary>
                /// Directories
                /// </summary>
                public PveDirectories Directories => _directories ??= new(_client);
                private PveChallengeSchema _challengeSchema;
                /// <summary>
                /// ChallengeSchema
                /// </summary>
                public PveChallengeSchema ChallengeSchema => _challengeSchema ??= new(_client);
                /// <summary>
                /// Plugins
                /// </summary>
                public class PvePlugins
                {
                    private readonly PveClient _client;

                    internal PvePlugins(PveClient client) { _client = client; }
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public PveIdItem this[object id] => new(_client, id);
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public class PveIdItem
                    {
                        private readonly PveClient _client;
                        private readonly object _id;
                        internal PveIdItem(PveClient client, object id) { _client = client; _id = id; }
                        /// <summary>
                        /// Delete ACME plugin configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> DeletePlugin() { return await _client.Delete($"/cluster/acme/plugins/{_id}"); }
                        /// <summary>
                        /// Get ACME plugin configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetPluginConfig() { return await _client.Get($"/cluster/acme/plugins/{_id}"); }
                        /// <summary>
                        /// Update ACME plugin configuration.
                        /// </summary>
                        /// <param name="api">API plugin name
                        ///   Enum: 1984hosting,acmedns,acmeproxy,active24,ad,ali,anx,artfiles,arvan,aurora,autodns,aws,azion,azure,bookmyname,bunny,cf,clouddns,cloudns,cn,conoha,constellix,cpanel,curanet,cyon,da,ddnss,desec,df,dgon,dnsexit,dnshome,dnsimple,dnsservices,do,doapi,domeneshop,dp,dpi,dreamhost,duckdns,durabledns,dyn,dynu,dynv6,easydns,edgedns,euserv,exoscale,fornex,freedns,gandi_livedns,gcloud,gcore,gd,geoscaling,googledomains,he,hetzner,hexonet,hostingde,huaweicloud,infoblox,infomaniak,internetbs,inwx,ionos,ipv64,ispconfig,jd,joker,kappernet,kas,kinghost,knot,la,leaseweb,lexicon,linode,linode_v4,loopia,lua,maradns,me,miab,misaka,myapi,mydevil,mydnsjp,mythic_beasts,namecheap,namecom,namesilo,nanelo,nederhost,neodigit,netcup,netlify,nic,njalla,nm,nsd,nsone,nsupdate,nw,oci,one,online,openprovider,openstack,opnsense,ovh,pdns,pleskxml,pointhq,porkbun,rackcorp,rackspace,rage4,rcode0,regru,scaleway,schlundtech,selectel,selfhost,servercow,simply,tele3,tencent,transip,udr,ultra,unoeuro,variomedia,veesp,vercel,vscale,vultr,websupport,world4you,yandex,yc,zilore,zone,zonomi</param>
                        /// <param name="data">DNS plugin data. (base64 encoded)</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="disable">Flag to disable the config.</param>
                        /// <param name="nodes">List of cluster node names.</param>
                        /// <param name="validation_delay">Extra delay in seconds to wait before requesting validation. Allows to cope with a long TTL of DNS records.</param>
                        /// <returns></returns>
                        public async Task<Result> UpdatePlugin(string api = null, string data = null, string delete = null, string digest = null, bool? disable = null, string nodes = null, int? validation_delay = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("api", api);
                            parameters.Add("data", data);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("disable", disable);
                            parameters.Add("nodes", nodes);
                            parameters.Add("validation-delay", validation_delay);
                            return await _client.Set($"/cluster/acme/plugins/{_id}", parameters);
                        }
                    }
                    /// <summary>
                    /// ACME plugin index.
                    /// </summary>
                    /// <param name="type">Only list ACME plugins of a specific type
                    ///   Enum: dns,standalone</param>
                    /// <returns></returns>
                    public async Task<Result> Index(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return await _client.Get($"/cluster/acme/plugins", parameters);
                    }
                    /// <summary>
                    /// Add ACME plugin configuration.
                    /// </summary>
                    /// <param name="id">ACME Plugin ID name</param>
                    /// <param name="type">ACME challenge type.
                    ///   Enum: dns,standalone</param>
                    /// <param name="api">API plugin name
                    ///   Enum: 1984hosting,acmedns,acmeproxy,active24,ad,ali,anx,artfiles,arvan,aurora,autodns,aws,azion,azure,bookmyname,bunny,cf,clouddns,cloudns,cn,conoha,constellix,cpanel,curanet,cyon,da,ddnss,desec,df,dgon,dnsexit,dnshome,dnsimple,dnsservices,do,doapi,domeneshop,dp,dpi,dreamhost,duckdns,durabledns,dyn,dynu,dynv6,easydns,edgedns,euserv,exoscale,fornex,freedns,gandi_livedns,gcloud,gcore,gd,geoscaling,googledomains,he,hetzner,hexonet,hostingde,huaweicloud,infoblox,infomaniak,internetbs,inwx,ionos,ipv64,ispconfig,jd,joker,kappernet,kas,kinghost,knot,la,leaseweb,lexicon,linode,linode_v4,loopia,lua,maradns,me,miab,misaka,myapi,mydevil,mydnsjp,mythic_beasts,namecheap,namecom,namesilo,nanelo,nederhost,neodigit,netcup,netlify,nic,njalla,nm,nsd,nsone,nsupdate,nw,oci,one,online,openprovider,openstack,opnsense,ovh,pdns,pleskxml,pointhq,porkbun,rackcorp,rackspace,rage4,rcode0,regru,scaleway,schlundtech,selectel,selfhost,servercow,simply,tele3,tencent,transip,udr,ultra,unoeuro,variomedia,veesp,vercel,vscale,vultr,websupport,world4you,yandex,yc,zilore,zone,zonomi</param>
                    /// <param name="data">DNS plugin data. (base64 encoded)</param>
                    /// <param name="disable">Flag to disable the config.</param>
                    /// <param name="nodes">List of cluster node names.</param>
                    /// <param name="validation_delay">Extra delay in seconds to wait before requesting validation. Allows to cope with a long TTL of DNS records.</param>
                    /// <returns></returns>
                    public async Task<Result> AddPlugin(string id, string type, string api = null, string data = null, bool? disable = null, string nodes = null, int? validation_delay = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("id", id);
                        parameters.Add("type", type);
                        parameters.Add("api", api);
                        parameters.Add("data", data);
                        parameters.Add("disable", disable);
                        parameters.Add("nodes", nodes);
                        parameters.Add("validation-delay", validation_delay);
                        return await _client.Create($"/cluster/acme/plugins", parameters);
                    }
                }
                /// <summary>
                /// Account
                /// </summary>
                public class PveAccount
                {
                    private readonly PveClient _client;

                    internal PveAccount(PveClient client) { _client = client; }
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public PveNameItem this[object name] => new(_client, name);
                    /// <summary>
                    /// NameItem
                    /// </summary>
                    public class PveNameItem
                    {
                        private readonly PveClient _client;
                        private readonly object _name;
                        internal PveNameItem(PveClient client, object name) { _client = client; _name = name; }
                        /// <summary>
                        /// Deactivate existing ACME account at CA.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> DeactivateAccount() { return await _client.Delete($"/cluster/acme/account/{_name}"); }
                        /// <summary>
                        /// Return existing ACME account information.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetAccount() { return await _client.Get($"/cluster/acme/account/{_name}"); }
                        /// <summary>
                        /// Update existing ACME account information with CA. Note: not specifying any new account information triggers a refresh.
                        /// </summary>
                        /// <param name="contact">Contact email addresses.</param>
                        /// <returns></returns>
                        public async Task<Result> UpdateAccount(string contact = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("contact", contact);
                            return await _client.Set($"/cluster/acme/account/{_name}", parameters);
                        }
                    }
                    /// <summary>
                    /// ACMEAccount index.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> AccountIndex() { return await _client.Get($"/cluster/acme/account"); }
                    /// <summary>
                    /// Register a new ACME account with CA.
                    /// </summary>
                    /// <param name="contact">Contact email addresses.</param>
                    /// <param name="directory">URL of ACME CA directory endpoint.</param>
                    /// <param name="eab_hmac_key">HMAC key for External Account Binding.</param>
                    /// <param name="eab_kid">Key Identifier for External Account Binding.</param>
                    /// <param name="name">ACME account config file name.</param>
                    /// <param name="tos_url">URL of CA TermsOfService - setting this indicates agreement.</param>
                    /// <returns></returns>
                    public async Task<Result> RegisterAccount(string contact, string directory = null, string eab_hmac_key = null, string eab_kid = null, string name = null, string tos_url = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("contact", contact);
                        parameters.Add("directory", directory);
                        parameters.Add("eab-hmac-key", eab_hmac_key);
                        parameters.Add("eab-kid", eab_kid);
                        parameters.Add("name", name);
                        parameters.Add("tos_url", tos_url);
                        return await _client.Create($"/cluster/acme/account", parameters);
                    }
                }
                /// <summary>
                /// Tos
                /// </summary>
                public class PveTos
                {
                    private readonly PveClient _client;

                    internal PveTos(PveClient client) { _client = client; }
                    /// <summary>
                    /// Retrieve ACME TermsOfService URL from CA. Deprecated, please use /cluster/acme/meta.
                    /// </summary>
                    /// <param name="directory">URL of ACME CA directory endpoint.</param>
                    /// <returns></returns>
                    public async Task<Result> GetTos(string directory = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("directory", directory);
                        return await _client.Get($"/cluster/acme/tos", parameters);
                    }
                }
                /// <summary>
                /// Meta
                /// </summary>
                public class PveMeta
                {
                    private readonly PveClient _client;

                    internal PveMeta(PveClient client) { _client = client; }
                    /// <summary>
                    /// Retrieve ACME Directory Meta Information
                    /// </summary>
                    /// <param name="directory">URL of ACME CA directory endpoint.</param>
                    /// <returns></returns>
                    public async Task<Result> GetMeta(string directory = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("directory", directory);
                        return await _client.Get($"/cluster/acme/meta", parameters);
                    }
                }
                /// <summary>
                /// Directories
                /// </summary>
                public class PveDirectories
                {
                    private readonly PveClient _client;

                    internal PveDirectories(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get named known ACME directory endpoints.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetDirectories() { return await _client.Get($"/cluster/acme/directories"); }
                }
                /// <summary>
                /// ChallengeSchema
                /// </summary>
                public class PveChallengeSchema
                {
                    private readonly PveClient _client;

                    internal PveChallengeSchema(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get schema of ACME challenge types.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Challengeschema() { return await _client.Get($"/cluster/acme/challenge-schema"); }
                }
                /// <summary>
                /// ACMEAccount index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/acme"); }
            }
            /// <summary>
            /// Ceph
            /// </summary>
            public class PveCeph
            {
                private readonly PveClient _client;

                internal PveCeph(PveClient client) { _client = client; }
                private PveMetadata _metadata;
                /// <summary>
                /// Metadata
                /// </summary>
                public PveMetadata Metadata => _metadata ??= new(_client);
                private PveStatus _status;
                /// <summary>
                /// Status
                /// </summary>
                public PveStatus Status => _status ??= new(_client);
                private PveFlags _flags;
                /// <summary>
                /// Flags
                /// </summary>
                public PveFlags Flags => _flags ??= new(_client);
                /// <summary>
                /// Metadata
                /// </summary>
                public class PveMetadata
                {
                    private readonly PveClient _client;

                    internal PveMetadata(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get ceph metadata.
                    /// </summary>
                    /// <param name="scope">
                    ///   Enum: all,versions</param>
                    /// <returns></returns>
                    public async Task<Result> Metadata(string scope = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("scope", scope);
                        return await _client.Get($"/cluster/ceph/metadata", parameters);
                    }
                }
                /// <summary>
                /// Status
                /// </summary>
                public class PveStatus
                {
                    private readonly PveClient _client;

                    internal PveStatus(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get ceph status.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Status() { return await _client.Get($"/cluster/ceph/status"); }
                }
                /// <summary>
                /// Flags
                /// </summary>
                public class PveFlags
                {
                    private readonly PveClient _client;

                    internal PveFlags(PveClient client) { _client = client; }
                    /// <summary>
                    /// FlagItem
                    /// </summary>
                    public PveFlagItem this[object flag] => new(_client, flag);
                    /// <summary>
                    /// FlagItem
                    /// </summary>
                    public class PveFlagItem
                    {
                        private readonly PveClient _client;
                        private readonly object _flag;
                        internal PveFlagItem(PveClient client, object flag) { _client = client; _flag = flag; }
                        /// <summary>
                        /// Get the status of a specific ceph flag.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetFlag() { return await _client.Get($"/cluster/ceph/flags/{_flag}"); }
                        /// <summary>
                        /// Set or clear (unset) a specific ceph flag
                        /// </summary>
                        /// <param name="value">The new value of the flag</param>
                        /// <returns></returns>
                        public async Task<Result> UpdateFlag(bool value)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("value", value);
                            return await _client.Set($"/cluster/ceph/flags/{_flag}", parameters);
                        }
                    }
                    /// <summary>
                    /// get the status of all ceph flags
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetAllFlags() { return await _client.Get($"/cluster/ceph/flags"); }
                    /// <summary>
                    /// Set/Unset multiple ceph flags at once.
                    /// </summary>
                    /// <param name="nobackfill">Backfilling of PGs is suspended.</param>
                    /// <param name="nodeep_scrub">Deep Scrubbing is disabled.</param>
                    /// <param name="nodown">OSD failure reports are being ignored, such that the monitors will not mark OSDs down.</param>
                    /// <param name="noin">OSDs that were previously marked out will not be marked back in when they start.</param>
                    /// <param name="noout">OSDs will not automatically be marked out after the configured interval.</param>
                    /// <param name="norebalance">Rebalancing of PGs is suspended.</param>
                    /// <param name="norecover">Recovery of PGs is suspended.</param>
                    /// <param name="noscrub">Scrubbing is disabled.</param>
                    /// <param name="notieragent">Cache tiering activity is suspended.</param>
                    /// <param name="noup">OSDs are not allowed to start.</param>
                    /// <param name="pause">Pauses read and writes.</param>
                    /// <returns></returns>
                    public async Task<Result> SetFlags(bool? nobackfill = null, bool? nodeep_scrub = null, bool? nodown = null, bool? noin = null, bool? noout = null, bool? norebalance = null, bool? norecover = null, bool? noscrub = null, bool? notieragent = null, bool? noup = null, bool? pause = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("nobackfill", nobackfill);
                        parameters.Add("nodeep-scrub", nodeep_scrub);
                        parameters.Add("nodown", nodown);
                        parameters.Add("noin", noin);
                        parameters.Add("noout", noout);
                        parameters.Add("norebalance", norebalance);
                        parameters.Add("norecover", norecover);
                        parameters.Add("noscrub", noscrub);
                        parameters.Add("notieragent", notieragent);
                        parameters.Add("noup", noup);
                        parameters.Add("pause", pause);
                        return await _client.Set($"/cluster/ceph/flags", parameters);
                    }
                }
                /// <summary>
                /// Cluster ceph index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Cephindex() { return await _client.Get($"/cluster/ceph"); }
            }
            /// <summary>
            /// Jobs
            /// </summary>
            public class PveJobs
            {
                private readonly PveClient _client;

                internal PveJobs(PveClient client) { _client = client; }
                private PveRealmSync _realmSync;
                /// <summary>
                /// RealmSync
                /// </summary>
                public PveRealmSync RealmSync => _realmSync ??= new(_client);
                private PveScheduleAnalyze _scheduleAnalyze;
                /// <summary>
                /// ScheduleAnalyze
                /// </summary>
                public PveScheduleAnalyze ScheduleAnalyze => _scheduleAnalyze ??= new(_client);
                /// <summary>
                /// RealmSync
                /// </summary>
                public class PveRealmSync
                {
                    private readonly PveClient _client;

                    internal PveRealmSync(PveClient client) { _client = client; }
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public PveIdItem this[object id] => new(_client, id);
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public class PveIdItem
                    {
                        private readonly PveClient _client;
                        private readonly object _id;
                        internal PveIdItem(PveClient client, object id) { _client = client; _id = id; }
                        /// <summary>
                        /// Delete realm-sync job definition.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> DeleteJob() { return await _client.Delete($"/cluster/jobs/realm-sync/{_id}"); }
                        /// <summary>
                        /// Read realm-sync job definition.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> ReadJob() { return await _client.Get($"/cluster/jobs/realm-sync/{_id}"); }
                        /// <summary>
                        /// Create new realm-sync job.
                        /// </summary>
                        /// <param name="schedule">Backup schedule. The format is a subset of `systemd` calendar events.</param>
                        /// <param name="comment">Description for the Job.</param>
                        /// <param name="enable_new">Enable newly synced users immediately.</param>
                        /// <param name="enabled">Determines if the job is enabled.</param>
                        /// <param name="realm">Authentication domain ID</param>
                        /// <param name="remove_vanished">A semicolon-seperated list of things to remove when they or the user vanishes during a sync. The following values are possible: 'entry' removes the user/group when not returned from the sync. 'properties' removes the set properties on existing user/group that do not appear in the source (even custom ones). 'acl' removes acls when the user/group is not returned from the sync. Instead of a list it also can be 'none' (the default).</param>
                        /// <param name="scope">Select what to sync.
                        ///   Enum: users,groups,both</param>
                        /// <returns></returns>
                        public async Task<Result> CreateJob(string schedule, string comment = null, bool? enable_new = null, bool? enabled = null, string realm = null, string remove_vanished = null, string scope = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("schedule", schedule);
                            parameters.Add("comment", comment);
                            parameters.Add("enable-new", enable_new);
                            parameters.Add("enabled", enabled);
                            parameters.Add("realm", realm);
                            parameters.Add("remove-vanished", remove_vanished);
                            parameters.Add("scope", scope);
                            return await _client.Create($"/cluster/jobs/realm-sync/{_id}", parameters);
                        }
                        /// <summary>
                        /// Update realm-sync job definition.
                        /// </summary>
                        /// <param name="schedule">Backup schedule. The format is a subset of `systemd` calendar events.</param>
                        /// <param name="comment">Description for the Job.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="enable_new">Enable newly synced users immediately.</param>
                        /// <param name="enabled">Determines if the job is enabled.</param>
                        /// <param name="remove_vanished">A semicolon-seperated list of things to remove when they or the user vanishes during a sync. The following values are possible: 'entry' removes the user/group when not returned from the sync. 'properties' removes the set properties on existing user/group that do not appear in the source (even custom ones). 'acl' removes acls when the user/group is not returned from the sync. Instead of a list it also can be 'none' (the default).</param>
                        /// <param name="scope">Select what to sync.
                        ///   Enum: users,groups,both</param>
                        /// <returns></returns>
                        public async Task<Result> UpdateJob(string schedule, string comment = null, string delete = null, bool? enable_new = null, bool? enabled = null, string remove_vanished = null, string scope = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("schedule", schedule);
                            parameters.Add("comment", comment);
                            parameters.Add("delete", delete);
                            parameters.Add("enable-new", enable_new);
                            parameters.Add("enabled", enabled);
                            parameters.Add("remove-vanished", remove_vanished);
                            parameters.Add("scope", scope);
                            return await _client.Set($"/cluster/jobs/realm-sync/{_id}", parameters);
                        }
                    }
                    /// <summary>
                    /// List configured realm-sync-jobs.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> SyncjobIndex() { return await _client.Get($"/cluster/jobs/realm-sync"); }
                }
                /// <summary>
                /// ScheduleAnalyze
                /// </summary>
                public class PveScheduleAnalyze
                {
                    private readonly PveClient _client;

                    internal PveScheduleAnalyze(PveClient client) { _client = client; }
                    /// <summary>
                    /// Returns a list of future schedule runtimes.
                    /// </summary>
                    /// <param name="schedule">Job schedule. The format is a subset of `systemd` calendar events.</param>
                    /// <param name="iterations">Number of event-iteration to simulate and return.</param>
                    /// <param name="starttime">UNIX timestamp to start the calculation from. Defaults to the current time.</param>
                    /// <returns></returns>
                    public async Task<Result> ScheduleAnalyze(string schedule, int? iterations = null, int? starttime = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("schedule", schedule);
                        parameters.Add("iterations", iterations);
                        parameters.Add("starttime", starttime);
                        return await _client.Get($"/cluster/jobs/schedule-analyze", parameters);
                    }
                }
                /// <summary>
                /// Index for jobs related endpoints.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/jobs"); }
            }
            /// <summary>
            /// Mapping
            /// </summary>
            public class PveMapping
            {
                private readonly PveClient _client;

                internal PveMapping(PveClient client) { _client = client; }
                private PvePci _pci;
                /// <summary>
                /// Pci
                /// </summary>
                public PvePci Pci => _pci ??= new(_client);
                private PveUsb _usb;
                /// <summary>
                /// Usb
                /// </summary>
                public PveUsb Usb => _usb ??= new(_client);
                /// <summary>
                /// Pci
                /// </summary>
                public class PvePci
                {
                    private readonly PveClient _client;

                    internal PvePci(PveClient client) { _client = client; }
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public PveIdItem this[object id] => new(_client, id);
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public class PveIdItem
                    {
                        private readonly PveClient _client;
                        private readonly object _id;
                        internal PveIdItem(PveClient client, object id) { _client = client; _id = id; }
                        /// <summary>
                        /// Remove Hardware Mapping.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/mapping/pci/{_id}"); }
                        /// <summary>
                        /// Get PCI Mapping.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Get() { return await _client.Get($"/cluster/mapping/pci/{_id}"); }
                        /// <summary>
                        /// Update a hardware mapping.
                        /// </summary>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="description">Description of the logical PCI device.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="map">A list of maps for the cluster nodes.</param>
                        /// <param name="mdev"></param>
                        /// <returns></returns>
                        public async Task<Result> Update(string delete = null, string description = null, string digest = null, string map = null, bool? mdev = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("delete", delete);
                            parameters.Add("description", description);
                            parameters.Add("digest", digest);
                            parameters.Add("map", map);
                            parameters.Add("mdev", mdev);
                            return await _client.Set($"/cluster/mapping/pci/{_id}", parameters);
                        }
                    }
                    /// <summary>
                    /// List PCI Hardware Mapping
                    /// </summary>
                    /// <param name="check_node">If given, checks the configurations on the given node for correctness, and adds relevant diagnostics for the devices to the response.</param>
                    /// <returns></returns>
                    public async Task<Result> Index(string check_node = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("check-node", check_node);
                        return await _client.Get($"/cluster/mapping/pci", parameters);
                    }
                    /// <summary>
                    /// Create a new hardware mapping.
                    /// </summary>
                    /// <param name="id">The ID of the logical PCI mapping.</param>
                    /// <param name="map">A list of maps for the cluster nodes.</param>
                    /// <param name="description">Description of the logical PCI device.</param>
                    /// <param name="mdev"></param>
                    /// <returns></returns>
                    public async Task<Result> Create(string id, string map, string description = null, bool? mdev = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("id", id);
                        parameters.Add("map", map);
                        parameters.Add("description", description);
                        parameters.Add("mdev", mdev);
                        return await _client.Create($"/cluster/mapping/pci", parameters);
                    }
                }
                /// <summary>
                /// Usb
                /// </summary>
                public class PveUsb
                {
                    private readonly PveClient _client;

                    internal PveUsb(PveClient client) { _client = client; }
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public PveIdItem this[object id] => new(_client, id);
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public class PveIdItem
                    {
                        private readonly PveClient _client;
                        private readonly object _id;
                        internal PveIdItem(PveClient client, object id) { _client = client; _id = id; }
                        /// <summary>
                        /// Remove Hardware Mapping.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/mapping/usb/{_id}"); }
                        /// <summary>
                        /// Get USB Mapping.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Get() { return await _client.Get($"/cluster/mapping/usb/{_id}"); }
                        /// <summary>
                        /// Update a hardware mapping.
                        /// </summary>
                        /// <param name="map">A list of maps for the cluster nodes.</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="description">Description of the logical USB device.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <returns></returns>
                        public async Task<Result> Update(string map, string delete = null, string description = null, string digest = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("map", map);
                            parameters.Add("delete", delete);
                            parameters.Add("description", description);
                            parameters.Add("digest", digest);
                            return await _client.Set($"/cluster/mapping/usb/{_id}", parameters);
                        }
                    }
                    /// <summary>
                    /// List USB Hardware Mappings
                    /// </summary>
                    /// <param name="check_node">If given, checks the configurations on the given node for correctness, and adds relevant errors to the devices.</param>
                    /// <returns></returns>
                    public async Task<Result> Index(string check_node = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("check-node", check_node);
                        return await _client.Get($"/cluster/mapping/usb", parameters);
                    }
                    /// <summary>
                    /// Create a new hardware mapping.
                    /// </summary>
                    /// <param name="id">The ID of the logical USB mapping.</param>
                    /// <param name="map">A list of maps for the cluster nodes.</param>
                    /// <param name="description">Description of the logical USB device.</param>
                    /// <returns></returns>
                    public async Task<Result> Create(string id, string map, string description = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("id", id);
                        parameters.Add("map", map);
                        parameters.Add("description", description);
                        return await _client.Create($"/cluster/mapping/usb", parameters);
                    }
                }
                /// <summary>
                /// List resource types.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/mapping"); }
            }
            /// <summary>
            /// Sdn
            /// </summary>
            public class PveSdn
            {
                private readonly PveClient _client;

                internal PveSdn(PveClient client) { _client = client; }
                private PveVnets _vnets;
                /// <summary>
                /// Vnets
                /// </summary>
                public PveVnets Vnets => _vnets ??= new(_client);
                private PveZones _zones;
                /// <summary>
                /// Zones
                /// </summary>
                public PveZones Zones => _zones ??= new(_client);
                private PveControllers _controllers;
                /// <summary>
                /// Controllers
                /// </summary>
                public PveControllers Controllers => _controllers ??= new(_client);
                private PveIpams _ipams;
                /// <summary>
                /// Ipams
                /// </summary>
                public PveIpams Ipams => _ipams ??= new(_client);
                private PveDns _dns;
                /// <summary>
                /// Dns
                /// </summary>
                public PveDns Dns => _dns ??= new(_client);
                /// <summary>
                /// Vnets
                /// </summary>
                public class PveVnets
                {
                    private readonly PveClient _client;

                    internal PveVnets(PveClient client) { _client = client; }
                    /// <summary>
                    /// VnetItem
                    /// </summary>
                    public PveVnetItem this[object vnet] => new(_client, vnet);
                    /// <summary>
                    /// VnetItem
                    /// </summary>
                    public class PveVnetItem
                    {
                        private readonly PveClient _client;
                        private readonly object _vnet;
                        internal PveVnetItem(PveClient client, object vnet) { _client = client; _vnet = vnet; }
                        private PveSubnets _subnets;
                        /// <summary>
                        /// Subnets
                        /// </summary>
                        public PveSubnets Subnets => _subnets ??= new(_client, _vnet);
                        private PveIps _ips;
                        /// <summary>
                        /// Ips
                        /// </summary>
                        public PveIps Ips => _ips ??= new(_client, _vnet);
                        /// <summary>
                        /// Subnets
                        /// </summary>
                        public class PveSubnets
                        {
                            private readonly PveClient _client;
                            private readonly object _vnet;
                            internal PveSubnets(PveClient client, object vnet) { _client = client; _vnet = vnet; }
                            /// <summary>
                            /// SubnetItem
                            /// </summary>
                            public PveSubnetItem this[object subnet] => new(_client, _vnet, subnet);
                            /// <summary>
                            /// SubnetItem
                            /// </summary>
                            public class PveSubnetItem
                            {
                                private readonly PveClient _client;
                                private readonly object _vnet;
                                private readonly object _subnet;
                                internal PveSubnetItem(PveClient client, object vnet, object subnet)
                                {
                                    _client = client; _vnet = vnet;
                                    _subnet = subnet;
                                }
                                /// <summary>
                                /// Delete sdn subnet object configuration.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Delete() { return await _client.Delete($"/cluster/sdn/vnets/{_vnet}/subnets/{_subnet}"); }
                                /// <summary>
                                /// Read sdn subnet configuration.
                                /// </summary>
                                /// <param name="pending">Display pending config.</param>
                                /// <param name="running">Display running config.</param>
                                /// <returns></returns>
                                public async Task<Result> Read(bool? pending = null, bool? running = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("pending", pending);
                                    parameters.Add("running", running);
                                    return await _client.Get($"/cluster/sdn/vnets/{_vnet}/subnets/{_subnet}", parameters);
                                }
                                /// <summary>
                                /// Update sdn subnet object configuration.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp_dns_server">IP address for the DNS server</param>
                                /// <param name="dhcp_range">A list of DHCP ranges for this subnet</param>
                                /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="dnszoneprefix">dns domain zone prefix  ex: 'adm' -&amp;gt; &amp;lt;hostname&amp;gt;.adm.mydomain.com</param>
                                /// <param name="gateway">Subnet Gateway: Will be assign on vnet for layer3 zones</param>
                                /// <param name="snat">enable masquerade for this subnet if pve-firewall</param>
                                /// <returns></returns>
                                public async Task<Result> Update(string delete = null, string dhcp_dns_server = null, string dhcp_range = null, string digest = null, string dnszoneprefix = null, string gateway = null, bool? snat = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("delete", delete);
                                    parameters.Add("dhcp-dns-server", dhcp_dns_server);
                                    parameters.Add("dhcp-range", dhcp_range);
                                    parameters.Add("digest", digest);
                                    parameters.Add("dnszoneprefix", dnszoneprefix);
                                    parameters.Add("gateway", gateway);
                                    parameters.Add("snat", snat);
                                    return await _client.Set($"/cluster/sdn/vnets/{_vnet}/subnets/{_subnet}", parameters);
                                }
                            }
                            /// <summary>
                            /// SDN subnets index.
                            /// </summary>
                            /// <param name="pending">Display pending config.</param>
                            /// <param name="running">Display running config.</param>
                            /// <returns></returns>
                            public async Task<Result> Index(bool? pending = null, bool? running = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("pending", pending);
                                parameters.Add("running", running);
                                return await _client.Get($"/cluster/sdn/vnets/{_vnet}/subnets", parameters);
                            }
                            /// <summary>
                            /// Create a new sdn subnet object.
                            /// </summary>
                            /// <param name="subnet">The SDN subnet object identifier.</param>
                            /// <param name="type">
                            ///   Enum: subnet</param>
                            /// <param name="dhcp_dns_server">IP address for the DNS server</param>
                            /// <param name="dhcp_range">A list of DHCP ranges for this subnet</param>
                            /// <param name="dnszoneprefix">dns domain zone prefix  ex: 'adm' -&amp;gt; &amp;lt;hostname&amp;gt;.adm.mydomain.com</param>
                            /// <param name="gateway">Subnet Gateway: Will be assign on vnet for layer3 zones</param>
                            /// <param name="snat">enable masquerade for this subnet if pve-firewall</param>
                            /// <returns></returns>
                            public async Task<Result> Create(string subnet, string type, string dhcp_dns_server = null, string dhcp_range = null, string dnszoneprefix = null, string gateway = null, bool? snat = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("subnet", subnet);
                                parameters.Add("type", type);
                                parameters.Add("dhcp-dns-server", dhcp_dns_server);
                                parameters.Add("dhcp-range", dhcp_range);
                                parameters.Add("dnszoneprefix", dnszoneprefix);
                                parameters.Add("gateway", gateway);
                                parameters.Add("snat", snat);
                                return await _client.Create($"/cluster/sdn/vnets/{_vnet}/subnets", parameters);
                            }
                        }
                        /// <summary>
                        /// Ips
                        /// </summary>
                        public class PveIps
                        {
                            private readonly PveClient _client;
                            private readonly object _vnet;
                            internal PveIps(PveClient client, object vnet) { _client = client; _vnet = vnet; }
                            /// <summary>
                            /// Delete IP Mappings in a VNet
                            /// </summary>
                            /// <param name="ip">The IP address to delete</param>
                            /// <param name="zone">The SDN zone object identifier.</param>
                            /// <param name="mac">Unicast MAC address.</param>
                            /// <returns></returns>
                            public async Task<Result> Ipdelete(string ip, string zone, string mac = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ip", ip);
                                parameters.Add("zone", zone);
                                parameters.Add("mac", mac);
                                return await _client.Delete($"/cluster/sdn/vnets/{_vnet}/ips", parameters);
                            }
                            /// <summary>
                            /// Create IP Mapping in a VNet
                            /// </summary>
                            /// <param name="ip">The IP address to associate with the given MAC address</param>
                            /// <param name="zone">The SDN zone object identifier.</param>
                            /// <param name="mac">Unicast MAC address.</param>
                            /// <returns></returns>
                            public async Task<Result> Ipcreate(string ip, string zone, string mac = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ip", ip);
                                parameters.Add("zone", zone);
                                parameters.Add("mac", mac);
                                return await _client.Create($"/cluster/sdn/vnets/{_vnet}/ips", parameters);
                            }
                            /// <summary>
                            /// Update IP Mapping in a VNet
                            /// </summary>
                            /// <param name="ip">The IP address to associate with the given MAC address</param>
                            /// <param name="zone">The SDN zone object identifier.</param>
                            /// <param name="mac">Unicast MAC address.</param>
                            /// <param name="vmid">The (unique) ID of the VM.</param>
                            /// <returns></returns>
                            public async Task<Result> Ipupdate(string ip, string zone, string mac = null, int? vmid = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ip", ip);
                                parameters.Add("zone", zone);
                                parameters.Add("mac", mac);
                                parameters.Add("vmid", vmid);
                                return await _client.Set($"/cluster/sdn/vnets/{_vnet}/ips", parameters);
                            }
                        }
                        /// <summary>
                        /// Delete sdn vnet object configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/sdn/vnets/{_vnet}"); }
                        /// <summary>
                        /// Read sdn vnet configuration.
                        /// </summary>
                        /// <param name="pending">Display pending config.</param>
                        /// <param name="running">Display running config.</param>
                        /// <returns></returns>
                        public async Task<Result> Read(bool? pending = null, bool? running = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("pending", pending);
                            parameters.Add("running", running);
                            return await _client.Get($"/cluster/sdn/vnets/{_vnet}", parameters);
                        }
                        /// <summary>
                        /// Update sdn vnet object configuration.
                        /// </summary>
                        /// <param name="alias">alias name of the vnet</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="tag">vlan or vxlan id</param>
                        /// <param name="vlanaware">Allow vm VLANs to pass through this vnet.</param>
                        /// <param name="zone">zone id</param>
                        /// <returns></returns>
                        public async Task<Result> Update(string alias = null, string delete = null, string digest = null, int? tag = null, bool? vlanaware = null, string zone = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("alias", alias);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("tag", tag);
                            parameters.Add("vlanaware", vlanaware);
                            parameters.Add("zone", zone);
                            return await _client.Set($"/cluster/sdn/vnets/{_vnet}", parameters);
                        }
                    }
                    /// <summary>
                    /// SDN vnets index.
                    /// </summary>
                    /// <param name="pending">Display pending config.</param>
                    /// <param name="running">Display running config.</param>
                    /// <returns></returns>
                    public async Task<Result> Index(bool? pending = null, bool? running = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("pending", pending);
                        parameters.Add("running", running);
                        return await _client.Get($"/cluster/sdn/vnets", parameters);
                    }
                    /// <summary>
                    /// Create a new sdn vnet object.
                    /// </summary>
                    /// <param name="vnet">The SDN vnet object identifier.</param>
                    /// <param name="zone">zone id</param>
                    /// <param name="alias">alias name of the vnet</param>
                    /// <param name="tag">vlan or vxlan id</param>
                    /// <param name="type">Type
                    ///   Enum: vnet</param>
                    /// <param name="vlanaware">Allow vm VLANs to pass through this vnet.</param>
                    /// <returns></returns>
                    public async Task<Result> Create(string vnet, string zone, string alias = null, int? tag = null, string type = null, bool? vlanaware = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("vnet", vnet);
                        parameters.Add("zone", zone);
                        parameters.Add("alias", alias);
                        parameters.Add("tag", tag);
                        parameters.Add("type", type);
                        parameters.Add("vlanaware", vlanaware);
                        return await _client.Create($"/cluster/sdn/vnets", parameters);
                    }
                }
                /// <summary>
                /// Zones
                /// </summary>
                public class PveZones
                {
                    private readonly PveClient _client;

                    internal PveZones(PveClient client) { _client = client; }
                    /// <summary>
                    /// ZoneItem
                    /// </summary>
                    public PveZoneItem this[object zone] => new(_client, zone);
                    /// <summary>
                    /// ZoneItem
                    /// </summary>
                    public class PveZoneItem
                    {
                        private readonly PveClient _client;
                        private readonly object _zone;
                        internal PveZoneItem(PveClient client, object zone) { _client = client; _zone = zone; }
                        /// <summary>
                        /// Delete sdn zone object configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/sdn/zones/{_zone}"); }
                        /// <summary>
                        /// Read sdn zone configuration.
                        /// </summary>
                        /// <param name="pending">Display pending config.</param>
                        /// <param name="running">Display running config.</param>
                        /// <returns></returns>
                        public async Task<Result> Read(bool? pending = null, bool? running = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("pending", pending);
                            parameters.Add("running", running);
                            return await _client.Get($"/cluster/sdn/zones/{_zone}", parameters);
                        }
                        /// <summary>
                        /// Update sdn zone object configuration.
                        /// </summary>
                        /// <param name="advertise_subnets">Advertise evpn subnets if you have silent hosts</param>
                        /// <param name="bridge"></param>
                        /// <param name="bridge_disable_mac_learning">Disable auto mac learning.</param>
                        /// <param name="controller">Frr router name</param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="dhcp">Type of the DHCP backend for this zone
                        ///   Enum: dnsmasq</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="disable_arp_nd_suppression">Disable ipv4 arp &amp;&amp; ipv6 neighbour discovery suppression</param>
                        /// <param name="dns">dns api server</param>
                        /// <param name="dnszone">dns domain zone  ex: mydomain.com</param>
                        /// <param name="dp_id">Faucet dataplane id</param>
                        /// <param name="exitnodes">List of cluster node names.</param>
                        /// <param name="exitnodes_local_routing">Allow exitnodes to connect to evpn guests</param>
                        /// <param name="exitnodes_primary">Force traffic to this exitnode first.</param>
                        /// <param name="ipam">use a specific ipam</param>
                        /// <param name="mac">Anycast logical router mac address</param>
                        /// <param name="mtu">MTU</param>
                        /// <param name="nodes">List of cluster node names.</param>
                        /// <param name="peers">peers address list.</param>
                        /// <param name="reversedns">reverse dns api server</param>
                        /// <param name="rt_import">Route-Target import</param>
                        /// <param name="tag">Service-VLAN Tag</param>
                        /// <param name="vlan_protocol">
                        ///   Enum: 802.1q,802.1ad</param>
                        /// <param name="vrf_vxlan">l3vni.</param>
                        /// <param name="vxlan_port">Vxlan tunnel udp port (default 4789).</param>
                        /// <returns></returns>
                        public async Task<Result> Update(bool? advertise_subnets = null, string bridge = null, bool? bridge_disable_mac_learning = null, string controller = null, string delete = null, string dhcp = null, string digest = null, bool? disable_arp_nd_suppression = null, string dns = null, string dnszone = null, int? dp_id = null, string exitnodes = null, bool? exitnodes_local_routing = null, string exitnodes_primary = null, string ipam = null, string mac = null, int? mtu = null, string nodes = null, string peers = null, string reversedns = null, string rt_import = null, int? tag = null, string vlan_protocol = null, int? vrf_vxlan = null, int? vxlan_port = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("advertise-subnets", advertise_subnets);
                            parameters.Add("bridge", bridge);
                            parameters.Add("bridge-disable-mac-learning", bridge_disable_mac_learning);
                            parameters.Add("controller", controller);
                            parameters.Add("delete", delete);
                            parameters.Add("dhcp", dhcp);
                            parameters.Add("digest", digest);
                            parameters.Add("disable-arp-nd-suppression", disable_arp_nd_suppression);
                            parameters.Add("dns", dns);
                            parameters.Add("dnszone", dnszone);
                            parameters.Add("dp-id", dp_id);
                            parameters.Add("exitnodes", exitnodes);
                            parameters.Add("exitnodes-local-routing", exitnodes_local_routing);
                            parameters.Add("exitnodes-primary", exitnodes_primary);
                            parameters.Add("ipam", ipam);
                            parameters.Add("mac", mac);
                            parameters.Add("mtu", mtu);
                            parameters.Add("nodes", nodes);
                            parameters.Add("peers", peers);
                            parameters.Add("reversedns", reversedns);
                            parameters.Add("rt-import", rt_import);
                            parameters.Add("tag", tag);
                            parameters.Add("vlan-protocol", vlan_protocol);
                            parameters.Add("vrf-vxlan", vrf_vxlan);
                            parameters.Add("vxlan-port", vxlan_port);
                            return await _client.Set($"/cluster/sdn/zones/{_zone}", parameters);
                        }
                    }
                    /// <summary>
                    /// SDN zones index.
                    /// </summary>
                    /// <param name="pending">Display pending config.</param>
                    /// <param name="running">Display running config.</param>
                    /// <param name="type">Only list SDN zones of specific type
                    ///   Enum: evpn,faucet,qinq,simple,vlan,vxlan</param>
                    /// <returns></returns>
                    public async Task<Result> Index(bool? pending = null, bool? running = null, string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("pending", pending);
                        parameters.Add("running", running);
                        parameters.Add("type", type);
                        return await _client.Get($"/cluster/sdn/zones", parameters);
                    }
                    /// <summary>
                    /// Create a new sdn zone object.
                    /// </summary>
                    /// <param name="type">Plugin type.
                    ///   Enum: evpn,faucet,qinq,simple,vlan,vxlan</param>
                    /// <param name="zone">The SDN zone object identifier.</param>
                    /// <param name="advertise_subnets">Advertise evpn subnets if you have silent hosts</param>
                    /// <param name="bridge"></param>
                    /// <param name="bridge_disable_mac_learning">Disable auto mac learning.</param>
                    /// <param name="controller">Frr router name</param>
                    /// <param name="dhcp">Type of the DHCP backend for this zone
                    ///   Enum: dnsmasq</param>
                    /// <param name="disable_arp_nd_suppression">Disable ipv4 arp &amp;&amp; ipv6 neighbour discovery suppression</param>
                    /// <param name="dns">dns api server</param>
                    /// <param name="dnszone">dns domain zone  ex: mydomain.com</param>
                    /// <param name="dp_id">Faucet dataplane id</param>
                    /// <param name="exitnodes">List of cluster node names.</param>
                    /// <param name="exitnodes_local_routing">Allow exitnodes to connect to evpn guests</param>
                    /// <param name="exitnodes_primary">Force traffic to this exitnode first.</param>
                    /// <param name="ipam">use a specific ipam</param>
                    /// <param name="mac">Anycast logical router mac address</param>
                    /// <param name="mtu">MTU</param>
                    /// <param name="nodes">List of cluster node names.</param>
                    /// <param name="peers">peers address list.</param>
                    /// <param name="reversedns">reverse dns api server</param>
                    /// <param name="rt_import">Route-Target import</param>
                    /// <param name="tag">Service-VLAN Tag</param>
                    /// <param name="vlan_protocol">
                    ///   Enum: 802.1q,802.1ad</param>
                    /// <param name="vrf_vxlan">l3vni.</param>
                    /// <param name="vxlan_port">Vxlan tunnel udp port (default 4789).</param>
                    /// <returns></returns>
                    public async Task<Result> Create(string type, string zone, bool? advertise_subnets = null, string bridge = null, bool? bridge_disable_mac_learning = null, string controller = null, string dhcp = null, bool? disable_arp_nd_suppression = null, string dns = null, string dnszone = null, int? dp_id = null, string exitnodes = null, bool? exitnodes_local_routing = null, string exitnodes_primary = null, string ipam = null, string mac = null, int? mtu = null, string nodes = null, string peers = null, string reversedns = null, string rt_import = null, int? tag = null, string vlan_protocol = null, int? vrf_vxlan = null, int? vxlan_port = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        parameters.Add("zone", zone);
                        parameters.Add("advertise-subnets", advertise_subnets);
                        parameters.Add("bridge", bridge);
                        parameters.Add("bridge-disable-mac-learning", bridge_disable_mac_learning);
                        parameters.Add("controller", controller);
                        parameters.Add("dhcp", dhcp);
                        parameters.Add("disable-arp-nd-suppression", disable_arp_nd_suppression);
                        parameters.Add("dns", dns);
                        parameters.Add("dnszone", dnszone);
                        parameters.Add("dp-id", dp_id);
                        parameters.Add("exitnodes", exitnodes);
                        parameters.Add("exitnodes-local-routing", exitnodes_local_routing);
                        parameters.Add("exitnodes-primary", exitnodes_primary);
                        parameters.Add("ipam", ipam);
                        parameters.Add("mac", mac);
                        parameters.Add("mtu", mtu);
                        parameters.Add("nodes", nodes);
                        parameters.Add("peers", peers);
                        parameters.Add("reversedns", reversedns);
                        parameters.Add("rt-import", rt_import);
                        parameters.Add("tag", tag);
                        parameters.Add("vlan-protocol", vlan_protocol);
                        parameters.Add("vrf-vxlan", vrf_vxlan);
                        parameters.Add("vxlan-port", vxlan_port);
                        return await _client.Create($"/cluster/sdn/zones", parameters);
                    }
                }
                /// <summary>
                /// Controllers
                /// </summary>
                public class PveControllers
                {
                    private readonly PveClient _client;

                    internal PveControllers(PveClient client) { _client = client; }
                    /// <summary>
                    /// ControllerItem
                    /// </summary>
                    public PveControllerItem this[object controller] => new(_client, controller);
                    /// <summary>
                    /// ControllerItem
                    /// </summary>
                    public class PveControllerItem
                    {
                        private readonly PveClient _client;
                        private readonly object _controller;
                        internal PveControllerItem(PveClient client, object controller) { _client = client; _controller = controller; }
                        /// <summary>
                        /// Delete sdn controller object configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/sdn/controllers/{_controller}"); }
                        /// <summary>
                        /// Read sdn controller configuration.
                        /// </summary>
                        /// <param name="pending">Display pending config.</param>
                        /// <param name="running">Display running config.</param>
                        /// <returns></returns>
                        public async Task<Result> Read(bool? pending = null, bool? running = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("pending", pending);
                            parameters.Add("running", running);
                            return await _client.Get($"/cluster/sdn/controllers/{_controller}", parameters);
                        }
                        /// <summary>
                        /// Update sdn controller object configuration.
                        /// </summary>
                        /// <param name="asn">autonomous system number</param>
                        /// <param name="bgp_multipath_as_path_relax"></param>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="ebgp">Enable ebgp. (remote-as external)</param>
                        /// <param name="ebgp_multihop"></param>
                        /// <param name="isis_domain">ISIS domain.</param>
                        /// <param name="isis_ifaces">ISIS interface.</param>
                        /// <param name="isis_net">ISIS network entity title.</param>
                        /// <param name="loopback">source loopback interface.</param>
                        /// <param name="node">The cluster node name.</param>
                        /// <param name="peers">peers address list.</param>
                        /// <returns></returns>
                        public async Task<Result> Update(int? asn = null, bool? bgp_multipath_as_path_relax = null, string delete = null, string digest = null, bool? ebgp = null, int? ebgp_multihop = null, string isis_domain = null, string isis_ifaces = null, string isis_net = null, string loopback = null, string node = null, string peers = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("asn", asn);
                            parameters.Add("bgp-multipath-as-path-relax", bgp_multipath_as_path_relax);
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("ebgp", ebgp);
                            parameters.Add("ebgp-multihop", ebgp_multihop);
                            parameters.Add("isis-domain", isis_domain);
                            parameters.Add("isis-ifaces", isis_ifaces);
                            parameters.Add("isis-net", isis_net);
                            parameters.Add("loopback", loopback);
                            parameters.Add("node", node);
                            parameters.Add("peers", peers);
                            return await _client.Set($"/cluster/sdn/controllers/{_controller}", parameters);
                        }
                    }
                    /// <summary>
                    /// SDN controllers index.
                    /// </summary>
                    /// <param name="pending">Display pending config.</param>
                    /// <param name="running">Display running config.</param>
                    /// <param name="type">Only list sdn controllers of specific type
                    ///   Enum: bgp,evpn,faucet,isis</param>
                    /// <returns></returns>
                    public async Task<Result> Index(bool? pending = null, bool? running = null, string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("pending", pending);
                        parameters.Add("running", running);
                        parameters.Add("type", type);
                        return await _client.Get($"/cluster/sdn/controllers", parameters);
                    }
                    /// <summary>
                    /// Create a new sdn controller object.
                    /// </summary>
                    /// <param name="controller">The SDN controller object identifier.</param>
                    /// <param name="type">Plugin type.
                    ///   Enum: bgp,evpn,faucet,isis</param>
                    /// <param name="asn">autonomous system number</param>
                    /// <param name="bgp_multipath_as_path_relax"></param>
                    /// <param name="ebgp">Enable ebgp. (remote-as external)</param>
                    /// <param name="ebgp_multihop"></param>
                    /// <param name="isis_domain">ISIS domain.</param>
                    /// <param name="isis_ifaces">ISIS interface.</param>
                    /// <param name="isis_net">ISIS network entity title.</param>
                    /// <param name="loopback">source loopback interface.</param>
                    /// <param name="node">The cluster node name.</param>
                    /// <param name="peers">peers address list.</param>
                    /// <returns></returns>
                    public async Task<Result> Create(string controller, string type, int? asn = null, bool? bgp_multipath_as_path_relax = null, bool? ebgp = null, int? ebgp_multihop = null, string isis_domain = null, string isis_ifaces = null, string isis_net = null, string loopback = null, string node = null, string peers = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("controller", controller);
                        parameters.Add("type", type);
                        parameters.Add("asn", asn);
                        parameters.Add("bgp-multipath-as-path-relax", bgp_multipath_as_path_relax);
                        parameters.Add("ebgp", ebgp);
                        parameters.Add("ebgp-multihop", ebgp_multihop);
                        parameters.Add("isis-domain", isis_domain);
                        parameters.Add("isis-ifaces", isis_ifaces);
                        parameters.Add("isis-net", isis_net);
                        parameters.Add("loopback", loopback);
                        parameters.Add("node", node);
                        parameters.Add("peers", peers);
                        return await _client.Create($"/cluster/sdn/controllers", parameters);
                    }
                }
                /// <summary>
                /// Ipams
                /// </summary>
                public class PveIpams
                {
                    private readonly PveClient _client;

                    internal PveIpams(PveClient client) { _client = client; }
                    /// <summary>
                    /// IpamItem
                    /// </summary>
                    public PveIpamItem this[object ipam] => new(_client, ipam);
                    /// <summary>
                    /// IpamItem
                    /// </summary>
                    public class PveIpamItem
                    {
                        private readonly PveClient _client;
                        private readonly object _ipam;
                        internal PveIpamItem(PveClient client, object ipam) { _client = client; _ipam = ipam; }
                        private PveStatus _status;
                        /// <summary>
                        /// Status
                        /// </summary>
                        public PveStatus Status => _status ??= new(_client, _ipam);
                        /// <summary>
                        /// Status
                        /// </summary>
                        public class PveStatus
                        {
                            private readonly PveClient _client;
                            private readonly object _ipam;
                            internal PveStatus(PveClient client, object ipam) { _client = client; _ipam = ipam; }
                            /// <summary>
                            /// List PVE IPAM Entries
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Ipamindex() { return await _client.Get($"/cluster/sdn/ipams/{_ipam}/status"); }
                        }
                        /// <summary>
                        /// Delete sdn ipam object configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/sdn/ipams/{_ipam}"); }
                        /// <summary>
                        /// Read sdn ipam configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Read() { return await _client.Get($"/cluster/sdn/ipams/{_ipam}"); }
                        /// <summary>
                        /// Update sdn ipam object configuration.
                        /// </summary>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="section"></param>
                        /// <param name="token"></param>
                        /// <param name="url"></param>
                        /// <returns></returns>
                        public async Task<Result> Update(string delete = null, string digest = null, int? section = null, string token = null, string url = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("section", section);
                            parameters.Add("token", token);
                            parameters.Add("url", url);
                            return await _client.Set($"/cluster/sdn/ipams/{_ipam}", parameters);
                        }
                    }
                    /// <summary>
                    /// SDN ipams index.
                    /// </summary>
                    /// <param name="type">Only list sdn ipams of specific type
                    ///   Enum: netbox,phpipam,pve</param>
                    /// <returns></returns>
                    public async Task<Result> Index(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return await _client.Get($"/cluster/sdn/ipams", parameters);
                    }
                    /// <summary>
                    /// Create a new sdn ipam object.
                    /// </summary>
                    /// <param name="ipam">The SDN ipam object identifier.</param>
                    /// <param name="type">Plugin type.
                    ///   Enum: netbox,phpipam,pve</param>
                    /// <param name="section"></param>
                    /// <param name="token"></param>
                    /// <param name="url"></param>
                    /// <returns></returns>
                    public async Task<Result> Create(string ipam, string type, int? section = null, string token = null, string url = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("ipam", ipam);
                        parameters.Add("type", type);
                        parameters.Add("section", section);
                        parameters.Add("token", token);
                        parameters.Add("url", url);
                        return await _client.Create($"/cluster/sdn/ipams", parameters);
                    }
                }
                /// <summary>
                /// Dns
                /// </summary>
                public class PveDns
                {
                    private readonly PveClient _client;

                    internal PveDns(PveClient client) { _client = client; }
                    /// <summary>
                    /// DnsItem
                    /// </summary>
                    public PveDnsItem this[object dns] => new(_client, dns);
                    /// <summary>
                    /// DnsItem
                    /// </summary>
                    public class PveDnsItem
                    {
                        private readonly PveClient _client;
                        private readonly object _dns;
                        internal PveDnsItem(PveClient client, object dns) { _client = client; _dns = dns; }
                        /// <summary>
                        /// Delete sdn dns object configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Delete() { return await _client.Delete($"/cluster/sdn/dns/{_dns}"); }
                        /// <summary>
                        /// Read sdn dns configuration.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Read() { return await _client.Get($"/cluster/sdn/dns/{_dns}"); }
                        /// <summary>
                        /// Update sdn dns object configuration.
                        /// </summary>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="key"></param>
                        /// <param name="reversemaskv6"></param>
                        /// <param name="ttl"></param>
                        /// <param name="url"></param>
                        /// <returns></returns>
                        public async Task<Result> Update(string delete = null, string digest = null, string key = null, int? reversemaskv6 = null, int? ttl = null, string url = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("delete", delete);
                            parameters.Add("digest", digest);
                            parameters.Add("key", key);
                            parameters.Add("reversemaskv6", reversemaskv6);
                            parameters.Add("ttl", ttl);
                            parameters.Add("url", url);
                            return await _client.Set($"/cluster/sdn/dns/{_dns}", parameters);
                        }
                    }
                    /// <summary>
                    /// SDN dns index.
                    /// </summary>
                    /// <param name="type">Only list sdn dns of specific type
                    ///   Enum: powerdns</param>
                    /// <returns></returns>
                    public async Task<Result> Index(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return await _client.Get($"/cluster/sdn/dns", parameters);
                    }
                    /// <summary>
                    /// Create a new sdn dns object.
                    /// </summary>
                    /// <param name="dns">The SDN dns object identifier.</param>
                    /// <param name="key"></param>
                    /// <param name="type">Plugin type.
                    ///   Enum: powerdns</param>
                    /// <param name="url"></param>
                    /// <param name="reversemaskv6"></param>
                    /// <param name="reversev6mask"></param>
                    /// <param name="ttl"></param>
                    /// <returns></returns>
                    public async Task<Result> Create(string dns, string key, string type, string url, int? reversemaskv6 = null, int? reversev6mask = null, int? ttl = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("dns", dns);
                        parameters.Add("key", key);
                        parameters.Add("type", type);
                        parameters.Add("url", url);
                        parameters.Add("reversemaskv6", reversemaskv6);
                        parameters.Add("reversev6mask", reversev6mask);
                        parameters.Add("ttl", ttl);
                        return await _client.Create($"/cluster/sdn/dns", parameters);
                    }
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/cluster/sdn"); }
                /// <summary>
                /// Apply sdn controller changes &amp;&amp; reload.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Reload() { return await _client.Set($"/cluster/sdn"); }
            }
            /// <summary>
            /// Log
            /// </summary>
            public class PveLog
            {
                private readonly PveClient _client;

                internal PveLog(PveClient client) { _client = client; }
                /// <summary>
                /// Read cluster log
                /// </summary>
                /// <param name="max">Maximum number of entries.</param>
                /// <returns></returns>
                public async Task<Result> Log(int? max = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("max", max);
                    return await _client.Get($"/cluster/log", parameters);
                }
            }
            /// <summary>
            /// Resources
            /// </summary>
            public class PveResources
            {
                private readonly PveClient _client;

                internal PveResources(PveClient client) { _client = client; }
                /// <summary>
                /// Resources index (cluster wide).
                /// </summary>
                /// <param name="type">
                ///   Enum: vm,storage,node,sdn</param>
                /// <returns></returns>
                public async Task<Result> Resources(string type = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("type", type);
                    return await _client.Get($"/cluster/resources", parameters);
                }
            }
            /// <summary>
            /// Tasks
            /// </summary>
            public class PveTasks
            {
                private readonly PveClient _client;

                internal PveTasks(PveClient client) { _client = client; }
                /// <summary>
                /// List recent tasks (cluster wide).
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Tasks() { return await _client.Get($"/cluster/tasks"); }
            }
            /// <summary>
            /// Options
            /// </summary>
            public class PveOptions
            {
                private readonly PveClient _client;

                internal PveOptions(PveClient client) { _client = client; }
                /// <summary>
                /// Get datacenter options. Without 'Sys.Audit' on '/' not all options are returned.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> GetOptions() { return await _client.Get($"/cluster/options"); }
                /// <summary>
                /// Set datacenter options.
                /// </summary>
                /// <param name="bwlimit">Set I/O bandwidth limit for various operations (in KiB/s).</param>
                /// <param name="console">Select the default Console viewer. You can either use the builtin java applet (VNC; deprecated and maps to html5), an external virt-viewer comtatible application (SPICE), an HTML5 based vnc viewer (noVNC), or an HTML5 based console client (xtermjs). If the selected viewer is not available (e.g. SPICE not activated for the VM), the fallback is noVNC.
                ///   Enum: applet,vv,html5,xtermjs</param>
                /// <param name="crs">Cluster resource scheduling settings.</param>
                /// <param name="delete">A list of settings you want to delete.</param>
                /// <param name="description">Datacenter description. Shown in the web-interface datacenter notes panel. This is saved as comment inside the configuration file.</param>
                /// <param name="email_from">Specify email address to send notification from (default is root@$hostname)</param>
                /// <param name="fencing">Set the fencing mode of the HA cluster. Hardware mode needs a valid configuration of fence devices in /etc/pve/ha/fence.cfg. With both all two modes are used.  WARNING: 'hardware' and 'both' are EXPERIMENTAL &amp; WIP
                ///   Enum: watchdog,hardware,both</param>
                /// <param name="ha">Cluster wide HA settings.</param>
                /// <param name="http_proxy">Specify external http proxy which is used for downloads (example: 'http://username:password@host:port/')</param>
                /// <param name="keyboard">Default keybord layout for vnc server.
                ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                /// <param name="language">Default GUI language.
                ///   Enum: ar,ca,da,de,en,es,eu,fa,fr,hr,he,it,ja,ka,kr,nb,nl,nn,pl,pt_BR,ru,sl,sv,tr,ukr,zh_CN,zh_TW</param>
                /// <param name="mac_prefix">Prefix for the auto-generated MAC addresses of virtual guests. The default 'BC:24:11' is the OUI assigned by the IEEE to Proxmox Server Solutions GmbH for a 24-bit large MAC block. You're allowed to use this in local networks, i.e., those not directly reachable by the public (e.g., in a LAN or behind NAT).</param>
                /// <param name="max_workers">Defines how many workers (per node) are maximal started  on actions like 'stopall VMs' or task from the ha-manager.</param>
                /// <param name="migration">For cluster wide migration settings.</param>
                /// <param name="migration_unsecure">Migration is secure using SSH tunnel by default. For secure private networks you can disable it to speed up migration. Deprecated, use the 'migration' property instead!</param>
                /// <param name="next_id">Control the range for the free VMID auto-selection pool.</param>
                /// <param name="notify">Cluster-wide notification settings.</param>
                /// <param name="registered_tags">A list of tags that require a `Sys.Modify` on '/' to set and delete. Tags set here that are also in 'user-tag-access' also require `Sys.Modify`.</param>
                /// <param name="tag_style">Tag style options.</param>
                /// <param name="u2f">u2f</param>
                /// <param name="user_tag_access">Privilege options for user-settable tags</param>
                /// <param name="webauthn">webauthn configuration</param>
                /// <returns></returns>
                public async Task<Result> SetOptions(string bwlimit = null, string console = null, string crs = null, string delete = null, string description = null, string email_from = null, string fencing = null, string ha = null, string http_proxy = null, string keyboard = null, string language = null, string mac_prefix = null, int? max_workers = null, string migration = null, bool? migration_unsecure = null, string next_id = null, string notify = null, string registered_tags = null, string tag_style = null, string u2f = null, string user_tag_access = null, string webauthn = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("bwlimit", bwlimit);
                    parameters.Add("console", console);
                    parameters.Add("crs", crs);
                    parameters.Add("delete", delete);
                    parameters.Add("description", description);
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
                    parameters.Add("next-id", next_id);
                    parameters.Add("notify", notify);
                    parameters.Add("registered-tags", registered_tags);
                    parameters.Add("tag-style", tag_style);
                    parameters.Add("u2f", u2f);
                    parameters.Add("user-tag-access", user_tag_access);
                    parameters.Add("webauthn", webauthn);
                    return await _client.Set($"/cluster/options", parameters);
                }
            }
            /// <summary>
            /// Status
            /// </summary>
            public class PveStatus
            {
                private readonly PveClient _client;

                internal PveStatus(PveClient client) { _client = client; }
                /// <summary>
                /// Get cluster status information.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> GetStatus() { return await _client.Get($"/cluster/status"); }
            }
            /// <summary>
            /// Nextid
            /// </summary>
            public class PveNextid
            {
                private readonly PveClient _client;

                internal PveNextid(PveClient client) { _client = client; }
                /// <summary>
                /// Get next free VMID. Pass a VMID to assert that its free (at time of check).
                /// </summary>
                /// <param name="vmid">The (unique) ID of the VM.</param>
                /// <returns></returns>
                public async Task<Result> Nextid(int? vmid = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("vmid", vmid);
                    return await _client.Get($"/cluster/nextid", parameters);
                }
            }
            /// <summary>
            /// Cluster index.
            /// </summary>
            /// <returns></returns>
            public async Task<Result> Index() { return await _client.Get($"/cluster"); }
        }
        /// <summary>
        /// Nodes
        /// </summary>
        public class PveNodes
        {
            private readonly PveClient _client;

            internal PveNodes(PveClient client) { _client = client; }
            /// <summary>
            /// NodeItem
            /// </summary>
            public PveNodeItem this[object node] => new(_client, node);
            /// <summary>
            /// NodeItem
            /// </summary>
            public class PveNodeItem
            {
                private readonly PveClient _client;
                private readonly object _node;
                internal PveNodeItem(PveClient client, object node) { _client = client; _node = node; }
                private PveQemu _qemu;
                /// <summary>
                /// Qemu
                /// </summary>
                public PveQemu Qemu => _qemu ??= new(_client, _node);
                private PveLxc _lxc;
                /// <summary>
                /// Lxc
                /// </summary>
                public PveLxc Lxc => _lxc ??= new(_client, _node);
                private PveCeph _ceph;
                /// <summary>
                /// Ceph
                /// </summary>
                public PveCeph Ceph => _ceph ??= new(_client, _node);
                private PveVzdump _vzdump;
                /// <summary>
                /// Vzdump
                /// </summary>
                public PveVzdump Vzdump => _vzdump ??= new(_client, _node);
                private PveServices _services;
                /// <summary>
                /// Services
                /// </summary>
                public PveServices Services => _services ??= new(_client, _node);
                private PveSubscription _subscription;
                /// <summary>
                /// Subscription
                /// </summary>
                public PveSubscription Subscription => _subscription ??= new(_client, _node);
                private PveNetwork _network;
                /// <summary>
                /// Network
                /// </summary>
                public PveNetwork Network => _network ??= new(_client, _node);
                private PveTasks _tasks;
                /// <summary>
                /// Tasks
                /// </summary>
                public PveTasks Tasks => _tasks ??= new(_client, _node);
                private PveScan _scan;
                /// <summary>
                /// Scan
                /// </summary>
                public PveScan Scan => _scan ??= new(_client, _node);
                private PveHardware _hardware;
                /// <summary>
                /// Hardware
                /// </summary>
                public PveHardware Hardware => _hardware ??= new(_client, _node);
                private PveCapabilities _capabilities;
                /// <summary>
                /// Capabilities
                /// </summary>
                public PveCapabilities Capabilities => _capabilities ??= new(_client, _node);
                private PveStorage _storage;
                /// <summary>
                /// Storage
                /// </summary>
                public PveStorage Storage => _storage ??= new(_client, _node);
                private PveDisks _disks;
                /// <summary>
                /// Disks
                /// </summary>
                public PveDisks Disks => _disks ??= new(_client, _node);
                private PveApt _apt;
                /// <summary>
                /// Apt
                /// </summary>
                public PveApt Apt => _apt ??= new(_client, _node);
                private PveFirewall _firewall;
                /// <summary>
                /// Firewall
                /// </summary>
                public PveFirewall Firewall => _firewall ??= new(_client, _node);
                private PveReplication _replication;
                /// <summary>
                /// Replication
                /// </summary>
                public PveReplication Replication => _replication ??= new(_client, _node);
                private PveCertificates _certificates;
                /// <summary>
                /// Certificates
                /// </summary>
                public PveCertificates Certificates => _certificates ??= new(_client, _node);
                private PveConfig _config;
                /// <summary>
                /// Config
                /// </summary>
                public PveConfig Config => _config ??= new(_client, _node);
                private PveSdn _sdn;
                /// <summary>
                /// Sdn
                /// </summary>
                public PveSdn Sdn => _sdn ??= new(_client, _node);
                private PveVersion _version;
                /// <summary>
                /// Version
                /// </summary>
                public PveVersion Version => _version ??= new(_client, _node);
                private PveStatus _status;
                /// <summary>
                /// Status
                /// </summary>
                public PveStatus Status => _status ??= new(_client, _node);
                private PveNetstat _netstat;
                /// <summary>
                /// Netstat
                /// </summary>
                public PveNetstat Netstat => _netstat ??= new(_client, _node);
                private PveExecute _execute;
                /// <summary>
                /// Execute
                /// </summary>
                public PveExecute Execute => _execute ??= new(_client, _node);
                private PveWakeonlan _wakeonlan;
                /// <summary>
                /// Wakeonlan
                /// </summary>
                public PveWakeonlan Wakeonlan => _wakeonlan ??= new(_client, _node);
                private PveRrd _rrd;
                /// <summary>
                /// Rrd
                /// </summary>
                public PveRrd Rrd => _rrd ??= new(_client, _node);
                private PveRrddata _rrddata;
                /// <summary>
                /// Rrddata
                /// </summary>
                public PveRrddata Rrddata => _rrddata ??= new(_client, _node);
                private PveSyslog _syslog;
                /// <summary>
                /// Syslog
                /// </summary>
                public PveSyslog Syslog => _syslog ??= new(_client, _node);
                private PveJournal _journal;
                /// <summary>
                /// Journal
                /// </summary>
                public PveJournal Journal => _journal ??= new(_client, _node);
                private PveVncshell _vncshell;
                /// <summary>
                /// Vncshell
                /// </summary>
                public PveVncshell Vncshell => _vncshell ??= new(_client, _node);
                private PveTermproxy _termproxy;
                /// <summary>
                /// Termproxy
                /// </summary>
                public PveTermproxy Termproxy => _termproxy ??= new(_client, _node);
                private PveVncwebsocket _vncwebsocket;
                /// <summary>
                /// Vncwebsocket
                /// </summary>
                public PveVncwebsocket Vncwebsocket => _vncwebsocket ??= new(_client, _node);
                private PveSpiceshell _spiceshell;
                /// <summary>
                /// Spiceshell
                /// </summary>
                public PveSpiceshell Spiceshell => _spiceshell ??= new(_client, _node);
                private PveDns _dns;
                /// <summary>
                /// Dns
                /// </summary>
                public PveDns Dns => _dns ??= new(_client, _node);
                private PveTime _time;
                /// <summary>
                /// Time
                /// </summary>
                public PveTime Time => _time ??= new(_client, _node);
                private PveAplinfo _aplinfo;
                /// <summary>
                /// Aplinfo
                /// </summary>
                public PveAplinfo Aplinfo => _aplinfo ??= new(_client, _node);
                private PveQueryUrlMetadata _queryUrlMetadata;
                /// <summary>
                /// QueryUrlMetadata
                /// </summary>
                public PveQueryUrlMetadata QueryUrlMetadata => _queryUrlMetadata ??= new(_client, _node);
                private PveReport _report;
                /// <summary>
                /// Report
                /// </summary>
                public PveReport Report => _report ??= new(_client, _node);
                private PveStartall _startall;
                /// <summary>
                /// Startall
                /// </summary>
                public PveStartall Startall => _startall ??= new(_client, _node);
                private PveStopall _stopall;
                /// <summary>
                /// Stopall
                /// </summary>
                public PveStopall Stopall => _stopall ??= new(_client, _node);
                private PveSuspendall _suspendall;
                /// <summary>
                /// Suspendall
                /// </summary>
                public PveSuspendall Suspendall => _suspendall ??= new(_client, _node);
                private PveMigrateall _migrateall;
                /// <summary>
                /// Migrateall
                /// </summary>
                public PveMigrateall Migrateall => _migrateall ??= new(_client, _node);
                private PveHosts _hosts;
                /// <summary>
                /// Hosts
                /// </summary>
                public PveHosts Hosts => _hosts ??= new(_client, _node);
                /// <summary>
                /// Qemu
                /// </summary>
                public class PveQemu
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveQemu(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// VmidItem
                    /// </summary>
                    public PveVmidItem this[object vmid] => new(_client, _node, vmid);
                    /// <summary>
                    /// VmidItem
                    /// </summary>
                    public class PveVmidItem
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        private readonly object _vmid;
                        internal PveVmidItem(PveClient client, object node, object vmid)
                        {
                            _client = client; _node = node;
                            _vmid = vmid;
                        }
                        private PveFirewall _firewall;
                        /// <summary>
                        /// Firewall
                        /// </summary>
                        public PveFirewall Firewall => _firewall ??= new(_client, _node, _vmid);
                        private PveAgent _agent;
                        /// <summary>
                        /// Agent
                        /// </summary>
                        public PveAgent Agent => _agent ??= new(_client, _node, _vmid);
                        private PveRrd _rrd;
                        /// <summary>
                        /// Rrd
                        /// </summary>
                        public PveRrd Rrd => _rrd ??= new(_client, _node, _vmid);
                        private PveRrddata _rrddata;
                        /// <summary>
                        /// Rrddata
                        /// </summary>
                        public PveRrddata Rrddata => _rrddata ??= new(_client, _node, _vmid);
                        private PveConfig _config;
                        /// <summary>
                        /// Config
                        /// </summary>
                        public PveConfig Config => _config ??= new(_client, _node, _vmid);
                        private PvePending _pending;
                        /// <summary>
                        /// Pending
                        /// </summary>
                        public PvePending Pending => _pending ??= new(_client, _node, _vmid);
                        private PveCloudinit _cloudinit;
                        /// <summary>
                        /// Cloudinit
                        /// </summary>
                        public PveCloudinit Cloudinit => _cloudinit ??= new(_client, _node, _vmid);
                        private PveUnlink _unlink;
                        /// <summary>
                        /// Unlink
                        /// </summary>
                        public PveUnlink Unlink => _unlink ??= new(_client, _node, _vmid);
                        private PveVncproxy _vncproxy;
                        /// <summary>
                        /// Vncproxy
                        /// </summary>
                        public PveVncproxy Vncproxy => _vncproxy ??= new(_client, _node, _vmid);
                        private PveTermproxy _termproxy;
                        /// <summary>
                        /// Termproxy
                        /// </summary>
                        public PveTermproxy Termproxy => _termproxy ??= new(_client, _node, _vmid);
                        private PveVncwebsocket _vncwebsocket;
                        /// <summary>
                        /// Vncwebsocket
                        /// </summary>
                        public PveVncwebsocket Vncwebsocket => _vncwebsocket ??= new(_client, _node, _vmid);
                        private PveSpiceproxy _spiceproxy;
                        /// <summary>
                        /// Spiceproxy
                        /// </summary>
                        public PveSpiceproxy Spiceproxy => _spiceproxy ??= new(_client, _node, _vmid);
                        private PveStatus _status;
                        /// <summary>
                        /// Status
                        /// </summary>
                        public PveStatus Status => _status ??= new(_client, _node, _vmid);
                        private PveSendkey _sendkey;
                        /// <summary>
                        /// Sendkey
                        /// </summary>
                        public PveSendkey Sendkey => _sendkey ??= new(_client, _node, _vmid);
                        private PveFeature _feature;
                        /// <summary>
                        /// Feature
                        /// </summary>
                        public PveFeature Feature => _feature ??= new(_client, _node, _vmid);
                        private PveClone _clone;
                        /// <summary>
                        /// Clone
                        /// </summary>
                        public PveClone Clone => _clone ??= new(_client, _node, _vmid);
                        private PveMoveDisk _moveDisk;
                        /// <summary>
                        /// MoveDisk
                        /// </summary>
                        public PveMoveDisk MoveDisk => _moveDisk ??= new(_client, _node, _vmid);
                        private PveMigrate _migrate;
                        /// <summary>
                        /// Migrate
                        /// </summary>
                        public PveMigrate Migrate => _migrate ??= new(_client, _node, _vmid);
                        private PveRemoteMigrate _remoteMigrate;
                        /// <summary>
                        /// RemoteMigrate
                        /// </summary>
                        public PveRemoteMigrate RemoteMigrate => _remoteMigrate ??= new(_client, _node, _vmid);
                        private PveMonitor _monitor;
                        /// <summary>
                        /// Monitor
                        /// </summary>
                        public PveMonitor Monitor => _monitor ??= new(_client, _node, _vmid);
                        private PveResize _resize;
                        /// <summary>
                        /// Resize
                        /// </summary>
                        public PveResize Resize => _resize ??= new(_client, _node, _vmid);
                        private PveSnapshot _snapshot;
                        /// <summary>
                        /// Snapshot
                        /// </summary>
                        public PveSnapshot Snapshot => _snapshot ??= new(_client, _node, _vmid);
                        private PveTemplate _template;
                        /// <summary>
                        /// Template
                        /// </summary>
                        public PveTemplate Template => _template ??= new(_client, _node, _vmid);
                        private PveMtunnel _mtunnel;
                        /// <summary>
                        /// Mtunnel
                        /// </summary>
                        public PveMtunnel Mtunnel => _mtunnel ??= new(_client, _node, _vmid);
                        private PveMtunnelwebsocket _mtunnelwebsocket;
                        /// <summary>
                        /// Mtunnelwebsocket
                        /// </summary>
                        public PveMtunnelwebsocket Mtunnelwebsocket => _mtunnelwebsocket ??= new(_client, _node, _vmid);
                        /// <summary>
                        /// Firewall
                        /// </summary>
                        public class PveFirewall
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveFirewall(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PveRules _rules;
                            /// <summary>
                            /// Rules
                            /// </summary>
                            public PveRules Rules => _rules ??= new(_client, _node, _vmid);
                            private PveAliases _aliases;
                            /// <summary>
                            /// Aliases
                            /// </summary>
                            public PveAliases Aliases => _aliases ??= new(_client, _node, _vmid);
                            private PveIpset _ipset;
                            /// <summary>
                            /// Ipset
                            /// </summary>
                            public PveIpset Ipset => _ipset ??= new(_client, _node, _vmid);
                            private PveOptions _options;
                            /// <summary>
                            /// Options
                            /// </summary>
                            public PveOptions Options => _options ??= new(_client, _node, _vmid);
                            private PveLog _log;
                            /// <summary>
                            /// Log
                            /// </summary>
                            public PveLog Log => _log ??= new(_client, _node, _vmid);
                            private PveRefs _refs;
                            /// <summary>
                            /// Refs
                            /// </summary>
                            public PveRefs Refs => _refs ??= new(_client, _node, _vmid);
                            /// <summary>
                            /// Rules
                            /// </summary>
                            public class PveRules
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveRules(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// PosItem
                                /// </summary>
                                public PvePosItem this[object pos] => new(_client, _node, _vmid, pos);
                                /// <summary>
                                /// PosItem
                                /// </summary>
                                public class PvePosItem
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _pos;
                                    internal PvePosItem(PveClient client, object node, object vmid, object pos)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _pos = pos;
                                    }
                                    /// <summary>
                                    /// Delete rule.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public async Task<Result> DeleteRule(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        return await _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}", parameters);
                                    }
                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public async Task<Result> GetRule() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}"); }
                                    /// <summary>
                                    /// Modify rule data.
                                    /// </summary>
                                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                    /// <param name="comment">Descriptive comment.</param>
                                    /// <param name="delete">A list of settings you want to delete.</param>
                                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="enable">Flag to enable/disable a rule.</param>
                                    /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
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
                                    public async Task<Result> UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("action", action);
                                        parameters.Add("comment", comment);
                                        parameters.Add("delete", delete);
                                        parameters.Add("dest", dest);
                                        parameters.Add("digest", digest);
                                        parameters.Add("dport", dport);
                                        parameters.Add("enable", enable);
                                        parameters.Add("icmp-type", icmp_type);
                                        parameters.Add("iface", iface);
                                        parameters.Add("log", log);
                                        parameters.Add("macro", macro);
                                        parameters.Add("moveto", moveto);
                                        parameters.Add("proto", proto);
                                        parameters.Add("source", source);
                                        parameters.Add("sport", sport);
                                        parameters.Add("type", type);
                                        return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/firewall/rules/{_pos}", parameters);
                                    }
                                }
                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetRules() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/rules"); }
                                /// <summary>
                                /// Create new rule.
                                /// </summary>
                                /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                /// <param name="type">Rule type.
                                ///   Enum: in,out,group</param>
                                /// <param name="comment">Descriptive comment.</param>
                                /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <param name="enable">Flag to enable/disable a rule.</param>
                                /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
                                /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                /// <param name="log">Log level for firewall rule.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public async Task<Result> CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("action", action);
                                    parameters.Add("type", type);
                                    parameters.Add("comment", comment);
                                    parameters.Add("dest", dest);
                                    parameters.Add("digest", digest);
                                    parameters.Add("dport", dport);
                                    parameters.Add("enable", enable);
                                    parameters.Add("icmp-type", icmp_type);
                                    parameters.Add("iface", iface);
                                    parameters.Add("log", log);
                                    parameters.Add("macro", macro);
                                    parameters.Add("pos", pos);
                                    parameters.Add("proto", proto);
                                    parameters.Add("source", source);
                                    parameters.Add("sport", sport);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/firewall/rules", parameters);
                                }
                            }
                            /// <summary>
                            /// Aliases
                            /// </summary>
                            public class PveAliases
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveAliases(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// NameItem
                                /// </summary>
                                public PveNameItem this[object name] => new(_client, _node, _vmid, name);
                                /// <summary>
                                /// NameItem
                                /// </summary>
                                public class PveNameItem
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _name;
                                    internal PveNameItem(PveClient client, object node, object vmid, object name)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }
                                    /// <summary>
                                    /// Remove IP or Network alias.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public async Task<Result> RemoveAlias(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        return await _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}", parameters);
                                    }
                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public async Task<Result> ReadAlias() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}"); }
                                    /// <summary>
                                    /// Update IP or Network alias.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="rename">Rename an existing alias.</param>
                                    /// <returns></returns>
                                    public async Task<Result> UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("digest", digest);
                                        parameters.Add("rename", rename);
                                        return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases/{_name}", parameters);
                                    }
                                }
                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetAliases() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases"); }
                                /// <summary>
                                /// Create IP or Network Alias.
                                /// </summary>
                                /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                /// <param name="name">Alias name.</param>
                                /// <param name="comment"></param>
                                /// <returns></returns>
                                public async Task<Result> CreateAlias(string cidr, string name, string comment = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("cidr", cidr);
                                    parameters.Add("name", name);
                                    parameters.Add("comment", comment);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/firewall/aliases", parameters);
                                }
                            }
                            /// <summary>
                            /// Ipset
                            /// </summary>
                            public class PveIpset
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveIpset(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// NameItem
                                /// </summary>
                                public PveNameItem this[object name] => new(_client, _node, _vmid, name);
                                /// <summary>
                                /// NameItem
                                /// </summary>
                                public class PveNameItem
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _name;
                                    internal PveNameItem(PveClient client, object node, object vmid, object name)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }
                                    /// <summary>
                                    /// CidrItem
                                    /// </summary>
                                    public PveCidrItem this[object cidr] => new(_client, _node, _vmid, _name, cidr);
                                    /// <summary>
                                    /// CidrItem
                                    /// </summary>
                                    public class PveCidrItem
                                    {
                                        private readonly PveClient _client;
                                        private readonly object _node;
                                        private readonly object _vmid;
                                        private readonly object _name;
                                        private readonly object _cidr;
                                        internal PveCidrItem(PveClient client, object node, object vmid, object name, object cidr)
                                        {
                                            _client = client; _node = node;
                                            _vmid = vmid;
                                            _name = name;
                                            _cidr = cidr;
                                        }
                                        /// <summary>
                                        /// Remove IP or Network from IPSet.
                                        /// </summary>
                                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                        /// <returns></returns>
                                        public async Task<Result> RemoveIp(string digest = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("digest", digest);
                                            return await _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}", parameters);
                                        }
                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public async Task<Result> ReadIp() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}"); }
                                        /// <summary>
                                        /// Update IP or Network settings
                                        /// </summary>
                                        /// <param name="comment"></param>
                                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                        /// <param name="nomatch"></param>
                                        /// <returns></returns>
                                        public async Task<Result> UpdateIp(string comment = null, string digest = null, bool? nomatch = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("comment", comment);
                                            parameters.Add("digest", digest);
                                            parameters.Add("nomatch", nomatch);
                                            return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}/{_cidr}", parameters);
                                        }
                                    }
                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <param name="force">Delete all members of the IPSet, if there are any.</param>
                                    /// <returns></returns>
                                    public async Task<Result> DeleteIpset(bool? force = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("force", force);
                                        return await _client.Delete($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}", parameters);
                                    }
                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public async Task<Result> GetIpset() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}"); }
                                    /// <summary>
                                    /// Add IP or Network to IPSet.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="nomatch"></param>
                                    /// <returns></returns>
                                    public async Task<Result> CreateIp(string cidr, string comment = null, bool? nomatch = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("nomatch", nomatch);
                                        return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset/{_name}", parameters);
                                    }
                                }
                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> IpsetIndex() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset"); }
                                /// <summary>
                                /// Create new IPSet
                                /// </summary>
                                /// <param name="name">IP set name.</param>
                                /// <param name="comment"></param>
                                /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                                /// <returns></returns>
                                public async Task<Result> CreateIpset(string name, string comment = null, string digest = null, string rename = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("name", name);
                                    parameters.Add("comment", comment);
                                    parameters.Add("digest", digest);
                                    parameters.Add("rename", rename);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/firewall/ipset", parameters);
                                }
                            }
                            /// <summary>
                            /// Options
                            /// </summary>
                            public class PveOptions
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveOptions(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetOptions() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/options"); }
                                /// <summary>
                                /// Set Firewall options.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp">Enable DHCP.</param>
                                /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="enable">Enable/disable firewall rules.</param>
                                /// <param name="ipfilter">Enable default IP filters. This is equivalent to adding an empty ipfilter-net&amp;lt;id&amp;gt; ipset for every interface. Such ipsets implicitly contain sane default restrictions such as restricting IPv6 link local addresses to the one derived from the interface's MAC address. For containers the configured IP addresses will be implicitly added.</param>
                                /// <param name="log_level_in">Log level for incoming traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="log_level_out">Log level for outgoing traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macfilter">Enable/disable MAC address filter.</param>
                                /// <param name="ndp">Enable NDP (Neighbor Discovery Protocol).</param>
                                /// <param name="policy_in">Input policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="policy_out">Output policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="radv">Allow sending Router Advertisement.</param>
                                /// <returns></returns>
                                public async Task<Result> SetOptions(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null)
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
                                    return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/firewall/options", parameters);
                                }
                            }
                            /// <summary>
                            /// Log
                            /// </summary>
                            public class PveLog
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveLog(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Read firewall log
                                /// </summary>
                                /// <param name="limit"></param>
                                /// <param name="since">Display log since this UNIX epoch.</param>
                                /// <param name="start"></param>
                                /// <param name="until">Display log until this UNIX epoch.</param>
                                /// <returns></returns>
                                public async Task<Result> Log(int? limit = null, int? since = null, int? start = null, int? until = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("limit", limit);
                                    parameters.Add("since", since);
                                    parameters.Add("start", start);
                                    parameters.Add("until", until);
                                    return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/log", parameters);
                                }
                            }
                            /// <summary>
                            /// Refs
                            /// </summary>
                            public class PveRefs
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveRefs(PveClient client, object node, object vmid)
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
                                public async Task<Result> Refs(string type = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall/refs", parameters);
                                }
                            }
                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/firewall"); }
                        }
                        /// <summary>
                        /// Agent
                        /// </summary>
                        public class PveAgent
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveAgent(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PveFsfreezeFreeze _fsfreezeFreeze;
                            /// <summary>
                            /// FsfreezeFreeze
                            /// </summary>
                            public PveFsfreezeFreeze FsfreezeFreeze => _fsfreezeFreeze ??= new(_client, _node, _vmid);
                            private PveFsfreezeStatus _fsfreezeStatus;
                            /// <summary>
                            /// FsfreezeStatus
                            /// </summary>
                            public PveFsfreezeStatus FsfreezeStatus => _fsfreezeStatus ??= new(_client, _node, _vmid);
                            private PveFsfreezeThaw _fsfreezeThaw;
                            /// <summary>
                            /// FsfreezeThaw
                            /// </summary>
                            public PveFsfreezeThaw FsfreezeThaw => _fsfreezeThaw ??= new(_client, _node, _vmid);
                            private PveFstrim _fstrim;
                            /// <summary>
                            /// Fstrim
                            /// </summary>
                            public PveFstrim Fstrim => _fstrim ??= new(_client, _node, _vmid);
                            private PveGetFsinfo _getFsinfo;
                            /// <summary>
                            /// GetFsinfo
                            /// </summary>
                            public PveGetFsinfo GetFsinfo => _getFsinfo ??= new(_client, _node, _vmid);
                            private PveGetHostName _getHostName;
                            /// <summary>
                            /// GetHostName
                            /// </summary>
                            public PveGetHostName GetHostName => _getHostName ??= new(_client, _node, _vmid);
                            private PveGetMemoryBlockInfo _getMemoryBlockInfo;
                            /// <summary>
                            /// GetMemoryBlockInfo
                            /// </summary>
                            public PveGetMemoryBlockInfo GetMemoryBlockInfo => _getMemoryBlockInfo ??= new(_client, _node, _vmid);
                            private PveGetMemoryBlocks _getMemoryBlocks;
                            /// <summary>
                            /// GetMemoryBlocks
                            /// </summary>
                            public PveGetMemoryBlocks GetMemoryBlocks => _getMemoryBlocks ??= new(_client, _node, _vmid);
                            private PveGetOsinfo _getOsinfo;
                            /// <summary>
                            /// GetOsinfo
                            /// </summary>
                            public PveGetOsinfo GetOsinfo => _getOsinfo ??= new(_client, _node, _vmid);
                            private PveGetTime _getTime;
                            /// <summary>
                            /// GetTime
                            /// </summary>
                            public PveGetTime GetTime => _getTime ??= new(_client, _node, _vmid);
                            private PveGetTimezone _getTimezone;
                            /// <summary>
                            /// GetTimezone
                            /// </summary>
                            public PveGetTimezone GetTimezone => _getTimezone ??= new(_client, _node, _vmid);
                            private PveGetUsers _getUsers;
                            /// <summary>
                            /// GetUsers
                            /// </summary>
                            public PveGetUsers GetUsers => _getUsers ??= new(_client, _node, _vmid);
                            private PveGetVcpus _getVcpus;
                            /// <summary>
                            /// GetVcpus
                            /// </summary>
                            public PveGetVcpus GetVcpus => _getVcpus ??= new(_client, _node, _vmid);
                            private PveInfo _info;
                            /// <summary>
                            /// Info
                            /// </summary>
                            public PveInfo Info => _info ??= new(_client, _node, _vmid);
                            private PveNetworkGetInterfaces _networkGetInterfaces;
                            /// <summary>
                            /// NetworkGetInterfaces
                            /// </summary>
                            public PveNetworkGetInterfaces NetworkGetInterfaces => _networkGetInterfaces ??= new(_client, _node, _vmid);
                            private PvePing _ping;
                            /// <summary>
                            /// Ping
                            /// </summary>
                            public PvePing Ping => _ping ??= new(_client, _node, _vmid);
                            private PveShutdown _shutdown;
                            /// <summary>
                            /// Shutdown
                            /// </summary>
                            public PveShutdown Shutdown => _shutdown ??= new(_client, _node, _vmid);
                            private PveSuspendDisk _suspendDisk;
                            /// <summary>
                            /// SuspendDisk
                            /// </summary>
                            public PveSuspendDisk SuspendDisk => _suspendDisk ??= new(_client, _node, _vmid);
                            private PveSuspendHybrid _suspendHybrid;
                            /// <summary>
                            /// SuspendHybrid
                            /// </summary>
                            public PveSuspendHybrid SuspendHybrid => _suspendHybrid ??= new(_client, _node, _vmid);
                            private PveSuspendRam _suspendRam;
                            /// <summary>
                            /// SuspendRam
                            /// </summary>
                            public PveSuspendRam SuspendRam => _suspendRam ??= new(_client, _node, _vmid);
                            private PveSetUserPassword _setUserPassword;
                            /// <summary>
                            /// SetUserPassword
                            /// </summary>
                            public PveSetUserPassword SetUserPassword => _setUserPassword ??= new(_client, _node, _vmid);
                            private PveExec _exec;
                            /// <summary>
                            /// Exec
                            /// </summary>
                            public PveExec Exec => _exec ??= new(_client, _node, _vmid);
                            private PveExecStatus _execStatus;
                            /// <summary>
                            /// ExecStatus
                            /// </summary>
                            public PveExecStatus ExecStatus => _execStatus ??= new(_client, _node, _vmid);
                            private PveFileRead _fileRead;
                            /// <summary>
                            /// FileRead
                            /// </summary>
                            public PveFileRead FileRead => _fileRead ??= new(_client, _node, _vmid);
                            private PveFileWrite _fileWrite;
                            /// <summary>
                            /// FileWrite
                            /// </summary>
                            public PveFileWrite FileWrite => _fileWrite ??= new(_client, _node, _vmid);
                            /// <summary>
                            /// FsfreezeFreeze
                            /// </summary>
                            public class PveFsfreezeFreeze
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveFsfreezeFreeze(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-freeze.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> FsfreezeFreeze() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-freeze"); }
                            }
                            /// <summary>
                            /// FsfreezeStatus
                            /// </summary>
                            public class PveFsfreezeStatus
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveFsfreezeStatus(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-status.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> FsfreezeStatus() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-status"); }
                            }
                            /// <summary>
                            /// FsfreezeThaw
                            /// </summary>
                            public class PveFsfreezeThaw
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveFsfreezeThaw(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fsfreeze-thaw.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> FsfreezeThaw() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fsfreeze-thaw"); }
                            }
                            /// <summary>
                            /// Fstrim
                            /// </summary>
                            public class PveFstrim
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveFstrim(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute fstrim.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Fstrim() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/fstrim"); }
                            }
                            /// <summary>
                            /// GetFsinfo
                            /// </summary>
                            public class PveGetFsinfo
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetFsinfo(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-fsinfo.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetFsinfo() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-fsinfo"); }
                            }
                            /// <summary>
                            /// GetHostName
                            /// </summary>
                            public class PveGetHostName
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetHostName(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-host-name.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetHostName() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-host-name"); }
                            }
                            /// <summary>
                            /// GetMemoryBlockInfo
                            /// </summary>
                            public class PveGetMemoryBlockInfo
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetMemoryBlockInfo(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-memory-block-info.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetMemoryBlockInfo() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-memory-block-info"); }
                            }
                            /// <summary>
                            /// GetMemoryBlocks
                            /// </summary>
                            public class PveGetMemoryBlocks
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetMemoryBlocks(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-memory-blocks.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetMemoryBlocks() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-memory-blocks"); }
                            }
                            /// <summary>
                            /// GetOsinfo
                            /// </summary>
                            public class PveGetOsinfo
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetOsinfo(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-osinfo.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetOsinfo() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-osinfo"); }
                            }
                            /// <summary>
                            /// GetTime
                            /// </summary>
                            public class PveGetTime
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetTime(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-time.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetTime() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-time"); }
                            }
                            /// <summary>
                            /// GetTimezone
                            /// </summary>
                            public class PveGetTimezone
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetTimezone(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-timezone.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetTimezone() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-timezone"); }
                            }
                            /// <summary>
                            /// GetUsers
                            /// </summary>
                            public class PveGetUsers
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetUsers(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-users.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetUsers() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-users"); }
                            }
                            /// <summary>
                            /// GetVcpus
                            /// </summary>
                            public class PveGetVcpus
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveGetVcpus(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute get-vcpus.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetVcpus() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/get-vcpus"); }
                            }
                            /// <summary>
                            /// Info
                            /// </summary>
                            public class PveInfo
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveInfo(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute info.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Info() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/info"); }
                            }
                            /// <summary>
                            /// NetworkGetInterfaces
                            /// </summary>
                            public class PveNetworkGetInterfaces
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveNetworkGetInterfaces(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute network-get-interfaces.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> NetworkGetInterfaces() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/network-get-interfaces"); }
                            }
                            /// <summary>
                            /// Ping
                            /// </summary>
                            public class PvePing
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PvePing(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute ping.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Ping() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/ping"); }
                            }
                            /// <summary>
                            /// Shutdown
                            /// </summary>
                            public class PveShutdown
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveShutdown(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute shutdown.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Shutdown() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/shutdown"); }
                            }
                            /// <summary>
                            /// SuspendDisk
                            /// </summary>
                            public class PveSuspendDisk
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveSuspendDisk(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-disk.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> SuspendDisk() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-disk"); }
                            }
                            /// <summary>
                            /// SuspendHybrid
                            /// </summary>
                            public class PveSuspendHybrid
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveSuspendHybrid(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-hybrid.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> SuspendHybrid() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-hybrid"); }
                            }
                            /// <summary>
                            /// SuspendRam
                            /// </summary>
                            public class PveSuspendRam
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveSuspendRam(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Execute suspend-ram.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> SuspendRam() { return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/suspend-ram"); }
                            }
                            /// <summary>
                            /// SetUserPassword
                            /// </summary>
                            public class PveSetUserPassword
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveSetUserPassword(PveClient client, object node, object vmid)
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
                                public async Task<Result> SetUserPassword(string password, string username, bool? crypted = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("password", password);
                                    parameters.Add("username", username);
                                    parameters.Add("crypted", crypted);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/set-user-password", parameters);
                                }
                            }
                            /// <summary>
                            /// Exec
                            /// </summary>
                            public class PveExec
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveExec(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Executes the given command in the vm via the guest-agent and returns an object with the pid.
                                /// </summary>
                                /// <param name="command">The command as a list of program + arguments.</param>
                                /// <param name="input_data">Data to pass as 'input-data' to the guest. Usually treated as STDIN to 'command'.</param>
                                /// <returns></returns>
                                public async Task<Result> Exec(string command, string input_data = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("command", command);
                                    parameters.Add("input-data", input_data);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/exec", parameters);
                                }
                            }
                            /// <summary>
                            /// ExecStatus
                            /// </summary>
                            public class PveExecStatus
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveExecStatus(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Gets the status of the given pid started by the guest-agent
                                /// </summary>
                                /// <param name="pid">The PID to query</param>
                                /// <returns></returns>
                                public async Task<Result> ExecStatus(int pid)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("pid", pid);
                                    return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/exec-status", parameters);
                                }
                            }
                            /// <summary>
                            /// FileRead
                            /// </summary>
                            public class PveFileRead
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveFileRead(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Reads the given file via guest agent. Is limited to 16777216 bytes.
                                /// </summary>
                                /// <param name="file">The path to the file</param>
                                /// <returns></returns>
                                public async Task<Result> FileRead(string file)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("file", file);
                                    return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent/file-read", parameters);
                                }
                            }
                            /// <summary>
                            /// FileWrite
                            /// </summary>
                            public class PveFileWrite
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveFileWrite(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Writes the given file via guest agent.
                                /// </summary>
                                /// <param name="content">The content to write into the file.</param>
                                /// <param name="file">The path to the file.</param>
                                /// <param name="encode">If set, the content will be encoded as base64 (required by QEMU).Otherwise the content needs to be encoded beforehand - defaults to true.</param>
                                /// <returns></returns>
                                public async Task<Result> FileWrite(string content, string file, bool? encode = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("content", content);
                                    parameters.Add("file", file);
                                    parameters.Add("encode", encode);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent/file-write", parameters);
                                }
                            }
                            /// <summary>
                            /// QEMU Guest Agent command index.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/agent"); }
                            /// <summary>
                            /// Execute QEMU Guest Agent commands.
                            /// </summary>
                            /// <param name="command">The QGA command.
                            ///   Enum: fsfreeze-freeze,fsfreeze-status,fsfreeze-thaw,fstrim,get-fsinfo,get-host-name,get-memory-block-info,get-memory-blocks,get-osinfo,get-time,get-timezone,get-users,get-vcpus,info,network-get-interfaces,ping,shutdown,suspend-disk,suspend-hybrid,suspend-ram</param>
                            /// <returns></returns>
                            public async Task<Result> Agent(string command)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("command", command);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/agent", parameters);
                            }
                        }
                        /// <summary>
                        /// Rrd
                        /// </summary>
                        public class PveRrd
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveRrd(PveClient client, object node, object vmid)
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
                            public async Task<Result> Rrd(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/rrd", parameters);
                            }
                        }
                        /// <summary>
                        /// Rrddata
                        /// </summary>
                        public class PveRrddata
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveRrddata(PveClient client, object node, object vmid)
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
                            public async Task<Result> Rrddata(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/rrddata", parameters);
                            }
                        }
                        /// <summary>
                        /// Config
                        /// </summary>
                        public class PveConfig
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveConfig(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get the virtual machine configuration with pending configuration changes applied. Set the 'current' parameter to get the current configuration instead.
                            /// </summary>
                            /// <param name="current">Get current values (instead of pending values).</param>
                            /// <param name="snapshot">Fetch config values from given snapshot.</param>
                            /// <returns></returns>
                            public async Task<Result> VmConfig(bool? current = null, string snapshot = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("current", current);
                                parameters.Add("snapshot", snapshot);
                                return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/config", parameters);
                            }
                            /// <summary>
                            /// Set virtual machine options (asynchrounous API).
                            /// </summary>
                            /// <param name="acpi">Enable/disable ACPI.</param>
                            /// <param name="affinity">List of host cores used to execute guest processes, for example: 0,5,8-11</param>
                            /// <param name="agent">Enable/disable communication with the QEMU Guest Agent and its properties.</param>
                            /// <param name="arch">Virtual processor architecture. Defaults to the host.
                            ///   Enum: x86_64,aarch64</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="audio0">Configure a audio device, useful in combination with QXL/Spice.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="background_delay">Time to wait for the task to finish. We return 'null' if the task finish within that time.</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MiB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Specify guest boot order. Use the 'order=' sub-property as usage with no key or 'legacy=' is deprecated.</param>
                            /// <param name="bootdisk">Enable booting from specified disk. Deprecated: Use 'boot: order=foo;bar' instead.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
                            /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                            /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                            /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                            ///   Enum: configdrive2,nocloud,opennebula</param>
                            /// <param name="ciupgrade">cloud-init: do an automatic package upgrade after the first boot.</param>
                            /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                            /// <param name="cores">The number of cores per socket.</param>
                            /// <param name="cpu">Emulated CPU type.</param>
                            /// <param name="cpulimit">Limit of CPU usage.</param>
                            /// <param name="cpuunits">CPU weight for a VM, will be clamped to [1, 10000] in cgroup v2.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the VM. Shown in the web-interface VM's summary. This is saved as comment inside the configuration file.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="efidisk0">Configure a disk for storing EFI vars. Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Note that SIZE_IN_GiB is ignored here and that the default EFI vars are copied to the volume instead. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory', 'usb' and 'cloudinit'. Use '0' to disable hotplug completely. Using '1' as value is an alias for the default `network,disk,usb`. USB hotplugging is possible for guests with machine version &amp;gt;= 7.1 and ostype l26 or windows &amp;gt; 7.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration. This requires cloud-init 19.4 or newer.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                            /// <param name="keephugepages">Use together with hugepages. If enabled, hugepages will not not be deleted after VM shutdown and can be used for subsequent starts.</param>
                            /// <param name="keyboard">Keyboard layout for VNC server. This option is generally not required and is often better handled from within the guest OS.
                            ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                            /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                            /// <param name="localtime">Set the real time clock (RTC) to local time. This is enabled by default if the `ostype` indicates a Microsoft Windows OS.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                            /// <param name="machine">Specifies the QEMU machine type.</param>
                            /// <param name="memory">Memory properties.</param>
                            /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                            /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                            /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                            /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="netN">Specify network devices.</param>
                            /// <param name="numa">Enable/disable NUMA.</param>
                            /// <param name="numaN">NUMA topology.</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">Specify guest operating system.
                            ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,win11,l24,l26,solaris</param>
                            /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                            /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                            /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="rng0">Configure a VirtIO-based Random Number Generator.</param>
                            /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 30). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="scsihw">SCSI controller model
                            ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                            /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                            /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                            /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                            /// <param name="sockets">The number of CPU sockets.</param>
                            /// <param name="spice_enhancements">Configure additional enhancements for SPICE.</param>
                            /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                            /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are:'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="tablet">Enable/disable the USB tablet device.</param>
                            /// <param name="tags">Tags of the VM. This is only meta information.</param>
                            /// <param name="tdf">Enable/disable time drift fix.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="tpmstate0">Configure a Disk for storing TPM state. The format is fixed to 'raw'. Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Note that SIZE_IN_GiB is ignored here and 4 MiB will be used instead. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <param name="usbN">Configure an USB device (n is 0 to 4, for machine version &amp;gt;= 7.1 and ostype l26 or windows &amp;gt; 7, n can be up to 14).</param>
                            /// <param name="vcpus">Number of hotplugged vcpus.</param>
                            /// <param name="vga">Configure the VGA hardware.</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public async Task<Result> UpdateVmAsync(bool? acpi = null, string affinity = null, string agent = null, string arch = null, string args = null, string audio0 = null, bool? autostart = null, int? background_delay = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, bool? ciupgrade = null, string ciuser = null, int? cores = null, string cpu = null, float? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, bool? keephugepages = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, string memory = null, float? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, string rng0 = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string spice_enhancements = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, string tags = null, bool? tdf = null, bool? template = null, string tpmstate0 = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("acpi", acpi);
                                parameters.Add("affinity", affinity);
                                parameters.Add("agent", agent);
                                parameters.Add("arch", arch);
                                parameters.Add("args", args);
                                parameters.Add("audio0", audio0);
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
                                parameters.Add("ciupgrade", ciupgrade);
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
                                parameters.Add("keephugepages", keephugepages);
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
                                parameters.Add("rng0", rng0);
                                parameters.Add("scsihw", scsihw);
                                parameters.Add("searchdomain", searchdomain);
                                parameters.Add("shares", shares);
                                parameters.Add("skiplock", skiplock);
                                parameters.Add("smbios1", smbios1);
                                parameters.Add("smp", smp);
                                parameters.Add("sockets", sockets);
                                parameters.Add("spice_enhancements", spice_enhancements);
                                parameters.Add("sshkeys", sshkeys);
                                parameters.Add("startdate", startdate);
                                parameters.Add("startup", startup);
                                parameters.Add("tablet", tablet);
                                parameters.Add("tags", tags);
                                parameters.Add("tdf", tdf);
                                parameters.Add("template", template);
                                parameters.Add("tpmstate0", tpmstate0);
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
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/config", parameters);
                            }
                            /// <summary>
                            /// Set virtual machine options (synchrounous API) - You should consider using the POST method instead for any actions involving hotplug or storage allocation.
                            /// </summary>
                            /// <param name="acpi">Enable/disable ACPI.</param>
                            /// <param name="affinity">List of host cores used to execute guest processes, for example: 0,5,8-11</param>
                            /// <param name="agent">Enable/disable communication with the QEMU Guest Agent and its properties.</param>
                            /// <param name="arch">Virtual processor architecture. Defaults to the host.
                            ///   Enum: x86_64,aarch64</param>
                            /// <param name="args">Arbitrary arguments passed to kvm.</param>
                            /// <param name="audio0">Configure a audio device, useful in combination with QXL/Spice.</param>
                            /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                            /// <param name="balloon">Amount of target RAM for the VM in MiB. Using zero disables the ballon driver.</param>
                            /// <param name="bios">Select BIOS implementation.
                            ///   Enum: seabios,ovmf</param>
                            /// <param name="boot">Specify guest boot order. Use the 'order=' sub-property as usage with no key or 'legacy=' is deprecated.</param>
                            /// <param name="bootdisk">Enable booting from specified disk. Deprecated: Use 'boot: order=foo;bar' instead.</param>
                            /// <param name="cdrom">This is an alias for option -ide2</param>
                            /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                            /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                            /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                            ///   Enum: configdrive2,nocloud,opennebula</param>
                            /// <param name="ciupgrade">cloud-init: do an automatic package upgrade after the first boot.</param>
                            /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                            /// <param name="cores">The number of cores per socket.</param>
                            /// <param name="cpu">Emulated CPU type.</param>
                            /// <param name="cpulimit">Limit of CPU usage.</param>
                            /// <param name="cpuunits">CPU weight for a VM, will be clamped to [1, 10000] in cgroup v2.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the VM. Shown in the web-interface VM's summary. This is saved as comment inside the configuration file.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="efidisk0">Configure a disk for storing EFI vars. Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Note that SIZE_IN_GiB is ignored here and that the default EFI vars are copied to the volume instead. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="force">Force physical removal. Without this, we simple remove the disk from the config file and create an additional configuration entry called 'unused[n]', which contains the volume ID. Unlink of unused[n] always cause physical removal.</param>
                            /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                            /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                            /// <param name="hostpciN">Map host PCI devices into guest.</param>
                            /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory', 'usb' and 'cloudinit'. Use '0' to disable hotplug completely. Using '1' as value is an alias for the default `network,disk,usb`. USB hotplugging is possible for guests with machine version &amp;gt;= 7.1 and ostype l26 or windows &amp;gt; 7.</param>
                            /// <param name="hugepages">Enable/disable hugepages memory.
                            ///   Enum: any,2,1024</param>
                            /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration. This requires cloud-init 19.4 or newer.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                            /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                            /// <param name="keephugepages">Use together with hugepages. If enabled, hugepages will not not be deleted after VM shutdown and can be used for subsequent starts.</param>
                            /// <param name="keyboard">Keyboard layout for VNC server. This option is generally not required and is often better handled from within the guest OS.
                            ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                            /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                            /// <param name="localtime">Set the real time clock (RTC) to local time. This is enabled by default if the `ostype` indicates a Microsoft Windows OS.</param>
                            /// <param name="lock_">Lock/unlock the VM.
                            ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                            /// <param name="machine">Specifies the QEMU machine type.</param>
                            /// <param name="memory">Memory properties.</param>
                            /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                            /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                            /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                            /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="netN">Specify network devices.</param>
                            /// <param name="numa">Enable/disable NUMA.</param>
                            /// <param name="numaN">NUMA topology.</param>
                            /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                            /// <param name="ostype">Specify guest operating system.
                            ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,win11,l24,l26,solaris</param>
                            /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                            /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                            /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="rng0">Configure a VirtIO-based Random Number Generator.</param>
                            /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 30). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="scsihw">SCSI controller model
                            ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                            /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                            /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                            /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                            /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                            /// <param name="sockets">The number of CPU sockets.</param>
                            /// <param name="spice_enhancements">Configure additional enhancements for SPICE.</param>
                            /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                            /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are:'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="tablet">Enable/disable the USB tablet device.</param>
                            /// <param name="tags">Tags of the VM. This is only meta information.</param>
                            /// <param name="tdf">Enable/disable time drift fix.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="tpmstate0">Configure a Disk for storing TPM state. The format is fixed to 'raw'. Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Note that SIZE_IN_GiB is ignored here and 4 MiB will be used instead. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <param name="usbN">Configure an USB device (n is 0 to 4, for machine version &amp;gt;= 7.1 and ostype l26 or windows &amp;gt; 7, n can be up to 14).</param>
                            /// <param name="vcpus">Number of hotplugged vcpus.</param>
                            /// <param name="vga">Configure the VGA hardware.</param>
                            /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                            /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                            /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                            /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                            /// <returns></returns>
                            public async Task<Result> UpdateVm(bool? acpi = null, string affinity = null, string agent = null, string arch = null, string args = null, string audio0 = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, bool? ciupgrade = null, string ciuser = null, int? cores = null, string cpu = null, float? cpulimit = null, int? cpuunits = null, string delete = null, string description = null, string digest = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, bool? keephugepages = null, string keyboard = null, bool? kvm = null, bool? localtime = null, string lock_ = null, string machine = null, string memory = null, float? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, bool? protection = null, bool? reboot = null, string revert = null, string rng0 = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, bool? skiplock = null, string smbios1 = null, int? smp = null, int? sockets = null, string spice_enhancements = null, string sshkeys = null, string startdate = null, string startup = null, bool? tablet = null, string tags = null, bool? tdf = null, bool? template = null, string tpmstate0 = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("acpi", acpi);
                                parameters.Add("affinity", affinity);
                                parameters.Add("agent", agent);
                                parameters.Add("arch", arch);
                                parameters.Add("args", args);
                                parameters.Add("audio0", audio0);
                                parameters.Add("autostart", autostart);
                                parameters.Add("balloon", balloon);
                                parameters.Add("bios", bios);
                                parameters.Add("boot", boot);
                                parameters.Add("bootdisk", bootdisk);
                                parameters.Add("cdrom", cdrom);
                                parameters.Add("cicustom", cicustom);
                                parameters.Add("cipassword", cipassword);
                                parameters.Add("citype", citype);
                                parameters.Add("ciupgrade", ciupgrade);
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
                                parameters.Add("keephugepages", keephugepages);
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
                                parameters.Add("rng0", rng0);
                                parameters.Add("scsihw", scsihw);
                                parameters.Add("searchdomain", searchdomain);
                                parameters.Add("shares", shares);
                                parameters.Add("skiplock", skiplock);
                                parameters.Add("smbios1", smbios1);
                                parameters.Add("smp", smp);
                                parameters.Add("sockets", sockets);
                                parameters.Add("spice_enhancements", spice_enhancements);
                                parameters.Add("sshkeys", sshkeys);
                                parameters.Add("startdate", startdate);
                                parameters.Add("startup", startup);
                                parameters.Add("tablet", tablet);
                                parameters.Add("tags", tags);
                                parameters.Add("tdf", tdf);
                                parameters.Add("template", template);
                                parameters.Add("tpmstate0", tpmstate0);
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
                                return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/config", parameters);
                            }
                        }
                        /// <summary>
                        /// Pending
                        /// </summary>
                        public class PvePending
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PvePending(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get the virtual machine configuration with both current and pending values.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> VmPending() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/pending"); }
                        }
                        /// <summary>
                        /// Cloudinit
                        /// </summary>
                        public class PveCloudinit
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveCloudinit(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PveDump _dump;
                            /// <summary>
                            /// Dump
                            /// </summary>
                            public PveDump Dump => _dump ??= new(_client, _node, _vmid);
                            /// <summary>
                            /// Dump
                            /// </summary>
                            public class PveDump
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveDump(PveClient client, object node, object vmid)
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
                                public async Task<Result> CloudinitGeneratedConfigDump(string type)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/cloudinit/dump", parameters);
                                }
                            }
                            /// <summary>
                            /// Get the cloudinit configuration with both current and pending values.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> CloudinitPending() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/cloudinit"); }
                            /// <summary>
                            /// Regenerate and change cloudinit config drive.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> CloudinitUpdate() { return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/cloudinit"); }
                        }
                        /// <summary>
                        /// Unlink
                        /// </summary>
                        public class PveUnlink
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveUnlink(PveClient client, object node, object vmid)
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
                            public async Task<Result> Unlink(string idlist, bool? force = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("idlist", idlist);
                                parameters.Add("force", force);
                                return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/unlink", parameters);
                            }
                        }
                        /// <summary>
                        /// Vncproxy
                        /// </summary>
                        public class PveVncproxy
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveVncproxy(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Creates a TCP VNC proxy connections.
                            /// </summary>
                            /// <param name="generate_password">Generates a random password to be used as ticket instead of the API ticket.</param>
                            /// <param name="websocket">Prepare for websocket upgrade (only required when using serial terminal, otherwise upgrade is always possible).</param>
                            /// <returns></returns>
                            public async Task<Result> Vncproxy(bool? generate_password = null, bool? websocket = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("generate-password", generate_password);
                                parameters.Add("websocket", websocket);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/vncproxy", parameters);
                            }
                        }
                        /// <summary>
                        /// Termproxy
                        /// </summary>
                        public class PveTermproxy
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveTermproxy(PveClient client, object node, object vmid)
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
                            public async Task<Result> Termproxy(string serial = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("serial", serial);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/termproxy", parameters);
                            }
                        }
                        /// <summary>
                        /// Vncwebsocket
                        /// </summary>
                        public class PveVncwebsocket
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveVncwebsocket(PveClient client, object node, object vmid)
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
                            public async Task<Result> Vncwebsocket(int port, string vncticket)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("port", port);
                                parameters.Add("vncticket", vncticket);
                                return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/vncwebsocket", parameters);
                            }
                        }
                        /// <summary>
                        /// Spiceproxy
                        /// </summary>
                        public class PveSpiceproxy
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveSpiceproxy(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Returns a SPICE configuration to connect to the VM.
                            /// </summary>
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            /// <returns></returns>
                            public async Task<Result> Spiceproxy(string proxy = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("proxy", proxy);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/spiceproxy", parameters);
                            }
                        }
                        /// <summary>
                        /// Status
                        /// </summary>
                        public class PveStatus
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveStatus(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PveCurrent _current;
                            /// <summary>
                            /// Current
                            /// </summary>
                            public PveCurrent Current => _current ??= new(_client, _node, _vmid);
                            private PveStart _start;
                            /// <summary>
                            /// Start
                            /// </summary>
                            public PveStart Start => _start ??= new(_client, _node, _vmid);
                            private PveStop _stop;
                            /// <summary>
                            /// Stop
                            /// </summary>
                            public PveStop Stop => _stop ??= new(_client, _node, _vmid);
                            private PveReset _reset;
                            /// <summary>
                            /// Reset
                            /// </summary>
                            public PveReset Reset => _reset ??= new(_client, _node, _vmid);
                            private PveShutdown _shutdown;
                            /// <summary>
                            /// Shutdown
                            /// </summary>
                            public PveShutdown Shutdown => _shutdown ??= new(_client, _node, _vmid);
                            private PveReboot _reboot;
                            /// <summary>
                            /// Reboot
                            /// </summary>
                            public PveReboot Reboot => _reboot ??= new(_client, _node, _vmid);
                            private PveSuspend _suspend;
                            /// <summary>
                            /// Suspend
                            /// </summary>
                            public PveSuspend Suspend => _suspend ??= new(_client, _node, _vmid);
                            private PveResume _resume;
                            /// <summary>
                            /// Resume
                            /// </summary>
                            public PveResume Resume => _resume ??= new(_client, _node, _vmid);
                            /// <summary>
                            /// Current
                            /// </summary>
                            public class PveCurrent
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveCurrent(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> VmStatus() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/status/current"); }
                            }
                            /// <summary>
                            /// Start
                            /// </summary>
                            public class PveStart
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveStart(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Start virtual machine.
                                /// </summary>
                                /// <param name="force_cpu">Override QEMU's -cpu argument with the given string.</param>
                                /// <param name="machine">Specifies the QEMU machine type.</param>
                                /// <param name="migratedfrom">The cluster node name.</param>
                                /// <param name="migration_network">CIDR of the (sub) network that is used for migration.</param>
                                /// <param name="migration_type">Migration traffic is encrypted using an SSH tunnel by default. On secure, completely private networks this can be disabled to increase performance.
                                ///   Enum: secure,insecure</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <param name="stateuri">Some command save/restore state from this location.</param>
                                /// <param name="targetstorage">Mapping from source to target storages. Providing only a single storage ID maps all source storages to that storage. Providing the special value '1' will map each source storage to itself.</param>
                                /// <param name="timeout">Wait maximal timeout seconds.</param>
                                /// <returns></returns>
                                public async Task<Result> VmStart(string force_cpu = null, string machine = null, string migratedfrom = null, string migration_network = null, string migration_type = null, bool? skiplock = null, string stateuri = null, string targetstorage = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("force-cpu", force_cpu);
                                    parameters.Add("machine", machine);
                                    parameters.Add("migratedfrom", migratedfrom);
                                    parameters.Add("migration_network", migration_network);
                                    parameters.Add("migration_type", migration_type);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("stateuri", stateuri);
                                    parameters.Add("targetstorage", targetstorage);
                                    parameters.Add("timeout", timeout);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/start", parameters);
                                }
                            }
                            /// <summary>
                            /// Stop
                            /// </summary>
                            public class PveStop
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveStop(PveClient client, object node, object vmid)
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
                                public async Task<Result> VmStop(bool? keepActive = null, string migratedfrom = null, bool? skiplock = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("keepActive", keepActive);
                                    parameters.Add("migratedfrom", migratedfrom);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("timeout", timeout);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/stop", parameters);
                                }
                            }
                            /// <summary>
                            /// Reset
                            /// </summary>
                            public class PveReset
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveReset(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Reset virtual machine.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public async Task<Result> VmReset(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/reset", parameters);
                                }
                            }
                            /// <summary>
                            /// Shutdown
                            /// </summary>
                            public class PveShutdown
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveShutdown(PveClient client, object node, object vmid)
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
                                public async Task<Result> VmShutdown(bool? forceStop = null, bool? keepActive = null, bool? skiplock = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("forceStop", forceStop);
                                    parameters.Add("keepActive", keepActive);
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("timeout", timeout);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/shutdown", parameters);
                                }
                            }
                            /// <summary>
                            /// Reboot
                            /// </summary>
                            public class PveReboot
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveReboot(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Reboot the VM by shutting it down, and starting it again. Applies pending changes.
                                /// </summary>
                                /// <param name="timeout">Wait maximal timeout seconds for the shutdown.</param>
                                /// <returns></returns>
                                public async Task<Result> VmReboot(int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("timeout", timeout);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/reboot", parameters);
                                }
                            }
                            /// <summary>
                            /// Suspend
                            /// </summary>
                            public class PveSuspend
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveSuspend(PveClient client, object node, object vmid)
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
                                public async Task<Result> VmSuspend(bool? skiplock = null, string statestorage = null, bool? todisk = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    parameters.Add("statestorage", statestorage);
                                    parameters.Add("todisk", todisk);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/suspend", parameters);
                                }
                            }
                            /// <summary>
                            /// Resume
                            /// </summary>
                            public class PveResume
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveResume(PveClient client, object node, object vmid)
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
                                public async Task<Result> VmResume(bool? nocheck = null, bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("nocheck", nocheck);
                                    parameters.Add("skiplock", skiplock);
                                    return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/status/resume", parameters);
                                }
                            }
                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Vmcmdidx() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/status"); }
                        }
                        /// <summary>
                        /// Sendkey
                        /// </summary>
                        public class PveSendkey
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveSendkey(PveClient client, object node, object vmid)
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
                            public async Task<Result> VmSendkey(string key, bool? skiplock = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("key", key);
                                parameters.Add("skiplock", skiplock);
                                return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/sendkey", parameters);
                            }
                        }
                        /// <summary>
                        /// Feature
                        /// </summary>
                        public class PveFeature
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveFeature(PveClient client, object node, object vmid)
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
                            public async Task<Result> VmFeature(string feature, string snapname = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("feature", feature);
                                parameters.Add("snapname", snapname);
                                return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/feature", parameters);
                            }
                        }
                        /// <summary>
                        /// Clone
                        /// </summary>
                        public class PveClone
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveClone(PveClient client, object node, object vmid)
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
                            public async Task<Result> CloneVm(int newid, int? bwlimit = null, string description = null, string format = null, bool? full = null, string name = null, string pool = null, string snapname = null, string storage = null, string target = null)
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
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/clone", parameters);
                            }
                        }
                        /// <summary>
                        /// MoveDisk
                        /// </summary>
                        public class PveMoveDisk
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMoveDisk(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Move volume to different storage or to a different VM.
                            /// </summary>
                            /// <param name="disk">The disk you want to move.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,scsi14,scsi15,scsi16,scsi17,scsi18,scsi19,scsi20,scsi21,scsi22,scsi23,scsi24,scsi25,scsi26,scsi27,scsi28,scsi29,scsi30,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0,tpmstate0,unused0,unused1,unused2,unused3,unused4,unused5,unused6,unused7,unused8,unused9,unused10,unused11,unused12,unused13,unused14,unused15,unused16,unused17,unused18,unused19,unused20,unused21,unused22,unused23,unused24,unused25,unused26,unused27,unused28,unused29,unused30,unused31,unused32,unused33,unused34,unused35,unused36,unused37,unused38,unused39,unused40,unused41,unused42,unused43,unused44,unused45,unused46,unused47,unused48,unused49,unused50,unused51,unused52,unused53,unused54,unused55,unused56,unused57,unused58,unused59,unused60,unused61,unused62,unused63,unused64,unused65,unused66,unused67,unused68,unused69,unused70,unused71,unused72,unused73,unused74,unused75,unused76,unused77,unused78,unused79,unused80,unused81,unused82,unused83,unused84,unused85,unused86,unused87,unused88,unused89,unused90,unused91,unused92,unused93,unused94,unused95,unused96,unused97,unused98,unused99,unused100,unused101,unused102,unused103,unused104,unused105,unused106,unused107,unused108,unused109,unused110,unused111,unused112,unused113,unused114,unused115,unused116,unused117,unused118,unused119,unused120,unused121,unused122,unused123,unused124,unused125,unused126,unused127,unused128,unused129,unused130,unused131,unused132,unused133,unused134,unused135,unused136,unused137,unused138,unused139,unused140,unused141,unused142,unused143,unused144,unused145,unused146,unused147,unused148,unused149,unused150,unused151,unused152,unused153,unused154,unused155,unused156,unused157,unused158,unused159,unused160,unused161,unused162,unused163,unused164,unused165,unused166,unused167,unused168,unused169,unused170,unused171,unused172,unused173,unused174,unused175,unused176,unused177,unused178,unused179,unused180,unused181,unused182,unused183,unused184,unused185,unused186,unused187,unused188,unused189,unused190,unused191,unused192,unused193,unused194,unused195,unused196,unused197,unused198,unused199,unused200,unused201,unused202,unused203,unused204,unused205,unused206,unused207,unused208,unused209,unused210,unused211,unused212,unused213,unused214,unused215,unused216,unused217,unused218,unused219,unused220,unused221,unused222,unused223,unused224,unused225,unused226,unused227,unused228,unused229,unused230,unused231,unused232,unused233,unused234,unused235,unused236,unused237,unused238,unused239,unused240,unused241,unused242,unused243,unused244,unused245,unused246,unused247,unused248,unused249,unused250,unused251,unused252,unused253,unused254,unused255</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="delete">Delete the original disk after successful copy. By default the original disk is kept as unused disk.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1" 		    ." digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="format">Target Format.
                            ///   Enum: raw,qcow2,vmdk</param>
                            /// <param name="storage">Target storage.</param>
                            /// <param name="target_digest">Prevent changes if the current config file of the target VM has a" 		    ." different SHA1 digest. This can be used to detect concurrent modifications.</param>
                            /// <param name="target_disk">The config key the disk will be moved to on the target VM (for example, ide0 or scsi1). Default is the source disk key.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,scsi14,scsi15,scsi16,scsi17,scsi18,scsi19,scsi20,scsi21,scsi22,scsi23,scsi24,scsi25,scsi26,scsi27,scsi28,scsi29,scsi30,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0,tpmstate0,unused0,unused1,unused2,unused3,unused4,unused5,unused6,unused7,unused8,unused9,unused10,unused11,unused12,unused13,unused14,unused15,unused16,unused17,unused18,unused19,unused20,unused21,unused22,unused23,unused24,unused25,unused26,unused27,unused28,unused29,unused30,unused31,unused32,unused33,unused34,unused35,unused36,unused37,unused38,unused39,unused40,unused41,unused42,unused43,unused44,unused45,unused46,unused47,unused48,unused49,unused50,unused51,unused52,unused53,unused54,unused55,unused56,unused57,unused58,unused59,unused60,unused61,unused62,unused63,unused64,unused65,unused66,unused67,unused68,unused69,unused70,unused71,unused72,unused73,unused74,unused75,unused76,unused77,unused78,unused79,unused80,unused81,unused82,unused83,unused84,unused85,unused86,unused87,unused88,unused89,unused90,unused91,unused92,unused93,unused94,unused95,unused96,unused97,unused98,unused99,unused100,unused101,unused102,unused103,unused104,unused105,unused106,unused107,unused108,unused109,unused110,unused111,unused112,unused113,unused114,unused115,unused116,unused117,unused118,unused119,unused120,unused121,unused122,unused123,unused124,unused125,unused126,unused127,unused128,unused129,unused130,unused131,unused132,unused133,unused134,unused135,unused136,unused137,unused138,unused139,unused140,unused141,unused142,unused143,unused144,unused145,unused146,unused147,unused148,unused149,unused150,unused151,unused152,unused153,unused154,unused155,unused156,unused157,unused158,unused159,unused160,unused161,unused162,unused163,unused164,unused165,unused166,unused167,unused168,unused169,unused170,unused171,unused172,unused173,unused174,unused175,unused176,unused177,unused178,unused179,unused180,unused181,unused182,unused183,unused184,unused185,unused186,unused187,unused188,unused189,unused190,unused191,unused192,unused193,unused194,unused195,unused196,unused197,unused198,unused199,unused200,unused201,unused202,unused203,unused204,unused205,unused206,unused207,unused208,unused209,unused210,unused211,unused212,unused213,unused214,unused215,unused216,unused217,unused218,unused219,unused220,unused221,unused222,unused223,unused224,unused225,unused226,unused227,unused228,unused229,unused230,unused231,unused232,unused233,unused234,unused235,unused236,unused237,unused238,unused239,unused240,unused241,unused242,unused243,unused244,unused245,unused246,unused247,unused248,unused249,unused250,unused251,unused252,unused253,unused254,unused255</param>
                            /// <param name="target_vmid">The (unique) ID of the VM.</param>
                            /// <returns></returns>
                            public async Task<Result> MoveVmDisk(string disk, int? bwlimit = null, bool? delete = null, string digest = null, string format = null, string storage = null, string target_digest = null, string target_disk = null, int? target_vmid = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                parameters.Add("format", format);
                                parameters.Add("storage", storage);
                                parameters.Add("target-digest", target_digest);
                                parameters.Add("target-disk", target_disk);
                                parameters.Add("target-vmid", target_vmid);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/move_disk", parameters);
                            }
                        }
                        /// <summary>
                        /// Migrate
                        /// </summary>
                        public class PveMigrate
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMigrate(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get preconditions for migration.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <returns></returns>
                            public async Task<Result> MigrateVmPrecondition(string target = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target", target);
                                return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/migrate", parameters);
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
                            /// <param name="online">Use online/live migration if VM is running. Ignored if VM is stopped.</param>
                            /// <param name="targetstorage">Mapping from source to target storages. Providing only a single storage ID maps all source storages to that storage. Providing the special value '1' will map each source storage to itself.</param>
                            /// <param name="with_local_disks">Enable live storage migration for local disk</param>
                            /// <returns></returns>
                            public async Task<Result> MigrateVm(string target, int? bwlimit = null, bool? force = null, string migration_network = null, string migration_type = null, bool? online = null, string targetstorage = null, bool? with_local_disks = null)
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
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/migrate", parameters);
                            }
                        }
                        /// <summary>
                        /// RemoteMigrate
                        /// </summary>
                        public class PveRemoteMigrate
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveRemoteMigrate(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Migrate virtual machine to a remote cluster. Creates a new migration task. EXPERIMENTAL feature!
                            /// </summary>
                            /// <param name="target_bridge">Mapping from source to target bridges. Providing only a single bridge ID maps all source bridges to that bridge. Providing the special value '1' will map each source bridge to itself.</param>
                            /// <param name="target_endpoint">Remote target endpoint</param>
                            /// <param name="target_storage">Mapping from source to target storages. Providing only a single storage ID maps all source storages to that storage. Providing the special value '1' will map each source storage to itself.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="delete">Delete the original VM and related data after successful migration. By default the original VM is kept on the source cluster in a stopped state.</param>
                            /// <param name="online">Use online/live migration if VM is running. Ignored if VM is stopped.</param>
                            /// <param name="target_vmid">The (unique) ID of the VM.</param>
                            /// <returns></returns>
                            public async Task<Result> RemoteMigrateVm(string target_bridge, string target_endpoint, string target_storage, int? bwlimit = null, bool? delete = null, bool? online = null, int? target_vmid = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target-bridge", target_bridge);
                                parameters.Add("target-endpoint", target_endpoint);
                                parameters.Add("target-storage", target_storage);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("delete", delete);
                                parameters.Add("online", online);
                                parameters.Add("target-vmid", target_vmid);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/remote_migrate", parameters);
                            }
                        }
                        /// <summary>
                        /// Monitor
                        /// </summary>
                        public class PveMonitor
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMonitor(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Execute QEMU monitor commands.
                            /// </summary>
                            /// <param name="command">The monitor command.</param>
                            /// <returns></returns>
                            public async Task<Result> Monitor(string command)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("command", command);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/monitor", parameters);
                            }
                        }
                        /// <summary>
                        /// Resize
                        /// </summary>
                        public class PveResize
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveResize(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Extend volume size.
                            /// </summary>
                            /// <param name="disk">The disk you want to resize.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,scsi14,scsi15,scsi16,scsi17,scsi18,scsi19,scsi20,scsi21,scsi22,scsi23,scsi24,scsi25,scsi26,scsi27,scsi28,scsi29,scsi30,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0,tpmstate0</param>
                            /// <param name="size">The new size. With the `+` sign the value is added to the actual size of the volume and without it, the value is taken as an absolute one. Shrinking disk size is not supported.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                            /// <returns></returns>
                            public async Task<Result> ResizeVm(string disk, string size, string digest = null, bool? skiplock = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                parameters.Add("size", size);
                                parameters.Add("digest", digest);
                                parameters.Add("skiplock", skiplock);
                                return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/resize", parameters);
                            }
                        }
                        /// <summary>
                        /// Snapshot
                        /// </summary>
                        public class PveSnapshot
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveSnapshot(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// SnapnameItem
                            /// </summary>
                            public PveSnapnameItem this[object snapname] => new(_client, _node, _vmid, snapname);
                            /// <summary>
                            /// SnapnameItem
                            /// </summary>
                            public class PveSnapnameItem
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                private readonly object _snapname;
                                internal PveSnapnameItem(PveClient client, object node, object vmid, object snapname)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                    _snapname = snapname;
                                }
                                private PveConfig _config;
                                /// <summary>
                                /// Config
                                /// </summary>
                                public PveConfig Config => _config ??= new(_client, _node, _vmid, _snapname);
                                private PveRollback _rollback;
                                /// <summary>
                                /// Rollback
                                /// </summary>
                                public PveRollback Rollback => _rollback ??= new(_client, _node, _vmid, _snapname);
                                /// <summary>
                                /// Config
                                /// </summary>
                                public class PveConfig
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _snapname;
                                    internal PveConfig(PveClient client, object node, object vmid, object snapname)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }
                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    /// <returns></returns>
                                    public async Task<Result> GetSnapshotConfig() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/config"); }
                                    /// <summary>
                                    /// Update snapshot metadata.
                                    /// </summary>
                                    /// <param name="description">A textual description or comment.</param>
                                    /// <returns></returns>
                                    public async Task<Result> UpdateSnapshotConfig(string description = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("description", description);
                                        return await _client.Set($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/config", parameters);
                                    }
                                }
                                /// <summary>
                                /// Rollback
                                /// </summary>
                                public class PveRollback
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _snapname;
                                    internal PveRollback(PveClient client, object node, object vmid, object snapname)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }
                                    /// <summary>
                                    /// Rollback VM state to specified snapshot.
                                    /// </summary>
                                    /// <param name="start">Whether the VM should get started after rolling back successfully. (Note: VMs will be automatically started if the snapshot includes RAM.)</param>
                                    /// <returns></returns>
                                    public async Task<Result> Rollback(bool? start = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("start", start);
                                        return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}/rollback", parameters);
                                    }
                                }
                                /// <summary>
                                /// Delete a VM snapshot.
                                /// </summary>
                                /// <param name="force">For removal from config file, even if removing disk snapshots fails.</param>
                                /// <returns></returns>
                                public async Task<Result> Delsnapshot(bool? force = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("force", force);
                                    return await _client.Delete($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}", parameters);
                                }
                                /// <summary>
                                ///
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> SnapshotCmdIdx() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot/{_snapname}"); }
                            }
                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> SnapshotList() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/snapshot"); }
                            /// <summary>
                            /// Snapshot a VM.
                            /// </summary>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="description">A textual description or comment.</param>
                            /// <param name="vmstate">Save the vmstate</param>
                            /// <returns></returns>
                            public async Task<Result> Snapshot(string snapname, string description = null, bool? vmstate = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("snapname", snapname);
                                parameters.Add("description", description);
                                parameters.Add("vmstate", vmstate);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/snapshot", parameters);
                            }
                        }
                        /// <summary>
                        /// Template
                        /// </summary>
                        public class PveTemplate
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveTemplate(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <param name="disk">If you want to convert only 1 disk to base image.
                            ///   Enum: ide0,ide1,ide2,ide3,scsi0,scsi1,scsi2,scsi3,scsi4,scsi5,scsi6,scsi7,scsi8,scsi9,scsi10,scsi11,scsi12,scsi13,scsi14,scsi15,scsi16,scsi17,scsi18,scsi19,scsi20,scsi21,scsi22,scsi23,scsi24,scsi25,scsi26,scsi27,scsi28,scsi29,scsi30,virtio0,virtio1,virtio2,virtio3,virtio4,virtio5,virtio6,virtio7,virtio8,virtio9,virtio10,virtio11,virtio12,virtio13,virtio14,virtio15,sata0,sata1,sata2,sata3,sata4,sata5,efidisk0,tpmstate0</param>
                            /// <returns></returns>
                            public async Task<Result> Template(string disk = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/template", parameters);
                            }
                        }
                        /// <summary>
                        /// Mtunnel
                        /// </summary>
                        public class PveMtunnel
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMtunnel(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Migration tunnel endpoint - only for internal use by VM migration.
                            /// </summary>
                            /// <param name="bridges">List of network bridges to check availability. Will be checked again for actually used bridges during migration.</param>
                            /// <param name="storages">List of storages to check permission and availability. Will be checked again for all actually used storages during migration.</param>
                            /// <returns></returns>
                            public async Task<Result> Mtunnel(string bridges = null, string storages = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("bridges", bridges);
                                parameters.Add("storages", storages);
                                return await _client.Create($"/nodes/{_node}/qemu/{_vmid}/mtunnel", parameters);
                            }
                        }
                        /// <summary>
                        /// Mtunnelwebsocket
                        /// </summary>
                        public class PveMtunnelwebsocket
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMtunnelwebsocket(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Migration tunnel endpoint for websocket upgrade - only for internal use by VM migration.
                            /// </summary>
                            /// <param name="socket">unix socket to forward to</param>
                            /// <param name="ticket">ticket return by initial 'mtunnel' API call, or retrieved via 'ticket' tunnel command</param>
                            /// <returns></returns>
                            public async Task<Result> Mtunnelwebsocket(string socket, string ticket)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("socket", socket);
                                parameters.Add("ticket", ticket);
                                return await _client.Get($"/nodes/{_node}/qemu/{_vmid}/mtunnelwebsocket", parameters);
                            }
                        }
                        /// <summary>
                        /// Destroy the VM and  all used/owned volumes. Removes any VM specific permissions and firewall rules
                        /// </summary>
                        /// <param name="destroy_unreferenced_disks">If set, destroy additionally all disks not referenced in the config but with a matching VMID from all enabled storages.</param>
                        /// <param name="purge">Remove VMID from configurations, like backup &amp; replication jobs and HA.</param>
                        /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                        /// <returns></returns>
                        public async Task<Result> DestroyVm(bool? destroy_unreferenced_disks = null, bool? purge = null, bool? skiplock = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("destroy-unreferenced-disks", destroy_unreferenced_disks);
                            parameters.Add("purge", purge);
                            parameters.Add("skiplock", skiplock);
                            return await _client.Delete($"/nodes/{_node}/qemu/{_vmid}", parameters);
                        }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Vmdiridx() { return await _client.Get($"/nodes/{_node}/qemu/{_vmid}"); }
                    }
                    /// <summary>
                    /// Virtual machine index (per node).
                    /// </summary>
                    /// <param name="full">Determine the full status of active VMs.</param>
                    /// <returns></returns>
                    public async Task<Result> Vmlist(bool? full = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("full", full);
                        return await _client.Get($"/nodes/{_node}/qemu", parameters);
                    }
                    /// <summary>
                    /// Create or restore a virtual machine.
                    /// </summary>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="acpi">Enable/disable ACPI.</param>
                    /// <param name="affinity">List of host cores used to execute guest processes, for example: 0,5,8-11</param>
                    /// <param name="agent">Enable/disable communication with the QEMU Guest Agent and its properties.</param>
                    /// <param name="arch">Virtual processor architecture. Defaults to the host.
                    ///   Enum: x86_64,aarch64</param>
                    /// <param name="archive">The backup archive. Either the file system path to a .tar or .vma file (use '-' to pipe data from stdin) or a proxmox storage backup volume identifier.</param>
                    /// <param name="args">Arbitrary arguments passed to kvm.</param>
                    /// <param name="audio0">Configure a audio device, useful in combination with QXL/Spice.</param>
                    /// <param name="autostart">Automatic restart after crash (currently ignored).</param>
                    /// <param name="balloon">Amount of target RAM for the VM in MiB. Using zero disables the ballon driver.</param>
                    /// <param name="bios">Select BIOS implementation.
                    ///   Enum: seabios,ovmf</param>
                    /// <param name="boot">Specify guest boot order. Use the 'order=' sub-property as usage with no key or 'legacy=' is deprecated.</param>
                    /// <param name="bootdisk">Enable booting from specified disk. Deprecated: Use 'boot: order=foo;bar' instead.</param>
                    /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                    /// <param name="cdrom">This is an alias for option -ide2</param>
                    /// <param name="cicustom">cloud-init: Specify custom files to replace the automatically generated ones at start.</param>
                    /// <param name="cipassword">cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.</param>
                    /// <param name="citype">Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
                    ///   Enum: configdrive2,nocloud,opennebula</param>
                    /// <param name="ciupgrade">cloud-init: do an automatic package upgrade after the first boot.</param>
                    /// <param name="ciuser">cloud-init: User name to change ssh keys and password for instead of the image's configured default user.</param>
                    /// <param name="cores">The number of cores per socket.</param>
                    /// <param name="cpu">Emulated CPU type.</param>
                    /// <param name="cpulimit">Limit of CPU usage.</param>
                    /// <param name="cpuunits">CPU weight for a VM, will be clamped to [1, 10000] in cgroup v2.</param>
                    /// <param name="description">Description for the VM. Shown in the web-interface VM's summary. This is saved as comment inside the configuration file.</param>
                    /// <param name="efidisk0">Configure a disk for storing EFI vars. Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Note that SIZE_IN_GiB is ignored here and that the default EFI vars are copied to the volume instead. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                    /// <param name="force">Allow to overwrite existing VM.</param>
                    /// <param name="freeze">Freeze CPU at startup (use 'c' monitor command to start execution).</param>
                    /// <param name="hookscript">Script that will be executed during various steps in the vms lifetime.</param>
                    /// <param name="hostpciN">Map host PCI devices into guest.</param>
                    /// <param name="hotplug">Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory', 'usb' and 'cloudinit'. Use '0' to disable hotplug completely. Using '1' as value is an alias for the default `network,disk,usb`. USB hotplugging is possible for guests with machine version &amp;gt;= 7.1 and ostype l26 or windows &amp;gt; 7.</param>
                    /// <param name="hugepages">Enable/disable hugepages memory.
                    ///   Enum: any,2,1024</param>
                    /// <param name="ideN">Use volume as IDE hard disk or CD-ROM (n is 0 to 3). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                    /// <param name="ipconfigN">cloud-init: Specify IP addresses and gateways for the corresponding interface.  IP addresses use CIDR notation, gateways are optional but need an IP of the same type specified.  The special string 'dhcp' can be used for IP addresses to use DHCP, in which case no explicit gateway should be provided. For IPv6 the special string 'auto' can be used to use stateless autoconfiguration. This requires cloud-init 19.4 or newer.  If cloud-init is enabled and neither an IPv4 nor an IPv6 address is specified, it defaults to using dhcp on IPv4. </param>
                    /// <param name="ivshmem">Inter-VM shared memory. Useful for direct communication between VMs, or to the host.</param>
                    /// <param name="keephugepages">Use together with hugepages. If enabled, hugepages will not not be deleted after VM shutdown and can be used for subsequent starts.</param>
                    /// <param name="keyboard">Keyboard layout for VNC server. This option is generally not required and is often better handled from within the guest OS.
                    ///   Enum: de,de-ch,da,en-gb,en-us,es,fi,fr,fr-be,fr-ca,fr-ch,hu,is,it,ja,lt,mk,nl,no,pl,pt,pt-br,sv,sl,tr</param>
                    /// <param name="kvm">Enable/disable KVM hardware virtualization.</param>
                    /// <param name="live_restore">Start the VM immediately from the backup and restore in background. PBS only.</param>
                    /// <param name="localtime">Set the real time clock (RTC) to local time. This is enabled by default if the `ostype` indicates a Microsoft Windows OS.</param>
                    /// <param name="lock_">Lock/unlock the VM.
                    ///   Enum: backup,clone,create,migrate,rollback,snapshot,snapshot-delete,suspending,suspended</param>
                    /// <param name="machine">Specifies the QEMU machine type.</param>
                    /// <param name="memory">Memory properties.</param>
                    /// <param name="migrate_downtime">Set maximum tolerated downtime (in seconds) for migrations.</param>
                    /// <param name="migrate_speed">Set maximum speed (in MB/s) for migrations. Value 0 is no limit.</param>
                    /// <param name="name">Set a name for the VM. Only used on the configuration web interface.</param>
                    /// <param name="nameserver">cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                    /// <param name="netN">Specify network devices.</param>
                    /// <param name="numa">Enable/disable NUMA.</param>
                    /// <param name="numaN">NUMA topology.</param>
                    /// <param name="onboot">Specifies whether a VM will be started during system bootup.</param>
                    /// <param name="ostype">Specify guest operating system.
                    ///   Enum: other,wxp,w2k,w2k3,w2k8,wvista,win7,win8,win10,win11,l24,l26,solaris</param>
                    /// <param name="parallelN">Map host parallel devices (n is 0 to 2).</param>
                    /// <param name="pool">Add the VM to the specified pool.</param>
                    /// <param name="protection">Sets the protection flag of the VM. This will disable the remove VM and remove disk operations.</param>
                    /// <param name="reboot">Allow reboot. If set to '0' the VM exit on reboot.</param>
                    /// <param name="rng0">Configure a VirtIO-based Random Number Generator.</param>
                    /// <param name="sataN">Use volume as SATA hard disk or CD-ROM (n is 0 to 5). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                    /// <param name="scsiN">Use volume as SCSI hard disk or CD-ROM (n is 0 to 30). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                    /// <param name="scsihw">SCSI controller model
                    ///   Enum: lsi,lsi53c810,virtio-scsi-pci,virtio-scsi-single,megasas,pvscsi</param>
                    /// <param name="searchdomain">cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.</param>
                    /// <param name="serialN">Create a serial device inside the VM (n is 0 to 3)</param>
                    /// <param name="shares">Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-ballooning is done by pvestatd.</param>
                    /// <param name="smbios1">Specify SMBIOS type 1 fields.</param>
                    /// <param name="smp">The number of CPUs. Please use option -sockets instead.</param>
                    /// <param name="sockets">The number of CPU sockets.</param>
                    /// <param name="spice_enhancements">Configure additional enhancements for SPICE.</param>
                    /// <param name="sshkeys">cloud-init: Setup public SSH keys (one key per line, OpenSSH format).</param>
                    /// <param name="start">Start VM after it was created successfully.</param>
                    /// <param name="startdate">Set the initial date of the real time clock. Valid format for date are:'now' or '2006-06-17T16:01:21' or '2006-06-17'.</param>
                    /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                    /// <param name="storage">Default storage.</param>
                    /// <param name="tablet">Enable/disable the USB tablet device.</param>
                    /// <param name="tags">Tags of the VM. This is only meta information.</param>
                    /// <param name="tdf">Enable/disable time drift fix.</param>
                    /// <param name="template">Enable/disable Template.</param>
                    /// <param name="tpmstate0">Configure a Disk for storing TPM state. The format is fixed to 'raw'. Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Note that SIZE_IN_GiB is ignored here and 4 MiB will be used instead. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                    /// <param name="unique">Assign a unique random ethernet address.</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <param name="usbN">Configure an USB device (n is 0 to 4, for machine version &amp;gt;= 7.1 and ostype l26 or windows &amp;gt; 7, n can be up to 14).</param>
                    /// <param name="vcpus">Number of hotplugged vcpus.</param>
                    /// <param name="vga">Configure the VGA hardware.</param>
                    /// <param name="virtioN">Use volume as VIRTIO hard disk (n is 0 to 15). Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume. Use STORAGE_ID:0 and the 'import-from' parameter to import from an existing volume.</param>
                    /// <param name="vmgenid">Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.</param>
                    /// <param name="vmstatestorage">Default storage for VM state volumes/files.</param>
                    /// <param name="watchdog">Create a virtual hardware watchdog device.</param>
                    /// <returns></returns>
                    public async Task<Result> CreateVm(int vmid, bool? acpi = null, string affinity = null, string agent = null, string arch = null, string archive = null, string args = null, string audio0 = null, bool? autostart = null, int? balloon = null, string bios = null, string boot = null, string bootdisk = null, int? bwlimit = null, string cdrom = null, string cicustom = null, string cipassword = null, string citype = null, bool? ciupgrade = null, string ciuser = null, int? cores = null, string cpu = null, float? cpulimit = null, int? cpuunits = null, string description = null, string efidisk0 = null, bool? force = null, bool? freeze = null, string hookscript = null, IDictionary<int, string> hostpciN = null, string hotplug = null, string hugepages = null, IDictionary<int, string> ideN = null, IDictionary<int, string> ipconfigN = null, string ivshmem = null, bool? keephugepages = null, string keyboard = null, bool? kvm = null, bool? live_restore = null, bool? localtime = null, string lock_ = null, string machine = null, string memory = null, float? migrate_downtime = null, int? migrate_speed = null, string name = null, string nameserver = null, IDictionary<int, string> netN = null, bool? numa = null, IDictionary<int, string> numaN = null, bool? onboot = null, string ostype = null, IDictionary<int, string> parallelN = null, string pool = null, bool? protection = null, bool? reboot = null, string rng0 = null, IDictionary<int, string> sataN = null, IDictionary<int, string> scsiN = null, string scsihw = null, string searchdomain = null, IDictionary<int, string> serialN = null, int? shares = null, string smbios1 = null, int? smp = null, int? sockets = null, string spice_enhancements = null, string sshkeys = null, bool? start = null, string startdate = null, string startup = null, string storage = null, bool? tablet = null, string tags = null, bool? tdf = null, bool? template = null, string tpmstate0 = null, bool? unique = null, IDictionary<int, string> unusedN = null, IDictionary<int, string> usbN = null, int? vcpus = null, string vga = null, IDictionary<int, string> virtioN = null, string vmgenid = null, string vmstatestorage = null, string watchdog = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("vmid", vmid);
                        parameters.Add("acpi", acpi);
                        parameters.Add("affinity", affinity);
                        parameters.Add("agent", agent);
                        parameters.Add("arch", arch);
                        parameters.Add("archive", archive);
                        parameters.Add("args", args);
                        parameters.Add("audio0", audio0);
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
                        parameters.Add("ciupgrade", ciupgrade);
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
                        parameters.Add("keephugepages", keephugepages);
                        parameters.Add("keyboard", keyboard);
                        parameters.Add("kvm", kvm);
                        parameters.Add("live-restore", live_restore);
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
                        parameters.Add("rng0", rng0);
                        parameters.Add("scsihw", scsihw);
                        parameters.Add("searchdomain", searchdomain);
                        parameters.Add("shares", shares);
                        parameters.Add("smbios1", smbios1);
                        parameters.Add("smp", smp);
                        parameters.Add("sockets", sockets);
                        parameters.Add("spice_enhancements", spice_enhancements);
                        parameters.Add("sshkeys", sshkeys);
                        parameters.Add("start", start);
                        parameters.Add("startdate", startdate);
                        parameters.Add("startup", startup);
                        parameters.Add("storage", storage);
                        parameters.Add("tablet", tablet);
                        parameters.Add("tags", tags);
                        parameters.Add("tdf", tdf);
                        parameters.Add("template", template);
                        parameters.Add("tpmstate0", tpmstate0);
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
                        return await _client.Create($"/nodes/{_node}/qemu", parameters);
                    }
                }
                /// <summary>
                /// Lxc
                /// </summary>
                public class PveLxc
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveLxc(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// VmidItem
                    /// </summary>
                    public PveVmidItem this[object vmid] => new(_client, _node, vmid);
                    /// <summary>
                    /// VmidItem
                    /// </summary>
                    public class PveVmidItem
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        private readonly object _vmid;
                        internal PveVmidItem(PveClient client, object node, object vmid)
                        {
                            _client = client; _node = node;
                            _vmid = vmid;
                        }
                        private PveConfig _config;
                        /// <summary>
                        /// Config
                        /// </summary>
                        public PveConfig Config => _config ??= new(_client, _node, _vmid);
                        private PveStatus _status;
                        /// <summary>
                        /// Status
                        /// </summary>
                        public PveStatus Status => _status ??= new(_client, _node, _vmid);
                        private PveSnapshot _snapshot;
                        /// <summary>
                        /// Snapshot
                        /// </summary>
                        public PveSnapshot Snapshot => _snapshot ??= new(_client, _node, _vmid);
                        private PveFirewall _firewall;
                        /// <summary>
                        /// Firewall
                        /// </summary>
                        public PveFirewall Firewall => _firewall ??= new(_client, _node, _vmid);
                        private PveRrd _rrd;
                        /// <summary>
                        /// Rrd
                        /// </summary>
                        public PveRrd Rrd => _rrd ??= new(_client, _node, _vmid);
                        private PveRrddata _rrddata;
                        /// <summary>
                        /// Rrddata
                        /// </summary>
                        public PveRrddata Rrddata => _rrddata ??= new(_client, _node, _vmid);
                        private PveVncproxy _vncproxy;
                        /// <summary>
                        /// Vncproxy
                        /// </summary>
                        public PveVncproxy Vncproxy => _vncproxy ??= new(_client, _node, _vmid);
                        private PveTermproxy _termproxy;
                        /// <summary>
                        /// Termproxy
                        /// </summary>
                        public PveTermproxy Termproxy => _termproxy ??= new(_client, _node, _vmid);
                        private PveVncwebsocket _vncwebsocket;
                        /// <summary>
                        /// Vncwebsocket
                        /// </summary>
                        public PveVncwebsocket Vncwebsocket => _vncwebsocket ??= new(_client, _node, _vmid);
                        private PveSpiceproxy _spiceproxy;
                        /// <summary>
                        /// Spiceproxy
                        /// </summary>
                        public PveSpiceproxy Spiceproxy => _spiceproxy ??= new(_client, _node, _vmid);
                        private PveRemoteMigrate _remoteMigrate;
                        /// <summary>
                        /// RemoteMigrate
                        /// </summary>
                        public PveRemoteMigrate RemoteMigrate => _remoteMigrate ??= new(_client, _node, _vmid);
                        private PveMigrate _migrate;
                        /// <summary>
                        /// Migrate
                        /// </summary>
                        public PveMigrate Migrate => _migrate ??= new(_client, _node, _vmid);
                        private PveFeature _feature;
                        /// <summary>
                        /// Feature
                        /// </summary>
                        public PveFeature Feature => _feature ??= new(_client, _node, _vmid);
                        private PveTemplate _template;
                        /// <summary>
                        /// Template
                        /// </summary>
                        public PveTemplate Template => _template ??= new(_client, _node, _vmid);
                        private PveClone _clone;
                        /// <summary>
                        /// Clone
                        /// </summary>
                        public PveClone Clone => _clone ??= new(_client, _node, _vmid);
                        private PveResize _resize;
                        /// <summary>
                        /// Resize
                        /// </summary>
                        public PveResize Resize => _resize ??= new(_client, _node, _vmid);
                        private PveMoveVolume _moveVolume;
                        /// <summary>
                        /// MoveVolume
                        /// </summary>
                        public PveMoveVolume MoveVolume => _moveVolume ??= new(_client, _node, _vmid);
                        private PvePending _pending;
                        /// <summary>
                        /// Pending
                        /// </summary>
                        public PvePending Pending => _pending ??= new(_client, _node, _vmid);
                        private PveInterfaces _interfaces;
                        /// <summary>
                        /// Interfaces
                        /// </summary>
                        public PveInterfaces Interfaces => _interfaces ??= new(_client, _node, _vmid);
                        private PveMtunnel _mtunnel;
                        /// <summary>
                        /// Mtunnel
                        /// </summary>
                        public PveMtunnel Mtunnel => _mtunnel ??= new(_client, _node, _vmid);
                        private PveMtunnelwebsocket _mtunnelwebsocket;
                        /// <summary>
                        /// Mtunnelwebsocket
                        /// </summary>
                        public PveMtunnelwebsocket Mtunnelwebsocket => _mtunnelwebsocket ??= new(_client, _node, _vmid);
                        /// <summary>
                        /// Config
                        /// </summary>
                        public class PveConfig
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveConfig(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get container configuration.
                            /// </summary>
                            /// <param name="current">Get current values (instead of pending values).</param>
                            /// <param name="snapshot">Fetch config values from given snapshot.</param>
                            /// <returns></returns>
                            public async Task<Result> VmConfig(bool? current = null, string snapshot = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("current", current);
                                parameters.Add("snapshot", snapshot);
                                return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/config", parameters);
                            }
                            /// <summary>
                            /// Set container options.
                            /// </summary>
                            /// <param name="arch">OS architecture type.
                            ///   Enum: amd64,i386,arm64,armhf,riscv32,riscv64</param>
                            /// <param name="cmode">Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode to 'shell', it simply invokes a shell inside the container (no login).
                            ///   Enum: shell,console,tty</param>
                            /// <param name="console">Attach a console device (/dev/console) to the container.</param>
                            /// <param name="cores">The number of cores assigned to the container. A container can use all available cores by default.</param>
                            /// <param name="cpulimit">Limit of CPU usage.  NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.</param>
                            /// <param name="cpuunits">CPU weight for a container, will be clamped to [1, 10000] in cgroup v2.</param>
                            /// <param name="debug">Try to be more verbose. For now this only enables debug log-level on start.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="description">Description for the Container. Shown in the web-interface CT's summary. This is saved as comment inside the configuration file.</param>
                            /// <param name="devN">Device to pass through to the container</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="features">Allow containers access to advanced features.</param>
                            /// <param name="hookscript">Script that will be exectued during various steps in the containers lifetime.</param>
                            /// <param name="hostname">Set a host name for the container.</param>
                            /// <param name="lock_">Lock/unlock the container.
                            ///   Enum: backup,create,destroyed,disk,fstrim,migrate,mounted,rollback,snapshot,snapshot-delete</param>
                            /// <param name="memory">Amount of RAM for the container in MB.</param>
                            /// <param name="mpN">Use volume as container mount point. Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume.</param>
                            /// <param name="nameserver">Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                            /// <param name="netN">Specifies network interfaces for the container.</param>
                            /// <param name="onboot">Specifies whether a container will be started during system bootup.</param>
                            /// <param name="ostype">OS type. This is used to setup configuration inside the container, and corresponds to lxc setup scripts in /usr/share/lxc/config/&amp;lt;ostype&amp;gt;.common.conf. Value 'unmanaged' can be used to skip and OS specific setup.
                            ///   Enum: debian,devuan,ubuntu,centos,fedora,opensuse,archlinux,alpine,gentoo,nixos,unmanaged</param>
                            /// <param name="protection">Sets the protection flag of the container. This will prevent the CT or CT's disk remove/update operation.</param>
                            /// <param name="revert">Revert a pending change.</param>
                            /// <param name="rootfs">Use volume as container root.</param>
                            /// <param name="searchdomain">Sets DNS search domains for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                            /// <param name="startup">Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, which specifies a delay to wait before the next VM is started or stopped.</param>
                            /// <param name="swap">Amount of SWAP for the container in MB.</param>
                            /// <param name="tags">Tags of the Container. This is only meta information.</param>
                            /// <param name="template">Enable/disable Template.</param>
                            /// <param name="timezone">Time zone to use in the container. If option isn't set, then nothing will be done. Can be set to 'host' to match the host time zone, or an arbitrary time zone option from /usr/share/zoneinfo/zone.tab</param>
                            /// <param name="tty">Specify the number of tty available to the container</param>
                            /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                            /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                            /// <returns></returns>
                            public async Task<Result> UpdateVm(string arch = null, string cmode = null, bool? console = null, int? cores = null, float? cpulimit = null, int? cpuunits = null, bool? debug = null, string delete = null, string description = null, IDictionary<int, string> devN = null, string digest = null, string features = null, string hookscript = null, string hostname = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, bool? protection = null, string revert = null, string rootfs = null, string searchdomain = null, string startup = null, int? swap = null, string tags = null, bool? template = null, string timezone = null, int? tty = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("arch", arch);
                                parameters.Add("cmode", cmode);
                                parameters.Add("console", console);
                                parameters.Add("cores", cores);
                                parameters.Add("cpulimit", cpulimit);
                                parameters.Add("cpuunits", cpuunits);
                                parameters.Add("debug", debug);
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
                                parameters.Add("revert", revert);
                                parameters.Add("rootfs", rootfs);
                                parameters.Add("searchdomain", searchdomain);
                                parameters.Add("startup", startup);
                                parameters.Add("swap", swap);
                                parameters.Add("tags", tags);
                                parameters.Add("template", template);
                                parameters.Add("timezone", timezone);
                                parameters.Add("tty", tty);
                                parameters.Add("unprivileged", unprivileged);
                                AddIndexedParameter(parameters, "dev", devN);
                                AddIndexedParameter(parameters, "mp", mpN);
                                AddIndexedParameter(parameters, "net", netN);
                                AddIndexedParameter(parameters, "unused", unusedN);
                                return await _client.Set($"/nodes/{_node}/lxc/{_vmid}/config", parameters);
                            }
                        }
                        /// <summary>
                        /// Status
                        /// </summary>
                        public class PveStatus
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveStatus(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PveCurrent _current;
                            /// <summary>
                            /// Current
                            /// </summary>
                            public PveCurrent Current => _current ??= new(_client, _node, _vmid);
                            private PveStart _start;
                            /// <summary>
                            /// Start
                            /// </summary>
                            public PveStart Start => _start ??= new(_client, _node, _vmid);
                            private PveStop _stop;
                            /// <summary>
                            /// Stop
                            /// </summary>
                            public PveStop Stop => _stop ??= new(_client, _node, _vmid);
                            private PveShutdown _shutdown;
                            /// <summary>
                            /// Shutdown
                            /// </summary>
                            public PveShutdown Shutdown => _shutdown ??= new(_client, _node, _vmid);
                            private PveSuspend _suspend;
                            /// <summary>
                            /// Suspend
                            /// </summary>
                            public PveSuspend Suspend => _suspend ??= new(_client, _node, _vmid);
                            private PveResume _resume;
                            /// <summary>
                            /// Resume
                            /// </summary>
                            public PveResume Resume => _resume ??= new(_client, _node, _vmid);
                            private PveReboot _reboot;
                            /// <summary>
                            /// Reboot
                            /// </summary>
                            public PveReboot Reboot => _reboot ??= new(_client, _node, _vmid);
                            /// <summary>
                            /// Current
                            /// </summary>
                            public class PveCurrent
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveCurrent(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get virtual machine status.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> VmStatus() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/status/current"); }
                            }
                            /// <summary>
                            /// Start
                            /// </summary>
                            public class PveStart
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveStart(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Start the container.
                                /// </summary>
                                /// <param name="debug">If set, enables very verbose debug log-level on start.</param>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public async Task<Result> VmStart(bool? debug = null, bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("debug", debug);
                                    parameters.Add("skiplock", skiplock);
                                    return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/start", parameters);
                                }
                            }
                            /// <summary>
                            /// Stop
                            /// </summary>
                            public class PveStop
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveStop(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Stop the container. This will abruptly stop all processes running in the container.
                                /// </summary>
                                /// <param name="skiplock">Ignore locks - only root is allowed to use this option.</param>
                                /// <returns></returns>
                                public async Task<Result> VmStop(bool? skiplock = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("skiplock", skiplock);
                                    return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/stop", parameters);
                                }
                            }
                            /// <summary>
                            /// Shutdown
                            /// </summary>
                            public class PveShutdown
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveShutdown(PveClient client, object node, object vmid)
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
                                public async Task<Result> VmShutdown(bool? forceStop = null, int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("forceStop", forceStop);
                                    parameters.Add("timeout", timeout);
                                    return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/shutdown", parameters);
                                }
                            }
                            /// <summary>
                            /// Suspend
                            /// </summary>
                            public class PveSuspend
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveSuspend(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Suspend the container. This is experimental.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> VmSuspend() { return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/suspend"); }
                            }
                            /// <summary>
                            /// Resume
                            /// </summary>
                            public class PveResume
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveResume(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Resume the container.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> VmResume() { return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/resume"); }
                            }
                            /// <summary>
                            /// Reboot
                            /// </summary>
                            public class PveReboot
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveReboot(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Reboot the container by shutting it down, and starting it again. Applies pending changes.
                                /// </summary>
                                /// <param name="timeout">Wait maximal timeout seconds for the shutdown.</param>
                                /// <returns></returns>
                                public async Task<Result> VmReboot(int? timeout = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("timeout", timeout);
                                    return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/status/reboot", parameters);
                                }
                            }
                            /// <summary>
                            /// Directory index
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Vmcmdidx() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/status"); }
                        }
                        /// <summary>
                        /// Snapshot
                        /// </summary>
                        public class PveSnapshot
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveSnapshot(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// SnapnameItem
                            /// </summary>
                            public PveSnapnameItem this[object snapname] => new(_client, _node, _vmid, snapname);
                            /// <summary>
                            /// SnapnameItem
                            /// </summary>
                            public class PveSnapnameItem
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                private readonly object _snapname;
                                internal PveSnapnameItem(PveClient client, object node, object vmid, object snapname)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                    _snapname = snapname;
                                }
                                private PveRollback _rollback;
                                /// <summary>
                                /// Rollback
                                /// </summary>
                                public PveRollback Rollback => _rollback ??= new(_client, _node, _vmid, _snapname);
                                private PveConfig _config;
                                /// <summary>
                                /// Config
                                /// </summary>
                                public PveConfig Config => _config ??= new(_client, _node, _vmid, _snapname);
                                /// <summary>
                                /// Rollback
                                /// </summary>
                                public class PveRollback
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _snapname;
                                    internal PveRollback(PveClient client, object node, object vmid, object snapname)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }
                                    /// <summary>
                                    /// Rollback LXC state to specified snapshot.
                                    /// </summary>
                                    /// <param name="start">Whether the container should get started after rolling back successfully</param>
                                    /// <returns></returns>
                                    public async Task<Result> Rollback(bool? start = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("start", start);
                                        return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/rollback", parameters);
                                    }
                                }
                                /// <summary>
                                /// Config
                                /// </summary>
                                public class PveConfig
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _snapname;
                                    internal PveConfig(PveClient client, object node, object vmid, object snapname)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _snapname = snapname;
                                    }
                                    /// <summary>
                                    /// Get snapshot configuration
                                    /// </summary>
                                    /// <returns></returns>
                                    public async Task<Result> GetSnapshotConfig() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/config"); }
                                    /// <summary>
                                    /// Update snapshot metadata.
                                    /// </summary>
                                    /// <param name="description">A textual description or comment.</param>
                                    /// <returns></returns>
                                    public async Task<Result> UpdateSnapshotConfig(string description = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("description", description);
                                        return await _client.Set($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}/config", parameters);
                                    }
                                }
                                /// <summary>
                                /// Delete a LXC snapshot.
                                /// </summary>
                                /// <param name="force">For removal from config file, even if removing disk snapshots fails.</param>
                                /// <returns></returns>
                                public async Task<Result> Delsnapshot(bool? force = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("force", force);
                                    return await _client.Delete($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}", parameters);
                                }
                                /// <summary>
                                ///
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> SnapshotCmdIdx() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot/{_snapname}"); }
                            }
                            /// <summary>
                            /// List all snapshots.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> List() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/snapshot"); }
                            /// <summary>
                            /// Snapshot a container.
                            /// </summary>
                            /// <param name="snapname">The name of the snapshot.</param>
                            /// <param name="description">A textual description or comment.</param>
                            /// <returns></returns>
                            public async Task<Result> Snapshot(string snapname, string description = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("snapname", snapname);
                                parameters.Add("description", description);
                                return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/snapshot", parameters);
                            }
                        }
                        /// <summary>
                        /// Firewall
                        /// </summary>
                        public class PveFirewall
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveFirewall(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            private PveRules _rules;
                            /// <summary>
                            /// Rules
                            /// </summary>
                            public PveRules Rules => _rules ??= new(_client, _node, _vmid);
                            private PveAliases _aliases;
                            /// <summary>
                            /// Aliases
                            /// </summary>
                            public PveAliases Aliases => _aliases ??= new(_client, _node, _vmid);
                            private PveIpset _ipset;
                            /// <summary>
                            /// Ipset
                            /// </summary>
                            public PveIpset Ipset => _ipset ??= new(_client, _node, _vmid);
                            private PveOptions _options;
                            /// <summary>
                            /// Options
                            /// </summary>
                            public PveOptions Options => _options ??= new(_client, _node, _vmid);
                            private PveLog _log;
                            /// <summary>
                            /// Log
                            /// </summary>
                            public PveLog Log => _log ??= new(_client, _node, _vmid);
                            private PveRefs _refs;
                            /// <summary>
                            /// Refs
                            /// </summary>
                            public PveRefs Refs => _refs ??= new(_client, _node, _vmid);
                            /// <summary>
                            /// Rules
                            /// </summary>
                            public class PveRules
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveRules(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// PosItem
                                /// </summary>
                                public PvePosItem this[object pos] => new(_client, _node, _vmid, pos);
                                /// <summary>
                                /// PosItem
                                /// </summary>
                                public class PvePosItem
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _pos;
                                    internal PvePosItem(PveClient client, object node, object vmid, object pos)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _pos = pos;
                                    }
                                    /// <summary>
                                    /// Delete rule.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public async Task<Result> DeleteRule(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        return await _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}", parameters);
                                    }
                                    /// <summary>
                                    /// Get single rule data.
                                    /// </summary>
                                    /// <returns></returns>
                                    public async Task<Result> GetRule() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}"); }
                                    /// <summary>
                                    /// Modify rule data.
                                    /// </summary>
                                    /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                    /// <param name="comment">Descriptive comment.</param>
                                    /// <param name="delete">A list of settings you want to delete.</param>
                                    /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                    /// <param name="enable">Flag to enable/disable a rule.</param>
                                    /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
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
                                    public async Task<Result> UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("action", action);
                                        parameters.Add("comment", comment);
                                        parameters.Add("delete", delete);
                                        parameters.Add("dest", dest);
                                        parameters.Add("digest", digest);
                                        parameters.Add("dport", dport);
                                        parameters.Add("enable", enable);
                                        parameters.Add("icmp-type", icmp_type);
                                        parameters.Add("iface", iface);
                                        parameters.Add("log", log);
                                        parameters.Add("macro", macro);
                                        parameters.Add("moveto", moveto);
                                        parameters.Add("proto", proto);
                                        parameters.Add("source", source);
                                        parameters.Add("sport", sport);
                                        parameters.Add("type", type);
                                        return await _client.Set($"/nodes/{_node}/lxc/{_vmid}/firewall/rules/{_pos}", parameters);
                                    }
                                }
                                /// <summary>
                                /// List rules.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetRules() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/rules"); }
                                /// <summary>
                                /// Create new rule.
                                /// </summary>
                                /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                                /// <param name="type">Rule type.
                                ///   Enum: in,out,group</param>
                                /// <param name="comment">Descriptive comment.</param>
                                /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <param name="enable">Flag to enable/disable a rule.</param>
                                /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
                                /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                                /// <param name="log">Log level for firewall rule.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macro">Use predefined standard macro.</param>
                                /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                                /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                                /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                                /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                                /// <returns></returns>
                                public async Task<Result> CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("action", action);
                                    parameters.Add("type", type);
                                    parameters.Add("comment", comment);
                                    parameters.Add("dest", dest);
                                    parameters.Add("digest", digest);
                                    parameters.Add("dport", dport);
                                    parameters.Add("enable", enable);
                                    parameters.Add("icmp-type", icmp_type);
                                    parameters.Add("iface", iface);
                                    parameters.Add("log", log);
                                    parameters.Add("macro", macro);
                                    parameters.Add("pos", pos);
                                    parameters.Add("proto", proto);
                                    parameters.Add("source", source);
                                    parameters.Add("sport", sport);
                                    return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/firewall/rules", parameters);
                                }
                            }
                            /// <summary>
                            /// Aliases
                            /// </summary>
                            public class PveAliases
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveAliases(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// NameItem
                                /// </summary>
                                public PveNameItem this[object name] => new(_client, _node, _vmid, name);
                                /// <summary>
                                /// NameItem
                                /// </summary>
                                public class PveNameItem
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _name;
                                    internal PveNameItem(PveClient client, object node, object vmid, object name)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }
                                    /// <summary>
                                    /// Remove IP or Network alias.
                                    /// </summary>
                                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                    /// <returns></returns>
                                    public async Task<Result> RemoveAlias(string digest = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("digest", digest);
                                        return await _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}", parameters);
                                    }
                                    /// <summary>
                                    /// Read alias.
                                    /// </summary>
                                    /// <returns></returns>
                                    public async Task<Result> ReadAlias() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}"); }
                                    /// <summary>
                                    /// Update IP or Network alias.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                    /// <param name="rename">Rename an existing alias.</param>
                                    /// <returns></returns>
                                    public async Task<Result> UpdateAlias(string cidr, string comment = null, string digest = null, string rename = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("digest", digest);
                                        parameters.Add("rename", rename);
                                        return await _client.Set($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases/{_name}", parameters);
                                    }
                                }
                                /// <summary>
                                /// List aliases
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetAliases() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases"); }
                                /// <summary>
                                /// Create IP or Network Alias.
                                /// </summary>
                                /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                /// <param name="name">Alias name.</param>
                                /// <param name="comment"></param>
                                /// <returns></returns>
                                public async Task<Result> CreateAlias(string cidr, string name, string comment = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("cidr", cidr);
                                    parameters.Add("name", name);
                                    parameters.Add("comment", comment);
                                    return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/firewall/aliases", parameters);
                                }
                            }
                            /// <summary>
                            /// Ipset
                            /// </summary>
                            public class PveIpset
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveIpset(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// NameItem
                                /// </summary>
                                public PveNameItem this[object name] => new(_client, _node, _vmid, name);
                                /// <summary>
                                /// NameItem
                                /// </summary>
                                public class PveNameItem
                                {
                                    private readonly PveClient _client;
                                    private readonly object _node;
                                    private readonly object _vmid;
                                    private readonly object _name;
                                    internal PveNameItem(PveClient client, object node, object vmid, object name)
                                    {
                                        _client = client; _node = node;
                                        _vmid = vmid;
                                        _name = name;
                                    }
                                    /// <summary>
                                    /// CidrItem
                                    /// </summary>
                                    public PveCidrItem this[object cidr] => new(_client, _node, _vmid, _name, cidr);
                                    /// <summary>
                                    /// CidrItem
                                    /// </summary>
                                    public class PveCidrItem
                                    {
                                        private readonly PveClient _client;
                                        private readonly object _node;
                                        private readonly object _vmid;
                                        private readonly object _name;
                                        private readonly object _cidr;
                                        internal PveCidrItem(PveClient client, object node, object vmid, object name, object cidr)
                                        {
                                            _client = client; _node = node;
                                            _vmid = vmid;
                                            _name = name;
                                            _cidr = cidr;
                                        }
                                        /// <summary>
                                        /// Remove IP or Network from IPSet.
                                        /// </summary>
                                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                        /// <returns></returns>
                                        public async Task<Result> RemoveIp(string digest = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("digest", digest);
                                            return await _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}", parameters);
                                        }
                                        /// <summary>
                                        /// Read IP or Network settings from IPSet.
                                        /// </summary>
                                        /// <returns></returns>
                                        public async Task<Result> ReadIp() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}"); }
                                        /// <summary>
                                        /// Update IP or Network settings
                                        /// </summary>
                                        /// <param name="comment"></param>
                                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                        /// <param name="nomatch"></param>
                                        /// <returns></returns>
                                        public async Task<Result> UpdateIp(string comment = null, string digest = null, bool? nomatch = null)
                                        {
                                            var parameters = new Dictionary<string, object>();
                                            parameters.Add("comment", comment);
                                            parameters.Add("digest", digest);
                                            parameters.Add("nomatch", nomatch);
                                            return await _client.Set($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}/{_cidr}", parameters);
                                        }
                                    }
                                    /// <summary>
                                    /// Delete IPSet
                                    /// </summary>
                                    /// <param name="force">Delete all members of the IPSet, if there are any.</param>
                                    /// <returns></returns>
                                    public async Task<Result> DeleteIpset(bool? force = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("force", force);
                                        return await _client.Delete($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}", parameters);
                                    }
                                    /// <summary>
                                    /// List IPSet content
                                    /// </summary>
                                    /// <returns></returns>
                                    public async Task<Result> GetIpset() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}"); }
                                    /// <summary>
                                    /// Add IP or Network to IPSet.
                                    /// </summary>
                                    /// <param name="cidr">Network/IP specification in CIDR format.</param>
                                    /// <param name="comment"></param>
                                    /// <param name="nomatch"></param>
                                    /// <returns></returns>
                                    public async Task<Result> CreateIp(string cidr, string comment = null, bool? nomatch = null)
                                    {
                                        var parameters = new Dictionary<string, object>();
                                        parameters.Add("cidr", cidr);
                                        parameters.Add("comment", comment);
                                        parameters.Add("nomatch", nomatch);
                                        return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset/{_name}", parameters);
                                    }
                                }
                                /// <summary>
                                /// List IPSets
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> IpsetIndex() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset"); }
                                /// <summary>
                                /// Create new IPSet
                                /// </summary>
                                /// <param name="name">IP set name.</param>
                                /// <param name="comment"></param>
                                /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="rename">Rename an existing IPSet. You can set 'rename' to the same value as 'name' to update the 'comment' of an existing IPSet.</param>
                                /// <returns></returns>
                                public async Task<Result> CreateIpset(string name, string comment = null, string digest = null, string rename = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("name", name);
                                    parameters.Add("comment", comment);
                                    parameters.Add("digest", digest);
                                    parameters.Add("rename", rename);
                                    return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/firewall/ipset", parameters);
                                }
                            }
                            /// <summary>
                            /// Options
                            /// </summary>
                            public class PveOptions
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveOptions(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Get VM firewall options.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> GetOptions() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/options"); }
                                /// <summary>
                                /// Set Firewall options.
                                /// </summary>
                                /// <param name="delete">A list of settings you want to delete.</param>
                                /// <param name="dhcp">Enable DHCP.</param>
                                /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                                /// <param name="enable">Enable/disable firewall rules.</param>
                                /// <param name="ipfilter">Enable default IP filters. This is equivalent to adding an empty ipfilter-net&amp;lt;id&amp;gt; ipset for every interface. Such ipsets implicitly contain sane default restrictions such as restricting IPv6 link local addresses to the one derived from the interface's MAC address. For containers the configured IP addresses will be implicitly added.</param>
                                /// <param name="log_level_in">Log level for incoming traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="log_level_out">Log level for outgoing traffic.
                                ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                                /// <param name="macfilter">Enable/disable MAC address filter.</param>
                                /// <param name="ndp">Enable NDP (Neighbor Discovery Protocol).</param>
                                /// <param name="policy_in">Input policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="policy_out">Output policy.
                                ///   Enum: ACCEPT,REJECT,DROP</param>
                                /// <param name="radv">Allow sending Router Advertisement.</param>
                                /// <returns></returns>
                                public async Task<Result> SetOptions(string delete = null, bool? dhcp = null, string digest = null, bool? enable = null, bool? ipfilter = null, string log_level_in = null, string log_level_out = null, bool? macfilter = null, bool? ndp = null, string policy_in = null, string policy_out = null, bool? radv = null)
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
                                    return await _client.Set($"/nodes/{_node}/lxc/{_vmid}/firewall/options", parameters);
                                }
                            }
                            /// <summary>
                            /// Log
                            /// </summary>
                            public class PveLog
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveLog(PveClient client, object node, object vmid)
                                {
                                    _client = client; _node = node;
                                    _vmid = vmid;
                                }
                                /// <summary>
                                /// Read firewall log
                                /// </summary>
                                /// <param name="limit"></param>
                                /// <param name="since">Display log since this UNIX epoch.</param>
                                /// <param name="start"></param>
                                /// <param name="until">Display log until this UNIX epoch.</param>
                                /// <returns></returns>
                                public async Task<Result> Log(int? limit = null, int? since = null, int? start = null, int? until = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("limit", limit);
                                    parameters.Add("since", since);
                                    parameters.Add("start", start);
                                    parameters.Add("until", until);
                                    return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/log", parameters);
                                }
                            }
                            /// <summary>
                            /// Refs
                            /// </summary>
                            public class PveRefs
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _vmid;
                                internal PveRefs(PveClient client, object node, object vmid)
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
                                public async Task<Result> Refs(string type = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall/refs", parameters);
                                }
                            }
                            /// <summary>
                            /// Directory index.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/firewall"); }
                        }
                        /// <summary>
                        /// Rrd
                        /// </summary>
                        public class PveRrd
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveRrd(PveClient client, object node, object vmid)
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
                            public async Task<Result> Rrd(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/rrd", parameters);
                            }
                        }
                        /// <summary>
                        /// Rrddata
                        /// </summary>
                        public class PveRrddata
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveRrddata(PveClient client, object node, object vmid)
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
                            public async Task<Result> Rrddata(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/rrddata", parameters);
                            }
                        }
                        /// <summary>
                        /// Vncproxy
                        /// </summary>
                        public class PveVncproxy
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveVncproxy(PveClient client, object node, object vmid)
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
                            public async Task<Result> Vncproxy(int? height = null, bool? websocket = null, int? width = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("height", height);
                                parameters.Add("websocket", websocket);
                                parameters.Add("width", width);
                                return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/vncproxy", parameters);
                            }
                        }
                        /// <summary>
                        /// Termproxy
                        /// </summary>
                        public class PveTermproxy
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveTermproxy(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Creates a TCP proxy connection.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Termproxy() { return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/termproxy"); }
                        }
                        /// <summary>
                        /// Vncwebsocket
                        /// </summary>
                        public class PveVncwebsocket
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveVncwebsocket(PveClient client, object node, object vmid)
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
                            public async Task<Result> Vncwebsocket(int port, string vncticket)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("port", port);
                                parameters.Add("vncticket", vncticket);
                                return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/vncwebsocket", parameters);
                            }
                        }
                        /// <summary>
                        /// Spiceproxy
                        /// </summary>
                        public class PveSpiceproxy
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveSpiceproxy(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Returns a SPICE configuration to connect to the CT.
                            /// </summary>
                            /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                            /// <returns></returns>
                            public async Task<Result> Spiceproxy(string proxy = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("proxy", proxy);
                                return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/spiceproxy", parameters);
                            }
                        }
                        /// <summary>
                        /// RemoteMigrate
                        /// </summary>
                        public class PveRemoteMigrate
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveRemoteMigrate(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Migrate the container to another cluster. Creates a new migration task. EXPERIMENTAL feature!
                            /// </summary>
                            /// <param name="target_bridge">Mapping from source to target bridges. Providing only a single bridge ID maps all source bridges to that bridge. Providing the special value '1' will map each source bridge to itself.</param>
                            /// <param name="target_endpoint">Remote target endpoint</param>
                            /// <param name="target_storage">Mapping from source to target storages. Providing only a single storage ID maps all source storages to that storage. Providing the special value '1' will map each source storage to itself.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="delete">Delete the original CT and related data after successful migration. By default the original CT is kept on the source cluster in a stopped state.</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="restart">Use restart migration</param>
                            /// <param name="target_vmid">The (unique) ID of the VM.</param>
                            /// <param name="timeout">Timeout in seconds for shutdown for restart migration</param>
                            /// <returns></returns>
                            public async Task<Result> RemoteMigrateVm(string target_bridge, string target_endpoint, string target_storage, float? bwlimit = null, bool? delete = null, bool? online = null, bool? restart = null, int? target_vmid = null, int? timeout = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target-bridge", target_bridge);
                                parameters.Add("target-endpoint", target_endpoint);
                                parameters.Add("target-storage", target_storage);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("delete", delete);
                                parameters.Add("online", online);
                                parameters.Add("restart", restart);
                                parameters.Add("target-vmid", target_vmid);
                                parameters.Add("timeout", timeout);
                                return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/remote_migrate", parameters);
                            }
                        }
                        /// <summary>
                        /// Migrate
                        /// </summary>
                        public class PveMigrate
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMigrate(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Migrate the container to another node. Creates a new migration task.
                            /// </summary>
                            /// <param name="target">Target node.</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="online">Use online/live migration.</param>
                            /// <param name="restart">Use restart migration</param>
                            /// <param name="target_storage">Mapping from source to target storages. Providing only a single storage ID maps all source storages to that storage. Providing the special value '1' will map each source storage to itself.</param>
                            /// <param name="timeout">Timeout in seconds for shutdown for restart migration</param>
                            /// <returns></returns>
                            public async Task<Result> MigrateVm(string target, float? bwlimit = null, bool? online = null, bool? restart = null, string target_storage = null, int? timeout = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("target", target);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("online", online);
                                parameters.Add("restart", restart);
                                parameters.Add("target-storage", target_storage);
                                parameters.Add("timeout", timeout);
                                return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/migrate", parameters);
                            }
                        }
                        /// <summary>
                        /// Feature
                        /// </summary>
                        public class PveFeature
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveFeature(PveClient client, object node, object vmid)
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
                            public async Task<Result> VmFeature(string feature, string snapname = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("feature", feature);
                                parameters.Add("snapname", snapname);
                                return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/feature", parameters);
                            }
                        }
                        /// <summary>
                        /// Template
                        /// </summary>
                        public class PveTemplate
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveTemplate(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Create a Template.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Template() { return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/template"); }
                        }
                        /// <summary>
                        /// Clone
                        /// </summary>
                        public class PveClone
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveClone(PveClient client, object node, object vmid)
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
                            public async Task<Result> CloneVm(int newid, float? bwlimit = null, string description = null, bool? full = null, string hostname = null, string pool = null, string snapname = null, string storage = null, string target = null)
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
                                return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/clone", parameters);
                            }
                        }
                        /// <summary>
                        /// Resize
                        /// </summary>
                        public class PveResize
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveResize(PveClient client, object node, object vmid)
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
                            public async Task<Result> ResizeVm(string disk, string size, string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("disk", disk);
                                parameters.Add("size", size);
                                parameters.Add("digest", digest);
                                return await _client.Set($"/nodes/{_node}/lxc/{_vmid}/resize", parameters);
                            }
                        }
                        /// <summary>
                        /// MoveVolume
                        /// </summary>
                        public class PveMoveVolume
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMoveVolume(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Move a rootfs-/mp-volume to a different storage or to a different container.
                            /// </summary>
                            /// <param name="volume">Volume which will be moved.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9,mp10,mp11,mp12,mp13,mp14,mp15,mp16,mp17,mp18,mp19,mp20,mp21,mp22,mp23,mp24,mp25,mp26,mp27,mp28,mp29,mp30,mp31,mp32,mp33,mp34,mp35,mp36,mp37,mp38,mp39,mp40,mp41,mp42,mp43,mp44,mp45,mp46,mp47,mp48,mp49,mp50,mp51,mp52,mp53,mp54,mp55,mp56,mp57,mp58,mp59,mp60,mp61,mp62,mp63,mp64,mp65,mp66,mp67,mp68,mp69,mp70,mp71,mp72,mp73,mp74,mp75,mp76,mp77,mp78,mp79,mp80,mp81,mp82,mp83,mp84,mp85,mp86,mp87,mp88,mp89,mp90,mp91,mp92,mp93,mp94,mp95,mp96,mp97,mp98,mp99,mp100,mp101,mp102,mp103,mp104,mp105,mp106,mp107,mp108,mp109,mp110,mp111,mp112,mp113,mp114,mp115,mp116,mp117,mp118,mp119,mp120,mp121,mp122,mp123,mp124,mp125,mp126,mp127,mp128,mp129,mp130,mp131,mp132,mp133,mp134,mp135,mp136,mp137,mp138,mp139,mp140,mp141,mp142,mp143,mp144,mp145,mp146,mp147,mp148,mp149,mp150,mp151,mp152,mp153,mp154,mp155,mp156,mp157,mp158,mp159,mp160,mp161,mp162,mp163,mp164,mp165,mp166,mp167,mp168,mp169,mp170,mp171,mp172,mp173,mp174,mp175,mp176,mp177,mp178,mp179,mp180,mp181,mp182,mp183,mp184,mp185,mp186,mp187,mp188,mp189,mp190,mp191,mp192,mp193,mp194,mp195,mp196,mp197,mp198,mp199,mp200,mp201,mp202,mp203,mp204,mp205,mp206,mp207,mp208,mp209,mp210,mp211,mp212,mp213,mp214,mp215,mp216,mp217,mp218,mp219,mp220,mp221,mp222,mp223,mp224,mp225,mp226,mp227,mp228,mp229,mp230,mp231,mp232,mp233,mp234,mp235,mp236,mp237,mp238,mp239,mp240,mp241,mp242,mp243,mp244,mp245,mp246,mp247,mp248,mp249,mp250,mp251,mp252,mp253,mp254,mp255,unused0,unused1,unused2,unused3,unused4,unused5,unused6,unused7,unused8,unused9,unused10,unused11,unused12,unused13,unused14,unused15,unused16,unused17,unused18,unused19,unused20,unused21,unused22,unused23,unused24,unused25,unused26,unused27,unused28,unused29,unused30,unused31,unused32,unused33,unused34,unused35,unused36,unused37,unused38,unused39,unused40,unused41,unused42,unused43,unused44,unused45,unused46,unused47,unused48,unused49,unused50,unused51,unused52,unused53,unused54,unused55,unused56,unused57,unused58,unused59,unused60,unused61,unused62,unused63,unused64,unused65,unused66,unused67,unused68,unused69,unused70,unused71,unused72,unused73,unused74,unused75,unused76,unused77,unused78,unused79,unused80,unused81,unused82,unused83,unused84,unused85,unused86,unused87,unused88,unused89,unused90,unused91,unused92,unused93,unused94,unused95,unused96,unused97,unused98,unused99,unused100,unused101,unused102,unused103,unused104,unused105,unused106,unused107,unused108,unused109,unused110,unused111,unused112,unused113,unused114,unused115,unused116,unused117,unused118,unused119,unused120,unused121,unused122,unused123,unused124,unused125,unused126,unused127,unused128,unused129,unused130,unused131,unused132,unused133,unused134,unused135,unused136,unused137,unused138,unused139,unused140,unused141,unused142,unused143,unused144,unused145,unused146,unused147,unused148,unused149,unused150,unused151,unused152,unused153,unused154,unused155,unused156,unused157,unused158,unused159,unused160,unused161,unused162,unused163,unused164,unused165,unused166,unused167,unused168,unused169,unused170,unused171,unused172,unused173,unused174,unused175,unused176,unused177,unused178,unused179,unused180,unused181,unused182,unused183,unused184,unused185,unused186,unused187,unused188,unused189,unused190,unused191,unused192,unused193,unused194,unused195,unused196,unused197,unused198,unused199,unused200,unused201,unused202,unused203,unused204,unused205,unused206,unused207,unused208,unused209,unused210,unused211,unused212,unused213,unused214,unused215,unused216,unused217,unused218,unused219,unused220,unused221,unused222,unused223,unused224,unused225,unused226,unused227,unused228,unused229,unused230,unused231,unused232,unused233,unused234,unused235,unused236,unused237,unused238,unused239,unused240,unused241,unused242,unused243,unused244,unused245,unused246,unused247,unused248,unused249,unused250,unused251,unused252,unused253,unused254,unused255</param>
                            /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                            /// <param name="delete">Delete the original volume after successful copy. By default the original is kept as an unused volume entry.</param>
                            /// <param name="digest">Prevent changes if current configuration file has different SHA1 " . 		    "digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="storage">Target Storage.</param>
                            /// <param name="target_digest">Prevent changes if current configuration file of the target " . 		    "container has a different SHA1 digest. This can be used to prevent " . 		    "concurrent modifications.</param>
                            /// <param name="target_vmid">The (unique) ID of the VM.</param>
                            /// <param name="target_volume">The config key the volume will be moved to. Default is the source volume key.
                            ///   Enum: rootfs,mp0,mp1,mp2,mp3,mp4,mp5,mp6,mp7,mp8,mp9,mp10,mp11,mp12,mp13,mp14,mp15,mp16,mp17,mp18,mp19,mp20,mp21,mp22,mp23,mp24,mp25,mp26,mp27,mp28,mp29,mp30,mp31,mp32,mp33,mp34,mp35,mp36,mp37,mp38,mp39,mp40,mp41,mp42,mp43,mp44,mp45,mp46,mp47,mp48,mp49,mp50,mp51,mp52,mp53,mp54,mp55,mp56,mp57,mp58,mp59,mp60,mp61,mp62,mp63,mp64,mp65,mp66,mp67,mp68,mp69,mp70,mp71,mp72,mp73,mp74,mp75,mp76,mp77,mp78,mp79,mp80,mp81,mp82,mp83,mp84,mp85,mp86,mp87,mp88,mp89,mp90,mp91,mp92,mp93,mp94,mp95,mp96,mp97,mp98,mp99,mp100,mp101,mp102,mp103,mp104,mp105,mp106,mp107,mp108,mp109,mp110,mp111,mp112,mp113,mp114,mp115,mp116,mp117,mp118,mp119,mp120,mp121,mp122,mp123,mp124,mp125,mp126,mp127,mp128,mp129,mp130,mp131,mp132,mp133,mp134,mp135,mp136,mp137,mp138,mp139,mp140,mp141,mp142,mp143,mp144,mp145,mp146,mp147,mp148,mp149,mp150,mp151,mp152,mp153,mp154,mp155,mp156,mp157,mp158,mp159,mp160,mp161,mp162,mp163,mp164,mp165,mp166,mp167,mp168,mp169,mp170,mp171,mp172,mp173,mp174,mp175,mp176,mp177,mp178,mp179,mp180,mp181,mp182,mp183,mp184,mp185,mp186,mp187,mp188,mp189,mp190,mp191,mp192,mp193,mp194,mp195,mp196,mp197,mp198,mp199,mp200,mp201,mp202,mp203,mp204,mp205,mp206,mp207,mp208,mp209,mp210,mp211,mp212,mp213,mp214,mp215,mp216,mp217,mp218,mp219,mp220,mp221,mp222,mp223,mp224,mp225,mp226,mp227,mp228,mp229,mp230,mp231,mp232,mp233,mp234,mp235,mp236,mp237,mp238,mp239,mp240,mp241,mp242,mp243,mp244,mp245,mp246,mp247,mp248,mp249,mp250,mp251,mp252,mp253,mp254,mp255,unused0,unused1,unused2,unused3,unused4,unused5,unused6,unused7,unused8,unused9,unused10,unused11,unused12,unused13,unused14,unused15,unused16,unused17,unused18,unused19,unused20,unused21,unused22,unused23,unused24,unused25,unused26,unused27,unused28,unused29,unused30,unused31,unused32,unused33,unused34,unused35,unused36,unused37,unused38,unused39,unused40,unused41,unused42,unused43,unused44,unused45,unused46,unused47,unused48,unused49,unused50,unused51,unused52,unused53,unused54,unused55,unused56,unused57,unused58,unused59,unused60,unused61,unused62,unused63,unused64,unused65,unused66,unused67,unused68,unused69,unused70,unused71,unused72,unused73,unused74,unused75,unused76,unused77,unused78,unused79,unused80,unused81,unused82,unused83,unused84,unused85,unused86,unused87,unused88,unused89,unused90,unused91,unused92,unused93,unused94,unused95,unused96,unused97,unused98,unused99,unused100,unused101,unused102,unused103,unused104,unused105,unused106,unused107,unused108,unused109,unused110,unused111,unused112,unused113,unused114,unused115,unused116,unused117,unused118,unused119,unused120,unused121,unused122,unused123,unused124,unused125,unused126,unused127,unused128,unused129,unused130,unused131,unused132,unused133,unused134,unused135,unused136,unused137,unused138,unused139,unused140,unused141,unused142,unused143,unused144,unused145,unused146,unused147,unused148,unused149,unused150,unused151,unused152,unused153,unused154,unused155,unused156,unused157,unused158,unused159,unused160,unused161,unused162,unused163,unused164,unused165,unused166,unused167,unused168,unused169,unused170,unused171,unused172,unused173,unused174,unused175,unused176,unused177,unused178,unused179,unused180,unused181,unused182,unused183,unused184,unused185,unused186,unused187,unused188,unused189,unused190,unused191,unused192,unused193,unused194,unused195,unused196,unused197,unused198,unused199,unused200,unused201,unused202,unused203,unused204,unused205,unused206,unused207,unused208,unused209,unused210,unused211,unused212,unused213,unused214,unused215,unused216,unused217,unused218,unused219,unused220,unused221,unused222,unused223,unused224,unused225,unused226,unused227,unused228,unused229,unused230,unused231,unused232,unused233,unused234,unused235,unused236,unused237,unused238,unused239,unused240,unused241,unused242,unused243,unused244,unused245,unused246,unused247,unused248,unused249,unused250,unused251,unused252,unused253,unused254,unused255</param>
                            /// <returns></returns>
                            public async Task<Result> MoveVolume(string volume, float? bwlimit = null, bool? delete = null, string digest = null, string storage = null, string target_digest = null, int? target_vmid = null, string target_volume = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("volume", volume);
                                parameters.Add("bwlimit", bwlimit);
                                parameters.Add("delete", delete);
                                parameters.Add("digest", digest);
                                parameters.Add("storage", storage);
                                parameters.Add("target-digest", target_digest);
                                parameters.Add("target-vmid", target_vmid);
                                parameters.Add("target-volume", target_volume);
                                return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/move_volume", parameters);
                            }
                        }
                        /// <summary>
                        /// Pending
                        /// </summary>
                        public class PvePending
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PvePending(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get container configuration, including pending changes.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> VmPending() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/pending"); }
                        }
                        /// <summary>
                        /// Interfaces
                        /// </summary>
                        public class PveInterfaces
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveInterfaces(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Get IP addresses of the specified container interface.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Ip() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/interfaces"); }
                        }
                        /// <summary>
                        /// Mtunnel
                        /// </summary>
                        public class PveMtunnel
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMtunnel(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Migration tunnel endpoint - only for internal use by CT migration.
                            /// </summary>
                            /// <param name="bridges">List of network bridges to check availability. Will be checked again for actually used bridges during migration.</param>
                            /// <param name="storages">List of storages to check permission and availability. Will be checked again for all actually used storages during migration.</param>
                            /// <returns></returns>
                            public async Task<Result> Mtunnel(string bridges = null, string storages = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("bridges", bridges);
                                parameters.Add("storages", storages);
                                return await _client.Create($"/nodes/{_node}/lxc/{_vmid}/mtunnel", parameters);
                            }
                        }
                        /// <summary>
                        /// Mtunnelwebsocket
                        /// </summary>
                        public class PveMtunnelwebsocket
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _vmid;
                            internal PveMtunnelwebsocket(PveClient client, object node, object vmid)
                            {
                                _client = client; _node = node;
                                _vmid = vmid;
                            }
                            /// <summary>
                            /// Migration tunnel endpoint for websocket upgrade - only for internal use by VM migration.
                            /// </summary>
                            /// <param name="socket">unix socket to forward to</param>
                            /// <param name="ticket">ticket return by initial 'mtunnel' API call, or retrieved via 'ticket' tunnel command</param>
                            /// <returns></returns>
                            public async Task<Result> Mtunnelwebsocket(string socket, string ticket)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("socket", socket);
                                parameters.Add("ticket", ticket);
                                return await _client.Get($"/nodes/{_node}/lxc/{_vmid}/mtunnelwebsocket", parameters);
                            }
                        }
                        /// <summary>
                        /// Destroy the container (also delete all uses files).
                        /// </summary>
                        /// <param name="destroy_unreferenced_disks">If set, destroy additionally all disks with the VMID from all enabled storages which are not referenced in the config.</param>
                        /// <param name="force">Force destroy, even if running.</param>
                        /// <param name="purge">Remove container from all related configurations. For example, backup jobs, replication jobs or HA. Related ACLs and Firewall entries will *always* be removed.</param>
                        /// <returns></returns>
                        public async Task<Result> DestroyVm(bool? destroy_unreferenced_disks = null, bool? force = null, bool? purge = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("destroy-unreferenced-disks", destroy_unreferenced_disks);
                            parameters.Add("force", force);
                            parameters.Add("purge", purge);
                            return await _client.Delete($"/nodes/{_node}/lxc/{_vmid}", parameters);
                        }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Vmdiridx() { return await _client.Get($"/nodes/{_node}/lxc/{_vmid}"); }
                    }
                    /// <summary>
                    /// LXC container index (per node).
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Vmlist() { return await _client.Get($"/nodes/{_node}/lxc"); }
                    /// <summary>
                    /// Create or restore a container.
                    /// </summary>
                    /// <param name="ostemplate">The OS template or backup file.</param>
                    /// <param name="vmid">The (unique) ID of the VM.</param>
                    /// <param name="arch">OS architecture type.
                    ///   Enum: amd64,i386,arm64,armhf,riscv32,riscv64</param>
                    /// <param name="bwlimit">Override I/O bandwidth limit (in KiB/s).</param>
                    /// <param name="cmode">Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode to 'shell', it simply invokes a shell inside the container (no login).
                    ///   Enum: shell,console,tty</param>
                    /// <param name="console">Attach a console device (/dev/console) to the container.</param>
                    /// <param name="cores">The number of cores assigned to the container. A container can use all available cores by default.</param>
                    /// <param name="cpulimit">Limit of CPU usage.  NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.</param>
                    /// <param name="cpuunits">CPU weight for a container, will be clamped to [1, 10000] in cgroup v2.</param>
                    /// <param name="debug">Try to be more verbose. For now this only enables debug log-level on start.</param>
                    /// <param name="description">Description for the Container. Shown in the web-interface CT's summary. This is saved as comment inside the configuration file.</param>
                    /// <param name="devN">Device to pass through to the container</param>
                    /// <param name="features">Allow containers access to advanced features.</param>
                    /// <param name="force">Allow to overwrite existing container.</param>
                    /// <param name="hookscript">Script that will be exectued during various steps in the containers lifetime.</param>
                    /// <param name="hostname">Set a host name for the container.</param>
                    /// <param name="ignore_unpack_errors">Ignore errors when extracting the template.</param>
                    /// <param name="lock_">Lock/unlock the container.
                    ///   Enum: backup,create,destroyed,disk,fstrim,migrate,mounted,rollback,snapshot,snapshot-delete</param>
                    /// <param name="memory">Amount of RAM for the container in MB.</param>
                    /// <param name="mpN">Use volume as container mount point. Use the special syntax STORAGE_ID:SIZE_IN_GiB to allocate a new volume.</param>
                    /// <param name="nameserver">Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.</param>
                    /// <param name="netN">Specifies network interfaces for the container.</param>
                    /// <param name="onboot">Specifies whether a container will be started during system bootup.</param>
                    /// <param name="ostype">OS type. This is used to setup configuration inside the container, and corresponds to lxc setup scripts in /usr/share/lxc/config/&amp;lt;ostype&amp;gt;.common.conf. Value 'unmanaged' can be used to skip and OS specific setup.
                    ///   Enum: debian,devuan,ubuntu,centos,fedora,opensuse,archlinux,alpine,gentoo,nixos,unmanaged</param>
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
                    /// <param name="swap">Amount of SWAP for the container in MB.</param>
                    /// <param name="tags">Tags of the Container. This is only meta information.</param>
                    /// <param name="template">Enable/disable Template.</param>
                    /// <param name="timezone">Time zone to use in the container. If option isn't set, then nothing will be done. Can be set to 'host' to match the host time zone, or an arbitrary time zone option from /usr/share/zoneinfo/zone.tab</param>
                    /// <param name="tty">Specify the number of tty available to the container</param>
                    /// <param name="unique">Assign a unique random ethernet address.</param>
                    /// <param name="unprivileged">Makes the container run as unprivileged user. (Should not be modified manually.)</param>
                    /// <param name="unusedN">Reference to unused volumes. This is used internally, and should not be modified manually.</param>
                    /// <returns></returns>
                    public async Task<Result> CreateVm(string ostemplate, int vmid, string arch = null, float? bwlimit = null, string cmode = null, bool? console = null, int? cores = null, float? cpulimit = null, int? cpuunits = null, bool? debug = null, string description = null, IDictionary<int, string> devN = null, string features = null, bool? force = null, string hookscript = null, string hostname = null, bool? ignore_unpack_errors = null, string lock_ = null, int? memory = null, IDictionary<int, string> mpN = null, string nameserver = null, IDictionary<int, string> netN = null, bool? onboot = null, string ostype = null, string password = null, string pool = null, bool? protection = null, bool? restore = null, string rootfs = null, string searchdomain = null, string ssh_public_keys = null, bool? start = null, string startup = null, string storage = null, int? swap = null, string tags = null, bool? template = null, string timezone = null, int? tty = null, bool? unique = null, bool? unprivileged = null, IDictionary<int, string> unusedN = null)
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
                        parameters.Add("debug", debug);
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
                        parameters.Add("tags", tags);
                        parameters.Add("template", template);
                        parameters.Add("timezone", timezone);
                        parameters.Add("tty", tty);
                        parameters.Add("unique", unique);
                        parameters.Add("unprivileged", unprivileged);
                        AddIndexedParameter(parameters, "dev", devN);
                        AddIndexedParameter(parameters, "mp", mpN);
                        AddIndexedParameter(parameters, "net", netN);
                        AddIndexedParameter(parameters, "unused", unusedN);
                        return await _client.Create($"/nodes/{_node}/lxc", parameters);
                    }
                }
                /// <summary>
                /// Ceph
                /// </summary>
                public class PveCeph
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveCeph(PveClient client, object node) { _client = client; _node = node; }
                    private PveCfg _cfg;
                    /// <summary>
                    /// Cfg
                    /// </summary>
                    public PveCfg Cfg => _cfg ??= new(_client, _node);
                    private PveOsd _osd;
                    /// <summary>
                    /// Osd
                    /// </summary>
                    public PveOsd Osd => _osd ??= new(_client, _node);
                    private PveMds _mds;
                    /// <summary>
                    /// Mds
                    /// </summary>
                    public PveMds Mds => _mds ??= new(_client, _node);
                    private PveMgr _mgr;
                    /// <summary>
                    /// Mgr
                    /// </summary>
                    public PveMgr Mgr => _mgr ??= new(_client, _node);
                    private PveMon _mon;
                    /// <summary>
                    /// Mon
                    /// </summary>
                    public PveMon Mon => _mon ??= new(_client, _node);
                    private PveFs _fs;
                    /// <summary>
                    /// Fs
                    /// </summary>
                    public PveFs Fs => _fs ??= new(_client, _node);
                    private PvePool _pool;
                    /// <summary>
                    /// Pool
                    /// </summary>
                    public PvePool Pool => _pool ??= new(_client, _node);
                    private PveInit _init;
                    /// <summary>
                    /// Init
                    /// </summary>
                    public PveInit Init => _init ??= new(_client, _node);
                    private PveStop _stop;
                    /// <summary>
                    /// Stop
                    /// </summary>
                    public PveStop Stop => _stop ??= new(_client, _node);
                    private PveStart _start;
                    /// <summary>
                    /// Start
                    /// </summary>
                    public PveStart Start => _start ??= new(_client, _node);
                    private PveRestart _restart;
                    /// <summary>
                    /// Restart
                    /// </summary>
                    public PveRestart Restart => _restart ??= new(_client, _node);
                    private PveStatus _status;
                    /// <summary>
                    /// Status
                    /// </summary>
                    public PveStatus Status => _status ??= new(_client, _node);
                    private PveCrush _crush;
                    /// <summary>
                    /// Crush
                    /// </summary>
                    public PveCrush Crush => _crush ??= new(_client, _node);
                    private PveLog _log;
                    /// <summary>
                    /// Log
                    /// </summary>
                    public PveLog Log => _log ??= new(_client, _node);
                    private PveRules _rules;
                    /// <summary>
                    /// Rules
                    /// </summary>
                    public PveRules Rules => _rules ??= new(_client, _node);
                    private PveCmdSafety _cmdSafety;
                    /// <summary>
                    /// CmdSafety
                    /// </summary>
                    public PveCmdSafety CmdSafety => _cmdSafety ??= new(_client, _node);
                    /// <summary>
                    /// Cfg
                    /// </summary>
                    public class PveCfg
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveCfg(PveClient client, object node) { _client = client; _node = node; }
                        private PveRaw _raw;
                        /// <summary>
                        /// Raw
                        /// </summary>
                        public PveRaw Raw => _raw ??= new(_client, _node);
                        private PveDb _db;
                        /// <summary>
                        /// Db
                        /// </summary>
                        public PveDb Db => _db ??= new(_client, _node);
                        private PveValue _value;
                        /// <summary>
                        /// Value
                        /// </summary>
                        public PveValue Value => _value ??= new(_client, _node);
                        /// <summary>
                        /// Raw
                        /// </summary>
                        public class PveRaw
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            internal PveRaw(PveClient client, object node) { _client = client; _node = node; }
                            /// <summary>
                            /// Get the Ceph configuration file.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Raw() { return await _client.Get($"/nodes/{_node}/ceph/cfg/raw"); }
                        }
                        /// <summary>
                        /// Db
                        /// </summary>
                        public class PveDb
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            internal PveDb(PveClient client, object node) { _client = client; _node = node; }
                            /// <summary>
                            /// Get the Ceph configuration database.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Db() { return await _client.Get($"/nodes/{_node}/ceph/cfg/db"); }
                        }
                        /// <summary>
                        /// Value
                        /// </summary>
                        public class PveValue
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            internal PveValue(PveClient client, object node) { _client = client; _node = node; }
                            /// <summary>
                            /// Get configured values from either the config file or config DB.
                            /// </summary>
                            /// <param name="config_keys">List of &amp;lt;section&amp;gt;:&amp;lt;config key&amp;gt; items.</param>
                            /// <returns></returns>
                            public async Task<Result> Value(string config_keys)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("config-keys", config_keys);
                                return await _client.Get($"/nodes/{_node}/ceph/cfg/value", parameters);
                            }
                        }
                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/ceph/cfg"); }
                    }
                    /// <summary>
                    /// Osd
                    /// </summary>
                    public class PveOsd
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveOsd(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// OsdidItem
                        /// </summary>
                        public PveOsdidItem this[object osdid] => new(_client, _node, osdid);
                        /// <summary>
                        /// OsdidItem
                        /// </summary>
                        public class PveOsdidItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _osdid;
                            internal PveOsdidItem(PveClient client, object node, object osdid)
                            {
                                _client = client; _node = node;
                                _osdid = osdid;
                            }
                            private PveMetadata _metadata;
                            /// <summary>
                            /// Metadata
                            /// </summary>
                            public PveMetadata Metadata => _metadata ??= new(_client, _node, _osdid);
                            private PveLvInfo _lvInfo;
                            /// <summary>
                            /// LvInfo
                            /// </summary>
                            public PveLvInfo LvInfo => _lvInfo ??= new(_client, _node, _osdid);
                            private PveIn _in;
                            /// <summary>
                            /// In
                            /// </summary>
                            public PveIn In => _in ??= new(_client, _node, _osdid);
                            private PveOut _out;
                            /// <summary>
                            /// Out
                            /// </summary>
                            public PveOut Out => _out ??= new(_client, _node, _osdid);
                            private PveScrub _scrub;
                            /// <summary>
                            /// Scrub
                            /// </summary>
                            public PveScrub Scrub => _scrub ??= new(_client, _node, _osdid);
                            /// <summary>
                            /// Metadata
                            /// </summary>
                            public class PveMetadata
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _osdid;
                                internal PveMetadata(PveClient client, object node, object osdid)
                                {
                                    _client = client; _node = node;
                                    _osdid = osdid;
                                }
                                /// <summary>
                                /// Get OSD details
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Osddetails() { return await _client.Get($"/nodes/{_node}/ceph/osd/{_osdid}/metadata"); }
                            }
                            /// <summary>
                            /// LvInfo
                            /// </summary>
                            public class PveLvInfo
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _osdid;
                                internal PveLvInfo(PveClient client, object node, object osdid)
                                {
                                    _client = client; _node = node;
                                    _osdid = osdid;
                                }
                                /// <summary>
                                /// Get OSD volume details
                                /// </summary>
                                /// <param name="type">OSD device type
                                ///   Enum: block,db,wal</param>
                                /// <returns></returns>
                                public async Task<Result> Osdvolume(string type = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("type", type);
                                    return await _client.Get($"/nodes/{_node}/ceph/osd/{_osdid}/lv-info", parameters);
                                }
                            }
                            /// <summary>
                            /// In
                            /// </summary>
                            public class PveIn
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _osdid;
                                internal PveIn(PveClient client, object node, object osdid)
                                {
                                    _client = client; _node = node;
                                    _osdid = osdid;
                                }
                                /// <summary>
                                /// ceph osd in
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> In() { return await _client.Create($"/nodes/{_node}/ceph/osd/{_osdid}/in"); }
                            }
                            /// <summary>
                            /// Out
                            /// </summary>
                            public class PveOut
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _osdid;
                                internal PveOut(PveClient client, object node, object osdid)
                                {
                                    _client = client; _node = node;
                                    _osdid = osdid;
                                }
                                /// <summary>
                                /// ceph osd out
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Out() { return await _client.Create($"/nodes/{_node}/ceph/osd/{_osdid}/out"); }
                            }
                            /// <summary>
                            /// Scrub
                            /// </summary>
                            public class PveScrub
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _osdid;
                                internal PveScrub(PveClient client, object node, object osdid)
                                {
                                    _client = client; _node = node;
                                    _osdid = osdid;
                                }
                                /// <summary>
                                /// Instruct the OSD to scrub.
                                /// </summary>
                                /// <param name="deep">If set, instructs a deep scrub instead of a normal one.</param>
                                /// <returns></returns>
                                public async Task<Result> Scrub(bool? deep = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("deep", deep);
                                    return await _client.Create($"/nodes/{_node}/ceph/osd/{_osdid}/scrub", parameters);
                                }
                            }
                            /// <summary>
                            /// Destroy OSD
                            /// </summary>
                            /// <param name="cleanup">If set, we remove partition table entries.</param>
                            /// <returns></returns>
                            public async Task<Result> Destroyosd(bool? cleanup = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cleanup", cleanup);
                                return await _client.Delete($"/nodes/{_node}/ceph/osd/{_osdid}", parameters);
                            }
                            /// <summary>
                            /// OSD index.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Osdindex() { return await _client.Get($"/nodes/{_node}/ceph/osd/{_osdid}"); }
                        }
                        /// <summary>
                        /// Get Ceph osd list/tree.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/ceph/osd"); }
                        /// <summary>
                        /// Create OSD
                        /// </summary>
                        /// <param name="dev">Block device name.</param>
                        /// <param name="crush_device_class">Set the device class of the OSD in crush.</param>
                        /// <param name="db_dev">Block device name for block.db.</param>
                        /// <param name="db_dev_size">Size in GiB for block.db.</param>
                        /// <param name="encrypted">Enables encryption of the OSD.</param>
                        /// <param name="osds_per_device">OSD services per physical device. Only useful for fast NVMe devices" 		    ." to utilize their performance better.</param>
                        /// <param name="wal_dev">Block device name for block.wal.</param>
                        /// <param name="wal_dev_size">Size in GiB for block.wal.</param>
                        /// <returns></returns>
                        public async Task<Result> Createosd(string dev, string crush_device_class = null, string db_dev = null, float? db_dev_size = null, bool? encrypted = null, int? osds_per_device = null, string wal_dev = null, float? wal_dev_size = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("dev", dev);
                            parameters.Add("crush-device-class", crush_device_class);
                            parameters.Add("db_dev", db_dev);
                            parameters.Add("db_dev_size", db_dev_size);
                            parameters.Add("encrypted", encrypted);
                            parameters.Add("osds-per-device", osds_per_device);
                            parameters.Add("wal_dev", wal_dev);
                            parameters.Add("wal_dev_size", wal_dev_size);
                            return await _client.Create($"/nodes/{_node}/ceph/osd", parameters);
                        }
                    }
                    /// <summary>
                    /// Mds
                    /// </summary>
                    public class PveMds
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveMds(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, _node, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Destroy Ceph Metadata Server
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Destroymds() { return await _client.Delete($"/nodes/{_node}/ceph/mds/{_name}"); }
                            /// <summary>
                            /// Create Ceph Metadata Server (MDS)
                            /// </summary>
                            /// <param name="hotstandby">Determines whether a ceph-mds daemon should poll and replay the log of an active MDS. Faster switch on MDS failure, but needs more idle resources.</param>
                            /// <returns></returns>
                            public async Task<Result> Createmds(bool? hotstandby = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("hotstandby", hotstandby);
                                return await _client.Create($"/nodes/{_node}/ceph/mds/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// MDS directory index.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/ceph/mds"); }
                    }
                    /// <summary>
                    /// Mgr
                    /// </summary>
                    public class PveMgr
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveMgr(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// IdItem
                        /// </summary>
                        public PveIdItem this[object id] => new(_client, _node, id);
                        /// <summary>
                        /// IdItem
                        /// </summary>
                        public class PveIdItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _id;
                            internal PveIdItem(PveClient client, object node, object id)
                            {
                                _client = client; _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Destroy Ceph Manager.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Destroymgr() { return await _client.Delete($"/nodes/{_node}/ceph/mgr/{_id}"); }
                            /// <summary>
                            /// Create Ceph Manager
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Createmgr() { return await _client.Create($"/nodes/{_node}/ceph/mgr/{_id}"); }
                        }
                        /// <summary>
                        /// MGR directory index.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/ceph/mgr"); }
                    }
                    /// <summary>
                    /// Mon
                    /// </summary>
                    public class PveMon
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveMon(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// MonidItem
                        /// </summary>
                        public PveMonidItem this[object monid] => new(_client, _node, monid);
                        /// <summary>
                        /// MonidItem
                        /// </summary>
                        public class PveMonidItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _monid;
                            internal PveMonidItem(PveClient client, object node, object monid)
                            {
                                _client = client; _node = node;
                                _monid = monid;
                            }
                            /// <summary>
                            /// Destroy Ceph Monitor and Manager.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Destroymon() { return await _client.Delete($"/nodes/{_node}/ceph/mon/{_monid}"); }
                            /// <summary>
                            /// Create Ceph Monitor and Manager
                            /// </summary>
                            /// <param name="mon_address">Overwrites autodetected monitor IP address(es). Must be in the public network(s) of Ceph.</param>
                            /// <returns></returns>
                            public async Task<Result> Createmon(string mon_address = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("mon-address", mon_address);
                                return await _client.Create($"/nodes/{_node}/ceph/mon/{_monid}", parameters);
                            }
                        }
                        /// <summary>
                        /// Get Ceph monitor list.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Listmon() { return await _client.Get($"/nodes/{_node}/ceph/mon"); }
                    }
                    /// <summary>
                    /// Fs
                    /// </summary>
                    public class PveFs
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveFs(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, _node, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object node, object name)
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
                            public async Task<Result> Createfs(bool? add_storage = null, int? pg_num = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("add-storage", add_storage);
                                parameters.Add("pg_num", pg_num);
                                return await _client.Create($"/nodes/{_node}/ceph/fs/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/ceph/fs"); }
                    }
                    /// <summary>
                    /// Pool
                    /// </summary>
                    public class PvePool
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PvePool(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, _node, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            private PveStatus _status;
                            /// <summary>
                            /// Status
                            /// </summary>
                            public PveStatus Status => _status ??= new(_client, _node, _name);
                            /// <summary>
                            /// Status
                            /// </summary>
                            public class PveStatus
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _name;
                                internal PveStatus(PveClient client, object node, object name)
                                {
                                    _client = client; _node = node;
                                    _name = name;
                                }
                                /// <summary>
                                /// Show the current pool status.
                                /// </summary>
                                /// <param name="verbose">If enabled, will display additional data(eg. statistics).</param>
                                /// <returns></returns>
                                public async Task<Result> Getpool(bool? verbose = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("verbose", verbose);
                                    return await _client.Get($"/nodes/{_node}/ceph/pool/{_name}/status", parameters);
                                }
                            }
                            /// <summary>
                            /// Destroy pool
                            /// </summary>
                            /// <param name="force">If true, destroys pool even if in use</param>
                            /// <param name="remove_ecprofile">Remove the erasure code profile. Defaults to true, if applicable.</param>
                            /// <param name="remove_storages">Remove all pveceph-managed storages configured for this pool</param>
                            /// <returns></returns>
                            public async Task<Result> Destroypool(bool? force = null, bool? remove_ecprofile = null, bool? remove_storages = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                parameters.Add("remove_ecprofile", remove_ecprofile);
                                parameters.Add("remove_storages", remove_storages);
                                return await _client.Delete($"/nodes/{_node}/ceph/pool/{_name}", parameters);
                            }
                            /// <summary>
                            /// Pool index.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Poolindex() { return await _client.Get($"/nodes/{_node}/ceph/pool/{_name}"); }
                            /// <summary>
                            /// Change POOL settings
                            /// </summary>
                            /// <param name="application">The application of the pool.
                            ///   Enum: rbd,cephfs,rgw</param>
                            /// <param name="crush_rule">The rule to use for mapping object placement in the cluster.</param>
                            /// <param name="min_size">Minimum number of replicas per object</param>
                            /// <param name="pg_autoscale_mode">The automatic PG scaling mode of the pool.
                            ///   Enum: on,off,warn</param>
                            /// <param name="pg_num">Number of placement groups.</param>
                            /// <param name="pg_num_min">Minimal number of placement groups.</param>
                            /// <param name="size">Number of replicas per object</param>
                            /// <param name="target_size">The estimated target size of the pool for the PG autoscaler.</param>
                            /// <param name="target_size_ratio">The estimated target ratio of the pool for the PG autoscaler.</param>
                            /// <returns></returns>
                            public async Task<Result> Setpool(string application = null, string crush_rule = null, int? min_size = null, string pg_autoscale_mode = null, int? pg_num = null, int? pg_num_min = null, int? size = null, string target_size = null, float? target_size_ratio = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("application", application);
                                parameters.Add("crush_rule", crush_rule);
                                parameters.Add("min_size", min_size);
                                parameters.Add("pg_autoscale_mode", pg_autoscale_mode);
                                parameters.Add("pg_num", pg_num);
                                parameters.Add("pg_num_min", pg_num_min);
                                parameters.Add("size", size);
                                parameters.Add("target_size", target_size);
                                parameters.Add("target_size_ratio", target_size_ratio);
                                return await _client.Set($"/nodes/{_node}/ceph/pool/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// List all pools and their settings (which are settable by the POST/PUT endpoints).
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Lspools() { return await _client.Get($"/nodes/{_node}/ceph/pool"); }
                        /// <summary>
                        /// Create Ceph pool
                        /// </summary>
                        /// <param name="name">The name of the pool. It must be unique.</param>
                        /// <param name="add_storages">Configure VM and CT storage using the new pool.</param>
                        /// <param name="application">The application of the pool.
                        ///   Enum: rbd,cephfs,rgw</param>
                        /// <param name="crush_rule">The rule to use for mapping object placement in the cluster.</param>
                        /// <param name="erasure_coding">Create an erasure coded pool for RBD with an accompaning replicated pool for metadata storage. With EC, the common ceph options 'size', 'min_size' and 'crush_rule' parameters will be applied to the metadata pool.</param>
                        /// <param name="min_size">Minimum number of replicas per object</param>
                        /// <param name="pg_autoscale_mode">The automatic PG scaling mode of the pool.
                        ///   Enum: on,off,warn</param>
                        /// <param name="pg_num">Number of placement groups.</param>
                        /// <param name="pg_num_min">Minimal number of placement groups.</param>
                        /// <param name="size">Number of replicas per object</param>
                        /// <param name="target_size">The estimated target size of the pool for the PG autoscaler.</param>
                        /// <param name="target_size_ratio">The estimated target ratio of the pool for the PG autoscaler.</param>
                        /// <returns></returns>
                        public async Task<Result> Createpool(string name, bool? add_storages = null, string application = null, string crush_rule = null, string erasure_coding = null, int? min_size = null, string pg_autoscale_mode = null, int? pg_num = null, int? pg_num_min = null, int? size = null, string target_size = null, float? target_size_ratio = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("name", name);
                            parameters.Add("add_storages", add_storages);
                            parameters.Add("application", application);
                            parameters.Add("crush_rule", crush_rule);
                            parameters.Add("erasure-coding", erasure_coding);
                            parameters.Add("min_size", min_size);
                            parameters.Add("pg_autoscale_mode", pg_autoscale_mode);
                            parameters.Add("pg_num", pg_num);
                            parameters.Add("pg_num_min", pg_num_min);
                            parameters.Add("size", size);
                            parameters.Add("target_size", target_size);
                            parameters.Add("target_size_ratio", target_size_ratio);
                            return await _client.Create($"/nodes/{_node}/ceph/pool", parameters);
                        }
                    }
                    /// <summary>
                    /// Init
                    /// </summary>
                    public class PveInit
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveInit(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Create initial ceph default configuration and setup symlinks.
                        /// </summary>
                        /// <param name="cluster_network">Declare a separate cluster network, OSDs will routeheartbeat, object replication and recovery traffic over it</param>
                        /// <param name="disable_cephx">Disable cephx authentication.  WARNING: cephx is a security feature protecting against man-in-the-middle attacks. Only consider disabling cephx if your network is private!</param>
                        /// <param name="min_size">Minimum number of available replicas per object to allow I/O</param>
                        /// <param name="network">Use specific network for all ceph related traffic</param>
                        /// <param name="pg_bits">Placement group bits, used to specify the default number of placement groups.  Depreacted. This setting was deprecated in recent Ceph versions.</param>
                        /// <param name="size">Targeted number of replicas per object</param>
                        /// <returns></returns>
                        public async Task<Result> Init(string cluster_network = null, bool? disable_cephx = null, int? min_size = null, string network = null, int? pg_bits = null, int? size = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("cluster-network", cluster_network);
                            parameters.Add("disable_cephx", disable_cephx);
                            parameters.Add("min_size", min_size);
                            parameters.Add("network", network);
                            parameters.Add("pg_bits", pg_bits);
                            parameters.Add("size", size);
                            return await _client.Create($"/nodes/{_node}/ceph/init", parameters);
                        }
                    }
                    /// <summary>
                    /// Stop
                    /// </summary>
                    public class PveStop
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveStop(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Stop ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public async Task<Result> Stop(string service = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("service", service);
                            return await _client.Create($"/nodes/{_node}/ceph/stop", parameters);
                        }
                    }
                    /// <summary>
                    /// Start
                    /// </summary>
                    public class PveStart
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveStart(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Start ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public async Task<Result> Start(string service = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("service", service);
                            return await _client.Create($"/nodes/{_node}/ceph/start", parameters);
                        }
                    }
                    /// <summary>
                    /// Restart
                    /// </summary>
                    public class PveRestart
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveRestart(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Restart ceph services.
                        /// </summary>
                        /// <param name="service">Ceph service name.</param>
                        /// <returns></returns>
                        public async Task<Result> Restart(string service = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("service", service);
                            return await _client.Create($"/nodes/{_node}/ceph/restart", parameters);
                        }
                    }
                    /// <summary>
                    /// Status
                    /// </summary>
                    public class PveStatus
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveStatus(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get ceph status.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Status() { return await _client.Get($"/nodes/{_node}/ceph/status"); }
                    }
                    /// <summary>
                    /// Crush
                    /// </summary>
                    public class PveCrush
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveCrush(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get OSD crush map
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Crush() { return await _client.Get($"/nodes/{_node}/ceph/crush"); }
                    }
                    /// <summary>
                    /// Log
                    /// </summary>
                    public class PveLog
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveLog(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Read ceph log
                        /// </summary>
                        /// <param name="limit"></param>
                        /// <param name="start"></param>
                        /// <returns></returns>
                        public async Task<Result> Log(int? limit = null, int? start = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("limit", limit);
                            parameters.Add("start", start);
                            return await _client.Get($"/nodes/{_node}/ceph/log", parameters);
                        }
                    }
                    /// <summary>
                    /// Rules
                    /// </summary>
                    public class PveRules
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveRules(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List ceph rules.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Rules() { return await _client.Get($"/nodes/{_node}/ceph/rules"); }
                    }
                    /// <summary>
                    /// CmdSafety
                    /// </summary>
                    public class PveCmdSafety
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveCmdSafety(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Heuristical check if it is safe to perform an action.
                        /// </summary>
                        /// <param name="action">Action to check
                        ///   Enum: stop,destroy</param>
                        /// <param name="id">ID of the service</param>
                        /// <param name="service">Service type
                        ///   Enum: osd,mon,mds</param>
                        /// <returns></returns>
                        public async Task<Result> CmdSafety(string action, string id, string service)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("id", id);
                            parameters.Add("service", service);
                            return await _client.Get($"/nodes/{_node}/ceph/cmd-safety", parameters);
                        }
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/ceph"); }
                }
                /// <summary>
                /// Vzdump
                /// </summary>
                public class PveVzdump
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveVzdump(PveClient client, object node) { _client = client; _node = node; }
                    private PveDefaults _defaults;
                    /// <summary>
                    /// Defaults
                    /// </summary>
                    public PveDefaults Defaults => _defaults ??= new(_client, _node);
                    private PveExtractconfig _extractconfig;
                    /// <summary>
                    /// Extractconfig
                    /// </summary>
                    public PveExtractconfig Extractconfig => _extractconfig ??= new(_client, _node);
                    /// <summary>
                    /// Defaults
                    /// </summary>
                    public class PveDefaults
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveDefaults(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get the currently configured vzdump defaults.
                        /// </summary>
                        /// <param name="storage">The storage identifier.</param>
                        /// <returns></returns>
                        public async Task<Result> Defaults(string storage = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("storage", storage);
                            return await _client.Get($"/nodes/{_node}/vzdump/defaults", parameters);
                        }
                    }
                    /// <summary>
                    /// Extractconfig
                    /// </summary>
                    public class PveExtractconfig
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveExtractconfig(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Extract configuration from vzdump backup archive.
                        /// </summary>
                        /// <param name="volume">Volume identifier</param>
                        /// <returns></returns>
                        public async Task<Result> Extractconfig(string volume)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("volume", volume);
                            return await _client.Get($"/nodes/{_node}/vzdump/extractconfig", parameters);
                        }
                    }
                    /// <summary>
                    /// Create backup.
                    /// </summary>
                    /// <param name="all">Backup all known guest systems on this host.</param>
                    /// <param name="bwlimit">Limit I/O bandwidth (in KiB/s).</param>
                    /// <param name="compress">Compress dump file.
                    ///   Enum: 0,1,gzip,lzo,zstd</param>
                    /// <param name="dumpdir">Store resulting files to specified directory.</param>
                    /// <param name="exclude">Exclude specified guest systems (assumes --all)</param>
                    /// <param name="exclude_path">Exclude certain files/directories (shell globs). Paths starting with '/' are anchored to the container's root,  other paths match relative to each subdirectory.</param>
                    /// <param name="ionice">Set IO priority when using the BFQ scheduler. For snapshot and suspend mode backups of VMs, this only affects the compressor. A value of 8 means the idle priority is used, otherwise the best-effort priority is used with the specified value.</param>
                    /// <param name="lockwait">Maximal time to wait for the global lock (minutes).</param>
                    /// <param name="mailnotification">Deprecated: use 'notification-policy' instead.
                    ///   Enum: always,failure</param>
                    /// <param name="mailto">Comma-separated list of email addresses or users that should receive email notifications. Has no effect if the 'notification-target' option  is set at the same time.</param>
                    /// <param name="maxfiles">Deprecated: use 'prune-backups' instead. Maximal number of backup files per guest system.</param>
                    /// <param name="mode">Backup mode.
                    ///   Enum: snapshot,suspend,stop</param>
                    /// <param name="notes_template">Template string for generating notes for the backup(s). It can contain variables which will be replaced by their values. Currently supported are {{cluster}}, {{guestname}}, {{node}}, and {{vmid}}, but more might be added in the future. Needs to be a single line, newline and backslash need to be escaped as '\n' and '\\' respectively.</param>
                    /// <param name="notification_policy">Specify when to send a notification
                    ///   Enum: always,failure,never</param>
                    /// <param name="notification_target">Determine the target to which notifications should be sent. Can either be a notification endpoint or a notification group. This option takes precedence over 'mailto', meaning that if both are  set, the 'mailto' option will be ignored.</param>
                    /// <param name="performance">Other performance-related settings.</param>
                    /// <param name="pigz">Use pigz instead of gzip when N&amp;gt;0. N=1 uses half of cores, N&amp;gt;1 uses N as thread count.</param>
                    /// <param name="pool">Backup all known guest systems included in the specified pool.</param>
                    /// <param name="protected_">If true, mark backup(s) as protected.</param>
                    /// <param name="prune_backups">Use these retention options instead of those from the storage configuration.</param>
                    /// <param name="quiet">Be quiet.</param>
                    /// <param name="remove">Prune older backups according to 'prune-backups'.</param>
                    /// <param name="script">Use specified hook script.</param>
                    /// <param name="stdexcludes">Exclude temporary files and logs.</param>
                    /// <param name="stdout">Write tar to stdout, not to a file.</param>
                    /// <param name="stop">Stop running backup jobs on this host.</param>
                    /// <param name="stopwait">Maximal time to wait until a guest system is stopped (minutes).</param>
                    /// <param name="storage">Store resulting file to this storage.</param>
                    /// <param name="tmpdir">Store temporary files to specified directory.</param>
                    /// <param name="vmid">The ID of the guest system you want to backup.</param>
                    /// <param name="zstd">Zstd threads. N=0 uses half of the available cores, N&amp;gt;0 uses N as thread count.</param>
                    /// <returns></returns>
                    public async Task<Result> Vzdump(bool? all = null, int? bwlimit = null, string compress = null, string dumpdir = null, string exclude = null, string exclude_path = null, int? ionice = null, int? lockwait = null, string mailnotification = null, string mailto = null, int? maxfiles = null, string mode = null, string notes_template = null, string notification_policy = null, string notification_target = null, string performance = null, int? pigz = null, string pool = null, bool? protected_ = null, string prune_backups = null, bool? quiet = null, bool? remove = null, string script = null, bool? stdexcludes = null, bool? stdout = null, bool? stop = null, int? stopwait = null, string storage = null, string tmpdir = null, string vmid = null, int? zstd = null)
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
                        parameters.Add("notes-template", notes_template);
                        parameters.Add("notification-policy", notification_policy);
                        parameters.Add("notification-target", notification_target);
                        parameters.Add("performance", performance);
                        parameters.Add("pigz", pigz);
                        parameters.Add("pool", pool);
                        parameters.Add("protected", protected_);
                        parameters.Add("prune-backups", prune_backups);
                        parameters.Add("quiet", quiet);
                        parameters.Add("remove", remove);
                        parameters.Add("script", script);
                        parameters.Add("stdexcludes", stdexcludes);
                        parameters.Add("stdout", stdout);
                        parameters.Add("stop", stop);
                        parameters.Add("stopwait", stopwait);
                        parameters.Add("storage", storage);
                        parameters.Add("tmpdir", tmpdir);
                        parameters.Add("vmid", vmid);
                        parameters.Add("zstd", zstd);
                        return await _client.Create($"/nodes/{_node}/vzdump", parameters);
                    }
                }
                /// <summary>
                /// Services
                /// </summary>
                public class PveServices
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveServices(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// ServiceItem
                    /// </summary>
                    public PveServiceItem this[object service] => new(_client, _node, service);
                    /// <summary>
                    /// ServiceItem
                    /// </summary>
                    public class PveServiceItem
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        private readonly object _service;
                        internal PveServiceItem(PveClient client, object node, object service)
                        {
                            _client = client; _node = node;
                            _service = service;
                        }
                        private PveState _state;
                        /// <summary>
                        /// State
                        /// </summary>
                        public PveState State => _state ??= new(_client, _node, _service);
                        private PveStart _start;
                        /// <summary>
                        /// Start
                        /// </summary>
                        public PveStart Start => _start ??= new(_client, _node, _service);
                        private PveStop _stop;
                        /// <summary>
                        /// Stop
                        /// </summary>
                        public PveStop Stop => _stop ??= new(_client, _node, _service);
                        private PveRestart _restart;
                        /// <summary>
                        /// Restart
                        /// </summary>
                        public PveRestart Restart => _restart ??= new(_client, _node, _service);
                        private PveReload _reload;
                        /// <summary>
                        /// Reload
                        /// </summary>
                        public PveReload Reload => _reload ??= new(_client, _node, _service);
                        /// <summary>
                        /// State
                        /// </summary>
                        public class PveState
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PveState(PveClient client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Read service properties
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ServiceState() { return await _client.Get($"/nodes/{_node}/services/{_service}/state"); }
                        }
                        /// <summary>
                        /// Start
                        /// </summary>
                        public class PveStart
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PveStart(PveClient client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Start service.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ServiceStart() { return await _client.Create($"/nodes/{_node}/services/{_service}/start"); }
                        }
                        /// <summary>
                        /// Stop
                        /// </summary>
                        public class PveStop
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PveStop(PveClient client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Stop service.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ServiceStop() { return await _client.Create($"/nodes/{_node}/services/{_service}/stop"); }
                        }
                        /// <summary>
                        /// Restart
                        /// </summary>
                        public class PveRestart
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PveRestart(PveClient client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Hard restart service. Use reload if you want to reduce interruptions.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ServiceRestart() { return await _client.Create($"/nodes/{_node}/services/{_service}/restart"); }
                        }
                        /// <summary>
                        /// Reload
                        /// </summary>
                        public class PveReload
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _service;
                            internal PveReload(PveClient client, object node, object service)
                            {
                                _client = client; _node = node;
                                _service = service;
                            }
                            /// <summary>
                            /// Reload service. Falls back to restart if service cannot be reloaded.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ServiceReload() { return await _client.Create($"/nodes/{_node}/services/{_service}/reload"); }
                        }
                        /// <summary>
                        /// Directory index
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Srvcmdidx() { return await _client.Get($"/nodes/{_node}/services/{_service}"); }
                    }
                    /// <summary>
                    /// Service list.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/services"); }
                }
                /// <summary>
                /// Subscription
                /// </summary>
                public class PveSubscription
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveSubscription(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Delete subscription key of this node.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Delete() { return await _client.Delete($"/nodes/{_node}/subscription"); }
                    /// <summary>
                    /// Read subscription info.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Get() { return await _client.Get($"/nodes/{_node}/subscription"); }
                    /// <summary>
                    /// Update subscription info.
                    /// </summary>
                    /// <param name="force">Always connect to server, even if local cache is still valid.</param>
                    /// <returns></returns>
                    public async Task<Result> Update(bool? force = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        return await _client.Create($"/nodes/{_node}/subscription", parameters);
                    }
                    /// <summary>
                    /// Set subscription key.
                    /// </summary>
                    /// <param name="key">Proxmox VE subscription key</param>
                    /// <returns></returns>
                    public async Task<Result> Set(string key)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("key", key);
                        return await _client.Set($"/nodes/{_node}/subscription", parameters);
                    }
                }
                /// <summary>
                /// Network
                /// </summary>
                public class PveNetwork
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveNetwork(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// IfaceItem
                    /// </summary>
                    public PveIfaceItem this[object iface] => new(_client, _node, iface);
                    /// <summary>
                    /// IfaceItem
                    /// </summary>
                    public class PveIfaceItem
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        private readonly object _iface;
                        internal PveIfaceItem(PveClient client, object node, object iface)
                        {
                            _client = client; _node = node;
                            _iface = iface;
                        }
                        /// <summary>
                        /// Delete network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> DeleteNetwork() { return await _client.Delete($"/nodes/{_node}/network/{_iface}"); }
                        /// <summary>
                        /// Read network device configuration
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> NetworkConfig() { return await _client.Get($"/nodes/{_node}/network/{_iface}"); }
                        /// <summary>
                        /// Update network device configuration
                        /// </summary>
                        /// <param name="type">Network interface type
                        ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,unknown</param>
                        /// <param name="address">IP address.</param>
                        /// <param name="address6">IP address.</param>
                        /// <param name="autostart">Automatically start interface on boot.</param>
                        /// <param name="bond_primary">Specify the primary interface for active-backup bond.</param>
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
                        /// <param name="mtu">MTU.</param>
                        /// <param name="netmask">Network mask.</param>
                        /// <param name="netmask6">Network mask.</param>
                        /// <param name="ovs_bonds">Specify the interfaces used by the bonding device.</param>
                        /// <param name="ovs_bridge">The OVS bridge associated with a OVS port. This is required when you create an OVS port.</param>
                        /// <param name="ovs_options">OVS interface options.</param>
                        /// <param name="ovs_ports">Specify the interfaces you want to add to your bridge.</param>
                        /// <param name="ovs_tag">Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)</param>
                        /// <param name="slaves">Specify the interfaces used by the bonding device.</param>
                        /// <param name="vlan_id">vlan-id for a custom named vlan interface (ifupdown2 only).</param>
                        /// <param name="vlan_raw_device">Specify the raw interface for the vlan interface.</param>
                        /// <returns></returns>
                        public async Task<Result> UpdateNetwork(string type, string address = null, string address6 = null, bool? autostart = null, string bond_primary = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string cidr = null, string cidr6 = null, string comments = null, string comments6 = null, string delete = null, string gateway = null, string gateway6 = null, int? mtu = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null, int? vlan_id = null, string vlan_raw_device = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("type", type);
                            parameters.Add("address", address);
                            parameters.Add("address6", address6);
                            parameters.Add("autostart", autostart);
                            parameters.Add("bond-primary", bond_primary);
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
                            parameters.Add("mtu", mtu);
                            parameters.Add("netmask", netmask);
                            parameters.Add("netmask6", netmask6);
                            parameters.Add("ovs_bonds", ovs_bonds);
                            parameters.Add("ovs_bridge", ovs_bridge);
                            parameters.Add("ovs_options", ovs_options);
                            parameters.Add("ovs_ports", ovs_ports);
                            parameters.Add("ovs_tag", ovs_tag);
                            parameters.Add("slaves", slaves);
                            parameters.Add("vlan-id", vlan_id);
                            parameters.Add("vlan-raw-device", vlan_raw_device);
                            return await _client.Set($"/nodes/{_node}/network/{_iface}", parameters);
                        }
                    }
                    /// <summary>
                    /// Revert network configuration changes.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> RevertNetworkChanges() { return await _client.Delete($"/nodes/{_node}/network"); }
                    /// <summary>
                    /// List available networks
                    /// </summary>
                    /// <param name="type">Only list specific interface types.
                    ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,any_bridge,any_local_bridge</param>
                    /// <returns></returns>
                    public async Task<Result> Index(string type = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        return await _client.Get($"/nodes/{_node}/network", parameters);
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
                    /// <param name="bond_primary">Specify the primary interface for active-backup bond.</param>
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
                    /// <param name="mtu">MTU.</param>
                    /// <param name="netmask">Network mask.</param>
                    /// <param name="netmask6">Network mask.</param>
                    /// <param name="ovs_bonds">Specify the interfaces used by the bonding device.</param>
                    /// <param name="ovs_bridge">The OVS bridge associated with a OVS port. This is required when you create an OVS port.</param>
                    /// <param name="ovs_options">OVS interface options.</param>
                    /// <param name="ovs_ports">Specify the interfaces you want to add to your bridge.</param>
                    /// <param name="ovs_tag">Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)</param>
                    /// <param name="slaves">Specify the interfaces used by the bonding device.</param>
                    /// <param name="vlan_id">vlan-id for a custom named vlan interface (ifupdown2 only).</param>
                    /// <param name="vlan_raw_device">Specify the raw interface for the vlan interface.</param>
                    /// <returns></returns>
                    public async Task<Result> CreateNetwork(string iface, string type, string address = null, string address6 = null, bool? autostart = null, string bond_primary = null, string bond_mode = null, string bond_xmit_hash_policy = null, string bridge_ports = null, bool? bridge_vlan_aware = null, string cidr = null, string cidr6 = null, string comments = null, string comments6 = null, string gateway = null, string gateway6 = null, int? mtu = null, string netmask = null, int? netmask6 = null, string ovs_bonds = null, string ovs_bridge = null, string ovs_options = null, string ovs_ports = null, int? ovs_tag = null, string slaves = null, int? vlan_id = null, string vlan_raw_device = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("iface", iface);
                        parameters.Add("type", type);
                        parameters.Add("address", address);
                        parameters.Add("address6", address6);
                        parameters.Add("autostart", autostart);
                        parameters.Add("bond-primary", bond_primary);
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
                        parameters.Add("mtu", mtu);
                        parameters.Add("netmask", netmask);
                        parameters.Add("netmask6", netmask6);
                        parameters.Add("ovs_bonds", ovs_bonds);
                        parameters.Add("ovs_bridge", ovs_bridge);
                        parameters.Add("ovs_options", ovs_options);
                        parameters.Add("ovs_ports", ovs_ports);
                        parameters.Add("ovs_tag", ovs_tag);
                        parameters.Add("slaves", slaves);
                        parameters.Add("vlan-id", vlan_id);
                        parameters.Add("vlan-raw-device", vlan_raw_device);
                        return await _client.Create($"/nodes/{_node}/network", parameters);
                    }
                    /// <summary>
                    /// Reload network configuration
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> ReloadNetworkConfig() { return await _client.Set($"/nodes/{_node}/network"); }
                }
                /// <summary>
                /// Tasks
                /// </summary>
                public class PveTasks
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveTasks(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// UpidItem
                    /// </summary>
                    public PveUpidItem this[object upid] => new(_client, _node, upid);
                    /// <summary>
                    /// UpidItem
                    /// </summary>
                    public class PveUpidItem
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        private readonly object _upid;
                        internal PveUpidItem(PveClient client, object node, object upid)
                        {
                            _client = client; _node = node;
                            _upid = upid;
                        }
                        private PveLog _log;
                        /// <summary>
                        /// Log
                        /// </summary>
                        public PveLog Log => _log ??= new(_client, _node, _upid);
                        private PveStatus _status;
                        /// <summary>
                        /// Status
                        /// </summary>
                        public PveStatus Status => _status ??= new(_client, _node, _upid);
                        /// <summary>
                        /// Log
                        /// </summary>
                        public class PveLog
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _upid;
                            internal PveLog(PveClient client, object node, object upid)
                            {
                                _client = client; _node = node;
                                _upid = upid;
                            }
                            /// <summary>
                            /// Read task log.
                            /// </summary>
                            /// <param name="download">Whether the tasklog file should be downloaded. This parameter can't be used in conjunction with other parameters</param>
                            /// <param name="limit">The amount of lines to read from the tasklog.</param>
                            /// <param name="start">Start at this line when reading the tasklog</param>
                            /// <returns></returns>
                            public async Task<Result> ReadTaskLog(bool? download = null, int? limit = null, int? start = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("download", download);
                                parameters.Add("limit", limit);
                                parameters.Add("start", start);
                                return await _client.Get($"/nodes/{_node}/tasks/{_upid}/log", parameters);
                            }
                        }
                        /// <summary>
                        /// Status
                        /// </summary>
                        public class PveStatus
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _upid;
                            internal PveStatus(PveClient client, object node, object upid)
                            {
                                _client = client; _node = node;
                                _upid = upid;
                            }
                            /// <summary>
                            /// Read task status.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ReadTaskStatus() { return await _client.Get($"/nodes/{_node}/tasks/{_upid}/status"); }
                        }
                        /// <summary>
                        /// Stop a task.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> StopTask() { return await _client.Delete($"/nodes/{_node}/tasks/{_upid}"); }
                        /// <summary>
                        ///
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> UpidIndex() { return await _client.Get($"/nodes/{_node}/tasks/{_upid}"); }
                    }
                    /// <summary>
                    /// Read task list for one node (finished tasks).
                    /// </summary>
                    /// <param name="errors">Only list tasks with a status of ERROR.</param>
                    /// <param name="limit">Only list this amount of tasks.</param>
                    /// <param name="since">Only list tasks since this UNIX epoch.</param>
                    /// <param name="source">List archived, active or all tasks.
                    ///   Enum: archive,active,all</param>
                    /// <param name="start">List tasks beginning from this offset.</param>
                    /// <param name="statusfilter">List of Task States that should be returned.</param>
                    /// <param name="typefilter">Only list tasks of this type (e.g., vzstart, vzdump).</param>
                    /// <param name="until">Only list tasks until this UNIX epoch.</param>
                    /// <param name="userfilter">Only list tasks from this user.</param>
                    /// <param name="vmid">Only list tasks for this VM.</param>
                    /// <returns></returns>
                    public async Task<Result> NodeTasks(bool? errors = null, int? limit = null, int? since = null, string source = null, int? start = null, string statusfilter = null, string typefilter = null, int? until = null, string userfilter = null, int? vmid = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("errors", errors);
                        parameters.Add("limit", limit);
                        parameters.Add("since", since);
                        parameters.Add("source", source);
                        parameters.Add("start", start);
                        parameters.Add("statusfilter", statusfilter);
                        parameters.Add("typefilter", typefilter);
                        parameters.Add("until", until);
                        parameters.Add("userfilter", userfilter);
                        parameters.Add("vmid", vmid);
                        return await _client.Get($"/nodes/{_node}/tasks", parameters);
                    }
                }
                /// <summary>
                /// Scan
                /// </summary>
                public class PveScan
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveScan(PveClient client, object node) { _client = client; _node = node; }
                    private PveNfs _nfs;
                    /// <summary>
                    /// Nfs
                    /// </summary>
                    public PveNfs Nfs => _nfs ??= new(_client, _node);
                    private PveCifs _cifs;
                    /// <summary>
                    /// Cifs
                    /// </summary>
                    public PveCifs Cifs => _cifs ??= new(_client, _node);
                    private PvePbs _pbs;
                    /// <summary>
                    /// Pbs
                    /// </summary>
                    public PvePbs Pbs => _pbs ??= new(_client, _node);
                    private PveGlusterfs _glusterfs;
                    /// <summary>
                    /// Glusterfs
                    /// </summary>
                    public PveGlusterfs Glusterfs => _glusterfs ??= new(_client, _node);
                    private PveIscsi _iscsi;
                    /// <summary>
                    /// Iscsi
                    /// </summary>
                    public PveIscsi Iscsi => _iscsi ??= new(_client, _node);
                    private PveLvm _lvm;
                    /// <summary>
                    /// Lvm
                    /// </summary>
                    public PveLvm Lvm => _lvm ??= new(_client, _node);
                    private PveLvmthin _lvmthin;
                    /// <summary>
                    /// Lvmthin
                    /// </summary>
                    public PveLvmthin Lvmthin => _lvmthin ??= new(_client, _node);
                    private PveZfs _zfs;
                    /// <summary>
                    /// Zfs
                    /// </summary>
                    public PveZfs Zfs => _zfs ??= new(_client, _node);
                    /// <summary>
                    /// Nfs
                    /// </summary>
                    public class PveNfs
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveNfs(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote NFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <returns></returns>
                        public async Task<Result> Nfsscan(string server)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("server", server);
                            return await _client.Get($"/nodes/{_node}/scan/nfs", parameters);
                        }
                    }
                    /// <summary>
                    /// Cifs
                    /// </summary>
                    public class PveCifs
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveCifs(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote CIFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <param name="domain">SMB domain (Workgroup).</param>
                        /// <param name="password">User password.</param>
                        /// <param name="username">User name.</param>
                        /// <returns></returns>
                        public async Task<Result> Cifsscan(string server, string domain = null, string password = null, string username = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("server", server);
                            parameters.Add("domain", domain);
                            parameters.Add("password", password);
                            parameters.Add("username", username);
                            return await _client.Get($"/nodes/{_node}/scan/cifs", parameters);
                        }
                    }
                    /// <summary>
                    /// Pbs
                    /// </summary>
                    public class PvePbs
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PvePbs(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote Proxmox Backup Server.
                        /// </summary>
                        /// <param name="password">User password or API token secret.</param>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <param name="username">User-name or API token-ID.</param>
                        /// <param name="fingerprint">Certificate SHA 256 fingerprint.</param>
                        /// <param name="port">Optional port.</param>
                        /// <returns></returns>
                        public async Task<Result> Pbsscan(string password, string server, string username, string fingerprint = null, int? port = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("password", password);
                            parameters.Add("server", server);
                            parameters.Add("username", username);
                            parameters.Add("fingerprint", fingerprint);
                            parameters.Add("port", port);
                            return await _client.Get($"/nodes/{_node}/scan/pbs", parameters);
                        }
                    }
                    /// <summary>
                    /// Glusterfs
                    /// </summary>
                    public class PveGlusterfs
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveGlusterfs(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote GlusterFS server.
                        /// </summary>
                        /// <param name="server">The server address (name or IP).</param>
                        /// <returns></returns>
                        public async Task<Result> Glusterfsscan(string server)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("server", server);
                            return await _client.Get($"/nodes/{_node}/scan/glusterfs", parameters);
                        }
                    }
                    /// <summary>
                    /// Iscsi
                    /// </summary>
                    public class PveIscsi
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveIscsi(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan remote iSCSI server.
                        /// </summary>
                        /// <param name="portal">The iSCSI portal (IP or DNS name with optional port).</param>
                        /// <returns></returns>
                        public async Task<Result> Iscsiscan(string portal)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("portal", portal);
                            return await _client.Get($"/nodes/{_node}/scan/iscsi", parameters);
                        }
                    }
                    /// <summary>
                    /// Lvm
                    /// </summary>
                    public class PveLvm
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveLvm(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local LVM volume groups.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Lvmscan() { return await _client.Get($"/nodes/{_node}/scan/lvm"); }
                    }
                    /// <summary>
                    /// Lvmthin
                    /// </summary>
                    public class PveLvmthin
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveLvmthin(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local LVM Thin Pools.
                        /// </summary>
                        /// <param name="vg"></param>
                        /// <returns></returns>
                        public async Task<Result> Lvmthinscan(string vg)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("vg", vg);
                            return await _client.Get($"/nodes/{_node}/scan/lvmthin", parameters);
                        }
                    }
                    /// <summary>
                    /// Zfs
                    /// </summary>
                    public class PveZfs
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveZfs(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Scan zfs pool list on local node.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Zfsscan() { return await _client.Get($"/nodes/{_node}/scan/zfs"); }
                    }
                    /// <summary>
                    /// Index of available scan methods
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/scan"); }
                }
                /// <summary>
                /// Hardware
                /// </summary>
                public class PveHardware
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveHardware(PveClient client, object node) { _client = client; _node = node; }
                    private PvePci _pci;
                    /// <summary>
                    /// Pci
                    /// </summary>
                    public PvePci Pci => _pci ??= new(_client, _node);
                    private PveUsb _usb;
                    /// <summary>
                    /// Usb
                    /// </summary>
                    public PveUsb Usb => _usb ??= new(_client, _node);
                    /// <summary>
                    /// Pci
                    /// </summary>
                    public class PvePci
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PvePci(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// PciidItem
                        /// </summary>
                        public PvePciidItem this[object pciid] => new(_client, _node, pciid);
                        /// <summary>
                        /// PciidItem
                        /// </summary>
                        public class PvePciidItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _pciid;
                            internal PvePciidItem(PveClient client, object node, object pciid)
                            {
                                _client = client; _node = node;
                                _pciid = pciid;
                            }
                            private PveMdev _mdev;
                            /// <summary>
                            /// Mdev
                            /// </summary>
                            public PveMdev Mdev => _mdev ??= new(_client, _node, _pciid);
                            /// <summary>
                            /// Mdev
                            /// </summary>
                            public class PveMdev
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _pciid;
                                internal PveMdev(PveClient client, object node, object pciid)
                                {
                                    _client = client; _node = node;
                                    _pciid = pciid;
                                }
                                /// <summary>
                                /// List mediated device types for given PCI device.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Mdevscan() { return await _client.Get($"/nodes/{_node}/hardware/pci/{_pciid}/mdev"); }
                            }
                            /// <summary>
                            /// Index of available pci methods
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Pciindex() { return await _client.Get($"/nodes/{_node}/hardware/pci/{_pciid}"); }
                        }
                        /// <summary>
                        /// List local PCI devices.
                        /// </summary>
                        /// <param name="pci_class_blacklist">A list of blacklisted PCI classes, which will not be returned. Following are filtered by default: Memory Controller (05), Bridge (06) and Processor (0b).</param>
                        /// <param name="verbose">If disabled, does only print the PCI IDs. Otherwise, additional information like vendor and device will be returned.</param>
                        /// <returns></returns>
                        public async Task<Result> Pciscan(string pci_class_blacklist = null, bool? verbose = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("pci-class-blacklist", pci_class_blacklist);
                            parameters.Add("verbose", verbose);
                            return await _client.Get($"/nodes/{_node}/hardware/pci", parameters);
                        }
                    }
                    /// <summary>
                    /// Usb
                    /// </summary>
                    public class PveUsb
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveUsb(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local USB devices.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Usbscan() { return await _client.Get($"/nodes/{_node}/hardware/usb"); }
                    }
                    /// <summary>
                    /// Index of hardware types
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/hardware"); }
                }
                /// <summary>
                /// Capabilities
                /// </summary>
                public class PveCapabilities
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveCapabilities(PveClient client, object node) { _client = client; _node = node; }
                    private PveQemu _qemu;
                    /// <summary>
                    /// Qemu
                    /// </summary>
                    public PveQemu Qemu => _qemu ??= new(_client, _node);
                    /// <summary>
                    /// Qemu
                    /// </summary>
                    public class PveQemu
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveQemu(PveClient client, object node) { _client = client; _node = node; }
                        private PveCpu _cpu;
                        /// <summary>
                        /// Cpu
                        /// </summary>
                        public PveCpu Cpu => _cpu ??= new(_client, _node);
                        private PveMachines _machines;
                        /// <summary>
                        /// Machines
                        /// </summary>
                        public PveMachines Machines => _machines ??= new(_client, _node);
                        /// <summary>
                        /// Cpu
                        /// </summary>
                        public class PveCpu
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            internal PveCpu(PveClient client, object node) { _client = client; _node = node; }
                            /// <summary>
                            /// List all custom and default CPU models.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/capabilities/qemu/cpu"); }
                        }
                        /// <summary>
                        /// Machines
                        /// </summary>
                        public class PveMachines
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            internal PveMachines(PveClient client, object node) { _client = client; _node = node; }
                            /// <summary>
                            /// Get available QEMU/KVM machine types.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Types() { return await _client.Get($"/nodes/{_node}/capabilities/qemu/machines"); }
                        }
                        /// <summary>
                        /// QEMU capabilities index.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> QemuCapsIndex() { return await _client.Get($"/nodes/{_node}/capabilities/qemu"); }
                    }
                    /// <summary>
                    /// Node capabilities index.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/capabilities"); }
                }
                /// <summary>
                /// Storage
                /// </summary>
                public class PveStorage
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveStorage(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// StorageItem
                    /// </summary>
                    public PveStorageItem this[object storage] => new(_client, _node, storage);
                    /// <summary>
                    /// StorageItem
                    /// </summary>
                    public class PveStorageItem
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        private readonly object _storage;
                        internal PveStorageItem(PveClient client, object node, object storage)
                        {
                            _client = client; _node = node;
                            _storage = storage;
                        }
                        private PvePrunebackups _prunebackups;
                        /// <summary>
                        /// Prunebackups
                        /// </summary>
                        public PvePrunebackups Prunebackups => _prunebackups ??= new(_client, _node, _storage);
                        private PveContent _content;
                        /// <summary>
                        /// Content
                        /// </summary>
                        public PveContent Content => _content ??= new(_client, _node, _storage);
                        private PveFileRestore _fileRestore;
                        /// <summary>
                        /// FileRestore
                        /// </summary>
                        public PveFileRestore FileRestore => _fileRestore ??= new(_client, _node, _storage);
                        private PveStatus _status;
                        /// <summary>
                        /// Status
                        /// </summary>
                        public PveStatus Status => _status ??= new(_client, _node, _storage);
                        private PveRrd _rrd;
                        /// <summary>
                        /// Rrd
                        /// </summary>
                        public PveRrd Rrd => _rrd ??= new(_client, _node, _storage);
                        private PveRrddata _rrddata;
                        /// <summary>
                        /// Rrddata
                        /// </summary>
                        public PveRrddata Rrddata => _rrddata ??= new(_client, _node, _storage);
                        private PveUpload _upload;
                        /// <summary>
                        /// Upload
                        /// </summary>
                        public PveUpload Upload => _upload ??= new(_client, _node, _storage);
                        private PveDownloadUrl _downloadUrl;
                        /// <summary>
                        /// DownloadUrl
                        /// </summary>
                        public PveDownloadUrl DownloadUrl => _downloadUrl ??= new(_client, _node, _storage);
                        /// <summary>
                        /// Prunebackups
                        /// </summary>
                        public class PvePrunebackups
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PvePrunebackups(PveClient client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// Prune backups. Only those using the standard naming scheme are considered.
                            /// </summary>
                            /// <param name="prune_backups">Use these retention options instead of those from the storage configuration.</param>
                            /// <param name="type">Either 'qemu' or 'lxc'. Only consider backups for guests of this type.
                            ///   Enum: qemu,lxc</param>
                            /// <param name="vmid">Only prune backups for this VM.</param>
                            /// <returns></returns>
                            public async Task<Result> Delete(string prune_backups = null, string type = null, int? vmid = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("prune-backups", prune_backups);
                                parameters.Add("type", type);
                                parameters.Add("vmid", vmid);
                                return await _client.Delete($"/nodes/{_node}/storage/{_storage}/prunebackups", parameters);
                            }
                            /// <summary>
                            /// Get prune information for backups. NOTE: this is only a preview and might not be what a subsequent prune call does if backups are removed/added in the meantime.
                            /// </summary>
                            /// <param name="prune_backups">Use these retention options instead of those from the storage configuration.</param>
                            /// <param name="type">Either 'qemu' or 'lxc'. Only consider backups for guests of this type.
                            ///   Enum: qemu,lxc</param>
                            /// <param name="vmid">Only consider backups for this guest.</param>
                            /// <returns></returns>
                            public async Task<Result> Dryrun(string prune_backups = null, string type = null, int? vmid = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("prune-backups", prune_backups);
                                parameters.Add("type", type);
                                parameters.Add("vmid", vmid);
                                return await _client.Get($"/nodes/{_node}/storage/{_storage}/prunebackups", parameters);
                            }
                        }
                        /// <summary>
                        /// Content
                        /// </summary>
                        public class PveContent
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PveContent(PveClient client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// VolumeItem
                            /// </summary>
                            public PveVolumeItem this[object volume] => new(_client, _node, _storage, volume);
                            /// <summary>
                            /// VolumeItem
                            /// </summary>
                            public class PveVolumeItem
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _storage;
                                private readonly object _volume;
                                internal PveVolumeItem(PveClient client, object node, object storage, object volume)
                                {
                                    _client = client; _node = node;
                                    _storage = storage;
                                    _volume = volume;
                                }
                                /// <summary>
                                /// Delete volume
                                /// </summary>
                                /// <param name="delay">Time to wait for the task to finish. We return 'null' if the task finish within that time.</param>
                                /// <returns></returns>
                                public async Task<Result> Delete(int? delay = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("delay", delay);
                                    return await _client.Delete($"/nodes/{_node}/storage/{_storage}/content/{_volume}", parameters);
                                }
                                /// <summary>
                                /// Get volume attributes
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Info() { return await _client.Get($"/nodes/{_node}/storage/{_storage}/content/{_volume}"); }
                                /// <summary>
                                /// Copy a volume. This is experimental code - do not use.
                                /// </summary>
                                /// <param name="target">Target volume identifier</param>
                                /// <param name="target_node">Target node. Default is local node.</param>
                                /// <returns></returns>
                                public async Task<Result> Copy(string target, string target_node = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("target", target);
                                    parameters.Add("target_node", target_node);
                                    return await _client.Create($"/nodes/{_node}/storage/{_storage}/content/{_volume}", parameters);
                                }
                                /// <summary>
                                /// Update volume attributes
                                /// </summary>
                                /// <param name="notes">The new notes.</param>
                                /// <param name="protected_">Protection status. Currently only supported for backups.</param>
                                /// <returns></returns>
                                public async Task<Result> Updateattributes(string notes = null, bool? protected_ = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("notes", notes);
                                    parameters.Add("protected", protected_);
                                    return await _client.Set($"/nodes/{_node}/storage/{_storage}/content/{_volume}", parameters);
                                }
                            }
                            /// <summary>
                            /// List storage content.
                            /// </summary>
                            /// <param name="content">Only list content of this type.</param>
                            /// <param name="vmid">Only list images for this VM</param>
                            /// <returns></returns>
                            public async Task<Result> Index(string content = null, int? vmid = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("content", content);
                                parameters.Add("vmid", vmid);
                                return await _client.Get($"/nodes/{_node}/storage/{_storage}/content", parameters);
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
                            public async Task<Result> Create(string filename, string size, int vmid, string format = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("filename", filename);
                                parameters.Add("size", size);
                                parameters.Add("vmid", vmid);
                                parameters.Add("format", format);
                                return await _client.Create($"/nodes/{_node}/storage/{_storage}/content", parameters);
                            }
                        }
                        /// <summary>
                        /// FileRestore
                        /// </summary>
                        public class PveFileRestore
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PveFileRestore(PveClient client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            private PveList _list;
                            /// <summary>
                            /// List
                            /// </summary>
                            public PveList List => _list ??= new(_client, _node, _storage);
                            private PveDownload _download;
                            /// <summary>
                            /// Download
                            /// </summary>
                            public PveDownload Download => _download ??= new(_client, _node, _storage);
                            /// <summary>
                            /// List
                            /// </summary>
                            public class PveList
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _storage;
                                internal PveList(PveClient client, object node, object storage)
                                {
                                    _client = client; _node = node;
                                    _storage = storage;
                                }
                                /// <summary>
                                /// List files and directories for single file restore under the given path.
                                /// </summary>
                                /// <param name="filepath">base64-path to the directory or file being listed, or "/".</param>
                                /// <param name="volume">Backup volume ID or name. Currently only PBS snapshots are supported.</param>
                                /// <returns></returns>
                                public async Task<Result> List(string filepath, string volume)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("filepath", filepath);
                                    parameters.Add("volume", volume);
                                    return await _client.Get($"/nodes/{_node}/storage/{_storage}/file-restore/list", parameters);
                                }
                            }
                            /// <summary>
                            /// Download
                            /// </summary>
                            public class PveDownload
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _storage;
                                internal PveDownload(PveClient client, object node, object storage)
                                {
                                    _client = client; _node = node;
                                    _storage = storage;
                                }
                                /// <summary>
                                /// Extract a file or directory (as zip archive) from a PBS backup.
                                /// </summary>
                                /// <param name="filepath">base64-path to the directory or file to download.</param>
                                /// <param name="volume">Backup volume ID or name. Currently only PBS snapshots are supported.</param>
                                /// <param name="tar">Download dirs as 'tar.zst' instead of 'zip'.</param>
                                /// <returns></returns>
                                public async Task<Result> Download(string filepath, string volume, bool? tar = null)
                                {
                                    var parameters = new Dictionary<string, object>();
                                    parameters.Add("filepath", filepath);
                                    parameters.Add("volume", volume);
                                    parameters.Add("tar", tar);
                                    return await _client.Get($"/nodes/{_node}/storage/{_storage}/file-restore/download", parameters);
                                }
                            }
                        }
                        /// <summary>
                        /// Status
                        /// </summary>
                        public class PveStatus
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PveStatus(PveClient client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// Read storage status.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ReadStatus() { return await _client.Get($"/nodes/{_node}/storage/{_storage}/status"); }
                        }
                        /// <summary>
                        /// Rrd
                        /// </summary>
                        public class PveRrd
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PveRrd(PveClient client, object node, object storage)
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
                            public async Task<Result> Rrd(string ds, string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("ds", ds);
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return await _client.Get($"/nodes/{_node}/storage/{_storage}/rrd", parameters);
                            }
                        }
                        /// <summary>
                        /// Rrddata
                        /// </summary>
                        public class PveRrddata
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PveRrddata(PveClient client, object node, object storage)
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
                            public async Task<Result> Rrddata(string timeframe, string cf = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("timeframe", timeframe);
                                parameters.Add("cf", cf);
                                return await _client.Get($"/nodes/{_node}/storage/{_storage}/rrddata", parameters);
                            }
                        }
                        /// <summary>
                        /// Upload
                        /// </summary>
                        public class PveUpload
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PveUpload(PveClient client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// Upload templates and ISO images.
                            /// </summary>
                            /// <param name="content">Content type.
                            ///   Enum: iso,vztmpl</param>
                            /// <param name="filename">The name of the file to create. Caution: This will be normalized!</param>
                            /// <param name="checksum">The expected checksum of the file.</param>
                            /// <param name="checksum_algorithm">The algorithm to calculate the checksum of the file.
                            ///   Enum: md5,sha1,sha224,sha256,sha384,sha512</param>
                            /// <param name="tmpfilename">The source file name. This parameter is usually set by the REST handler. You can only overwrite it when connecting to the trusted port on localhost.</param>
                            /// <returns></returns>
                            public async Task<Result> Upload(string content, string filename, string checksum = null, string checksum_algorithm = null, string tmpfilename = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("content", content);
                                parameters.Add("filename", filename);
                                parameters.Add("checksum", checksum);
                                parameters.Add("checksum-algorithm", checksum_algorithm);
                                parameters.Add("tmpfilename", tmpfilename);
                                return await _client.Create($"/nodes/{_node}/storage/{_storage}/upload", parameters);
                            }
                        }
                        /// <summary>
                        /// DownloadUrl
                        /// </summary>
                        public class PveDownloadUrl
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _storage;
                            internal PveDownloadUrl(PveClient client, object node, object storage)
                            {
                                _client = client; _node = node;
                                _storage = storage;
                            }
                            /// <summary>
                            /// Download templates and ISO images by using an URL.
                            /// </summary>
                            /// <param name="content">Content type.
                            ///   Enum: iso,vztmpl</param>
                            /// <param name="filename">The name of the file to create. Caution: This will be normalized!</param>
                            /// <param name="url">The URL to download the file from.</param>
                            /// <param name="checksum">The expected checksum of the file.</param>
                            /// <param name="checksum_algorithm">The algorithm to calculate the checksum of the file.
                            ///   Enum: md5,sha1,sha224,sha256,sha384,sha512</param>
                            /// <param name="compression">Decompress the downloaded file using the specified compression algorithm.</param>
                            /// <param name="verify_certificates">If false, no SSL/TLS certificates will be verified.</param>
                            /// <returns></returns>
                            public async Task<Result> DownloadUrl(string content, string filename, string url, string checksum = null, string checksum_algorithm = null, string compression = null, bool? verify_certificates = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("content", content);
                                parameters.Add("filename", filename);
                                parameters.Add("url", url);
                                parameters.Add("checksum", checksum);
                                parameters.Add("checksum-algorithm", checksum_algorithm);
                                parameters.Add("compression", compression);
                                parameters.Add("verify-certificates", verify_certificates);
                                return await _client.Create($"/nodes/{_node}/storage/{_storage}/download-url", parameters);
                            }
                        }
                        /// <summary>
                        ///
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Diridx() { return await _client.Get($"/nodes/{_node}/storage/{_storage}"); }
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
                    public async Task<Result> Index(string content = null, bool? enabled = null, bool? format = null, string storage = null, string target = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("content", content);
                        parameters.Add("enabled", enabled);
                        parameters.Add("format", format);
                        parameters.Add("storage", storage);
                        parameters.Add("target", target);
                        return await _client.Get($"/nodes/{_node}/storage", parameters);
                    }
                }
                /// <summary>
                /// Disks
                /// </summary>
                public class PveDisks
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveDisks(PveClient client, object node) { _client = client; _node = node; }
                    private PveLvm _lvm;
                    /// <summary>
                    /// Lvm
                    /// </summary>
                    public PveLvm Lvm => _lvm ??= new(_client, _node);
                    private PveLvmthin _lvmthin;
                    /// <summary>
                    /// Lvmthin
                    /// </summary>
                    public PveLvmthin Lvmthin => _lvmthin ??= new(_client, _node);
                    private PveDirectory _directory;
                    /// <summary>
                    /// Directory
                    /// </summary>
                    public PveDirectory Directory => _directory ??= new(_client, _node);
                    private PveZfs _zfs;
                    /// <summary>
                    /// Zfs
                    /// </summary>
                    public PveZfs Zfs => _zfs ??= new(_client, _node);
                    private PveList _list;
                    /// <summary>
                    /// List
                    /// </summary>
                    public PveList List => _list ??= new(_client, _node);
                    private PveSmart _smart;
                    /// <summary>
                    /// Smart
                    /// </summary>
                    public PveSmart Smart => _smart ??= new(_client, _node);
                    private PveInitgpt _initgpt;
                    /// <summary>
                    /// Initgpt
                    /// </summary>
                    public PveInitgpt Initgpt => _initgpt ??= new(_client, _node);
                    private PveWipedisk _wipedisk;
                    /// <summary>
                    /// Wipedisk
                    /// </summary>
                    public PveWipedisk Wipedisk => _wipedisk ??= new(_client, _node);
                    /// <summary>
                    /// Lvm
                    /// </summary>
                    public class PveLvm
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveLvm(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, _node, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Remove an LVM Volume Group.
                            /// </summary>
                            /// <param name="cleanup_config">Marks associated storage(s) as not available on this node anymore or removes them from the configuration (if configured for this node only).</param>
                            /// <param name="cleanup_disks">Also wipe disks so they can be repurposed afterwards.</param>
                            /// <returns></returns>
                            public async Task<Result> Delete(bool? cleanup_config = null, bool? cleanup_disks = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cleanup-config", cleanup_config);
                                parameters.Add("cleanup-disks", cleanup_disks);
                                return await _client.Delete($"/nodes/{_node}/disks/lvm/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// List LVM Volume Groups
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/disks/lvm"); }
                        /// <summary>
                        /// Create an LVM Volume Group
                        /// </summary>
                        /// <param name="device">The block device you want to create the volume group on</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the Volume Group</param>
                        /// <returns></returns>
                        public async Task<Result> Create(string device, string name, bool? add_storage = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("device", device);
                            parameters.Add("name", name);
                            parameters.Add("add_storage", add_storage);
                            return await _client.Create($"/nodes/{_node}/disks/lvm", parameters);
                        }
                    }
                    /// <summary>
                    /// Lvmthin
                    /// </summary>
                    public class PveLvmthin
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveLvmthin(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, _node, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Remove an LVM thin pool.
                            /// </summary>
                            /// <param name="volume_group">The storage identifier.</param>
                            /// <param name="cleanup_config">Marks associated storage(s) as not available on this node anymore or removes them from the configuration (if configured for this node only).</param>
                            /// <param name="cleanup_disks">Also wipe disks so they can be repurposed afterwards.</param>
                            /// <returns></returns>
                            public async Task<Result> Delete(string volume_group, bool? cleanup_config = null, bool? cleanup_disks = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("volume-group", volume_group);
                                parameters.Add("cleanup-config", cleanup_config);
                                parameters.Add("cleanup-disks", cleanup_disks);
                                return await _client.Delete($"/nodes/{_node}/disks/lvmthin/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// List LVM thinpools
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/disks/lvmthin"); }
                        /// <summary>
                        /// Create an LVM thinpool
                        /// </summary>
                        /// <param name="device">The block device you want to create the thinpool on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the thinpool.</param>
                        /// <returns></returns>
                        public async Task<Result> Create(string device, string name, bool? add_storage = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("device", device);
                            parameters.Add("name", name);
                            parameters.Add("add_storage", add_storage);
                            return await _client.Create($"/nodes/{_node}/disks/lvmthin", parameters);
                        }
                    }
                    /// <summary>
                    /// Directory
                    /// </summary>
                    public class PveDirectory
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveDirectory(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, _node, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Unmounts the storage and removes the mount unit.
                            /// </summary>
                            /// <param name="cleanup_config">Marks associated storage(s) as not available on this node anymore or removes them from the configuration (if configured for this node only).</param>
                            /// <param name="cleanup_disks">Also wipe disk so it can be repurposed afterwards.</param>
                            /// <returns></returns>
                            public async Task<Result> Delete(bool? cleanup_config = null, bool? cleanup_disks = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cleanup-config", cleanup_config);
                                parameters.Add("cleanup-disks", cleanup_disks);
                                return await _client.Delete($"/nodes/{_node}/disks/directory/{_name}", parameters);
                            }
                        }
                        /// <summary>
                        /// PVE Managed Directory storages.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/disks/directory"); }
                        /// <summary>
                        /// Create a Filesystem on an unused disk. Will be mounted under '/mnt/pve/NAME'.
                        /// </summary>
                        /// <param name="device">The block device you want to create the filesystem on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="add_storage">Configure storage using the directory.</param>
                        /// <param name="filesystem">The desired filesystem.
                        ///   Enum: ext4,xfs</param>
                        /// <returns></returns>
                        public async Task<Result> Create(string device, string name, bool? add_storage = null, string filesystem = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("device", device);
                            parameters.Add("name", name);
                            parameters.Add("add_storage", add_storage);
                            parameters.Add("filesystem", filesystem);
                            return await _client.Create($"/nodes/{_node}/disks/directory", parameters);
                        }
                    }
                    /// <summary>
                    /// Zfs
                    /// </summary>
                    public class PveZfs
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveZfs(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public PveNameItem this[object name] => new(_client, _node, name);
                        /// <summary>
                        /// NameItem
                        /// </summary>
                        public class PveNameItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _name;
                            internal PveNameItem(PveClient client, object node, object name)
                            {
                                _client = client; _node = node;
                                _name = name;
                            }
                            /// <summary>
                            /// Destroy a ZFS pool.
                            /// </summary>
                            /// <param name="cleanup_config">Marks associated storage(s) as not available on this node anymore or removes them from the configuration (if configured for this node only).</param>
                            /// <param name="cleanup_disks">Also wipe disks so they can be repurposed afterwards.</param>
                            /// <returns></returns>
                            public async Task<Result> Delete(bool? cleanup_config = null, bool? cleanup_disks = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("cleanup-config", cleanup_config);
                                parameters.Add("cleanup-disks", cleanup_disks);
                                return await _client.Delete($"/nodes/{_node}/disks/zfs/{_name}", parameters);
                            }
                            /// <summary>
                            /// Get details about a zpool.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Detail() { return await _client.Get($"/nodes/{_node}/disks/zfs/{_name}"); }
                        }
                        /// <summary>
                        /// List Zpools.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/disks/zfs"); }
                        /// <summary>
                        /// Create a ZFS pool.
                        /// </summary>
                        /// <param name="devices">The block devices you want to create the zpool on.</param>
                        /// <param name="name">The storage identifier.</param>
                        /// <param name="raidlevel">The RAID level to use.
                        ///   Enum: single,mirror,raid10,raidz,raidz2,raidz3,draid,draid2,draid3</param>
                        /// <param name="add_storage">Configure storage using the zpool.</param>
                        /// <param name="ashift">Pool sector size exponent.</param>
                        /// <param name="compression">The compression algorithm to use.
                        ///   Enum: on,off,gzip,lz4,lzjb,zle,zstd</param>
                        /// <param name="draid_config"></param>
                        /// <returns></returns>
                        public async Task<Result> Create(string devices, string name, string raidlevel, bool? add_storage = null, int? ashift = null, string compression = null, string draid_config = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("devices", devices);
                            parameters.Add("name", name);
                            parameters.Add("raidlevel", raidlevel);
                            parameters.Add("add_storage", add_storage);
                            parameters.Add("ashift", ashift);
                            parameters.Add("compression", compression);
                            parameters.Add("draid-config", draid_config);
                            return await _client.Create($"/nodes/{_node}/disks/zfs", parameters);
                        }
                    }
                    /// <summary>
                    /// List
                    /// </summary>
                    public class PveList
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveList(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List local disks.
                        /// </summary>
                        /// <param name="include_partitions">Also include partitions.</param>
                        /// <param name="skipsmart">Skip smart checks.</param>
                        /// <param name="type">Only list specific types of disks.
                        ///   Enum: unused,journal_disks</param>
                        /// <returns></returns>
                        public async Task<Result> List(bool? include_partitions = null, bool? skipsmart = null, string type = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("include-partitions", include_partitions);
                            parameters.Add("skipsmart", skipsmart);
                            parameters.Add("type", type);
                            return await _client.Get($"/nodes/{_node}/disks/list", parameters);
                        }
                    }
                    /// <summary>
                    /// Smart
                    /// </summary>
                    public class PveSmart
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveSmart(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get SMART Health of a disk.
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <param name="healthonly">If true returns only the health status</param>
                        /// <returns></returns>
                        public async Task<Result> Smart(string disk, bool? healthonly = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("disk", disk);
                            parameters.Add("healthonly", healthonly);
                            return await _client.Get($"/nodes/{_node}/disks/smart", parameters);
                        }
                    }
                    /// <summary>
                    /// Initgpt
                    /// </summary>
                    public class PveInitgpt
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveInitgpt(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Initialize Disk with GPT
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <param name="uuid">UUID for the GPT table</param>
                        /// <returns></returns>
                        public async Task<Result> Initgpt(string disk, string uuid = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("disk", disk);
                            parameters.Add("uuid", uuid);
                            return await _client.Create($"/nodes/{_node}/disks/initgpt", parameters);
                        }
                    }
                    /// <summary>
                    /// Wipedisk
                    /// </summary>
                    public class PveWipedisk
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveWipedisk(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Wipe a disk or partition.
                        /// </summary>
                        /// <param name="disk">Block device name</param>
                        /// <returns></returns>
                        public async Task<Result> WipeDisk(string disk)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("disk", disk);
                            return await _client.Set($"/nodes/{_node}/disks/wipedisk", parameters);
                        }
                    }
                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/disks"); }
                }
                /// <summary>
                /// Apt
                /// </summary>
                public class PveApt
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveApt(PveClient client, object node) { _client = client; _node = node; }
                    private PveUpdate _update;
                    /// <summary>
                    /// Update
                    /// </summary>
                    public PveUpdate Update => _update ??= new(_client, _node);
                    private PveChangelog _changelog;
                    /// <summary>
                    /// Changelog
                    /// </summary>
                    public PveChangelog Changelog => _changelog ??= new(_client, _node);
                    private PveRepositories _repositories;
                    /// <summary>
                    /// Repositories
                    /// </summary>
                    public PveRepositories Repositories => _repositories ??= new(_client, _node);
                    private PveVersions _versions;
                    /// <summary>
                    /// Versions
                    /// </summary>
                    public PveVersions Versions => _versions ??= new(_client, _node);
                    /// <summary>
                    /// Update
                    /// </summary>
                    public class PveUpdate
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveUpdate(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// List available updates.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> ListUpdates() { return await _client.Get($"/nodes/{_node}/apt/update"); }
                        /// <summary>
                        /// This is used to resynchronize the package index files from their sources (apt-get update).
                        /// </summary>
                        /// <param name="notify">Send notification about new packages.</param>
                        /// <param name="quiet">Only produces output suitable for logging, omitting progress indicators.</param>
                        /// <returns></returns>
                        public async Task<Result> UpdateDatabase(bool? notify = null, bool? quiet = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("notify", notify);
                            parameters.Add("quiet", quiet);
                            return await _client.Create($"/nodes/{_node}/apt/update", parameters);
                        }
                    }
                    /// <summary>
                    /// Changelog
                    /// </summary>
                    public class PveChangelog
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveChangelog(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get package changelogs.
                        /// </summary>
                        /// <param name="name">Package name.</param>
                        /// <param name="version">Package version.</param>
                        /// <returns></returns>
                        public async Task<Result> Changelog(string name, string version = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("name", name);
                            parameters.Add("version", version);
                            return await _client.Get($"/nodes/{_node}/apt/changelog", parameters);
                        }
                    }
                    /// <summary>
                    /// Repositories
                    /// </summary>
                    public class PveRepositories
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveRepositories(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get APT repository information.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Repositories() { return await _client.Get($"/nodes/{_node}/apt/repositories"); }
                        /// <summary>
                        /// Change the properties of a repository. Currently only allows enabling/disabling.
                        /// </summary>
                        /// <param name="index">Index within the file (starting from 0).</param>
                        /// <param name="path">Path to the containing file.</param>
                        /// <param name="digest">Digest to detect modifications.</param>
                        /// <param name="enabled">Whether the repository should be enabled or not.</param>
                        /// <returns></returns>
                        public async Task<Result> ChangeRepository(int index, string path, string digest = null, bool? enabled = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("index", index);
                            parameters.Add("path", path);
                            parameters.Add("digest", digest);
                            parameters.Add("enabled", enabled);
                            return await _client.Create($"/nodes/{_node}/apt/repositories", parameters);
                        }
                        /// <summary>
                        /// Add a standard repository to the configuration
                        /// </summary>
                        /// <param name="handle">Handle that identifies a repository.</param>
                        /// <param name="digest">Digest to detect modifications.</param>
                        /// <returns></returns>
                        public async Task<Result> AddRepository(string handle, string digest = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("handle", handle);
                            parameters.Add("digest", digest);
                            return await _client.Set($"/nodes/{_node}/apt/repositories", parameters);
                        }
                    }
                    /// <summary>
                    /// Versions
                    /// </summary>
                    public class PveVersions
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveVersions(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get package information for important Proxmox packages.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Versions() { return await _client.Get($"/nodes/{_node}/apt/versions"); }
                    }
                    /// <summary>
                    /// Directory index for apt (Advanced Package Tool).
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/apt"); }
                }
                /// <summary>
                /// Firewall
                /// </summary>
                public class PveFirewall
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveFirewall(PveClient client, object node) { _client = client; _node = node; }
                    private PveRules _rules;
                    /// <summary>
                    /// Rules
                    /// </summary>
                    public PveRules Rules => _rules ??= new(_client, _node);
                    private PveOptions _options;
                    /// <summary>
                    /// Options
                    /// </summary>
                    public PveOptions Options => _options ??= new(_client, _node);
                    private PveLog _log;
                    /// <summary>
                    /// Log
                    /// </summary>
                    public PveLog Log => _log ??= new(_client, _node);
                    /// <summary>
                    /// Rules
                    /// </summary>
                    public class PveRules
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveRules(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// PosItem
                        /// </summary>
                        public PvePosItem this[object pos] => new(_client, _node, pos);
                        /// <summary>
                        /// PosItem
                        /// </summary>
                        public class PvePosItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _pos;
                            internal PvePosItem(PveClient client, object node, object pos)
                            {
                                _client = client; _node = node;
                                _pos = pos;
                            }
                            /// <summary>
                            /// Delete rule.
                            /// </summary>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <returns></returns>
                            public async Task<Result> DeleteRule(string digest = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("digest", digest);
                                return await _client.Delete($"/nodes/{_node}/firewall/rules/{_pos}", parameters);
                            }
                            /// <summary>
                            /// Get single rule data.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> GetRule() { return await _client.Get($"/nodes/{_node}/firewall/rules/{_pos}"); }
                            /// <summary>
                            /// Modify rule data.
                            /// </summary>
                            /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                            /// <param name="comment">Descriptive comment.</param>
                            /// <param name="delete">A list of settings you want to delete.</param>
                            /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                            /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                            /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                            /// <param name="enable">Flag to enable/disable a rule.</param>
                            /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
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
                            public async Task<Result> UpdateRule(string action = null, string comment = null, string delete = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? moveto = null, string proto = null, string source = null, string sport = null, string type = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("action", action);
                                parameters.Add("comment", comment);
                                parameters.Add("delete", delete);
                                parameters.Add("dest", dest);
                                parameters.Add("digest", digest);
                                parameters.Add("dport", dport);
                                parameters.Add("enable", enable);
                                parameters.Add("icmp-type", icmp_type);
                                parameters.Add("iface", iface);
                                parameters.Add("log", log);
                                parameters.Add("macro", macro);
                                parameters.Add("moveto", moveto);
                                parameters.Add("proto", proto);
                                parameters.Add("source", source);
                                parameters.Add("sport", sport);
                                parameters.Add("type", type);
                                return await _client.Set($"/nodes/{_node}/firewall/rules/{_pos}", parameters);
                            }
                        }
                        /// <summary>
                        /// List rules.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetRules() { return await _client.Get($"/nodes/{_node}/firewall/rules"); }
                        /// <summary>
                        /// Create new rule.
                        /// </summary>
                        /// <param name="action">Rule action ('ACCEPT', 'DROP', 'REJECT') or security group name.</param>
                        /// <param name="type">Rule type.
                        ///   Enum: in,out,group</param>
                        /// <param name="comment">Descriptive comment.</param>
                        /// <param name="dest">Restrict packet destination address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="dport">Restrict TCP/UDP destination port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <param name="enable">Flag to enable/disable a rule.</param>
                        /// <param name="icmp_type">Specify icmp-type. Only valid if proto equals 'icmp' or 'icmpv6'/'ipv6-icmp'.</param>
                        /// <param name="iface">Network interface name. You have to use network configuration key names for VMs and containers ('net\d+'). Host related rules can use arbitrary strings.</param>
                        /// <param name="log">Log level for firewall rule.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="macro">Use predefined standard macro.</param>
                        /// <param name="pos">Update rule at position &amp;lt;pos&amp;gt;.</param>
                        /// <param name="proto">IP protocol. You can use protocol names ('tcp'/'udp') or simple numbers, as defined in '/etc/protocols'.</param>
                        /// <param name="source">Restrict packet source address. This can refer to a single IP address, an IP set ('+ipsetname') or an IP alias definition. You can also specify an address range like '20.34.101.207-201.3.9.99', or a list of IP addresses and networks (entries are separated by comma). Please do not mix IPv4 and IPv6 addresses inside such lists.</param>
                        /// <param name="sport">Restrict TCP/UDP source port. You can use service names or simple numbers (0-65535), as defined in '/etc/services'. Port ranges can be specified with '\d+:\d+', for example '80:85', and you can use comma separated list to match several ports or ranges.</param>
                        /// <returns></returns>
                        public async Task<Result> CreateRule(string action, string type, string comment = null, string dest = null, string digest = null, string dport = null, int? enable = null, string icmp_type = null, string iface = null, string log = null, string macro = null, int? pos = null, string proto = null, string source = null, string sport = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("action", action);
                            parameters.Add("type", type);
                            parameters.Add("comment", comment);
                            parameters.Add("dest", dest);
                            parameters.Add("digest", digest);
                            parameters.Add("dport", dport);
                            parameters.Add("enable", enable);
                            parameters.Add("icmp-type", icmp_type);
                            parameters.Add("iface", iface);
                            parameters.Add("log", log);
                            parameters.Add("macro", macro);
                            parameters.Add("pos", pos);
                            parameters.Add("proto", proto);
                            parameters.Add("source", source);
                            parameters.Add("sport", sport);
                            return await _client.Create($"/nodes/{_node}/firewall/rules", parameters);
                        }
                    }
                    /// <summary>
                    /// Options
                    /// </summary>
                    public class PveOptions
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveOptions(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get host firewall options.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetOptions() { return await _client.Get($"/nodes/{_node}/firewall/options"); }
                        /// <summary>
                        /// Set Firewall options.
                        /// </summary>
                        /// <param name="delete">A list of settings you want to delete.</param>
                        /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                        /// <param name="enable">Enable host firewall rules.</param>
                        /// <param name="log_level_in">Log level for incoming traffic.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="log_level_out">Log level for outgoing traffic.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="log_nf_conntrack">Enable logging of conntrack information.</param>
                        /// <param name="ndp">Enable NDP (Neighbor Discovery Protocol).</param>
                        /// <param name="nf_conntrack_allow_invalid">Allow invalid packets on connection tracking.</param>
                        /// <param name="nf_conntrack_helpers">Enable conntrack helpers for specific protocols. Supported protocols: amanda, ftp, irc, netbios-ns, pptp, sane, sip, snmp, tftp</param>
                        /// <param name="nf_conntrack_max">Maximum number of tracked connections.</param>
                        /// <param name="nf_conntrack_tcp_timeout_established">Conntrack established timeout.</param>
                        /// <param name="nf_conntrack_tcp_timeout_syn_recv">Conntrack syn recv timeout.</param>
                        /// <param name="nosmurfs">Enable SMURFS filter.</param>
                        /// <param name="protection_synflood">Enable synflood protection</param>
                        /// <param name="protection_synflood_burst">Synflood protection rate burst by ip src.</param>
                        /// <param name="protection_synflood_rate">Synflood protection rate syn/sec by ip src.</param>
                        /// <param name="smurf_log_level">Log level for SMURFS filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcp_flags_log_level">Log level for illegal tcp flags filter.
                        ///   Enum: emerg,alert,crit,err,warning,notice,info,debug,nolog</param>
                        /// <param name="tcpflags">Filter illegal combinations of TCP flags.</param>
                        /// <returns></returns>
                        public async Task<Result> SetOptions(string delete = null, string digest = null, bool? enable = null, string log_level_in = null, string log_level_out = null, bool? log_nf_conntrack = null, bool? ndp = null, bool? nf_conntrack_allow_invalid = null, string nf_conntrack_helpers = null, int? nf_conntrack_max = null, int? nf_conntrack_tcp_timeout_established = null, int? nf_conntrack_tcp_timeout_syn_recv = null, bool? nosmurfs = null, bool? protection_synflood = null, int? protection_synflood_burst = null, int? protection_synflood_rate = null, string smurf_log_level = null, string tcp_flags_log_level = null, bool? tcpflags = null)
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
                            parameters.Add("nf_conntrack_helpers", nf_conntrack_helpers);
                            parameters.Add("nf_conntrack_max", nf_conntrack_max);
                            parameters.Add("nf_conntrack_tcp_timeout_established", nf_conntrack_tcp_timeout_established);
                            parameters.Add("nf_conntrack_tcp_timeout_syn_recv", nf_conntrack_tcp_timeout_syn_recv);
                            parameters.Add("nosmurfs", nosmurfs);
                            parameters.Add("protection_synflood", protection_synflood);
                            parameters.Add("protection_synflood_burst", protection_synflood_burst);
                            parameters.Add("protection_synflood_rate", protection_synflood_rate);
                            parameters.Add("smurf_log_level", smurf_log_level);
                            parameters.Add("tcp_flags_log_level", tcp_flags_log_level);
                            parameters.Add("tcpflags", tcpflags);
                            return await _client.Set($"/nodes/{_node}/firewall/options", parameters);
                        }
                    }
                    /// <summary>
                    /// Log
                    /// </summary>
                    public class PveLog
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveLog(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Read firewall log
                        /// </summary>
                        /// <param name="limit"></param>
                        /// <param name="since">Display log since this UNIX epoch.</param>
                        /// <param name="start"></param>
                        /// <param name="until">Display log until this UNIX epoch.</param>
                        /// <returns></returns>
                        public async Task<Result> Log(int? limit = null, int? since = null, int? start = null, int? until = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("limit", limit);
                            parameters.Add("since", since);
                            parameters.Add("start", start);
                            parameters.Add("until", until);
                            return await _client.Get($"/nodes/{_node}/firewall/log", parameters);
                        }
                    }
                    /// <summary>
                    /// Directory index.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/firewall"); }
                }
                /// <summary>
                /// Replication
                /// </summary>
                public class PveReplication
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveReplication(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public PveIdItem this[object id] => new(_client, _node, id);
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public class PveIdItem
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        private readonly object _id;
                        internal PveIdItem(PveClient client, object node, object id)
                        {
                            _client = client; _node = node;
                            _id = id;
                        }
                        private PveStatus _status;
                        /// <summary>
                        /// Status
                        /// </summary>
                        public PveStatus Status => _status ??= new(_client, _node, _id);
                        private PveLog _log;
                        /// <summary>
                        /// Log
                        /// </summary>
                        public PveLog Log => _log ??= new(_client, _node, _id);
                        private PveScheduleNow _scheduleNow;
                        /// <summary>
                        /// ScheduleNow
                        /// </summary>
                        public PveScheduleNow ScheduleNow => _scheduleNow ??= new(_client, _node, _id);
                        /// <summary>
                        /// Status
                        /// </summary>
                        public class PveStatus
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _id;
                            internal PveStatus(PveClient client, object node, object id)
                            {
                                _client = client; _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Get replication job status.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> JobStatus() { return await _client.Get($"/nodes/{_node}/replication/{_id}/status"); }
                        }
                        /// <summary>
                        /// Log
                        /// </summary>
                        public class PveLog
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _id;
                            internal PveLog(PveClient client, object node, object id)
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
                            public async Task<Result> ReadJobLog(int? limit = null, int? start = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("limit", limit);
                                parameters.Add("start", start);
                                return await _client.Get($"/nodes/{_node}/replication/{_id}/log", parameters);
                            }
                        }
                        /// <summary>
                        /// ScheduleNow
                        /// </summary>
                        public class PveScheduleNow
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _id;
                            internal PveScheduleNow(PveClient client, object node, object id)
                            {
                                _client = client; _node = node;
                                _id = id;
                            }
                            /// <summary>
                            /// Schedule replication job to start as soon as possible.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ScheduleNow() { return await _client.Create($"/nodes/{_node}/replication/{_id}/schedule_now"); }
                        }
                        /// <summary>
                        /// Directory index.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/replication/{_id}"); }
                    }
                    /// <summary>
                    /// List status of all replication jobs on this node.
                    /// </summary>
                    /// <param name="guest">Only list replication jobs for this guest.</param>
                    /// <returns></returns>
                    public async Task<Result> Status(int? guest = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("guest", guest);
                        return await _client.Get($"/nodes/{_node}/replication", parameters);
                    }
                }
                /// <summary>
                /// Certificates
                /// </summary>
                public class PveCertificates
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveCertificates(PveClient client, object node) { _client = client; _node = node; }
                    private PveAcme _acme;
                    /// <summary>
                    /// Acme
                    /// </summary>
                    public PveAcme Acme => _acme ??= new(_client, _node);
                    private PveInfo _info;
                    /// <summary>
                    /// Info
                    /// </summary>
                    public PveInfo Info => _info ??= new(_client, _node);
                    private PveCustom _custom;
                    /// <summary>
                    /// Custom
                    /// </summary>
                    public PveCustom Custom => _custom ??= new(_client, _node);
                    /// <summary>
                    /// Acme
                    /// </summary>
                    public class PveAcme
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveAcme(PveClient client, object node) { _client = client; _node = node; }
                        private PveCertificate _certificate;
                        /// <summary>
                        /// Certificate
                        /// </summary>
                        public PveCertificate Certificate => _certificate ??= new(_client, _node);
                        /// <summary>
                        /// Certificate
                        /// </summary>
                        public class PveCertificate
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            internal PveCertificate(PveClient client, object node) { _client = client; _node = node; }
                            /// <summary>
                            /// Revoke existing certificate from CA.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> RevokeCertificate() { return await _client.Delete($"/nodes/{_node}/certificates/acme/certificate"); }
                            /// <summary>
                            /// Order a new certificate from ACME-compatible CA.
                            /// </summary>
                            /// <param name="force">Overwrite existing custom certificate.</param>
                            /// <returns></returns>
                            public async Task<Result> NewCertificate(bool? force = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                return await _client.Create($"/nodes/{_node}/certificates/acme/certificate", parameters);
                            }
                            /// <summary>
                            /// Renew existing certificate from CA.
                            /// </summary>
                            /// <param name="force">Force renewal even if expiry is more than 30 days away.</param>
                            /// <returns></returns>
                            public async Task<Result> RenewCertificate(bool? force = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("force", force);
                                return await _client.Set($"/nodes/{_node}/certificates/acme/certificate", parameters);
                            }
                        }
                        /// <summary>
                        /// ACME index.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/certificates/acme"); }
                    }
                    /// <summary>
                    /// Info
                    /// </summary>
                    public class PveInfo
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveInfo(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// Get information about node's certificates.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Info() { return await _client.Get($"/nodes/{_node}/certificates/info"); }
                    }
                    /// <summary>
                    /// Custom
                    /// </summary>
                    public class PveCustom
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveCustom(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// DELETE custom certificate chain and key.
                        /// </summary>
                        /// <param name="restart">Restart pveproxy.</param>
                        /// <returns></returns>
                        public async Task<Result> RemoveCustomCert(bool? restart = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("restart", restart);
                            return await _client.Delete($"/nodes/{_node}/certificates/custom", parameters);
                        }
                        /// <summary>
                        /// Upload or update custom certificate chain and key.
                        /// </summary>
                        /// <param name="certificates">PEM encoded certificate (chain).</param>
                        /// <param name="force">Overwrite existing custom or ACME certificate files.</param>
                        /// <param name="key">PEM encoded private key.</param>
                        /// <param name="restart">Restart pveproxy.</param>
                        /// <returns></returns>
                        public async Task<Result> UploadCustomCert(string certificates, bool? force = null, string key = null, bool? restart = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("certificates", certificates);
                            parameters.Add("force", force);
                            parameters.Add("key", key);
                            parameters.Add("restart", restart);
                            return await _client.Create($"/nodes/{_node}/certificates/custom", parameters);
                        }
                    }
                    /// <summary>
                    /// Node index.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/certificates"); }
                }
                /// <summary>
                /// Config
                /// </summary>
                public class PveConfig
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveConfig(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Get node configuration options.
                    /// </summary>
                    /// <param name="property">Return only a specific property from the node configuration.
                    ///   Enum: acme,acmedomain0,acmedomain1,acmedomain2,acmedomain3,acmedomain4,acmedomain5,description,startall-onboot-delay,wakeonlan</param>
                    /// <returns></returns>
                    public async Task<Result> GetConfig(string property = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("property", property);
                        return await _client.Get($"/nodes/{_node}/config", parameters);
                    }
                    /// <summary>
                    /// Set node configuration options.
                    /// </summary>
                    /// <param name="acme">Node specific ACME settings.</param>
                    /// <param name="acmedomainN">ACME domain and validation plugin</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="description">Description for the Node. Shown in the web-interface node notes panel. This is saved as comment inside the configuration file.</param>
                    /// <param name="digest">Prevent changes if current configuration file has different SHA1 digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="startall_onboot_delay">Initial delay in seconds, before starting all the Virtual Guests with on-boot enabled.</param>
                    /// <param name="wakeonlan">MAC address for wake on LAN</param>
                    /// <returns></returns>
                    public async Task<Result> SetOptions(string acme = null, IDictionary<int, string> acmedomainN = null, string delete = null, string description = null, string digest = null, int? startall_onboot_delay = null, string wakeonlan = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("acme", acme);
                        parameters.Add("delete", delete);
                        parameters.Add("description", description);
                        parameters.Add("digest", digest);
                        parameters.Add("startall-onboot-delay", startall_onboot_delay);
                        parameters.Add("wakeonlan", wakeonlan);
                        AddIndexedParameter(parameters, "acmedomain", acmedomainN);
                        return await _client.Set($"/nodes/{_node}/config", parameters);
                    }
                }
                /// <summary>
                /// Sdn
                /// </summary>
                public class PveSdn
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveSdn(PveClient client, object node) { _client = client; _node = node; }
                    private PveZones _zones;
                    /// <summary>
                    /// Zones
                    /// </summary>
                    public PveZones Zones => _zones ??= new(_client, _node);
                    /// <summary>
                    /// Zones
                    /// </summary>
                    public class PveZones
                    {
                        private readonly PveClient _client;
                        private readonly object _node;
                        internal PveZones(PveClient client, object node) { _client = client; _node = node; }
                        /// <summary>
                        /// ZoneItem
                        /// </summary>
                        public PveZoneItem this[object zone] => new(_client, _node, zone);
                        /// <summary>
                        /// ZoneItem
                        /// </summary>
                        public class PveZoneItem
                        {
                            private readonly PveClient _client;
                            private readonly object _node;
                            private readonly object _zone;
                            internal PveZoneItem(PveClient client, object node, object zone)
                            {
                                _client = client; _node = node;
                                _zone = zone;
                            }
                            private PveContent _content;
                            /// <summary>
                            /// Content
                            /// </summary>
                            public PveContent Content => _content ??= new(_client, _node, _zone);
                            /// <summary>
                            /// Content
                            /// </summary>
                            public class PveContent
                            {
                                private readonly PveClient _client;
                                private readonly object _node;
                                private readonly object _zone;
                                internal PveContent(PveClient client, object node, object zone)
                                {
                                    _client = client; _node = node;
                                    _zone = zone;
                                }
                                /// <summary>
                                /// List zone content.
                                /// </summary>
                                /// <returns></returns>
                                public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/sdn/zones/{_zone}/content"); }
                            }
                            /// <summary>
                            ///
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> Diridx() { return await _client.Get($"/nodes/{_node}/sdn/zones/{_zone}"); }
                        }
                        /// <summary>
                        /// Get status for all zones.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}/sdn/zones"); }
                    }
                    /// <summary>
                    /// SDN index.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Sdnindex() { return await _client.Get($"/nodes/{_node}/sdn"); }
                }
                /// <summary>
                /// Version
                /// </summary>
                public class PveVersion
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveVersion(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// API version details
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Version() { return await _client.Get($"/nodes/{_node}/version"); }
                }
                /// <summary>
                /// Status
                /// </summary>
                public class PveStatus
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveStatus(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read node status
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Status() { return await _client.Get($"/nodes/{_node}/status"); }
                    /// <summary>
                    /// Reboot or shutdown a node.
                    /// </summary>
                    /// <param name="command">Specify the command.
                    ///   Enum: reboot,shutdown</param>
                    /// <returns></returns>
                    public async Task<Result> NodeCmd(string command)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("command", command);
                        return await _client.Create($"/nodes/{_node}/status", parameters);
                    }
                }
                /// <summary>
                /// Netstat
                /// </summary>
                public class PveNetstat
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveNetstat(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read tap/vm network device interface counters
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Netstat() { return await _client.Get($"/nodes/{_node}/netstat"); }
                }
                /// <summary>
                /// Execute
                /// </summary>
                public class PveExecute
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveExecute(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Execute multiple commands in order, root only.
                    /// </summary>
                    /// <param name="commands">JSON encoded array of commands.</param>
                    /// <returns></returns>
                    public async Task<Result> Execute(string commands)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("commands", commands);
                        return await _client.Create($"/nodes/{_node}/execute", parameters);
                    }
                }
                /// <summary>
                /// Wakeonlan
                /// </summary>
                public class PveWakeonlan
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveWakeonlan(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Try to wake a node via 'wake on LAN' network packet.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Wakeonlan() { return await _client.Create($"/nodes/{_node}/wakeonlan"); }
                }
                /// <summary>
                /// Rrd
                /// </summary>
                public class PveRrd
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveRrd(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read node RRD statistics (returns PNG)
                    /// </summary>
                    /// <param name="ds">The list of datasources you want to display.</param>
                    /// <param name="timeframe">Specify the time frame you are interested in.
                    ///   Enum: hour,day,week,month,year</param>
                    /// <param name="cf">The RRD consolidation function
                    ///   Enum: AVERAGE,MAX</param>
                    /// <returns></returns>
                    public async Task<Result> Rrd(string ds, string timeframe, string cf = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("ds", ds);
                        parameters.Add("timeframe", timeframe);
                        parameters.Add("cf", cf);
                        return await _client.Get($"/nodes/{_node}/rrd", parameters);
                    }
                }
                /// <summary>
                /// Rrddata
                /// </summary>
                public class PveRrddata
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveRrddata(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read node RRD statistics
                    /// </summary>
                    /// <param name="timeframe">Specify the time frame you are interested in.
                    ///   Enum: hour,day,week,month,year</param>
                    /// <param name="cf">The RRD consolidation function
                    ///   Enum: AVERAGE,MAX</param>
                    /// <returns></returns>
                    public async Task<Result> Rrddata(string timeframe, string cf = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("timeframe", timeframe);
                        parameters.Add("cf", cf);
                        return await _client.Get($"/nodes/{_node}/rrddata", parameters);
                    }
                }
                /// <summary>
                /// Syslog
                /// </summary>
                public class PveSyslog
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveSyslog(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read system log
                    /// </summary>
                    /// <param name="limit"></param>
                    /// <param name="service">Service ID</param>
                    /// <param name="since">Display all log since this date-time string.</param>
                    /// <param name="start"></param>
                    /// <param name="until">Display all log until this date-time string.</param>
                    /// <returns></returns>
                    public async Task<Result> Syslog(int? limit = null, string service = null, string since = null, int? start = null, string until = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("limit", limit);
                        parameters.Add("service", service);
                        parameters.Add("since", since);
                        parameters.Add("start", start);
                        parameters.Add("until", until);
                        return await _client.Get($"/nodes/{_node}/syslog", parameters);
                    }
                }
                /// <summary>
                /// Journal
                /// </summary>
                public class PveJournal
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveJournal(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read Journal
                    /// </summary>
                    /// <param name="endcursor">End before the given Cursor. Conflicts with 'until'</param>
                    /// <param name="lastentries">Limit to the last X lines. Conflicts with a range.</param>
                    /// <param name="since">Display all log since this UNIX epoch. Conflicts with 'startcursor'.</param>
                    /// <param name="startcursor">Start after the given Cursor. Conflicts with 'since'</param>
                    /// <param name="until">Display all log until this UNIX epoch. Conflicts with 'endcursor'.</param>
                    /// <returns></returns>
                    public async Task<Result> Journal(string endcursor = null, int? lastentries = null, int? since = null, string startcursor = null, int? until = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("endcursor", endcursor);
                        parameters.Add("lastentries", lastentries);
                        parameters.Add("since", since);
                        parameters.Add("startcursor", startcursor);
                        parameters.Add("until", until);
                        return await _client.Get($"/nodes/{_node}/journal", parameters);
                    }
                }
                /// <summary>
                /// Vncshell
                /// </summary>
                public class PveVncshell
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveVncshell(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login (requires 'root@pam')
                    ///   Enum: ceph_install,login,upgrade</param>
                    /// <param name="cmd_opts">Add parameters to a command. Encoded as null terminated strings.</param>
                    /// <param name="height">sets the height of the console in pixels.</param>
                    /// <param name="websocket">use websocket instead of standard vnc.</param>
                    /// <param name="width">sets the width of the console in pixels.</param>
                    /// <returns></returns>
                    public async Task<Result> Vncshell(string cmd = null, string cmd_opts = null, int? height = null, bool? websocket = null, int? width = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cmd", cmd);
                        parameters.Add("cmd-opts", cmd_opts);
                        parameters.Add("height", height);
                        parameters.Add("websocket", websocket);
                        parameters.Add("width", width);
                        return await _client.Create($"/nodes/{_node}/vncshell", parameters);
                    }
                }
                /// <summary>
                /// Termproxy
                /// </summary>
                public class PveTermproxy
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveTermproxy(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Creates a VNC Shell proxy.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login (requires 'root@pam')
                    ///   Enum: ceph_install,login,upgrade</param>
                    /// <param name="cmd_opts">Add parameters to a command. Encoded as null terminated strings.</param>
                    /// <returns></returns>
                    public async Task<Result> Termproxy(string cmd = null, string cmd_opts = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cmd", cmd);
                        parameters.Add("cmd-opts", cmd_opts);
                        return await _client.Create($"/nodes/{_node}/termproxy", parameters);
                    }
                }
                /// <summary>
                /// Vncwebsocket
                /// </summary>
                public class PveVncwebsocket
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveVncwebsocket(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Opens a websocket for VNC traffic.
                    /// </summary>
                    /// <param name="port">Port number returned by previous vncproxy call.</param>
                    /// <param name="vncticket">Ticket from previous call to vncproxy.</param>
                    /// <returns></returns>
                    public async Task<Result> Vncwebsocket(int port, string vncticket)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("port", port);
                        parameters.Add("vncticket", vncticket);
                        return await _client.Get($"/nodes/{_node}/vncwebsocket", parameters);
                    }
                }
                /// <summary>
                /// Spiceshell
                /// </summary>
                public class PveSpiceshell
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveSpiceshell(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Creates a SPICE shell.
                    /// </summary>
                    /// <param name="cmd">Run specific command or default to login (requires 'root@pam')
                    ///   Enum: ceph_install,login,upgrade</param>
                    /// <param name="cmd_opts">Add parameters to a command. Encoded as null terminated strings.</param>
                    /// <param name="proxy">SPICE proxy server. This can be used by the client to specify the proxy server. All nodes in a cluster runs 'spiceproxy', so it is up to the client to choose one. By default, we return the node where the VM is currently running. As reasonable setting is to use same node you use to connect to the API (This is window.location.hostname for the JS GUI).</param>
                    /// <returns></returns>
                    public async Task<Result> Spiceshell(string cmd = null, string cmd_opts = null, string proxy = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("cmd", cmd);
                        parameters.Add("cmd-opts", cmd_opts);
                        parameters.Add("proxy", proxy);
                        return await _client.Create($"/nodes/{_node}/spiceshell", parameters);
                    }
                }
                /// <summary>
                /// Dns
                /// </summary>
                public class PveDns
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveDns(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read DNS settings.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Dns() { return await _client.Get($"/nodes/{_node}/dns"); }
                    /// <summary>
                    /// Write DNS settings.
                    /// </summary>
                    /// <param name="search">Search domain for host-name lookup.</param>
                    /// <param name="dns1">First name server IP address.</param>
                    /// <param name="dns2">Second name server IP address.</param>
                    /// <param name="dns3">Third name server IP address.</param>
                    /// <returns></returns>
                    public async Task<Result> UpdateDns(string search, string dns1 = null, string dns2 = null, string dns3 = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("search", search);
                        parameters.Add("dns1", dns1);
                        parameters.Add("dns2", dns2);
                        parameters.Add("dns3", dns3);
                        return await _client.Set($"/nodes/{_node}/dns", parameters);
                    }
                }
                /// <summary>
                /// Time
                /// </summary>
                public class PveTime
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveTime(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Read server time and time zone settings.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Time() { return await _client.Get($"/nodes/{_node}/time"); }
                    /// <summary>
                    /// Set time zone.
                    /// </summary>
                    /// <param name="timezone">Time zone. The file '/usr/share/zoneinfo/zone.tab' contains the list of valid names.</param>
                    /// <returns></returns>
                    public async Task<Result> SetTimezone(string timezone)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("timezone", timezone);
                        return await _client.Set($"/nodes/{_node}/time", parameters);
                    }
                }
                /// <summary>
                /// Aplinfo
                /// </summary>
                public class PveAplinfo
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveAplinfo(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Get list of appliances.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Aplinfo() { return await _client.Get($"/nodes/{_node}/aplinfo"); }
                    /// <summary>
                    /// Download appliance templates.
                    /// </summary>
                    /// <param name="storage">The storage where the template will be stored</param>
                    /// <param name="template">The template which will downloaded</param>
                    /// <returns></returns>
                    public async Task<Result> AplDownload(string storage, string template)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("storage", storage);
                        parameters.Add("template", template);
                        return await _client.Create($"/nodes/{_node}/aplinfo", parameters);
                    }
                }
                /// <summary>
                /// QueryUrlMetadata
                /// </summary>
                public class PveQueryUrlMetadata
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveQueryUrlMetadata(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Query metadata of an URL: file size, file name and mime type.
                    /// </summary>
                    /// <param name="url">The URL to query the metadata from.</param>
                    /// <param name="verify_certificates">If false, no SSL/TLS certificates will be verified.</param>
                    /// <returns></returns>
                    public async Task<Result> QueryUrlMetadata(string url, bool? verify_certificates = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("url", url);
                        parameters.Add("verify-certificates", verify_certificates);
                        return await _client.Get($"/nodes/{_node}/query-url-metadata", parameters);
                    }
                }
                /// <summary>
                /// Report
                /// </summary>
                public class PveReport
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveReport(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Gather various systems information about a node
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Report() { return await _client.Get($"/nodes/{_node}/report"); }
                }
                /// <summary>
                /// Startall
                /// </summary>
                public class PveStartall
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveStartall(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Start all VMs and containers located on this node (by default only those with onboot=1).
                    /// </summary>
                    /// <param name="force">Issue start command even if virtual guest have 'onboot' not set or set to off.</param>
                    /// <param name="vms">Only consider guests from this comma separated list of VMIDs.</param>
                    /// <returns></returns>
                    public async Task<Result> Startall(bool? force = null, string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force", force);
                        parameters.Add("vms", vms);
                        return await _client.Create($"/nodes/{_node}/startall", parameters);
                    }
                }
                /// <summary>
                /// Stopall
                /// </summary>
                public class PveStopall
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveStopall(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Stop all VMs and Containers.
                    /// </summary>
                    /// <param name="force_stop">Force a hard-stop after the timeout.</param>
                    /// <param name="timeout">Timeout for each guest shutdown task. Depending on `force-stop`, the shutdown gets then simply aborted or a hard-stop is forced.</param>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <returns></returns>
                    public async Task<Result> Stopall(bool? force_stop = null, int? timeout = null, string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("force-stop", force_stop);
                        parameters.Add("timeout", timeout);
                        parameters.Add("vms", vms);
                        return await _client.Create($"/nodes/{_node}/stopall", parameters);
                    }
                }
                /// <summary>
                /// Suspendall
                /// </summary>
                public class PveSuspendall
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveSuspendall(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Suspend all VMs.
                    /// </summary>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <returns></returns>
                    public async Task<Result> Suspendall(string vms = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("vms", vms);
                        return await _client.Create($"/nodes/{_node}/suspendall", parameters);
                    }
                }
                /// <summary>
                /// Migrateall
                /// </summary>
                public class PveMigrateall
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveMigrateall(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Migrate all VMs and Containers.
                    /// </summary>
                    /// <param name="target">Target node.</param>
                    /// <param name="maxworkers">Maximal number of parallel migration job. If not set, uses'max_workers' from datacenter.cfg. One of both must be set!</param>
                    /// <param name="vms">Only consider Guests with these IDs.</param>
                    /// <param name="with_local_disks">Enable live storage migration for local disk</param>
                    /// <returns></returns>
                    public async Task<Result> Migrateall(string target, int? maxworkers = null, string vms = null, bool? with_local_disks = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("target", target);
                        parameters.Add("maxworkers", maxworkers);
                        parameters.Add("vms", vms);
                        parameters.Add("with-local-disks", with_local_disks);
                        return await _client.Create($"/nodes/{_node}/migrateall", parameters);
                    }
                }
                /// <summary>
                /// Hosts
                /// </summary>
                public class PveHosts
                {
                    private readonly PveClient _client;
                    private readonly object _node;
                    internal PveHosts(PveClient client, object node) { _client = client; _node = node; }
                    /// <summary>
                    /// Get the content of /etc/hosts.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> GetEtcHosts() { return await _client.Get($"/nodes/{_node}/hosts"); }
                    /// <summary>
                    /// Write /etc/hosts.
                    /// </summary>
                    /// <param name="data">The target content of /etc/hosts.</param>
                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                    /// <returns></returns>
                    public async Task<Result> WriteEtcHosts(string data, string digest = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("data", data);
                        parameters.Add("digest", digest);
                        return await _client.Create($"/nodes/{_node}/hosts", parameters);
                    }
                }
                /// <summary>
                /// Node index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/nodes/{_node}"); }
            }
            /// <summary>
            /// Cluster node index.
            /// </summary>
            /// <returns></returns>
            public async Task<Result> Index() { return await _client.Get($"/nodes"); }
        }
        /// <summary>
        /// Storage
        /// </summary>
        public class PveStorage
        {
            private readonly PveClient _client;

            internal PveStorage(PveClient client) { _client = client; }
            /// <summary>
            /// StorageItem
            /// </summary>
            public PveStorageItem this[object storage] => new(_client, storage);
            /// <summary>
            /// StorageItem
            /// </summary>
            public class PveStorageItem
            {
                private readonly PveClient _client;
                private readonly object _storage;
                internal PveStorageItem(PveClient client, object storage) { _client = client; _storage = storage; }
                /// <summary>
                /// Delete storage configuration.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Delete() { return await _client.Delete($"/storage/{_storage}"); }
                /// <summary>
                /// Read storage configuration.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Read() { return await _client.Get($"/storage/{_storage}"); }
                /// <summary>
                /// Update storage configuration.
                /// </summary>
                /// <param name="blocksize">block size</param>
                /// <param name="bwlimit">Set I/O bandwidth limit for various operations (in KiB/s).</param>
                /// <param name="comstar_hg">host group for comstar views</param>
                /// <param name="comstar_tg">target group for comstar views</param>
                /// <param name="content">Allowed content types.  NOTE: the value 'rootdir' is used for Containers, and value 'images' for VMs. </param>
                /// <param name="content_dirs">Overrides for default content type directories.</param>
                /// <param name="create_base_path">Create the base directory if it doesn't exist.</param>
                /// <param name="create_subdirs">Populate the directory with the default structure.</param>
                /// <param name="data_pool">Data Pool (for erasure coding only)</param>
                /// <param name="delete">A list of settings you want to delete.</param>
                /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                /// <param name="disable">Flag to disable the storage.</param>
                /// <param name="domain">CIFS domain.</param>
                /// <param name="encryption_key">Encryption key. Use 'autogen' to generate one automatically without passphrase.</param>
                /// <param name="fingerprint">Certificate SHA 256 fingerprint.</param>
                /// <param name="format">Default image format.</param>
                /// <param name="fs_name">The Ceph filesystem name.</param>
                /// <param name="fuse">Mount CephFS through FUSE.</param>
                /// <param name="is_mountpoint">Assume the given path is an externally managed mountpoint and consider the storage offline if it is not mounted. Using a boolean (yes/no) value serves as a shortcut to using the target path in this field.</param>
                /// <param name="keyring">Client keyring contents (for external clusters).</param>
                /// <param name="krbd">Always access rbd through krbd kernel module.</param>
                /// <param name="lio_tpg">target portal group for Linux LIO targets</param>
                /// <param name="master_pubkey">Base64-encoded, PEM-formatted public RSA key. Used to encrypt a copy of the encryption-key which will be added to each encrypted backup.</param>
                /// <param name="max_protected_backups">Maximal number of protected backups per guest. Use '-1' for unlimited.</param>
                /// <param name="maxfiles">Deprecated: use 'prune-backups' instead. Maximal number of backup files per VM. Use '0' for unlimited.</param>
                /// <param name="mkdir">Create the directory if it doesn't exist and populate it with default sub-dirs. NOTE: Deprecated, use the 'create-base-path' and 'create-subdirs' options instead.</param>
                /// <param name="monhost">IP addresses of monitors (for external clusters).</param>
                /// <param name="mountpoint">mount point</param>
                /// <param name="namespace_">Namespace.</param>
                /// <param name="nocow">Set the NOCOW flag on files. Disables data checksumming and causes data errors to be unrecoverable from while allowing direct I/O. Only use this if data does not need to be any more safe than on a single ext4 formatted disk with no underlying raid system.</param>
                /// <param name="nodes">List of cluster node names.</param>
                /// <param name="nowritecache">disable write caching on the target</param>
                /// <param name="options">NFS/CIFS mount options (see 'man nfs' or 'man mount.cifs')</param>
                /// <param name="password">Password for accessing the share/datastore.</param>
                /// <param name="pool">Pool.</param>
                /// <param name="port">For non default port.</param>
                /// <param name="preallocation">Preallocation mode for raw and qcow2 images. Using 'metadata' on raw images results in preallocation=off.
                ///   Enum: off,metadata,falloc,full</param>
                /// <param name="prune_backups">The retention options with shorter intervals are processed first with --keep-last being the very first one. Each option covers a specific period of time. We say that backups within this period are covered by this option. The next option does not take care of already covered backups and only considers older backups.</param>
                /// <param name="saferemove">Zero-out data when removing LVs.</param>
                /// <param name="saferemove_throughput">Wipe throughput (cstream -t parameter value).</param>
                /// <param name="server">Server IP or DNS name.</param>
                /// <param name="server2">Backup volfile server IP or DNS name.</param>
                /// <param name="shared">Mark storage as shared.</param>
                /// <param name="smbversion">SMB protocol version. 'default' if not set, negotiates the highest SMB2+ version supported by both the client and server.
                ///   Enum: default,2.0,2.1,3,3.0,3.11</param>
                /// <param name="sparse">use sparse volumes</param>
                /// <param name="subdir">Subdir to mount.</param>
                /// <param name="tagged_only">Only use logical volumes tagged with 'pve-vm-ID'.</param>
                /// <param name="transport">Gluster transport: tcp or rdma
                ///   Enum: tcp,rdma,unix</param>
                /// <param name="username">RBD Id.</param>
                /// <returns></returns>
                public async Task<Result> Update(string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, string content_dirs = null, bool? create_base_path = null, bool? create_subdirs = null, string data_pool = null, string delete = null, string digest = null, bool? disable = null, string domain = null, string encryption_key = null, string fingerprint = null, string format = null, string fs_name = null, bool? fuse = null, string is_mountpoint = null, string keyring = null, bool? krbd = null, string lio_tpg = null, string master_pubkey = null, int? max_protected_backups = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string mountpoint = null, string namespace_ = null, bool? nocow = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string pool = null, int? port = null, string preallocation = null, string prune_backups = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string transport = null, string username = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("blocksize", blocksize);
                    parameters.Add("bwlimit", bwlimit);
                    parameters.Add("comstar_hg", comstar_hg);
                    parameters.Add("comstar_tg", comstar_tg);
                    parameters.Add("content", content);
                    parameters.Add("content-dirs", content_dirs);
                    parameters.Add("create-base-path", create_base_path);
                    parameters.Add("create-subdirs", create_subdirs);
                    parameters.Add("data-pool", data_pool);
                    parameters.Add("delete", delete);
                    parameters.Add("digest", digest);
                    parameters.Add("disable", disable);
                    parameters.Add("domain", domain);
                    parameters.Add("encryption-key", encryption_key);
                    parameters.Add("fingerprint", fingerprint);
                    parameters.Add("format", format);
                    parameters.Add("fs-name", fs_name);
                    parameters.Add("fuse", fuse);
                    parameters.Add("is_mountpoint", is_mountpoint);
                    parameters.Add("keyring", keyring);
                    parameters.Add("krbd", krbd);
                    parameters.Add("lio_tpg", lio_tpg);
                    parameters.Add("master-pubkey", master_pubkey);
                    parameters.Add("max-protected-backups", max_protected_backups);
                    parameters.Add("maxfiles", maxfiles);
                    parameters.Add("mkdir", mkdir);
                    parameters.Add("monhost", monhost);
                    parameters.Add("mountpoint", mountpoint);
                    parameters.Add("namespace", namespace_);
                    parameters.Add("nocow", nocow);
                    parameters.Add("nodes", nodes);
                    parameters.Add("nowritecache", nowritecache);
                    parameters.Add("options", options);
                    parameters.Add("password", password);
                    parameters.Add("pool", pool);
                    parameters.Add("port", port);
                    parameters.Add("preallocation", preallocation);
                    parameters.Add("prune-backups", prune_backups);
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
                    return await _client.Set($"/storage/{_storage}", parameters);
                }
            }
            /// <summary>
            /// Storage index.
            /// </summary>
            /// <param name="type">Only list storage of specific type
            ///   Enum: btrfs,cephfs,cifs,dir,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,pbs,rbd,zfs,zfspool</param>
            /// <returns></returns>
            public async Task<Result> Index(string type = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("type", type);
                return await _client.Get($"/storage", parameters);
            }
            /// <summary>
            /// Create a new storage.
            /// </summary>
            /// <param name="storage">The storage identifier.</param>
            /// <param name="type">Storage type.
            ///   Enum: btrfs,cephfs,cifs,dir,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,pbs,rbd,zfs,zfspool</param>
            /// <param name="authsupported">Authsupported.</param>
            /// <param name="base_">Base volume. This volume is automatically activated.</param>
            /// <param name="blocksize">block size</param>
            /// <param name="bwlimit">Set I/O bandwidth limit for various operations (in KiB/s).</param>
            /// <param name="comstar_hg">host group for comstar views</param>
            /// <param name="comstar_tg">target group for comstar views</param>
            /// <param name="content">Allowed content types.  NOTE: the value 'rootdir' is used for Containers, and value 'images' for VMs. </param>
            /// <param name="content_dirs">Overrides for default content type directories.</param>
            /// <param name="create_base_path">Create the base directory if it doesn't exist.</param>
            /// <param name="create_subdirs">Populate the directory with the default structure.</param>
            /// <param name="data_pool">Data Pool (for erasure coding only)</param>
            /// <param name="datastore">Proxmox Backup Server datastore name.</param>
            /// <param name="disable">Flag to disable the storage.</param>
            /// <param name="domain">CIFS domain.</param>
            /// <param name="encryption_key">Encryption key. Use 'autogen' to generate one automatically without passphrase.</param>
            /// <param name="export">NFS export path.</param>
            /// <param name="fingerprint">Certificate SHA 256 fingerprint.</param>
            /// <param name="format">Default image format.</param>
            /// <param name="fs_name">The Ceph filesystem name.</param>
            /// <param name="fuse">Mount CephFS through FUSE.</param>
            /// <param name="is_mountpoint">Assume the given path is an externally managed mountpoint and consider the storage offline if it is not mounted. Using a boolean (yes/no) value serves as a shortcut to using the target path in this field.</param>
            /// <param name="iscsiprovider">iscsi provider</param>
            /// <param name="keyring">Client keyring contents (for external clusters).</param>
            /// <param name="krbd">Always access rbd through krbd kernel module.</param>
            /// <param name="lio_tpg">target portal group for Linux LIO targets</param>
            /// <param name="master_pubkey">Base64-encoded, PEM-formatted public RSA key. Used to encrypt a copy of the encryption-key which will be added to each encrypted backup.</param>
            /// <param name="max_protected_backups">Maximal number of protected backups per guest. Use '-1' for unlimited.</param>
            /// <param name="maxfiles">Deprecated: use 'prune-backups' instead. Maximal number of backup files per VM. Use '0' for unlimited.</param>
            /// <param name="mkdir">Create the directory if it doesn't exist and populate it with default sub-dirs. NOTE: Deprecated, use the 'create-base-path' and 'create-subdirs' options instead.</param>
            /// <param name="monhost">IP addresses of monitors (for external clusters).</param>
            /// <param name="mountpoint">mount point</param>
            /// <param name="namespace_">Namespace.</param>
            /// <param name="nocow">Set the NOCOW flag on files. Disables data checksumming and causes data errors to be unrecoverable from while allowing direct I/O. Only use this if data does not need to be any more safe than on a single ext4 formatted disk with no underlying raid system.</param>
            /// <param name="nodes">List of cluster node names.</param>
            /// <param name="nowritecache">disable write caching on the target</param>
            /// <param name="options">NFS/CIFS mount options (see 'man nfs' or 'man mount.cifs')</param>
            /// <param name="password">Password for accessing the share/datastore.</param>
            /// <param name="path">File system path.</param>
            /// <param name="pool">Pool.</param>
            /// <param name="port">For non default port.</param>
            /// <param name="portal">iSCSI portal (IP or DNS name with optional port).</param>
            /// <param name="preallocation">Preallocation mode for raw and qcow2 images. Using 'metadata' on raw images results in preallocation=off.
            ///   Enum: off,metadata,falloc,full</param>
            /// <param name="prune_backups">The retention options with shorter intervals are processed first with --keep-last being the very first one. Each option covers a specific period of time. We say that backups within this period are covered by this option. The next option does not take care of already covered backups and only considers older backups.</param>
            /// <param name="saferemove">Zero-out data when removing LVs.</param>
            /// <param name="saferemove_throughput">Wipe throughput (cstream -t parameter value).</param>
            /// <param name="server">Server IP or DNS name.</param>
            /// <param name="server2">Backup volfile server IP or DNS name.</param>
            /// <param name="share">CIFS share.</param>
            /// <param name="shared">Mark storage as shared.</param>
            /// <param name="smbversion">SMB protocol version. 'default' if not set, negotiates the highest SMB2+ version supported by both the client and server.
            ///   Enum: default,2.0,2.1,3,3.0,3.11</param>
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
            public async Task<Result> Create(string storage, string type, string authsupported = null, string base_ = null, string blocksize = null, string bwlimit = null, string comstar_hg = null, string comstar_tg = null, string content = null, string content_dirs = null, bool? create_base_path = null, bool? create_subdirs = null, string data_pool = null, string datastore = null, bool? disable = null, string domain = null, string encryption_key = null, string export = null, string fingerprint = null, string format = null, string fs_name = null, bool? fuse = null, string is_mountpoint = null, string iscsiprovider = null, string keyring = null, bool? krbd = null, string lio_tpg = null, string master_pubkey = null, int? max_protected_backups = null, int? maxfiles = null, bool? mkdir = null, string monhost = null, string mountpoint = null, string namespace_ = null, bool? nocow = null, string nodes = null, bool? nowritecache = null, string options = null, string password = null, string path = null, string pool = null, int? port = null, string portal = null, string preallocation = null, string prune_backups = null, bool? saferemove = null, string saferemove_throughput = null, string server = null, string server2 = null, string share = null, bool? shared = null, string smbversion = null, bool? sparse = null, string subdir = null, bool? tagged_only = null, string target = null, string thinpool = null, string transport = null, string username = null, string vgname = null, string volume = null)
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
                parameters.Add("content-dirs", content_dirs);
                parameters.Add("create-base-path", create_base_path);
                parameters.Add("create-subdirs", create_subdirs);
                parameters.Add("data-pool", data_pool);
                parameters.Add("datastore", datastore);
                parameters.Add("disable", disable);
                parameters.Add("domain", domain);
                parameters.Add("encryption-key", encryption_key);
                parameters.Add("export", export);
                parameters.Add("fingerprint", fingerprint);
                parameters.Add("format", format);
                parameters.Add("fs-name", fs_name);
                parameters.Add("fuse", fuse);
                parameters.Add("is_mountpoint", is_mountpoint);
                parameters.Add("iscsiprovider", iscsiprovider);
                parameters.Add("keyring", keyring);
                parameters.Add("krbd", krbd);
                parameters.Add("lio_tpg", lio_tpg);
                parameters.Add("master-pubkey", master_pubkey);
                parameters.Add("max-protected-backups", max_protected_backups);
                parameters.Add("maxfiles", maxfiles);
                parameters.Add("mkdir", mkdir);
                parameters.Add("monhost", monhost);
                parameters.Add("mountpoint", mountpoint);
                parameters.Add("namespace", namespace_);
                parameters.Add("nocow", nocow);
                parameters.Add("nodes", nodes);
                parameters.Add("nowritecache", nowritecache);
                parameters.Add("options", options);
                parameters.Add("password", password);
                parameters.Add("path", path);
                parameters.Add("pool", pool);
                parameters.Add("port", port);
                parameters.Add("portal", portal);
                parameters.Add("preallocation", preallocation);
                parameters.Add("prune-backups", prune_backups);
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
                return await _client.Create($"/storage", parameters);
            }
        }
        /// <summary>
        /// Access
        /// </summary>
        public class PveAccess
        {
            private readonly PveClient _client;

            internal PveAccess(PveClient client) { _client = client; }
            private PveUsers _users;
            /// <summary>
            /// Users
            /// </summary>
            public PveUsers Users => _users ??= new(_client);
            private PveGroups _groups;
            /// <summary>
            /// Groups
            /// </summary>
            public PveGroups Groups => _groups ??= new(_client);
            private PveRoles _roles;
            /// <summary>
            /// Roles
            /// </summary>
            public PveRoles Roles => _roles ??= new(_client);
            private PveAcl _acl;
            /// <summary>
            /// Acl
            /// </summary>
            public PveAcl Acl => _acl ??= new(_client);
            private PveDomains _domains;
            /// <summary>
            /// Domains
            /// </summary>
            public PveDomains Domains => _domains ??= new(_client);
            private PveOpenid _openid;
            /// <summary>
            /// Openid
            /// </summary>
            public PveOpenid Openid => _openid ??= new(_client);
            private PveTfa _tfa;
            /// <summary>
            /// Tfa
            /// </summary>
            public PveTfa Tfa => _tfa ??= new(_client);
            private PveTicket _ticket;
            /// <summary>
            /// Ticket
            /// </summary>
            public PveTicket Ticket => _ticket ??= new(_client);
            private PvePassword _password;
            /// <summary>
            /// Password
            /// </summary>
            public PvePassword Password => _password ??= new(_client);
            private PvePermissions _permissions;
            /// <summary>
            /// Permissions
            /// </summary>
            public PvePermissions Permissions => _permissions ??= new(_client);
            /// <summary>
            /// Users
            /// </summary>
            public class PveUsers
            {
                private readonly PveClient _client;

                internal PveUsers(PveClient client) { _client = client; }
                /// <summary>
                /// UseridItem
                /// </summary>
                public PveUseridItem this[object userid] => new(_client, userid);
                /// <summary>
                /// UseridItem
                /// </summary>
                public class PveUseridItem
                {
                    private readonly PveClient _client;
                    private readonly object _userid;
                    internal PveUseridItem(PveClient client, object userid) { _client = client; _userid = userid; }
                    private PveTfa _tfa;
                    /// <summary>
                    /// Tfa
                    /// </summary>
                    public PveTfa Tfa => _tfa ??= new(_client, _userid);
                    private PveUnlockTfa _unlockTfa;
                    /// <summary>
                    /// UnlockTfa
                    /// </summary>
                    public PveUnlockTfa UnlockTfa => _unlockTfa ??= new(_client, _userid);
                    private PveToken _token;
                    /// <summary>
                    /// Token
                    /// </summary>
                    public PveToken Token => _token ??= new(_client, _userid);
                    /// <summary>
                    /// Tfa
                    /// </summary>
                    public class PveTfa
                    {
                        private readonly PveClient _client;
                        private readonly object _userid;
                        internal PveTfa(PveClient client, object userid) { _client = client; _userid = userid; }
                        /// <summary>
                        /// Get user TFA types (Personal and Realm).
                        /// </summary>
                        /// <param name="multiple">Request all entries as an array.</param>
                        /// <returns></returns>
                        public async Task<Result> ReadUserTfaType(bool? multiple = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("multiple", multiple);
                            return await _client.Get($"/access/users/{_userid}/tfa", parameters);
                        }
                    }
                    /// <summary>
                    /// UnlockTfa
                    /// </summary>
                    public class PveUnlockTfa
                    {
                        private readonly PveClient _client;
                        private readonly object _userid;
                        internal PveUnlockTfa(PveClient client, object userid) { _client = client; _userid = userid; }
                        /// <summary>
                        /// Unlock a user's TFA authentication.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> UnlockTfa() { return await _client.Set($"/access/users/{_userid}/unlock-tfa"); }
                    }
                    /// <summary>
                    /// Token
                    /// </summary>
                    public class PveToken
                    {
                        private readonly PveClient _client;
                        private readonly object _userid;
                        internal PveToken(PveClient client, object userid) { _client = client; _userid = userid; }
                        /// <summary>
                        /// TokenidItem
                        /// </summary>
                        public PveTokenidItem this[object tokenid] => new(_client, _userid, tokenid);
                        /// <summary>
                        /// TokenidItem
                        /// </summary>
                        public class PveTokenidItem
                        {
                            private readonly PveClient _client;
                            private readonly object _userid;
                            private readonly object _tokenid;
                            internal PveTokenidItem(PveClient client, object userid, object tokenid)
                            {
                                _client = client; _userid = userid;
                                _tokenid = tokenid;
                            }
                            /// <summary>
                            /// Remove API token for a specific user.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> RemoveToken() { return await _client.Delete($"/access/users/{_userid}/token/{_tokenid}"); }
                            /// <summary>
                            /// Get specific API token information.
                            /// </summary>
                            /// <returns></returns>
                            public async Task<Result> ReadToken() { return await _client.Get($"/access/users/{_userid}/token/{_tokenid}"); }
                            /// <summary>
                            /// Generate a new API token for a specific user. NOTE: returns API token value, which needs to be stored as it cannot be retrieved afterwards!
                            /// </summary>
                            /// <param name="comment"></param>
                            /// <param name="expire">API token expiration date (seconds since epoch). '0' means no expiration date.</param>
                            /// <param name="privsep">Restrict API token privileges with separate ACLs (default), or give full privileges of corresponding user.</param>
                            /// <returns></returns>
                            public async Task<Result> GenerateToken(string comment = null, int? expire = null, bool? privsep = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("comment", comment);
                                parameters.Add("expire", expire);
                                parameters.Add("privsep", privsep);
                                return await _client.Create($"/access/users/{_userid}/token/{_tokenid}", parameters);
                            }
                            /// <summary>
                            /// Update API token for a specific user.
                            /// </summary>
                            /// <param name="comment"></param>
                            /// <param name="expire">API token expiration date (seconds since epoch). '0' means no expiration date.</param>
                            /// <param name="privsep">Restrict API token privileges with separate ACLs (default), or give full privileges of corresponding user.</param>
                            /// <returns></returns>
                            public async Task<Result> UpdateTokenInfo(string comment = null, int? expire = null, bool? privsep = null)
                            {
                                var parameters = new Dictionary<string, object>();
                                parameters.Add("comment", comment);
                                parameters.Add("expire", expire);
                                parameters.Add("privsep", privsep);
                                return await _client.Set($"/access/users/{_userid}/token/{_tokenid}", parameters);
                            }
                        }
                        /// <summary>
                        /// Get user API tokens.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> TokenIndex() { return await _client.Get($"/access/users/{_userid}/token"); }
                    }
                    /// <summary>
                    /// Delete user.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> DeleteUser() { return await _client.Delete($"/access/users/{_userid}"); }
                    /// <summary>
                    /// Get user configuration.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> ReadUser() { return await _client.Get($"/access/users/{_userid}"); }
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
                    public async Task<Result> UpdateUser(bool? append = null, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null)
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
                        return await _client.Set($"/access/users/{_userid}", parameters);
                    }
                }
                /// <summary>
                /// User index.
                /// </summary>
                /// <param name="enabled">Optional filter for enable property.</param>
                /// <param name="full">Include group and token information.</param>
                /// <returns></returns>
                public async Task<Result> Index(bool? enabled = null, bool? full = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("enabled", enabled);
                    parameters.Add("full", full);
                    return await _client.Get($"/access/users", parameters);
                }
                /// <summary>
                /// Create new user.
                /// </summary>
                /// <param name="userid">Full User ID, in the `name@realm` format.</param>
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
                public async Task<Result> CreateUser(string userid, string comment = null, string email = null, bool? enable = null, int? expire = null, string firstname = null, string groups = null, string keys = null, string lastname = null, string password = null)
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
                    return await _client.Create($"/access/users", parameters);
                }
            }
            /// <summary>
            /// Groups
            /// </summary>
            public class PveGroups
            {
                private readonly PveClient _client;

                internal PveGroups(PveClient client) { _client = client; }
                /// <summary>
                /// GroupidItem
                /// </summary>
                public PveGroupidItem this[object groupid] => new(_client, groupid);
                /// <summary>
                /// GroupidItem
                /// </summary>
                public class PveGroupidItem
                {
                    private readonly PveClient _client;
                    private readonly object _groupid;
                    internal PveGroupidItem(PveClient client, object groupid) { _client = client; _groupid = groupid; }
                    /// <summary>
                    /// Delete group.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> DeleteGroup() { return await _client.Delete($"/access/groups/{_groupid}"); }
                    /// <summary>
                    /// Get group configuration.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> ReadGroup() { return await _client.Get($"/access/groups/{_groupid}"); }
                    /// <summary>
                    /// Update group data.
                    /// </summary>
                    /// <param name="comment"></param>
                    /// <returns></returns>
                    public async Task<Result> UpdateGroup(string comment = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("comment", comment);
                        return await _client.Set($"/access/groups/{_groupid}", parameters);
                    }
                }
                /// <summary>
                /// Group index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/access/groups"); }
                /// <summary>
                /// Create new group.
                /// </summary>
                /// <param name="groupid"></param>
                /// <param name="comment"></param>
                /// <returns></returns>
                public async Task<Result> CreateGroup(string groupid, string comment = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("groupid", groupid);
                    parameters.Add("comment", comment);
                    return await _client.Create($"/access/groups", parameters);
                }
            }
            /// <summary>
            /// Roles
            /// </summary>
            public class PveRoles
            {
                private readonly PveClient _client;

                internal PveRoles(PveClient client) { _client = client; }
                /// <summary>
                /// RoleidItem
                /// </summary>
                public PveRoleidItem this[object roleid] => new(_client, roleid);
                /// <summary>
                /// RoleidItem
                /// </summary>
                public class PveRoleidItem
                {
                    private readonly PveClient _client;
                    private readonly object _roleid;
                    internal PveRoleidItem(PveClient client, object roleid) { _client = client; _roleid = roleid; }
                    /// <summary>
                    /// Delete role.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> DeleteRole() { return await _client.Delete($"/access/roles/{_roleid}"); }
                    /// <summary>
                    /// Get role configuration.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> ReadRole() { return await _client.Get($"/access/roles/{_roleid}"); }
                    /// <summary>
                    /// Update an existing role.
                    /// </summary>
                    /// <param name="append"></param>
                    /// <param name="privs"></param>
                    /// <returns></returns>
                    public async Task<Result> UpdateRole(bool? append = null, string privs = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("append", append);
                        parameters.Add("privs", privs);
                        return await _client.Set($"/access/roles/{_roleid}", parameters);
                    }
                }
                /// <summary>
                /// Role index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/access/roles"); }
                /// <summary>
                /// Create new role.
                /// </summary>
                /// <param name="roleid"></param>
                /// <param name="privs"></param>
                /// <returns></returns>
                public async Task<Result> CreateRole(string roleid, string privs = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("roleid", roleid);
                    parameters.Add("privs", privs);
                    return await _client.Create($"/access/roles", parameters);
                }
            }
            /// <summary>
            /// Acl
            /// </summary>
            public class PveAcl
            {
                private readonly PveClient _client;

                internal PveAcl(PveClient client) { _client = client; }
                /// <summary>
                /// Get Access Control List (ACLs).
                /// </summary>
                /// <returns></returns>
                public async Task<Result> ReadAcl() { return await _client.Get($"/access/acl"); }
                /// <summary>
                /// Update Access Control List (add or remove permissions).
                /// </summary>
                /// <param name="path">Access control path</param>
                /// <param name="roles">List of roles.</param>
                /// <param name="delete">Remove permissions (instead of adding it).</param>
                /// <param name="groups">List of groups.</param>
                /// <param name="propagate">Allow to propagate (inherit) permissions.</param>
                /// <param name="tokens">List of API tokens.</param>
                /// <param name="users">List of users.</param>
                /// <returns></returns>
                public async Task<Result> UpdateAcl(string path, string roles, bool? delete = null, string groups = null, bool? propagate = null, string tokens = null, string users = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("path", path);
                    parameters.Add("roles", roles);
                    parameters.Add("delete", delete);
                    parameters.Add("groups", groups);
                    parameters.Add("propagate", propagate);
                    parameters.Add("tokens", tokens);
                    parameters.Add("users", users);
                    return await _client.Set($"/access/acl", parameters);
                }
            }
            /// <summary>
            /// Domains
            /// </summary>
            public class PveDomains
            {
                private readonly PveClient _client;

                internal PveDomains(PveClient client) { _client = client; }
                /// <summary>
                /// RealmItem
                /// </summary>
                public PveRealmItem this[object realm] => new(_client, realm);
                /// <summary>
                /// RealmItem
                /// </summary>
                public class PveRealmItem
                {
                    private readonly PveClient _client;
                    private readonly object _realm;
                    internal PveRealmItem(PveClient client, object realm) { _client = client; _realm = realm; }
                    private PveSync _sync;
                    /// <summary>
                    /// Sync
                    /// </summary>
                    public PveSync Sync => _sync ??= new(_client, _realm);
                    /// <summary>
                    /// Sync
                    /// </summary>
                    public class PveSync
                    {
                        private readonly PveClient _client;
                        private readonly object _realm;
                        internal PveSync(PveClient client, object realm) { _client = client; _realm = realm; }
                        /// <summary>
                        /// Syncs users and/or groups from the configured LDAP to user.cfg. NOTE: Synced groups will have the name 'name-$realm', so make sure those groups do not exist to prevent overwriting.
                        /// </summary>
                        /// <param name="dry_run">If set, does not write anything.</param>
                        /// <param name="enable_new">Enable newly synced users immediately.</param>
                        /// <param name="full">DEPRECATED: use 'remove-vanished' instead. If set, uses the LDAP Directory as source of truth, deleting users or groups not returned from the sync and removing all locally modified properties of synced users. If not set, only syncs information which is present in the synced data, and does not delete or modify anything else.</param>
                        /// <param name="purge">DEPRECATED: use 'remove-vanished' instead. Remove ACLs for users or groups which were removed from the config during a sync.</param>
                        /// <param name="remove_vanished">A semicolon-seperated list of things to remove when they or the user vanishes during a sync. The following values are possible: 'entry' removes the user/group when not returned from the sync. 'properties' removes the set properties on existing user/group that do not appear in the source (even custom ones). 'acl' removes acls when the user/group is not returned from the sync. Instead of a list it also can be 'none' (the default).</param>
                        /// <param name="scope">Select what to sync.
                        ///   Enum: users,groups,both</param>
                        /// <returns></returns>
                        public async Task<Result> Sync(bool? dry_run = null, bool? enable_new = null, bool? full = null, bool? purge = null, string remove_vanished = null, string scope = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("dry-run", dry_run);
                            parameters.Add("enable-new", enable_new);
                            parameters.Add("full", full);
                            parameters.Add("purge", purge);
                            parameters.Add("remove-vanished", remove_vanished);
                            parameters.Add("scope", scope);
                            return await _client.Create($"/access/domains/{_realm}/sync", parameters);
                        }
                    }
                    /// <summary>
                    /// Delete an authentication server.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Delete() { return await _client.Delete($"/access/domains/{_realm}"); }
                    /// <summary>
                    /// Get auth server configuration.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> Read() { return await _client.Get($"/access/domains/{_realm}"); }
                    /// <summary>
                    /// Update authentication server settings.
                    /// </summary>
                    /// <param name="acr_values">Specifies the Authentication Context Class Reference values that theAuthorization Server is being requested to use for the Auth Request.</param>
                    /// <param name="autocreate">Automatically create users if they do not exist.</param>
                    /// <param name="base_dn">LDAP base domain name</param>
                    /// <param name="bind_dn">LDAP bind domain name</param>
                    /// <param name="capath">Path to the CA certificate store</param>
                    /// <param name="case_sensitive">username is case-sensitive</param>
                    /// <param name="cert">Path to the client certificate</param>
                    /// <param name="certkey">Path to the client certificate key</param>
                    /// <param name="check_connection">Check bind connection to the server.</param>
                    /// <param name="client_id">OpenID Client ID</param>
                    /// <param name="client_key">OpenID Client Key</param>
                    /// <param name="comment">Description.</param>
                    /// <param name="default_">Use this as default realm</param>
                    /// <param name="delete">A list of settings you want to delete.</param>
                    /// <param name="digest">Prevent changes if current configuration file has a different digest. This can be used to prevent concurrent modifications.</param>
                    /// <param name="domain">AD domain name</param>
                    /// <param name="filter">LDAP filter for user sync.</param>
                    /// <param name="group_classes">The objectclasses for groups.</param>
                    /// <param name="group_dn">LDAP base domain name for group sync. If not set, the base_dn will be used.</param>
                    /// <param name="group_filter">LDAP filter for group sync.</param>
                    /// <param name="group_name_attr">LDAP attribute representing a groups name. If not set or found, the first value of the DN will be used as name.</param>
                    /// <param name="issuer_url">OpenID Issuer Url</param>
                    /// <param name="mode">LDAP protocol mode.
                    ///   Enum: ldap,ldaps,ldap+starttls</param>
                    /// <param name="password">LDAP bind password. Will be stored in '/etc/pve/priv/realm/&amp;lt;REALM&amp;gt;.pw'.</param>
                    /// <param name="port">Server port.</param>
                    /// <param name="prompt">Specifies whether the Authorization Server prompts the End-User for reauthentication and consent.</param>
                    /// <param name="scopes">Specifies the scopes (user details) that should be authorized and returned, for example 'email' or 'profile'.</param>
                    /// <param name="secure">Use secure LDAPS protocol. DEPRECATED: use 'mode' instead.</param>
                    /// <param name="server1">Server IP address (or DNS name)</param>
                    /// <param name="server2">Fallback Server IP address (or DNS name)</param>
                    /// <param name="sslversion">LDAPS TLS/SSL version. It's not recommended to use version older than 1.2!
                    ///   Enum: tlsv1,tlsv1_1,tlsv1_2,tlsv1_3</param>
                    /// <param name="sync_defaults_options">The default options for behavior of synchronizations.</param>
                    /// <param name="sync_attributes">Comma separated list of key=value pairs for specifying which LDAP attributes map to which PVE user field. For example, to map the LDAP attribute 'mail' to PVEs 'email', write  'email=mail'. By default, each PVE user field is represented  by an LDAP attribute of the same name.</param>
                    /// <param name="tfa">Use Two-factor authentication.</param>
                    /// <param name="user_attr">LDAP user attribute name</param>
                    /// <param name="user_classes">The objectclasses for users.</param>
                    /// <param name="verify">Verify the server's SSL certificate</param>
                    /// <returns></returns>
                    public async Task<Result> Update(string acr_values = null, bool? autocreate = null, string base_dn = null, string bind_dn = null, string capath = null, bool? case_sensitive = null, string cert = null, string certkey = null, bool? check_connection = null, string client_id = null, string client_key = null, string comment = null, bool? default_ = null, string delete = null, string digest = null, string domain = null, string filter = null, string group_classes = null, string group_dn = null, string group_filter = null, string group_name_attr = null, string issuer_url = null, string mode = null, string password = null, int? port = null, string prompt = null, string scopes = null, bool? secure = null, string server1 = null, string server2 = null, string sslversion = null, string sync_defaults_options = null, string sync_attributes = null, string tfa = null, string user_attr = null, string user_classes = null, bool? verify = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("acr-values", acr_values);
                        parameters.Add("autocreate", autocreate);
                        parameters.Add("base_dn", base_dn);
                        parameters.Add("bind_dn", bind_dn);
                        parameters.Add("capath", capath);
                        parameters.Add("case-sensitive", case_sensitive);
                        parameters.Add("cert", cert);
                        parameters.Add("certkey", certkey);
                        parameters.Add("check-connection", check_connection);
                        parameters.Add("client-id", client_id);
                        parameters.Add("client-key", client_key);
                        parameters.Add("comment", comment);
                        parameters.Add("default", default_);
                        parameters.Add("delete", delete);
                        parameters.Add("digest", digest);
                        parameters.Add("domain", domain);
                        parameters.Add("filter", filter);
                        parameters.Add("group_classes", group_classes);
                        parameters.Add("group_dn", group_dn);
                        parameters.Add("group_filter", group_filter);
                        parameters.Add("group_name_attr", group_name_attr);
                        parameters.Add("issuer-url", issuer_url);
                        parameters.Add("mode", mode);
                        parameters.Add("password", password);
                        parameters.Add("port", port);
                        parameters.Add("prompt", prompt);
                        parameters.Add("scopes", scopes);
                        parameters.Add("secure", secure);
                        parameters.Add("server1", server1);
                        parameters.Add("server2", server2);
                        parameters.Add("sslversion", sslversion);
                        parameters.Add("sync-defaults-options", sync_defaults_options);
                        parameters.Add("sync_attributes", sync_attributes);
                        parameters.Add("tfa", tfa);
                        parameters.Add("user_attr", user_attr);
                        parameters.Add("user_classes", user_classes);
                        parameters.Add("verify", verify);
                        return await _client.Set($"/access/domains/{_realm}", parameters);
                    }
                }
                /// <summary>
                /// Authentication domain index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/access/domains"); }
                /// <summary>
                /// Add an authentication server.
                /// </summary>
                /// <param name="realm">Authentication domain ID</param>
                /// <param name="type">Realm type.
                ///   Enum: ad,ldap,openid,pam,pve</param>
                /// <param name="acr_values">Specifies the Authentication Context Class Reference values that theAuthorization Server is being requested to use for the Auth Request.</param>
                /// <param name="autocreate">Automatically create users if they do not exist.</param>
                /// <param name="base_dn">LDAP base domain name</param>
                /// <param name="bind_dn">LDAP bind domain name</param>
                /// <param name="capath">Path to the CA certificate store</param>
                /// <param name="case_sensitive">username is case-sensitive</param>
                /// <param name="cert">Path to the client certificate</param>
                /// <param name="certkey">Path to the client certificate key</param>
                /// <param name="check_connection">Check bind connection to the server.</param>
                /// <param name="client_id">OpenID Client ID</param>
                /// <param name="client_key">OpenID Client Key</param>
                /// <param name="comment">Description.</param>
                /// <param name="default_">Use this as default realm</param>
                /// <param name="domain">AD domain name</param>
                /// <param name="filter">LDAP filter for user sync.</param>
                /// <param name="group_classes">The objectclasses for groups.</param>
                /// <param name="group_dn">LDAP base domain name for group sync. If not set, the base_dn will be used.</param>
                /// <param name="group_filter">LDAP filter for group sync.</param>
                /// <param name="group_name_attr">LDAP attribute representing a groups name. If not set or found, the first value of the DN will be used as name.</param>
                /// <param name="issuer_url">OpenID Issuer Url</param>
                /// <param name="mode">LDAP protocol mode.
                ///   Enum: ldap,ldaps,ldap+starttls</param>
                /// <param name="password">LDAP bind password. Will be stored in '/etc/pve/priv/realm/&amp;lt;REALM&amp;gt;.pw'.</param>
                /// <param name="port">Server port.</param>
                /// <param name="prompt">Specifies whether the Authorization Server prompts the End-User for reauthentication and consent.</param>
                /// <param name="scopes">Specifies the scopes (user details) that should be authorized and returned, for example 'email' or 'profile'.</param>
                /// <param name="secure">Use secure LDAPS protocol. DEPRECATED: use 'mode' instead.</param>
                /// <param name="server1">Server IP address (or DNS name)</param>
                /// <param name="server2">Fallback Server IP address (or DNS name)</param>
                /// <param name="sslversion">LDAPS TLS/SSL version. It's not recommended to use version older than 1.2!
                ///   Enum: tlsv1,tlsv1_1,tlsv1_2,tlsv1_3</param>
                /// <param name="sync_defaults_options">The default options for behavior of synchronizations.</param>
                /// <param name="sync_attributes">Comma separated list of key=value pairs for specifying which LDAP attributes map to which PVE user field. For example, to map the LDAP attribute 'mail' to PVEs 'email', write  'email=mail'. By default, each PVE user field is represented  by an LDAP attribute of the same name.</param>
                /// <param name="tfa">Use Two-factor authentication.</param>
                /// <param name="user_attr">LDAP user attribute name</param>
                /// <param name="user_classes">The objectclasses for users.</param>
                /// <param name="username_claim">OpenID claim used to generate the unique username.</param>
                /// <param name="verify">Verify the server's SSL certificate</param>
                /// <returns></returns>
                public async Task<Result> Create(string realm, string type, string acr_values = null, bool? autocreate = null, string base_dn = null, string bind_dn = null, string capath = null, bool? case_sensitive = null, string cert = null, string certkey = null, bool? check_connection = null, string client_id = null, string client_key = null, string comment = null, bool? default_ = null, string domain = null, string filter = null, string group_classes = null, string group_dn = null, string group_filter = null, string group_name_attr = null, string issuer_url = null, string mode = null, string password = null, int? port = null, string prompt = null, string scopes = null, bool? secure = null, string server1 = null, string server2 = null, string sslversion = null, string sync_defaults_options = null, string sync_attributes = null, string tfa = null, string user_attr = null, string user_classes = null, string username_claim = null, bool? verify = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("realm", realm);
                    parameters.Add("type", type);
                    parameters.Add("acr-values", acr_values);
                    parameters.Add("autocreate", autocreate);
                    parameters.Add("base_dn", base_dn);
                    parameters.Add("bind_dn", bind_dn);
                    parameters.Add("capath", capath);
                    parameters.Add("case-sensitive", case_sensitive);
                    parameters.Add("cert", cert);
                    parameters.Add("certkey", certkey);
                    parameters.Add("check-connection", check_connection);
                    parameters.Add("client-id", client_id);
                    parameters.Add("client-key", client_key);
                    parameters.Add("comment", comment);
                    parameters.Add("default", default_);
                    parameters.Add("domain", domain);
                    parameters.Add("filter", filter);
                    parameters.Add("group_classes", group_classes);
                    parameters.Add("group_dn", group_dn);
                    parameters.Add("group_filter", group_filter);
                    parameters.Add("group_name_attr", group_name_attr);
                    parameters.Add("issuer-url", issuer_url);
                    parameters.Add("mode", mode);
                    parameters.Add("password", password);
                    parameters.Add("port", port);
                    parameters.Add("prompt", prompt);
                    parameters.Add("scopes", scopes);
                    parameters.Add("secure", secure);
                    parameters.Add("server1", server1);
                    parameters.Add("server2", server2);
                    parameters.Add("sslversion", sslversion);
                    parameters.Add("sync-defaults-options", sync_defaults_options);
                    parameters.Add("sync_attributes", sync_attributes);
                    parameters.Add("tfa", tfa);
                    parameters.Add("user_attr", user_attr);
                    parameters.Add("user_classes", user_classes);
                    parameters.Add("username-claim", username_claim);
                    parameters.Add("verify", verify);
                    return await _client.Create($"/access/domains", parameters);
                }
            }
            /// <summary>
            /// Openid
            /// </summary>
            public class PveOpenid
            {
                private readonly PveClient _client;

                internal PveOpenid(PveClient client) { _client = client; }
                private PveAuthUrl _authUrl;
                /// <summary>
                /// AuthUrl
                /// </summary>
                public PveAuthUrl AuthUrl => _authUrl ??= new(_client);
                private PveLogin _login;
                /// <summary>
                /// Login
                /// </summary>
                public PveLogin Login => _login ??= new(_client);
                /// <summary>
                /// AuthUrl
                /// </summary>
                public class PveAuthUrl
                {
                    private readonly PveClient _client;

                    internal PveAuthUrl(PveClient client) { _client = client; }
                    /// <summary>
                    /// Get the OpenId Authorization Url for the specified realm.
                    /// </summary>
                    /// <param name="realm">Authentication domain ID</param>
                    /// <param name="redirect_url">Redirection Url. The client should set this to the used server url (location.origin).</param>
                    /// <returns></returns>
                    public async Task<Result> AuthUrl(string realm, string redirect_url)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("realm", realm);
                        parameters.Add("redirect-url", redirect_url);
                        return await _client.Create($"/access/openid/auth-url", parameters);
                    }
                }
                /// <summary>
                /// Login
                /// </summary>
                public class PveLogin
                {
                    private readonly PveClient _client;

                    internal PveLogin(PveClient client) { _client = client; }
                    /// <summary>
                    ///  Verify OpenID authorization code and create a ticket.
                    /// </summary>
                    /// <param name="code">OpenId authorization code.</param>
                    /// <param name="redirect_url">Redirection Url. The client should set this to the used server url (location.origin).</param>
                    /// <param name="state">OpenId state.</param>
                    /// <returns></returns>
                    public async Task<Result> Login(string code, string redirect_url, string state)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("code", code);
                        parameters.Add("redirect-url", redirect_url);
                        parameters.Add("state", state);
                        return await _client.Create($"/access/openid/login", parameters);
                    }
                }
                /// <summary>
                /// Directory index.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> Index() { return await _client.Get($"/access/openid"); }
            }
            /// <summary>
            /// Tfa
            /// </summary>
            public class PveTfa
            {
                private readonly PveClient _client;

                internal PveTfa(PveClient client) { _client = client; }
                /// <summary>
                /// UseridItem
                /// </summary>
                public PveUseridItem this[object userid] => new(_client, userid);
                /// <summary>
                /// UseridItem
                /// </summary>
                public class PveUseridItem
                {
                    private readonly PveClient _client;
                    private readonly object _userid;
                    internal PveUseridItem(PveClient client, object userid) { _client = client; _userid = userid; }
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public PveIdItem this[object id] => new(_client, _userid, id);
                    /// <summary>
                    /// IdItem
                    /// </summary>
                    public class PveIdItem
                    {
                        private readonly PveClient _client;
                        private readonly object _userid;
                        private readonly object _id;
                        internal PveIdItem(PveClient client, object userid, object id)
                        {
                            _client = client; _userid = userid;
                            _id = id;
                        }
                        /// <summary>
                        /// Delete a TFA entry by ID.
                        /// </summary>
                        /// <param name="password">The current password.</param>
                        /// <returns></returns>
                        public async Task<Result> DeleteTfa(string password = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("password", password);
                            return await _client.Delete($"/access/tfa/{_userid}/{_id}", parameters);
                        }
                        /// <summary>
                        /// Fetch a requested TFA entry if present.
                        /// </summary>
                        /// <returns></returns>
                        public async Task<Result> GetTfaEntry() { return await _client.Get($"/access/tfa/{_userid}/{_id}"); }
                        /// <summary>
                        /// Add a TFA entry for a user.
                        /// </summary>
                        /// <param name="description">A description to distinguish multiple entries from one another</param>
                        /// <param name="enable">Whether the entry should be enabled for login.</param>
                        /// <param name="password">The current password.</param>
                        /// <returns></returns>
                        public async Task<Result> UpdateTfaEntry(string description = null, bool? enable = null, string password = null)
                        {
                            var parameters = new Dictionary<string, object>();
                            parameters.Add("description", description);
                            parameters.Add("enable", enable);
                            parameters.Add("password", password);
                            return await _client.Set($"/access/tfa/{_userid}/{_id}", parameters);
                        }
                    }
                    /// <summary>
                    /// List TFA configurations of users.
                    /// </summary>
                    /// <returns></returns>
                    public async Task<Result> ListUserTfa() { return await _client.Get($"/access/tfa/{_userid}"); }
                    /// <summary>
                    /// Add a TFA entry for a user.
                    /// </summary>
                    /// <param name="type">TFA Entry Type.
                    ///   Enum: totp,u2f,webauthn,recovery,yubico</param>
                    /// <param name="challenge">When responding to a u2f challenge: the original challenge string</param>
                    /// <param name="description">A description to distinguish multiple entries from one another</param>
                    /// <param name="password">The current password.</param>
                    /// <param name="totp">A totp URI.</param>
                    /// <param name="value">The current value for the provided totp URI, or a Webauthn/U2F challenge response</param>
                    /// <returns></returns>
                    public async Task<Result> AddTfaEntry(string type, string challenge = null, string description = null, string password = null, string totp = null, string value = null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("type", type);
                        parameters.Add("challenge", challenge);
                        parameters.Add("description", description);
                        parameters.Add("password", password);
                        parameters.Add("totp", totp);
                        parameters.Add("value", value);
                        return await _client.Create($"/access/tfa/{_userid}", parameters);
                    }
                }
                /// <summary>
                /// List TFA configurations of users.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> ListTfa() { return await _client.Get($"/access/tfa"); }
            }
            /// <summary>
            /// Ticket
            /// </summary>
            public class PveTicket
            {
                private readonly PveClient _client;

                internal PveTicket(PveClient client) { _client = client; }
                /// <summary>
                /// Dummy. Useful for formatters which want to provide a login page.
                /// </summary>
                /// <returns></returns>
                public async Task<Result> GetTicket() { return await _client.Get($"/access/ticket"); }
                /// <summary>
                /// Create or verify authentication ticket.
                /// </summary>
                /// <param name="password">The secret password. This can also be a valid ticket.</param>
                /// <param name="username">User name</param>
                /// <param name="new_format">This parameter is now ignored and assumed to be 1.</param>
                /// <param name="otp">One-time password for Two-factor authentication.</param>
                /// <param name="path">Verify ticket, and check if user have access 'privs' on 'path'</param>
                /// <param name="privs">Verify ticket, and check if user have access 'privs' on 'path'</param>
                /// <param name="realm">You can optionally pass the realm using this parameter. Normally the realm is simply added to the username &amp;lt;username&amp;gt;@&amp;lt;relam&amp;gt;.</param>
                /// <param name="tfa_challenge">The signed TFA challenge string the user wants to respond to.</param>
                /// <returns></returns>
                public async Task<Result> CreateTicket(string password, string username, bool? new_format = null, string otp = null, string path = null, string privs = null, string realm = null, string tfa_challenge = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("password", password);
                    parameters.Add("username", username);
                    parameters.Add("new-format", new_format);
                    parameters.Add("otp", otp);
                    parameters.Add("path", path);
                    parameters.Add("privs", privs);
                    parameters.Add("realm", realm);
                    parameters.Add("tfa-challenge", tfa_challenge);
                    return await _client.Create($"/access/ticket", parameters);
                }
            }
            /// <summary>
            /// Password
            /// </summary>
            public class PvePassword
            {
                private readonly PveClient _client;

                internal PvePassword(PveClient client) { _client = client; }
                /// <summary>
                /// Change user password.
                /// </summary>
                /// <param name="password">The new password.</param>
                /// <param name="userid">Full User ID, in the `name@realm` format.</param>
                /// <returns></returns>
                public async Task<Result> ChangePassword(string password, string userid)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("password", password);
                    parameters.Add("userid", userid);
                    return await _client.Set($"/access/password", parameters);
                }
            }
            /// <summary>
            /// Permissions
            /// </summary>
            public class PvePermissions
            {
                private readonly PveClient _client;

                internal PvePermissions(PveClient client) { _client = client; }
                /// <summary>
                /// Retrieve effective permissions of given user/token.
                /// </summary>
                /// <param name="path">Only dump this specific path, not the whole tree.</param>
                /// <param name="userid">User ID or full API token ID</param>
                /// <returns></returns>
                public async Task<Result> Permissions(string path = null, string userid = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("path", path);
                    parameters.Add("userid", userid);
                    return await _client.Get($"/access/permissions", parameters);
                }
            }
            /// <summary>
            /// Directory index.
            /// </summary>
            /// <returns></returns>
            public async Task<Result> Index() { return await _client.Get($"/access"); }
        }
        /// <summary>
        /// Pools
        /// </summary>
        public class PvePools
        {
            private readonly PveClient _client;

            internal PvePools(PveClient client) { _client = client; }
            /// <summary>
            /// PoolidItem
            /// </summary>
            public PvePoolidItem this[object poolid] => new(_client, poolid);
            /// <summary>
            /// PoolidItem
            /// </summary>
            public class PvePoolidItem
            {
                private readonly PveClient _client;
                private readonly object _poolid;
                internal PvePoolidItem(PveClient client, object poolid) { _client = client; _poolid = poolid; }
                /// <summary>
                /// Delete pool (deprecated, no support for nested pools, use 'DELETE /pools/?poolid={poolid}').
                /// </summary>
                /// <returns></returns>
                public async Task<Result> DeletePoolDeprecated() { return await _client.Delete($"/pools/{_poolid}"); }
                /// <summary>
                /// Get pool configuration (deprecated, no support for nested pools, use 'GET /pools/?poolid={poolid}').
                /// </summary>
                /// <param name="type">
                ///   Enum: qemu,lxc,storage</param>
                /// <returns></returns>
                public async Task<Result> ReadPool(string type = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("type", type);
                    return await _client.Get($"/pools/{_poolid}", parameters);
                }
                /// <summary>
                /// Update pool data (deprecated, no support for nested pools - use 'PUT /pools/?poolid={poolid}' instead).
                /// </summary>
                /// <param name="allow_move">Allow adding a guest even if already in another pool. The guest will be removed from its current pool and added to this one.</param>
                /// <param name="comment"></param>
                /// <param name="delete">Remove the passed VMIDs and/or storage IDs instead of adding them.</param>
                /// <param name="storage">List of storage IDs to add or remove from this pool.</param>
                /// <param name="vms">List of guest VMIDs to add or remove from this pool.</param>
                /// <returns></returns>
                public async Task<Result> UpdatePoolDeprecated(bool? allow_move = null, string comment = null, bool? delete = null, string storage = null, string vms = null)
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("allow-move", allow_move);
                    parameters.Add("comment", comment);
                    parameters.Add("delete", delete);
                    parameters.Add("storage", storage);
                    parameters.Add("vms", vms);
                    return await _client.Set($"/pools/{_poolid}", parameters);
                }
            }
            /// <summary>
            /// Delete pool.
            /// </summary>
            /// <param name="poolid"></param>
            /// <returns></returns>
            public async Task<Result> DeletePool(string poolid)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("poolid", poolid);
                return await _client.Delete($"/pools", parameters);
            }
            /// <summary>
            /// List pools or get pool configuration.
            /// </summary>
            /// <param name="poolid"></param>
            /// <param name="type">
            ///   Enum: qemu,lxc,storage</param>
            /// <returns></returns>
            public async Task<Result> Index(string poolid = null, string type = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("poolid", poolid);
                parameters.Add("type", type);
                return await _client.Get($"/pools", parameters);
            }
            /// <summary>
            /// Create new pool.
            /// </summary>
            /// <param name="poolid"></param>
            /// <param name="comment"></param>
            /// <returns></returns>
            public async Task<Result> CreatePool(string poolid, string comment = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("poolid", poolid);
                parameters.Add("comment", comment);
                return await _client.Create($"/pools", parameters);
            }
            /// <summary>
            /// Update pool.
            /// </summary>
            /// <param name="poolid"></param>
            /// <param name="allow_move">Allow adding a guest even if already in another pool. The guest will be removed from its current pool and added to this one.</param>
            /// <param name="comment"></param>
            /// <param name="delete">Remove the passed VMIDs and/or storage IDs instead of adding them.</param>
            /// <param name="storage">List of storage IDs to add or remove from this pool.</param>
            /// <param name="vms">List of guest VMIDs to add or remove from this pool.</param>
            /// <returns></returns>
            public async Task<Result> UpdatePool(string poolid, bool? allow_move = null, string comment = null, bool? delete = null, string storage = null, string vms = null)
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("poolid", poolid);
                parameters.Add("allow-move", allow_move);
                parameters.Add("comment", comment);
                parameters.Add("delete", delete);
                parameters.Add("storage", storage);
                parameters.Add("vms", vms);
                return await _client.Set($"/pools", parameters);
            }
        }
        /// <summary>
        /// Version
        /// </summary>
        public class PveVersion
        {
            private readonly PveClient _client;

            internal PveVersion(PveClient client) { _client = client; }
            /// <summary>
            /// API version details, including some parts of the global datacenter config.
            /// </summary>
            /// <returns></returns>
            public async Task<Result> Version() { return await _client.Get($"/version"); }
        }

    }
}