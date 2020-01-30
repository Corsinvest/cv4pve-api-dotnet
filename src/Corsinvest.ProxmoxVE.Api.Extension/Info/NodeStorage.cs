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

namespace Corsinvest.ProxmoxVE.Api.Extension.Info
{
    /// <summary>
    /// Node Storage Info
    /// </summary>
    public class NodeStorage
    {
        /// <summary>
        /// Content
        /// </summary>
        /// <value></value>
        public dynamic Content { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        /// <value></value>
        public dynamic Status { get; set; }

        /// <summary>
        /// RrdData 
        /// </summary>
        public RrdData RrdData { get; set; } = new RrdData();

        /// <summary>
        /// Item
        /// </summary>
        /// <value></value>
        public dynamic Item { get; set; }
    }
}