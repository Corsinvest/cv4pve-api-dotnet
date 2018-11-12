using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnterpriseVE.ProxmoxVE.Api.Extension.Utils;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
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
            foreach (var snapshot in _vm.VMApi.Snapshot.GetRest().Response.data)
            {
                snapshots.Add(new Snapshot(_vm, snapshot));
            }
            _snapshots = snapshots.OrderBy(a => a.Date).ToList();
        }

        public Snapshot this[string name] => _snapshots.Where(a => a.Name == name).FirstOrDefault();
        public Snapshot this[int index] => _snapshots[index];
        public int Count => _snapshots.Count;
        public IEnumerator<Snapshot> GetEnumerator() { return _snapshots.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_snapshots).GetEnumerator(); }

        public Result Create(string name, string description, bool state, bool wait)
        {
            Result result = null;
            var node = _vm.NodeApi;
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu:
                    result = node.Qemu[_vm.Id].Snapshot.CreateRest(name, description, state);
                    break;

                case VMTypeEnum.Lxc:
                    result = node.Lxc[_vm.Id].Snapshot.CreateRest(name, description);
                    break;
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
            foreach (var snapshot in this.Reverse().Take(keep + 1).Reverse())
            {
                result = Remove(snapshot, true);
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
            var result = (Result)_vm.VMApi.Snapshot[snapshot.Name].DeleteRest();
            result.WaitForTaskToFinish(_vm, wait);

            RefreshList();

            return result;
        }      
    }
}