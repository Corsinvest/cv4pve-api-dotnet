# Corsinvest.ProxmoxVE.Api.Extension

```bash
dotnet add package Corsinvest.ProxmoxVE.Api.Extension
```

## Key Features

- **Shell** - Run API calls, expand aliases and read the API schema as data
- **Strongly-Typed Results** - Extension method **Get()** to decode JSON from Result
- **VM/CT Discovery** - Retrieve VM/CT data from name or ID  
- **Simplified Management** - Simplified VM/CT and snapshot operations
- **Resource Operations** - Enhanced cluster resource management

---

## Extension Method Get()

The main extension method `Get()` converts dynamic API responses to strongly-typed objects:

```csharp
using Corsinvest.ProxmoxVE.Api.Extension;

var client = new PveClient("pve.example.com");
await client.Login("admin@pve", "password");

// Get strongly-typed cluster status  
// Instead of: var result = await client.Cluster.Status.Status();
// Use:
IEnumerable<ClusterStatus> clusterStatus = await client.Cluster.Status.Get();

foreach (var node in clusterStatus)
{
    Console.WriteLine($"Node: {node.Name} - Status: {node.Status}");
}

// Get strongly-typed node information
IEnumerable<NodeInfo> nodes = await client.Nodes.Get();
foreach (var node in nodes)
{
    Console.WriteLine($"Node: {node.Node} - CPU: {node.Cpu:P2}");
}

// Get strongly-typed VM list
IEnumerable<VmInfo> vms = await client.Cluster.Resources.Get();
foreach (var vm in vms.Where(r => r.Type == "qemu"))
{
    Console.WriteLine($"VM: {vm.Name} ({vm.VmId}) - Status: {vm.Status}");
}
```

---

## VM/CT Discovery

The extension provides methods to retrieve VM/CT data from name or ID, as mentioned in the README:

```csharp
// The extension library provides functionality to:
// - Retrieve VM/CT data from name or id
// - Simplify management of VM/CT e.g snapshot

// Note: Specific method implementations may vary
// Check the extension library source for exact method signatures
```

---

## Simplified VM/CT Management

### Enhanced Snapshot Operations

```csharp
// Get VM snapshots with strongly-typed results
var snapshots = await client.Nodes["pve1"].Qemu[100].Snapshot.Get();
foreach (var snapshot in snapshots)
{
    Console.WriteLine($"Snapshot: {snapshot.Name}");
    Console.WriteLine($"Description: {snapshot.Description}");
    Console.WriteLine($"Date: {snapshot.SnapTime}");
}

// Create snapshot (using core API)
await client.Nodes["pve1"].Qemu[100].Snapshot.Snapshot("backup-2024");
Console.WriteLine("Snapshot created successfully");

// Delete snapshot
await client.Nodes["pve1"].Qemu[100].Snapshot["backup-2024"].Delsnapshot();
Console.WriteLine("Snapshot deleted successfully");
```

###  VM Configuration and Status

```csharp
// Get VM configuration with typed results
var vmConfig = await client.Nodes["pve1"].Qemu[100].Config.Get();
Console.WriteLine($"VM Memory: {vmConfig.Memory} MB");
Console.WriteLine($"VM Cores: {vmConfig.Cores}");
Console.WriteLine($"VM Name: {vmConfig.Name}");

// Get VM status
var vmStatus = await client.Nodes["pve1"].Qemu[100].Status.Current.Get();
Console.WriteLine($"Status: {vmStatus.Status}");
Console.WriteLine($"CPU Usage: {vmStatus.Cpu:P2}");
Console.WriteLine($"Memory Usage: {vmStatus.Mem / vmStatus.MaxMem:P2}");
```

### Container Operations

