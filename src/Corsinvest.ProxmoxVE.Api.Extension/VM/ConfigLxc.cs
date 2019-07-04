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