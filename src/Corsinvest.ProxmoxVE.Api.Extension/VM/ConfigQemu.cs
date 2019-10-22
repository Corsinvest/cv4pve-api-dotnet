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