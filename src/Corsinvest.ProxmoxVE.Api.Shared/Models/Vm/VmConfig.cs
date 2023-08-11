/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Vm Config
    /// </summary>
    public class VmConfig
    {
        /// <summary>
        /// Architecture type.
        /// </summary>
        [JsonProperty("arch")]
        public string Arch { get; set; }

        /// <summary>
        /// Parent
        /// </summary>
        [JsonProperty("parent")]
        public string Parent { get; set; }

        /// <summary>
        /// Memory
        /// </summary>
        [JsonProperty("memory")]
        public long Memory { get; set; }

        /// <summary>
        /// Os type
        /// </summary>
        [JsonProperty("ostype")]
        public string OsType { get; set; }

        /// <summary>
        /// Vm Os Type
        /// </summary>
        public VmOsType VmOsType
            => OsType switch
            {
                "l26" or "l24" => VmOsType.Linux,
                "win11" or "win10" or "win8" or "win7" or "w2k8" or "wxp" or "w2k" => VmOsType.Windows,
                "solaris" => VmOsType.Solaris,
                "other" => VmOsType.Other,
                _ => VmOsType.Linux,
            };

        /// <summary>
        /// OsType Decode
        /// </summary>
        public string OsTypeDecode
            => OsType switch
            {
                "l26" => "Linux 5.x - 2.6 Kernel",
                "l24" => "Linux 2.4 Kernel",
                "win11" => "Microsoft Windows 11/2022",
                "win10" => "Microsoft Windows 10/2016/2019",
                "win8" => "Microsoft Windows 8.x/2012/2012r2",
                "win7" => "Microsoft Windows 7/2008r2",
                "w2k8" => "Microsoft Windows Vista/2008",
                "wxp" => "Microsoft Windows XP/2003",
                "w2k" => "Microsoft Windows 2000",
                "solaris" => "Solaris Kernel",
                "other" => "Other",
                _ => OsType,
            };

        /// <summary>
        /// Lock
        /// </summary>
        public bool IsLocked => !string.IsNullOrWhiteSpace(Lock);

        /// <summary>
        /// On boot
        /// </summary>
        [JsonProperty("onboot")]
        public bool OnBoot { get; set; }

        /// <summary>
        /// On boot
        /// </summary>
        [JsonProperty("protection")]
        public bool Protection { get; set; }

        /// <summary>
        /// On boot
        /// </summary>
        [JsonProperty("lock")]
        public string Lock { get; set; }

        /// <summary>
        /// /// Disks
        /// </summary>
        public IEnumerable<VmDisk> Disks { get; private set; }

        /// <summary>
        /// Extension Data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        [OnDeserialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            var disks = new List<VmDisk>();
            foreach (var key in ExtensionData.Keys)
            {
                var def = ExtensionData[key] + "";
                if (key == "rootfs"
                    || (Regex.IsMatch(key, @"(efidisk|virtio|ide|scsi|sata|mp)\d+") && !Regex.IsMatch(def, "media=cdrom")) //bus match
                    || key == "efidisk0")
                {
                    var info1 = def.Split(':');
                    var info2 = info1[1].Split(',');

                    disks.Add(new VmDisk
                    {
                        Id = key,
                        Storage = info1[0],
                        FileName = info2[0],
                        Size = info2.Where(a => a.StartsWith("size=")).Select(a => a.Substring(5)).FirstOrDefault(),
                        Backup = !info2.Contains("backup=0")
                    });
                }
            }
            Disks = disks;
        }
    }
}