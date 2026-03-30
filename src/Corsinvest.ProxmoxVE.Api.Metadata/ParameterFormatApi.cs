/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>
/// Format parameter
/// </summary>
public class ParameterFormatApi
{
    /// <summary>
    /// Constructor
    /// </summary>
    public ParameterFormatApi(JToken token)
    {
        Name = ((JProperty)token.Parent).Name;
        Description = token["description"] + string.Empty;
        Optional = (token["optional"] ?? 0).ToString() == "1";
        Type = token["type"] + string.Empty;
        Maximum = token["maximum"] == null ? null : (int?)token["maximum"];
        Minimum = token["minimum"] == null ? null : (int?)token["minimum"];
        DefaultKey = token["default_key"] + string.Empty;
        FormatDescription = token["format_description"] + string.Empty;
        Format = token["format"] + string.Empty;
        Alias = token["alias"] + string.Empty;
        MaxLength = token["maxLength"] == null ? null : (int?)token["maxLength"];

        #region create enum values
        var enumValues = new List<string>();
        if (token["enum"] != null)
        {
            foreach (var item in token["enum"]) { enumValues.Add(item.ToString()); }
        }
        EnumValues = [.. enumValues];
        #endregion
    }

    /// <summary>
    /// Enum values
    /// </summary>
    public string[] EnumValues { get; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Type
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Comment
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Optional
    /// </summary>
    public bool Optional { get; }

    /// <summary>
    /// Minimum
    /// </summary>
    public int? Minimum { get; }

    /// <summary>
    /// Default Key
    /// </summary>
    public string DefaultKey { get; }

    /// <summary>
    /// Format description
    /// </summary>
    public string FormatDescription { get; }

    /// <summary>
    /// Format
    /// </summary>
    public string Format { get; }

    /// <summary>
    /// Alias
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Max length
    /// </summary>
    public int? MaxLength { get; }

    /// <summary>
    /// Maximum
    /// </summary>
    public int? Maximum { get; }
}