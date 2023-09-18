/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// Node Helper
    /// </summary>
    public static class NodeHelper
    {
        /// <summary>
        /// Decode level support
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static NodeLevel DecodeLevelSupport(string level)
        => level switch
        {
            "c" => NodeLevel.Community,
            "p" => NodeLevel.Premium,
            "b" => NodeLevel.Basic,
            "s" => NodeLevel.Standard,
            _ => NodeLevel.None,
        };
    }
}