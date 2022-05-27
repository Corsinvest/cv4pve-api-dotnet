/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Access;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public static async Task<IEnumerable<AccessUser>> Get(this PveClient.PveAccess.PveUsers item, bool? enabled = null, bool? full = null)
            => (await item.Index(enabled, full)).ToModel<IEnumerable<AccessUser>>();

        /// <summary>
        /// Group index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<AccessGroup>> Get(this PveClient.PveAccess.PveGroups item)
            => (await item.Index()).ToModel<IEnumerable<AccessGroup>>();

        /// <summary>
        /// Role index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<AccessRole>> Get(this PveClient.PveAccess.PveRoles item)
            => (await item.Index()).ToModel<IEnumerable<AccessRole>>();

        /// <summary>
        /// Get Access Control List (ACLs).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<AccessAcl>> Get(this PveClient.PveAccess.PveAcl item)
            => (await item.ReadAcl()).ToModel<IEnumerable<AccessAcl>>();

        /// <summary>
        /// Authentication domain index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<AccessDomain>> Get(this PveClient.PveAccess.PveDomains item)
            => (await item.Index()).ToModel<IEnumerable<AccessDomain>>();

        /// <summary>
        /// Get cluster status information.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterStatus>> Get(this PveClient.PveCluster.PveStatus item)
            => (await item.GetStatus()).ToModel<IEnumerable<ClusterStatus>>();

        /// <summary>
        /// List vzdump backup schedule.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterBackup>> Get(this PveClient.PveCluster.PveBackup item)
            => (await item.Index()).ToModel<IEnumerable<ClusterBackup>>();

        /// <summary>
        /// List replication jobs.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterReplication>> Get(this PveClient.PveCluster.PveReplication item)
            => (await item.Index()).ToModel<IEnumerable<ClusterReplication>>();

        /// <summary>
        /// Resources index (cluster wide).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">
        ///   Enum: vm,storage,node,sdn</param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterResource>> Get(this PveClient.PveCluster.PveResources item, string type = null)
            => (await item.Resources(type)).ToModel<IEnumerable<ClusterResource>>();

        /// <summary>
        /// Corosync node list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterConfigNode>> Get(this PveClient.PveCluster.PveConfig.PveNodes item)
            => (await item.Nodes()).ToModel<IEnumerable<ClusterConfigNode>>();

        /// <summary>
        /// Get HA manger status.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterHaStatusCurrent>> Get(this PveClient.PveCluster.PveHa.PveStatus.PveCurrent item)
            => (await item.Status()).ToModel<IEnumerable<ClusterHaStatusCurrent>>();

        /// <summary>
        /// Get HA groups.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterHaGroup>> Get(this PveClient.PveCluster.PveHa.PveGroups item)
            => (await item.Index()).ToModel<IEnumerable<ClusterHaGroup>>();

        /// <summary>
        /// List HA resources.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list resources of specific type
        ///   Enum: ct,vm</param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterHaResource>> Get(this PveClient.PveCluster.PveHa.PveResources item, string type = null)
            => (await item.Index(type)).ToModel<IEnumerable<ClusterHaResource>>();

        /// <summary>
        /// List aliases
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallAlias>> Get(this PveClient.PveCluster.PveFirewall.PveAliases item)
            => (await item.GetAliases()).ToModel<IEnumerable<FirewallAlias>>();

        /// <summary>
        /// List IPSets
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallIpSet>> Get(this PveClient.PveCluster.PveFirewall.PveIpset item)
            => (await item.IpsetIndex()).ToModel<IEnumerable<FirewallIpSet>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallRule>> Get(this PveClient.PveCluster.PveFirewall.PveRules item)
            => (await item.GetRules()).ToModel<IEnumerable<FirewallRule>>();

        /// <summary>
        /// List IPSet content
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallIpSetContent>> Get(this PveClient.PveCluster.PveFirewall.PveIpset.PveNameItem item)
            => (await item.GetIpset()).ToModel<IEnumerable<FirewallIpSetContent>>();

        /// <summary>
        /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list references of specified type.
        ///   Enum: alias,ipset</param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallRef>> Get(this PveClient.PveCluster.PveFirewall.PveRefs item, string type = null)
            => (await item.Refs(type)).ToModel<IEnumerable<FirewallRef>>();

        /// <summary>
        /// List security groups.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterFirewallGroup>> Get(this PveClient.PveCluster.PveFirewall.PveGroups item)
            => (await item.ListSecurityGroups()).ToModel<IEnumerable<ClusterFirewallGroup>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallRule>> Get(this PveClient.PveCluster.PveFirewall.PveGroups.PveGroupItem item)
            => (await item.GetRules()).ToModel<IEnumerable<FirewallRule>>();

        /// <summary>
        /// Pool index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Shared.Models.Pool.PoolItem>> Get(this PveClient.PvePools item)
            => (await item.Index()).ToModel<IEnumerable<Shared.Models.Pool.PoolItem>>();

        /// <summary>
        /// Cluster node index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeItem>> Get(this PveClient.PveNodes item)
            => (await item.Index()).ToModel<IEnumerable<NodeItem>>();

        /// <summary>
        /// List status of all replication jobs on this node.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="guest">Only list replication jobs for this guest.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeReplication>> Get(this PveClient.PveNodes.PveNodeItem.PveReplication item, int? guest = null)
            => (await item.Status(guest)).ToModel<IEnumerable<NodeReplication>>();

        /// <summary>
        /// List local disks.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="include_partitions">Also include partitions.</param>
        /// <param name="skipsmart">Skip smart checks.</param>
        /// <param name="type">Only list specific types of disks.
        ///   Enum: unused,journal_disks</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeDiskList>> Get(this PveClient.PveNodes.PveNodeItem.PveDisks.PveList item, bool? include_partitions = null, bool? skipsmart = null, string type = null)
            => (await item.List(include_partitions, skipsmart, type)).ToModel<IEnumerable<NodeDiskList>>();

        /// <summary>
        /// List Zpools.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeDiskZfs>> Get(this PveClient.PveNodes.PveNodeItem.PveDisks.PveZfs item)
            => (await item.Index()).ToModel<IEnumerable<NodeDiskZfs>>();

        /// <summary>
        /// List local PCI devices.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pci_class_blacklist">A list of blacklisted PCI classes, which will not be returned. Following are filtered by default: Memory Controller (05), Bridge (06) and Processor (0b).</param>
        /// <param name="verbose">If disabled, does only print the PCI IDs. Otherwise, additional information like vendor and device will be returned.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeHardwarePci>> Get(this PveClient.PveNodes.PveNodeItem.PveHardware.PvePci item, string pci_class_blacklist = null, bool? verbose = null)
            => (await item.Pciscan(pci_class_blacklist, verbose)).ToModel<IEnumerable<NodeHardwarePci>>();

        /// <summary>
        /// List local USB devices.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeHardwareUsb>> Get(this PveClient.PveNodes.PveNodeItem.PveHardware.PveUsb item)
            => (await item.Usbscan()).ToModel<IEnumerable<NodeHardwareUsb>>();

        /// <summary>
        /// List available networks
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list specific interface types.
        ///   Enum: bridge,bond,eth,alias,vlan,OVSBridge,OVSBond,OVSPort,OVSIntPort,any_bridge</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeNetwork>> Get(this PveClient.PveNodes.PveNodeItem.PveNetwork item, string type = null)
            => (await item.Index(type)).ToModel<IEnumerable<NodeNetwork>>();

        /// <summary>
        /// Get package information for important Proxmox packages.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeAptVersion>> Get(this PveClient.PveNodes.PveNodeItem.PveApt.PveVersions item)
            => (await item.Versions()).ToModel<IEnumerable<NodeAptVersion>>();

        /// <summary>
        /// List available updates.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeAptUpdate>> Get(this PveClient.PveNodes.PveNodeItem.PveApt.PveUpdate item)
            => (await item.ListUpdates()).ToModel<IEnumerable<NodeAptUpdate>>();

        /// <summary>
        /// Read tap/vm network device interface counters
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeNetstat>> Get(this PveClient.PveNodes.PveNodeItem.PveNetstat item)
            => (await item.Netstat()).ToModel<IEnumerable<NodeNetstat>>();

        /// <summary>
        /// Service list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeService>> Get(this PveClient.PveNodes.PveNodeItem.PveServices item)
            => (await item.Index()).ToModel<IEnumerable<NodeService>>();

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
        public static async Task<IEnumerable<NodeTask>> Get(this PveClient.PveNodes.PveNodeItem.PveTasks item, bool? errors = null, int? limit = null, int? since = null, string source = null, int? start = null, string statusfilter = null, string typefilter = null, int? until = null, string userfilter = null, int? vmid = null)
            => (await item.NodeTasks(errors, limit, since, source, start, statusfilter, typefilter, until, userfilter, vmid)).ToModel<IEnumerable<NodeTask>>();

        /// <summary>
        /// Read node RRD statistics
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeframe">Specify the time frame you are interested in.
        ///   Enum: hour,day,week,month,year</param>
        /// <param name="cf">The RRD consolidation function
        ///   Enum: AVERAGE,MAX</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeRrdData>> Get(this PveClient.PveNodes.PveNodeItem.PveRrddata item, string timeframe, string cf = null)
            => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<NodeRrdData>>();

        /// <summary>
        /// Virtual machine index (per node).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="full">Determine the full status of active VMs.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeVmQemu>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu item, bool? full = null)
            => (await item.Vmlist(full)).ToModel<IEnumerable<NodeVmQemu>>();

        /// <summary>
        /// LXC container index (per node).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeVmLxc>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc item)
            => (await item.Vmlist()).ToModel<IEnumerable<NodeVmLxc>>();

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
        public static async Task<IEnumerable<NodeStorage>> Get(this PveClient.PveNodes.PveNodeItem.PveStorage item, string content = null, bool? enabled = null, bool? format = null, string storage = null, string target = null)
            => (await item.Index(content, enabled, format, storage, target)).ToModel<IEnumerable<NodeStorage>>();

        /// <summary>
        /// List storage content.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="content">Only list content of this type.</param>
        /// <param name="vmid">Only list images for this VM</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeStorageContent>> Get(this PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveContent item, string content = null, int? vmid = null)
            => (await item.Index(content, vmid)).ToModel<IEnumerable<NodeStorageContent>>();

        /// <summary>
        /// Read storage RRD statistics.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeframe">Specify the time frame you are interested in.
        ///   Enum: hour,day,week,month,year</param>
        /// <param name="cf">The RRD consolidation function
        ///   Enum: AVERAGE,MAX</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeStorageRrdData>> Get(this PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveRrddata item, string timeframe, string cf = null)
            => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<NodeStorageRrdData>>();

        /// <summary>
        /// List files and directories for single file restore under the given path.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="filepath">base64-path to the directory or file being listed, or "/".</param>
        /// <param name="volume">Backup volume ID or name. Currently only PBS snapshots are supported.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeBackupFile>> Get(this PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveFileRestore.PveList item, string filepath, string volume)
            => (await item.List(filepath, volume)).ToModel<IEnumerable<NodeBackupFile>>();

        /// <summary>
        /// Get information about node's certificates.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeCertificate>> Get(this PveClient.PveNodes.PveNodeItem.PveCertificates.PveInfo item)
            => (await item.Info()).ToModel<IEnumerable<NodeCertificate>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallRule>> Get(this PveClient.PveNodes.PveNodeItem.PveFirewall.PveRules item)
            => (await item.GetRules()).ToModel<IEnumerable<FirewallRule>>();

        /// <summary>
        /// Storage index.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list storage of specific type
        ///   Enum: btrfs,cephfs,cifs,dir,glusterfs,iscsi,iscsidirect,lvm,lvmthin,nfs,pbs,rbd,zfs,zfspool</param>
        /// <returns></returns>
        public static async Task<IEnumerable<Shared.Models.Storage.StorageItem>> Get(this PveClient.PveStorage item, string type = null)
            => (await item.Index(type)).ToModel<IEnumerable<Shared.Models.Storage.StorageItem>>();

        /// <summary>
        /// Read VM RRD statistics
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeframe">Specify the time frame you are interested in.
        ///   Enum: hour,day,week,month,year</param>
        /// <param name="cf">The RRD consolidation function
        ///   Enum: AVERAGE,MAX</param>
        /// <returns></returns>
        public static async Task<IEnumerable<VmRrdData>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveRrddata item, string timeframe, string cf = null)
            => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<VmRrdData>>();

        /// <summary>
        /// List all snapshots.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<VmSnapshot>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveSnapshot item)
            => (await item.SnapshotList()).ToModel<IEnumerable<VmSnapshot>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallRule>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveRules item)
            => (await item.GetRules()).ToModel<IEnumerable<FirewallRule>>();

        /// <summary>
        /// List aliases
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallAlias>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveAliases item)
            => (await item.GetAliases()).ToModel<IEnumerable<FirewallAlias>>();

        /// <summary>
        /// List IPSets
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallIpSet>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveIpset item)
            => (await item.IpsetIndex()).ToModel<IEnumerable<FirewallIpSet>>();

        /// <summary>
        /// List IPSet content
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallIpSetContent>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveIpset.PveNameItem item)
            => (await item.GetIpset()).ToModel<IEnumerable<FirewallIpSetContent>>();

        /// <summary>
        /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list references of specified type.
        ///   Enum: alias,ipset</param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallRef>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveRefs item, string type = null)
            => (await item.Refs(type)).ToModel<IEnumerable<FirewallRef>>();

        /// <summary>
        /// Get the virtual machine configuration with both current and pending values.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<KeyValue>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PvePending item)
            => (await item.VmPending()).ToModel<IEnumerable<KeyValue>>();

        /// <summary>
        /// Read VM RRD statistics
        /// </summary>
        /// <param name="item"></param>
        /// <param name="timeframe">Specify the time frame you are interested in.
        ///   Enum: hour,day,week,month,year</param>
        /// <param name="cf">The RRD consolidation function
        ///   Enum: AVERAGE,MAX</param>
        /// <returns></returns>
        public static async Task<IEnumerable<VmRrdData>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveRrddata item, string timeframe, string cf = null)
            => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<VmRrdData>>();

        /// <summary>
        /// List all snapshots.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<VmSnapshot>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveSnapshot item)
            => (await item.List()).ToModel<IEnumerable<VmSnapshot>>();

        /// <summary>
        /// List rules.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallRule>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveRules item)
            => (await item.GetRules()).ToModel<IEnumerable<FirewallRule>>();

        /// <summary>
        /// List aliases
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallAlias>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveAliases item)
            => (await item.GetAliases()).ToModel<IEnumerable<FirewallAlias>>();

        /// <summary>
        /// List IPSets
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallIpSet>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveIpset item)
            => (await item.IpsetIndex()).ToModel<IEnumerable<FirewallIpSet>>();

        /// <summary>
        /// List IPSet content
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallIpSetContent>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveIpset.PveNameItem item)
            => (await item.GetIpset()).ToModel<IEnumerable<FirewallIpSetContent>>();

        /// <summary>
        /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type">Only list references of specified type.
        ///   Enum: alias,ipset</param>
        /// <returns></returns>
        public static async Task<IEnumerable<FirewallRef>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveRefs item, string type = null)
            => (await item.Refs(type)).ToModel<IEnumerable<FirewallRef>>();

        /// <summary>
        /// Get container configuration, including pending changes.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<KeyValue>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PvePending item)
            => (await item.VmPending()).ToModel<IEnumerable<KeyValue>>();


        /// <summary>
        /// Get datacenter options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<ClusterOptions> Get(this PveClient.PveCluster.PveOptions item)
            => (await item.GetOptions()).ToModel<ClusterOptions>();

        /// <summary>
        /// Get QDevice status
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<ClusterConfigQDevice> Get(this PveClient.PveCluster.PveConfig.PveQdevice item)
            => (await item.Status()).ToModel<ClusterConfigQDevice>();

        /// <summary>
        /// Get corosync totem protocol settings.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<ClusterConfigTotem> Get(this PveClient.PveCluster.PveConfig.PveTotem item)
            => (await item.Totem()).ToModel<ClusterConfigTotem>();

        /// <summary>
        /// Get information needed to join this cluster over the connected node.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="node">The node for which the joinee gets the nodeinfo. </param>
        /// <returns></returns>
        public static async Task<ClusterConfigJoin> Get(this PveClient.PveCluster.PveConfig.PveJoin item, string node = null)
            => (await item.JoinInfo(node)).ToModel<ClusterConfigJoin>();

        /// <summary>
        /// Get Firewall options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<ClusterFirewallOptions> Get(this PveClient.PveCluster.PveFirewall.PveOptions item)
            => (await item.GetOptions()).ToModel<ClusterFirewallOptions>();

        /// <summary>
        /// Read DNS settings.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<NodeDns> Get(this PveClient.PveNodes.PveNodeItem.PveDns item)
            => (await item.Dns()).ToModel<NodeDns>();

        /// <summary>
        /// Read node status
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<NodeStatus> Get(this PveClient.PveNodes.PveNodeItem.PveStatus item)
            => (await item.Status()).ToModel<NodeStatus>();

        /// <summary>
        /// API version details
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<NodeVersion> Get(this PveClient.PveNodes.PveNodeItem.PveVersion item)
            => (await item.Version()).ToModel<NodeVersion>();

        /// <summary>
        /// Read storage status.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<NodeStorage> Get(this PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveStatus item)
            => (await item.ReadStatus()).ToModel<NodeStorage>();

        /// <summary>
        /// Read subscription info.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<NodeSubscription> GetEx(this PveClient.PveNodes.PveNodeItem.PveSubscription item)
            => (await item.Get()).ToModel<NodeSubscription>();

        /// <summary>
        /// Get SMART Health of a disk.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="disk">Block device name</param>
        /// <param name="healthonly">If true returns only the health status</param>
        /// <returns></returns>
        public static async Task<NodeDiskSmart> Get(this PveClient.PveNodes.PveNodeItem.PveDisks.PveSmart item, string disk, bool? healthonly = null)
            => (await item.Smart(disk, healthonly)).ToModel<NodeDiskSmart>();

        /// <summary>
        /// Get details about a zpool.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<NodeDiskZfsDetail> Get(this PveClient.PveNodes.PveNodeItem.PveDisks.PveZfs.PveNameItem item)
            => (await item.Detail()).ToModel<NodeDiskZfsDetail>();

        /// <summary>
        /// Get host firewall options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<NodeFirewallOptions> Get(this PveClient.PveNodes.PveNodeItem.PveFirewall.PveOptions item)
            => (await item.GetOptions()).ToModel<NodeFirewallOptions>();

        /// <summary>
        /// Get pool configuration.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<Shared.Models.Pool.PoolDetail> Get(this PveClient.PvePools.PvePoolidItem item)
            => (await item.ReadPool()).ToModel<Shared.Models.Pool.PoolDetail>();

        /// <summary>
        /// Get the virtual machine configuration with pending configuration changes applied. Set the 'current' parameter to get the current configuration instead.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="current">Get current values (instead of pending values).</param>
        /// <param name="snapshot">Fetch config values from given snapshot.</param>
        /// <returns></returns>
        public static async Task<VmConfigQemu> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveConfig item, bool? current = null, string snapshot = null)
            => (await item.VmConfig(current, snapshot)).ToModel<VmConfigQemu>();

        /// <summary>
        /// Execute get-fsinfo.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmQemuAgentGetFsInfo> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetFsinfo item)
            => (await item.GetFsinfo()).ToModel<VmQemuAgentGetFsInfo>();

        /// <summary>
        /// Execute get-host-name.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmQemuAgentGetHostName> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetHostName item)
            => (await item.GetHostName()).ToModel<VmQemuAgentGetHostName>();

        /// <summary>
        /// Execute network-get-interfaces.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmQemuAgentNetworkGetInterfaces> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveNetworkGetInterfaces item)
            => (await item.NetworkGetInterfaces()).ToModel<VmQemuAgentNetworkGetInterfaces>();

        /// <summary>
        /// Execute info.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmQemuAgentInfo> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveInfo item)
            => (await item.Info()).ToModel<VmQemuAgentInfo>();

        /// <summary>
        /// Execute get-osinfo.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmQemuAgentOsInfo> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetOsinfo item)
            => (await item.GetOsinfo()).ToModel<VmQemuAgentOsInfo>();

        /// <summary>
        /// Execute get-vcpus.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmQemuAgentGetVCpus> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetVcpus item)
            => (await item.GetVcpus()).ToModel<VmQemuAgentGetVCpus>();

        /// <summary>
        /// Execute get-timezone.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmQemuAgentGetTimeZone> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetTimezone item)
            => (await item.GetTimezone()).ToModel<VmQemuAgentGetTimeZone>();

        /// <summary>
        /// Get virtual machine status.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmQemuStatusCurrent> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveStatus.PveCurrent item)
            => (await item.VmStatus()).ToModel<VmQemuStatusCurrent>();

        /// <summary>
        /// Get VM firewall options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmFirewallOptions> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveOptions item)
            => (await item.GetOptions()).ToModel<VmFirewallOptions>();

        /// <summary>
        /// Get container configuration.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="current">Get current values (instead of pending values).</param>
        /// <param name="snapshot">Fetch config values from given snapshot.</param>
        /// <returns></returns>
        public static async Task<VmConfigLxc> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveConfig item, bool? current = null, string snapshot = null)
            => (await item.VmConfig(current, snapshot)).ToModel<VmConfigLxc>();

        /// <summary>
        /// Get virtual machine status.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmLxcStatusCurrent> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveStatus.PveCurrent item)
            => (await item.VmStatus()).ToModel<VmLxcStatusCurrent>();

        /// <summary>
        /// Get VM firewall options.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task<VmFirewallOptions> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveOptions item)
            => (await item.GetOptions()).ToModel<VmFirewallOptions>();



    }
}