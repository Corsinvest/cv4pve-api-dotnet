/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Extension
{
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
        public static async Task<IEnumerable<ClusterResource>> Get(this PveClient.PveCluster.PveResources item, ClusterResourceType resourceType)
            => await item.Get(resourceType switch
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
        public static async Task<IEnumerable<NodeStorageRrdData>> Get(this PveClient.PveNodes.PveNodeItem.PveStorage.PveStorageItem.PveRrddata item,
                                                                      RrdDataTimeFrame dataTimeFrame,
                                                                      RrdDataConsolidation dataConsolidation)
            => await item.Get(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

        /// <summary>
        /// Read node RRD statistics
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dataTimeFrame"></param>
        /// <param name="dataConsolidation"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeRrdData>> Get(this PveClient.PveNodes.PveNodeItem.PveRrddata item,
                                                               RrdDataTimeFrame dataTimeFrame,
                                                               RrdDataConsolidation dataConsolidation)
            => await item.Get(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

        /// <summary>
        /// Read task log
        /// </summary>
        /// <param name="item"></param>
        /// <param name="limit">The maximum amount of lines that should be printed.</param>
        /// <param name="start">The line number to start printing at.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> Get(this PveClient.PveNodes.PveNodeItem.PveTasks.PveUpidItem.PveLog item,
                                                          int? limit = null,
                                                          int? start = null)
            => (await item.ReadTaskLog(null, limit, start)).ToEnumerable().OrderBy(a => a.n).Select(a => a.t as string);

        /// <summary>
        /// Get backups in all storages
        /// </summary>
        /// <param name="item"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<NodeStorageContent>> GetBackupsInAllStorages(this PveClient.PveNodes.PveNodeItem item,
                                                                                          int? vmId = null)
        {
            var ret = new List<NodeStorageContent>();
            foreach (var item1 in await item.Storage.Get(enabled: true, content: "backup"))
            {
                ret.AddRange(await item.Storage[item1.Storage].Content.Get("backup", vmId));
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
        public static async Task<IEnumerable<VmRrdData>> Get(this PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem.PveRrddata item,
                                                             RrdDataTimeFrame dataTimeFrame,
                                                             RrdDataConsolidation dataConsolidation)
            => await item.Get(dataTimeFrame.GetValue(), dataConsolidation.GetValue());

        /// <summary>
        /// Read VM RRD statistics
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dataTimeFrame"></param>
        /// <param name="dataConsolidation"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<VmRrdData>> Get(this PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem.PveRrddata item,
                                                             RrdDataTimeFrame dataTimeFrame,
                                                             RrdDataConsolidation dataConsolidation)
            => await item.Get(dataTimeFrame.GetValue(), dataConsolidation.GetValue());
    }
}