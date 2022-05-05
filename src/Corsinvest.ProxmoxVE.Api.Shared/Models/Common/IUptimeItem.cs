﻿/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// interface uptime
    /// </summary>
    public interface IUptimeItem
    {
        /// <summary>
        /// Uptime
        /// </summary>
        /// <value></value>
        [JsonProperty("uptime")]
        [DisplayFormat(DataFormatString = FormatHelper.FormatUptimeUnixTime)]
        long Uptime { get; set; }
    }
}