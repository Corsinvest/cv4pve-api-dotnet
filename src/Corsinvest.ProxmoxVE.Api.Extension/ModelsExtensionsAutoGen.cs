/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Extension;

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
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessUser>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveUsers item, bool? enabled = null, bool? full = null)
        => (await item.Index(enabled, full)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessUser>>();

    /// <summary>
    /// Group index.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessGroup>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveGroups item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessGroup>>();

    /// <summary>
    /// Role index.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessRole>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveRoles item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessRole>>();

    /// <summary>
    /// Get Access Control List (ACLs).
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessAcl>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveAcl item)
        => (await item.ReadAcl()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessAcl>>();

    /// <summary>
    /// List TFA configurations of users.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessTfa>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveTfa item)
        => (await item.ListTfa()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessTfa>>();

    /// <summary>
    /// Authentication domain index.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessDomain>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveDomains item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessDomain>>();

    /// <summary>
    /// Get user API tokens.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessUserToken>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveUsers.PveUseridItem.PveToken item)
        => (await item.TokenIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessUserToken>>();

    /// <summary>
    /// List TFA configurations of users.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessTfaEntry>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveAccess.PveTfa.PveUseridItem item)
        => (await item.ListUserTfa()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Access.AccessTfaEntry>>();

    /// <summary>
    /// List configured metric servers.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterMetricsServer>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveMetrics.PveServer item)
        => (await item.ServerIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterMetricsServer>>();

    /// <summary>
    /// Get HA rules.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="resource">Limit the returned list to rules affecting the specified resource.</param>
    /// <param name="type">Limit the returned list to the specified rule type.
    ///   Enum: node-affinity,resource-affinity</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaRule>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveHa.PveRules item, string resource = null, string type = null)
        => (await item.Index(resource, type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaRule>>();

    /// <summary>
    /// ACME plugin index.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list ACME plugins of a specific type
    ///   Enum: dns,standalone</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterAcmePlugin>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveAcme.PvePlugins item, string type = null)
        => (await item.Index(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterAcmePlugin>>();

    /// <summary>
    /// List configured realm-sync-jobs.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterJobRealmSync>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveJobs.PveRealmSync item)
        => (await item.SyncjobIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterJobRealmSync>>();

    /// <summary>
    /// List directory mapping
    /// </summary>
    /// <param name="item"></param>
    /// <param name="check_node">If given, checks the configurations on the given node for correctness, and adds relevant diagnostics for the directory to the response.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterMappingDir>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveMapping.PveDir item, string check_node = null)
        => (await item.Index(check_node)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterMappingDir>>();

    /// <summary>
    /// List PCI Hardware Mapping
    /// </summary>
    /// <param name="item"></param>
    /// <param name="check_node">If given, checks the configurations on the given node for correctness, and adds relevant diagnostics for the devices to the response.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterMappingPci>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveMapping.PvePci item, string check_node = null)
        => (await item.Index(check_node)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterMappingPci>>();

    /// <summary>
    /// List USB Hardware Mappings
    /// </summary>
    /// <param name="item"></param>
    /// <param name="check_node">If given, checks the configurations on the given node for correctness, and adds relevant errors to the devices.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterMappingUsb>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveMapping.PveUsb item, string check_node = null)
        => (await item.Index(check_node)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterMappingUsb>>();

    /// <summary>
    /// SDN vnets index.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pending">Display pending config.</param>
    /// <param name="running">Display running config.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnVnet>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveSdn.PveVnets item, bool? pending = null, bool? running = null)
        => (await item.Index(pending, running)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnVnet>>();

    /// <summary>
    /// SDN zones index.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pending">Display pending config.</param>
    /// <param name="running">Display running config.</param>
    /// <param name="type">Only list SDN zones of specific type
    ///   Enum: evpn,faucet,qinq,simple,vlan,vxlan</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnZone>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveSdn.PveZones item, bool? pending = null, bool? running = null, string type = null)
        => (await item.Index(pending, running, type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnZone>>();

    /// <summary>
    /// SDN controllers index.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pending">Display pending config.</param>
    /// <param name="running">Display running config.</param>
    /// <param name="type">Only list sdn controllers of specific type
    ///   Enum: bgp,evpn,faucet,isis</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnController>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveSdn.PveControllers item, bool? pending = null, bool? running = null, string type = null)
        => (await item.Index(pending, running, type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnController>>();

    /// <summary>
    /// SDN ipams index.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list sdn ipams of specific type
    ///   Enum: netbox,phpipam,pve</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnIpam>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveSdn.PveIpams item, string type = null)
        => (await item.Index(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnIpam>>();

    /// <summary>
    /// SDN dns index.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list sdn dns of specific type
    ///   Enum: powerdns</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnDns>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveSdn.PveDns item, string type = null)
        => (await item.Index(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnDns>>();

    /// <summary>
    /// SDN subnets index.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pending">Display pending config.</param>
    /// <param name="running">Display running config.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnSubnet>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveSdn.PveVnets.PveVnetItem.PveSubnets item, bool? pending = null, bool? running = null)
        => (await item.Index(pending, running)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterSdnSubnet>>();

    /// <summary>
    /// List recent tasks (cluster wide).
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTask>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveTasks item)
        => (await item.Tasks()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTask>>();

    /// <summary>
    /// Read cluster log
    /// </summary>
    /// <param name="item"></param>
    /// <param name="max">Maximum number of entries.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterLog>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveLog item, int? max = null)
        => (await item.Log(max)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterLog>>();

    /// <summary>
    /// Get cluster status information.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterStatus>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveStatus item)
        => (await item.GetStatus()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterStatus>>();

    /// <summary>
    /// List vzdump backup schedule.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterBackup>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveBackup item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterBackup>>();

    /// <summary>
    /// List replication jobs.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterReplication>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveReplication item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterReplication>>();

    /// <summary>
    /// Resources index (cluster wide).
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Resource type.
    ///   Enum: vm,storage,node,sdn</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterResource>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveResources item, string type = null)
        => (await item.Resources(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterResource>>();

    /// <summary>
    /// Corosync node list.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigNode>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveConfig.PveNodes item)
        => (await item.Nodes()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigNode>>();

    /// <summary>
    /// Get HA manger status.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaStatusCurrent>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveHa.PveStatus.PveCurrent item)
        => (await item.Status()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaStatusCurrent>>();

    /// <summary>
    /// Get HA groups. (deprecated in favor of HA rules)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaGroup>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveHa.PveGroups item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaGroup>>();

    /// <summary>
    /// List HA resources.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list resources of specific type
    ///   Enum: ct,vm</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaResource>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveHa.PveResources item, string type = null)
        => (await item.Index(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterHaResource>>();

    /// <summary>
    /// List aliases
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveAliases item)
        => (await item.GetAliases()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>>();

    /// <summary>
    /// List IPSets
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveIpset item)
        => (await item.IpsetIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>>();

    /// <summary>
    /// List rules.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveRules item)
        => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

    /// <summary>
    /// List IPSet content
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveIpset.PveNameItem item)
        => (await item.GetIpset()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>>();

    /// <summary>
    /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list references of specified type.
    ///   Enum: alias,ipset</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveRefs item, string type = null)
        => (await item.Refs(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>>();

    /// <summary>
    /// List security groups.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterFirewallGroup>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveGroups item)
        => (await item.ListSecurityGroups()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterFirewallGroup>>();

    /// <summary>
    /// List rules.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveGroups.PveGroupItem item)
        => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

    /// <summary>
    /// List pools or get pool configuration.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="poolid"></param>
    /// <param name="type">
    ///   Enum: qemu,lxc,storage</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Pool.PoolItem>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PvePools item, string poolid = null, string type = null)
        => (await item.Index(poolid, type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Pool.PoolItem>>();

    /// <summary>
    /// Cluster node index.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeItem>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeItem>>();

    /// <summary>
    /// List status of all replication jobs on this node.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="guest">Only list replication jobs for this guest.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeReplication>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveReplication item, int? guest = null)
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
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskList>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveList item, bool? include_partitions = null, bool? skipsmart = null, string type = null)
        => (await item.List(include_partitions, skipsmart, type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskList>>();

    /// <summary>
    /// List Zpools.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskZfs>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveZfs item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskZfs>>();

    /// <summary>
    /// List local PCI devices.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pci_class_blacklist">A list of blacklisted PCI classes, which will not be returned. Following are filtered by default: Memory Controller (05), Bridge (06) and Processor (0b).</param>
    /// <param name="verbose">If disabled, does only print the PCI IDs. Otherwise, additional information like vendor and device will be returned.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeHardwarePci>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveHardware.PvePci item, string pci_class_blacklist = null, bool? verbose = null)
        => (await item.PciScan(pci_class_blacklist, verbose)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeHardwarePci>>();

    /// <summary>
    /// List local USB devices.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeHardwareUsb>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveHardware.PveUsb item)
        => (await item.Usbscan()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeHardwareUsb>>();

    /// <summary>
    /// List available networks
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list specific interface types.
    ///   Enum: bridge,bond,eth,alias,vlan,fabric,OVSBridge,OVSBond,OVSPort,OVSIntPort,vnet,any_bridge,any_local_bridge,include_sdn</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeNetwork>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveNetwork item, string type = null)
        => (await item.Index(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeNetwork>>();

    /// <summary>
    /// Get package information for important Proxmox packages.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptVersion>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveApt.PveVersions item)
        => (await item.Versions()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptVersion>>();

    /// <summary>
    /// List available updates.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptUpdate>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveApt.PveUpdate item)
        => (await item.ListUpdates()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptUpdate>>();

    /// <summary>
    /// Read tap/vm network device interface counters
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeNetstat>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveNetstat item)
        => (await item.Netstat()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeNetstat>>();

    /// <summary>
    /// Service list.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeService>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveServices item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeService>>();

    /// <summary>
    /// Read task list for one node (finished tasks).
    /// </summary>
    /// <param name="item"></param>
    /// <param name="errors">Only list tasks with a status of ERROR.</param>
    /// <param name="limit">Only list this number of tasks.</param>
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
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTask>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveTasks item, bool? errors = null, int? limit = null, int? since = null, string source = null, int? start = null, string statusfilter = null, string typefilter = null, int? until = null, string userfilter = null, int? vmid = null)
        => (await item.NodeTasks(errors, limit, since, source, start, statusfilter, typefilter, until, userfilter, vmid)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTask>>();

    /// <summary>
    /// Read node RRD statistics
    /// </summary>
    /// <param name="item"></param>
    /// <param name="timeframe">Specify the time frame you are interested in.
    ///   Enum: hour,day,week,month,year,decade</param>
    /// <param name="cf">The RRD consolidation function
    ///   Enum: AVERAGE,MAX</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeRrdData>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveRrddata item, string timeframe, string cf = null)
        => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeRrdData>>();

    /// <summary>
    /// Virtual machine index (per node).
    /// </summary>
    /// <param name="item"></param>
    /// <param name="full">Determine the full status of active VMs.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVmQemu>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu item, bool? full = null)
        => (await item.Vmlist(full)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVmQemu>>();

    /// <summary>
    /// LXC container index (per node).
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVmLxc>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc item)
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
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorage>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage item, string content = null, bool? enabled = null, bool? format = null, string storage = null, string target = null)
        => (await item.Index(content, enabled, format, storage, target)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorage>>();

    /// <summary>
    /// List storage content.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="content">Only list content of this type.</param>
    /// <param name="vmid">Only list images for this VM</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorageContent>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveContent item, string content = null, int? vmid = null)
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
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorageRrdData>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveRrddata item, string timeframe, string cf = null)
        => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorageRrdData>>();

    /// <summary>
    /// List files and directories for single file restore under the given path.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="filepath">base64-path to the directory or file being listed, or "/".</param>
    /// <param name="volume">Backup volume ID or name. Currently only PBS snapshots are supported.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeBackupFile>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveFileRestore.PveList item, string filepath, string volume)
        => (await item.List(filepath, volume)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeBackupFile>>();

    /// <summary>
    /// MDS directory index.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephMds>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveCeph.PveMds item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephMds>>();

    /// <summary>
    /// MGR directory index.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephMgr>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveCeph.PveMgr item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephMgr>>();

    /// <summary>
    /// Get Ceph monitor list.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephMon>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveCeph.PveMon item)
        => (await item.Listmon()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephMon>>();

    /// <summary>
    /// Directory index.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephFs>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveCeph.PveFs item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephFs>>();

    /// <summary>
    /// List all pools and their settings (which are settable by the POST/PUT endpoints).
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephPool>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveCeph.PvePool item)
        => (await item.Lspools()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCephPool>>();

    /// <summary>
    /// List LVM thinpools
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskLvmThin>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveLvmthin item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskLvmThin>>();

    /// <summary>
    /// PVE Managed Directory storages.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskDirectory>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveDirectory item)
        => (await item.Index()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskDirectory>>();

    /// <summary>
    /// Get IP addresses of the specified container interface.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeLxcInterfaces>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveInterfaces item)
        => (await item.Ip()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeLxcInterfaces>>();

    /// <summary>
    /// Get information about node's certificates.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCertificate>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveCertificates.PveInfo item)
        => (await item.Info()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeCertificate>>();

    /// <summary>
    /// List rules.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveFirewall.PveRules item)
        => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

    /// <summary>
    /// Storage index.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list storage of specific type
    ///   Enum: btrfs,cephfs,cifs,dir,esxi,iscsi,iscsidirect,lvm,lvmthin,nfs,pbs,rbd,zfs,zfspool</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Storage.StorageItem>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveStorage item, string type = null)
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
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmRrdData>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveRrddata item, string timeframe, string cf = null)
        => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmRrdData>>();

    /// <summary>
    /// List all snapshots.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmSnapshot>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveSnapshot item)
        => (await item.SnapshotList()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmSnapshot>>();

    /// <summary>
    /// List rules.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveRules item)
        => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

    /// <summary>
    /// List aliases
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveAliases item)
        => (await item.GetAliases()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>>();

    /// <summary>
    /// List IPSets
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveIpset item)
        => (await item.IpsetIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>>();

    /// <summary>
    /// List IPSet content
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveIpset.PveNameItem item)
        => (await item.GetIpset()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>>();

    /// <summary>
    /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list references of specified type.
    ///   Enum: alias,ipset</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveRefs item, string type = null)
        => (await item.Refs(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>>();

    /// <summary>
    /// Get the virtual machine configuration with both current and pending values.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.KeyValue>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PvePending item)
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
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmRrdData>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveRrddata item, string timeframe, string cf = null)
        => (await item.Rrddata(timeframe, cf)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmRrdData>>();

    /// <summary>
    /// List all snapshots.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmSnapshot>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveSnapshot item)
        => (await item.List()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmSnapshot>>();

    /// <summary>
    /// List rules.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveRules item)
        => (await item.GetRules()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRule>>();

    /// <summary>
    /// List aliases
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveAliases item)
        => (await item.GetAliases()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallAlias>>();

    /// <summary>
    /// List IPSets
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveIpset item)
        => (await item.IpsetIndex()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSet>>();

    /// <summary>
    /// List IPSet content
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveIpset.PveNameItem item)
        => (await item.GetIpset()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallIpSetContent>>();

    /// <summary>
    /// Lists possible IPSet/Alias reference which are allowed in source/dest properties.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">Only list references of specified type.
    ///   Enum: alias,ipset</param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveRefs item, string type = null)
        => (await item.Refs(type)).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.FirewallRef>>();

    /// <summary>
    /// Get container configuration, including pending changes.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.KeyValue>> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PvePending item)
        => (await item.VmPending()).ToModel<IEnumerable<Corsinvest.ProxmoxVE.Api.Shared.Models.Common.KeyValue>>();


    /// <summary>
    /// API version details, including some parts of the global datacenter config.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVersion> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveVersion item)
        => (await item.Version()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVersion>();

    /// <summary>
    /// Get datacenter options. Without 'Sys.Audit' on '/' not all options are returned.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterOptions> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveOptions item)
        => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterOptions>();

    /// <summary>
    /// Get QDevice status
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigQDevice> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveConfig.PveQdevice item)
        => (await item.Status()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigQDevice>();

    /// <summary>
    /// Get corosync totem protocol settings.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigTotem> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveConfig.PveTotem item)
        => (await item.Totem()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigTotem>();

    /// <summary>
    /// Get information needed to join this cluster over the connected node.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="node">The node for which the joinee gets the nodeinfo. </param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigJoin> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveConfig.PveJoin item, string node = null)
        => (await item.JoinInfo(node)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterConfigJoin>();

    /// <summary>
    /// Get Firewall options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterFirewallOptions> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveCluster.PveFirewall.PveOptions item)
        => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterFirewallOptions>();

    /// <summary>
    /// Read DNS settings.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDns> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDns item)
        => (await item.Dns()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDns>();

    /// <summary>
    /// Read node status
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStatus> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStatus item)
        => (await item.Status()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStatus>();

    /// <summary>
    /// API version details
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVersion> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveVersion item)
        => (await item.Version()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeVersion>();

    /// <summary>
    /// Read storage status.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorage> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveStatus item)
        => (await item.ReadStatus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeStorage>();

    /// <summary>
    /// Read subscription info.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeSubscription> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveSubscription item)
        => (await item.Get()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeSubscription>();

    /// <summary>
    /// Get SMART Health of a disk.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="disk">Block device name</param>
    /// <param name="healthonly">If true returns only the health status</param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskSmart> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveSmart item, string disk, bool? healthonly = null)
        => (await item.Smart(disk, healthonly)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskSmart>();

    /// <summary>
    /// Get details about a zpool.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskZfsDetail> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveDisks.PveZfs.PveNameItem item)
        => (await item.Detail()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeDiskZfsDetail>();

    /// <summary>
    /// Get host firewall options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeFirewallOptions> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveFirewall.PveOptions item)
        => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeFirewallOptions>();

    /// <summary>
    /// Read server time and time zone settings.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTime> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveTime item)
        => (await item.Time()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTime>();

    /// <summary>
    /// Read task status.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTaskStatus> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveTasks.PveUpidItem.PveStatus item)
        => (await item.ReadTaskStatus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeTaskStatus>();

    /// <summary>
    /// Get APT repository information.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptRepositories> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveApt.PveRepositories item)
        => (await item.Repositories()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Node.NodeAptRepositories>();

    /// <summary>
    /// Get pool configuration (deprecated, no support for nested pools, use 'GET /pools/?poolid={poolid}').
    /// </summary>
    /// <param name="item"></param>
    /// <param name="type">
    ///   Enum: qemu,lxc,storage</param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Pool.PoolDetail> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PvePools.PvePoolidItem item, string type = null)
        => (await item.ReadPool(type)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Pool.PoolDetail>();

    /// <summary>
    /// Get the virtual machine configuration with pending configuration changes applied. Set the 'current' parameter to get the current configuration instead.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="current">Get current values (instead of pending values).</param>
    /// <param name="snapshot">Fetch config values from given snapshot.</param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmConfigQemu> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveConfig item, bool? current = null, string snapshot = null)
        => (await item.VmConfig(current, snapshot)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmConfigQemu>();

    /// <summary>
    /// Execute get-fsinfo.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetFsInfo> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetFsinfo item)
        => (await item.GetFsinfo()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetFsInfo>();

    /// <summary>
    /// Execute get-host-name.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetHostName> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetHostName item)
        => (await item.GetHostName()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetHostName>();

    /// <summary>
    /// Execute network-get-interfaces.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentNetworkGetInterfaces> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveNetworkGetInterfaces item)
        => (await item.NetworkGetInterfaces()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentNetworkGetInterfaces>();

    /// <summary>
    /// Execute info.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentInfo> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveInfo item)
        => (await item.Info()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentInfo>();

    /// <summary>
    /// Execute get-osinfo.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentOsInfo> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetOsinfo item)
        => (await item.GetOsinfo()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentOsInfo>();

    /// <summary>
    /// Execute get-vcpus.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetVCpus> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetVcpus item)
        => (await item.GetVcpus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetVCpus>();

    /// <summary>
    /// Execute get-timezone.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetTimeZone> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveAgent.PveGetTimezone item)
        => (await item.GetTimezone()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuAgentGetTimeZone>();

    /// <summary>
    /// Get virtual machine status.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuStatusCurrent> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveStatus.PveCurrent item)
        => (await item.VmStatus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmQemuStatusCurrent>();

    /// <summary>
    /// Get VM firewall options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmFirewallOptions> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveOptions item)
        => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmFirewallOptions>();

    /// <summary>
    /// Get container configuration.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="current">Get current values (instead of pending values).</param>
    /// <param name="snapshot">Fetch config values from given snapshot.</param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmConfigLxc> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveConfig item, bool? current = null, string snapshot = null)
        => (await item.VmConfig(current, snapshot)).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmConfigLxc>();

    /// <summary>
    /// Get virtual machine status.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmLxcStatusCurrent> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveStatus.PveCurrent item)
        => (await item.VmStatus()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmLxcStatusCurrent>();

    /// <summary>
    /// Get VM firewall options.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmFirewallOptions> GetAsync(this Corsinvest.ProxmoxVE.Api.PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveOptions item)
        => (await item.GetOptions()).ToModel<Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmFirewallOptions>();



}