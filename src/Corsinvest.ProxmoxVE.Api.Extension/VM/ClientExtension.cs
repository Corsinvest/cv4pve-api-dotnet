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

using System;
using System.Collections.Generic;
using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
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
        public static VMInfo[] GetVMs(this PveClient client)
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
        public static VMInfo GetVM(this PveClient client, string idOrName)
        {
            var vm = GetVMs(client).Where(a => CheckIdOrName(a, idOrName)).FirstOrDefault();
            if (vm == null) { throw new ArgumentException($"VM/CT {idOrName} not found!"); }
            return vm;
        }

        private static bool CheckIdOrName(VMInfo vm, string id)
        {
            var nameLower = vm.Name.ToLower();
            var idLower = id.ToLower();

            if (StringHelper.IsNumeric(id))
            {
                //numeric
                return vm.Id == id;
            }
            else if (id.StartsWith("%") && id.EndsWith("%"))
            {
                //contain
                return nameLower.Contains(idLower.Replace("%", ""));
            }
            else if (id.StartsWith("%"))
            {
                //startwith
                return nameLower.StartsWith(idLower.Replace("%", ""));
            }
            else if (id.EndsWith("%"))
            {
                //endwith
                return nameLower.EndsWith(idLower.Replace("%", ""));
            }
            else
            {
                return nameLower == idLower;
            }
        }

        /// <summary>
        /// Get vms from jolly.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="jolly">all for all vm, 
        /// <para>all-nodename all vm in host,</para>
        /// <para>vmid id vm</para>
        /// <para>start with '-' exclude vm</para>
        /// <para>comma reparated</para></param>
        /// <returns></returns>
        public static VMInfo[] GetVMs(this PveClient client, string jolly)
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
                    vms.AddRange(allVms.Where(a => a.Node.ToLower() == id.ToLower().Substring(4)));
                }
                else
                {
                    //specific id
                    var vm = allVms.Where(a => CheckIdOrName(a, id)).FirstOrDefault();
                    if (vm != null) { vms.Add(vm); }
                }
            }

            //exclude data 
            foreach (var id in jolly.Split(',').Where(a => a.StartsWith("-")).Select(a => a.Substring(1)))
            {
                var vm = allVms.Where(a => CheckIdOrName(a, id)).FirstOrDefault();
                if (vm != null) { vms.Remove(vm); }
            }

            return vms.Distinct().ToArray();
        }
    }
}