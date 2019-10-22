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

using System.Collections.Generic;
using System.Linq;
using BetterConsoleTables;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// VM Extension
    /// </summary>
    public static class VMExtension
    {
        /// <summary>
        /// Info vms.
        /// </summary>
        /// <param name="vms"></param>
        /// <returns></returns>
        public static string Info(this IReadOnlyList<VMInfo> vms)
        {
            if (vms.Count == 0)
            {
                var table = new Table(TableConfiguration.Unicode());
                table.AddColumns(Alignment.Left, Alignment.Left, VMInfo.GetTitlesInfo());
                foreach (var vm in vms) { table.AddRow(vm.GetRowInfo()); }
                return table.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Info snapshots
        /// </summary>
        /// <param name="snapshots"></param>
        /// <param name="showNodeAndVm"></param>
        /// <returns></returns>
        public static string Info(this IEnumerable<Snapshot> snapshots, bool showNodeAndVm)
        {
            if (snapshots.Count() > 0)
            {
                var table = new Table(TableConfiguration.Unicode());
                table.AddColumns(Alignment.Left, Alignment.Left, Snapshot.GetTitlesInfo(showNodeAndVm));
                foreach (var snapshot in snapshots) { table.AddRow(snapshot.GetRowInfo(showNodeAndVm)); }
                return table.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}