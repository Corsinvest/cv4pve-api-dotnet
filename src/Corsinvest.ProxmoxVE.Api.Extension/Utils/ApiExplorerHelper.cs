/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Api Explorer
/// </summary>
[Obsolete("Use the classes in Corsinvest.ProxmoxVE.Api.Extension.Shell: ApiRequest, ApiCommandLine, ApiSchema.")]
public static partial class ApiExplorerHelper
{
    /// <summary>
    /// Alias command.
    /// </summary>
    public partial class AliasDef(string name, string description, string command, bool system)
    {

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; } = description;

        /// <summary>
        /// Command
        /// </summary>
        public string Command { get; } = command;

        /// <summary>
        /// System
        /// </summary>
        public bool System { get; } = system;

        /// <summary>
        /// Check exists name or alias
        /// </summary>
        public bool Exists(string name) => name.Split(',').Any(a => Names.Contains(a));

        /// <summary>
        /// Name alias
        /// </summary>
        public string[] Names => Name.Split(',');

        /// <summary>
        /// Check name is valid
        /// </summary>
        public static bool IsValid(string name) => ValidNameRegex().IsMatch(name);
        [GeneratedRegex("^[a-zA-Z0-9,_-]*$")]
        private static partial Regex ValidNameRegex();
    }

    /// <summary>
    /// Alias manager
    /// </summary>
    public class AliasManager
    {
        private readonly List<AliasDef> _alias =
        [
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
            new("node-vzdump-config,nvcfg", "Node Extract configuration from vzdump backup archive", "get /nodes/{node}/vzdump/extractconfig volume:{volume}", true),

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
        ];

        /// <summary>
        /// Alias
        /// </summary>
        public ReadOnlyCollection<AliasDef> Alias => _alias.AsReadOnly();

        /// <summary>
        /// To table
        /// </summary>
        public string ToTable(bool verbose, TableGenerator.Output output)
            => TableGenerator.From(Alias.OrderByDescending(a => a.System).ThenBy(a => a.Name))
                             .Column(a => a.Name).Title("name")
                             .Column(a => a.Description).Title("description")
                             .Column(a => a.Command).Title("command").When(verbose)
                             .Column(a => string.Join(",", ApiCommandLine.GetPlaceholders(a.Command))).Title("args").When(verbose)
                             .Column(a => a.System ? "X" : string.Empty).Title("sys")
                             .To(output);

        /// <summary>
        /// Create new alias
        /// </summary>
        public bool Create(string name, string description, string command, bool system)
        {
            if (!AliasDef.IsValid(name) || Exists(name)) { return false; }
            _alias.Add(new AliasDef(name, description, command, system));
            return true;
        }

        /// <summary>
        /// Exists alias
        /// </summary>
        public bool Exists(string name) => _alias.Any(a => a.Exists(name));

        /// <summary>
        /// Clear all alias
        /// </summary>
        public void Clear() => _alias.Clear();

        /// <summary>
        /// Remove alias
        /// </summary>
        public bool Remove(string name)
        {
            var item = _alias.FirstOrDefault(a => a.Names.Contains(name) && !a.System);
            if (item != null) { _alias.Remove(item); }
            return item != null;
        }

        /// <summary>
        /// Filename
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Load from file
        /// </summary>
        public void Load()
        {
            if (!File.Exists(FileName)) { File.WriteAllLines(FileName, []); }

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
    public static string CreateArgumentTag(string name) => "{" + name + "}";

    /// <summary>
    /// Get argument into command start "{" end "}"
    /// </summary>
    [Obsolete("Use Corsinvest.ProxmoxVE.Api.Extension.Shell.ApiCommandLine.GetPlaceholders.")]
    public static string[] GetArgumentTags(string command) => [.. ApiCommandLine.GetPlaceholders(command)];

    /// <summary>
    /// Get parameter names for a resource and method (excludes path keys).
    /// </summary>
    public static string[] GetMethodParameters(ClassApi classApiRoot, string resource, MethodType methodType)
        => [.. ApiSchema.GetMethod(classApiRoot, resource, methodType)?.Parameters.Select(p => p.Name) ?? []];

    /// <summary>
    /// Get enum values for a specific parameter of a resource/method. Returns empty if not an enum.
    /// </summary>
    public static string[] GetMethodParameterEnumValues(ClassApi classApiRoot, string resource, MethodType methodType, string paramName)
    {
        var parameter = ApiSchema.GetMethod(classApiRoot, resource, methodType)?
                                 .Parameters
                                 .FirstOrDefault(p => string.Equals(p.Name, paramName, StringComparison.OrdinalIgnoreCase));
        return parameter == null ? [] : [.. ApiSchema.GetAllowedValues(parameter)];
    }

    /// <summary>
    /// Create parameter resource split ':'
    /// </summary>
    /// <exception cref="ArgumentException">A parameter is given more than once.</exception>
    public static IDictionary<string, object> CreateParameterResource(IEnumerable<string> items)
    {
        var parameters = new Dictionary<string, object>();
        foreach (var item in items)
        {
            var pos = item.IndexOf(':');
            if (pos < 0) { continue; }

            var key = item[..pos];
            if (!parameters.TryAdd(key, item[(pos + 1)..]))
            {
                throw new ArgumentException($"Parameter '{key}' is given more than once.");
            }
        }
        return parameters;
    }

    /// <summary>
    /// Execute methods
    /// </summary>
    /// <param name="client">Client.</param>
    /// <param name="classApiRoot">API schema.</param>
    /// <param name="resource">API path.</param>
    /// <param name="methodType">Method.</param>
    /// <param name="parameters">Parameters of the call.</param>
    /// <param name="wait">Wait for the task started by the call to finish.</param>
    /// <param name="output">Output format.</param>
    /// <param name="verbose">Return the full JSON answer.</param>
    /// <param name="waitTimeout">Milliseconds to wait with <paramref name="wait"/>; 0 or less waits until the task ends.</param>
    [Obsolete("Use Corsinvest.ProxmoxVE.Api.Extension.Shell.ApiRequest.ExecuteAsync, which returns the answer as data.")]
    public static async Task<(int ResultCode, string ResultText)> ExecuteAsync(PveClient client,
                                                                               ClassApi classApiRoot,
                                                                               string resource,
                                                                               MethodType methodType,
                                                                               IDictionary<string, object> parameters,
                                                                               bool wait = false,
                                                                               TableGenerator.Output output = TableGenerator.Output.Text,
                                                                               bool verbose = false,
                                                                               long waitTimeout = 30000)
    {
        var response = await ApiRequest.ExecuteAsync(client,
                                                     new ApiCommand(methodType, resource, new Dictionary<string, object>(parameters ?? new Dictionary<string, object>())),
                                                     wait
                                                        ? new ApiWaitOptions(waitTimeout > 0 ? TimeSpan.FromMilliseconds(waitTimeout) : null)
                                                        : null);

        var ret = new StringBuilder();
        if (response.StatusCode is < 200 or > 299)
        {
            ret.AppendLine(response.Error);
            ret.AppendLine(verbose
                            ? JsonConvert.SerializeObject(response.Raw, Formatting.Indented)
                            : string.Join(Environment.NewLine, response.ParameterErrors.Select(a => $"{a.Key} : {a.Value}")));
        }
        else if (!response.IsSuccess)
        {
            // 200 with "errors": reported as text with status 200, as before.
            ret.AppendLine(response.Error);
        }
        else if (verbose)
        {
            ret.AppendLine(JsonConvert.SerializeObject(response.Raw, Formatting.Indented));
        }
        else
        {
            var classApi = ClassApi.GetFromResource(classApiRoot, resource);
            if (classApi == null)
            {
                ret.AppendLine($"no such resource '{resource}'");
            }
            else if (response.Data != null)
            {
                var json = output is TableGenerator.Output.Json or TableGenerator.Output.JsonPretty;
                var table = ApiSchema.ToTable(response.Data, classApiRoot, resource, new ApiTableOptions(AllColumns: json, HumanReadable: !json));
                ret.Append(table == null ? response.Data + string.Empty : table.To(output));
            }
        }

        return (response.StatusCode, ret.ToString());
    }

    /// <summary>
    /// Usage resource
    /// </summary>
    public static string Usage(ClassApi classApiRoot,
                               string resource,
                               TableGenerator.Output output,
                               bool returnsType = false,
                               string command = null,
                               bool verbose = false,
                               bool optionStyle = false)
    {
        var methods = ApiSchema.GetMethods(classApiRoot, resource);
        if (methods == null) { return $"no such resource '{resource}'{Environment.NewLine}"; }

        // Same order as before: by HTTP method name (DELETE, GET, POST, PUT).
        return ApiSchemaText.Usage(resource,
                                   methods.Where(a => string.IsNullOrWhiteSpace(command)
                                                      || string.Equals(a.Method.ToString(), command, StringComparison.OrdinalIgnoreCase))
                                          .OrderBy(a => HttpMethodName(a.Method), StringComparer.Ordinal),
                                   verbose,
                                   returnsType,
                                   output,
                                   optionStyle);
    }

    private static string HttpMethodName(MethodType method)
        => method switch
        {
            MethodType.Get => "GET",
            MethodType.Set => "PUT",
            MethodType.Create => "POST",
            _ => "DELETE",
        };

    /// <summary>
    /// List values resource
    /// </summary>
    public static async Task<(IEnumerable<(string Attribute, string Value)> Values, string Error)> ListValuesAsync(PveClient client,
                                                                                                                   ClassApi classApiRoot,
                                                                                                                   string resource)
    {
        var result = await ApiSchema.GetChildrenAsync(client, classApiRoot, resource);
        return ([.. result.Children.Select(a => (string.Concat(a.HasChildren ? "D" : "-", "r--", a.AcceptsCreate ? "c" : "-"), a.Name))],
                result.Error ?? string.Empty);
    }

    /// <summary>
    /// List structure
    /// </summary>
    public static async Task<string> ListAsync(PveClient client, ClassApi classApiRoot, string resource)
        => ApiSchemaText.List(await ApiSchema.GetChildrenAsync(client, classApiRoot, resource));
}
