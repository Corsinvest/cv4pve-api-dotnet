# Basic Examples 📚

This guide provides common usage patterns and practical examples for getting started with the Proxmox VE API.

## 🚀 Getting Started

### 🔌 **Basic Connection**

```csharp
using Corsinvest.ProxmoxVE.Api;
using Corsinvest.ProxmoxVE.Api.Extension;

// Create client and authenticate
var client = new PveClient("pve.example.com");
client.ApiToken = "user@pve!token=uuid";

// Test connection
var version = await client.Version.Version();
if (version.IsSuccessStatusCode)
{
    Console.WriteLine($"Connected to Proxmox VE {version.Response.data.version}");
}
```

### 🏗️ **Client Setup with Error Handling**

```csharp
public static async Task<PveClient> CreateClient()
{
    var client = new PveClient("pve.local")
    {
        ValidateCertificate = false, // For development
        Timeout = TimeSpan.FromMinutes(2)
    };
    
    try
    {
        // Use API token or login
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PVE_TOKEN")))
        {
            client.ApiToken = Environment.GetEnvironmentVariable("PVE_TOKEN");
        }
        else
        {
            var success = await client.Login("root@pam", "password");
            if (!success)
            {
                throw new Exception("Authentication failed");
            }
        }
        
        return client;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to create client: {ex.Message}");
        throw;
    }
}
```

---

## 🖥️ Virtual Machine Operations

### 📋 **List Virtual Machines**

```csharp
// Get all VMs in cluster
var resources = await client.Cluster.Resources.Resources();
if (resources.IsSuccessStatusCode)
{
    foreach (var resource in resources.Response.data)
    {
        if (resource.type == "qemu")
        {
            Console.WriteLine($"VM {resource.vmid}: {resource.name} on {resource.node} - {resource.status}");
        }
    }
}

// Using Extension method for strongly-typed results
var vms = await client.Cluster.Resources.Get()
    .Where(r => r.Type == "qemu");
    
foreach (var vm in vms)
{
    Console.WriteLine($"VM {vm.VmId}: {vm.Name} ({vm.Status})");
}
```

### ⚙️ **Get VM Configuration**

```csharp
// Get VM configuration
var vmConfig = await client.Nodes["pve1"].Qemu[100].Config.VmConfig();
if (vmConfig.IsSuccessStatusCode)
{
    var config = vmConfig.Response.data;
    Console.WriteLine($"VM Name: {config.name}");
    Console.WriteLine($"Memory: {config.memory} MB");
    Console.WriteLine($"CPU Cores: {config.cores}");
    Console.WriteLine($"Boot Order: {config.boot}");
}

// Using Extension method
var config = await client.Nodes["pve1"].Qemu[100].Config.Get();
Console.WriteLine($"VM: {config.Name} - {config.Memory} MB RAM, {config.Cores} cores");
```

### 🔄 **VM Power Management**

```csharp
var vm = client.Nodes["pve1"].Qemu[100];

// Start VM
var startResult = await vm.Status.Start.VmStart();
if (startResult.IsSuccessStatusCode)
{
    Console.WriteLine("✅ VM started successfully");
}

// Stop VM
var stopResult = await vm.Status.Stop.VmStop();
if (stopResult.IsSuccessStatusCode)
{
    Console.WriteLine("⏹️ VM stopped successfully");
}

// Restart VM
var rebootResult = await vm.Status.Reboot.VmReboot();
if (rebootResult.IsSuccessStatusCode)
{
    Console.WriteLine("🔄 VM restarted successfully");
}

// Get current status
var status = await vm.Status.Current.VmStatus();
if (status.IsSuccessStatusCode)
{
    Console.WriteLine($"VM Status: {status.Response.data.status}");
    Console.WriteLine($"CPU Usage: {status.Response.data.cpu:P2}");
    Console.WriteLine($"Memory: {status.Response.data.mem / status.Response.data.maxmem:P2}");
}
```

### 📸 **Snapshot Management**

```csharp
var vm = client.Nodes["pve1"].Qemu[100];

// Create snapshot
var snapshotResult = await vm.Snapshot.Snapshot("backup-2024", "Pre-update backup");
if (snapshotResult.IsSuccessStatusCode)
{
    Console.WriteLine("📸 Snapshot created successfully");
}

// List snapshots
var snapshots = await vm.Snapshot.SnapshotList();
if (snapshots.IsSuccessStatusCode)
{
    Console.WriteLine("Available snapshots:");
    foreach (var snapshot in snapshots.Response.data)
    {
        Console.WriteLine($"  - {snapshot.name}: {snapshot.description} ({snapshot.snaptime})");
    }
}

// Restore snapshot
var restoreResult = await vm.Snapshot["backup-2024"].Rollback.RollbackVm();
if (restoreResult.IsSuccessStatusCode)
{
    Console.WriteLine("🔄 Snapshot restored successfully");
}

// Delete snapshot
var deleteResult = await vm.Snapshot["backup-2024"].Delsnapshot();
if (deleteResult.IsSuccessStatusCode)
{
    Console.WriteLine("🗑️ Snapshot deleted successfully");
}
```

