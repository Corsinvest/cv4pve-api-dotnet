/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using BetterConsoleTables;
using Corsinvest.ProxmoxVE.Api.Metadata;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers.Shell
{
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
        /// <param name="hasInnerRows"></param>
        /// <returns></returns>
        public static Table CreateTable(string[] columns, IEnumerable<object[]> rows, bool hasInnerRows = true)
        {
            var table = new Table();
            table.Config = TableConfiguration.Unicode();
            table.Config.hasInnerRows = hasInnerRows;

            table.AddColumns(Alignment.Left, Alignment.Left, columns);
            table.AddRows(rows);

            return table;
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="data"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static string CreateTable(dynamic data, List<ParameterApi> returnParameters = null)
        {
            if (data is ExpandoObject)
            {
                return CreateTable(data,
                                   ((IDictionary<string, object>)data).OrderBy(a => a.Key)
                                                                      .Select(a => a.Key)
                                                                      .ToArray(),
                                    returnParameters);
            }
            else if (data is IList)
            {
                //array data
                var keys = new List<string>();
                foreach (IDictionary<string, object> item in data) { keys.AddRange(item.Keys.ToArray()); }
                return CreateTable(data, keys.Distinct().OrderBy(a => a).ToArray(), returnParameters);
            }
            else
            {
                return data;
            }
        }

        /// <summary>
        /// Render value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static object RendererValue(object value, string key, List<ParameterApi> returnParameters)
        {
            if (returnParameters == null)
            {
                return (value is ExpandoObject || value is IList) ?
                        Newtonsoft.Json.JsonConvert.SerializeObject(value) :
                        value;
            }
            else
            {
                return returnParameters.Where(a => a.Name == key).FirstOrDefault().RendererValue(value);
            }
        }

        /// <summary>
        /// Get column header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static ColumnHeader GetHeardColumn(string key, List<ParameterApi> returnParameters)
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
        /// To table form data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static Table ToTable(dynamic data, List<ParameterApi> returnParameters = null)
        {
            if (data is ExpandoObject)
            {
                return ToTable(data,
                               ((IDictionary<string, object>)data).OrderBy(a => a.Key)
                                                                  .Select(a => a.Key)
                                                                  .ToArray(),
                               returnParameters);
            }
            else if (data is IList)
            {
                //array data
                var keys = new List<string>();
                foreach (IDictionary<string, object> item in data) { keys.AddRange(item.Keys.ToArray()); }
                return ToTable(data, keys.Distinct().OrderBy(a => a).ToArray(), returnParameters);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// To table data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static Table ToTable(dynamic data, string[] keys, List<ParameterApi> returnParameters)
        {
            if (data is ExpandoObject)
            {
                var table = new Table();
                table.AddColumns(Alignment.Left, Alignment.Left, new[] { "key", "value" });

                //dictionary
                var dic = (IDictionary<string, object>)data;

                foreach (var key in keys.OrderBy(a => a))
                {
                    if (dic.TryGetValue(key, out var value))
                    {
                        table.AddRow(new object[] { key, RendererValue(value, key, returnParameters) });
                    }
                }

                return table.Rows.Count == 0 ? null : table;
            }
            else if (data is IList)
            {
                var table = new Table(keys.Select(a => GetHeardColumn(a, returnParameters)).ToArray());
                var rows = new List<KeyValuePair<object, object[]>>();

                //array data
                foreach (IDictionary<string, object> item in (IList)data)
                {
                    //create rows
                    var row = new List<object>();
                    foreach (var column in table.Columns)
                    {
                        if (item.TryGetValue(column.Title, out var value))
                        {
                            value = RendererValue(value, column.Title, returnParameters);
                        }
                        if (value == null) { value = ""; }
                        row.Add(value);
                    }

                    rows.Add(new KeyValuePair<object, object[]>(row[0] + "", row.ToArray()));
                }

                //order row by first column
                table.AddRows(rows.OrderBy(a => a.Key).Select(a => a.Value).ToArray());

                return table.Rows.Count == 0 ? null : table;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static string CreateTable(dynamic data, string[] keys, List<ParameterApi> returnParameters)
        {
            var table = (Table)ToTable(data, keys, returnParameters);
            if (table == null)
            {
                return data;
            }
            else
            {
                table.Config = TableConfiguration.Unicode();
                table.Config.hasInnerRows = true;
                return table.ToString();
            }
        }
    }
}