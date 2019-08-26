using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Configuration
    /// </summary>
    public abstract class Config
    {
        internal Config(VMInfo vm, object apiData)
        {
            ApiData = apiData;
            VM = vm;
        }

        /// <summary>
        /// Info VM
        /// </summary>
        /// <value></value>
        public VMInfo VM { get; }

        /// <summary>
        /// Data Json API.
        /// </summary>
        /// <value></value>
        public dynamic ApiData { get; }

        /// <summary>
        /// Cores assigned.
        /// </summary>
        public int Cores => ApiData.cores;

        /// <summary>
        /// Memory
        /// </summary>
        public int Memory => ApiData.memory;

        /// <summary>
        /// Os type
        /// </summary>
        public string OsType => ApiData.ostype;

        /// <summary>
        /// Get disks.
        /// </summary>
        /// <value></value>
        public IReadOnlyList<Disk> Disks
        {
            get
            {
                var disks = new List<Disk>();
                foreach (var item in (IDictionary<string, object>)ApiData)
                {
                    var definition = item.Value + "";

                    switch (VM.Type)
                    {
                        case VMTypeEnum.Qemu:
                            if (Regex.IsMatch(item.Key, @"(ide|sata|scsi|virtio)\d+") &&
                                !Regex.IsMatch(definition, "cdrom|none"))
                            {
                                disks.Add(new DiskQemu(VM.Client, item.Key, definition));
                            }
                            break;

                        case VMTypeEnum.Lxc:
                            if (item.Key == "rootfs") { disks.Add(new DiskLxc(VM.Client, item.Key, definition)); }
                            break;

                        default: break;
                    }
                }

                return disks.AsReadOnly();
            }
        }

        private string CreateConfig(IDictionary<string, object> items, string snapName)
        {
            var ret = "";
            if (items != null)
            {
                if (!string.IsNullOrWhiteSpace(snapName)) { ret += $"[{snapName}]" + Environment.NewLine; }

                var retTmp = "";
                foreach (var key in items.Keys.OrderBy(a => a))
                {
                    var value = items[key];
                    if (key == "description") { ret += $"#{value}" + Environment.NewLine; }
                  //  else if (key == "digest") { }
                    else { retTmp += $"{key}: {value}" + Environment.NewLine; }
                }

                ret += retTmp;
            }

            return ret;
        }

        /// <summary>
        /// Get all configuration 
        /// </summary>
        /// <returns></returns>
        public string GetAllConfigs()
        {
            var ret = new List<string>();

            //current
            ret.Add(CreateConfig(ApiData, null));

            //snapshots
            foreach (var snapshot in VM.Snapshots)
            {
                ret.Add(CreateConfig(snapshot.Config.Response.data, snapshot.Name));
            }

            return string.Join(Environment.NewLine, ret);
        }
    }
}