```csharp
// Get container configuration
var ctConfig = await client.Nodes["pve1"].Lxc[101].Config.Get();
Console.WriteLine($"Container: {ctConfig.Hostname}");
Console.WriteLine($"OS Template: {ctConfig.OsTemplate}");

// Get container status
var ctStatus = await client.Nodes["pve1"].Lxc[101].Status.Current.Get();
Console.WriteLine($"Container Status: {ctStatus.Status}");
Console.WriteLine($"Uptime: {ctStatus.Uptime} seconds");
```

---

## Cluster Resource Management

```csharp
// Get all cluster resources with filtering
var allResources = await client.Cluster.Resources.Get();

// Filter VMs
var vms = allResources.Where(r => r.Type == "qemu");
foreach (var vm in vms)
{
    Console.WriteLine($"VM: {vm.Name} ({vm.VmId}) on {vm.Node} - {vm.Status}");
}

// Filter containers
var containers = allResources.Where(r => r.Type == "lxc");
foreach (var ct in containers)
{
    Console.WriteLine($"CT: {ct.Name} ({ct.VmId}) on {ct.Node} - {ct.Status}");
}

// Filter nodes
var nodes = allResources.Where(r => r.Type == "node");
foreach (var node in nodes)
{
    Console.WriteLine($"Node: {node.Node} - CPU: {node.Cpu:P2}, Memory: {node.Mem / node.MaxMem:P2}");
}

// Filter storage
var storages = allResources.Where(r => r.Type == "storage");
foreach (var storage in storages)
{
    Console.WriteLine($"Storage: {storage.Storage} on {storage.Node} - {storage.Disk / storage.MaxDisk:P2} used");
}
```

---

## Shell: API calls from a command line or a chat

`Corsinvest.ProxmoxVE.Api.Extension.Shell` runs API calls and expands aliases, returning data. It is used by
cv4pve-cli and the bots; formatting and storing aliases are up to the caller.

```csharp
using Corsinvest.ProxmoxVE.Api.Extension.Shell;

// --key value parameters, as typed on a command line
var (parameters, _) = ApiCommandLine.ParseParameters(["--type", "vm"]);
var command = new ApiCommand(MethodType.Get, "/cluster/resources",
                             parameters.ToDictionary(a => a.Key, a => (object)a.Value));

var response = await ApiRequest.ExecuteAsync(client, command);
if (!response.IsSuccess)
{
    Console.Error.WriteLine($"{response.StatusCode} {response.Error}");
    foreach (var (name, error) in response.ParameterErrors) { Console.Error.WriteLine($"{name}: {error}"); }
}

// An alias: placeholders filled by position, --guest looked up in the cluster, --yes for confirmation
var alias = new ApiAlias("do start vm", "Start a VM", "create /nodes/{node}/qemu/{vmid}/status/start");
var expanded = await ApiCommandLine.ExpandAliasAsync(alias, ["--guest", "web01"], client);
if (expanded.Command != null)
{
    var started = await ApiRequest.ExecuteAsync(client, expanded.Command, new ApiWaitOptions(TimeSpan.FromMinutes(5)));
    Console.WriteLine(started.Task?.Succeeded == true ? "started" : started.Task?.ExitStatus);
}
```

`ApiSchema` reads the API schema as data: the methods of a path with their parameters and returned fields,
the values a parameter accepts, and what is under a path (fixed names, or values read from the cluster).

```csharp
var root = await GeneratorClassApi.GenerateAsync("pve01.example.com"); // schema read from a node

foreach (var method in ApiSchema.GetMethods(root, "/nodes/pve01/qemu/100/config") ?? [])
{
    Console.WriteLine($"{method.Method}: {method.Description}");
    foreach (var parameter in method.Parameters)
    {
        Console.WriteLine($"  --{parameter.Name} {string.Join(",", ApiSchema.GetAllowedValues(parameter))}");
    }
}

var children = await ApiSchema.GetChildrenAsync(client, root, "/nodes");
foreach (var child in children.Children) { Console.WriteLine(child.Name); }
if (children.Error != null) { Console.Error.WriteLine(children.Error); }
```

