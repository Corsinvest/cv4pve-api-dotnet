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
using BetterConsoleTables;

namespace Corsinvest.ProxmoxVE.Api.Extension.Node
{
    /// <summary>
    /// Extension client for node.
    /// </summary>
    public static class ClientExtension
    {
        /// <summary>
        /// Info nodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static string Info(this IReadOnlyList<NodeInfo> nodes)
        {
            if (nodes.Count > 0)
            {
                var table = new Table(TableConfiguration.Unicode());
                table.AddColumns(Alignment.Left, Alignment.Left, NodeInfo.GetTitlesInfo());
                foreach (var node in nodes) { table.AddRow(node.GetRowInfo()); }
                return table.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Return node info from id.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NodeInfo GetNode(this PveClient client, string name)
        {
            var node = GetNodes(client).Where(a => a.Node == name).FirstOrDefault();
            if (node == null) { throw new ArgumentException($"Node {name} not found!"); }
            return node;
        }

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