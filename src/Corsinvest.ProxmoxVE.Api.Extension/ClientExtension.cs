/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel;
using System.Dynamic;
using System.Net.Http.Headers;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Extension;

/// <summary>
/// Client extension
/// </summary>
public static class ClientExtension
{
    #region Cluster
    /// <summary>
    /// Get resources
    /// </summary>
    public static Task<IEnumerable<ClusterResource>> GetResourcesAsync(this PveClient client, ClusterResourceType resourceType)
        => client.Cluster.Resources.GetAsync(resourceType);

    /// <summary>
    /// Get resource type
    /// </summary>
    public static Task<IEnumerable<ClusterResource>> GetResourcesAsync(this PveClient client, string type)
        => client.Cluster.Resources.GetAsync(type);

    /// <summary>
    /// Get host and ip
    /// </summary>
    /// <returns>Dictionary host, ip</returns>
    public static async Task<IReadOnlyDictionary<string, string>> GetHostAndIpAsync(this PveClient client)
        => (await client.Cluster.Status.GetAsync())
                .Where(a => a.Type == PveConstants.KeyApiNode)
                .ToDictionary(a => a.Name, a => a.IpAddress);
    #endregion

    #region HA
    private static string FormatHaSid(long vmId, VmType vmType) => $"{(vmType == VmType.Lxc ? "ct" : "vm")}:{vmId}";

