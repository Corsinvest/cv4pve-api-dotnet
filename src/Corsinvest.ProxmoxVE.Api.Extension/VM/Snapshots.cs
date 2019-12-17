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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Snapshots
    /// </summary>
    public class Snapshots : IReadOnlyList<Snapshot>
    {
        private readonly VMInfo _vm;
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
                default: break;
            }

            foreach (var snapshot in result.Response.data) { snapshots.Add(new Snapshot(_vm, snapshot)); }
            _snapshots = snapshots.OrderBy(a => a.Date).ToList();
        }

        /// <summary>
        /// Get from name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Snapshot this[string name] => _snapshots.Where(a => a.Name == name).FirstOrDefault();

        /// <summary>
        /// Get from index
        /// </summary>
        public Snapshot this[int index] => _snapshots[index];

        /// <summary>
        /// Count
        /// </summary>
        public int Count => _snapshots.Count;

        /// <summary>
        /// Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Snapshot> GetEnumerator() => _snapshots.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_snapshots).GetEnumerator();

        /// <summary>
        /// Create new snapshot
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="state"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Result Create(string name, string description, bool state, long timeout)
        {
            Result result = null;
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: result = _vm.QemuApi.Snapshot.CreateRest(name, description, state); break;
                case VMTypeEnum.Lxc: result = _vm.LxcApi.Snapshot.CreateRest(name, description); break;
                default: break;
            }

            result.WaitForTaskToFinish(_vm, timeout);

            RefreshList();

            return result;
        }

        /// <summary>
        /// Clear a VM/CT snapshot.
        /// </summary>
        /// <param name="keep"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Result Clear(int keep, long timeout)
        {
            Result result = null;
            var snapshots = this.OrderBy(a => a.Date).ToArray();
            for (int i = 0; i < Count - keep; i++)
            {
                result = Remove(snapshots[i], timeout);
                if (result.ResponseInError) { break; }
            }

            return result;
        }

        /// <summary>
        /// Remove snapshot.
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Result Remove(Snapshot snapshot, long timeout)
        {
            Result result = null;
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: result = _vm.QemuApi.Snapshot[snapshot.Name].DeleteRest(); break;
                case VMTypeEnum.Lxc: result = _vm.LxcApi.Snapshot[snapshot.Name].DeleteRest(); break;
                default: break;
            }

            result.WaitForTaskToFinish(_vm, timeout);

            RefreshList();

            return result;
        }
    }
}