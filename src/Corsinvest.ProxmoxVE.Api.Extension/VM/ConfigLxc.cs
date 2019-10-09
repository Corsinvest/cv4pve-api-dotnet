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