    /// <summary>
    /// Get a specific HA resource by its SID (e.g. vm:100 or ct:101).
    /// Returns null if the resource is not found; throws PveResultException on API errors.
    /// </summary>
    public static async Task<ClusterHaResource> GetHaResourceAsync(this PveClient client, string sid)
    {
        if (string.IsNullOrWhiteSpace(sid))
        {
            return null;
        }

        var resources = await client.Cluster.Ha.Resources.GetAsync();
        return resources?.FirstOrDefault(r => string.Equals(r.Sid, sid, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Get a specific HA resource by VMID and type.
    /// Returns null if the resource is not found; throws PveResultException on API errors.
    /// </summary>
    public static Task<ClusterHaResource> GetHaResourceAsync(this PveClient client, long vmId, VmType vmType = VmType.Qemu)
        => client.GetHaResourceAsync(FormatHaSid(vmId, vmType));

    /// <summary>
    /// Create an HA resource for a guest.
    /// When <paramref name="state"/> is null (default), Proxmox VE applies its server-side default ('started').
    /// </summary>
    public static Task<Result> CreateHaResourceAsync(
        this PveClient client,
        long vmId,
        VmType vmType = VmType.Qemu,
        string state = null,
        string comment = null,
        int? maxRestart = null,
        int? maxRelocate = null,
        bool? autoRebalance = null,
        bool? failback = null)
        => client.Cluster.Ha.Resources.Create(
            sid: FormatHaSid(vmId, vmType),
            state: state,
            comment: comment,
            max_restart: maxRestart,
            max_relocate: maxRelocate,
            auto_rebalance: autoRebalance,
            failback: failback
        );

    /// <summary>
    /// Ensure an HA resource exists: creates it if absent, or updates its configuration if it already exists.
    /// When <paramref name="state"/> is null (default), an existing resource retains its current state on update,
    /// while an absent resource is created with Proxmox VE's server-side default state ('started').
    /// </summary>
    public static async Task<Result> EnsureHaResourceAsync(
        this PveClient client,
        long vmId,
        VmType vmType = VmType.Qemu,
        string state = null,
        string comment = null,
        int? maxRestart = null,
        int? maxRelocate = null,
        bool? autoRebalance = null,
        bool? failback = null)
    {
        string sid = FormatHaSid(vmId, vmType);
        var existing = await client.GetHaResourceAsync(sid);
        if (existing == null)
        {
            return await client.CreateHaResourceAsync(
                vmId: vmId,
                vmType: vmType,
                state: state,
                comment: comment,
                maxRestart: maxRestart,
                maxRelocate: maxRelocate,
                autoRebalance: autoRebalance,
                failback: failback
            );
        }

        return await client.UpdateHaResourceAsync(
            vmId: vmId,
            vmType: vmType,
            state: state,
            comment: comment,
            maxRestart: maxRestart,
            maxRelocate: maxRelocate,
            autoRebalance: autoRebalance,
            failback: failback
        );
    }

    /// <summary>
    /// Update configuration of an HA resource.
    /// </summary>
    public static Task<Result> UpdateHaResourceAsync(
        this PveClient client,
        long vmId,
        VmType vmType = VmType.Qemu,
        string state = null,
        string comment = null,
        int? maxRestart = null,
        int? maxRelocate = null,
        bool? autoRebalance = null,
        bool? failback = null)
        => client.Cluster.Ha.Resources[FormatHaSid(vmId, vmType)].Update(
            state: state,
            comment: comment,
            max_restart: maxRestart,
            max_relocate: maxRelocate,
            auto_rebalance: autoRebalance,
            failback: failback
        );

    /// <summary>
    /// Delete an HA resource by sid (e.g. vm:100 or ct:100).
    /// </summary>
    public static async Task<bool> DeleteHaResourceAsync(
        this PveClient client,
        string sid,
        bool? purge = null,
        bool ignoreNotFound = true)
    {
        if (string.IsNullOrWhiteSpace(sid))
        {
            return ignoreNotFound;
        }

        if (ignoreNotFound)
        {
            var existing = await client.GetHaResourceAsync(sid);
            if (existing == null)
            {
                return true;
            }
        }

        var result = await client.Cluster.Ha.Resources[sid].Delete(purge: purge);
        return result.IsSuccessStatusCode;
    }

    /// <summary>
    /// Delete an HA resource by VM ID and type.
    /// </summary>
    public static Task<bool> DeleteHaResourceAsync(
        this PveClient client,
        long vmId,
        VmType vmType = VmType.Qemu,
        bool? purge = null,
        bool ignoreNotFound = true)
        => client.DeleteHaResourceAsync(FormatHaSid(vmId, vmType), purge, ignoreNotFound);
    #endregion

    #region Nodes
    /// <summary>
    /// Return all nodes info.
    /// </summary>
    public static async Task<IEnumerable<IClusterResourceNode>> GetNodesAsync(this PveClient client)
        => await client.GetResourcesAsync(ClusterResourceType.Node);

    /// <summary>
    /// Get node info from id.
    /// </summary>
    public static async Task<IClusterResourceNode> GetNodeAsync(this PveClient client, string node)
        => (await GetNodesAsync(client)).FirstOrDefault(a => a.Node == node) ??
                    throw new ArgumentException($"Node '{node}' not found!");

    /// <summary>
    /// Returns the x86-64 CPU compatibility level for each online node in the cluster.
    /// The minimum level across all nodes is the safest CPU type to assign to VMs
    /// for live migration compatibility.
    /// </summary>
    /// <returns>List of (Node name, CpuX86Level) tuples</returns>
    public static async Task<IEnumerable<(string Node, CpuX86Level Level)>> GetClusterCpuX86LevelsAsync(this PveClient client)
    {
        var resources = await client.Cluster.Resources.GetAsync();
        var onlineNodes = resources.Where(a => a.ResourceType == ClusterResourceType.Node && a.IsOnline);

        var result = new List<(string, CpuX86Level)>();
        foreach (var node in onlineNodes)
        {
            var status = await client.Nodes[node.Node].Status.GetAsync();
            var flags = status?.CpuInfo?.Flags ?? string.Empty;
            result.Add((node.Node, NodeHelper.GetCpuX86Level(flags)));
        }
        return result;
    }
    #endregion

    #region Storage
    /// <summary>
    /// Return all storage info.
    /// </summary>
    public static async Task<IEnumerable<IClusterResourceStorage>> GetStoragesAsync(this PveClient client)
        => await client.GetResourcesAsync(ClusterResourceType.Storage);

    /// <summary>
    /// Get storage info from id.
    /// </summary>
    public static async Task<IClusterResourceStorage> GetStorageAsync(this PveClient client, string storage)
        => (await GetStoragesAsync(client)).FirstOrDefault(a => a.Storage == storage) ??
                    throw new ArgumentException($"Storage '{storage}' not found!");

    /// <summary>
    /// Allocate a disk volume in storage for a VM or Container.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="storage">Storage identifier (e.g. "local-zfs", "local-lvm").</param>
    /// <param name="vmId">Owner VM or Container ID.</param>
    /// <param name="size">Disk size (e.g. "10G", "1024M").</param>
    /// <param name="filename">Optional volume filename. When null, Proxmox VE allocates the name automatically.</param>
    /// <param name="format">Optional disk image format (raw, qcow2, subvol, vmdk).</param>
    /// <returns>Result of the storage allocation.</returns>
    public static Task<Result> CreateDiskAsync(this PveClient client,
                                               string node,
                                               string storage,
                                               long vmId,
                                               string size,
                                               string filename = null,
                                               string format = null)
        => client.Nodes[node].Storage[storage].Content.Create(filename, size, (int)vmId, format);

    /// <summary>
    /// Allocate a disk volume in storage for a VM or Container.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="storage">Storage identifier (e.g. "local-zfs", "local-lvm").</param>
    /// <param name="vmId">Owner VM or Container ID.</param>
    /// <param name="sizeGb">Disk size in gigabytes.</param>
    /// <param name="filename">Optional volume filename. When null, Proxmox VE allocates the name automatically.</param>
    /// <param name="format">Optional disk image format (raw, qcow2, subvol, vmdk).</param>
    /// <returns>Result of the storage allocation.</returns>
    public static Task<Result> CreateDiskAsync(this PveClient client,
                                               string node,
                                               string storage,
                                               long vmId,
                                               int sizeGb,
                                               string filename = null,
                                               string format = null)
        => client.CreateDiskAsync(node, storage, vmId, $"{sizeGb}G", filename, format);

    /// <summary>
    /// Delete a disk volume from storage.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="storage">Storage identifier (e.g. "local-zfs").</param>
    /// <param name="volume">Volume identifier or filename (e.g. "vm-100-disk-0" or "local-zfs:vm-100-disk-0").</param>
    /// <param name="delay">Optional delay in seconds to wait for background task.</param>
    /// <returns>Result of the volume deletion.</returns>
    public static Task<Result> DeleteDiskVolumeAsync(this PveClient client,
                                                     string node,
                                                     string storage,
                                                     string volume,
                                                     int? delay = null)
        => client.Nodes[node].Storage[storage].Content[volume].Delete(delay);
    #endregion

    #region Vm
    /// <summary>
    /// Get all VM/CT from cluster.
    /// </summary>
    public static async Task<IEnumerable<IClusterResourceVm>> GetVmsAsync(this PveClient client)
        => (await client.GetResourcesAsync(ClusterResourceType.Vm))
                        .OrderBy(a => a.Node)
                        .ThenBy(a => a.VmId);

    /// <summary>
    /// Get VM/CT from id or name.
    /// </summary>
    public static Task<IClusterResourceVm> GetVmAsync(this PveClient client, long vmId)
        => client.GetVmAsync(vmId + string.Empty);

    /// <summary>
    /// Get VM/CT from id or name.
    /// </summary>
    public static async Task<IClusterResourceVm> GetVmAsync(this PveClient client, string vmIdOrName)
        => (await GetVmsAsync(client)).FirstOrDefault(a => VmHelper.CheckIdOrName(a, vmIdOrName)) ??
                throw new ArgumentException($"VM/CT '{vmIdOrName}' not found!");

    /// <summary>
    /// Get vms from jolly.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="jolly">all for all vm,
    /// <para>@all-nodeName all vm in host,</para>
    /// <para>@node-nodeName all vm in host,</para>
    /// <para>@pool-name all vm in pool,</para>
    /// <para>@tag-name all vm contain tags,</para>
    /// <para>vmid id vm</para>
    /// <para>Range 100:104 return vm with id in range</para>
    /// <para>start with '-' exclude vm</para>
    /// <para>comma separated</para>
    /// </param>
    public static async Task<IEnumerable<IClusterResourceVm>> GetVmsAsync(this PveClient client, string jolly)
    {
        IEnumerable<Shared.Models.Pool.PoolItem> pools = null;

        async Task<IEnumerable<string>> GetPoolMemberIdsAsync(string name)
        {
            pools ??= await client.Pools.GetAsync();

            //nested pool ids are separated by '/', a child starts with the parent id followed by '/'
            var poolNames = pools.Select(a => a.Id)
                                 .Where(a => string.Equals(a, name, StringComparison.OrdinalIgnoreCase)
                                                || a.StartsWith(name + "/", StringComparison.OrdinalIgnoreCase))
                                 .ToList();

            var memberIds = new HashSet<string>();
            foreach (var poolName in poolNames)
            {
                //use 'GET /pools?poolid=' because 'GET /pools/{poolid}' does not support nested pools
                var pool = (await client.Pools.GetAsync(poolName)).FirstOrDefault();
                if (pool == null) { continue; }
                memberIds.UnionWith(pool.Members.Select(a => a.Id));
            }

            return memberIds;
        }

        return await VmHelper.GetVmsFromJollyAsync(await GetVmsAsync(client), jolly, GetPoolMemberIdsAsync);
    }

    /// <summary>
    /// Change status VM/CT
    /// </summary>
    public static Task<Result> ChangeStatusVmAsync(this PveClient client, long vmId, string status)
        => client.ChangeStatusVmAsync(vmId, Enum.Parse<VmStatus>(status, true));

    /// <summary>
    /// Change status VM/CT
    /// </summary>
    public static async Task<Result> ChangeStatusVmAsync(this PveClient client, long vmId, VmStatus status)
    {
        var vm = await client.GetVmAsync(vmId);
        return await VmHelper.ChangeStatusVmAsync(client, vm.Node, vm.VmType, vm.VmId, status);
    }

    /// <summary>
    /// Get Vm Status
    /// </summary>
    public static async Task<VmBaseStatusCurrent> GetVmStatusAsync(this PveClient client,
                                                                   string node,
                                                                   VmType vmType,
                                                                   long vmId)
        => vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Status.Current.GetAsync(),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Status.Current.GetAsync(),
            _ => throw new InvalidEnumArgumentException(),
        };

    /// <summary>
    /// Get Vm Config
    /// </summary>
    public static async Task<VmConfig> GetVmConfigAsync(this PveClient client,
                                                        string node,
                                                        VmType vmType,
                                                        long vmId)
        => vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Config.GetAsync(),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Config.GetAsync(),
            _ => throw new InvalidEnumArgumentException(),
        };

    /// <summary>
    /// Unlock vm
    /// </summary>
    public static async Task VmUnlockAsync(this PveClient client, string node, VmType vmType, long vmId)
    {
        switch (vmType)
        {
            case VmType.Qemu: await client.Nodes[node].Qemu[vmId].Config.UpdateVm(delete: "lock", skiplock: true); break;
            case VmType.Lxc: await client.Nodes[node].Lxc[vmId].Config.UpdateVm(delete: "lock"); break;
            default: throw new InvalidEnumArgumentException();
        }
    }

    /// <summary>
    /// Get Vm RrdData
    /// </summary>
    public static async Task<IEnumerable<VmRrdData>> GetVmRrdDataAsync(this PveClient client,
                                                                       string node,
                                                                       VmType vmType,
                                                                       long vmId,
                                                                       RrdDataTimeFrame rrdDataTimeFrame,
                                                                       RrdDataConsolidation rrdDataConsolidation)
        => vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Rrddata.GetAsync(rrdDataTimeFrame, rrdDataConsolidation),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Rrddata.GetAsync(rrdDataTimeFrame, rrdDataConsolidation),
            _ => throw new InvalidEnumArgumentException(),
        };

