/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// Net I/O
    /// </summary>
    public interface INetIO
    {
        /// <summary>
        /// Net in
        /// </summary>
        /// <value></value>
        [JsonProperty("netin")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        long NetIn { get; set; }

        /// <summary>
        /// Net out
        /// </summary>
        /// <value></value>
        [JsonProperty("netout")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        long NetOut { get; set; }
    }
}