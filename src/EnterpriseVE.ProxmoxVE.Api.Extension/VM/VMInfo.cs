using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseVE.ProxmoxVE.Api.Extension.Utils;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    public class VMInfo : BaseInfo
    {
        private const string FORMAT = "{0,-10} {1,5} {2,-20} {3,7} {4,8} {5,-9} {6,-7} {7,-4}";

        internal VMInfo(Client client, object apiData) : base(client, apiData) { }
        public string Id => ApiData.vmid + "";
        public string Name => ApiData.name + "";
        public string Node => ApiData.node;

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

        public double CPU => ApiData.cpu;
        public long Memory => ApiData.mem;
        public long MemoryMax => ApiData.maxmem;
        public long MaxDisk => ApiData.maxdisk;
        public TimeSpan? UpTime => ApiData.uptime == 0 ? null : TimeSpan.FromSeconds(ApiData.uptime);
        public bool IsTemplate => ApiData.template == 1;

        public string Status => ApiData.status;

        public bool SetStatus(StatusEnum state, bool wait)
        {
            var ret = true;

            if (state != StatusEnum.Current)
            {
                var status = VMApi.Status;
                Result result = null;

                switch (state)
                {
                    case StatusEnum.Reset:
                        if (Type == VMTypeEnum.Lxc) { return false; }

                        result = status.Reset.VmReset();
                        break;

                    case StatusEnum.Shutdown: result = status.Shutdown.VmShutdown(); break;
                    case StatusEnum.Start: result = status.Start.VmStart(); break;
                    case StatusEnum.Stop: result = status.Stop.VmStop(); break;
                    case StatusEnum.Suspend: result = status.Suspend.VmSuspend(); break;
                }

                result.WaitForTaskToFinish(this, wait);
                if (result.InError()) { ret = false; }
            }

            return ret;
        }

        internal Client.PVENodes.PVEItemNode NodeApi => Client.Nodes[Node];

        internal dynamic VMApi
        {
            get => Type == VMTypeEnum.Qemu ? (dynamic)NodeApi.Qemu[Id] : (dynamic)NodeApi.Lxc[Id];
        }

        public Config Config
        {
            get
            {
                switch (Type)
                {
                    case VMTypeEnum.Qemu:
                        return new ConfigQemu(this, VMApi.Config.GetRest(true).Response.data);

                    case VMTypeEnum.Lxc:
                        return new ConfigLxc(this, VMApi.Config.GetRest().Response.data);

                    default: return null;
                }
            }
        }

        public Snapshots Snapshots => new Snapshots(this);

        public Backup Backup => new Backup(this);

        public Result Migrate(string target, bool online) { return VMApi.Migrate.MigrateVm(target, online: online); }

        public static string HeaderInfo()
        {
            return string.Format(FORMAT, "NODE", "VMID", "NAME", "MEM(MB)", "DISK(GB)", "UPTIME", "STATUS", "TYPE");
        }

        private string[] GetInfoValue()
        {
            return new string[] {
                Node,
                Id,
                Name,
                UnitOfMeasurementHelper.MbToString(Memory),
                UnitOfMeasurementHelper.GbToString(MaxDisk),
                UnitOfMeasurementHelper.UpTimeToString(UpTime),
                Status,
                Type + ""
            };
        }


        public string RowInfo() { return string.Format(FORMAT, GetInfoValue()); }

        public string[] Info()
        {
            var infoValue = GetInfoValue();
            return new string[] {
                $"Node:   {infoValue[0]}",
                $"VmId:   {infoValue[1]}",
                $"Name:   {infoValue[2]}",
                $"CPU:    {infoValue[3]}",
                $"Memory: {infoValue[4]} MB",
                $"Disk:   {infoValue[5]} GB",
                $"Uptime: {infoValue[6]}",
                $"Status: {infoValue[7]}",
                $"Type:   {infoValue[8]}",
            };
        }
    }
}