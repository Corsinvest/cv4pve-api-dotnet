/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Access;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Pool;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Storage;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    /// <summary>
    /// Global Info
    /// </summary>
    public static class InfoHelper
    {
        /// <summary>
        /// Info
        /// </summary>
        public class Info
        {
            /// <summary>
            /// Version
            /// </summary>
            /// <value></value>
            public Version Version { get; set; }

            /// <summary>
            /// Data execution
            /// </summary>
            /// <value></value>
            public DateTime Date { get; set; }

            /// <summary>
            /// Data execution
            /// </summary>
            /// <value></value>
            public TimeSpan CollectExecution { get; set; }

            /// <summary>
            /// Permissions
            /// </summary>
            /// <returns></returns>
            public AccessInfo Access { get; } = new AccessInfo();

            /// <summary>
            /// Permissions
            /// </summary>
            /// <returns></returns>
            public ClusterInfo Cluster { get; } = new ClusterInfo();

            /// <summary>
            /// Pools
            /// </summary>
            /// <value></value>
            public IEnumerable<PoolInfo> Pools { get; set; }

            /// <summary>
            /// Storage
            /// </summary>
            /// <value></value>
            public IEnumerable<StorageItem> Storages { get; set; }

            /// <summary>
            /// Nodes
            /// </summary>
            /// <value></value>
            public IEnumerable<NodeInfo> Nodes { get; set; }

            /// <summary>
            /// Pool
            /// </summary>
            public class PoolInfo : PoolItem
            {
                /// <summary>
                /// Pools
                /// </summary>
                /// <value></value>
                public PoolDetail Detail { get; set; }
            }

            /// <summary>
            /// Rrddata
            /// </summary>
            public class RrdDataInfo<T>
            {
                /// <summary>
                /// RrdData Day
                /// </summary>
                public IEnumerable<T> Day { get; set; } = Enumerable.Empty<T>();

                /// <summary>
                /// RrdData week
                /// </summary>
                public IEnumerable<T> Week { get; set; } = Enumerable.Empty<T>();
            }


            /// <summary>
            /// Firewall
            /// </summary>
            public class FirewallInfo
            {
                /// <summary>
                /// Aliases
                /// </summary>
                public IEnumerable<FirewallAlias> Aliases { get; set; }

                /// <summary>
                /// IpSet
                /// </summary>
                public IEnumerable<FirewallIpsetInfo> IpSets { get; set; }

                /// <summary>
                /// Refs
                /// </summary>
                public IEnumerable<FirewallRef> Refs { get; set; }

                /// <summary>
                /// Rules
                /// </summary>
                public IEnumerable<FirewallRule> Rules { get; set; }

                /// <summary>
                /// Firewall Ipset Info
                /// </summary>
                public class FirewallIpsetInfo
                {
                    /// <summary>
                    /// Ipset
                    /// </summary>
                    public FirewallIpSet Ipset { get; set; }

                    /// <summary>
                    /// Contents
                    /// </summary>
                    public IEnumerable<FirewallIpSetContent> Contents { get; set; }
                }
            }

            /// <summary>
            /// Access Info
            /// </summary>
            public class AccessInfo
            {
                /// <summary>
                /// Users
                /// </summary>
                /// <value></value>
                public IEnumerable<AccessUser> Users { get; set; }

                /// <summary>
                /// Groups
                /// </summary>
                /// <value></value>
                public IEnumerable<AccessGroup> Groups { get; set; }

                /// <summary>
                /// Roles
                /// </summary>
                /// <value></value>
                public IEnumerable<AccessRole> Roles { get; set; }

                /// <summary>
                /// Acl
                /// </summary>
                /// <value></value>
                public IEnumerable<AccessAcl> Acl { get; set; }

                /// <summary>
                /// Domains
                /// </summary>
                /// <value></value>
                public IEnumerable<AccessDomain> Domains { get; set; }
            }

            /// <summary>
            /// Cluster
            /// </summary>
            public class ClusterInfo
            {
                /// <summary>
                /// Resources
                /// </summary>
                /// <value></value>
                public IEnumerable<ClusterResource> Resources { get; set; }

                /// <summary>
                /// Status
                /// </summary>
                /// <value></value>
                public IEnumerable<ClusterStatus> Status { get; set; }

                /// <summary>
                /// Replication
                /// </summary>
                /// <value></value>
                public IEnumerable<ClusterReplication> Replication { get; set; }

                /// <summary>
                /// Backups
                /// </summary>
                /// <value></value>
                public IEnumerable<ClusterBackup> Backups { get; set; }

                /// <summary>
                /// Config
                /// </summary>
                public ConfigInfo Config { get; } = new ConfigInfo();

                /// <summary>
                /// Options
                /// </summary>
                public ClusterOptions Options { get; set; }

                /// <summary>
                /// Has
                /// </summary>
                public HaInfo Ha { get; set; } = new();

                /// <summary>
                /// Firewall
                /// </summary>
                public ClusterFirewallInfo Firewall { get; set; } = new ClusterFirewallInfo();

                /// <summary>
                /// Firewall
                /// </summary>
                public class ClusterFirewallInfo : FirewallInfo
                {
                    /// <summary>
                    /// Options
                    /// </summary>
                    public ClusterFirewallOptions Options { get; set; }

                    /// <summary>
                    /// Groups
                    /// </summary>
                    public IEnumerable<GroupInfo> Groups { get; set; }

                    /// <summary>
                    /// Group
                    /// </summary>
                    public class GroupInfo
                    {
                        /// <summary>
                        /// Groups
                        /// </summary>
                        public ClusterFirewallGroup Group { get; set; }

                        /// <summary>
                        /// Rules
                        /// </summary>
                        public IEnumerable<FirewallRule> Rules { get; set; }
                    }
                }

                /// <summary>
                /// Ha
                /// </summary>
                public class HaInfo
                {
                    /// <summary>
                    /// Status
                    /// </summary>
                    public StatusInfo Status { get; set; } = new StatusInfo();

                    /// <summary>
                    /// Groups
                    /// </summary>
                    public IEnumerable<ClusterHaGroup> Groups { get; set; }

                    /// <summary>
                    /// Resources
                    /// </summary>
                    public IEnumerable<ClusterHaResource> Resources { get; internal set; }

                    /// <summary>
                    /// Status
                    /// </summary>
                    public class StatusInfo
                    {
                        /// <summary>
                        /// Current
                        /// </summary>
                        public IEnumerable<ClusterHaStatusCurrent> Current { get; internal set; }
                    }
                }

                /// <summary>
                /// Config
                /// </summary>
                public class ConfigInfo
                {
                    /// <summary>
                    /// Config nodes
                    /// </summary>
                    public IEnumerable<ClusterConfigNode> Nodes { get; set; }

                    /// <summary>
                    /// Config Qdevice
                    /// </summary>
                    public ClusterConfigQDevice Qdevice { get; set; }

                    /// <summary>
                    /// Totem
                    /// </summary>
                    public ClusterConfigTotem Totem { get; set; }

                    /// <summary>
                    /// Join
                    /// </summary>
                    public ClusterConfigJoin Join { get; set; }

                    /// <summary>
                    /// Api Version
                    /// </summary>
                    public string ApiVersion { get; set; }
                }
            }

            /// <summary>
            /// Node Info
            /// </summary>
            public class NodeInfo
            {
                /// <summary>
                /// Node item
                /// </summary>
                public NodeItem Detail { get; set; }

                /// <summary>
                /// Status
                /// </summary>
                /// <value></value>
                public NodeStatus Status { get; set; }

                /// <summary>
                /// Disks
                /// </summary>
                /// <returns></returns>
                public DisksInfo Disks { get; set; } = new DisksInfo();

                /// <summary>
                /// Dns
                /// </summary>
                /// <value></value>
                public NodeDns Dns { get; set; }

                /// <summary>
                /// Hosts
                /// </summary>
                /// <value></value>
                public IEnumerable<string> Hosts { get; set; }

                /// <summary>
                /// Netstat
                /// </summary>
                /// <value></value>
                public IEnumerable<NodeNetstat> Netstat { get; set; }

                /// <summary>
                /// Network
                /// </summary>
                /// <value></value>
                public IEnumerable<NodeNetwork> Network { get; set; }

                /// <summary>
                /// Services
                /// </summary>
                /// <value></value>
                public IEnumerable<NodeService> Services { get; set; }

                /// <summary>
                /// Subscription
                /// </summary>
                /// <value></value>
                public NodeSubscription Subscription { get; set; }

                /// <summary>
                /// Version
                /// </summary>
                /// <value></value>
                public NodeVersion Version { get; set; }

                /// <summary>
                /// Tasks
                /// </summary>
                /// <value></value>
                public IEnumerable<NodeTask> Tasks { get; set; }

                /// <summary>
                /// Report
                /// </summary>
                /// <value></value>
                public IEnumerable<string> Report { get; set; }

                /// <summary>
                /// Apt
                /// </summary>
                public AptInfo Apt { get; set; } = new AptInfo();

                /// <summary>
                /// Time zone
                /// </summary>
                /// <value></value>
                public string Timezone { get; set; }

                /// <summary>
                /// Rrddata
                /// </summary>
                public RrdDataInfo<NodeRrdData> RrdData { get; set; } = new RrdDataInfo<NodeRrdData>();

                /// <summary>
                /// Replication
                /// </summary>
                /// <value></value>
                public IEnumerable<NodeReplication> Replication { get; set; }

                /// <summary>
                /// Hardware
                /// </summary>
                /// <returns></returns>
                public HardwareInfo Hardware { get; set; } = new HardwareInfo();

                /// <summary>
                /// Qemu
                /// </summary>
                /// <value></value>
                public IEnumerable<QemuInfo> Qemu { get; set; }

                /// <summary>
                /// Qemu
                /// </summary>
                /// <value></value>
                public IEnumerable<LxcInfo> Lxc { get; set; }

                /// <summary>
                /// Storages
                /// </summary>
                public IEnumerable<StorageInfo> Storages { get; set; }

                /// <summary>
                /// Certificates
                /// </summary>
                public IEnumerable<NodeCertificate> Certificates { get; set; }

                /// <summary>
                /// Firewall
                /// </summary>
                public NodeFirewallInfo Firewall { get; set; } = new NodeFirewallInfo();

                /// <summary>
                /// Firewall info
                /// </summary>
                public class NodeFirewallInfo
                {
                    /// <summary>
                    /// Rules
                    /// </summary>
                    public IEnumerable<FirewallRule> Rules { get; set; }

                    /// <summary>
                    /// Options
                    /// </summary>
                    public NodeFirewallOptions Options { get; internal set; }
                }

                /// <summary>
                /// apt
                /// </summary>
                public class AptInfo
                {
                    /// <summary>
                    /// Package Versions
                    /// </summary>
                    /// <value></value>
                    public IEnumerable<NodeAptVersion> Version { get; set; }

                    /// <summary>
                    /// Apt Update
                    /// </summary>
                    /// <value></value>
                    public IEnumerable<NodeAptUpdate> Update { get; set; }
                }

                /// <summary>
                /// Node Storage Info
                /// </summary>
                public class StorageInfo
                {
                    /// <summary>
                    /// Storage Detail
                    /// </summary>
                    public NodeStorage Detail { get; set; }

                    /// <summary>
                    /// Content
                    /// </summary>
                    /// <value></value>
                    public IEnumerable<NodeStorageContent> Content { get; set; }

                    /// <summary>
                    /// Status
                    /// </summary>
                    /// <value></value>
                    public NodeStorage Status { get; set; }

                    /// <summary>
                    /// Rrddata
                    /// </summary>
                    public RrdDataInfo<NodeStorageRrdData> RrdData { get; set; } = new RrdDataInfo<NodeStorageRrdData>();
                }

                /// <summary>
                /// Node hardware
                /// </summary>
                public class HardwareInfo
                {
                    /// <summary>
                    /// Pci
                    /// </summary>
                    /// <value></value>
                    public IEnumerable<NodeHardwarePci> Pci { get; set; }

                    /// <summary>
                    /// Pci
                    /// </summary>
                    /// <value></value>
                    public IEnumerable<NodeHardwareUsb> Usb { get; set; }
                }

                /// <summary>
                /// Node Disk Info
                /// </summary>
                public class DisksInfo
                {
                    /// <summary>
                    /// Disks
                    /// </summary>
                    /// <value></value>
                    public IEnumerable<DiskInfo> List { get; set; }

                    /// <summary>
                    /// Zfs
                    /// </summary>
                    /// <value></value>
                    public IEnumerable<ZfsInfo> Zfs { get; set; }

                    /// <summary>
                    /// Node disk
                    /// </summary>
                    public class DiskInfo
                    {
                        /// <summary>
                        /// List
                        /// </summary>
                        /// <value></value>
                        public NodeDiskList Disk { get; set; }

                        /// <summary>
                        /// List
                        /// </summary>
                        /// <value></value>
                        public NodeDiskSmart Smart { get; set; }

                        /// <summary>
                        /// Partitions
                        /// </summary>
                        public IEnumerable<NodeDiskList> Partitions { get; set; }
                    }

                    /// <summary>
                    /// Node disk zfs
                    /// </summary>
                    public class ZfsInfo
                    {
                        /// <summary>
                        /// Zfs
                        /// </summary>
                        /// <value></value>
                        public NodeDiskZfs Zfs { get; set; }

                        /// <summary>
                        /// Zfs Details
                        /// </summary>
                        /// <value></value>
                        public NodeDiskZfsDetail Detail { get; set; }
                    }
                }

                /// <summary>
                /// Node Vm Info
                /// </summary>
                public abstract class VmBaseInfo<TDetail, TConfig>
                    where TDetail : NodeVmBase
                    where TConfig : VmConfig
                {
                    /// <summary>
                    /// Config
                    /// </summary>
                    /// <value></value>
                    public TConfig Config { get; set; }

                    /// <summary>
                    /// Rrddata
                    /// </summary>
                    public RrdDataInfo<VmRrdData> RrdData { get; set; } = new RrdDataInfo<VmRrdData>();

                    /// <summary>
                    /// Snapshots
                    /// </summary>
                    /// <value></value>
                    public IEnumerable<VmSnapshot> Snapshots { get; set; }

                    /// <summary>
                    /// Detail
                    /// </summary>
                    public TDetail Detail { get; set; }

                    /// <summary>
                    /// Firewall
                    /// </summary>
                    public VmFirewallInfo Firewall { get; set; } = new VmFirewallInfo();

                    /// <summary>
                    /// Pending
                    /// </summary>
                    public IEnumerable<KeyValue> Pending { get; set; }

                    /// <summary>
                    /// Firewall vm
                    /// </summary>
                    public class VmFirewallInfo : FirewallInfo
                    {
                        /// <summary>
                        /// Options
                        /// </summary>
                        public VmFirewallOptions Options { get; set; }
                    }
                }

                /// <summary>
                /// Node Vm Lxc
                /// </summary>
                public class LxcInfo : VmBaseInfo<NodeVmLxc, VmConfigLxc>
                {
                    /// <summary>
                    /// Status
                    /// </summary>
                    public VmLxcStatusCurrent Status { get; set; }
                }

                /// <summary>
                /// Node Vm Qemu
                /// </summary>
                public class QemuInfo : VmBaseInfo<NodeVmQemu, VmConfigQemu>
                {
                    /// <summary>
                    /// Qemu Agent
                    /// </summary>
                    /// <value></value>
                    public AgentInfo Agent { get; set; } = new AgentInfo();

                    /// <summary>
                    /// Status current
                    /// </summary>
                    public VmQemuStatusCurrent Status { get; set; }

                    /// <summary>
                    /// Qemu Agent
                    /// </summary>
                    public class AgentInfo
                    {
                        /// <summary>
                        /// Qemu Agent NetworkGetInterfaces
                        /// </summary>
                        /// <value></value>
                        public VmQemuAgentNetworkGetInterfaces NetworkGetInterfaces { get; set; }

                        /// <summary>
                        /// Qemu Agent GetFsInfo
                        /// </summary>
                        public VmQemuAgentGetFsInfo GetFsInfo { get; set; }

                        /// <summary>
                        /// Qemu Agent Guest GetHostName
                        /// </summary>
                        public VmQemuAgentGetHostName GetHostName { get; set; }

                        /// <summary>
                        /// Qemu Agent Info
                        /// </summary>
                        public VmQemuAgentInfo Info { get; set; }

                        /// <summary>
                        /// Qemu Agent Get Os Info
                        /// </summary>
                        public VmQemuAgentOsInfo GetOsInfo { get; set; }

                        /// <summary>
                        /// Qemu Agent Get Virtual Cpus
                        /// </summary>
                        public VmQemuAgentGetVCpus GetVCpus { get; set; }

                        /// <summary>
                        /// Qemu Agent Get Time Zone
                        /// </summary>
                        public VmQemuAgentGetTimeZone GetTimeZone { get; set; }
                    }
                }
            }
        }

        /// <summary>
        /// Collect data from client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="removeSecurity"></param>
        /// <param name="tasksDay"></param>
        /// <param name="tasksOnlyErrors"></param>
        /// <param name="nodeReport"></param>
        public static async Task<Info> Collect(PveClient client,
                                               bool removeSecurity,
                                               int tasksDay,
                                               bool tasksOnlyErrors = true,
                                               bool nodeReport = false)
        {
            var stopwatch = Stopwatch.StartNew();

            var info = new Info
            {
                Version = new Version(2, 0, 0),
                Date = DateTime.Now
            };

            info.Access.Users = await client.Access.Users.Get();
            info.Access.Groups = await client.Access.Groups.Get();
            info.Access.Roles = await client.Access.Roles.Get();
            info.Access.Acl = await client.Access.Acl.Get();
            info.Access.Domains = await client.Access.Domains.Get();

            info.Storages = await client.Storage.Get();
            if (removeSecurity)
            {
                foreach (var item in info.Storages)
                {
                    item.Fingerprint = "REMOVED FOR SECURITY";
                }
            }

            #region Pools
            var pools = new List<Info.PoolInfo>();
            info.Pools = pools;
            foreach (var item in await client.Pools.Get())
            {
                pools.Add(new Info.PoolInfo
                {
                    Id = item.Id,
                    Comment = item.Comment,
                    Detail = await client.Pools[item.Id].Get()
                });
            }
            #endregion

            await ReadCluster(info, client);

            info.Nodes = await ReadNodes(client, removeSecurity, tasksDay, tasksOnlyErrors, nodeReport);

            stopwatch.Stop();
            info.CollectExecution = stopwatch.Elapsed;

            return info;
        }

        private static async Task ReadCluster(Info info, PveClient client)
        {
            info.Cluster.Config.Nodes = await client.Cluster.Config.Nodes.Get();
            info.Cluster.Config.Qdevice = await client.Cluster.Config.Qdevice.Get();
            info.Cluster.Config.Join = await client.Cluster.Config.Join.Get();
            info.Cluster.Config.Totem = await client.Cluster.Config.Totem.Get();
            info.Cluster.Config.ApiVersion = (await client.Cluster.Config.Apiversion.JoinApiVersion()).ToData() as string;

            info.Cluster.Ha.Status.Current = await client.Cluster.Ha.Status.Current.Get();
            info.Cluster.Ha.Groups = await client.Cluster.Ha.Groups.Get();
            info.Cluster.Ha.Resources = await client.Cluster.Ha.Resources.Get();

            info.Cluster.Firewall.Options = await client.Cluster.Firewall.Options.Get();
            info.Cluster.Firewall.Aliases = await client.Cluster.Firewall.Aliases.Get();
            info.Cluster.Firewall.Refs = await client.Cluster.Firewall.Refs.Get();
            info.Cluster.Firewall.Rules = await client.Cluster.Firewall.Rules.Get();

            var ipSets = new List<Info.FirewallInfo.FirewallIpsetInfo>();
            info.Cluster.Firewall.IpSets = ipSets;
            foreach (var item in await client.Cluster.Firewall.Ipset.Get())
            {
                ipSets.Add(new Info.FirewallInfo.FirewallIpsetInfo
                {
                    Ipset = item,
                    Contents = await client.Cluster.Firewall.Ipset[item.Name].Get()
                });
            }

            var groups = new List<Info.ClusterInfo.ClusterFirewallInfo.GroupInfo>();
            info.Cluster.Firewall.Groups = groups;
            foreach (var item in await client.Cluster.Firewall.Groups.Get())
            {
                groups.Add(new Info.ClusterInfo.ClusterFirewallInfo.GroupInfo
                {
                    Group = item,
                    Rules = await client.Cluster.Firewall.Groups[item.Group].Get()
                });
            }

            //TODO acme, backupinfo, ceph, log, metrics

            info.Cluster.Options = await client.Cluster.Options.Get();
            info.Cluster.Replication = await client.Cluster.Replication.Get();
            info.Cluster.Backups = await client.Cluster.Backup.Get();
            info.Cluster.Resources = await client.Cluster.Resources.Get();
            info.Cluster.Status = await client.Cluster.Status.Get();
        }

        private static async Task<IEnumerable<Info.NodeInfo>> ReadNodes(PveClient client,
                                                                        bool removeSecurity,
                                                                        int tasksDay,
                                                                        bool tasksOnlyErrors = true,
                                                                        bool nodeReport = false)
        {
            var dayTask = new DateTimeOffset(DateTime.Now.AddDays(-tasksDay)).ToUnixTimeSeconds();

            var nodes = new List<Info.NodeInfo>();
            foreach (var nodeItem in (await client.Nodes.Get()).Where(a => a.IsOnline).OrderBy(a => a.Node))
            {
                var nodeApi = client.Nodes[nodeItem.Node];
                var node = new Info.NodeInfo
                {
                    Detail = nodeItem,
                    Version = await nodeApi.Version.Get(),
                    Subscription = await nodeApi.Subscription.GetEx(),
                    Tasks = (await nodeApi.Tasks.Get(errors: tasksOnlyErrors, limit: 1000)).Where(a => a.StartTime >= dayTask),
                    Services = await nodeApi.Services.Get(),
                    Hosts = ((string)(await nodeApi.Hosts.GetEtcHosts()).ToData().data).Split('\n'),
                    Dns = await nodeApi.Dns.Get(),
                    Netstat = await nodeApi.Netstat.Get(),
                    Network = await nodeApi.Network.Get(),
                    Status = await nodeApi.Status.Get(),
                    Timezone = (await nodeApi.Time.Time()).ToData().timezone as string,
                    Replication = await nodeApi.Replication.Get(),
                    Certificates = await nodeApi.Certificates.Info.Get(),
                };

                node.Apt.Version = await nodeApi.Apt.Versions.Get();
                node.Apt.Update = await nodeApi.Apt.Update.Get();
                node.RrdData.Day = await nodeApi.Rrddata.Get(RrdDataTimeFrame.Day, RrdDataConsolidation.Average);
                node.RrdData.Week = await nodeApi.Rrddata.Get(RrdDataTimeFrame.Week, RrdDataConsolidation.Average);

                if (removeSecurity)
                {
                    node.Detail.SslFingerprint = "REMOVED FOR SECURITY";
                    node.Subscription.Key = "REMOVED FOR SECURITY";

                    foreach (var item in node.Certificates)
                    {
                        item.Fingerprint = "REMOVED FOR SECURITY";
                        item.Pem = "REMOVED FOR SECURITY";
                    }
                }

                if (nodeReport) { node.Report = ((string)(await nodeApi.Report.Report()).ToData()).Split('\n'); }

                #region Hardware
                node.Hardware.Pci = await nodeApi.Hardware.Pci.Get();
                node.Hardware.Usb = await nodeApi.Hardware.Usb.Get();
                #endregion

                #region Disks
                //disks
                var disks = new List<Info.NodeInfo.DisksInfo.DiskInfo>();
                node.Disks.List = disks;
                var disksAll = await nodeApi.Disks.List.Get(include_partitions: true);
                foreach (var item in disksAll.Where(a => string.IsNullOrWhiteSpace(a.Parent)))
                {
                    disks.Add(new Info.NodeInfo.DisksInfo.DiskInfo
                    {
                        Disk = item,
                        Partitions = disksAll.Where(a => a.Type == "partition" && a.Parent == item.DevPath).ToList(),
                        Smart = await nodeApi.Disks.Smart.Get(item.DevPath)
                    });
                }

                //zfs
                var zfs = new List<Info.NodeInfo.DisksInfo.ZfsInfo>();
                node.Disks.Zfs = zfs;
                foreach (var item in await nodeApi.Disks.Zfs.Get() ?? Enumerable.Empty<NodeDiskZfs>())
                {
                    zfs.Add(new Info.NodeInfo.DisksInfo.ZfsInfo
                    {
                        Zfs = item,
                        Detail = await nodeApi.Disks.Zfs[item.Name].Get()
                    });
                }

                //TODO directory,lvm,lvmthin
                #endregion

                node.Firewall.Options = await nodeApi.Firewall.Options.Get();
                node.Firewall.Rules = await nodeApi.Firewall.Rules.Get();

                //TODO cpu, aplinfo, capabilities, ceph, config,  journal, syslog

                node.Storages = await ReadStorages(nodeApi);
                node.Lxc = await ReadLxc(nodeApi);
                node.Qemu = await ReadQemu(nodeApi, removeSecurity);

                nodes.Add(node);
            }

            return nodes;
        }

        private static async Task<IEnumerable<Info.NodeInfo.StorageInfo>> ReadStorages(PveClient.PveNodes.PveNodeItem nodeApi)
        {
            var storages = new List<Info.NodeInfo.StorageInfo>();
            foreach (var item in (await nodeApi.Storage.Get()).OrderBy(a => a.Storage))
            {
                var storageNode = nodeApi.Storage[item.Storage];
                var storage = new Info.NodeInfo.StorageInfo
                {
                    Detail = item,
                    Status = await storageNode.Status.Get(),
                    Content = item.Active
                                    ? await storageNode.Content.Get()
                                    : new List<NodeStorageContent>()
                };

                if (storage.Detail.Enabled)
                {
                    storage.RrdData.Day = await storageNode.Rrddata.Get(RrdDataTimeFrame.Day, RrdDataConsolidation.Average);
                    storage.RrdData.Week = await storageNode.Rrddata.Get(RrdDataTimeFrame.Week, RrdDataConsolidation.Average);
                }

                storages.Add(storage);
            }
            return storages;
        }

        private static async Task<IEnumerable<Info.NodeInfo.LxcInfo>> ReadLxc(PveClient.PveNodes.PveNodeItem nodeApi)
        {
            var vms = new List<Info.NodeInfo.LxcInfo>();
            foreach (var item in (await nodeApi.Lxc.Get()).OrderBy(a => a.VmId))
            {
                var vmApi = nodeApi.Lxc[item.VmId];
                var vm = new Info.NodeInfo.LxcInfo
                {
                    Detail = item,
                    Status = await vmApi.Status.Current.Get(),
                    Config = await vmApi.Config.Get(),
                    Snapshots = await vmApi.Snapshot.Get(),
                    Pending = await vmApi.Pending.Get(),
                };

                #region Firewall
                vm.Firewall.Options = await vmApi.Firewall.Options.Get();
                vm.Firewall.Aliases = await vmApi.Firewall.Aliases.Get();
                vm.Firewall.Rules = await vmApi.Firewall.Rules.Get();
                vm.Firewall.Refs = await vmApi.Firewall.Refs.Get();

                var ipSets = new List<Info.FirewallInfo.FirewallIpsetInfo>();
                vm.Firewall.IpSets = ipSets;
                foreach (var itemIpset in await vmApi.Firewall.Ipset.Get())
                {
                    ipSets.Add(new Info.FirewallInfo.FirewallIpsetInfo
                    {
                        Ipset = itemIpset,
                        Contents = await vmApi.Firewall.Ipset[itemIpset.Name].Get()
                    });
                }
                #endregion

                vm.RrdData.Day = await vmApi.Rrddata.Get(RrdDataTimeFrame.Day, RrdDataConsolidation.Average);
                vm.RrdData.Week = await vmApi.Rrddata.Get(RrdDataTimeFrame.Week, RrdDataConsolidation.Average);

                vms.Add(vm);
            }
            return vms;
        }

        private static async Task<IEnumerable<Info.NodeInfo.QemuInfo>> ReadQemu(PveClient.PveNodes.PveNodeItem nodeApi, bool removeSecurity)
        {
            var vms = new List<Info.NodeInfo.QemuInfo>();
            foreach (var item in (await nodeApi.Qemu.Get()).OrderBy(a => a.VmId))
            {
                var vmApi = nodeApi.Qemu[item.VmId];
                var vm = new Info.NodeInfo.QemuInfo
                {
                    Detail = item,
                    Status = await vmApi.Status.Current.Get(),
                    Config = await vmApi.Config.Get(),
                    Snapshots = await vmApi.Snapshot.Get(),
                    Pending = await vmApi.Pending.Get(),
                };

                #region Firewall
                vm.Firewall.Options = await vmApi.Firewall.Options.Get();
                vm.Firewall.Aliases = await vmApi.Firewall.Aliases.Get();
                vm.Firewall.Rules = await vmApi.Firewall.Rules.Get();
                vm.Firewall.Refs = await vmApi.Firewall.Refs.Get();

                var ipSets = new List<Info.FirewallInfo.FirewallIpsetInfo>();
                vm.Firewall.IpSets = ipSets;
                foreach (var itemIpset in await vmApi.Firewall.Ipset.Get())
                {
                    ipSets.Add(new Info.FirewallInfo.FirewallIpsetInfo
                    {
                        Ipset = itemIpset,
                        Contents = await vmApi.Firewall.Ipset[itemIpset.Name].Get()
                    });
                }
                #endregion

                vm.RrdData.Day = await vmApi.Rrddata.Get(RrdDataTimeFrame.Day, RrdDataConsolidation.Average);
                vm.RrdData.Week = await vmApi.Rrddata.Get(RrdDataTimeFrame.Week, RrdDataConsolidation.Average);

                //TODO agent get-memory-block-info,get-memory-blocks,get-time,get-users
                // vm.Agent.GetFsInfo = await qemuApi.Agent.GetFsinfo.Get();

                try
                {
                    vm.Agent.GetHostName = await vmApi.Agent.GetHostName.Get();
                }
                catch //(PveExceptionResult ex)
                { }

                // vm.Agent.NetworkGetInterfaces = await qemuApi.Agent.NetworkGetInterfaces.Get();
                // vm.Agent.Info = await qemuApi.Agent.Info.Get();
                // vm.Agent.GetOsInfo = await qemuApi.Agent.GetOsinfo.Get();
                // vm.Agent.GetVCpus = await qemuApi.Agent.GetVcpus.Get();
                // vm.Agent.GetTimeZone = await qemuApi.Agent.GetTimezone.Get();

                if (removeSecurity) { vm.Config.Description = "REMOVED FOR SECURITY"; }

                vms.Add(vm);
            }
            return vms;
        }
    }
}