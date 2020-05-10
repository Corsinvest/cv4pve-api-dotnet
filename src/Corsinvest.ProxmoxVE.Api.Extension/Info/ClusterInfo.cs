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
        public void Collect(PveClient client)
        {
            var stopwatch = Stopwatch.StartNew();

            Version = new Version(1, 0, 0);
            Date = DateTime.Now;

            Pools = client.Pools.Index().ToEnumerable();

            Permissions.Users = client.Access.Users.Index().ToEnumerable();
            Permissions.Groups = client.Access.Groups.Index().ToEnumerable();
            Permissions.Roles = client.Access.Roles.Index().ToEnumerable();
            Permissions.Acl = client.Access.Acl.ReadAcl().ToEnumerable();
            Permissions.Domains = client.Access.Domains.Index().ToEnumerable();

            Storage = client.Storage.Index().ToEnumerable();

            Replication = client.Cluster.Replication.Index().ToEnumerable();
            Config.Totem = client.Cluster.Config.Totem.Totem().Response.data;
            Config.Nodes = client.Cluster.Config.Nodes.Nodes().ToEnumerable();
            Backups = client.Cluster.Backup.Index().ToEnumerable();
            Resources = client.Cluster.Resources.Resources().ToEnumerable();
            Status = client.Cluster.Status.GetStatus().ToEnumerable();

            Options = client.Cluster.Options.GetOptions().Response.data;

            HA.Status = client.Cluster.Ha.Status.Current.Status().ToEnumerable();
            HA.Resources = client.Cluster.Ha.Resources.Index().ToEnumerable();
            HA.Groups = client.Cluster.Ha.Groups.Index().ToEnumerable();

            Firewall.Aliases = client.Cluster.Firewall.Aliases.GetAliases().ToEnumerable();
            Firewall.Groups = client.Cluster.Firewall.Groups.ListSecurityGroups().ToEnumerable();
            Firewall.Ipset = client.Cluster.Firewall.Ipset.IpsetIndex().ToEnumerable();
            Firewall.Macros = client.Cluster.Firewall.Macros.GetMacros().ToEnumerable();
            Firewall.Options = client.Cluster.Firewall.Options.GetOptions().Response.data;
            Firewall.Refs = client.Cluster.Firewall.Refs.Refs().ToEnumerable();
            Firewall.Rules = client.Cluster.Firewall.Rules.GetRules().ToEnumerable();

            var nodeList = new List<Node>();
            foreach (var nodeItem in client.Nodes.Index().ToEnumerable())
            {
                var node = client.Nodes[nodeItem.node as string];
                var nodeDetail = new Node();
                // { Item = nodeItem, };
                //nodeDetail.Item.ssl_fingerprint = "REMOVED FOR SECURITY";

                var nodeResource = Resources.Where(a => a.type == "node" &&
                                                        a.node == nodeItem.node)
                                            .FirstOrDefault();
                nodeResource.Detail = nodeDetail;
                if (nodeItem.status != "online") { continue; }

                //nodeResult.Report = node.Report.Report().Response.data;
                nodeDetail.PackageVersions = node.Apt.Versions.Versions().ToEnumerable();
                nodeDetail.AptUpdate = node.Apt.Update.ListUpdates().ToEnumerable();
                nodeDetail.Status = node.Status.Status().Response.data;
                nodeDetail.Services = node.Services.Index().ToEnumerable();
                nodeDetail.Subscription = node.Subscription.Get().Response.data;
                nodeDetail.Subscription.key = "REMOVED FOR SECURITY";

                nodeDetail.Firewall.Options = node.Firewall.Options.GetOptions().Response.data;
                nodeDetail.Firewall.Rules = node.Firewall.Rules.GetRules().ToEnumerable();

                nodeDetail.Version = node.Version.Version().Response.data;

                nodeDetail.Certificates = node.Certificates.Info.Info().ToEnumerable();
                foreach (var item in nodeDetail.Certificates)
                {
                    item.fingerprint = "REMOVED FOR SECURITY";
                    item.pem = "REMOVED FOR SECURITY";
                }

                //ceph
                nodeDetail.Ceph.Status = node.Ceph.Status.Status().Response.data;
                if (nodeDetail.Ceph.Status != null)
                {
                    nodeDetail.Ceph.Config = node.Ceph.Config.Config().Response.data;
                    nodeDetail.Ceph.Crush = node.Ceph.Crush.Crush().Response.data;
                    //nodeDetail.Ceph.Disks = node.Ceph.Disks.Disks().ToEnumerable();
                    nodeDetail.Ceph.Flags = node.Ceph.Flags.GetFlags().Response.data;
                    nodeDetail.Ceph.Mon = node.Ceph.Mon.Listmon().ToEnumerable();
                    nodeDetail.Ceph.Osd = node.Ceph.Osd.Index().Response.data;
                    nodeDetail.Ceph.Fs = node.Ceph.Fs.Index().Response.data;
                    nodeDetail.Ceph.Pools = node.Ceph.Pools.Lspools().ToEnumerable();
                    nodeDetail.Ceph.Rules = node.Ceph.Rules.Rules().ToEnumerable();
                    nodeDetail.Ceph.Mds = node.Ceph.Mds.Index().ToEnumerableOrDefault();
                    nodeDetail.Ceph.Mgr = node.Ceph.Mgr.Index().ToEnumerableOrDefault();
                }

                //last 2 days
                nodeDetail.Tasks = node.Tasks.NodeTasks(errors: true, limit: 1000)
                                        .ToEnumerable().Where(a => a.starttime >= DateTimeUnixHelper.ConvertToUnixTime(DateTime.Now.AddDays(-2)));
                nodeDetail.Replication = node.Replication.Status().ToEnumerable();
                nodeDetail.Hardware.Pci = node.Hardware.Pci.Pciscan().ToEnumerable();

                var diskList = node.Disks.List.List().ToEnumerable();
                foreach (var item in diskList)
                {
                    item.Smart = node.Disks.Smart.Smart(item.devpath).Response.data;
                }

                nodeDetail.Disks.List = diskList;
                nodeDetail.Disks.Directory = node.Disks.Directory.Index().ToEnumerable();
                nodeDetail.Disks.Lvm = node.Disks.Lvm.Index().Response.data;
                nodeDetail.Disks.Lvmthin = node.Disks.Lvmthin.Index().ToEnumerable();

                var diskZfs = node.Disks.Zfs.Index().ToEnumerable();
                foreach (var item in diskZfs)
                {
                    item.Detail = node.Disks.Zfs[item.name].Detail().Response.data;
                }
                nodeDetail.Disks.Zfs = diskZfs;

                nodeDetail.Dns = node.Dns.Dns().Response.data;
                nodeDetail.Hosts = node.Hosts.GetEtcHosts().Response.data.data;
                nodeDetail.Netstat = node.Netstat.Netstat().ToEnumerable();
                nodeDetail.Network = node.Network.Index().ToEnumerable();
                nodeDetail.Timezone = node.Time.Time().Response.data.timezone as string;

                ReadRrdData(nodeDetail.RrdData, node.Rrddata);

                //get storage for backups
                var storageWithBackups = node.Storage.Index(format: true, content: "backup")
                                                     .ToEnumerable()
                                                     .Select(a => a.storage as string);

                void SetCommonVM(Vm vmDetail, dynamic vm, int vmId, string type)
                {
                    vmDetail.Config.description = "REMOVED FOR SECURITY";

                    ReadRrdData(vmDetail.RrdData, vm.Rrddata);

                    vmDetail.Replication = node.Replication.Status(vmId).ToEnumerable();

                    vmDetail.Tasks = node.Tasks.NodeTasks(errors: true, limit: 1000, vmid: vmId)
                                               .ToEnumerable().Where(a => a.starttime >= DateTimeUnixHelper.ConvertToUnixTime(DateTime.Now.AddDays(-2)));

                    vmDetail.Backups = storageWithBackups.SelectMany(a => node.Storage[a].Content.Index("backup", vmId).ToEnumerable());

                    vmDetail.Permissions = Permissions.Acl.Where(a => a.path == $"/vms/{vmId}").ToList();

                    //firewall
                    vmDetail.Firewall.Aliases = ((Result)vm.Firewall.Aliases.GetAliases()).ToEnumerable();
                    vmDetail.Firewall.Ipset = ((Result)vm.Firewall.Ipset.IpsetIndex()).ToEnumerable();
                    vmDetail.Firewall.Options = vm.Firewall.Options.GetOptions().Response.data;
                    vmDetail.Firewall.Refs = ((Result)vm.Firewall.Refs.Refs()).ToEnumerable();
                    vmDetail.Firewall.Refs = ((Result)vm.Firewall.Rules.GetRules()).ToEnumerable();

                    var vmResource = Resources.Where(a => a.type == type &&
                                                          a.vmid == vmId)
                                              .FirstOrDefault();
                    vmResource.Detail = vmDetail;
                }

                //lxc
                foreach (var vmItem in node.Lxc.Vmlist().ToEnumerable())
                {
                    var vm = node.Lxc[vmItem.vmid as string];
                    var vmDetail = new Vm
                    {
                        Config = vm.Config.VmConfig().Response.data,
                        Snapshots = vm.Snapshot.List().ToEnumerable(),
                        Status = vm.Status.Current.VmStatus().Response.data,
                    };

                    SetCommonVM(vmDetail, vm, int.Parse(vmItem.vmid as string), "lxc");
                }

                //qemu
                foreach (var vmItem in node.Qemu.Vmlist().ToEnumerable())
                {
                    var vm = node.Qemu[vmItem.vmid as string];
                    var vmDetail = new Vm
                    {
                        Config = vm.Config.VmConfig().Response.data,
                        AgentGuestRunning = vm.Agent.Network_Get_Interfaces.Network_Get_Interfaces().Response.data != null,
                        Snapshots = vm.Snapshot.SnapshotList().ToEnumerable(),
                        Status = vm.Status.Current.VmStatus().Response.data,
                    };

                    SetCommonVM(vmDetail, vm, int.Parse(vmItem.vmid as string), "qemu");
                }

                //storages
                foreach (var storageItem in node.Storage.Index().ToEnumerable()
                                                                .Where(a => a.enabled == 1 && a.active == 1))
                {
                    var storageNode = node.Storage[storageItem.storage as string];
                    var storeageDetail = new Storage
                    {
                        Content = storageNode.Content.Index().Response.data,
                        Status = storageNode.Status.ReadStatus().Response.data,
                    };

                    ReadRrdData(storeageDetail.RrdData, storageNode.Rrddata);

                    storeageDetail.Permissions = this.Permissions.Acl.Where(a => a.path == $"/storage/{storageItem.storage}").ToList();

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

        private static void ReadRrdData(RrdData rrdDataItem, dynamic itemToRead)
        {
            rrdDataItem.Day = ((Result)itemToRead.Rrddata("day", "AVERAGE")).ToEnumerable();
            rrdDataItem.Week = ((Result)itemToRead.Rrddata("week", "AVERAGE")).ToEnumerable();
        }
    }
}