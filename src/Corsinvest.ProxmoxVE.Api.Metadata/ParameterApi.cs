/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Dynamic;

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>
/// Parameter Api
/// </summary>
public class ParameterApi
{
    /// <summary>Constructor from flat cache</summary>
    /// <param name="flat">Flat cache parameter info</param>
    internal ParameterApi(FlatParamInfo flat)
    {
        Name = flat.Name;
        NameIndexed = flat.Name.Replace("[n]", string.Empty);
        IsIndexed = flat.Name.EndsWith("[n]");
        Type = flat.Type ?? string.Empty;
        TypeText = flat.TypeText ?? string.Empty;
        Description = flat.Description ?? string.Empty;
        Optional = flat.Optional ?? false;
        Default = flat.Default ?? string.Empty;
        Minimum = flat.Minimum.HasValue ? flat.Minimum.Value : null;
        Maximum = flat.Maximum;
        EnumValues = flat.EnumValues ?? [];
    }

    /// <summary>Constructor from JSON token</summary>
    /// <param name="token">JSON token representing the parameter</param>
    public ParameterApi(JToken token)
    {
        Name = ((JProperty)token.Parent).Name;
        NameIndexed = Name.Replace("[n]", string.Empty);
        IsIndexed = Name.EndsWith("[n]");
        Description = token["description"] + string.Empty;
        VerboseDescription = token["verbose_description"] + string.Empty;
        Optional = (token["optional"] ?? 0).ToString() == "1";
        Type = token["type"] + string.Empty;
        TypeText = token["typetext"] + string.Empty;
        Maximum = token["maximum"] == null ? null : (long?)token["maximum"];
        Minimum = token["minimum"] == null ? null : (int?)token["minimum"];
        Renderer = token["renderer"] + string.Empty;
        Default = token["default"] == null ? null : token["default"] + string.Empty;

        if (token["properties"] != null)
        {
            Items.AddRange([.. token["properties"].Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))]);
        }
        else if (token["items"]?["properties"] != null)
        {
            Items.AddRange([.. token["items"]["properties"].Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))]);
        }

        #region create enum values
        var enumValues = new List<string>();
        if (token["enum"] != null)
        {
            foreach (var item in token["enum"]) { enumValues.Add(item.ToString()); }
        }
        EnumValues = [.. enumValues];
        #endregion

        #region formats
        // PVE format is typically a string (like "pve-configid"), but check for JObject with properties just in case
        if (token["format"] is JObject formatObj && formatObj["properties"] is JObject formatProperties)
        {
            Formats.AddRange([.. ((IEnumerable<JToken>)formatProperties).Select(a => new ParameterFormatApi(a.Parent[((JProperty)a).Name]))]);
        }
        #endregion
    }

    /// <summary>
    /// Name Indexed
    /// </summary>
    public string NameIndexed { get; }

    /// <summary>
    /// Enum values
    /// </summary>
    public string[] EnumValues { get; }

    /// <summary>
    /// Parameters
    /// </summary>
    public List<ParameterFormatApi> Formats { get; } = [];

    /// <summary>
    /// Items
    /// </summary>
    public List<ParameterApi> Items { get; } = [];

    /// <summary>
    /// Get alignment value
    /// </summary>
    public string GetAlignmentValue()
        => Renderer switch
        {
            "fraction_as_percentage" => "R",
            "bytes" => "R",
            "duration" => "R",
            "timestamp" => "R",
            "timestamp_gmt" => "R",
            _ => "L",
        };

    /// <summary>
    /// Renderer value.
    /// </summary>
    public object RendererValue(object value)
    {
        switch (Renderer)
        {
            case "fraction_as_percentage":
                value = double.TryParse(value.ToString(), out var perValue) && perValue > 0
                        ? Math.Round(perValue * 100, 2) + "%"
                        : string.Empty;
                break;

            case "bytes":
                if (value != null && long.TryParse(value.ToString(), out var bytesValue) && bytesValue > 0)
                {
                    var sizes = new string[] { "B", "KiB", "MiB", "GiB", "TiB" };
                    var order = 0;
                    while (bytesValue >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        bytesValue /= 1024;
                    }
                    value = $"{bytesValue} {sizes[order]}";
                }
                else
                {
                    value = string.Empty;
                }
                break;

            case "duration":
                value = int.TryParse(value.ToString(), out var duration) && duration > 0
                            ? new TimeSpan(0, 0, duration).ToString(@"d\d\ h\h\ m\m\ ss\s")
                            : string.Empty;
                break;

            case "timestamp":
                value = long.TryParse(value?.ToString(), out var ts) && ts > 0
                            ? DateTimeOffset.FromUnixTimeSeconds(ts).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                            : string.Empty;
                break;

            case "timestamp_gmt":
                value = long.TryParse(value?.ToString(), out var tsGmt) && tsGmt > 0
                            ? DateTimeOffset.FromUnixTimeSeconds(tsGmt).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                            : string.Empty;
                break;

            default:
                if (value is ExpandoObject || value is IList)
                {
                    value = JsonConvert.SerializeObject(value);
                }
                break;
        }

        return value;
    }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Type
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Type text
    /// </summary>
    public string TypeText { get; }

    /// <summary>
    /// Comment
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Verbose description
    /// </summary>
    public string VerboseDescription { get; }

    /// <summary>
    /// Optional
    /// </summary>
    public bool Optional { get; }

    /// <summary>
    /// Is Indexed
    /// </summary>
    public bool IsIndexed { get; }

    /// <summary>
    /// Minimum
    /// </summary>
    public int? Minimum { get; }

    /// <summary>
    /// Render
    /// </summary>
    public string Renderer { get; }

    /// <summary>
    /// Default
    /// </summary>
    public string Default { get; }

    /// <summary>
    /// Maximum
    /// </summary>
    public long? Maximum { get; }
}