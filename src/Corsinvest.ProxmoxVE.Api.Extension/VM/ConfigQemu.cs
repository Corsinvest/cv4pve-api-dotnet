
using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Config QEMU
    /// </summary>
    public class ConfigQemu : Config
    {
        internal ConfigQemu(VMInfo vm, object apiData) : base(vm, apiData)
            => JsonHelper.GetValueOrCreate(apiData, "agent", "0");

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