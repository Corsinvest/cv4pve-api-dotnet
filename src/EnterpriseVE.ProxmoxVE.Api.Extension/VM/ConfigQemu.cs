
using EnterpriseVE.ProxmoxVE.Api.Extension.Utils;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    public class ConfigQemu : Config
    {
        internal ConfigQemu(VMInfo vm, object apiData) : base(vm, apiData)
        {
            AttributeHelper.NotExistCreate(apiData, "agent", 0);
        }

        public bool Agent => ApiData.agent == 1;
        public int Sockets => ApiData.sockets;
        public string Name => ApiData.name;
   }
}