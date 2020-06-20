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
    /// Snapshot
    /// </summary>
    public class Snapshot
    {
        private readonly dynamic _apiData;

        internal Snapshot(VMInfo vm, object apiData)
        {
            _apiData = apiData;
            VM = vm;

            DynamicHelper.CheckKeyOrCreate(_apiData, "description", "no-description");
            DynamicHelper.CheckKeyOrCreate(_apiData, "parent", "no-parent");
            DynamicHelper.CheckKeyOrCreate(_apiData, "snaptime");
            DynamicHelper.CheckKeyOrCreate(_apiData, "vmstate", 0);

            Date = _apiData.snaptime == null ?
                   DateTime.Now :
                   DateTimeOffset.FromUnixTimeSeconds(_apiData.snaptime).DateTime;
        }

        /// <summary>
        /// VM/CT
        /// </summary>
        /// <value></value>
        public VMInfo VM { get; }

        /// <summary>
        /// Config
        /// </summary>
        /// <value></value>
        public Result Config
            => VM.Type switch
            {
                VMTypeEnum.Qemu => VM.QemuApi.Snapshot[Name].Config.GetRest(),
                VMTypeEnum.Lxc => VM.LxcApi.Snapshot[Name].Config.GetRest(),
                _ => null,
            };

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public Result Update(string description)
            => VM.Type switch
            {
                VMTypeEnum.Qemu => VM.QemuApi.Snapshot[Name].Config.SetRest(description),
                VMTypeEnum.Lxc => VM.LxcApi.Snapshot[Name].Config.SetRest(description),
                _ => null,
            };

        /// <summary>
        /// Rollback
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Result Rollback(long timeout)
        {
            Result result = null;
            switch (VM.Type)
            {
                case VMTypeEnum.Qemu: result = VM.QemuApi.Snapshot[Name].Rollback.CreateRest(); break;
                case VMTypeEnum.Lxc: result = VM.LxcApi.Snapshot[Name].Rollback.CreateRest(); break;
                default: break;
            }
            result.WaitForTaskToFinish(VM, timeout);
            return result;
        }

        /// <summary>
        /// Date
        /// </summary>
        /// <value></value>
        public DateTime Date { get; }

        /// <summary>
        /// Parent
        /// </summary>
        public string Parent => _apiData.parent;

        /// <summary>
        /// Name
        /// </summary>
        public string Name => _apiData.name;

        /// <summary>
        /// Description
        /// </summary>
        public string Description => (_apiData.description as string).TrimEnd();

        /// <summary>
        /// Ram used
        /// </summary>
        public bool Ram => _apiData.vmstate != 0;

        /// <summary>
        /// Get title info
        /// </summary>
        /// <param name="showNodeAndVm"></param>
        /// <returns></returns>
        public static string[] GetTitlesInfo(bool showNodeAndVm)
        {
            var data = new List<string>();
            if (showNodeAndVm) { data.AddRange(new[] { "NODE", "VM" }); }
            data.AddRange(new[] { "TIME", "PARENT", "NAME", "DESCRIPTION", "RAM" });
            return data.ToArray();
        }

        /// <summary>
        /// Row info
        /// </summary>
        /// <param name="showNodeAndVm"></param>
        /// <returns></returns>
        public string[] GetRowInfo(bool showNodeAndVm)
        {
            var data = new List<string>();
            if (showNodeAndVm) { data.AddRange(new[] { VM.Node, VM.Id, }); }
            data.AddRange(new[]{ Date.ToString("yy/MM/dd HH:mm:ss"),
                                 Parent,
                                 Name,
                                 Description,
                                 Ram ? "X" : ""});

            return data.ToArray();
        }
    }
}