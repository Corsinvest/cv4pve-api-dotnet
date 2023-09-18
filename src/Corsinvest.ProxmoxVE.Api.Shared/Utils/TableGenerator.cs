/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// Table generator
    /// </summary>
    public class TableGenerator
    {
        /// <summary>
        /// Export to
        /// </summary>
        public enum Output
        {
            /// <summary>
            /// Unicode
            /// </summary>
            Text,

            /// <summary>
            /// Html
            /// </summary>
            Html,

            /// <summary>
            /// Markdown
            /// </summary>
            Markdown,

            /// <summary>
            /// Json
            /// </summary>
            Json,

            /// <summary>
            /// Json Pretty
            /// </summary>
            JsonPretty
        }

        private static readonly IEnumerable<Type> _numericTypes = new[]
        {
            typeof(int),
            typeof(double),
            typeof(decimal),
            typeof(long),
            typeof(short),
            typeof(sbyte),
            typeof(byte),
            typeof(ulong),
            typeof(ushort),
            typeof(uint),
            typeof(float)
        };

        private static void CheckData(IEnumerable<string> columns, IEnumerable<IEnumerable<object>> rows)
        {
            if (rows.Any(a => a.Count() != columns.Count()))
            {
                throw new ArgumentException($"The number columns in the row ({columns.Count()}) does not match!");
            }
        }

        private static string CreateStringFormat(IEnumerable<string> columns, IEnumerable<IEnumerable<object>> rows, char delimiter = '|')
        {
            var cols = columns.ToArray();

            var columnLengths = cols.Select((t, index) => rows.Select(r => r.ToArray()[index])
                                    .Union(new[] { cols[index] })
                                    .Where(x => x != null)
                                    .Select(x => (x + "").Length).Max())
                                    .ToList();

            var columnAlignment = Enumerable.Range(0, cols.Length)
                                            .Select(a => _numericTypes.Contains(cols[a].GetType()) ? "" : "-")
                                            .ToList();

            var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString();

            var format = (Enumerable.Range(0, cols.Length)
                                    .Select(a => " " + delimiterStr + " {" + a + "," + columnAlignment[a] + columnLengths[a] + "}")
                                    .Aggregate((s, a) => s + a) + " " + delimiterStr).Trim();
            return format;
        }

        /// <summary>
        /// Export to
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static string To(IEnumerable<string> columns, IEnumerable<IEnumerable<object>> rows, Output output)
            => output switch
            {
                Output.Text => ToText(columns, rows),
                Output.Html => ToHtml(columns, rows),
                Output.Markdown => ToMarkdown(columns, rows),
                Output.Json => ToJson(columns, rows, false),
                Output.JsonPretty => ToJson(columns, rows, true),
                _ => throw new InvalidEnumArgumentException(),
            };

        /// <summary>
        /// To Markdown
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static string ToMarkdown(IEnumerable<string> columns, IEnumerable<IEnumerable<object>> rows)
        {
            CheckData(columns, rows);

            var ret = new StringBuilder();
            var format = CreateStringFormat(columns, rows);
            var columnHeaders = string.Format(format, columns.ToArray());

            ret.AppendLine(columnHeaders);
            ret.AppendLine(Regex.Replace(columnHeaders, @"[^|]", "-"));
            rows.Select(row => string.Format(format, row.ToArray())).ToList().ForEach(row => ret.AppendLine(row));
            return ret.ToString();
        }

        /// <summary>
        /// To Text
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static string ToText(IEnumerable<string> columns, IEnumerable<IEnumerable<object>> rows)
        {
            CheckData(columns, rows);

            var ret = new StringBuilder();
            var format = CreateStringFormat(columns, rows);
            var columnHeaders = string.Format(format, columns.ToArray());
            var dividerPlus = Regex.Replace(columnHeaders, @"[^|]", "-").Replace("|", "+");

            ret.AppendLine(dividerPlus);
            ret.AppendLine(columnHeaders);
            ret.AppendLine(dividerPlus);
            rows.Select(a => string.Format(format, a.ToArray())).ToList().ForEach(a => ret.AppendLine(a));
            ret.AppendLine(dividerPlus);

            return ret.ToString();
        }

        /// <summary>
        /// To Html
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static string ToHtml(IEnumerable<string> columns, IEnumerable<IEnumerable<object>> rows)
        {
            CheckData(columns, rows);

            static string CreateRow(string tag, IEnumerable<object> values)
                => "<tr>" +
                    string.Join("", values.Select(a => $"<{tag} style='border: 1px solid black;'>{(a + "").Trim()}</{tag}>")) +
                    "</tr>";

            return "<table style='width: 100%;border-collapse: collapse;border: 1px solid black;'>" +
                    $"<thead>{CreateRow("th", columns.ToArray())}</thead>" +
                    $"<tbody>{string.Join("", rows.Select(a => CreateRow("td", a)))}</tbody>" +
                    "</table>";
        }

        /// <summary>
        /// To Json
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="pretty"></param>
        /// <returns></returns>
        public static string ToJson(IEnumerable<string> columns, IEnumerable<IEnumerable<object>> rows, bool pretty)
        {
            CheckData(columns, rows);

            var data = rows.Select(a => a.ToArray())
                           .Select(r => columns.Select((c, i) => new { col = c, row = r[i] })
                                               .ToDictionary(a => a.col, a => a.row));

            return JsonConvert.SerializeObject(data, pretty
                                                        ? Formatting.Indented
                                                        : Formatting.None);
        }
    }
}