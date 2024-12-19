﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Disk IO
/// </summary>
public interface IDiskIO
{
    /// <summary>
    /// Disk read
    /// </summary>
    /// <value></value>
    [JsonProperty("diskread")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    long DiskRead { get; set; }

    /// <summary>
    /// Disk write
    /// </summary>
    /// <value></value>
    [JsonProperty("diskwrite")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    long DiskWrite { get; set; }
}