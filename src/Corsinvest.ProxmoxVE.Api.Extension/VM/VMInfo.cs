using System;
using System.Collections.Generic;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// vm iNFO
    /// </summary>
    public class VMInfo : BaseInfo
    {
        private const string FORMAT = "{0,-10} {1,5} {2,-20} {3,7} {4,8} {5,-9} {6,-7} {7,-4}";

        internal VMInfo(Client client, object apiData) : base(client, apiData) { }

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id => ApiData.vmid + "";

        /// <summary>
        /// Name
        /// </summary>
        public string Name => ApiData.name + "";

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
        /// Type
        /// </summary>
        /// <value></value>
        public VMTypeEnum Type
        {
            get
            {
                switch (ApiData.type)
                {
                    case "lxc": return VMTypeEnum.Lxc;
                    case "qemu": return VMTypeEnum.Qemu;
                    default: return VMTypeEnum.Qemu;
                }
            }
        }

        /// <summary>
        /// Se status
        /// </summary>
        /// <param name="state"></param>
        /// <param name="wait"></param>
        /// <returns></returns>

        public bool SetStatus(StatusEnum state, bool wait)
        {
            Result result = null;

            switch (state)
            {
                case StatusEnum.Reset:
                    switch (Type)
                    {
                        case VMTypeEnum.Qemu: result = QemuApi.Status.Reset.VmReset(); break;
                        case VMTypeEnum.Lxc: throw new Exception("Not possible in Container");
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

            result.WaitForTaskToFinish(this, wait);
            return !result.InError();
        }

        internal Client.PVENodes.PVEItemNode NodeApi => Client.Nodes[Node];

        /// <summary>
        /// QEMU
        /// </summary>
        /// <returns></returns>
        public Client.PVENodes.PVEItemNode.PVEQemu.PVEItemVmid QemuApi => (dynamic)Client.Nodes[Node].Qemu[Id];

        /// <summary>
        /// LXC
        /// </summary>
        /// <returns></returns>
        public Client.PVENodes.PVEItemNode.PVELxc.PVEItemVmid LxcApi => (dynamic)Client.Nodes[Node].Lxc[Id];

        /// <summary>
        /// Config
        /// </summary>
        /// <value></value>
        public Config Config
        {
            get
            {
                switch (Type)
                {
                    case VMTypeEnum.Qemu: return new ConfigQemu(this, QemuApi.Config.GetRest(true).Response.data);
                    case VMTypeEnum.Lxc: return new ConfigLxc(this, LxcApi.Config.GetRest().Response.data);
                    default: return null;
                }
            }
        }

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
        {
            switch (Type)
            {
                case VMTypeEnum.Qemu: return QemuApi.Migrate.MigrateVm(target, online: online);
                case VMTypeEnum.Lxc: return LxcApi.Migrate.MigrateVm(target, online: online);
                default: return null;
            }
        }

        /// <summary>
        /// Header info
        /// </summary>
        /// <returns></returns>
        public static string HeaderInfo()
            => string.Format(FORMAT, "NODE", "VMID", "NAME", "MEM(GB)", "DISK(GB)", "UPTIME", "STATUS", "TYPE");

        private string[] GetInfoValue()
            => new string[] { Node,
                              Id,
                              Name,
                              UnitOfMeasurementHelper.GbToString(Memory),
                              UnitOfMeasurementHelper.GbToString(MaxDisk),
                              UnitOfMeasurementHelper.UpTimeToString(UpTime),
                              Status,
                              Type + ""};

        /// <summary>
        /// ROw info
        /// </summary>
        /// <returns></returns>
        public string RowInfo() => string.Format(FORMAT, GetInfoValue());

        /// <summary>
        /// Info
        /// </summary>
        /// <returns></returns>
        public string[] Info()
        {
            var ret = new List<string>(
                new string[] {
                    $"Node:      {Node}",
                    $"VmId:      {Id}",
                    $"OsType:    {Config.OsType}",
                    $"Name:      {Name}",
                    $"CPU:       {Math.Round(CPU,2)}",
                    $"MaxCpu:    {MaxCpu}",
                    $"Memory:    {UnitOfMeasurementHelper.GbToString(Memory)} GB",
                    $"MemoryMax: {UnitOfMeasurementHelper.GbToString(MemoryMax)} GB",
                    $"Uptime:    {UnitOfMeasurementHelper.UpTimeToString(UpTime)}",
                    $"Status:    {Status}",
                    $"Type:      {Type}",
                    $"Disk:      {UnitOfMeasurementHelper.GbToString(MaxDisk)} GB"});

            foreach (var disk in Config.Disks)
            {
                ret.Add($"{disk.Storage} {disk.StorageInfo.TypeString} {disk.Name} {disk.Size}");
            }

            return ret.ToArray();
        }
    }
}