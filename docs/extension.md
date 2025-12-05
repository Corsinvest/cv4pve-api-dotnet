# Corsinvest.ProxmoxVE.Api.Extension

```bash
dotnet add package Corsinvest.ProxmoxVE.Api.Extension
```

## Key Features

- **ApiExplorer** - Explore and discover API endpoints
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
var result = await client.Nodes["pve1"].Qemu[100].Snapshot.Snapshot("backup-2024");
if (result.IsSuccessStatusCode)
{
    Console.WriteLine("Snapshot created successfully");
}

// Delete snapshot
var deleteResult = await client.Nodes["pve1"].Qemu[100].Snapshot["backup-2024"].Delsnapshot();
if (deleteResult.IsSuccessStatusCode)
{
    Console.WriteLine("Snapshot deleted successfully");
}
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

## ApiExplorer

Discover available API endpoints and their structure:

```csharp
// Note: ApiExplorer functionality allows you to explore the API structure
// This is useful for development and debugging purposes

// Example: Explore available methods on a VM
var vm = client.Nodes["pve1"].Qemu[100];

// The extension provides better IntelliSense and discoverability
// of available methods and their return types
```

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
if (result.IsSuccessStatusCode)
{
    foreach (var node in result.Response.data)
    {
        Console.WriteLine($"Node: {node.node}"); // No IntelliSense, prone to errors
    }
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
        var startResult = await vmInstance.Status.Start.VmStart();
        if (startResult.IsSuccessStatusCode)
        {
            Console.WriteLine($"Started VM {vm.Name}");
        }
    }
}
```
