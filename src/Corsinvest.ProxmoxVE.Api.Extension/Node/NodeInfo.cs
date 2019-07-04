using System;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Extension.VM;

namespace Corsinvest.ProxmoxVE.Api.Extension.Node
{
    /// <summary>
    /// Node info
    /// </summary>
    public class NodeInfo : BaseInfo
    {
        private const string FORMAT = "{0,-10} {1,7} {2,8} {3,-9} {4,-7} {5,-4}";

        internal NodeInfo(Client client, object apiData) : base(client, apiData) { }

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
        public int MaxCPU => ApiData.maxcpu;

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
        /// Type
        /// </summary>
        public string Type => ApiData.type;

        /// <summary>
        /// Up Time
        /// </summary>
        public TimeSpan? UpTime => ApiData.uptime == 0 ? null : TimeSpan.FromSeconds(ApiData.uptime);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string HeaderInfo() => string.Format(FORMAT, "NODE", "MEM(GB)", "DISK(GB)", "UPTIME", "STATUS", "TYPE");

        /// <summary>
        /// Row info
        /// </summary>
        /// <returns></returns>
        public string RowInfo()
            => string.Format(FORMAT,
                             Node,
                             UnitOfMeasurementHelper.GbToString(Memory),
                             UnitOfMeasurementHelper.GbToString(MaxDisk),
                             UnitOfMeasurementHelper.UpTimeToString(UpTime),
                             Status,
                             Type);

        /// <summary>
        /// Info
        /// </summary>
        /// <returns></returns>
        public string[] Info()
            => new string[]{ $"Node:   {Node}",
                             $"Id:     {Id}",
                             $"CPU:    {UnitOfMeasurementHelper.CPUUsageToStirng(CPU, ApiData.maxcpu)}",
                             $"Memory: {UnitOfMeasurementHelper.MbToString(Memory)} MB",
                             $"Disk:   {UnitOfMeasurementHelper.GbToString(MaxDisk)} GB",
                             $"Uptime: {UnitOfMeasurementHelper.UpTimeToString(UpTime)}",
                             $"Status: {Status}",
                             $"Type:   {Type}"};

        /// <summary>
        /// Restore backup
        /// </summary>
        /// <param name="type"></param>
        /// <param name="vmId"></param>
        /// <param name="archive"></param>
        /// <returns></returns>
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