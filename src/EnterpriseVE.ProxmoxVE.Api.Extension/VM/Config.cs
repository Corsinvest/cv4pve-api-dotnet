using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    public abstract class Config
    {
        private VMInfo _vm;

        internal Config(VMInfo vm, object apiData)
        {
            ApiData = apiData;
            _vm = vm;
        }

        protected dynamic ApiData { get; }
        public int Cores => ApiData.cores;
        public int Memory => ApiData.memory;
        public string OsType => ApiData.ostype;

        public IReadOnlyList<Disk> Disks
        {
            get
            {
                var disks = new List<Disk>();
                foreach (string key in ApiData)
                {
                    string definition = ApiData[key];
                    switch (_vm.Type)
                    {
                        case VMTypeEnum.Qemu:
                            if (Regex.IsMatch(key, @"(ide|sata|scsi|virtio)\d+") &&
                                !Regex.IsMatch(definition, "cdrom|none"))
                            {

                                disks.Add(new DiskQemu(key, definition));
                            }
                            break;

                        case VMTypeEnum.Lxc:
                            if (key == "rootfs") { disks.Add(new DiskLxc(key, definition)); }
                            break;

                        default: break;
                    }

                }
                return disks.AsReadOnly();
            }
        }

        private string CreateConfig(IDictionary<string, string> items, string snapName)
        {
            var ret = "";
            if (!string.IsNullOrWhiteSpace(snapName)) { ret += $"[{snapName}]" + Environment.NewLine; }
            foreach (var item in items) { ret += $"{item.Key} {item.Value}" + Environment.NewLine; }
            return ret;
        }

        public string GetAllConfigs()
        {
            //current
            var ret = CreateConfig(ApiData, null);

            //snapshots
            foreach (var snapshot in _vm.Snapshots)
            {
                ret += CreateConfig(snapshot.Config.Response.data, snapshot.Name);
            }

            return ret.ToString();
        }
    }
}