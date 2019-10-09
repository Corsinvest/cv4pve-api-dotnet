/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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