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
 
namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Backup function
    /// </summary>
    public class Backup
    {
        private VMInfo _vm;

        internal Backup(VMInfo vm) => _vm = vm;

        /// <summary>
        /// Information backup.
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public Result Info(string volume) => _vm.NodeApi.Vzdump.Extractconfig.GetRest(volume);

        /// <summary>
        /// Create backup.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="compress"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailnotification"></param>
        /// <param name="storage"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public Result Create(string mode,
                             string compress,
                             string mailTo,
                             BackupMailNotificationEnum mailnotification,
                             string storage,
                             bool wait)
        {
            var result = _vm.NodeApi.Vzdump.CreateRest(vmid: _vm.Id,
                                                       mode: mode,
                                                       compress: compress,
                                                       remove: false,
                                                       mailto: mailTo,
                                                       mailnotification: (mailnotification + "").ToLower(),
                                                       storage: storage);

            result.WaitForTaskToFinish(_vm, wait);
            return result;
        }
    }
}