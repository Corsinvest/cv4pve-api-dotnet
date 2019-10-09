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
        /// <param name="wait"></param>
        /// <returns></returns>
        public Result Create(string name, string description, bool state, bool wait)
        {
            Result result = null;
            switch (_vm.Type)
            {
                case VMTypeEnum.Qemu: result = _vm.QemuApi.Snapshot.CreateRest(name, description, state); break;
                case VMTypeEnum.Lxc: result = _vm.LxcApi.Snapshot.CreateRest(name, description); break;
                default: break;
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
                default: break;
            }

            result.WaitForTaskToFinish(_vm, wait);

            RefreshList();

            return result;
        }
    }
}