/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Metadata;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    /// <summary>
    /// Api Explorer
    /// </summary>
    public static class ApiExplorerHelper
    {
        /// <summary>
        /// Alias command.
        /// </summary>
        public class AliasDef
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="name"></param>
            /// <param name="description"></param>
            /// <param name="command"></param>
            /// <param name="system"></param>
            public AliasDef(string name, string description, string command, bool system)
            {
                Name = name;
                Description = description;
                Command = command;
                System = system;
            }

            /// <summary>
            /// Name
            /// </summary>
            /// <value></value>
            public string Name { get; }

            /// <summary>
            /// Description
            /// </summary>
            /// <value></value>
            public string Description { get; }

            /// <summary>
            /// Command
            /// </summary>
            /// <value></value>
            public string Command { get; }

            /// <summary>
            /// System
            /// </summary>
            /// <value></value>
            public bool System { get; }

            /// <summary>
            /// Check exists name or alias
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool Exists(string name) => name.Split(',').Any(a => Names.Contains(a));

            /// <summary>
            /// Name alias
            /// </summary>
            /// <returns></returns>
            public string[] Names => Name.Split(',');

            /// <summary>
            /// Check name is valid
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static bool IsValid(string name) => new Regex("^[a-zA-Z0-9,_-]*$").IsMatch(name);
        }

        /// <summary>
        /// Alias manager
        /// </summary>
        public class AliasManager
        {
            private readonly List<AliasDef> _alias = new()
            {
                //cluster
                new("cluster-top,ct,top,❤️", "Cluster top", "get /cluster/resources", true),
                new("cluster-top-node,ctn,topn", "Cluster top for node", "get /cluster/resources type:node", true),
                new("cluster-top-storage,cts,tops", "Cluster top for storage", "get /cluster/resources type:storage", true),
                new("cluster-top-vm,ctv,topv", "Cluster top for VM/CT", "get /cluster/resources type:vm", true),
                new("cluster-status,csts", "Cluster status", "get /cluster/ha/status/current", true),
                new("cluster-replication,crep", "Cluster replication", "get /cluster/replication", true),
                new("cluster-backup,cbck", "Cluster list vzdump backup schedule", "get /cluster/backup", true),
                new("cluster-backup-info,cbckinf", "Cluster info backup schedule", "get /cluster/backup/{backup}", true),

                //node
                new("nodes-list,nlst", "Node services", "get /nodes", true),
                new("node-status,nsts", "Node status", "get /nodes/{node}/status", true),
                new("node-services,nsvc", "Node services", "get /nodes/{node}/services", true),
                new("node-tasks-active,ntact", "Node tasks active", "get /nodes/{node}/tasks source:active", true),
                new("node-tasks-error,nterr", "Node tasks errors", "get /nodes/{node}/tasks errors:1", true),
                new("node-disks-list,ndlst", "Node discks list", "get /nodes/{node}/disks/list", true),
                new("node-version,nver", "Node version", "get /nodes/{node}/version", true),
                new("node-storage,nsto", "Node storage info", "get /nodes/{node}/storage", true),
                new("node-storage-content,nstoc", "Node storage content", "get /nodes/{node}/storage/{storage}/content", true),
                new("node-report,nrpt", "Node report", "get /nodes/{node}/report", true),
                new("node-shutdown,nreb", "Node reboot or shutdown", "create /nodes/{node}/status command:cmd", true),
                new("node-vzdump-list,nvlst", "Node list backup", "/get /nodes/{node}/storage/{storage}/content vmid:{vmid} content:backup", true),
                new("node-vzdump-config,nvcfg", "Node Extract configuration from vzdump backup archive",
                             "get /nodes/{node}/vzdump/extractconfig volume:{volume}", true),

                //Qemu
                new("qemu-list,qlst", "Qemu list vm", "get /nodes/{node}/qemu", true),
                new("qemu-exec,qexe", "Qemu exec command vm", "create /nodes/{node}/qemu/{vmid}/agent/exec command:{command}", true),
                new("qemu-migrate,qmig", "Qemu migrate vm other node", " get /nodes/{node}/qemu/{vmid}/migrate target:{target}", true),
                new("qemu-vzdump-restore,qvrst", "Qemu restore vzdump", " create /nodes/{node}/qemu vmid:{vmid} archive:{archive}", true),

                //status
                new("qemu-status,qsts", "Qemu current status vm", "get /nodes/{node}/qemu/{vmid}/status/current", true),
                new("qemu-start,qstr", "Qemu start vm", "create /nodes/{node}/qemu/{vmid}/status/start", true),
                new("qemu-stop,qsto", "Qemu stop vm", "create /nodes/{node}/qemu/{vmid}/status/stop", true),
                new("qemu-shutdown,qsdwn", "Qemu shutdown vm", "create /nodes/{node}/qemu/{vmid}/status/shutdown", true),
                new("qemu-config,qcfg", "Qemu config vm", "get /nodes/{node}/qemu/{vmid}/config", true),

                //snapshot
                new("qemu-snap-list,qslst", "Qemu snapshot vm list", "get /nodes/{node}/qemu/{vmid}/snapshot", true),
                new("qemu-snap-create,qscrt", "Qemu snapshot vm create", "create /nodes/{node}/qemu/{vmid}/snapshot snapname:{snapname} description:{description}",
                             true),
                new("qemu-snap-delete,qsdel", "Qemu snapshot vm delete", "delete /nodes/{node}/qemu/{vmid}/snapshot/{snapname}", true),
                new("qemu-snap-config,qscfg", "Qemu snapshot vm delete", "get /nodes/{node}/qemu/{vmid}/snapshot/{snapname}/config", true),
                new("qemu-snap-rollback,qsrbck", "Qemu snapshot vm rollback", "create /nodes/{node}/qemu/{vmid}/snapshot/{snapname}/rollback", true),

                //LXC
                new("lxc-list,llst", "LXC list vm", "get /nodes/{node}/lxc", true),
                new("lxc-migrate,lmig", "LXC migrate vm other node", "get /nodes/{node}/lxc/{vmid}/migrate target:{target}", true),
                new("lxc-vzdump-restore,lvrst", "LXC restore vzdump", "create /nodes/{node}/lxc vmid:{vmid} ostemplate:{archive} restore:1", true),

                //status
                new("lxc-status,lsts", "LXC current status vm", "get /nodes/{node}/lxc/{vmid}/status/current", true),
                new("lxc-start,lstr", "LXC start vm", "create /nodes/{node}/lxc/{vmid}/status/start", true),
                new("lxc-stop,lsto", "LXC stop vm", "create /nodes/{node}/lxc/{vmid}/status/stop", true),
                new("lxc-shutdown,lsdwn", "LXC shutdown vm", "create /nodes/{node}/lxc/{vmid}/status/shutdown", true),
                new("lxc-config,lcfg", "LXC config vm", "get /nodes/{node}/lxc/{vmid}/config", true),

                //snapshot
                new("lxc-snap-list,lslst", "LXC snapshot vm list", "get /nodes/{node}/lxc/{vmid}/snapshot", true),
                new("lxc-snap-create,lscrt", "LXC snapshot vm create", "create /nodes/{node}/lxc/{vmid}/snapshot snapname:{snapname} description:{description}", true),
                new("lxc-snap-delete,lsdel", "LXC snapshot vm delete", "delete /nodes/{node}/lxc/{vmid}/snapshot/{snapname}", true),
                new("lxc-snap-config,lscfg", "LXC snapshot vm delete", "get /nodes/{node}/lxc/{vmid}/snapshot/{snapname}/config", true),
                new("lxc-snap-rollback,lsrbck", "LXC snapshot vm rollback", "create /nodes/{node}/lxc/{vmid}/snapshot/{snapname}/rollback", true),
            };

            /// <summary>
            /// Alias
            /// </summary>
            /// <returns></returns>
            public ReadOnlyCollection<AliasDef> Alias => _alias.AsReadOnly();

            /// <summary>
            /// To table
            /// </summary>
            /// <param name="verbose"></param>
            /// <param name="output"></param>
            /// <returns></returns>
            public string ToTable(bool verbose, TableGenerator.Output output)
            {
                var columns = verbose
                                ? new[] { "name", "description", "command", "args", "sys" }
                                : new[] { "name", "description", "sys" };

                static string EncodeSystem(bool system) => system ? "X" : "";

                var rows = Alias.OrderByDescending(a => a.System)
                                .ThenBy(a => a.Name)
                                .Select(a => verbose
                                        ? new[] { a.Name,
                                                  a.Description,
                                                  a.Command,
                                                  string.Join(",", GetArgumentTags(a.Command)),
                                                  EncodeSystem(a.System) }

                                        : new[] { a.Name,
                                                  a.Description,
                                                  EncodeSystem(a.System) });

                return TableGenerator.To(columns, rows, output);
            }

            /// <summary>
            /// Create new alias
            /// </summary>
            /// <param name="name"></param>
            /// <param name="description"></param>
            /// <param name="command"></param>
            /// <param name="system"></param>
            /// <returns></returns>
            public bool Create(string name, string description, string command, bool system)
            {
                if (!AliasDef.IsValid(name) || Exists(name)) { return false; }
                _alias.Add(new AliasDef(name, description, command, system));
                return true;
            }

            /// <summary>
            /// Exists alias
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool Exists(string name) => _alias.Any(a => a.Exists(name));

            /// <summary>
            /// Clear all alias
            /// </summary>
            public void Clear() => _alias.Clear();

            /// <summary>
            /// Remove alias
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public bool Remove(string name)
            {
                var item = _alias.FirstOrDefault(a => a.Names.Contains(name) && !a.System);
                if (item != null) { _alias.Remove(item); }
                return item != null;
            }

            /// <summary>
            /// Filename
            /// </summary>
            /// <value></value>
            public string FileName { get; set; }

            /// <summary>
            /// Load from file
            /// </summary>
            public void Load()
            {
                if (!File.Exists(FileName)) { File.WriteAllLines(FileName, new string[] { }); }

                foreach (var line in File.ReadAllLines(FileName))
                {
                    var data = line.Split('\t');
                    if (data.Length == 3) { Create(data[0], data[1], data[2], false); }
                }
            }

            /// <summary>
            /// Save to file.
            /// </summary>
            public void Save()
                => File.WriteAllLines(FileName, _alias.Where(a => !a.System)
                                                      .Select(a => $"{a.Name}\t{a.Description}\t{a.Command}"));
        }

        /// <summary>
        /// Create tag argument
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CreateArgumentTag(string name) => "{" + name + "}";

        /// <summary>
        /// Get argument into command start "{" end "}"
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string[] GetArgumentTags(string command)
            => new Regex(@"{\s*(.+?)\s*}").Matches(command)
                                          .OfType<Match>()
                                          .Where(a => a.Success)
                                          .Select(a => a.Groups[1].Value)
                                          .ToArray();

        /// <summary>
        /// Create parameter resource split ':'
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IDictionary<string, object> CreateParameterResource(IEnumerable<string> items)
        {
            var parameters = new Dictionary<string, object>();
            foreach (var item in items)
            {
                var pos = item.IndexOf(":");
                if (pos >= 0) { parameters.Add(item.Substring(0, pos), item.Substring(pos + 1)); }
            }
            return parameters;
        }

        /// <summary>
        /// Execute methods
        /// </summary>
        /// <param name="client"></param>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <param name="methodType"></param>
        /// <param name="parameters"></param>
        /// <param name="wait"></param>
        /// <param name="output"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static async Task<(int ResultCode, string ResultText)> Execute(PveClient client,
                                                                              ClassApi classApiRoot,
                                                                              string resource,
                                                                              MethodType methodType,
                                                                              IDictionary<string, object> parameters,
                                                                              bool wait = false,
                                                                              TableGenerator.Output output = TableGenerator.Output.Text,
                                                                              bool verbose = false)
        {
            //create result
            var result = methodType switch
            {
                MethodType.Get => await client.Get(resource, parameters),
                MethodType.Set => await client.Set(resource, parameters),
                MethodType.Create => await client.Create(resource, parameters),
                MethodType.Delete => await client.Delete(resource, parameters),
                _ => throw new InvalidEnumArgumentException(),
            };

            var ret = new StringBuilder();
            if (!result.IsSuccessStatusCode)
            {
                ret.AppendLine(result.ReasonPhrase);
                ret.AppendLine(verbose
                                    ? JsonConvert.SerializeObject((string)result.Response.errors, Formatting.Indented)
                                    : result.GetError());
            }
            else if (result.InError())
            {
                ret.AppendLine(result.ReasonPhrase);
            }
            else
            {
                if (verbose)
                {
                    //verbose full response json
                    ret.AppendLine(JsonConvert.SerializeObject(result.Response, Formatting.Indented));
                }
                else
                {
                    var data = result.ToData();
                    var classApi = ClassApi.GetFromResource(classApiRoot, resource);
                    if (classApi == null)
                    {
                        ret.AppendLine($"no such resource '{resource}'");
                    }
                    else if (data != null)
                    {
                        var returnParameters = classApi.Methods.FirstOrDefault(a => a.IsGet)?.ReturnParameters;
                        if (returnParameters == null || returnParameters.Count == 0)
                        {
                            //no return defined
                            ret.Append(CreateTableDynamic(data, null, output, null));
                        }
                        else
                        {
                            var keys = returnParameters.OrderBy(a => a.Optional)
                                                       .ThenBy(a => a.Name)
                                                       .Select(a => a.Name)
                                                       .ToArray();

                            ret.Append(CreateTableDynamic(data, keys, output, returnParameters));
                        }
                    }
                }

                if (wait)
                {
                    await client.WaitForTaskToFinish(result, 1000, 30000);
                }
            }

            return ((int)result.StatusCode, ret.ToString());
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <param name="output"></param>
        /// <param name="returnParameters"></param>
        /// <returns></returns>
        private static string CreateTableDynamic(dynamic data, string[] keys, TableGenerator.Output output, List<ParameterApi> returnParameters)
        {
            var columns = new List<string>();
            var rows = new List<object[]>();

            if (data is ExpandoObject)
            {
                //dictionary
                var dic = (IDictionary<string, object>)data;

                columns.Add("key");
                columns.Add("value");

                keys ??= dic.Select(a => a.Key).ToArray();

                foreach (var key in keys.OrderBy(a => a))
                {
                    if (dic.TryGetValue(key, out var value))
                    {
                        rows.Add(new object[] { key, GetValue(value, key, returnParameters) });
                    }
                }
            }
            else if (data is IList list)
            {
                if (keys == null)
                {
                    var keysTmp = new List<string>();
                    foreach (IDictionary<string, object> item in data) { keysTmp.AddRange(item.Keys.ToArray()); }
                    keys = keysTmp.Distinct().OrderBy(a => a).ToArray();
                }

                columns.AddRange(keys);
                var rowsTmp = new List<KeyValuePair<object, object[]>>();

                //array data
                foreach (IDictionary<string, object> item in list)
                {
                    //create rows
                    var row = new List<object>();
                    foreach (var title in columns)
                    {
                        if (item.TryGetValue(title, out var value))
                        {
                            value = GetValue(value, title, returnParameters);
                        }

                        value ??= "";
                        row.Add(value);
                    }

                    rowsTmp.Add(new KeyValuePair<object, object[]>(row[0] + "", row.ToArray()));
                }

                //order row by first column
                rows.AddRange(rowsTmp.OrderBy(a => a.Key).Select(a => a.Value).ToArray());
                if (rows.Count == 0) { data = ""; }
            }

            if (rows.Count > 0)
            {
                return TableGenerator.To(columns, rows, output);
            }
            else
            {
                return data;
            }
        }

        private static object GetValue(object value, string key, List<ParameterApi> returnParameters)
        {
            if (returnParameters == null)
            {
                return (value is ExpandoObject || value is IList)
                            ? JsonConvert.SerializeObject(value)
                            : value;
            }
            else
            {
                return returnParameters.FirstOrDefault(a => a.Name == key).RendererValue(value);
            }
        }

        private static void CreateTable(IEnumerable<ParameterApi> parameters, StringBuilder resultText, TableGenerator.Output output)
        {
            if (parameters.Any())
            {
                var values = new List<object[]>();
                foreach (var param in parameters)
                {
                    var partsComment = JoinWord(param.Description
                                                     .Replace("\n", " ")
                                                     .Trim()
                                                     .Split(new[] { " " }, StringSplitOptions.None), 45, " ");

                    //type
                    var partsType = new[] { param.Type };
                    if (!string.IsNullOrWhiteSpace(param.TypeText))
                    {
                        //explicit text
                        partsType = JoinWord(param.TypeText.Split(' '), 18, "");
                    }
                    else if (param.EnumValues.Any())
                    {
                        //enums
                        partsType = JoinWord(param.EnumValues, 18, ",");
                    }

                    for (var i = 0; i < Math.Max(partsType.Length, partsComment.Length); i++)
                    {
                        values.Add(new[] { i == 0 ? param.Name : "",
                                           i < partsType.Length ? partsType[i] : "",
                                           i < partsComment.Length ? partsComment[i] : "" });
                    }
                }

                resultText.Append(TableGenerator.To(new[] { "param", "type", "description" }, values, output));
            }
        }

        /// <summary>
        /// Usage resource
        /// </summary>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <param name="output"></param>
        /// <param name="returnsType"></param>
        /// <param name="command"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public static string Usage(ClassApi classApiRoot,
                                   string resource,
                                   TableGenerator.Output output,
                                   bool returnsType = false,
                                   string command = null,
                                   bool verbose = false)
        {
            var ret = new StringBuilder();
            var classApi = ClassApi.GetFromResource(classApiRoot, resource);
            if (classApi == null)
            {
                ret.AppendLine($"no such resource '{resource}'");
            }
            else
            {
                foreach (var method in classApi.Methods.OrderBy(a => a.MethodType))
                {
                    //exclude other command
                    if (!string.IsNullOrWhiteSpace(command)
                        && method.GetMethodTypeHumanized().ToLower() != command.ToLower())
                    {
                        continue;
                    }

                    ret.Append($"USAGE: {method.GetMethodTypeHumanized()} {resource}");

                    //only parameters no keys
                    var parameters = method.Parameters.Where(a => !classApi.Keys.Contains(a.Name));

                    var opts = string.Join("", parameters.Where(a => !a.Optional).Select(a => $" {a.Name}:<{a.Type}>"));
                    if (!string.IsNullOrWhiteSpace(opts)) { ret.Append(opts); }

                    //optional parameter
                    if (parameters.Any(a => a.Optional)) { ret.Append(" [OPTIONS]"); }

                    ret.AppendLine();

                    if (verbose)
                    {
                        ret.AppendLine().AppendLine("  " + method.Comment);
                        CreateTable(parameters, ret, output);
                    }

                    if (returnsType)
                    {
                        //show returns
                        ret.AppendLine("RETURNS:");
                        CreateTable(method.ReturnParameters, ret, output);
                    }

                    if (verbose) { ret.AppendLine(); }
                }
            }

            return ret.ToString();
        }

        private static string[] JoinWord(string[] words, int numChar, string separator)
        {
            var ret = new List<string>();
            var line = new StringBuilder();
            foreach (var item in words)
            {
                if (!string.IsNullOrWhiteSpace(line.ToString())) { line.Append(separator); }
                line.Append(item);
                if (line.Length >= numChar)
                {
                    ret.Add(line.ToString().Trim());
                    line.Clear();
                }
            }

            if (!string.IsNullOrWhiteSpace(line.ToString())) { ret.Add(line.ToString().Trim()); }
            return ret.ToArray();
        }

        /// <summary>
        /// List values resource
        /// </summary>
        /// <param name="client"></param>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static async Task<(IEnumerable<(string Attribute, string Value)> Values, string Error)> ListValues(PveClient client,
                                                                                                                  ClassApi classApiRoot,
                                                                                                                  string resource)
        {
            var values = new List<(string Attribute, string Value)>();
            var error = "";

            var classApi = ClassApi.GetFromResource(classApiRoot, resource);
            if (classApi == null)
            {
                error = $"no such resource '{resource}'";
            }
            else
            {
                if (classApi.SubClasses.Count == 0)
                {
                    error = $"resource '{resource}' does not define child links";
                }
                else
                {
                    string key = null;
                    foreach (var subClass in classApi.SubClasses.OrderBy(a => a.Name))
                    {
                        var attribute = string.Join("",
                                                    new[] { subClass.SubClasses.Count > 0 ? "D" : "-",
                                                            "r--",
                                                            subClass.Methods.Any(a => a.IsPost)? "c" : "-"});

                        if (subClass.IsIndexed)
                        {
                            var result = await client.Get(resource);
                            if (result.InError())
                            {
                                error = result.GetError();
                            }
                            else
                            {
                                if (key == null)
                                {
                                    var returnLinkHRef = classApi.Methods.FirstOrDefault(a => a.IsGet).ReturnLinkHRef;
                                    if (!string.IsNullOrWhiteSpace(returnLinkHRef))
                                    {
                                        key = returnLinkHRef.Replace("{", "").Replace("}", "");
                                    }
                                }

                                if (result.ToData() != null && !string.IsNullOrWhiteSpace(key))
                                {
                                    var data = new List<object>();
                                    foreach (IDictionary<string, object> item in result.ToData()) { data.Add(item[key]); }
                                    foreach (var item in data.OrderBy(a => a)) { values.Add((attribute, item + "")); }
                                }
                            }
                        }
                        else
                        {
                            values.Add((attribute, subClass.Name));
                        }
                    }
                }
            }

            return (values, error);
        }

        /// <summary>
        /// List structure
        /// </summary>
        /// <param name="client"></param>
        /// <param name="classApiRoot"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static async Task<string> List(PveClient client, ClassApi classApiRoot, string resource)
        {
            var (Values, Error) = await ListValues(client, classApiRoot, resource);
            return string.Join(Environment.NewLine, Values.Select(a => $"{a.Attribute}        {a.Value}")) +
                   (string.IsNullOrWhiteSpace(Error) ? "" : Environment.NewLine + Error) +
                   Environment.NewLine;
        }
    }
}