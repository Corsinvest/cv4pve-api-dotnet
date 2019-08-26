using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using BetterConsoleTables;
using Corsinvest.ProxmoxVE.Api.Metadata;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils.Shell
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
            var config = TableConfiguration.Unicode();
            config.hasInnerRows = hasInnerRows;
            var table = new Table(config);

            table.AddColumns(columns);
            table.AddRows(rows);

            return table;
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CreateTable(dynamic data)
        {
            if (data is ExpandoObject)
            {
                return CreateTable(data,
                                   ((IDictionary<string, object>)data).OrderBy(a => a.Key)
                                                                      .Select(a => a.Key)
                                                                      .ToArray(),
                                    null);
            }
            else if (data is IList)
            {
                //array data
                var keys = new List<string>();
                foreach (IDictionary<string, object> item in data) { keys.AddRange(item.Keys.ToArray()); }
                return CreateTable(data, keys.Distinct().OrderBy(a => a).ToArray(), null);
            }
            else
            {
                return data;
            }
        }

        private static object RendererValue(object value, string key, List<ParameterApi> returnParameters)
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
        /// Create table
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        public static string CreateTable(dynamic data, string[] keys, List<ParameterApi> returnParameters)
        {
            var config = TableConfiguration.Unicode();
            config.hasInnerRows = true;
            var table = new Table(config);

            if (data is ExpandoObject)
            {
                //dictionary
                var dic = (IDictionary<string, object>)data;
                table.AddColumns(new string[] { "key", "value" });

                foreach (var key in keys.OrderBy(a => a))
                {
                    if (dic.TryGetValue(key, out var value))
                    {
                        table.AddRow(new object[] { key, RendererValue(value, key, returnParameters) });
                    }
                }
            }
            else if (data is IList)
            {
                table.AddColumns(keys);

                var rows = new List<KeyValuePair<object, object[]>>();

                //array data
                foreach (IDictionary<string, object> item in (IList)data)
                {
                    //create rows
                    var row = new List<object>();
                    foreach (string key in table.Columns)
                    {
                        if (item.TryGetValue(key, out var value)) { value = RendererValue(value, key, returnParameters); }
                        if (value == null) { value = ""; }
                        row.Add(value);
                    }

                    rows.Add(new KeyValuePair<object, object[]>(row[0] + "", row.ToArray()));
                }

                //order row by first column
                table.AddRows(rows.OrderBy(a => a.Key).Select(a => a.Value).ToArray());
            }
            else
            {
                return data;
            }

            return table.ToString();
        }
    }
}