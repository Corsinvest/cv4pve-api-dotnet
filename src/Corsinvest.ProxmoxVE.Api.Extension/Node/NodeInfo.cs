using System;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Extension.VM;

namespace Corsinvest.ProxmoxVE.Api.Extension.Node
{
    public class NodeInfo : BaseInfo
    {
        private const string FORMAT = "{0,-10} {1,7} {2,8} {3,-9} {4,-7} {5,-4}";

        internal NodeInfo(Client client, object apiData) : base(client, apiData) { }

        public string Id => ApiData.id;
        public string Node => ApiData.node;
        public double CPU => ApiData.cpu;
        public int MaxCPU => ApiData.maxcpu;
        public long Disk => ApiData.disk;
        public long MaxDisk => ApiData.maxdisk;
        public long Memory => ApiData.mem;
        public long MemoryMax => ApiData.maxmem;
        public string Status => ApiData.status;
        public string Type => ApiData.type;
        public TimeSpan? UpTime => ApiData.uptime == 0 ? null : TimeSpan.FromSeconds(ApiData.uptime);
        public static string HeaderInfo() => string.Format(FORMAT, "NODE", "MEM(GB)", "DISK(GB)", "UPTIME", "STATUS", "TYPE");

        public string RowInfo()
            => string.Format(FORMAT,
                             Node,
                             UnitOfMeasurementHelper.GbToString(Memory),
                             UnitOfMeasurementHelper.GbToString(MaxDisk),
                             UnitOfMeasurementHelper.UpTimeToString(UpTime),
                             Status,
                             Type);

        public string[] Info()
            => new string[]{ $"Node:   {Node}",
                             $"Id:     {Id}",
                             $"CPU:    {UnitOfMeasurementHelper.CPUUsageToStirng(CPU, ApiData.maxcpu)}",
                             $"Memory: {UnitOfMeasurementHelper.MbToString(Memory)} MB",
                             $"Disk:   {UnitOfMeasurementHelper.GbToString(MaxDisk)} GB",
                             $"Uptime: {UnitOfMeasurementHelper.UpTimeToString(UpTime)}",
                             $"Status: {Status}",
                             $"Type:   {Type}"};

        public Result RestoreBackup(VMTypeEnum type, int vmId, string archive)
        {
            switch (type)
            {
                case VMTypeEnum.Qemu: return Client.Nodes[Id].Qemu.CreateRest(vmid: vmId, archive: archive);
                case VMTypeEnum.Lxc: return Client.Nodes[Id].Lxc.CreateRest(vmid: vmId, ostemplate: archive);
                default: return null;
            }
        }
    }
}