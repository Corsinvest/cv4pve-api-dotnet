
using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    public class ConfigQemu : Config
    {
        internal ConfigQemu(VMInfo vm, object apiData) : base(vm, apiData)
            => JsonHelper.GetValueOrCreate(apiData, "agent", "0");

        public string GetMessageElablingAgent() => $"VM {VM.Id} consider enabling QEMU agent" +
                                                    " see https://pve.proxmox.com/wiki/Qemu-guest-agent";

        public bool AgentEnabled => ApiData.agent == "1";
        public int Sockets => ApiData.sockets;
        public string Name => ApiData.name;
    }
}