---

## 📦 Container Operations

### 🐳 **List Containers**

```csharp
// Get all containers
var containers = await client.Cluster.Resources.Get()
    .Where(r => r.Type == "lxc");
    
foreach (var container in containers)
{
    Console.WriteLine($"CT {container.VmId}: {container.Name} on {container.Node} - {container.Status}");
}
```

### ⚙️ **Container Management**

```csharp
var container = client.Nodes["pve1"].Lxc[101];

// Get container configuration
var config = await container.Config.VmConfig();
if (config.IsSuccessStatusCode)
{
    var ctConfig = config.Response.data;
    Console.WriteLine($"Container: {ctConfig.hostname}");
    Console.WriteLine($"OS Template: {ctConfig.ostemplate}");
    Console.WriteLine($"Memory: {ctConfig.memory} MB");
}

// Start container
var startResult = await container.Status.Start.VmStart();
if (startResult.IsSuccessStatusCode)
{
    Console.WriteLine("🚀 Container started");
}

// Get container status
var status = await container.Status.Current.VmStatus();
if (status.IsSuccessStatusCode)
{
    Console.WriteLine($"Status: {status.Response.data.status}");
    Console.WriteLine($"Uptime: {status.Response.data.uptime} seconds");
}
```

---

## 🏗️ Cluster Operations

### 📊 **Cluster Status**

```csharp
// Get cluster status
var clusterStatus = await client.Cluster.Status.Status();
if (clusterStatus.IsSuccessStatusCode)
{
    Console.WriteLine("Cluster Status:");
    foreach (var item in clusterStatus.Response.data)
    {
        Console.WriteLine($"  {item.type}: {item.name} - {item.status}");
    }
}

// Using Extension method
var status = await client.Cluster.Status.Get();
foreach (var item in status)
{
    Console.WriteLine($"{item.Type}: {item.Name} - {item.Status}");
}
```

### 🖥️ **Node Information**

```csharp
// Get all nodes
var nodes = await client.Nodes.Index();
if (nodes.IsSuccessStatusCode)
{
    Console.WriteLine("Available Nodes:");
    foreach (var node in nodes.Response.data)
    {
        Console.WriteLine($"  {node.node}: {node.status}");
        Console.WriteLine($"    CPU: {node.cpu:P2}");
        Console.WriteLine($"    Memory: {node.mem / node.maxmem:P2}");
        Console.WriteLine($"    Uptime: {TimeSpan.FromSeconds(node.uptime)}");
    }
}

// Using Extension method
var nodeList = await client.Nodes.Get();
foreach (var node in nodeList)
{
    Console.WriteLine($"Node {node.Node}: CPU {node.Cpu:P2}, Memory {node.Mem.ToHumanReadableSize()}/{node.MaxMem.ToHumanReadableSize()}");
}
```

### 💾 **Storage Information**

```csharp
// Get storage for a specific node
var storages = await client.Nodes["pve1"].Storage.Index();
if (storages.IsSuccessStatusCode)
{
    Console.WriteLine("Available Storage:");
    foreach (var storage in storages.Response.data)
    {
        var usedPercent = (double)storage.used / storage.total * 100;
        Console.WriteLine($"  {storage.storage} ({storage.type}): {usedPercent:F1}% used");
        Console.WriteLine($"    Total: {storage.total / (1024*1024*1024):F2} GB");
        Console.WriteLine($"    Available: {storage.avail / (1024*1024*1024):F2} GB");
    }
}
```

---

## 🔧 Common Patterns

### 📊 **Resource Monitoring**

```csharp
public static async Task MonitorResources(PveClient client)
{
    while (true)
    {
        var resources = await client.Cluster.Resources.Get();
        
        Console.Clear();
        Console.WriteLine($"Proxmox VE Resource Monitor - {DateTime.Now:HH:mm:ss}");
        Console.WriteLine("=".PadRight(50, '='));
        
        // Group by type
        var nodes = resources.Where(r => r.Type == "node");
        var vms = resources.Where(r => r.Type == "qemu");
        var containers = resources.Where(r => r.Type == "lxc");
        
        Console.WriteLine($"Nodes: {nodes.Count()}");
        foreach (var node in nodes)
        {
            Console.WriteLine($"  📊 {node.Node}: CPU {node.Cpu:P1}, Memory {node.Mem.ToPercentage(node.MaxMem):F1}%");
        }
        
        Console.WriteLine($"\nVMs: {vms.Count()} ({vms.Count(v => v.Status == "running")} running)");
        Console.WriteLine($"Containers: {containers.Count()} ({containers.Count(c => c.Status == "running")} running)");
        
        await Task.Delay(5000); // Update every 5 seconds
    }
}
```

