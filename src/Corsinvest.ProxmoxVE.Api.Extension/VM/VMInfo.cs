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
using System.Collections.Generic;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// vm iNFO
    /// </summary>
    public class VMInfo : BaseInfo
    {
        internal VMInfo(PveClient client, object apiData) : base(client, apiData) { }

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id => ApiData.vmid + "";

        /// <summary>
        /// Name
        /// </summary>
        public string Name => DynamicHelper.GetValue(ApiData, "name") + "";

        /// <summary>
        /// Node
        /// </summary>
        public string Node => ApiData.node;

        /// <summary>
        /// CPU
        /// </summary>
        public double CPU => ApiData.cpu;

        /// <summary>
        /// Memory
        /// </summary>
        public long Memory => ApiData.mem;

        /// <summary>
        /// Memory Max
        /// </summary>
        public long MemoryMax => ApiData.maxmem;

        /// <summary>
        /// Max Disk
        /// </summary>
        public long MaxDisk => ApiData.maxdisk;

        /// <summary>
        /// Max CPU
        /// </summary>
        public long MaxCpu => ApiData.maxcpu;

        /// <summary>
        /// Up Time
        /// </summary>
        /// <returns></returns>
        public TimeSpan? UpTime => ApiData.uptime == 0 ? null : TimeSpan.FromSeconds(ApiData.uptime);

        /// <summary>
        /// Is Template
        /// </summary>
        public bool IsTemplate => ApiData.template == 1;

        /// <summary>
        /// Status
        /// </summary>
        public string Status => ApiData.status;

        /// <summary>
        /// Check is running
        /// </summary>
        public bool IsRunning => Status == "running";

        /// <summary>
        /// Check is running
        /// </summary>
        public bool IsStopped => Status == "stopped";

        /// <summary>
        /// Check is unknown
        /// </summary>
        public bool IsUnknown => Status == "unknown";

        /// <summary>
        /// Type
        /// </summary>
        /// <value></value>
        public VMTypeEnum Type
            => ApiData.type switch
            {
                "lxc" => VMTypeEnum.Lxc,
                "qemu" => VMTypeEnum.Qemu,
                _ => VMTypeEnum.Qemu,
            };

        /// <summary>
        /// Get file for SPICE client using spiceconfig
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public string GetSpiceFileVV(string proxy)
        {
            var ret = "";

            if (Type == VMTypeEnum.Qemu)
            {
                var oldResponseType = Client.ResponseType;
                Client.ResponseType = ResponseType.None;

                var response = Client.Create($"/spiceconfig/nodes/{Node}/qemu/{Id}/spiceproxy",
                                             new Dictionary<string, object> { { "proxy", proxy } });

                Client.ResponseType = oldResponseType;
                ret = response.Response + "";
            }

            return ret;
        }

        /// <summary>
        /// Se status
        /// </summary>
        /// <param name="state"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool SetStatus(StatusEnum state, long timeout)
        {
            Result result = null;

            switch (state)
            {
                case StatusEnum.Reset:
                    switch (Type)
                    {
                        case VMTypeEnum.Qemu: result = QemuApi.Status.Reset.VmReset(); break;
                        case VMTypeEnum.Lxc: throw new ApplicationException("Not possible in Container");
                    }
                    break;

                case StatusEnum.Shutdown:
                    switch (Type)
                    {
                        case VMTypeEnum.Qemu: result = QemuApi.Status.Shutdown.VmShutdown(); break;
                        case VMTypeEnum.Lxc: result = LxcApi.Status.Shutdown.VmShutdown(); break;
                    }
                    break;

                case StatusEnum.Start:
                    switch (Type)
                    {
                        case VMTypeEnum.Qemu: result = QemuApi.Status.Start.VmStart(); break;
                        case VMTypeEnum.Lxc: result = LxcApi.Status.Start.VmStart(); break;
                    }
                    break;

                case StatusEnum.Stop:
                    switch (Type)
                    {
                        case VMTypeEnum.Qemu: result = QemuApi.Status.Stop.VmStop(); break;
                        case VMTypeEnum.Lxc: result = LxcApi.Status.Stop.VmStop(); break;
                    }
                    break;

                case StatusEnum.Suspend:
                    switch (Type)
                    {
                        case VMTypeEnum.Qemu: result = QemuApi.Status.Suspend.VmSuspend(); break;
                        case VMTypeEnum.Lxc: result = LxcApi.Status.Suspend.VmSuspend(); break;
                    }
                    break;
            }

            result.WaitForTaskToFinish(this, timeout);
            return !result.InError();
        }

        internal PveClient.PVENodes.PVEItemNode NodeApi => Client.Nodes[Node];

        /// <summary>
        /// QEMU
        /// </summary>
        /// <returns></returns>
        public PveClient.PVENodes.PVEItemNode.PVEQemu.PVEItemVmid QemuApi => (dynamic)NodeApi.Qemu[Id];

        /// <summary>
        /// LXC
        /// </summary>
        /// <returns></returns>
        public PveClient.PVENodes.PVEItemNode.PVELxc.PVEItemVmid LxcApi => (dynamic)NodeApi.Lxc[Id];


        /// <summary>
        /// Config
        /// </summary>
        /// <value></value>
        public Config Config
            => Type switch
            {
                VMTypeEnum.Qemu => new ConfigQemu(this, QemuApi.Config.GetRest(true).Response.data),
                VMTypeEnum.Lxc => new ConfigLxc(this, LxcApi.Config.GetRest().Response.data),
                _ => null,
            };

        /// <summary>
        /// Snapshots
        /// </summary>
        /// <returns></returns>
        public Snapshots Snapshots => new Snapshots(this);

        /// <summary>
        /// Backup
        /// </summary>
        /// <returns></returns>
        public Backup Backup => new Backup(this);

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="target"></param>
        /// <param name="online"></param>
        /// <returns></returns>
        public Result Migrate(string target, bool online)
            => Type switch
            {
                VMTypeEnum.Qemu => QemuApi.Migrate.MigrateVm(target, online: online),
                VMTypeEnum.Lxc => LxcApi.Migrate.MigrateVm(target, online: online),
                _ => null,
            };

        /// <summary>
        /// Titles info
        /// </summary>
        /// <returns></returns>
        public static string[] GetTitlesInfo()
            => new[] { "NODE", "VMID", "NAME", "OS TYPE", "MEM(GB)",
                       "DISK(GB)", "UPTIME", "CPU", "STATUS", "TYPE" };

        /// <summary>
        /// Row Info
        /// </summary>
        /// <returns></returns>
        public string[] GetRowInfo()
            => new[] { Node,
                       Id,
                       Name,
                       Config.OsType,
                       UnitOfMeasurementHelper.GbToString(Memory),
                       UnitOfMeasurementHelper.GbToString(MaxDisk) ,
                       UnitOfMeasurementHelper.UpTimeToString(UpTime),
                       UnitOfMeasurementHelper.CPUUsageToString(CPU,MaxCpu),
                       Status,
                       Type + ""};
    }
}