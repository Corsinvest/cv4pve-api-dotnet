/*
 * SPDX-FileCopyrightText: 2019 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata
{
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
            NameIndexed = Name.Replace("[n]", "");
            IsIndexed = Name.EndsWith("[n]");
            Description = token["description"] + "";
            VerboseDescription = token["verbose_description"] + "";
            Optional = (token["optional"] ?? 0).ToString() == "1";
            Type = token["type"] + "";
            TypeText = token["typetext"] + "";
            Maximum = token["maximum"] == null ? null : (long?)token["maximum"];
            Minimum = token["minimum"] == null ? null : (int?)token["minimum"];
            Renderer = token["renderer"] + "";
            Default = token["default"] == null ? null : token["default"] + "";

            if (token["properties"] != null)
            {
                Items.AddRange(token["properties"]
                                .Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))
                                .ToArray());
            }
            else if (token["items"] != null && token["items"]["properties"] != null)
            {
                Items.AddRange(token["items"]["properties"]
                                .Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))
                                .ToArray());
            }

            #region create enum values
            var enumValues = new List<string>();
            if (token["enum"] != null)
            {
                foreach (var item in token["enum"]) { enumValues.Add(item.ToString()); }
            }
            EnumValues = enumValues.ToArray();
            #endregion

            #region formats
            if (token["format"] != null)
            {
                Formats.AddRange(token["format"]
                          .Select(a => new ParameterFormatApi(a.Parent[((JProperty)a).Name]))
                          .ToArray());
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
        public List<ParameterFormatApi> Formats { get; } = new List<ParameterFormatApi>();

        /// <summary>
        /// Items
        /// </summary>
        /// <returns></returns>
        public List<ParameterApi> Items { get; } = new List<ParameterApi>();

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
                    if (double.TryParse(value.ToString(), out var perValue) && perValue > 0)
                    {
                        value = Math.Round(perValue * 100, 2) + "%";
                    }
                    else
                    {
                        value = "";
                    }
                    break;

                case "bytes":
                    if (long.TryParse(value.ToString(), out var bytesValue) && bytesValue > 0)
                    {
                        if (bytesValue > Math.Pow(1024, 4))
                        {
                            value = Math.Round(bytesValue / Math.Pow(1024, 4), 2) + " TiB";
                        }
                        else if (bytesValue > Math.Pow(1024, 3))
                        {
                            value = Math.Round(bytesValue / Math.Pow(1024, 3), 2) + " GiB";
                        }
                        else if (bytesValue > Math.Pow(1024, 2))
                        {
                            value = Math.Round(bytesValue / Math.Pow(1024, 2), 2) + " MiB";
                        }
                        else if (bytesValue > 1024)
                        {
                            value = Math.Round(bytesValue / 1024.0, 0) + " KiB";
                        }
                        else { value = bytesValue + " B"; }
                    }
                    else
                    {
                        value = "";
                    }
                    break;

                case "duration":
                    if (int.TryParse(value.ToString(), out var duration) && duration > 0)
                    {
                        value = new TimeSpan(0, 0, duration).ToString(@"d\d\ h\h\ m\m\ ss\s");
                    }
                    else
                    {
                        value = "";
                    }
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
        /// Verbose descritpion
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
}