    /// <summary>
    /// Find the first free SCSI bus slot (scsi0..scsi30) on a QEMU VM, checking both current and pending configurations.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmId">VM ID.</param>
    /// <param name="maxSlots">Maximum number of SCSI slots to check (default: 31, representing scsi0..scsi30).</param>
    /// <returns>Free SCSI slot name (e.g. "scsi0", "scsi1").</returns>
    public static async Task<string> FindFirstFreeScsiSlotAsync(this PveClient client,
                                                                string node,
                                                                long vmId,
                                                                int maxSlots = 31)
    {
        var configResult = await client.Nodes[node].Qemu[vmId].Config.VmConfig();
        if (!configResult.IsSuccessStatusCode)
        {
            throw new PveResultException(configResult, !string.IsNullOrWhiteSpace(configResult.ReasonPhrase) ? configResult.ReasonPhrase : configResult.GetError());
        }

        var takenSlots = new HashSet<int>();
        if (configResult.ResponseHasData && configResult.ResponseToDictionary["data"] is IDictionary<string, object> dict)
        {
            foreach (var key in dict.Keys)
            {
                if (key.StartsWith("scsi", StringComparison.OrdinalIgnoreCase) && int.TryParse(key[4..], out var slot))
                {
                    takenSlots.Add(slot);
                }
            }
        }

        try
        {
            var pendingResult = await client.Nodes[node].Qemu[vmId].Pending.VmPending();
            if (pendingResult.IsSuccessStatusCode)
            {
                foreach (var item in pendingResult.ToEnumerable())
                {
                    if (item is IDictionary<string, object> pendingDict &&
                        pendingDict.TryGetValue("key", out var keyObj) &&
                        keyObj is string key &&
                        key.StartsWith("scsi", StringComparison.OrdinalIgnoreCase) &&
                        int.TryParse(key[4..], out var pendingSlot))
                    {
                        takenSlots.Add(pendingSlot);
                    }
                }
            }
        }
        catch
        {
            // Pending endpoint is best-effort for compatibility with older PVE nodes or mock handlers
        }

        for (var i = 0; i < maxSlots; i++)
        {
            if (!takenSlots.Contains(i))
            {
                return $"scsi{i}";
            }
        }

        throw new InvalidOperationException($"No free SCSI slots available on VM {vmId} (scsi0..scsi{maxSlots - 1} are occupied).");
    }

