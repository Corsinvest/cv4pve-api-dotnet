/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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