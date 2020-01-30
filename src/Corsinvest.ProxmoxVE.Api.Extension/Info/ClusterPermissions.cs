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
    /// Access Info
    /// </summary>
    public class ClusterPermissions
    {
        /// <summary>
        /// Users
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Users { get; set; }

        /// <summary>
        /// Groups
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Groups { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Roles { get; set; }

        /// <summary>
        /// Acl
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Acl { get; set; }

        /// <summary>
        /// Domains
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Domains { get; set; }
    }
}