/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node Level
    /// </summary>
    public enum NodeLevel
    {
        /// <summary>
        /// Community
        /// </summary>
        Community,

        /// <summary>
        /// Basic
        /// </summary>
        Basic,

        /// <summary>
        /// Standard
        /// </summary>
        Standard,

        /// <summary>
        /// Premium
        /// </summary>
        Premium,
    }
}