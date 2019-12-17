/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using BetterConsoleTables;
using Corsinvest.ProxmoxVE.Api.Metadata;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// Table output type
    /// </summary>
    public enum TableOutputType
    {
        /// <summary>
        /// Text
        /// </summary>
        Text,

        /// <summary>
        /// Unicode
        /// </summary>
        Unicode,

        /// <summary>
        /// Unicode
        /// </summary>
        UnicodeAlt,

        /// <summary>
        /// Markdown
        /// </summary>
        Markdown,

        /// <summary>
        /// Html
        /// </summary>
        Html,
    }

    /// <summary>
    /// Table helper
    /// </summary>
    public class TableHelper
    {
        /// <summary> 
        /// Create table
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="outputType"></param>
        /// <param name="hasInnerRows"></param>
        /// <returns></returns>
        public static string Create(IEnumerable<string> columns,
                                   IEnumerable<object[]> rows,
                                   TableOutputType outputType,
                                   bool hasInnerRows)
            => Create(columns.Select(a => new ColumnHeader(a, Alignment.Left, Alignment.Left)),
                      rows,
                      outputType,
                      hasInnerRows);

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="outputType"></param>
        /// <param name="hasInnerRows"></param>
        /// <returns></returns>
        private static string Create(IEnumerable<ColumnHeader> columns,
                                   IEnumerable<object[]> rows,
                                   TableOutputType outputType,
                                   bool hasInnerRows)
        {
            var table = new Table(columns.ToArray());

            switch (outputType)
            {
                case TableOutputType.Text: table.Config = TableConfiguration.Default(); break;
                case TableOutputType.Unicode: table.Config = TableConfiguration.Unicode(); break;
                case TableOutputType.UnicodeAlt: table.Config = TableConfiguration.UnicodeAlt(); break;
                case TableOutputType.Markdown: table.Config = TableConfiguration.Markdown(); break;
                case TableOutputType.Html: table.Config = TableConfiguration.Unicode(); break;
                default: break;
            }

            table.Config.hasInnerRows = hasInnerRows;
            table.AddRows(rows);

            var ret = table.ToString();
            if (outputType == TableOutputType.Html) { ret = ToHtml(ret); }

            return ret;
        }

        private static string CreateRow(string row, string tag)
        {
            var data = row.Split('│');
            var ret = "";
            for (int i = 1; i < data.Length - 1; i++)
            {
                ret += $"<{tag} style='border: 1px solid black;'>{data[i].Trim()}</{tag}>";
            }

            return $"<tr>{ret}</tr>";
        }

        private static string ToHtml(string stringTable)
        {
            var ret = "<table style='width: 100%;border-collapse: collapse;border: 1px solid black;'>";
            var rows = stringTable.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 1; i < rows.Length; i++)
            {
                if (i == 1)
                {
                    //column header
                    ret += "<thead>" + CreateRow(rows[i], "th") + "</thead>";
                    ret += "<tbody>";
                }
                else if (rows[i].StartsWith("│"))
                {
                    ret += CreateRow(rows[i], "td") ;
                }
            }

            ret += "</tbody></table>";

            return ret;
        }

        /// <summary>
        /// Get column header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        private static ColumnHeader GetHeardColumn(string key, List<ParameterApi> returnParameters)
        {
            var alignment = Alignment.Left;

            var rp = returnParameters?.Where(a => a.Name == key).FirstOrDefault();
            if (rp != null)
            {
                switch (rp.GetAlignmentValue())
                {
                    case "R": alignment = Alignment.Right; break;
                    case "L": alignment = Alignment.Left; break;
                    default: alignment = Alignment.Left; break;
                }
            }

            return new ColumnHeader(key, alignment, alignment);
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="hasInnerRows"></param>
        /// <param name="outputType"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static string Create(dynamic data,
                                    string[] keys,
                                    bool hasInnerRows,
                                    TableOutputType outputType,
                                    List<ParameterApi> returnParameters)
        {
            var columns = new List<ColumnHeader>();
            var rows = new List<object[]>();

            if (data is ExpandoObject)
            {
                //dictionary
                var dic = (IDictionary<string, object>)data;

                columns.Add(new ColumnHeader("key", Alignment.Left, Alignment.Left));
                columns.Add(new ColumnHeader("value", Alignment.Left, Alignment.Left));

                if (keys == null)
                {
                    keys = dic.OrderBy(a => a.Key).Select(a => a.Key).ToArray();
                }

                foreach (var key in keys.OrderBy(a => a))
                {
                    if (dic.TryGetValue(key, out var value))
                    {
                        rows.Add(new object[] { key, RenderHelper.Value(value, key, returnParameters) });
                    }
                }
            }
            else if (data is IList)
            {
                if (keys == null)
                {
                    var keysTmp = new List<string>();
                    foreach (IDictionary<string, object> item in data) { keysTmp.AddRange(item.Keys.ToArray()); }
                    keys = keysTmp.Distinct().OrderBy(a => a).ToArray();
                }

                columns.AddRange(keys.Select(a => GetHeardColumn(a, returnParameters)));
                var rowsTmp = new List<KeyValuePair<object, object[]>>();

                //array data
                foreach (IDictionary<string, object> item in (IList)data)
                {
                    //create rows
                    var row = new List<object>();
                    foreach (var column in columns)
                    {
                        if (item.TryGetValue(column.Title, out var value))
                        {
                            value = RenderHelper.Value(value, column.Title, returnParameters);
                        }
                        if (value == null) { value = ""; }
                        row.Add(value);
                    }

                    rowsTmp.Add(new KeyValuePair<object, object[]>(row[0] + "", row.ToArray()));
                }

                //order row by first column
                rows.AddRange(rowsTmp.OrderBy(a => a.Key).Select(a => a.Value).ToArray());
            }

            if (rows.Count > 0)
            {
                return Create(columns, rows, outputType, hasInnerRows);
            }
            else
            {
                return data;
            }
        }
    }
}