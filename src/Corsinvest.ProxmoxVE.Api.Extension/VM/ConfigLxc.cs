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

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Config LXC
    /// </summary>
    public class ConfigLxc : Config
    {
        internal ConfigLxc(VMInfo vm, object apiData) : base(vm, apiData) { }

        /// <summary>
        /// Hostname
        /// </summary>
        public string Hostname => ApiData.hostname;

        /// <summary>
        /// Name server
        /// </summary>
        public string Nameserver => ApiData.nameserver;

        /// <summary>
        /// Search domain
        /// </summary>
        public string SearchDomain => ApiData.searchdomain;

        /// <summary>
        /// Swap
        /// </summary>
        public int Swap => ApiData.swap;

        /// <summary>
        /// Arc
        /// </summary>
        public int Arch => ApiData.arch;
    }
}