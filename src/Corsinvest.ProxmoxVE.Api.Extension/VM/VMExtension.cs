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
            var table = new Table(TableConfiguration.Unicode());
            table.AddColumns(VMInfo.GetTitlesInfo());
            foreach (var vm in vms) { table.AddRow(vm.GetRowInfo()); }
            return table.ToString();
        }

        /// <summary>
        /// Info snapshots
        /// </summary>
        /// <param name="snapshots"></param>
        /// <param name="showNodeAndVm"></param>
        /// <returns></returns>
        public static string Info(this IEnumerable<Snapshot> snapshots, bool showNodeAndVm)
        {
            var table = new Table(TableConfiguration.Unicode());
            table.AddColumns(Snapshot.GetTitlesInfo(showNodeAndVm));
            foreach (var snapshot in snapshots) { table.AddRow(snapshot.GetRowInfo(showNodeAndVm)); }
            return table.ToString();
        }
    }
}