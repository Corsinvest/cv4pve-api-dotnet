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

using System.Collections.Generic;
using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Helpers;

namespace Corsinvest.ProxmoxVE.Api.Extension.Node
{
    /// <summary>
    /// Extension client for node.
    /// </summary>
    public static class NodeExtension
    {
        /// <summary>
        /// Info nodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public static string Info(this IReadOnlyList<NodeInfo> nodes, TableOutputType outputType)
        {
            return nodes.Count > 0 ?
                    TableHelper.Create(NodeInfo.GetTitlesInfo(),
                                       nodes.Select(a => a.GetRowInfo()),
                                       outputType,
                                       false) :
                    "";
        }
    }
}