    /// <summary>
    /// Find the first free mount point slot (mp0..mp31) on an LXC container, checking both current and pending configurations.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="containerId">Container ID.</param>
    /// <param name="maxMountPoints">Maximum number of mount points to check (default: 32, representing mp0..mp31).</param>
    /// <returns>Free mount point slot name (e.g. "mp0", "mp1").</returns>
    public static async Task<string> FindFirstFreeMountPointAsync(this PveClient client,
                                                                  string node,
                                                                  long containerId,
                                                                  int maxMountPoints = 32)
    {
        var configResult = await client.Nodes[node].Lxc[containerId].Config.VmConfig();
        if (!configResult.IsSuccessStatusCode)
        {
            throw new PveResultException(configResult, !string.IsNullOrWhiteSpace(configResult.ReasonPhrase) ? configResult.ReasonPhrase : configResult.GetError());
        }

        var takenSlots = new HashSet<int>();
        if (configResult.ResponseHasData && configResult.ResponseToDictionary["data"] is IDictionary<string, object> dict)
        {
            foreach (var key in dict.Keys)
            {
                if (key.StartsWith("mp", StringComparison.OrdinalIgnoreCase) && int.TryParse(key[2..], out var slot))
                {
                    takenSlots.Add(slot);
                }
            }
        }

        try
        {
            var pendingResult = await client.Nodes[node].Lxc[containerId].Pending.VmPending();
            if (pendingResult.IsSuccessStatusCode)
            {
                foreach (var item in pendingResult.ToEnumerable())
                {
                    if (item is IDictionary<string, object> pendingDict &&
                        pendingDict.TryGetValue("key", out var keyObj) &&
                        keyObj is string key &&
                        key.StartsWith("mp", StringComparison.OrdinalIgnoreCase) &&
                        int.TryParse(key[2..], out var pendingSlot))
                    {
                        takenSlots.Add(pendingSlot);
                    }
                }
            }
        }
        catch
        {
            // Pending endpoint is best-effort for compatibility with older PVE nodes or mock handlers
        }

        for (var i = 0; i < maxMountPoints; i++)
        {
            if (!takenSlots.Contains(i))
            {
                return $"mp{i}";
            }
        }

        throw new InvalidOperationException($"No free mount points available on Container {containerId} (mp0..mp{maxMountPoints - 1} are occupied).");
    }

