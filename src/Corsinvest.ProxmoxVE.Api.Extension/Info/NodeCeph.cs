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
    /// Node Ceph
    /// </summary>
    public class NodeCeph
    {
        /// <summary>
        /// Status
        /// </summary>
        /// <value></value>
        public dynamic Status { get; set; }

        /// <summary>
        /// Config
        /// </summary>
        /// <value></value>
        public dynamic Config { get; set; }

        /// <summary>
        /// Crush
        /// </summary>
        /// <value></value>
        public dynamic Crush { get; set; }

        // /// <summary>
        // /// Disks
        // /// </summary>
        // /// <value></value>
        // public IEnumerable<dynamic> Disks { get; set; }

        /// <summary>
        /// Flags
        /// </summary>
        /// <value></value>
        public dynamic Flags { get; set; }

        /// <summary>
        /// Mon
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Mon { get; set; }

        /// <summary>
        /// Osd
        /// </summary>
        /// <value></value>
        public dynamic Osd { get; set; }

        /// <summary>
        /// Pools
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Pools { get; set; }

        /// <summary>
        /// Rules
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Rules { get; set; }

        /// <summary>
        /// Ceph FS
        /// </summary>
        /// <value></value>
        public dynamic Fs { get; internal set; }

        /// <summary>
        /// Metadata server
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Mds { get; internal set; }

        /// <summary>
        /// Manager
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Mgr { get; internal set; }
    }
}