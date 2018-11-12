using System.Collections.Generic;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    public static class VMExtension
    {
        /// <summary>
        /// Info vms.
        /// </summary>
        /// <param name="vms"></param>
        /// <returns></returns>
        public static string[] Info(this IReadOnlyList<VMInfo> vms)
        {
            var ret = new List<string>();
            ret.Add(VMInfo.HeaderInfo());
            foreach (var vm in vms) { ret.Add(vm.RowInfo()); }
            return ret.ToArray();
        }

        public static string[] Info(this IEnumerable<Snapshot> snapshots,bool showNodeAndVm)
        {
            var ret = new List<string>();
            ret.Add(Snapshot.HeaderInfo(showNodeAndVm));
            foreach (var snapshot in snapshots) { ret.Add(snapshot.RowInfo(showNodeAndVm)); }
            return ret.ToArray();
        }
    }
}