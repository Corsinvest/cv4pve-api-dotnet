# Corsinvest.ProxmoxVE.Api

```
   ______                _                      __
  / ____/___  __________(_)___ _   _____  _____/ /_
 / /   / __ \/ ___/ ___/ / __ \ | / / _ \/ ___/ __/
/ /___/ /_/ / /  (__  ) / / / / |/ /  __(__  ) /_
\____/\____/_/  /____/_/_/ /_/|___/\___/____/\__/

Proxmox VE API Client for .NET (Made in Italy)
```

[![License](https://img.shields.io/github/license/Corsinvest/cv4pve-api-dotnet.svg?style=flat-square)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0%2B-blue?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

---

## Quick Start

```bash
# Install the main API package
dotnet add package Corsinvest.ProxmoxVE.Api

# Install extension package for additional functionality
dotnet add package Corsinvest.ProxmoxVE.Api.Extension
```

```csharp
using Corsinvest.ProxmoxVE.Api;

var client = new PveClient("your-proxmox-host.com");
if (await client.Login("root", "your-password"))
{
    // Get cluster status
    var status = await client.Cluster.Status.Status();
    Console.WriteLine($"Cluster: {status.Response.data[0].name}");

    // Manage VMs
    var vm = await client.Nodes["pve1"].Qemu[100].Config.VmConfig();
    Console.WriteLine($"VM: {vm.Response.data.name}");
}
```

---

## Package Suite

| Package | Version | Downloads | Description |
|---------|---------|-----------|-------------|
| [Corsinvest.ProxmoxVE.Api](./docs/api.md) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api) | Core .NET client library for Proxmox VE API. Foundation package with complete API coverage. |
| [Corsinvest.ProxmoxVE.Api.Extension](./docs/extension.md) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Extension.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Extension) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Extension.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Extension) | Extension methods and helper utilities for common operations and task management. |
| [Corsinvest.ProxmoxVE.Api.Shared](./docs/shared.md) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Shared.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Shared) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Shared.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Shared) | Shared models, utilities, and common types used across the package suite. |
| [Corsinvest.ProxmoxVE.Api.Console](./docs/console.md) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Console.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Console) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Console.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Console) | Console application helpers for building CLI tools with Proxmox VE. |
| [Corsinvest.ProxmoxVE.Api.Metadata](./docs/metadata.md) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Metadata.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Metadata) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Metadata.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Metadata) | API metadata extraction and documentation generation tools. |

---

## Key Features

### Developer Experience
- **Async/Await** throughout the library
- **Strongly typed** models and responses
- **IntelliSense** support in all IDEs
- **Auto-generated** from official API docs
- **Tree structure** matching Proxmox VE API

### Core Functionality
- **Full API coverage** for Proxmox VE
- **VM/CT management** (create, configure, snapshot)
- **Cluster operations** (status, resources, HA)
- **Storage management** (local, shared, backup)
- **Network configuration** (bridges, VLANs, SDN)

### Enterprise Ready
- **API token** authentication (Proxmox VE 6.2+)
- **Two-factor** authentication support
- **SSL certificate** validation
- **Configurable timeouts** and retry logic
- **Microsoft.Extensions.Logging** integration

### Advanced Features
- **Extension methods** for common operations
- **Task management** utilities
- **Bulk operations** with pattern matching
- **Response type** switching (JSON, PNG)
- **Console application** helpers

---

## Documentation

### Getting Started

- **[Authentication](./docs/authentication.md)** - API tokens and security
- **[Basic Examples](./docs/examples.md)** - Common usage patterns
- **[Advanced Usage](./docs/advanced.md)** - Complex scenarios and best practices
- **[Common Issues](./docs/common-issues.md)** - Configuration patterns and troubleshooting

### API Reference

- **[API Structure](./docs/apistructure.md)** - Understanding the tree structure
- **[Result Handling](./docs/results.md)** - Working with responses
- **[Error Handling](./docs/errorhandling.md)** - Exception management
- **[Task Management](./docs/tasks.md)** - Long-running operations

---

## Examples

### VM Management

```csharp
// Create and configure a VM
var client = new PveClient("pve.example.com");
await client.Login("admin@pve", "password");

var result = await client.Nodes["pve1"].Qemu.CreateVm(
    vmid: 100,
    name: "web-server",
    memory: 4096,
    cores: 2
);

if (result.IsSuccessStatusCode)
{
    Console.WriteLine("VM created successfully!");
}
```

### Cluster Monitoring

```csharp
using Corsinvest.ProxmoxVE.Api.Extension;

// Get cluster overview with extension methods
var nodes = await client.GetNodesAsync();
foreach (var node in nodes)
{
    Console.WriteLine($"Node {node.Node}: CPU {node.CpuUsage:P2}, Memory {node.MemoryUsage:P2}");
}
```

### VM Discovery

```csharp
// Find VMs using patterns (like cv4pve-autosnap)
var productionVms = await client.GetVmsAsync("@tag-production");
var webVms = await client.GetVmsAsync("web%");
var allExceptTest = await client.GetVmsAsync("@all,-@tag-test");
```

---

## Support

Professional support and consulting available through [Corsinvest](https://www.corsinvest.it/cv4pve).

---

Part of [cv4pve](https://www.corsinvest.it/cv4pve) suite | Made with ❤️ in Italy by [Corsinvest](https://www.corsinvest.it)

Copyright © Corsinvest Srl
