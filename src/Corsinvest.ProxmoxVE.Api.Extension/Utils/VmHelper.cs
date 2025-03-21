/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using System.ComponentModel;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

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
    /// <param name="noVnc"></param>
    /// <param name="xtermJs"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> GetConsoleNoVncAsync(PveClient client,
                                                                    string node,
                                                                    long vmId,
                                                                    string vmName,
                                                                    VmType vmType,
                                                                    bool noVnc,
                                                                    bool xtermJs,
                                                                    string parameters = null)
        => await GetConsoleNoVncAsync(client,
                                      node,
                                      vmId,
                                      vmName,
                                      NoVncHelper.GetConsoleType(vmType),
                                      noVnc,
                                      xtermJs,
                                      parameters);

    /// <summary>
    /// Get console NoVnc
    /// </summary>
    /// <param name="client"></param>
    /// <param name="node"></param>
    /// <param name="vmId"></param>
    /// <param name="vmName"></param>
    /// <param name="console"></param>
    /// <param name="noVnc"></param>
    /// <param name="xtermJs"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> GetConsoleNoVncAsync(PveClient client,
                                                                       string node,
                                                                       long vmId,
                                                                       string vmName,
                                                                       string console,
                                                                       bool noVnc,
                                                                       bool xtermJs,
                                                                       string parameters = null)
    {
        using var httpClient = client.GetHttpClient();
        return await httpClient.GetAsync(NoVncHelper.GetConsoleUrl(client.Host,
                                                                   client.Port,
                                                                   console,
                                                                   node,
                                                                   vmId,
                                                                   vmName,
                                                                   noVnc,
                                                                   xtermJs,
                                                                   parameters));
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
                    && data.VmId >= rangeMin
                    && data.VmId <= rangeMax;
        }
        else if (long.TryParse(vmIdOrName, out var vmId))
        {
            return data.VmId == vmId;
        }
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
            else
            {
                return name == vmIdOrNameLower;
            }
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
    public static async Task<Result> ChangeStatusVmAsync(PveClient client, string node, VmType vmType, long vmId, VmStatus status)
        => vmType switch
        {
            VmType.Qemu => status switch
            {
                VmStatus.Reboot => await client.Nodes[node].Qemu[vmId].Status.Reboot.VmReboot(),
                VmStatus.Resume => await client.Nodes[node].Qemu[vmId].Status.Resume.VmResume(),
                VmStatus.Reset => await client.Nodes[node].Qemu[vmId].Status.Reset.VmReset(),
                VmStatus.Shutdown => await client.Nodes[node].Qemu[vmId].Status.Shutdown.VmShutdown(),
                VmStatus.Start => await client.Nodes[node].Qemu[vmId].Status.Start.VmStart(),
                VmStatus.Stop => await client.Nodes[node].Qemu[vmId].Status.Stop.VmStop(),
                VmStatus.Suspend => await client.Nodes[node].Qemu[vmId].Status.Suspend.VmSuspend(),
                _ => throw new InvalidEnumArgumentException(),
            },
            VmType.Lxc => status switch
            {
                VmStatus.Reboot => await client.Nodes[node].Lxc[vmId].Status.Reboot.VmReboot(),
                VmStatus.Resume => await client.Nodes[node].Lxc[vmId].Status.Resume.VmResume(),
                VmStatus.Reset => throw new InvalidEnumArgumentException("Not possible in Container"),
                VmStatus.Shutdown => await client.Nodes[node].Lxc[vmId].Status.Shutdown.VmShutdown(),
                VmStatus.Start => await client.Nodes[node].Lxc[vmId].Status.Start.VmStart(),
                VmStatus.Stop => await client.Nodes[node].Lxc[vmId].Status.Stop.VmStop(),
                VmStatus.Suspend => await client.Nodes[node].Lxc[vmId].Status.Suspend.VmSuspend(),
                _ => throw new InvalidEnumArgumentException(),
            },
            _ => throw new InvalidEnumArgumentException(),
        };

    /// <summary>
    /// Get Vms Jolly Keys
    /// </summary>
    /// <param name="client"></param>
    /// <param name="addAll"></param>
    /// <param name="addNodes"></param>
    /// <param name="addPools"></param>
    /// <param name="addVmId"></param>
    /// <param name="addVmName"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<string>> GetVmsJollyKeysAsync(PveClient client,
                                                                       bool addAll,
                                                                       bool addNodes,
                                                                       bool addPools,
                                                                       bool addVmId,
                                                                       bool addVmName)
    {
        var vmIds = new List<string>();
        var resources = await client.GetResourcesAsync(ClusterResourceType.All);

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

    /// <summary>
    /// Populate VM/CT Os INfo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static async Task PopulateVmOsInfoAsync<T>(PveClient client, T item)
         where T : IClusterResourceVm, IClusterResourceVmOsInfo
    {
        //set info Vm
        switch (item.VmType)
        {
            case VmType.Qemu:
                var qemuApi = client.Nodes[item.Node].Qemu[item.VmId];
                var qemuConfig = await qemuApi.Config.GetAsync();
                item.OsType = qemuConfig.VmOsType;
                item.OsVersion = qemuConfig.OsTypeDecode;

                if (item.IsRunning)
                {
                    if (qemuConfig.AgentEnabled)
                    {
                        try
                        {
                            item.VmQemuAgentOsInfo = await qemuApi.Agent.GetOsinfo.GetAsync();
                            item.OsVersion = item.VmQemuAgentOsInfo?.Result?.OsVersion;
                            item.HostName = (await qemuApi.Agent.GetHostName.GetAsync())?.Result?.HostName ?? "Error Agent data!";
                        }
                        catch
                        {
                            item.HostName = "Error Agent data!";
                        }
                    }
                    else
                    {
                        item.HostName = "Agent not enabled!";
                    }
                }
                break;

            case VmType.Lxc:
                var lxcApi = client.Nodes[item.Node].Lxc[item.VmId];
                var lxcConfig = await lxcApi.Config.GetAsync(true);
                item.HostName = lxcConfig.Hostname;
                item.OsVersion = lxcConfig.OsTypeDecode;
                item.OsType = lxcConfig.VmOsType;
                break;

            default: throw new InvalidEnumArgumentException();
        }
    }
}