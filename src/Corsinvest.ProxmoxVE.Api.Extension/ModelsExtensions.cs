/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
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
    public static async Task<IEnumerable<NodeStorageRrdData>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveRrddata item,
                                                                       RrdDataTimeFrame dataTimeFrame,
                                                                       RrdDataConsolidation dataConsolidation)
        => await item.GetAsync(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

    /// <summary>
    /// Read node RRD statistics
    /// </summary>
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
    public static async Task<IEnumerable<string>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveTasks.PveUpidItem.PveLog item,
                                                       int? limit = null,
                                                       int? start = null)
        => (await item.ReadTaskLog(null, limit, start)).ToLogs();

    /// <summary>
    /// Read Journal
    /// </summary>
    /// <param name="item"></param>
    /// <param name="endcursor">End before the given Cursor. Conflicts with 'until'</param>
    /// <param name="lastentries">Limit to the last X lines. Conflicts with a range.</param>
    /// <param name="since">Display all log since this UNIX epoch. Conflicts with 'startcursor'.</param>
    /// <param name="startcursor">Start after the given Cursor. Conflicts with 'since'</param>
    /// <param name="until">Display all log until this UNIX epoch. Conflicts with 'endcursor'.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<string>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveJournal item,
                                                           string endcursor = null,
                                                           int? lastentries = null,
                                                           int? since = null,
                                                           string startcursor = null,
                                                           int? until = null)
    {
        var result = await item.Journal(endcursor, lastentries, since, startcursor, until);
        return result.ResponseToDictionary.TryGetValue("data", out var _)
                ? [.. result.ToEnumerable().OfType<string>().Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("s="))]
                : [];
    }

    /// <summary>
    /// Read firewall log
    /// </summary>
    /// <param name="item"></param>
    /// <param name="limit"></param>
    /// <param name="since">Display log since this UNIX epoch.</param>
    /// <param name="start"></param>
    /// <param name="until">Display log until this UNIX epoch.</param>
    public static async Task<IEnumerable<string>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveFirewall.PveLog item,
                                                           int? limit = null,
                                                           int? since = null,
                                                           int? start = null,
                                                           int? until = null)
        => (await item.Log(limit, since, start, until)).ToLogs();

    /// <summary>
    /// Read firewall log
    /// </summary>
    /// <param name="item"></param>
    /// <param name="limit"></param>
    /// <param name="since">Display log since this UNIX epoch.</param>
    /// <param name="start"></param>
    /// <param name="until">Display log until this UNIX epoch.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<string>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveFirewall.PveLog item,
                                                           int? limit = null,
                                                           int? since = null,
                                                           int? start = null,
                                                           int? until = null)
        => (await item.Log(limit, since, start, until)).ToLogs();

    /// <summary>
    /// Read firewall log
    /// </summary>
    /// <param name="item"></param>
    /// <param name="limit"></param>
    /// <param name="since">Display log since this UNIX epoch.</param>
    /// <param name="start"></param>
    /// <param name="until">Display log until this UNIX epoch.</param>
    /// <returns></returns>
    public static async Task<IEnumerable<string>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveFirewall.PveLog item,
                                                           int? limit = null,
                                                           int? since = null,
                                                           int? start = null,
                                                           int? until = null)
        => (await item.Log(limit, since, start, until)).ToLogs();

    /// <summary>
    /// Read replication job log.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="limit">The maximum amount of lines that should be printed.</param>
    /// <param name="start">The line number to start printing at.</param>
    public static async Task<IEnumerable<string>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveReplication.PveIdItem.PveLog item,
                                                       int? limit = null,
                                                       int? start = null)
        => (await item.ReadJobLog(limit, start)).ToLogs();


    /// <summary>
    /// Get backups in all storages
    /// </summary>
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
    public static async Task<IEnumerable<VmRrdData>> GetAsync(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveRrddata item,
                                                              RrdDataTimeFrame dataTimeFrame,
                                                              RrdDataConsolidation dataConsolidation)
        => await item.GetAsync(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

    /// <summary>
    /// Read VM RRD statistics
    /// </summary>
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
    public static async Task<(bool Success, string ReasonPhrase, string Content)> GetSpiceFileVVAsync(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveSpiceproxy item,
                                                                                                  string proxy)
    => CreateSpiceFileVV(await item.Spiceproxy(proxy));

    /// <summary>
    /// Get file for SPICE client using spice config
    /// </summary>
    /// <param name="item"></param>
    /// <param name="proxy"></param>
    public static async Task<(bool Success, string ReasonPhrase, string Content)> GetSpiceFileVVAsync(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveSpiceproxy item,
                                                                                                  string proxy)
    => CreateSpiceFileVV(await item.Spiceproxy(proxy));

    /// <summary>
    /// Get file for SPICE client using spice config
    /// </summary>
    /// <param name="item"></param>
    /// <param name="proxy"></param>
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