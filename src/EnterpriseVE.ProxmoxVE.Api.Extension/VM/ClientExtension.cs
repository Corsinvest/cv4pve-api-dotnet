using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseVE.ProxmoxVE.Api;
using EnterpriseVE.ProxmoxVE.Api.Extension.Utils;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Client extension for vm
    /// </summary>
    public static class ClientExtension
    {
        /// <summary>
        /// Get all vms info.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static VMInfo[] GetVMs(this Client client)
        {
            var vms = new List<VMInfo>();
            foreach (var vm in client.Cluster.Resources.GetRest("vm").Response.data)
            {
                vms.Add(new VMInfo(client, vm));
            }
            return vms.OrderBy(a => a.Node).ThenBy(a => a.Id).ToArray();
        }

        /// <summary>
        /// Get vm info from id or name.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="idOrName"></param>
        /// <returns></returns>
        public static VMInfo GetVM(this Client client, string idOrName)
        {
            var vm = GetVMs(client).Where(a => CheckIdOrName(a, idOrName)).FirstOrDefault();
            if (vm == null) { throw new ArgumentException($"VM/CT {idOrName} not found!"); }
            return vm;
        }

        private static bool CheckIdOrName(VMInfo vm, string id)
        {
            return (StringHelper.IsNumeric(id) ? vm.Id == id : vm.Name == id);
        }

        /// <summary>
        /// Get vms from jolly.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="jolly">all for all vm, 
        /// <para><para>all-nodename all vm in host, 
        /// <para><para>vmid id vm
        /// <para><para>comma reparated</param>
        /// <returns></returns>
        public static VMInfo[] GetVMs(this Client client, string jolly)
        {
            var vms = new List<VMInfo>();
            var allVms = GetVMs(client);

            foreach (var id in jolly.Split(','))
            {
                if (id == "all")
                {
                    //all nodes
                    vms.AddRange(allVms);
                }
                else if (id.StartsWith("all-"))
                {
                    //all in specific node
                    vms.AddRange(allVms.Where(a => a.Node == id.Substring(4)));
                }
                else
                {
                    //specific id
                    var vm = allVms.Where(a => CheckIdOrName(a, id)).FirstOrDefault();
                    if (vm != null) { vms.Add(vm); }
                }
            }

            return vms.Distinct().ToArray();
        }
    }
}