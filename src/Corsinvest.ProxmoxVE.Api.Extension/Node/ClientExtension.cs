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
            var table = new Table(TableConfiguration.Unicode());
            table.AddColumns(NodeInfo.GetTitlesInfo());
            foreach (var node in nodes) { table.AddRow(node.GetRowInfo()); }
            return table.ToString();
        }

        /// <summary>
        /// Return node info from id.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NodeInfo GetNode(this Client client, string name)
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
        public static IReadOnlyList<NodeInfo> GetNodes(this Client client)
        {
            var nodes = new List<NodeInfo>();
            foreach (var node in client.Nodes.GetRest().Response.data) { nodes.Add(new NodeInfo(client, node)); }
            return nodes.OrderBy(a => a.Node).ToList().AsReadOnly();
        }
    }
}