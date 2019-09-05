using System;
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
                table.AddColumns(VMInfo.GetTitlesInfo());
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
                table.AddColumns(Snapshot.GetTitlesInfo(showNodeAndVm));
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