/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

using System;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;
using Corsinvest.ProxmoxVE.Api.Extension.VM;

namespace Corsinvest.ProxmoxVE.Api.Extension.Node
{
    /// <summary>
    /// Node info
    /// </summary>
    public class NodeInfo : BaseInfo
    {
        internal NodeInfo(PveClient client, object apiData) : base(client, apiData) { }

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id => ApiData.id;

        /// <summary>
        /// Node
        /// </summary>
        public string Node => ApiData.node;

        /// <summary>
        /// Cpu
        /// </summary>
        public double CPU => ApiData.cpu;

        /// <summary>
        /// Max Cpu
        /// </summary>
        public long MaxCPU => ApiData.maxcpu;

        /// <summary>
        /// Disk
        /// </summary>
        public long Disk => ApiData.disk;

        /// <summary>
        /// Max Disk
        /// </summary>
        public long MaxDisk => ApiData.maxdisk;

        /// <summary>
        /// Memory
        /// </summary>
        public long Memory => ApiData.mem;

        /// <summary>
        /// Max Memory
        /// </summary>
        public long MemoryMax => ApiData.maxmem;

        /// <summary>
        /// Status
        /// </summary>
        public string Status => ApiData.status;

        /// <summary>
        /// Check is Online
        /// </summary>
        public bool IsOnline => Status == "online";

        /// <summary>
        /// Pve node
        /// </summary>
        public PveClient.PVENodes.PVEItemNode NodeApi => Client.Nodes[Node];

        /// <summary>
        /// Server id
        /// </summary>
        /// <returns></returns>
        public string ServerId => IsOnline ? NodeApi.Subscription.Get().Response.data.serverid : "";

        /// <summary>
        /// Type
        /// </summary>
        public string Type => ApiData.type;

        /// <summary>
        /// Up Time
        /// </summary>
        public TimeSpan? UpTime => ApiData.uptime == 0 ? null : TimeSpan.FromSeconds(ApiData.uptime);

        /// <summary>
        /// Title info
        /// </summary>
        /// <returns></returns>
        public static string[] GetTitlesInfo()
            => new[] { "NODE", "CPU", "MEM(GB)", "DISK(GB)", "UPTIME", "STATUS", "TYPE" };

        /// <summary>
        /// Row info
        /// </summary>
        /// <returns></returns>
        public string[] GetRowInfo()
            => new string[] { Node,
                              UnitOfMeasurementHelper.CPUUsageToString(CPU, ApiData.maxcpu),
                              UnitOfMeasurementHelper.GbToString(Memory),
                              UnitOfMeasurementHelper.GbToString(MaxDisk),
                              UnitOfMeasurementHelper.UpTimeToString(UpTime),
                              Status,
                              Type };

        /// <summary>
        /// Restore backup
        /// </summary>
        /// <param name="type"></param>
        /// <param name="vmId"></param>
        /// <param name="archive"></param>
        /// <returns></returns>
        public Result RestoreBackup(VMTypeEnum type, int vmId, string archive)
            => type switch
            {
                VMTypeEnum.Qemu => NodeApi.Qemu.CreateRest(vmid: vmId, archive: archive),
                VMTypeEnum.Lxc => NodeApi.Lxc.CreateRest(vmid: vmId, ostemplate: archive),
                _ => null,
            };
    }
}
