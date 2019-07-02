using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    public class Snapshots : IReadOnlyList<Snapshot>
    {
        private VMInfo _vm;
        private List<Snapshot> _snapshots;

        internal Snapshots(VMInfo vm)
        {
            _vm = vm;
            RefreshList();
        }

        private void RefreshList()
        {
            var snapshots = new List<Snapshot>();

            Result result = null;
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: result = _vm.QemuApi.Snapshot.GetRest(); break;
                case VMTypeEnum.Lxc: result = _vm.LxcApi.Snapshot.GetRest(); break;
            }

            foreach (var snapshot in result.Response.data) { snapshots.Add(new Snapshot(_vm, snapshot)); }
            _snapshots = snapshots.OrderBy(a => a.Date).ToList();
        }

        public Snapshot this[string name] => _snapshots.Where(a => a.Name == name).FirstOrDefault();
        public Snapshot this[int index] => _snapshots[index];
        public int Count => _snapshots.Count;
        public IEnumerator<Snapshot> GetEnumerator() => _snapshots.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_snapshots).GetEnumerator();

        public Result Create(string name, string description, bool state, bool wait)
        {
            Result result = null;
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: result = _vm.QemuApi.Snapshot.CreateRest(name, description, state); break;
                case VMTypeEnum.Lxc: result = _vm.LxcApi.Snapshot.CreateRest(name, description); break;
            }

            result.WaitForTaskToFinish(_vm, wait);

            RefreshList();

            return result;
        }

        /// <summary>
        /// Clear a VM/CT snapshot.
        /// </summary>
        /// <param name="keep"></param>
        /// <returns></returns>
        public Result Clear(int keep)
        {
            Result result = null;
            var snapshots = this.OrderBy(a => a.Date).ToArray();
            for (int i = 0; i < Count - keep; i++)
            {
                result = Remove(snapshots[i], true);
                if (result.ResponseInError) { break; }
            }

            return result;
        }

        /// <summary>
        /// Remove snapshot.
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public Result Remove(Snapshot snapshot, bool wait)
        {
            Result result = null;
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: result = _vm.QemuApi.Snapshot[snapshot.Name].DeleteRest(); break;
                case VMTypeEnum.Lxc: result = _vm.LxcApi.Snapshot[snapshot.Name].DeleteRest(); break;
            }
            result.WaitForTaskToFinish(_vm, wait);

            RefreshList();

            return result;
        }
    }
}