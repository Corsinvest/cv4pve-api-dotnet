/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Access;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

namespace Corsinvest.ProxmoxVE.Api.Extension
{
    /// <summary>
    /// Models extensions
    /// </summary>
    public static class ModelsExtensionsAutoGen
    {
        /// <summary>
        /// User index.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="enabled">Optional filter for enable property.</param>
        /// <param name="full">Include group and token information.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessUser>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveUsers item, bool? enabled = null, bool? full = null)
            => (await item.Index(enabled, full)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessUser>>();

        /// <summary>
        /// Group index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessGroup>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveGroups item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessGroup>>();

        /// <summary>
        /// Role index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessRole>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveRoles item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessRole>>();

        /// <summary>
        /// Get Access Control List (ACLs).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessAcl>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveAcl item)
            => (await item.ReadAcl()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessAcl>>();

        /// <summary>
        /// Authentication domain index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessDomain>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveDomains item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessDomain>>();

        /// <summary>
        /// List recent tasks (cluster wide).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTask>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveTasks item)
            => (await item.Tasks()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTask>>();

        /// <summary>
        /// Read cluster log
        /// </summary>
        /// <param name="item"></param>
        /// <param name="max">Maximum number of entries.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterLog>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveLog item, int? max = null)
            => (await item.Log(max)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterLog>>();

        /// <summary>
        /// Get cluster status information.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterStatus>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveStatus item)
            => (await item.GetStatus()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterStatus>>();

        /// <summary>
        /// List vzdump backup schedule.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterBackup>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveBackup item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterBackup>>();

        /// <summary>
        /// List replication jobs.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterReplication>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveReplication item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterReplication>>();

        /// <summary>
        /// Resources index (cluster wide).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">
        ///   Enum: vm,storage,node,sdn</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterResource>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveResources item, string type = null)
            => (await item.Resources(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterResource>>();

        /// <summary>
        /// Corosync node list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigNode>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveConfig.PveNodes item)
            => (await item.Nodes()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigNode>>();

        /// <summary>
        /// Get HA manger status.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaStatusCurrent>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveHa.PveStatus.PveCurrent item)
            => (await item.Status()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaStatusCurrent>>();

        /// <summary>
        /// Get HA groups.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaGroup>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveHa.PveGroups item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaGroup>>();

        /// <summary>
        /// List HA resources.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list resources of specific type
        ///   Enum: ct,vm</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaResource>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveHa.PveResources item, string type = null)
            => (await item.Index(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaResource>>();

        /// <summary>
        /// List aliases
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveAliases item)
            => (await item.GetAliases()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>>();

        /// <summary>
        /// List IPSets
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveIpset item)
            => (await item.IpsetIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveRules item)
            => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

        /// <summary>
        /// List IPSet content
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveIpset.PveNameItem item)
            => (await item.GetIpset()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>>();

        /// <summary>
        /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list references of specified type.
        ///   Enum: alias,ipset</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveRefs item, string type = null)
            => (await item.Refs(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>>();

        /// <summary>
        /// List security groups.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterFirewallGroup>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveGroups item)
            => (await item.ListSecurityGroups()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterFirewallGroup>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveGroups.PveGroupItem item)
            => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

        /// <summary>
        /// Pool index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Pool.PoolItem>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PvePools item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Pool.PoolItem>>();

        /// <summary>
        /// Cluster node index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeItem>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeItem>>();

        /// <summary>
        /// List status of all replication jobs on this node.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="guest">Only list replication jobs for this guest.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeReplication>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveReplication item, int? guest = null)
            => (await item.Status(guest)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeReplication>>();

        /// <summary>
        /// List local disks.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="include_partitions">Also include partitions.</param>
        /// <param name="skipsmart">Skip smart checks.</param>
        /// <param name="type">Only list specific types of disks.
        ///   Enum: unused,journal_disks</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskList>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveList item, bool? include_partitions = null, bool? skipsmart = null, string type = null)
            => (await item.List(include_partitions, skipsmart, type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskList>>();

        /// <summary>
        /// List Zpools.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskZfs>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveZfs item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskZfs>>();

        /// <summary>
        /// List local PCI devices.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pci_class_blacklist">A list of blacklisted PCI classes, which will not be returned. Following are filtered by default: Memory Controller (05), Bridge (06) and Processor (0b).</param>
        /// <param name="verbose">If disabled, does only print the PCI IDs. Otherwise, additional information like vendor and device will be returned.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeHardwarePci>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveHardware.PvePci item, string pci_class_blacklist = null, bool? verbose = null)
            => (await item.Pciscan(pci_class_blacklist, verbose)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeHardwarePci>>();

        /// <summary>
        /// List local USB devices.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeHardwareUsb>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveHardware.PveUsb item)
            => (await item.Usbscan()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeHardwareUsb>>();

        /// <summary>
        /// List available networks
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list specific interface types.
        ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,any_bridge,any_local_bridge</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeNetwork>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveNetwork item, string type = null)
            => (await item.Index(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeNetwork>>();

        /// <summary>
        /// Get package information for important Proxmox packages.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptVersion>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveApt.PveVersions item)
            => (await item.Versions()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptVersion>>();

        /// <summary>
        /// List available updates.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptUpdate>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveApt.PveUpdate item)
            => (await item.ListUpdates()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptUpdate>>();

        /// <summary>
        /// Read tap/vm network device interface counters
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeNetstat>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveNetstat item)
            => (await item.Netstat()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeNetstat>>();

        /// <summary>
        /// Service list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeService>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveServices item)
            => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeService>>();

        /// <summary>
        /// Read task list for one node (finished tasks).
        /// </summary>
        /// <param name="item"></param>
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
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTask>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveTasks item, bool? errors = null, int? limit = null, int? since = null, string source = null, int? start = null, string statusfilter = null, string typefilter = null, int? until = null, string userfilter = null, int? vmid = null)
            => (await item.NodeTasks(errors, limit, since, source, start, statusfilter, typefilter, until, userfilter, vmid)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTask>>();

        /// <summary>
        /// Read node RRD statistics
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeframe">Specify the time frame you are interested in.
        ///   Enum: hour,day,week,month,year</param>
        /// <param name="cf">The RRD consolidation function
        ///   Enum: AVERAGE,MAX</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeRrdData>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveRrddata item, string timeframe, string cf = null)
            => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeRrdData>>();

        /// <summary>
        /// Virtual machine index (per node).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="full">Determine the full status of active VMs.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVmQemu>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu item, bool? full = null)
            => (await item.Vmlist(full)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVmQemu>>();

        /// <summary>
        /// LXC container index (per node).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVmLxc>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc item)
            => (await item.Vmlist()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVmLxc>>();

        /// <summary>
        /// Get status for all datastores.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="content">Only list stores which support this content type.</param>
        /// <param name="enabled">Only list stores which are enabled (not disabled in config).</param>
        /// <param name="format">Include information about formats</param>
        /// <param name="storage">Only list status for  specified storage</param>
        /// <param name="target">If target is different to 'node', we only lists shared storages which content is accessible on this 'node' and the specified 'target' node.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorage>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage item, string content = null, bool? enabled = null, bool? format = null, string storage = null, string target = null)
            => (await item.Index(content, enabled, format, storage, target)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorage>>();

        /// <summary>
        /// List storage content.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="content">Only list content of this type.</param>
        /// <param name="vmid">Only list images for this VM</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorageContent>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveContent item, string content = null, int? vmid = null)
            => (await item.Index(content, vmid)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorageContent>>();

        /// <summary>
        /// Read storage RRD statistics.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeframe">Specify the time frame you are interested in.
        ///   Enum: hour,day,week,month,year</param>
        /// <param name="cf">The RRD consolidation function
        ///   Enum: AVERAGE,MAX</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorageRrdData>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveRrddata item, string timeframe, string cf = null)
            => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorageRrdData>>();

        /// <summary>
        /// List files and directories for single file restore under the given path.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="filepath">base64-path to the directory or file being listed, or "/".</param>
        /// <param name="volume">Backup volume ID or name. Currently only PBS snapshots are supported.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeBackupFile>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveFileRestore.PveList item, string filepath, string volume)
            => (await item.List(filepath, volume)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeBackupFile>>();

        /// <summary>
        /// Get information about node's certificates.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCertificate>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveCertificates.PveInfo item)
            => (await item.Info()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCertificate>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveFirewall.PveRules item)
            => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

        /// <summary>
        /// Storage index.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list storage of specific type
        ///   Enum: btrfs,cephfs,cifs,dir,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,pbs,rbd,zfs,zfspool</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Storage.StorageItem>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveStorage item, string type = null)
            => (await item.Index(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Storage.StorageItem>>();

        /// <summary>
        /// Read VM RRD statistics
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeframe">Specify the time frame you are interested in.
        ///   Enum: hour,day,week,month,year</param>
        /// <param name="cf">The RRD consolidation function
        ///   Enum: AVERAGE,MAX</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmRrdData>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveRrddata item, string timeframe, string cf = null)
            => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmRrdData>>();

        /// <summary>
        /// List all snapshots.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmSnapshot>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveSnapshot item)
            => (await item.SnapshotList()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmSnapshot>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveRules item)
            => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

        /// <summary>
        /// List aliases
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveAliases item)
            => (await item.GetAliases()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>>();

        /// <summary>
        /// List IPSets
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveIpset item)
            => (await item.IpsetIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>>();

        /// <summary>
        /// List IPSet content
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveIpset.PveNameItem item)
            => (await item.GetIpset()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>>();

        /// <summary>
        /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list references of specified type.
        ///   Enum: alias,ipset</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveRefs item, string type = null)
            => (await item.Refs(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>>();

        /// <summary>
        /// Get the virtual machine configuration with both current and pending values.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.KeyValue>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PvePending item)
            => (await item.VmPending()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.KeyValue>>();

        /// <summary>
        /// Read VM RRD statistics
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeframe">Specify the time frame you are interested in.
        ///   Enum: hour,day,week,month,year</param>
        /// <param name="cf">The RRD consolidation function
        ///   Enum: AVERAGE,MAX</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmRrdData>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveRrddata item, string timeframe, string cf = null)
            => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmRrdData>>();

        /// <summary>
        /// List all snapshots.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmSnapshot>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveSnapshot item)
            => (await item.List()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmSnapshot>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveRules item)
            => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

        /// <summary>
        /// List aliases
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveAliases item)
            => (await item.GetAliases()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>>();

        /// <summary>
        /// List IPSets
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveIpset item)
            => (await item.IpsetIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>>();

        /// <summary>
        /// List IPSet content
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveIpset.PveNameItem item)
            => (await item.GetIpset()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>>();

        /// <summary>
        /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list references of specified type.
        ///   Enum: alias,ipset</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveRefs item, string type = null)
            => (await item.Refs(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>>();

        /// <summary>
        /// Get container configuration, including pending changes.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.KeyValue>> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PvePending item)
            => (await item.VmPending()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.KeyValue>>();


        /// <summary>
        /// API version details, including some parts of the global datacenter config.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVersion> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveVersion item)
            => (await item.Version()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVersion>();

        /// <summary>
        /// Get datacenter options. Without 'Sys.Audit' on '/' not all options are returned.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterOptions> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveOptions item)
            => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterOptions>();

        /// <summary>
        /// Get QDevice status
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigQDevice> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveConfig.PveQdevice item)
            => (await item.Status()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigQDevice>();

        /// <summary>
        /// Get corosync totem protocol settings.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigTotem> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveConfig.PveTotem item)
            => (await item.Totem()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigTotem>();

        /// <summary>
        /// Get information needed to join this cluster over the connected node.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="node">The node for which the joinee gets the nodeinfo. </param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigJoin> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveConfig.PveJoin item, string node = null)
            => (await item.JoinInfo(node)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigJoin>();

        /// <summary>
        /// Get Firewall options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterFirewallOptions> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveOptions item)
            => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterFirewallOptions>();

        /// <summary>
        /// Read DNS settings.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDns> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDns item)
            => (await item.Dns()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDns>();

        /// <summary>
        /// Read node status
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStatus> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStatus item)
            => (await item.Status()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStatus>();

        /// <summary>
        /// API version details
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVersion> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveVersion item)
            => (await item.Version()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVersion>();

        /// <summary>
        /// Read storage status.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorage> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveStatus item)
            => (await item.ReadStatus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorage>();

        /// <summary>
        /// Read subscription info.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeSubscription> GetEx(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveSubscription item)
            => (await item.Get()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeSubscription>();

        /// <summary>
        /// Get SMART Health of a disk.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="disk">Block device name</param>
        /// <param name="healthonly">If true returns only the health status</param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskSmart> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveSmart item, string disk, bool? healthonly = null)
            => (await item.Smart(disk, healthonly)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskSmart>();

        /// <summary>
        /// Get details about a zpool.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskZfsDetail> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveZfs.PveNameItem item)
            => (await item.Detail()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskZfsDetail>();

        /// <summary>
        /// Get host firewall options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeFirewallOptions> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveFirewall.PveOptions item)
            => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeFirewallOptions>();

        /// <summary>
        /// Get pool configuration.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">
        ///   Enum: qemu,lxc,storage</param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Pool.PoolDetail> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PvePools.PvePoolidItem item, string type = null)
            => (await item.ReadPool(type)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Pool.PoolDetail>();

        /// <summary>
        /// Get the virtual machine configuration with pending configuration changes applied. Set the 'current' parameter to get the current configuration instead.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="current">Get current values (instead of pending values).</param>
        /// <param name="snapshot">Fetch config values from given snapshot.</param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmConfigQemu> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveConfig item, bool? current = null, string snapshot = null)
            => (await item.VmConfig(current, snapshot)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmConfigQemu>();

        /// <summary>
        /// Execute get-fsinfo.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetFsInfo> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetFsinfo item)
            => (await item.GetFsinfo()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetFsInfo>();

        /// <summary>
        /// Execute get-host-name.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetHostName> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetHostName item)
            => (await item.GetHostName()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetHostName>();

        /// <summary>
        /// Execute network-get-interfaces.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentNetworkGetInterfaces> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveNetworkGetInterfaces item)
            => (await item.NetworkGetInterfaces()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentNetworkGetInterfaces>();

        /// <summary>
        /// Execute info.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentInfo> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveInfo item)
            => (await item.Info()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentInfo>();

        /// <summary>
        /// Execute get-osinfo.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentOsInfo> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetOsinfo item)
            => (await item.GetOsinfo()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentOsInfo>();

        /// <summary>
        /// Execute get-vcpus.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetVCpus> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetVcpus item)
            => (await item.GetVcpus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetVCpus>();

        /// <summary>
        /// Execute get-timezone.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetTimeZone> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetTimezone item)
            => (await item.GetTimezone()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetTimeZone>();

        /// <summary>
        /// Get virtual machine status.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuStatusCurrent> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveStatus.PveCurrent item)
            => (await item.VmStatus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuStatusCurrent>();

        /// <summary>
        /// Get VM firewall options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmFirewallOptions> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveOptions item)
            => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmFirewallOptions>();

        /// <summary>
        /// Get container configuration.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="current">Get current values (instead of pending values).</param>
        /// <param name="snapshot">Fetch config values from given snapshot.</param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmConfigLxc> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveConfig item, bool? current = null, string snapshot = null)
            => (await item.VmConfig(current, snapshot)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmConfigLxc>();

        /// <summary>
        /// Get virtual machine status.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmLxcStatusCurrent> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveStatus.PveCurrent item)
            => (await item.VmStatus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmLxcStatusCurrent>();

        /// <summary>
        /// Get VM firewall options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmFirewallOptions> Get(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveOptions item)
            => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmFirewallOptions>();



    }
}