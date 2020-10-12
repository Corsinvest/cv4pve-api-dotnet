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

using System.IO;
using System.Text;
using Corsinvest.ProxmoxVE.Api.Extension.VM;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers
{
    /// <summary>
    /// SPICE client helper
    /// </summary>
    public static class SpiceHelper
    {
        /// <summary>
        /// Create file for SPICE client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="vmIdOrName"></param>
        /// <param name="proxy"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CreateFileSpaceClient(this PveClient client,
                                                 string vmIdOrName,
                                                 string proxy,
                                                 string fileName)
        {
            var ret = false;

            var vm = client.GetVM(vmIdOrName);
            if (vm != null && vm.Type == VMTypeEnum.Qemu)
            {
                var response = vm.QemuApi.Spiceproxy.Spiceproxy(proxy);
                if (response.IsSuccessStatusCode)
                {
                    var contests = new StringBuilder();
                    contests.AppendLine("[virt-viewer]");
                    foreach (var item in response.ToEnumerable()) { contests.AppendLine($"{item.Key}={item.Value}"); }
                    File.WriteAllText(fileName, contests.ToString());
                    ret = true;
                }
            }

            return ret;
        }
    }
}