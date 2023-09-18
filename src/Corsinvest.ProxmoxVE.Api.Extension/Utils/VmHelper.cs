/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    /// <summary>
    /// Vm Helper
    /// </summary>
    public static class VmHelper
    {
        #region NoVnc
        /// <summary>
        /// Get console NoVnc
        /// </summary>
        /// <param name="client"></param>
        /// <param name="node"></param>
        /// <param name="vmId"></param>
        /// <param name="vmName"></param>
        /// <param name="vmType"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetConsoleNoVnc(PveClient client, string node, long vmId, string vmName, VmType vmType)
            => await GetConsoleNoVnc(client, node, vmId, vmName, NoVncHelper.GetConsoleType(vmType));

        /// <summary>
        /// Get console NoVnc
        /// </summary>
        /// <param name="client"></param>
        /// <param name="node"></param>
        /// <param name="vmId"></param>
        /// <param name="vmName"></param>
        /// <param name="console"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetConsoleNoVnc(PveClient client, string node, long vmId, string vmName, string console)
        {
            using var httpClient = client.GetHttpClient();
            return await httpClient.GetAsync(NoVncHelper.GetConsoleUrl(client.Host,
                                                                          client.Port,
                                                                          console,
                                                                          node,
                                                                          vmId,
                                                                          vmName));
        }
        #endregion

        /// <summary>
        /// Vm check Id or Name
        /// </summary>
        /// <param name="data"></param>
        /// <param name="vmIdOrName"></param>
        /// <returns></returns>
        public static bool CheckIdOrName(IClusterResourceVm data, string vmIdOrName)
        {
            if (vmIdOrName.Contains(":"))
            {
                //range number
                var range = vmIdOrName.Split(':');
                return !(range.Length != 2
                         || !long.TryParse(range[0], out var rangeMin)
                         || !long.TryParse(range[1], out var rangeMax))
                        && data.VmId >= rangeMin && data.VmId <= rangeMax;
            }
            else if (long.TryParse(vmIdOrName, out var vmId)) { return data.VmId == vmId; }
            else
            {
                //string check name
                var name = data.Name.ToLower();
                var vmIdOrNameLower = vmIdOrName.Replace("%", "").ToLower();
                if (vmIdOrName.Contains("%"))
                {
                    if (vmIdOrName.StartsWith("%") && vmIdOrName.EndsWith("%")) { return name.Contains(vmIdOrNameLower); }
                    else if (vmIdOrName.StartsWith("%")) { return name.StartsWith(vmIdOrNameLower); }
                    else if (vmIdOrName.EndsWith("%")) { return name.EndsWith(vmIdOrNameLower); }
                    else { return false; }
                }
                else { return name == vmIdOrNameLower; }
            }
        }

        /// <summary>
        /// Change Status Vm
        /// </summary>
        /// <param name="client"></param>
        /// <param name="node"></param>
        /// <param name="vmType"></param>
        /// <param name="vmId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static async Task<Result> ChangeStatusVm(PveClient client, string node, VmType vmType, long vmId, VmStatus status)
        {
            PveClient.PveNodes.PveNodeItem.PveQemu.PveVmidItem GetQemuApi() => client.Nodes[node].Qemu[vmId];
            PveClient.PveNodes.PveNodeItem.PveLxc.PveVmidItem GetLxcApi() => client.Nodes[node].Lxc[vmId];

            return vmType switch
            {
                VmType.Qemu => status switch
                {
                    VmStatus.Reboot => await GetQemuApi().Status.Reboot.VmReboot(),
                    VmStatus.Resume => await GetQemuApi().Status.Resume.VmResume(),
                    VmStatus.Reset => await GetQemuApi().Status.Reset.VmReset(),
                    VmStatus.Shutdown => await GetQemuApi().Status.Shutdown.VmShutdown(),
                    VmStatus.Start => await GetQemuApi().Status.Start.VmStart(),
                    VmStatus.Stop => await GetQemuApi().Status.Stop.VmStop(),
                    VmStatus.Suspend => await GetQemuApi().Status.Suspend.VmSuspend(),
                    _ => throw new InvalidEnumArgumentException(),
                },
                VmType.Lxc => status switch
                {
                    VmStatus.Reboot => await GetLxcApi().Status.Reboot.VmReboot(),
                    VmStatus.Resume => await GetLxcApi().Status.Resume.VmResume(),
                    VmStatus.Reset => throw new InvalidEnumArgumentException("Not possible in Container"),
                    VmStatus.Shutdown => await GetLxcApi().Status.Shutdown.VmShutdown(),
                    VmStatus.Start => await GetLxcApi().Status.Start.VmStart(),
                    VmStatus.Stop => await GetLxcApi().Status.Stop.VmStop(),
                    VmStatus.Suspend => await GetLxcApi().Status.Suspend.VmSuspend(),
                    _ => throw new InvalidEnumArgumentException(),
                },
                _ => throw new InvalidEnumArgumentException(),
            };
        }

        /// <summary>
        /// Get vm status
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task<VmBaseStatusCurrent> GetVmStatus(PveClient client, IClusterResourceVm vm)
            => vm.VmType switch
            {
                VmType.Qemu => await client.Nodes[vm.Node].Qemu[vm.VmId].Status.Current.Get(),
                VmType.Lxc => await client.Nodes[vm.Node].Lxc[vm.VmId].Status.Current.Get(),
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>
        /// Get Vms Jolly Keys <see cref="ClientExtension.GetVms(PveClient, string)"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="addAll"></param>
        /// <param name="addNodes"></param>
        /// <param name="addPools"></param>
        /// <param name="addVmId"></param>
        /// <param name="addVmName"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> GetVmsJollyKeys(PveClient client,
                                                                      bool addAll,
                                                                      bool addNodes,
                                                                      bool addPools,
                                                                      bool addVmId,
                                                                      bool addVmName)
        {
            var vmIds = new List<string>();
            var resources = await client.GetResources(ClusterResourceType.All);

            if (addAll) { vmIds.Add("@all"); }

            if (addPools)
            {
                vmIds.AddRange(resources.Where(a => a.ResourceType == ClusterResourceType.Pool)
                                        .Select(a => $"@pool-{a.Pool}"));
            }

            if (addNodes)
            {
                vmIds.AddRange(resources.Where(a => a.ResourceType == ClusterResourceType.Node && a.IsOnline)
                                        .Select(a => $"@all-{a.Node}"));
            }

            var vms = resources.Where(a => a.ResourceType == ClusterResourceType.Vm && !a.IsUnknown);
            if (addVmId) { vmIds.AddRange(vms.Select(a => a.VmId + "").OrderBy(a => a)); }
            if (addVmName) { vmIds.AddRange(vms.Select(a => a.Name).OrderBy(a => a)); }

            return vmIds.Distinct();
        }
    }
}