    /// <summary>
    /// Attach a disk volume to a QEMU VM.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmId">VM ID.</param>
    /// <param name="volumeId">Storage volume identifier (e.g. "local-zfs:vm-100-disk-0").</param>
    /// <param name="busSlot">Target bus slot (e.g. "scsi0", "sata1", "virtio0"). When null, auto-assigns the first free SCSI slot.</param>
    /// <param name="options">Optional disk options (e.g. "discard=on,ssd=1", "backup=0").</param>
    /// <param name="digest">Optional configuration digest to prevent race conditions.</param>
    /// <returns>Result of the update operation.</returns>
    public static async Task<Result> AttachDiskToVmAsync(this PveClient client,
                                                         string node,
                                                         long vmId,
                                                         string volumeId,
                                                         string busSlot = null,
                                                         string options = null,
                                                         string digest = null)
    {
        busSlot ??= await client.FindFirstFreeScsiSlotAsync(node, vmId);

        var diskValue = string.IsNullOrWhiteSpace(options) ? volumeId : $"{volumeId},{options}";

        IDictionary<int, string> scsiN = null;
        IDictionary<int, string> sataN = null;
        IDictionary<int, string> virtioN = null;
        IDictionary<int, string> ideN = null;

        var lowerSlot = busSlot.ToLowerInvariant();
        if (lowerSlot.StartsWith("scsi") && int.TryParse(lowerSlot[4..], out var scsiIdx))
        {
            scsiN = new Dictionary<int, string> { { scsiIdx, diskValue } };
        }
        else if (lowerSlot.StartsWith("sata") && int.TryParse(lowerSlot[4..], out var sataIdx))
        {
            sataN = new Dictionary<int, string> { { sataIdx, diskValue } };
        }
        else if (lowerSlot.StartsWith("virtio") && int.TryParse(lowerSlot[6..], out var virtioIdx))
        {
            virtioN = new Dictionary<int, string> { { virtioIdx, diskValue } };
        }
        else if (lowerSlot.StartsWith("ide") && int.TryParse(lowerSlot[3..], out var ideIdx))
        {
            ideN = new Dictionary<int, string> { { ideIdx, diskValue } };
        }
        else
        {
            throw new ArgumentException($"Unsupported or invalid bus slot '{busSlot}'. Expected scsiN, sataN, virtioN, or ideN.", nameof(busSlot));
        }

        return await client.Nodes[node].Qemu[vmId].Config.UpdateVmAsync(scsiN: scsiN,
                                                                       sataN: sataN,
                                                                       virtioN: virtioN,
                                                                       ideN: ideN,
                                                                       digest: digest);
    }

