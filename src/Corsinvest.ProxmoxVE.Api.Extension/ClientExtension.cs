/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Dynamic;
using System.Net.Http.Headers;

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
    public static async Task<IEnumerable<ClusterResource>> GetResourcesAsync(this PveClient client, ClusterResourceType resourceType)
        => await client.Cluster.Resources.GetAsync(resourceType);

    /// <summary>
    /// Get resource type
    /// </summary>
    public static async Task<IEnumerable<ClusterResource>> GetResourcesAsync(this PveClient client, string type)
        => await client.Cluster.Resources.GetAsync(type);

    /// <summary>
    /// Get host and ip
    /// </summary>
    /// <returns>Dictionary host, ip</returns>
    public static async Task<IReadOnlyDictionary<string, string>> GetHostAndIpAsync(this PveClient client)
        => (await client.Cluster.Status.GetAsync())
                .Where(a => a.Type == PveConstants.KeyApiNode)
                .ToDictionary(a => a.Name, a => a.IpAddress);
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
    public static async Task<IClusterResourceVm> GetVmAsync(this PveClient client, long vmId)
        => await client.GetVmAsync(vmId + string.Empty);

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
        var vms = await GetVmsAsync(client);
        IEnumerable<Shared.Models.Pool.PoolItem> pools = null;

        async Task<IEnumerable<IClusterResourceVm>> GetVmsFromIdAsync(string id)
        {
            var data = Enumerable.Empty<IClusterResourceVm>();

            if (id == "all" || id == "@all")
            {
                //all nodes
                data = vms;
            }
            else if (id.StartsWith("all-") || id.StartsWith("@all-"))
            {
                //all in specific node
                var idx = id.StartsWith("all-") ? 4 : 5;
                var nodeName = id[idx..];
                data = vms.Where(a => a.Node == nodeName || string.Equals(a.Node, nodeName, StringComparison.OrdinalIgnoreCase));
            }
            else if (id.StartsWith("@node-"))
            {
                //all in specific node
                var nodeName = id[6..];
                data = vms.Where(a => a.Node == nodeName || string.Equals(a.Node, nodeName, StringComparison.OrdinalIgnoreCase));
            }
            else if (id.StartsWith("@pool-"))
            {
                //all in specific pool
                var name = id[6..];
                pools ??= await client.Pools.GetAsync();
                var poolName = pools.Select(a => a.Id)
                                    .FirstOrDefault(a => a == name || string.Equals(a, name, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(poolName))
                {
                    data = (await client.Pools[poolName].GetAsync()).Members.Where(a => vms.Any(b => b.Id == a.Id));
                }
            }
            else if (id.StartsWith("@tag-"))
            {
                //all in specific tag
                var tagName = id[5..];
                data = vms.Where(a => (a.Tags + string.Empty).ToLower().Split(';').Contains(tagName.ToLower())
                                        || (a.Tags + string.Empty).Split(';').Contains(tagName));
            }
            else
            {
                data = vms.Where(a => VmHelper.CheckIdOrName(a, id));
            }

            return data;
        }

        var ret = new List<IClusterResourceVm>();

        //add
        foreach (var id in jolly.Split(',')) { ret.AddRange(await GetVmsFromIdAsync(id)); }

        ret = [.. ret.Distinct()];

        //exclude data
        foreach (var id in jolly.Split(',').Where(a => a.StartsWith("-")).Select(a => a[1..]))
        {
            foreach (var item in await GetVmsFromIdAsync(id))
            {
                ret.Remove(item);
            }
        }

        return [.. ret];
    }

    /// <summary>
    /// Change status VM/CT
    /// </summary>
    public static async Task<Result> ChangeStatusVmAsync(this PveClient client, long vmId, string status)
        => await client.ChangeStatusVmAsync(vmId, (VmStatus)Enum.Parse(typeof(VmStatus), status, true));

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
        if (addVmId) { vmIds.AddRange(vms.Select(a => a.VmId + string.Empty).OrderBy(a => a)); }
        if (addVmName) { vmIds.AddRange(vms.Select(a => a.Name).OrderBy(a => a)); }

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

        var httpClient = client.GetHttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(secondsTimeout);

        var resource = $"/nodes/{node}/storage/{storage}/upload";
        var request = client.CreateHttpRequestMessage(HttpMethod.Post, client.GetApiUrl() + resource);
        request.Content = mpfdContent;

        var response = await httpClient.SendAsync(request, cancellationToken);
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
