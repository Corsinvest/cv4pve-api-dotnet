/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Collections;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        Default = flat.Default;
        Minimum = flat.Minimum;
        Maximum = flat.Maximum;
        EnumValues = flat.EnumValues ?? [];
        Renderer = flat.Renderer ?? string.Empty;
        VerboseDescription = flat.VerboseDescription ?? string.Empty;
        if (flat.Formats != null) { Formats.AddRange(flat.Formats.Select(a => new ParameterFormatApi(a))); }
        if (flat.Items != null) { Items.AddRange(flat.Items.Select(a => new ParameterApi(a))); }
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
    /// <remarks>As pvesh renders them: sizes with two decimals (<c>1.50 KiB</c>), percentages (<c>4.44%</c>),
    /// durations without the leading empty units (<c>1h 0m 5s</c>), dates as <c>yyyy-MM-dd HH:mm:ss</c> (local, or
    /// UTC for <c>timestamp_gmt</c>; 0 is no date). A value that is not a number stays as it is.</remarks>
    public object RendererValue(object value)
    {
        var invariant = System.Globalization.CultureInfo.InvariantCulture;

        // A number as the API returns it (long, double) or as text in invariant format: never through the culture.
        bool TryNumber(out double number)
        {
            if (value is not string and IConvertible convertible and not bool)
            {
                try
                {
                    number = convertible.ToDouble(invariant);
                    return true;
                }
                catch (FormatException) { }
                catch (InvalidCastException) { }
            }

            return double.TryParse(value + string.Empty, System.Globalization.NumberStyles.Float, invariant, out number);
        }

        switch (Renderer)
        {
            case "fraction_as_percentage":
                if (TryNumber(out var fraction))
                {
                    value = Math.Round(fraction * 100, 2).ToString(invariant) + "%";
                }
                break;

            case "bytes":
                if (TryNumber(out var bytes) && bytes >= 0)
                {
                    var sizes = new[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB" };
                    var order = 0;
                    while (bytes >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        bytes /= 1024;
                    }
                    value = order == 0 ? $"{bytes.ToString(invariant)} B" : $"{bytes.ToString("F2", invariant)} {sizes[order]}";
                }
                break;

            case "duration":
                if (TryNumber(out var seconds) && seconds >= 0)
                {
                    var time = TimeSpan.FromSeconds(Math.Floor(seconds));
                    value = time.Days > 0
                            ? $"{time.Days}d {time.Hours}h {time.Minutes}m {time.Seconds}s"
                            : time.Hours > 0
                                ? $"{time.Hours}h {time.Minutes}m {time.Seconds}s"
                                : time.Minutes > 0 ? $"{time.Minutes}m {time.Seconds}s" : $"{time.Seconds}s";
                }
                break;

            case "timestamp":
            case "timestamp_gmt":
                if (TryNumber(out var unixValue))
                {
                    var unix = (long)unixValue;
                    var date = DateTimeOffset.FromUnixTimeSeconds(unix);
                    value = unix <= 0
                            ? string.Empty
                            : (Renderer == "timestamp" ? date.ToLocalTime() : date.ToUniversalTime()).ToString("yyyy-MM-dd HH:mm:ss", invariant);
                }
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
