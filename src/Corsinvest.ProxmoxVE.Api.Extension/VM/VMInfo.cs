using System;
using System.Collections.Generic;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    public class VMInfo : BaseInfo
    {
        private const string FORMAT = "{0,-10} {1,5} {2,-20} {3,7} {4,8} {5,-9} {6,-7} {7,-4}";

        internal VMInfo(Client client, object apiData) : base(client, apiData) { }
        public string Id => ApiData.vmid + "";
        public string Name => ApiData.name + "";
        public string Node => ApiData.node;
        public double CPU => ApiData.cpu;
        public long Memory => ApiData.mem;
        public long MemoryMax => ApiData.maxmem;
        public long MaxDisk => ApiData.maxdisk;
        public long MaxCpu => ApiData.maxcpu;
        public TimeSpan? UpTime => ApiData.uptime == 0 ? null : TimeSpan.FromSeconds(ApiData.uptime);
        public bool IsTemplate => ApiData.template == 1;
        public string Status => ApiData.status;

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
        public Client.PVENodes.PVEItemNode.PVEQemu.PVEItemVmid QemuApi => (dynamic)Client.Nodes[Node].Qemu[Id];
        public Client.PVENodes.PVEItemNode.PVELxc.PVEItemVmid LxcApi => (dynamic)Client.Nodes[Node].Lxc[Id];

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

        public Snapshots Snapshots => new Snapshots(this);
        public Backup Backup => new Backup(this);

        public Result Migrate(string target, bool online)
        {
            switch (Type)
            {
                case VMTypeEnum.Qemu: return QemuApi.Migrate.MigrateVm(target, online: online);
                case VMTypeEnum.Lxc: return LxcApi.Migrate.MigrateVm(target, online: online);
                default: return null;
            }
        }

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

        public string RowInfo() => string.Format(FORMAT, GetInfoValue());

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