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

namespace Corsinvest.ProxmoxVE.Api.Extension.Node
{
    /// <summary>
    /// Extension client for node.
    /// </summary>
    public static class ClientExtension
    {
        /// <summary>
        /// Return node info from id.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NodeInfo GetNode(this PveClient client, string name)
            => GetNodes(client).Where(a => a.Node == name).FirstOrDefault() ??
                        throw new ArgumentException($"Node '{name}' not found!");

        /// <summary>
        /// Return all nodes info.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static IReadOnlyList<NodeInfo> GetNodes(this PveClient client)
        {
            var nodes = new List<NodeInfo>();
            foreach (var node in client.Nodes.GetRest().Response.data) { nodes.Add(new NodeInfo(client, node)); }
            return nodes.OrderBy(a => a.Node).ToList().AsReadOnly();
        }
    }
}