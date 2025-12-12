/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
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
    /// <summary>
    /// Parameter Api
    /// </summary>
    /// <param name="token"></param>
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
        if (token["format"] != null)
        {
            Formats.AddRange([.. token["format"].Select(a => new ParameterFormatApi(a.Parent[((JProperty)a).Name]))]);
        }
        #endregion
    }

    /// <summary>
    /// Name Indexed
    /// </summary>
    /// <returns></returns>
    public string NameIndexed { get; }

    /// <summary>
    /// Enum values
    /// </summary>
    /// <value></value>
    public string[] EnumValues { get; }

    /// <summary>
    /// Parameters
    /// </summary>
    /// <returns></returns>
    public List<ParameterFormatApi> Formats { get; } = [];

    /// <summary>
    /// Items
    /// </summary>
    /// <returns></returns>
    public List<ParameterApi> Items { get; } = [];

    /// <summary>
    /// Get alignment value
    /// </summary>
    /// <returns></returns>
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
    /// <param name="value"></param>
    /// <returns></returns>
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

            case "timestamp": break;
            case "timestamp_gmt": break;

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
    /// <value></value>
    public string Name { get; }

    /// <summary>
    /// Type
    /// </summary>
    /// <value></value>
    public string Type { get; }

    /// <summary>
    /// Type text
    /// </summary>
    /// <value></value>
    public string TypeText { get; }

    /// <summary>
    /// Comment
    /// </summary>
    /// <value></value>
    public string Description { get; }

    /// <summary>
    /// Verbose description
    /// </summary>
    /// <value></value>
    public string VerboseDescription { get; }

    /// <summary>
    /// Optional
    /// </summary>
    /// <value></value>
    public bool Optional { get; }

    /// <summary>
    /// Is Indexed
    /// </summary>
    /// <returns></returns>
    public bool IsIndexed { get; }

    /// <summary>
    /// Minimum
    /// </summary>
    /// <value></value>
    public int? Minimum { get; }

    /// <summary>
    /// Render
    /// </summary>
    /// <value></value>
    public string Renderer { get; }

    /// <summary>
    /// Default
    /// </summary>
    /// <value></value>
    public string Default { get; }

    /// <summary>
    /// Maximum
    /// </summary>
    /// <value></value>
    public long? Maximum { get; }
}