    /// <summary>
    /// Detach a disk from a QEMU VM.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmId">VM ID.</param>
    /// <param name="busSlot">Bus slot name (e.g. "scsi0", "sata1", "virtio0").</param>
    /// <param name="digest">Optional configuration digest to prevent race conditions.</param>
    /// <returns>Result of the update operation.</returns>
    public static Task<Result> DetachDiskFromVmAsync(this PveClient client,
                                                     string node,
                                                     long vmId,
                                                     string busSlot,
                                                     string digest = null)
        => client.Nodes[node].Qemu[vmId].Config.UpdateVmAsync(delete: busSlot, digest: digest);

    /// <summary>
    /// Attach a disk volume as a mount point to an LXC container.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="containerId">Container ID.</param>
    /// <param name="volumeId">Storage volume identifier (e.g. "local-zfs:subvol-100-disk-0").</param>
    /// <param name="mountPath">Destination directory path inside container (e.g. "/mnt/data").</param>
    /// <param name="mpSlot">Mount point slot name (e.g. "mp0"). If null, auto-assigns the first free mount point.</param>
    /// <param name="options">Optional extra mount point options (e.g. "backup=1,ro=1").</param>
    /// <param name="digest">Optional configuration digest to prevent race conditions.</param>
    /// <returns>Result of the update operation.</returns>
    public static async Task<Result> AttachDiskToLxcAsync(this PveClient client,
                                                          string node,
                                                          long containerId,
                                                          string volumeId,
                                                          string mountPath,
                                                          string mpSlot = null,
                                                          string options = null,
                                                          string digest = null)
    {
        mpSlot ??= await client.FindFirstFreeMountPointAsync(node, containerId);

        var lowerSlot = mpSlot.ToLowerInvariant();
        if (!lowerSlot.StartsWith("mp") || !int.TryParse(lowerSlot[2..], out var mpIdx))
        {
            throw new ArgumentException($"Invalid mount point slot '{mpSlot}'. Expected mpN (e.g. mp0..mp31).", nameof(mpSlot));
        }

        var mpValue = string.IsNullOrWhiteSpace(options)
            ? $"{volumeId},mp={mountPath}"
            : $"{volumeId},mp={mountPath},{options}";

        var mpN = new Dictionary<int, string> { { mpIdx, mpValue } };
        return await client.Nodes[node].Lxc[containerId].Config.UpdateVm(mpN: mpN, digest: digest);
    }

    /// <summary>
    /// Detach a mount point disk from an LXC container.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="containerId">Container ID.</param>
    /// <param name="mpSlot">Mount point slot name (e.g. "mp0", "mp1").</param>
    /// <param name="digest">Optional configuration digest to prevent race conditions.</param>
    /// <returns>Result of the update operation.</returns>
    public static Task<Result> DetachDiskFromLxcAsync(this PveClient client,
                                                      string node,
                                                      long containerId,
                                                      string mpSlot,
                                                      string digest = null)
        => client.Nodes[node].Lxc[containerId].Config.UpdateVm(delete: mpSlot, digest: digest);

