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
            => client.Cluster.Resources.GetRest("vm").ToEnumerable()
                                       .Select(a => new VMInfo(client, a))
                                       .OrderBy(a => a.Node)
                                       .ThenBy(a => a.Id)
                                       .ToArray();

        /// <summary>
        /// Get vm info from id or name.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="idOrName"></param>
        /// <returns></returns>
        public static VMInfo GetVM(this PveClient client, string idOrName)
            => GetVMs(client).Where(a => VmCheckIdOrName(a, idOrName)).FirstOrDefault() ??
                    throw new ArgumentException($"VM/CT '{idOrName}' not found!");

        /// <summary>
        /// Vm check Id or Name
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="idOrNameCheck"></param>
        /// <returns></returns>
        public static bool VmCheckIdOrName(VMInfo vm, string idOrNameCheck)
        {
            var nameLower = vm.Name.ToLower();
            var idLower = idOrNameCheck.ToLower();

            if (idOrNameCheck.Contains(":"))
            {
                //range number
                var range = idOrNameCheck.Split(':');
                if (range.Count() != 2 ||
                    !StringHelper.IsNumeric(range[0]) ||
                    !StringHelper.IsNumeric(range[1])) { return false; }

                return long.Parse(vm.Id) >= long.Parse(range[0]) &&
                        long.Parse(vm.Id) <= long.Parse(range[1]);
            }
            else if (StringHelper.IsNumeric(idOrNameCheck)) { return vm.Id == idOrNameCheck; }
            else if (idOrNameCheck.StartsWith("%") && idOrNameCheck.EndsWith("%")) { return nameLower.Contains(idLower.Replace("%", "")); }
            else if (idOrNameCheck.StartsWith("%")) { return nameLower.StartsWith(idLower.Replace("%", "")); }
            else if (idOrNameCheck.EndsWith("%")) { return nameLower.EndsWith(idLower.Replace("%", "")); }
            else { return nameLower == idLower; }
        }

        /// <summary>
        /// Get vms from jolly.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="jolly">all for all vm,
        /// <para>all-nodename all vm in host,</para>
        /// <para>vmid id vm</para>
        /// <para>Range 100:104 return vm with id in range</para>
        /// <para>start with '-' exclude vm</para>
        /// <para>comma reparated</para>
        /// </param>
        /// <returns></returns>
        public static VMInfo[] GetVMs(this PveClient client, string jolly)
        {
            var allVms = GetVMs(client);
            var ret = new List<VMInfo>();
            foreach (var id in jolly.Split(','))
            {
                if (id == "all" || id == "@all")
                {
                    //all nodes
                    ret.AddRange(allVms);
                }
                else if (id.StartsWith("all-") || id.StartsWith("@all-"))
                {
                    //all in specific node
                    var idx = id.StartsWith("all-") ? 4 : 5;
                    ret.AddRange(allVms.Where(a => a.Node.ToLower() == id.ToLower().Substring(idx)));
                }
                else if (id.StartsWith("@pool-"))
                {
                    //all in specific pool
                    var poolName = id.ToLower().Substring(6);
                    var result = client.Pools[poolName].GetRest();
                    if (result.IsSuccessStatusCode)
                    {
                        foreach (var item in result.Response.data.members)
                        {
                            if (item.type == "qemu" || item.type == "lxc")
                            {
                                ret.Add(allVms.Where(a => a.Id == item.vmid + "").SingleOrDefault());
                            }
                        }
                    }
                }
                else
                {
                    ret.AddRange(allVms.Where(a => VmCheckIdOrName(a, id)));
                }
            }

            ret = ret.Distinct().ToList();

            //exclude data
            foreach (var id in jolly.Split(',').Where(a => a.StartsWith("-")).Select(a => a.Substring(1)))
            {
                foreach (var item in allVms.Where(a => VmCheckIdOrName(a, id)))
                {
                    ret.Remove(item);
                }
            }

            return ret.ToArray();
        }
    }
}