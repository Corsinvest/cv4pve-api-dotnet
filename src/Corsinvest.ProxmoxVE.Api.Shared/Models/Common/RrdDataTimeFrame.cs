/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// Rrd time frame
    /// </summary>
    public enum RrdDataTimeFrame
    {
        /// <summary>
        /// Hour
        /// </summary>
        Hour,

        /// <summary>
        /// Day
        /// </summary>
        Day,

        /// <summary>
        /// Week
        /// </summary>
        Week,

        /// <summary>
        /// Month
        /// </summary>
        Month,

        /// <summary>
        /// Year
        /// </summary>
        Year
    }
}