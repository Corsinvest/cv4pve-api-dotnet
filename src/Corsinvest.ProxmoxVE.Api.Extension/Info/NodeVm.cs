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

namespace Corsinvest.ProxmoxVE.Api.Extension.Info
{
    /// <summary>
    /// Node Vm Info
    /// </summary>
    public class NodeVm
    {
        /// <summary>
        /// Config
        /// </summary>
        /// <value></value>
        public dynamic Config { get; set; }

        /// <summary>
        /// RrdData 
        /// </summary>
        public RrdData RrdData { get; set; } = new RrdData();

        /// <summary>
        /// Snapshots
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Snapshots { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        /// <value></value>
        public dynamic Status { get; set; }

        /// <summary>
        /// Item
        /// </summary>
        /// <value></value>
        public dynamic Item { get; set; }

        /// <summary>
        /// Agent Guest Running
        /// </summary>
        /// <value></value>
        public dynamic AgentGuestRunning { get; internal set; }
    }
}