namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    public class ConfigLxc : Config
    {
        internal ConfigLxc(VMInfo vm, object apiData) : base(vm, apiData) { }
        public string Hostname => ApiData.hostname;
        public string Nameserver => ApiData.nameserver;
        public string SearchDomain => ApiData.searchdomain;
        public int Swap => ApiData.swap;
        public int Arch => ApiData.arch;
    }
}