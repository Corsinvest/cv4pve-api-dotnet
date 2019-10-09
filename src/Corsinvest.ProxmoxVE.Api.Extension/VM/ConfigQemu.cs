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
 
using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Config QEMU
    /// </summary>
    public class ConfigQemu : Config
    {
        internal ConfigQemu(VMInfo vm, object apiData) : base(vm, apiData)
            => DynamicHelper.CheckKeyOrCreate(apiData, "agent", "0");

        /// <summary>
        /// Message enabling agent.
        /// </summary>
        /// <returns></returns>
        public string GetMessageEnablingAgent() => $"VM {VM.Id} consider enabling QEMU agent" +
                                                    " see https://pve.proxmox.com/wiki/Qemu-guest-agent";

        /// <summary>
        /// Agent
        /// </summary>
        public string Agent => ApiData.agent;

        /// <summary>
        /// Agent enabled.
        /// </summary>
        public bool AgentEnabled => Agent.Split(',').Select(a => a.Trim()).Any(a => a == "1");

        /// <summary>
        /// Sockets.
        /// </summary>
        public int Sockets => ApiData.sockets;

        /// <summary>
        /// Name.
        /// </summary>
        public string Name => ApiData.name;
    }
}