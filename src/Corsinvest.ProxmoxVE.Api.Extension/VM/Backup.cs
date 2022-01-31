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

using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Backup function
    /// </summary>
    public class Backup
    {
        private readonly VMInfo _vm;

        internal Backup(VMInfo vm) => _vm = vm;

        /// <summary>
        /// Information backup.
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public async Task<Result> Info(string volume) => await _vm.NodeApi.Vzdump.Extractconfig.GetRest(volume);

        /// <summary>
        /// Create backup.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="compress"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailnotification"></param>
        /// <param name="storage"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Result> Create(string mode,
                                         string compress,
                                         string mailTo,
                                         BackupMailNotificationEnum mailnotification,
                                         string storage,
                                         long timeout)
        {
            var result = await _vm.NodeApi.Vzdump.CreateRest(vmid: _vm.Id,
                                                             mode: mode,
                                                             compress: compress,
                                                             remove: false,
                                                             mailto: mailTo,
                                                             mailnotification: (mailnotification + "").ToLower(),
                                                             storage: storage);

            await result.WaitForTaskToFinish(_vm, timeout);
            return result;
        }
    }
}