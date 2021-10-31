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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;

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
            DynamicHelper.CheckKeyOrCreate(apiData, "lock", "");
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
        /// Lock
        /// </summary>
        public string Lock => DynamicHelper.GetValue(ApiData, "lock", "");

        /// <summary>
        /// Is lock
        /// </summary>
        /// <returns></returns>
        public bool IsLock => !string.IsNullOrWhiteSpace(Lock);

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
            var ret = new StringBuilder();
            if (items != null)
            {
                if (!string.IsNullOrWhiteSpace(snapName)) { ret.AppendLine($"[{snapName}]"); }

                var retTmp = new StringBuilder();
                foreach (var key in items.Keys.OrderBy(a => a))
                {
                    var value = items[key];
                    if (key == "description") { ret.AppendLine($"#{value}"); }
                    else { retTmp.AppendLine($"{key}: {value}"); }
                }

                ret.Append(retTmp.ToString());
            }

            return ret.ToString();
        }

        /// <summary>
        /// Get all configuration
        /// </summary>
        /// <returns></returns>
        public string GetAllConfigs()
        {
            var ret = new List<string>
            {
                //current
                CreateConfig(ApiData, null)
            };

            //snapshots
            foreach (var snapshot in VM.Snapshots)
            {
                ret.Add(CreateConfig(snapshot.Config.Response.data, snapshot.Name));
            }

            return string.Join(Environment.NewLine, ret);
        }
    }
}