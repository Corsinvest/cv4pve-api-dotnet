# Corsinvest.ProxmoxVE.Api.Shared

<div align="center">

[![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Shared.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Shared)
[![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Shared.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Shared)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

**Shared Models and Utilities for Proxmox VE**

*Common models, types, and utilities used across Proxmox VE API packages*

</div>

---

## Overview

The `Corsinvest.ProxmoxVE.Api.Shared` package contains model definitions for JSON conversion and utility classes that are shared across the Proxmox VE API ecosystem. It provides strongly-typed representations of Proxmox VE objects and common utilities.

## Installation

```bash
dotnet add package Corsinvest.ProxmoxVE.Api.Shared
```

> **Note:** This package is typically included as a dependency by other Corsinvest.ProxmoxVE packages.

## Key Features

- **Data Models** - Strongly-typed models for Proxmox VE objects
- **JSON Conversion** - Models optimized for JSON serialization/deserialization
- **Utility Classes** - Common utilities and helper methods
- **Type Definitions** - Enums and constants for Proxmox VE values
- **Shared Interfaces** - Common interfaces used across packages

---

## Common Models

### Virtual Machine Models

```csharp
using Corsinvest.ProxmoxVE.Api.Shared.Models;

// VM Configuration model
public class VmConfig
{
    public string Name { get; set; }
    public int Memory { get; set; }
    public int Cores { get; set; }
    public int Sockets { get; set; }
    public string OsType { get; set; }
    public string Boot { get; set; }
    public bool OnBoot { get; set; }
    public string Agent { get; set; }
    // Additional properties...
}

// VM Status model
public class VmStatus
{
    public string Status { get; set; }
    public long Uptime { get; set; }
    public double Cpu { get; set; }
    public long Mem { get; set; }
    public long MaxMem { get; set; }
    public long Disk { get; set; }
    public long MaxDisk { get; set; }
    public long NetIn { get; set; }
    public long NetOut { get; set; }
    // Additional properties...
}
```

### Container Models

```csharp
// Container Configuration model
public class ContainerConfig
{
    public string Hostname { get; set; }
    public string OsTemplate { get; set; }
    public int Memory { get; set; }
    public int Swap { get; set; }
    public string RootFs { get; set; }
    public bool OnBoot { get; set; }
    public bool Unprivileged { get; set; }
    // Additional properties...
}

// Container Status model
public class ContainerStatus
{
    public string Status { get; set; }
    public long Uptime { get; set; }
    public double Cpu { get; set; }
    public long Mem { get; set; }
    public long MaxMem { get; set; }
    public long Swap { get; set; }
    public long MaxSwap { get; set; }
    // Additional properties...
}
```

### Cluster Models

```csharp
// Cluster Status model
public class ClusterStatus
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Id { get; set; }
    public bool Online { get; set; }
    public string Ip { get; set; }
    // Additional properties...
}

// Cluster Resource model
public class ClusterResource
{
    public string Type { get; set; }
    public string Node { get; set; }
    public int? VmId { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public double? Cpu { get; set; }
    public long? Mem { get; set; }
    public long? MaxMem { get; set; }
    public long? Disk { get; set; }
    public long? MaxDisk { get; set; }
    // Additional properties...
}
```

### Node Models

```csharp
// Node Information model
public class NodeInfo
{
    public string Node { get; set; }
    public string Status { get; set; }
    public double Cpu { get; set; }
    public long Mem { get; set; }
    public long MaxMem { get; set; }
    public long Disk { get; set; }
    public long MaxDisk { get; set; }
    public long Uptime { get; set; }
    public string Level { get; set; }
    // Additional properties...
}
```

### Snapshot Models

```csharp
// Snapshot Information model
public class SnapshotInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public long SnapTime { get; set; }
    public string Parent { get; set; }
    public bool Running { get; set; }
    public bool VmState { get; set; }
    // Additional properties...
}
```

---

## Enums and Constants

### Resource Types

```csharp
// Cluster resource types
public enum ClusterResourceType
{
    Node,
    Qemu,
    Lxc,
    Storage,
    Pool
}

// VM/CT Status
public enum VmStatus
{
    Running,
    Stopped,
    Suspended,
    Paused
}

// Node Status
public enum NodeStatus
{
    Online,
    Offline,
    Unknown
}
```

### Configuration Constants

```csharp
// Common Proxmox VE constants
public static class PveConstants
{
    public const string DefaultRealm = "pam";
    public const int DefaultPort = 8006;
    public const string ApiPath = "/api2/json";

    // Resource type strings
    public const string ResourceTypeNode = "node";
    public const string ResourceTypeQemu = "qemu";
    public const string ResourceTypeLxc = "lxc";
    public const string ResourceTypeStorage = "storage";
}
```

---

## Utility Classes

### JSON Converters

```csharp
using Corsinvest.ProxmoxVE.Api.Shared.Utils;

// Custom JSON converters for Proxmox VE specific data types
public class UnixTimestampConverter : JsonConverter<DateTime>
{
    // Converts Unix timestamps to DateTime objects
}

public class BooleanConverter : JsonConverter<bool>
{
    // Handles Proxmox VE boolean representations (0/1, true/false)
}
```

### Extension Methods

```csharp
// Extension methods for common operations
public static class Extensions
{
    // Convert Unix timestamp to DateTime
    public static DateTime FromUnixTime(this long unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
    }

    // Format bytes to human-readable string
    public static string ToHumanReadableSize(this long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    // Calculate percentage
    public static double ToPercentage(this long used, long total)
    {
        return total > 0 ? (double)used / total * 100 : 0;
    }
}
```

---

## Usage Examples

### Working with Models

```csharp
using Corsinvest.ProxmoxVE.Api.Shared.Models;
using Newtonsoft.Json;

// Deserialize JSON response to strongly-typed model
string jsonResponse = "{ \"name\": \"test-vm\", \"memory\": 2048, \"cores\": 2 }";
var vmConfig = JsonConvert.DeserializeObject<VmConfig>(jsonResponse);

Console.WriteLine($"VM: {vmConfig.Name}");
Console.WriteLine($"Memory: {vmConfig.Memory} MB");
Console.WriteLine($"Cores: {vmConfig.Cores}");

// Serialize model to JSON
var newConfig = new VmConfig
{
    Name = "new-vm",
    Memory = 4096,
    Cores = 4,
    OnBoot = true
};

string json = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
Console.WriteLine(json);
```

### Using Enums

```csharp
using Corsinvest.ProxmoxVE.Api.Shared;

// Filter resources by type
var resourceType = ClusterResourceType.Qemu;
var resources = GetClusterResources(); // Your method to get resources

var vms = resources.Where(r => r.Type == resourceType.ToString().ToLower());

// Check VM status
if (vmStatus.Status == VmStatus.Running.ToString().ToLower())
{
    Console.WriteLine("VM is running");
}
```

### Using Utilities

```csharp
using Corsinvest.ProxmoxVE.Api.Shared.Utils;

// Format timestamps
long unixTimestamp = 1640995200; // Example timestamp
DateTime date = unixTimestamp.FromUnixTime();
Console.WriteLine($"Snapshot created: {date:yyyy-MM-dd HH:mm:ss}");

// Format file sizes
long diskSize = 53687091200; // 50GB in bytes
Console.WriteLine($"Disk size: {diskSize.ToHumanReadableSize()}"); // "50.00 GB"

// Calculate usage percentages
long usedMemory = 2147483648; // 2GB
long totalMemory = 4294967296; // 4GB
double percentage = usedMemory.ToPercentage(totalMemory);
Console.WriteLine($"Memory usage: {percentage:F1}%"); // "50.0%"
```

---

## Integration with Other Packages

### With Core API

```csharp
using Corsinvest.ProxmoxVE.Api;
using Corsinvest.ProxmoxVE.Api.Shared.Models;

var client = new PveClient("pve.example.com");
await client.Login("admin@pve", "password");

// Get raw response
var result = await client.Nodes["pve1"].Qemu[100].Config.VmConfig();

if (result.IsSuccessStatusCode)
{
    // Convert to strongly-typed model
    var vmConfig = JsonConvert.DeserializeObject<VmConfig>(
        JsonConvert.SerializeObject(result.Response.data));

    Console.WriteLine($"VM: {vmConfig.Name} - Memory: {vmConfig.Memory} MB");
}
```

### With Extension Package

```csharp
using Corsinvest.ProxmoxVE.Api.Extension;
using Corsinvest.ProxmoxVE.Api.Shared.Models;

// Extension package often uses Shared models internally
var nodes = await client.Nodes.Get(); // Returns IEnumerable<NodeInfo>

foreach (var node in nodes)
{
    // NodeInfo is defined in the Shared package
    var memoryUsage = node.Mem.ToPercentage(node.MaxMem);
    Console.WriteLine($"Node {node.Node}: {memoryUsage:F1}% memory used");
}
```

---

## Best Practices

### **Model Usage**

```csharp
// Use strongly-typed models for better code reliability
var vmConfig = JsonConvert.DeserializeObject<VmConfig>(jsonData);
if (vmConfig.Memory < 1024)
{
    Console.WriteLine("Low memory configuration");
}

// Instead of working with dynamic objects
dynamic config = JsonConvert.DeserializeObject(jsonData);
if (config.memory < 1024) // No compile-time checking
{
    Console.WriteLine("Low memory configuration");
}
```

### **Utility Methods**

```csharp
// Use utility extension methods for common formatting
Console.WriteLine($"Disk usage: {diskUsed.ToHumanReadableSize()} / {diskTotal.ToHumanReadableSize()}");
Console.WriteLine($"Memory: {memUsed.ToPercentage(memTotal):F1}%");

// Use constants instead of magic strings
if (resource.Type == PveConstants.ResourceTypeQemu)
{
    // Handle VM
}
```