`ApiExplorerHelper` is obsolete: it returns text and is kept only for existing callers. It now runs on these
classes; use them instead.

---

## Tables: TableGenerator

`Corsinvest.ProxmoxVE.Api.Shared.Utils.TableGenerator` writes a list as Text, Markdown, Html or Json
(`TableGenerator.Output`, the values of the `-o` option of the cv4pve tools). Numbers are aligned right.

```csharp
// typed items: title from the member name unless .Title() says otherwise
Console.Write(TableGenerator.From(snapshots)
                            .Column(a => a.Node).Title("NODE")
                            .Column(a => a.VmId).Title("VM")
                            .Column(a => a.Running ? "X" : "").Title("RUNNING")
                            .To(TableGenerator.Output.Text));

// API answers: columns read by key, a missing key is an empty cell
IEnumerable<dynamic> tasks = (await client.Cluster.Tasks.Tasks()).ToEnumerable();
Console.Write(TableGenerator.From(tasks)
                            .Column("upid")
                            .Column("status").Format(v => v ?? "running")
                            .ToMarkdown());

// rows by hand
Console.Write(new TableGenerator("key", "value").AddRow("memory", 4096).ToText());
```

`ApiSchema.ToTable(response.Data, root, resource)` turns the data of an API answer into a `TableGenerator`, as
pvesh shows it: an object with every key, a list with the columns the schema describes (values rendered as the schema
says), or null for a single value. `ApiTableOptions` chooses what to show:
`ToTable(data, root, resource, new ApiTableOptions(AllColumns: true), out var hidden)` shows every column of a list
(without it, `hidden` names the columns left out); `HumanReadable: false` keeps the values as the API returns them
instead of sizes, percentages, durations and dates as text (use it for Json).

The static `TableGenerator.To(columns, rows, output)` and `ToText/ToMarkdown/ToHtml/ToJson(columns, rows)` of previous
versions are removed: use `new TableGenerator(columns).AddRows(rows).To(output)`.

---

## Best Practices

### **Using Extension Methods**

```csharp
// Use Get() extension method for strongly-typed results
var nodes = await client.Nodes.Get();
foreach (var node in nodes)
{
    Console.WriteLine($"Node: {node.Node}");
}

// Instead of working with dynamic objects
var result = await client.Nodes.Index();
foreach (var node in result.Response.data)
{
    Console.WriteLine($"Node: {node.node}"); // No IntelliSense, prone to errors
}
```

### **VM/CT Discovery Pattern**

```csharp
// Use extension methods for VM/CT discovery
// (Exact implementation depends on the extension library)

// Combine with LINQ for powerful filtering of strongly-typed results
var runningVms = (await client.Cluster.Resources.Get())
    .Where(r => r.Type == "qemu" && r.Status == "running")
    .ToList();
```

---

## Model Types

The extension library provides strongly-typed models for common Proxmox VE objects:

- **ClusterStatus** - Cluster node status information
- **NodeInfo** - Node details and resource usage  
- **VmInfo** - Virtual machine information
- **VmConfig** - VM configuration details
- **VmStatus** - VM runtime status
- **ContainerInfo** - LXC container information
- **SnapshotInfo** - Snapshot details
- **StorageInfo** - Storage resource information

---

## Integration with Core API

The Extension package works seamlessly with the core API:

```csharp
// Use extension methods for reading
var vms = await client.Cluster.Resources.Get();

// Use core API for operations
foreach (var vm in vms.Where(v => v.Type == "qemu"))
{
    var vmInstance = client.Nodes[vm.Node].Qemu[vm.VmId];
    
    // Get strongly-typed status
    var status = await vmInstance.Status.Current.Get();
    
    if (status.Status == "stopped")
    {
        // Use core API for actions
        await vmInstance.Status.Start.VmStart();
        Console.WriteLine($"Started VM {vm.Name}");
    }
}
```
