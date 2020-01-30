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

        // /// <summary>
        // /// Nodes
        // /// </summary>
        // /// <value></value>
        // public IEnumerable<Node> Nodes { get; set; }

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

            var nodeList = new List<Node>();
            foreach (var nodeItem in client.Nodes.Index().ToEnumerable())
            {
                var node = client.Nodes[nodeItem.node as string];
                var nodeDetail = new Node { Item = nodeItem, };

                nodeDetail.Item.ssl_fingerprint = "REMOVED FOR SECURITY";

                var nodeResource = Resources.Where(a => a.type == "node" &&
                                                        a.node == nodeItem.node)
                                            .FirstOrDefault();
                nodeResource.Detail = nodeDetail;

                //ssl_fingerprint
                if (nodeItem.status != "online") { continue; }

                //nodeResult.Report = node.Report.Report().Response.data;
                nodeDetail.PackageVersions = node.Apt.Versions.Versions().ToEnumerable();
                nodeDetail.AptUpdate = node.Apt.Update.ListUpdates().ToEnumerable();
                nodeDetail.Status = node.Status.Status().Response.data;
                nodeDetail.Services = node.Services.Index().ToEnumerable();
                nodeDetail.Subscription = node.Subscription.Get().Response.data;
                nodeDetail.Version = node.Version.Version().Response.data;

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

                //nodeDetail.RrdData.Hour = node.Rrddata.Rrddata("hour","AVERAGE").ToEnumerable();
                nodeDetail.RrdData.Day = node.Rrddata.Rrddata("day","AVERAGE").ToEnumerable();
                nodeDetail.RrdData.Week = node.Rrddata.Rrddata("week", "AVERAGE").ToEnumerable();
                //nodeDetail.RrdData.Month = node.Rrddata.Rrddata("month", "AVERAGE").ToEnumerable();
                //nodeDetail.RrdData.Year = node.Rrddata.Rrddata("year","AVERAGE").ToEnumerable();

                //lxc
                foreach (var vmItem in node.Lxc.Vmlist().ToEnumerable())
                {
                    var vm = node.Lxc[vmItem.vmid as string];
                    var vmDetail = new NodeVm
                    {
                        Item = vmItem,
                        Config = vm.Config.VmConfig().Response.data,
                        Snapshots = vm.Snapshot.List().ToEnumerable(),
                        Status = vm.Status.Current.VmStatus().Response.data,
                    };

                    vmDetail.Config.description = "REMOVED FOR SECURITY";
                    //vmDetail.RrdData.Hour = vm.Rrddata.Rrddata("hour","AVERAGE").ToEnumerable();
                    vmDetail.RrdData.Day = vm.Rrddata.Rrddata("day","AVERAGE").ToEnumerable();
                    vmDetail.RrdData.Week = vm.Rrddata.Rrddata("week", "AVERAGE").ToEnumerable();
                    //vmDetail.RrdData.Month = vm.Rrddata.Rrddata("month", "AVERAGE").ToEnumerable();
                    //vmDetail.RrdData.Year = vm.Rrddata.Rrddata("year","AVERAGE").ToEnumerable();

                    var vmResource = Resources.Where(a => a.type == "lxc" &&
                                                          a.vmid + "" == vmItem.vmid)
                                              .FirstOrDefault();
                    vmResource.Detail = vmDetail;
                }

                //qemu
                foreach (var vmItem in node.Qemu.Vmlist().ToEnumerable())
                {
                    var vm = node.Qemu[vmItem.vmid as string];
                    var vmDetail = new NodeVm
                    {
                        Item = vmItem,
                        Config = vm.Config.VmConfig().Response.data,
                        AgentGuestRunning = vm.Agent.Network_Get_Interfaces.Network_Get_Interfaces().Response.data != null,
                        Snapshots = vm.Snapshot.SnapshotList().ToEnumerable(),
                        Status = vm.Status.Current.VmStatus().Response.data,
                    };

                    vmDetail.Config.description = "REMOVED FOR SECURITY";
                    //vmDetail.RrdData.Hour = vm.Rrddata.Rrddata("hour","AVERAGE").ToEnumerable();
                    vmDetail.RrdData.Day = vm.Rrddata.Rrddata("day","AVERAGE").ToEnumerable();
                    vmDetail.RrdData.Week = vm.Rrddata.Rrddata("week", "AVERAGE").ToEnumerable();
                    //vmDetail.RrdData.Month = vm.Rrddata.Rrddata("month", "AVERAGE").ToEnumerable();
                    //vmDetail.RrdData.Year = vm.Rrddata.Rrddata("year","AVERAGE").ToEnumerable();

                    var vmResource = Resources.Where(a => a.type == "qemu" &&
                                                          a.vmid + "" == vmItem.vmid)
                                              .FirstOrDefault();
                    vmResource.Detail = vmDetail;
                }

                //storages
                //var storageList = new List<NodeStorage>();
                foreach (var storageItem in node.Storage.Index().ToEnumerable()
                                                                .Where(a => a.enabled == 1 && a.active == 1))
                {
                    var storageNode = node.Storage[storageItem.storage as string];
                    var storeageDetail = new NodeStorage
                    {
                        Item = storageItem,
                        Content = storageNode.Content.Index().Response.data,
                        Status = storageNode.Status.ReadStatus().Response.data,
                    };

                    //storeageDetail.RrdData.Hour = storageNode.Rrddata.Rrddata("hour","AVERAGE").ToEnumerable();
                    storeageDetail.RrdData.Day = storageNode.Rrddata.Rrddata("day","AVERAGE").ToEnumerable();
                    storeageDetail.RrdData.Week = storageNode.Rrddata.Rrddata("week", "AVERAGE").ToEnumerable();
                    //storeageDetail.RrdData.Month = storageNode.Rrddata.Rrddata("month", "AVERAGE").ToEnumerable();
                    //storeageDetail.RrdData.Year = storageNode.Rrddata.Rrddata("year","AVERAGE").ToEnumerable();

                    var storageResource = Resources.Where(a => a.type == "storage" &&
                                                               a.node == nodeItem.node &&
                                                               a.storage == storageItem.storage)
                                                   .FirstOrDefault();
                    storageResource.Detail = storeageDetail;
                }

                nodeList.Add(nodeDetail);
            }
        }
    }
}