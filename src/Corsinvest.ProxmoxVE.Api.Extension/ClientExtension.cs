/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Extension
{
    /// <summary>
    /// Client extension
    /// </summary>
    public static class ClientExtension
    {
        #region Cluster
        /// <summary>
        /// Get resources
        /// </summary>
        /// <param name="client"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterResource>> GetResources(this PveClient client, ClusterResourceType resourceType)
            => await client.Cluster.Resources.Get(resourceType);

        /// <summary>
        /// Get resource type
        /// </summary>
        /// <param name="client"></param>
        /// <param name="type">
        ///   Enum: vm,storage,node,sdn</param>
        /// <returns></returns>
        public static async Task<IEnumerable<ClusterResource>> GetResources(this PveClient client, string type)
            => await client.Cluster.Resources.Get(type);

        /// <summary>
        /// Get host and ip
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Dictionary host, ip</returns>
        public static async Task<IReadOnlyDictionary<string, string>> GetHostAndIp(this PveClient client)
            => (await client.Cluster.Status.Get())
                    .Where(a => a.Type == PveConstants.KeyApiNode)
                    .ToDictionary(a => a.Name, a => a.IpAddress);
        #endregion

        #region Nodes
        /// <summary>
        /// Return all nodes info.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<IClusterResourceNode>> GetNodes(this PveClient client)
            => await client.GetResources(ClusterResourceType.Node);


        /// <summary>
        /// Get node info from id.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<IClusterResourceNode> GetNode(this PveClient client, string node)
            => (await GetNodes(client)).FirstOrDefault(a => a.Node == node) ??
                        throw new ArgumentException($"Node '{node}' not found!");
        #endregion

        #region Storage
        /// <summary>
        /// Return all storage info.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<IClusterResourceStorage>> GetStorages(this PveClient client)
            => await client.GetResources(ClusterResourceType.Storage);

        /// <summary>
        /// Get storage info from id.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<IClusterResourceStorage> GetStorage(this PveClient client, string storage)
            => (await GetStorages(client)).FirstOrDefault(a => a.Storage == storage) ??
                        throw new ArgumentException($"Storage '{storage}' not found!");
        #endregion

        #region Vm
        /// <summary>
        /// Get all VM/CT from cluster.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<IClusterResourceVm>> GetVms(this PveClient client)
            => (await client.GetResources(ClusterResourceType.Vm))
                            .OrderBy(a => a.Node)
                            .ThenBy(a => a.VmId);

        /// <summary>
        /// Get VM/CT from id or name.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        public static async Task<IClusterResourceVm> GetVm(this PveClient client, long vmId)
            => await client.GetVm(vmId + "");

        /// <summary>
        /// Get VM/CT from id or name.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vmIdOrName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<IClusterResourceVm> GetVm(this PveClient client, string vmIdOrName)
            => (await GetVms(client)).FirstOrDefault(a => VmHelper.CheckIdOrName(a, vmIdOrName)) ??
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
        /// <returns></returns>
        public static async Task<IEnumerable<IClusterResourceVm>> GetVms(this PveClient client, string jolly)
        {
            var allVms = await GetVms(client);

            async Task<IEnumerable<IClusterResourceVm>> GetVmsFromId(string id)
            {
                var data = Enumerable.Empty<IClusterResourceVm>();

                if (id == "all" || id == "@all")
                {
                    //all nodes
                    data = allVms;
                }
                else if (id.StartsWith("all-") || id.StartsWith("@all-"))
                {
                    //all in specific node
                    var idx = id.StartsWith("all-") ? 4 : 5;
                    data = allVms.Where(a => a.Node.ToLower() == id.ToLower().Substring(idx));
                }
                else if (id.StartsWith("@node-"))
                {
                    //all in specific node
                    var nodeName = id.ToLower().Substring(6);
                    data = allVms.Where(a => a.Node.ToLower() == nodeName.ToLower());
                }
                else if (id.StartsWith("@pool-"))
                {
                    //all in specific pool
                    var poolName = id.ToLower().Substring(6);

                    if ((await client.Pools.Get()).Any(a => a.Id == poolName))
                    {
                        data = (await client.Pools[poolName].Get()).Members.Where(a => allVms.Any(b => b.Id == a.Id));
                    }
                }
                else if (id.StartsWith("@tag-"))
                {
                    //all in specific tag
                    var tagName = id.ToLower().Substring(5);
                    data = allVms.Where(a => (a.Tags + "").ToLower().Split(';').Contains(tagName));
                }
                else
                {
                    data = allVms.Where(a => VmHelper.CheckIdOrName(a, id));
                }

                return data;
            }

            var ret = new List<IClusterResourceVm>();

            //add
            foreach (var id in jolly.Split(',')) { ret.AddRange(await GetVmsFromId(id)); }

            ret = ret.Distinct().ToList();

            //exclude data
            foreach (var id in jolly.Split(',').Where(a => a.StartsWith("-")).Select(a => a.Substring(1)))
            {
                foreach (var item in await GetVmsFromId(id))
                {
                    ret.Remove(item);
                }
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Change status VM/CT
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vmId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static async Task<Result> ChangeStatusVm(this PveClient client, long vmId, string status)
            => await client.ChangeStatusVm(vmId, (VmStatus)Enum.Parse(typeof(VmStatus), status, true));

        /// <summary>
        /// Change status VM/CT
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vmId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static async Task<Result> ChangeStatusVm(this PveClient client, long vmId, VmStatus status)
        {
            var vm = await client.GetVm(vmId);
            return await VmHelper.ChangeStatusVm(client, vm.Node, vm.VmType, vm.VmId, status);
        }

        /// <summary>
        /// Get Vm Status
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static async Task<VmBaseStatusCurrent> GetVmStatus(this PveClient client, IClusterResourceVm vm)
            => vm.VmType switch
            {
                VmType.Qemu => await client.Nodes[vm.Node].Qemu[vm.VmId].Status.Current.Get(),
                VmType.Lxc => await client.Nodes[vm.Node].Lxc[vm.VmId].Status.Current.Get(),
                _ => throw new InvalidEnumArgumentException(),
            };

        /// <summary>
        /// Get Vm RrdData
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vm"></param>
        /// <param name="rrdDataTimeFrame"></param>
        /// <param name="rrdDataConsolidation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static async Task<IEnumerable<VmRrdData>> GetVmRrdData(this PveClient client,
                                                                      IClusterResourceVm vm,
                                                                      RrdDataTimeFrame rrdDataTimeFrame,
                                                                      RrdDataConsolidation rrdDataConsolidation)
            => vm.VmType switch
            {
                VmType.Qemu => await client.Nodes[vm.Node].Qemu[vm.VmId].Rrddata.Get(rrdDataTimeFrame, rrdDataConsolidation),
                VmType.Lxc => await client.Nodes[vm.Node].Lxc[vm.VmId].Rrddata.Get(rrdDataTimeFrame, rrdDataConsolidation),
                _ => throw new InvalidEnumArgumentException(),
            };
        #endregion
    }

    // /// <summary>
    // /// Upload templates and ISO images to storage.
    // /// </summary>
    // /// <param name="client"></param>
    // /// <param name="node">Node.</param>
    // /// <param name="storage">Node.</param>
    // /// <param name="content">Content type.
    // ///   Enum: iso,vztmpl</param>
    // /// <param name="fileName">The name of the file to create. Caution: This will be normalized!</param>
    // /// <param name="fileNameToUpload"></param>
    // /// <param name="checksum">The expected checksum of the file.</param>
    // /// <param name="checksumAlgorithm">The algorithm to calculate the checksum of the file.
    // ///   Enum: md5,sha1,sha224,sha256,sha384,sha512</param>
    // /// <param name="tmpFileName">The source file name. This parameter is usually set by the REST handler.
    // /// You can only overwrite it when connecting to the trusted port on localhost.</param>
    // /// <returns></returns>
    // public static async Task<Result> UploadFileToStorage(this PveClient client,
    //                                                      string node,
    //                                                      string storage,
    //                                                      string content,
    //                                                      string fileName,
    //                                                      string fileNameToUpload,
    //                                                      string checksum = null,
    //                                                      string checksumAlgorithm = null,
    //                                                      string tmpFileName = null)
    // {

    //     //        iso: ".img, .iso",
    //     //  	    vztmpl: ".tar.gz, .tar.xz",

    //     if (!new[] { "iso", "vztmpl" }.Contains(content)) { throw new PveException("Content type non valid! 'iso' or 'vztmpl'"); }

    //     // using var stream = File.OpenRead(fileNameToUpload);
    //     // using var streamContent = new StreamContent(stream);
    //     // //    streamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

    //     // using var multipartFormContent = new MultipartFormDataContent
    //     // {
    //     //     { streamContent, "filename"}, //fileName
    //     //     { new StringContent(content), "content" },
    //     // };

    //     // if (!string.IsNullOrEmpty(checksum)) { multipartFormContent.Add(new StringContent(checksum), "checksum"); }
    //     // if (!string.IsNullOrEmpty(checksumAlgorithm)) { multipartFormContent.Add(new StringContent(checksumAlgorithm), "checksum_algorithm"); }
    //     // if (!string.IsNullOrEmpty(tmpFileName)) { multipartFormContent.Add(new StringContent(tmpFileName), "tmpfilename"); }

    //     // var parameters = new Dictionary<string, object>
    //     //  {
    //     //      { "content", content },
    //     //      { "filename", stream },
    //     //      { "checksum", checksum },
    //     //      { "checksum-algorithm", checksum_algorithm },
    //     //      { "tmpfilename", tmpFileName }
    //     //  };


    //     //      var aa = await httpClient.PostAsync(new Uri($"{client.GetApiUrl()}/nodes/{node}/storage/{storage}/upload"), multipartFormContent);
    //     //    return await client.Create($"/nodes/{node}/storage/{storage}/upload", null, multipartFormContent);
    //     return null;
    // }
}