    /// <summary>
    /// Purge an unused disk entry from a VM or Container by exact volume ID match.
    /// <para>
    /// CAUTION: On Proxmox VE QEMU VMs, deleting an "unused[n]" configuration entry triggers
    /// physical removal and permanent deletion of the underlying disk volume from storage.
    /// </para>
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="guestId">VM or Container ID.</param>
    /// <param name="vmType">QEMU or LXC.</param>
    /// <param name="exactVolumeId">Exact volume ID or filename to purge (e.g. "local-zfs:vm-100-disk-1" or "vm-100-disk-1").</param>
    /// <param name="digest">Optional configuration digest to prevent race conditions.</param>
    /// <returns>Result of the purge operation.</returns>
    public static async Task<Result> PurgeUnusedDiskAsync(this PveClient client,
                                                          string node,
                                                          long guestId,
                                                          VmType vmType,
                                                          string exactVolumeId,
                                                          string digest = null)
    {
        var configResult = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[guestId].Config.VmConfig(),
            VmType.Lxc => await client.Nodes[node].Lxc[guestId].Config.VmConfig(),
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType))
        };

        if (!configResult.IsSuccessStatusCode)
        {
            throw new PveResultException(configResult, !string.IsNullOrWhiteSpace(configResult.ReasonPhrase) ? configResult.ReasonPhrase : configResult.GetError());
        }

        string matchedSlot = null;
        if (configResult.ResponseHasData && configResult.ResponseToDictionary["data"] is IDictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                if (kvp.Key.StartsWith("unused", StringComparison.OrdinalIgnoreCase) &&
                    MatchExactVolume(kvp.Value + string.Empty, exactVolumeId))
                {
                    matchedSlot = kvp.Key;
                    break;
                }
            }
        }

        if (matchedSlot == null)
        {
            throw new ArgumentException($"Unused disk '{exactVolumeId}' not found on {vmType} {guestId}.");
        }

        return vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[guestId].Config.UpdateVmAsync(delete: matchedSlot, digest: digest),
            VmType.Lxc => await client.Nodes[node].Lxc[guestId].Config.UpdateVm(delete: matchedSlot, digest: digest),
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType))
        };
    }

    private static bool MatchExactVolume(string configValue, string exactVolumeId)
    {
        if (string.IsNullOrWhiteSpace(configValue) || string.IsNullOrWhiteSpace(exactVolumeId))
        {
            return false;
        }

        var volId = configValue.Split(',')[0].Trim();
        if (string.Equals(volId, exactVolumeId, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var colonIndex = volId.IndexOf(':');
        if (colonIndex >= 0)
        {
            var nameWithoutStorage = volId[(colonIndex + 1)..];
            if (string.Equals(nameWithoutStorage, exactVolumeId, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    /// <summary>
    /// Get Vm Ids
    /// </summary>
    public static async Task<IEnumerable<string>> GetVmIdsAsync(this PveClient client,
                                                                bool addAll,
                                                                bool addNodes,
                                                                bool addPools,
                                                                bool addTags,
                                                                bool addVmId,
                                                                bool addVmName)
    {
        var vmIds = new List<string>();
        var resources = await client.GetResourcesAsync(ClusterResourceType.All);

        if (addAll) { vmIds.Add("@all"); }

        if (addNodes)
        {
            //old
            vmIds.AddRange(resources.Where(a => a.ResourceType == ClusterResourceType.Node && a.IsOnline)
                                    .OrderBy(a => a.Node)
                                    .Select(a => $"@all-{a.Node}"));

            vmIds.AddRange(resources.Where(a => a.ResourceType == ClusterResourceType.Node && a.IsOnline)
                                    .OrderBy(a => a.Node)
                                    .Select(a => $"@node-{a.Node}"));
        }

        if (addPools)
        {
            vmIds.AddRange(resources.Where(a => a.ResourceType == ClusterResourceType.Pool)
                                    .OrderBy(a => a.Pool)
                                    .Select(a => $"@pool-{a.Pool}"));
        }

        if (addTags)
        {
            var tags = (await client.Cluster.Options.GetAsync()).AllowedTags ?? [];
            vmIds.AddRange(tags.Select(a => $"@tag-{a}"));
        }

        var vms = resources.Where(a => a.ResourceType == ClusterResourceType.Vm && !a.IsUnknown);
        if (addVmId) { vmIds.AddRange(vms.Select(a => a.VmId + string.Empty).Order()); }
        if (addVmName) { vmIds.AddRange(vms.Select(a => a.Name).Order()); }

        return vmIds.Distinct();
    }

    /// <summary>
    /// Upload templates and ISO images.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="node">Node.</param>
    /// <param name="storage">Node.</param>
    /// <param name="content">Content type.
    ///   Enum: iso,vztmpl</param>
    /// <param name="fileStream">Stream of file</param>
    /// <param name="fileName">The name of the file to create. Caution: This will be normalized!</param>
    /// <param name="cancellationToken"></param>
    /// <param name="secondsTimeout">Timeout in secods</param>
    /// <param name="checksum">The expected checksum of the file.</param>
    /// <param name="checksumAlgorithm">The algorithm to calculate the checksum of the file.
    ///   Enum: md5,sha1,sha224,sha256,sha384,sha512</param>
    /// <param name="tmpFileName">The source file name. This parameter is usually set by the REST handler.
    /// You can only overwrite it when connecting to the trusted port on localhost.</param>
    public static async Task<Result> UploadFileToStorageAsync(this PveClient client,
                                                          string node,
                                                          string storage,
                                                          string content,
                                                          Stream fileStream,
                                                          string fileName,
                                                          CancellationToken cancellationToken,
                                                          int secondsTimeout = 600,
                                                          string checksum = null,
                                                          string checksumAlgorithm = null,
                                                          string tmpFileName = null)
    {
        if (!new[] { "iso", "vztmpl" }.Contains(content)) { throw new PveException("Content type non valid! 'iso' or 'vztmpl'"); }

        var boundary = Guid.NewGuid().ToString();
        var mpfdContent = new MultipartFormDataContent(boundary)
        {
            { new StringContent(content), "\"content\"" },
            { new StreamContent(fileStream), "\"filename\"", $"\"{fileName}\"" }
        };

        var parameters = new Dictionary<string, object>
        {
             { "content", content },
             { "checksum", checksum },
             { "checksum-algorithm", checksumAlgorithm },
             { "tmpfilename", tmpFileName }
        };

        // foreach (var item in parameters.Where(a => a.Value != null))
        // {
        //     mpfdContent.Add(new StringContent(item.Value.ToString()), $"\"{item.Key}\"");
        // }

        mpfdContent.Headers.ContentType = MediaTypeHeaderValue.Parse($"multipart/form-data; boundary={boundary}");

        //the timeout of the HttpClient cannot change after the first request: the time of the upload is counted here
        using var cts = client.CreateTimeoutTokenSource(TimeSpan.FromSeconds(secondsTimeout), cancellationToken);

        var resource = $"/nodes/{node}/storage/{storage}/upload";
        var request = client.CreateHttpRequestMessage(HttpMethod.Post, client.GetApiUrl() + resource);
        request.Content = mpfdContent;

        var response = await client.GetHttpClient().SendAsync(request, cts.Token);
        var result = new Result(JsonConvert.DeserializeObject<ExpandoObject>(await response.Content.ReadAsStringAsync()),
                                response.StatusCode,
                                response.ReasonPhrase,
                                response.IsSuccessStatusCode,
                                resource,
                                parameters,
                                MethodType.Create,
                                ResponseType.Json,
                                TimeSpan.Zero);

        return result;
    }

    /// <summary>
    /// Get default web console type
    /// </summary>
    public static async Task<WebConsoleType> GetDefaultWebConsoleAsync(this PveClient client)
        => (await client.Cluster.Options.GetAsync()).Console switch
        {
            "vv" => WebConsoleType.Spice,
            "html5" => WebConsoleType.NoVnc,
            "xtermjs" => WebConsoleType.XtermJs,
            _ => WebConsoleType.XtermJs
        };

    /// <summary>
    /// Get disk smart info
    /// </summary>
    public static async Task<NodeDiskSmart> GetDiskSmart(this PveClient client, string node, string devPath)
    {
        try
        {
            return await client.Nodes[node].Disks.Smart.GetAsync(devPath);
        }
        catch (Exception ex)
        {
            return new()
            {
                Attributes = [new() { Id = "0", Name = "Error", Raw = ex.Message, Value = -1 }]
            };
        }
    }

    /// <summary>
    /// Get cluster info
    /// </summary>
    public static async Task<(ClusterType Type, string Name)> GetClusterInfoAsync(this PveClient client)
    {
        var status = await client.Cluster.Status.GetAsync();
        var clusterName = status.FirstOrDefault(a => a.Type == PveConstants.KeyApiCluster)?.Name;

        var type = string.IsNullOrEmpty(clusterName)
                        ? ClusterType.SingleNode
                        : ClusterType.Cluster;

        var name = string.IsNullOrEmpty(clusterName)
                        ? status.FirstOrDefault()!.Name
                        : clusterName;

        return (type, name);
    }
}
