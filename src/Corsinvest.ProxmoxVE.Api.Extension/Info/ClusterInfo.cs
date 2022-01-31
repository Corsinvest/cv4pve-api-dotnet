/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;

namespace Corsinvest.ProxmoxVE.Api.Extension.Info
{
    /// <summary>
    /// Global Info
    /// </summary>
    public class ClusterInfo
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
        /// Resources
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Resources { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Status { get; set; }

        /// <summary>
        /// Replication
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Replication { get; set; }

        /// <summary>
        /// Backups
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Backups { get; set; }

        /// <summary>
        /// Config
        /// </summary>
        /// <returns></returns>
        public ClusterConfig Config { get; set; } = new ClusterConfig();

        /// <summary>
        /// Firewall
        /// </summary>
        /// <returns></returns>
        public ClusterFirewall Firewall { get; set; } = new ClusterFirewall();

        /// <summary>
        /// Config
        /// </summary>
        /// <returns></returns>
        public ClusterHA HA { get; set; } = new ClusterHA();

        /// <summary>
        /// Options
        /// </summary>
        /// <value></value>
        public dynamic Options { get; set; }

        /// <summary>
        /// Storage
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Storage { get; set; }

        /// <summary>
        /// Permissions
        /// </summary>
        /// <returns></returns>
        public ClusterPermissions Permissions { get; set; } = new ClusterPermissions();

        /// <summary>
        /// Pools
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Pools { get; set; }

        /// <summary>
        /// Collect data from client
        /// </summary>
        /// <param name="client"></param>
        public async Task Collect(PveClient client)
        {
            var stopwatch = Stopwatch.StartNew();

            Version = new Version(1, 0, 0);
            Date = DateTime.Now;

            Pools = (await client.Pools.Index()).ToEnumerable();

            Permissions.Users = (await client.Access.Users.Index()).ToEnumerable();
            Permissions.Groups = (await client.Access.Groups.Index()).ToEnumerable();
            Permissions.Roles = (await client.Access.Roles.Index()).ToEnumerable();
            Permissions.Acl = (await client.Access.Acl.ReadAcl()).ToEnumerable();
            Permissions.Domains = (await client.Access.Domains.Index()).ToEnumerable();

            //client.Cluster.Ceph.Flags

            Storage = (await client.Storage.Index()).ToEnumerable();

            Replication = (await client.Cluster.Replication.Index()).ToEnumerable();
            Config.Totem = (await client.Cluster.Config.Totem.Totem()).ToData();
            Config.Nodes = (await client.Cluster.Config.Nodes.Nodes()).ToEnumerable();
            Backups = (await client.Cluster.Backup.Index()).ToEnumerable();
            Resources = (await client.Cluster.Resources.Resources()).ToEnumerable();
            Status = (await client.Cluster.Status.GetStatus()).ToEnumerable();

            Options = (await client.Cluster.Options.GetOptions()).ToData();

            var status = await client.Cluster.Ha.Status.Current.Status();
            HA.Status = status.IsSuccessStatusCode
                                ? (await client.Cluster.Ha.Status.Current.Status()).ToEnumerable()
                                : new List<dynamic>();

            HA.Resources = (await client.Cluster.Ha.Resources.Index()).ToEnumerable();
            HA.Groups = (await client.Cluster.Ha.Groups.Index()).ToEnumerable();

            Firewall.Aliases = (await client.Cluster.Firewall.Aliases.GetAliases()).ToEnumerable();
            Firewall.Groups = (await client.Cluster.Firewall.Groups.ListSecurityGroups()).ToEnumerable();
            Firewall.Ipset = (await client.Cluster.Firewall.Ipset.IpsetIndex()).ToEnumerable();
            Firewall.Macros = (await client.Cluster.Firewall.Macros.GetMacros()).ToEnumerable();
            Firewall.Options = (await client.Cluster.Firewall.Options.GetOptions()).ToData();
            Firewall.Refs = (await client.Cluster.Firewall.Refs.Refs()).ToEnumerable();
            Firewall.Rules = (await client.Cluster.Firewall.Rules.GetRules()).ToEnumerable();

            var nodeList = new List<Node>();
            foreach (var nodeItem in (await client.Nodes.Index()).ToEnumerable())
            {
                var node = client.Nodes[nodeItem.node as string];
                var nodeDetail = new Node();
                // { Item = nodeItem, };
                //nodeDetail.Item.ssl_fingerprint = "REMOVED FOR SECURITY";

                var nodeResource = Resources.Where(a => a.type == "node" && a.node == nodeItem.node).FirstOrDefault();
                nodeResource.Detail = nodeDetail;
                if (nodeItem.status != "online") { continue; }

                //nodeResult.Report = node.Report.Report().ToData();
                nodeDetail.PackageVersions = (await node.Apt.Versions.Versions()).ToEnumerable();
                nodeDetail.AptUpdate = (await node.Apt.Update.ListUpdates()).ToEnumerable();
                nodeDetail.Status = (await node.Status.Status()).ToData();
                nodeDetail.Services = (await node.Services.Index()).ToEnumerable();
                nodeDetail.Subscription = (await node.Subscription.Get()).ToData();
                nodeDetail.Subscription.key = "REMOVED FOR SECURITY";

                nodeDetail.Firewall.Options = (await node.Firewall.Options.GetOptions()).ToData();
                nodeDetail.Firewall.Rules = (await node.Firewall.Rules.GetRules()).ToEnumerable();

                nodeDetail.Version = (await node.Version.Version()).ToData();

                nodeDetail.Certificates = (await node.Certificates.Info.Info()).ToEnumerable();
                foreach (var item in nodeDetail.Certificates)
                {
                    item.fingerprint = "REMOVED FOR SECURITY";
                    item.pem = "REMOVED FOR SECURITY";
                }

                //ceph
                nodeDetail.Ceph.Status = (await node.Ceph.Status.Status()).ToData();
                if (nodeDetail.Ceph.Status != null)
                {
                    nodeDetail.Ceph.Config = (await node.Ceph.Config.Config()).ToData();
                    nodeDetail.Ceph.Crush = (await node.Ceph.Crush.Crush()).ToData();
                    //nodeDetail.Ceph.Disks = node.Ceph.Disks.Disks().ToEnumerable();
                    //nodeDetail.Ceph.Flags = node.Ceph.Flags.GetFlags().ToData();
                    nodeDetail.Ceph.Mon = (await node.Ceph.Mon.Listmon()).ToEnumerable();
                    nodeDetail.Ceph.Osd = (await node.Ceph.Osd.Index()).ToData();
                    nodeDetail.Ceph.Fs = (await node.Ceph.Fs.Index()).ToData();
                    nodeDetail.Ceph.Pools = (await node.Ceph.Pools.Lspools()).ToEnumerable();
                    nodeDetail.Ceph.Rules = (await node.Ceph.Rules.Rules()).ToEnumerable();
                    nodeDetail.Ceph.Mds = (await node.Ceph.Mds.Index()).ToEnumerableOrDefault();
                    nodeDetail.Ceph.Mgr = (await node.Ceph.Mgr.Index()).ToEnumerableOrDefault();
                }

                //last 2 days
                nodeDetail.Tasks = (await node.Tasks.NodeTasks(errors: true, limit: 1000))
                                             .ToEnumerable()
                                             .Where(a => a.starttime >= DateTimeUnixHelper.ConvertToUnixTime(DateTime.Now.AddDays(-2)));
                nodeDetail.Replication = (await node.Replication.Status()).ToEnumerable();
                nodeDetail.Hardware.Pci = (await node.Hardware.Pci.Pciscan()).ToEnumerable();

                var diskList = (await node.Disks.List.List()).ToEnumerable();
                foreach (var item in diskList)
                {
                    Result result = await node.Disks.Smart.Smart(item.devpath);
                    item.Smart = result.ToData();
                }

                nodeDetail.Disks.List = diskList;
                nodeDetail.Disks.Directory = (await node.Disks.Directory.Index()).ToEnumerable();
                nodeDetail.Disks.Lvm = (await node.Disks.Lvm.Index()).ToData();
                nodeDetail.Disks.Lvmthin = (await node.Disks.Lvmthin.Index()).ToEnumerable();

                var diskZfs = (await node.Disks.Zfs.Index()).ToEnumerable();
                foreach (var item in diskZfs)
                {
                    Result result = await node.Disks.Zfs[item.name].Detail();
                    item.Detail = result.ToData();
                }
                nodeDetail.Disks.Zfs = diskZfs;

                nodeDetail.Dns = (await node.Dns.Dns()).ToData();
                nodeDetail.Hosts = (await node.Hosts.GetEtcHosts()).ToData().data;
                nodeDetail.Netstat = (await node.Netstat.Netstat()).ToEnumerable();
                nodeDetail.Network = (await node.Network.Index()).ToEnumerable();
                nodeDetail.Timezone = (await node.Time.Time()).ToData().timezone as string;

                await ReadRrdData(nodeDetail.RrdData, node.Rrddata);

                //get storage for backups
                var storageWithBackups = (await node.Storage.Index(format: true, content: "backup"))
                                                            .ToEnumerable()
                                                            .Where(a => a.active == 1)
                                                            .Select(a => a.storage as string);

                async Task SetCommonVM(Vm vmDetail, dynamic vm, int vmId, string type)
                {
                    vmDetail.Config.description = "REMOVED FOR SECURITY";

                    await ReadRrdData(vmDetail.RrdData, vm.Rrddata);

                    vmDetail.Replication = (await node.Replication.Status(vmId)).ToEnumerable();

                    vmDetail.Tasks = (await node.Tasks.NodeTasks(errors: true, limit: 1000, vmid: vmId))
                                               .ToEnumerable()
                                               .Where(a => a.starttime >= DateTimeUnixHelper.ConvertToUnixTime(DateTime.Now.AddDays(-2)));

                    var backups = new List<dynamic>();
                    foreach (var item in storageWithBackups)
                    {
                        backups.AddRange((await node.Storage[item].Content.Index("backup", vmId)).ToEnumerable());
                    }
                    vmDetail.Backups = backups;

                    vmDetail.Permissions = Permissions.Acl.Where(a => a.path == $"/vms/{vmId}").ToList();

                    //firewall
                    vmDetail.Firewall.Aliases = ((Result) await ((dynamic)vm).Firewall.Aliases.GetAliases()).ToEnumerable();
                    vmDetail.Firewall.Ipset = ((Result) await ((dynamic)vm).Firewall.Ipset.IpsetIndex()).ToEnumerable();
                    vmDetail.Firewall.Options =  ((Result)await ((dynamic)vm).Firewall.Options.GetOptions()).ToData();
                    vmDetail.Firewall.Refs = ((Result) await ((dynamic)vm).Firewall.Refs.Refs()).ToEnumerable();
                    vmDetail.Firewall.Refs = ((Result) await ((dynamic)vm).Firewall.Rules.GetRules()).ToEnumerable();

                    var vmResource = Resources.Where(a => a.type == type && a.vmid == vmId).FirstOrDefault();
                    vmResource.Detail = vmDetail;
                }

                //lxc
                foreach (var vmItem in (await node.Lxc.Vmlist()).ToEnumerable())
                {
                    int vmId = Convert.ToInt32(vmItem.vmid);
                    var vm = node.Lxc[vmId];
                    var vmDetail = new Vm
                    {
                        Config = (await vm.Config.VmConfig()).ToData(),
                        Snapshots = (await vm.Snapshot.List()).ToEnumerable(),
                        Status = (await vm.Status.Current.VmStatus()).ToData()
                    };

                    await SetCommonVM(vmDetail, vm, vmId, "lxc");
                }

                //qemu
                foreach (var vmItem in (await node.Qemu.Vmlist()).ToEnumerable())
                {
                    int vmId = Convert.ToInt32(vmItem.vmid);
                    var vm = node.Qemu[vmId];
                    var vmDetail = new Vm
                    {
                        Config = (await vm.Config.VmConfig()).ToData(),
                        AgentGuestRunning = (await vm.Agent.Network_Get_Interfaces.Network_Get_Interfaces()).ToData() != null,
                        Snapshots = (await vm.Snapshot.SnapshotList()).ToEnumerable(),
                        Status = (await vm.Status.Current.VmStatus()).ToData()
                    };

                    await SetCommonVM(vmDetail, vm, vmId, "qemu");
                }

                //storages
                foreach (var storageItem in (await node.Storage.Index()).ToEnumerable()
                                                                .Where(a => a.enabled == 1 && a.active == 1))
                {
                    var storageNode = node.Storage[storageItem.storage as string];
                    var storeageDetail = new Storage
                    {
                        Content = (await storageNode.Content.Index()).ToData(),
                        Status = (await storageNode.Status.ReadStatus()).ToData(),
                    };

                    await ReadRrdData(storeageDetail.RrdData, storageNode.Rrddata);

                    storeageDetail.Permissions = Permissions.Acl.Where(a => a.path == $"/storage/{storageItem.storage}")
                                                                .ToList();

                    var storageResource = Resources.Where(a => a.type == "storage" &&
                                                               a.node == nodeItem.node &&
                                                               a.storage == storageItem.storage)
                                                   .FirstOrDefault();
                    storageResource.Detail = storeageDetail;
                }

                nodeList.Add(nodeDetail);
            }

            stopwatch.Stop();
            CollectExecution = stopwatch.Elapsed;
        }

        private static async Task ReadRrdData(RrdData rrdDataItem, dynamic itemToRead)
        {
            rrdDataItem.Day = ((Result)await itemToRead.Rrddata("day", "AVERAGE")).ToEnumerable();
            rrdDataItem.Week = ((Result)await itemToRead.Rrddata("week", "AVERAGE")).ToEnumerable();
        }
    }
}