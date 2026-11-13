/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Microsoft.Extensions.Logging;

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
    public static Task<HttpResponseMessage> GetConsoleNoVncAsync(PveClient client,
                                                                   string node,
                                                                   long vmId,
                                                                   string vmName,
                                                                   VmType vmType,
                                                                   bool noVnc,
                                                                   bool xtermJs,
                                                                   string parameters = null)
        => GetConsoleNoVncAsync(client,
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
    public static async Task<HttpResponseMessage> GetConsoleNoVncAsync(PveClient client,
                                                                   string node,
                                                                   long vmId,
                                                                   string vmName,
                                                                   string console,
                                                                   bool noVnc,
                                                                   bool xtermJs,
                                                                   string parameters = null)
    {
        var url = NoVncHelper.GetConsoleUrl(client.Host,
                                            client.Port,
                                            console,
                                            node,
                                            vmId,
                                            vmName,
                                            noVnc,
                                            xtermJs,
                                            parameters);

        var httpClient = client.GetHttpClient();
        var request = client.CreateHttpRequestMessage(HttpMethod.Get, url);
        return await httpClient.SendAsync(request);
    }
    #endregion

    /// <summary>
    /// Select VM/CT from a jolly: comma separated items, applied to <paramref name="vms"/>.
    /// <para>'@all' or 'all' all VM/CT; '@all-node', 'all-node' or '@node-node' all VM/CT on a node;
    /// '@pool-name' all VM/CT in a pool and its nested pools; '@tag-name' all VM/CT with a tag;
    /// id, name, range '100:110', '%text%' (contains), 'text%' (starts with), '%text' (ends with).</para>
    /// <para>An item starting with '-' excludes the VM/CT it selects.</para>
    /// </summary>
    /// <param name="vms">All VM/CT of the cluster.</param>
    /// <param name="jolly">Selection.</param>
    /// <param name="getPoolMemberIdsAsync">Ids ('qemu/100', 'lxc/101') of the members of a pool and its nested pools.</param>
    /// <returns>Items of <paramref name="vms"/>, each once.</returns>
    public static async Task<IEnumerable<IClusterResourceVm>> GetVmsFromJollyAsync(IEnumerable<IClusterResourceVm> vms,
                                                                                    string jolly,
                                                                                    Func<string, Task<IEnumerable<string>>> getPoolMemberIdsAsync)
    {
        async Task<IEnumerable<IClusterResourceVm>> GetVmsFromIdAsync(string id)
        {
            if (id == "all" || id == "@all")
            {
                //all nodes
                return vms;
            }
            else if (id.StartsWith("all-") || id.StartsWith("@all-") || id.StartsWith("@node-"))
            {
                //all in specific node
                var nodeName = id[(id.IndexOf('-') + 1)..];
                return vms.Where(a => string.Equals(a.Node, nodeName, StringComparison.OrdinalIgnoreCase));
            }
            else if (id.StartsWith("@pool-"))
            {
                //all in specific pool and its nested pools
                //return the items of 'vms', not the pool members: Distinct and Remove compare by reference
                var memberIds = (await getPoolMemberIdsAsync(id[6..])).ToHashSet();
                return vms.Where(a => memberIds.Contains(a.Id));
            }
            else if (id.StartsWith("@tag-"))
            {
                //all in specific tag
                var tagName = id[5..];
                return vms.Where(a => (a.Tags + string.Empty).Split(';').Contains(tagName, StringComparer.OrdinalIgnoreCase));
            }
            else
            {
                return vms.Where(a => CheckIdOrName(a, id));
            }
        }

        var ret = new List<IClusterResourceVm>();

        //add: exclusions are applied below, here '-100:110' would be read as the range -100 to 110
        foreach (var id in jolly.Split(',').Where(a => !a.StartsWith('-'))) { ret.AddRange(await GetVmsFromIdAsync(id)); }

        ret = [.. ret.Distinct()];

        //exclude data
        foreach (var id in jolly.Split(',').Where(a => a.StartsWith('-')).Select(a => a[1..]))
        {
            foreach (var item in await GetVmsFromIdAsync(id))
            {
                ret.Remove(item);
            }
        }

        return ret;
    }

    /// <summary>
    /// Vm check Id or Name
    /// </summary>
    public static bool CheckIdOrName(IClusterResourceVm data, string vmIdOrName)
    {
        if (vmIdOrName.Contains(':'))
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
            //string check name: Proxmox VE gives no name for a guest without statistics
            if (data.Name == null) { return false; }

            var name = data.Name.ToLower();
            var vmIdOrNameLower = vmIdOrName.Replace("%", string.Empty).ToLower();
            if (vmIdOrName.Contains('%'))
            {
                if (vmIdOrName.StartsWith('%') && vmIdOrName.EndsWith('%')) { return name.Contains(vmIdOrNameLower); }
                else if (vmIdOrName.StartsWith('%')) { return name.EndsWith(vmIdOrNameLower); }
                else if (vmIdOrName.EndsWith('%')) { return name.StartsWith(vmIdOrNameLower); }
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
        if (addVmId) { vmIds.AddRange(vms.Select(a => a.VmId + string.Empty).Order()); }
        if (addVmName) { vmIds.AddRange(vms.Select(a => a.Name).Order()); }

        return vmIds.Distinct();
    }

    /// <summary>
    /// Populate VM/CT Os INfo
    /// </summary>
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

    /// <summary>
    /// Safely destroys a VM or LXC container.
    /// If the guest is running, stops it first and waits for the stop task to complete to prevent disk locking contention.
    /// Initiates destroy with purge and destroy-unreferenced-disks enabled by default, and awaits the destroy background task.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmType">VM type (Qemu or Lxc).</param>
    /// <param name="vmId">VM or Container ID.</param>
    /// <param name="stopIfRunning">Gracefully stop the guest before destroying if it is running. Default true.</param>
    /// <param name="purge">Remove from backup jobs, replication, and HA. Default true.</param>
    /// <param name="destroyUnreferencedDisks">Remove disks with matching VMID not referenced in config. Default true.</param>
    /// <param name="skiplock">For QEMU: ignore locks. Default null.</param>
    /// <param name="force">For LXC: force destroy. Default null.</param>
    /// <param name="timeout">Timeout in milliseconds to wait for the destroy task. Default 300,000 (5 minutes). 0 to not wait.</param>
    /// <param name="stopTimeout">Timeout in milliseconds to wait for the stop task if running. Default 120,000 (2 minutes).</param>
    /// <param name="wait">Polling interval in milliseconds when awaiting tasks. Default 500.</param>
    /// <returns>The Result of the destroy API operation.</returns>
    public static async Task<Result> DestroyVmAsync(
        PveClient client,
        string node,
        VmType vmType,
        long vmId,
        bool stopIfRunning = true,
        bool purge = true,
        bool destroyUnreferencedDisks = true,
        bool? skiplock = null,
        bool? force = null,
        long timeout = 300000,
        long stopTimeout = 120000,
        int wait = 500)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (string.IsNullOrWhiteSpace(node)) { throw new ArgumentException("Node cannot be null or empty.", nameof(node)); }
        if (vmId <= 0) { throw new ArgumentOutOfRangeException(nameof(vmId), "VM ID must be greater than zero."); }
        if (vmType != VmType.Qemu && vmType != VmType.Lxc)
        {
            throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType));
        }

        node = node.Trim();

        // 1. If running and stopIfRunning is requested, stop and wait
        if (stopIfRunning)
        {
            try
            {
                var status = await client.GetVmStatusAsync(node, vmType, vmId);
                if (status != null && (status.IsRunning || status.IsPaused))
                {
                    var stopResult = await ChangeStatusVmAsync(client, node, vmType, vmId, VmStatus.Stop);
                    if (stopResult.IsSuccessStatusCode)
                    {
                        var stopped = await client.WaitForTaskToFinishAsync(stopResult, wait, stopTimeout);
                        if (!stopped)
                        {
                            throw new TimeoutException($"Timed out waiting {stopTimeout}ms for {vmType} {vmId} on node '{node}' to stop.");
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is not TimeoutException)
            {
                // If status check or stop fails, log warning and proceed to destroy attempt
                var logger = client.LoggerFactory.CreateLogger(typeof(VmHelper));
                logger.LogWarning(ex, "Could not verify stop status for {VmType} {VmId} before destroying.", vmType, vmId);
            }
        }

        // 2. Issue destroy
        var destroyResult = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].DestroyVm(
                destroy_unreferenced_disks: destroyUnreferencedDisks,
                purge: purge,
                skiplock: skiplock),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].DestroyVm(
                destroy_unreferenced_disks: destroyUnreferencedDisks,
                force: force,
                purge: purge),
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType)),
        };

        // 3. Await destroy task if requested
        if (destroyResult.IsSuccessStatusCode && timeout > 0)
        {
            var finished = await client.WaitForTaskToFinishAsync(destroyResult, wait, timeout);
            if (!finished)
            {
                throw new TimeoutException($"Timed out waiting {timeout}ms for destroy task of {vmType} {vmId} on node '{node}' to finish.");
            }

            // 4. Check destroy task log for warnings (e.g. unremoved disks)
            var upid = GetTaskId(destroyResult);
            if (!string.IsNullOrEmpty(upid))
            {
                try
                {
                    var logLines = await client.Nodes[node].Tasks[upid].Log.GetAsync();
                    var logger = client.LoggerFactory.CreateLogger(typeof(VmHelper));
                    foreach (var line in logLines)
                    {
                        if (line.Contains("could not remove disk", StringComparison.OrdinalIgnoreCase) ||
                            line.Contains("unable to remove", StringComparison.OrdinalIgnoreCase))
                        {
                            logger.LogWarning("VM {VmId} destroy completed with warning: {Line}", vmId, line);
                        }
                    }
                }
                catch
                {
                    // Ignore task log read failures
                }
            }
        }

        return destroyResult;
    }

    private static string GetTaskId(Result result)
        => result is { IsSuccessStatusCode: true, ResponseInError: false, ResponseHasData: true }
           && result.ResponseToDictionary.TryGetValue("data", out var data)
           && data is string task && task.StartsWith("UPID:")
            ? task
            : null;
}
