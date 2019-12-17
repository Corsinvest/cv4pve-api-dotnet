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
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CreateFileSpaceClient(this PveClient client, string vmIdOrName, string fileName)
        {
            var ret = true;

            var vm = client.GetVM(vmIdOrName);
            if (vm != null)
            {
                var response = vm.QemuApi.Spiceproxy.Spiceproxy(client.Hostname);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Response.data;

                    var contests = $@"[virt-viewer]
host={data.host}
delete-this-file={DynamicHelper.GetValue(data, "delete-this-file")}
password={data.password}
title={data.title}
secure-attention={DynamicHelper.GetValue(data, "secure-attention")}
toggle-fullscreen={DynamicHelper.GetValue(data, "toggle-fullscreen")}
tls-port={DynamicHelper.GetValue(data, "tls-port")}
type={data.type}
release-cursor={DynamicHelper.GetValue(data, "release-cursor")}
host-subject={DynamicHelper.GetValue(data, "host-subject")}
proxy={data.proxy}
ca={data.ca}
";

                    File.WriteAllText(fileName, contests);
                }
                else
                {
                    ret = false;
                }
            }
            else
            {
                ret = false;
            }

            return ret;
        }
    }
}