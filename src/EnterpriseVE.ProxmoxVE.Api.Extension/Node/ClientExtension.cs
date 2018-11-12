using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseVE.ProxmoxVE.Api;
using EnterpriseVE.ProxmoxVE.Api.Extension.Utils;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.Node
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
        public static string[] Info(this IReadOnlyList<NodeInfo> nodes)
        {
            var ret = new List<string>();
            ret.Add(NodeInfo.HeaderInfo());
            foreach (var node in nodes) { ret.Add(node.RowInfo()); }
            return ret.ToArray();
        }

        /// <summary>
        /// Return node info from id.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static NodeInfo GetNode(this Client client, string id)
        {
            var node = GetNodes(client).Where(a => a.Id == id).FirstOrDefault();
            if (node == null) { throw new ArgumentException($"Node {id} not found!"); }
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