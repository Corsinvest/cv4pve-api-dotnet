﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Backup data
/// </summary>
public class NodeStorageContent : ModelBase
{
    /// <summary>
    /// Content
    /// </summary>
    [JsonProperty("content")]
    public string Content { get; set; }

    /// <summary>
    /// Content Description
    /// </summary>
    /// <value></value>
    public string ContentDescription => FormatHelper.ContentToDescription(Content);

    /// <summary>
    /// Creation unix
    /// </summary>
    [JsonProperty("ctime")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUnixTime )]
    public long Creation { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    /// <returns></returns>
    public DateTime CreationDate => DateTimeOffset.FromUnixTimeSeconds(Creation).DateTime;

    /// <summary>
    /// Storage
    /// </summary>
    public string Storage => Volume.Split(':')[0];

    /// <summary>
    /// Filename
    /// </summary>
    public string FileName => Volume.Split(':')[1];

    /// <summary>
    /// Format
    /// </summary>
    [JsonProperty("format")]
    public string Format { get; set; }

    /// <summary>
    /// File size
    /// </summary>
    [JsonProperty("size")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Size { get; set; }

    /// <summary>
    /// Size info
    /// </summary>
    public string SizeInfo => FormatHelper.FromBytes(Size);

    /// <summary>
    /// VmId
    /// </summary>
    [JsonProperty("vmid")]
    public long VmId { get; set; }

    /// <summary>
    /// Volume Id
    /// </summary>
    [JsonProperty("volid")]
    public string Volume { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Parent
    /// </summary>
    [JsonProperty("parent")]
    public string Parent { get; set; }

    /// <summary>
    /// notes
    /// </summary>
    [JsonProperty("notes")]
    public string Notes { get; set; }

    /// <summary>
    /// excripted
    /// </summary>
    [JsonProperty("excripted")]
    public bool Encrypted { get; set; }

    /// <summary>
    /// Protected
    /// </summary>
    [JsonProperty("protected")]
    public bool Protected { get; set; }

    /// <summary>
    /// Verified
    /// </summary>
        [JsonIgnore]
    public bool Verified
        => ExtensionData != null
            && ExtensionData.TryGetValue("verification", out dynamic verification)
            && verification.state == "ok";
}