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

using System.Collections.Generic;
using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;

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
        /// <param name="outputType"></param>
        /// <returns></returns>
        public static string Info(this IReadOnlyList<VMInfo> vms, TableOutputType outputType)
        {
            return vms.Count > 0 ?
                    TableHelper.Create(VMInfo.GetTitlesInfo(),
                                       vms.Select(a => a.GetRowInfo()),
                                       outputType,
                                       false) :
                    "";
        }

        /// <summary>
        /// Info snapshots
        /// </summary>
        /// <param name="snapshots"></param>
        /// <param name="showNodeAndVm"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public static string Info(this IEnumerable<Snapshot> snapshots, bool showNodeAndVm, TableOutputType outputType)
        {
            return snapshots.Count() > 0 ?
                    TableHelper.Create(Snapshot.GetTitlesInfo(showNodeAndVm),
                                       snapshots.Select(a => a.GetRowInfo(showNodeAndVm)),
                                       outputType,
                                       false) :
                    "";
        }
    }
}