### 🔄 **Batch Operations**

```csharp
public static async Task BatchVmOperation(PveClient client, int[] vmIds, string operation)
{
    var tasks = new List<Task<Result>>();
    
    foreach (var vmId in vmIds)
    {
        // Find VM location
        var resources = await client.Cluster.Resources.Get();
        var vm = resources.FirstOrDefault(r => r.Type == "qemu" && r.VmId == vmId);
        
        if (vm != null)
        {
            var vmInstance = client.Nodes[vm.Node].Qemu[vmId];
            
            Task<Result> task = operation.ToLower() switch
            {
                "start" => vmInstance.Status.Start.VmStart(),
                "stop" => vmInstance.Status.Stop.VmStop(),
                "restart" => vmInstance.Status.Reboot.VmReboot(),
                _ => throw new ArgumentException($"Unknown operation: {operation}")
            };
            
            tasks.Add(task);
        }
    }
    
    var results = await Task.WhenAll(tasks);
    
    for (int i = 0; i < vmIds.Length; i++)
    {
        var success = results[i].IsSuccessStatusCode;
        Console.WriteLine($"VM {vmIds[i]} {operation}: {(success ? "✅" : "❌")}");
    }
}
```

### 📈 **Performance Monitoring**

```csharp
public static async Task GetVmPerformance(PveClient client, string node, int vmId)
{
    var vm = client.Nodes[node].Qemu[vmId];
    
    // Get current status
    var status = await vm.Status.Current.VmStatus();
    if (status.IsSuccessStatusCode)
    {
        var data = status.Response.data;
        
        Console.WriteLine($"VM {vmId} Performance:");
        Console.WriteLine($"  Status: {data.status}");
        Console.WriteLine($"  CPU Usage: {data.cpu:P2}");
        Console.WriteLine($"  Memory: {data.mem.ToHumanReadableSize()} / {data.maxmem.ToHumanReadableSize()} ({data.mem.ToPercentage(data.maxmem):F1}%)");
        Console.WriteLine($"  Disk Read: {data.diskread.ToHumanReadableSize()}");
        Console.WriteLine($"  Disk Write: {data.diskwrite.ToHumanReadableSize()}");
        Console.WriteLine($"  Network In: {data.netin.ToHumanReadableSize()}");
        Console.WriteLine($"  Network Out: {data.netout.ToHumanReadableSize()}");
        Console.WriteLine($"  Uptime: {TimeSpan.FromSeconds(data.uptime)}");
    }
}
```

---

## 🎯 Best Practices

### ✅ **Error Handling**

```csharp
public static async Task<bool> SafeVmOperation(PveClient client, string node, int vmId, string operation)
{
    try
    {
        var vm = client.Nodes[node].Qemu[vmId];
        
        Result result = operation.ToLower() switch
        {
            "start" => await vm.Status.Start.VmStart(),
            "stop" => await vm.Status.Stop.VmStop(),
            _ => throw new ArgumentException($"Unknown operation: {operation}")
        };
        
        if (result.IsSuccessStatusCode)
        {
            Console.WriteLine($"✅ VM {vmId} {operation} successful");
            return true;
        }
        else
        {
            Console.WriteLine($"❌ VM {vmId} {operation} failed: {result.GetError()}");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Exception during {operation} on VM {vmId}: {ex.Message}");
        return false;
    }
}
```

### 📊 **Resource Discovery**

```csharp
public static async Task<(string node, int vmId)?> FindVm(PveClient client, string vmName)
{
    var resources = await client.Cluster.Resources.Get();
    var vm = resources.FirstOrDefault(r => 
        r.Type == "qemu" && 
        r.Name?.Equals(vmName, StringComparison.OrdinalIgnoreCase) == true);
    
    return vm != null ? (vm.Node, vm.VmId) : null;
}

// Usage
var vmLocation = await FindVm(client, "web-server");
if (vmLocation.HasValue)
{
    var (node, vmId) = vmLocation.Value;
    var vm = client.Nodes[node].Qemu[vmId];
    // ... work with VM
}
```

---

<div align="center">
  <sub>Made with ❤️ in Italy 🇮🇹 by <a href="https://www.corsinvest.it">Corsinvest</a></sub>
</div>
