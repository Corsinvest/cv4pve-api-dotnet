/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using System.ComponentModel;

namespace Corsinvest.ProxmoxVE.Api.Extension;

/// <summary>
/// Models extensions
/// </summary>
public static class ModelsExtensions
{
    /// <summary>
    /// Resources index (cluster wide).
    /// </summary>
    /// <param name="item"></param>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static async Task<IEnumerable<ClusterResource>> GetAsync(this PveClient.PveCluster.PveResources item, ClusterResourceType resourceType)
        => await item.GetAsync(resourceType switch
        {
            ClusterResourceType.Storage or ClusterResourceType.Node or ClusterResourceType.Vm => resourceType.ToString().ToLower(),
            ClusterResourceType.All => null,
            _ => throw new InvalidEnumArgumentException(),
        });

    /// <summary>
    /// Read storage RRD statistics.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="dataTimeFrame"></param>
    /// <param name="dataConsolidation"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<NodeStorageRrdData>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveRrddata item,
                                                                       RrdDataTimeFrame dataTimeFrame,
                                                                       RrdDataConsolidation dataConsolidation)
        => await item.GetAsync(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

    /// <summary>
    /// Read node RRD statistics
    /// </summary>
    /// <param name="item"></param>
    /// <param name="dataTimeFrame"></param>
    /// <param name="dataConsolidation"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<NodeRrdData>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveRrddata item,
                                                                RrdDataTimeFrame dataTimeFrame,
                                                                RrdDataConsolidation dataConsolidation)
        => await item.GetAsync(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

    /// <summary>
    /// Read task log
    /// </summary>
    /// <param name="item"></param>
    /// <param name="limit">The maximum amount of lines that should be printed.</param>
    /// <param name="start">The line number to start printing at.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<string>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveTasks.PveUpidItem.PveLog item,
                                                           int? limit = null,
                                                           int? start = null)
        => (await item.ReadTaskLog(null, limit, start)).ToLogs();

    /// <summary>
    /// Read replication job log.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="limit">The maximum amount of lines that should be printed.</param>
    /// <param name="start">The line number to start printing at.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<string>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveReplication.PveIdItem.PveLog item,
                                                           int? limit = null,
                                                           int? start = null)
        => (await item.ReadJobLog(limit, start)).ToLogs();

    /// <summary>
    /// Get backups in all storages
    /// </summary>
    /// <param name="item"></param>
    /// <param name="vmId"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<NodeStorageContent>> GetBackupsInAllStoragesAsync(this PveClient.PveNodes.PveNodeItem item,
                                                                                           int? vmId = null)
    {
        var ret = new List<NodeStorageContent>();
        foreach (var item1 in await item.Storage.GetAsync(enabled: true, content: "backup"))
        {
            if (item1.Active)
            {
                ret.AddRange(await item.Storage[item1.Storage].Content.GetAsync("backup", vmId));
            }
        }

        return ret;
    }

    /// <summary>
    /// Read VM RRD statistics
    /// </summary>
    /// <param name="item"></param>
    /// <param name="dataTimeFrame"></param>
    /// <param name="dataConsolidation"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<VmRrdData>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveRrddata item,
                                                              RrdDataTimeFrame dataTimeFrame,
                                                              RrdDataConsolidation dataConsolidation)
        => await item.GetAsync(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

    /// <summary>
    /// Read VM RRD statistics
    /// </summary>
    /// <param name="item"></param>
    /// <param name="dataTimeFrame"></param>
    /// <param name="dataConsolidation"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<VmRrdData>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveRrddata item,
                                                              RrdDataTimeFrame dataTimeFrame,
                                                              RrdDataConsolidation dataConsolidation)
        => await item.GetAsync(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

    #region Spice
    /// <summary>
    /// Get file for SPICE client using spice config
    /// </summary>
    /// <param name="item"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static async Task<(bool Success, string ReasonPhrase, string Content)> GetSpiceFileVVAsync(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveSpiceproxy item,
                                                                                                      string proxy)
        => CreateSpiceFileVV(await item.Spiceproxy(proxy));

    /// <summary>
    /// Get file for SPICE client using spice config
    /// </summary>
    /// <param name="item"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static async Task<(bool Success, string ReasonPhrase, string Content)> GetSpiceFileVVAsync(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveSpiceproxy item,
                                                                                                      string proxy)
        => CreateSpiceFileVV(await item.Spiceproxy(proxy));

    /// <summary>
    /// Get file for SPICE client using spice config
    /// </summary>
    /// <param name="item"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static async Task<(bool Success, string ReasonPhrase, string Content)> GetSpiceFileVVAsync(this PveClient.PveNodes.PveNodeItem.PveSpiceshell item,
                                                                                                      string proxy)
        => CreateSpiceFileVV(await item.Spiceshell(proxy: proxy));

    private static (bool Success, string ReasonPhrase, string Content) CreateSpiceFileVV(Result response)
    {
        var content = response.IsSuccessStatusCode
                        ? "[virt-viewer]" +
                            Environment.NewLine +
                            string.Join(Environment.NewLine, ((IDictionary<string, object>)response.ToData()).Select(a => $"{a.Key}={a.Value}"))
                        : string.Empty;

        return (response.IsSuccessStatusCode, response.ReasonPhrase, content);
    }
    #endregion

    /// <summary>
    /// Retrieve effective permissions of given user/token.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="path">Only dump this specific path, not the whole tree.</param>
    /// <param name="userid">User ID or full API token ID</param>
    /// <returns></returns>
    public static async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetPermissionsAsync(this PveClient.PveAccess.PvePermissions item,
                                                                                                     string path = null,
                                                                                                     string userid = null)
    {
        var result = await item.Permissions(path, userid);

        var permissions = new Dictionary<string, IReadOnlyList<string>>();

        foreach (var data in (IDictionary<string, object>)result.ToData())
        {
            permissions.Add(data.Key,
                            ((IDictionary<string, object>)data.Value)
                                .Select(a => a.Key)
                                .ToList()
                                .AsReadOnly());
        }

        return permissions;
    }
}