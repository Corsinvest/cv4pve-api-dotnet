# Corsinvest.ProxmoxVE.Api 🔧

<div align="center">

[![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api)
[![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)

**🚀 Core .NET Client for Proxmox VE API**

*The foundation package for all Proxmox VE operations in .NET*

</div>

---

## 📖 Overview

The `Corsinvest.ProxmoxVE.Api` package is the core library that provides direct access to the Proxmox VE REST API. It features a 1:1 mapping with the official API structure, making it intuitive for developers familiar with Proxmox VE.

## 🚀 Installation

```bash
dotnet add package Corsinvest.ProxmoxVE.Api
```

## 🎯 Key Features

- **🌳 Tree Structure** - Mirrors the Proxmox VE API hierarchy exactly
- **⚡ Async/Await** - Full asynchronous support throughout
- **🔧 Auto-Generated** - Generated from official Proxmox VE API documentation
- **📝 IntelliSense** - Complete code completion and documentation
- **🔐 Multiple Auth** - Username/password, API tokens, 2FA support
- **📊 Flexible Results** - Dynamic responses with comprehensive metadata
- **🌐 HttpClient Injection** - Custom HttpClient support for enterprise scenarios
- **🛡️ Enterprise Ready** - SSL validation, timeouts, logging integration

---

## 🏗️ API Structure

The library follows the exact structure of the [Proxmox VE API](https://pve.proxmox.com/pve-docs/api-viewer/):

```csharp
// API Path: /cluster/status
client.Cluster.Status.Status()

// API Path: /nodes/{node}/qemu/{vmid}/config  
client.Nodes["pve1"].Qemu[100].Config.VmConfig()

// API Path: /nodes/{node}/lxc/{vmid}/snapshot
client.Nodes["pve1"].Lxc[101].Snapshot.Snapshot("snap-name")

// API Path: /nodes/{node}/storage/{storage}
client.Nodes["pve1"].Storage["local"].Status()
```

### 🔧 HTTP Method Mapping

| HTTP Method | C# Method | Purpose | Example |
|-------------|-----------|---------|---------|
| `GET` | `await resource.Get()` | Retrieve information | `await vm.Config.Get()` |
| `POST` | `await resource.Create(parameters)` | Create resources | `await vm.Snapshot.Create("snap-name")` |
| `PUT` | `await resource.Set(parameters)` | Update resources | `await vm.Config.Set(memory: 4096)` |
| `DELETE` | `await resource.Delete()` | Remove resources | `await vm.Delete()` |

> **Note:** Some endpoints also have specific method names like `VmConfig()`, `Snapshot()`, etc. that map to the appropriate HTTP verbs.

---

## 🔐 Authentication

### 🔑 Username/Password Authentication

```csharp
using Corsinvest.ProxmoxVE.Api;

var client = new PveClient("pve.example.com");

// Basic login
bool success = await client.Login("root", "password");

// Login with realm
bool success = await client.Login("admin@pve", "password");

// Two-factor authentication
bool success = await client.Login("root", "password", "123456");
```

### 🎯 API Token Authentication (Recommended)

```csharp
var client = new PveClient("pve.example.com");

// Set API token (Proxmox VE 6.2+)
client.ApiToken = "user@realm!tokenid=uuid";

// No login() call needed with API tokens
var version = await client.Version.Version();
```

### ⚙️ Advanced Configuration

```csharp
// Basic configuration
var client = new PveClient("pve.example.com")
{
    // Custom timeout (default: 100 seconds)
    Timeout = TimeSpan.FromMinutes(5),
    
    // Validate SSL certificates (default: false)  
    ValidateCertificate = true,
    
    // Response type: "json" or "png" (for charts)
    ResponseType = "json"
};

// Custom HttpClient injection (for enterprise scenarios)
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");

var client = new PveClient("pve.example.com", httpClient)
{
    ValidateCertificate = true
};

// HttpClient with custom configuration
var handler = new HttpClientHandler()
{
    // Custom SSL handling
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => 
    {
        // Custom certificate validation logic
        return ValidateCustomCertificate(cert);
    },
    
    // Proxy configuration
    Proxy = new WebProxy("http://proxy.company.com:8080"),
    UseProxy = true
};

var httpClient = new HttpClient(handler)
{
    Timeout = TimeSpan.FromMinutes(10)
};

var client = new PveClient("pve.example.com", httpClient);
```

### 🏢 Enterprise HttpClient Scenarios

<details>
<summary><strong>🔐 Custom Certificate Validation</strong></summary>

```csharp
// Custom certificate validation for corporate environments
var handler = new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    {
        // Allow specific certificate thumbprints
        var allowedThumbprints = new[]
        {
            "A1:B2:C3:D4:E5:F6:...", // Production cert
            "F6:E5:D4:C3:B2:A1:..."  // Staging cert
        };
        
        return allowedThumbprints.Contains(cert.GetCertHashString());
    }
};

var httpClient = new HttpClient(handler);
var client = new PveClient("pve.company.com", httpClient);
```

</details>

<details>
<summary><strong>🌐 Proxy Configuration</strong></summary>

```csharp
// Corporate proxy setup
var proxy = new WebProxy("http://proxy.company.com:8080")
{
    // Proxy authentication if required
    Credentials = new NetworkCredential("proxyuser", "proxypass")
};

var handler = new HttpClientHandler()
{
    Proxy = proxy,
    UseProxy = true
};

var httpClient = new HttpClient(handler);
var client = new PveClient("pve.company.com", httpClient);
```

</details>

<details>
<summary><strong>📊 Request/Response Logging</strong></summary>

```csharp
// Custom logging handler for debugging
public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger _logger;
    
    public LoggingHandler(ILogger logger)
    {
        _logger = logger;
        InnerHandler = new HttpClientHandler();
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Request: {request.Method} {request.RequestUri}");
        
        var response = await base.SendAsync(request, cancellationToken);
        
        _logger.LogDebug($"Response: {response.StatusCode}");
        
        return response;
    }
}

// Usage
var httpClient = new HttpClient(new LoggingHandler(logger));
var client = new PveClient("pve.example.com", httpClient);
```

</details>

<details>
<summary><strong>🔄 Retry Policies with Polly</strong></summary>

```csharp
using Polly;
using Polly.Extensions.Http;

// Retry policy for resilient HTTP calls
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
    );

var httpClient = new HttpClient();
// Apply retry policy via HttpClientFactory or custom handler

var client = new PveClient("pve.example.com", httpClient);
```

</details>

---

## 📊 Working with Results

Every API call returns a `Result` object containing comprehensive response information:

```csharp
var result = await client.Nodes["pve1"].Qemu[100].Config.VmConfig();

// Check success
if (result.IsSuccessStatusCode)
{
    // Access response data (dynamic object)
    Console.WriteLine($"VM Name: {result.Response.data.name}");
    Console.WriteLine($"Memory: {result.Response.data.memory}");
    Console.WriteLine($"Cores: {result.Response.data.cores}");
    
    // Convert to dictionary for easier access
    var dict = result.ResponseToDictionary();
    foreach (var item in dict)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
    }
}
else
{
    // Handle errors
    Console.WriteLine($"Error: {result.GetError()}");
    Console.WriteLine($"Status: {result.StatusCode} - {result.ReasonPhrase}");
}
```

### 📋 Result Properties

```csharp
public class Result
{
    // Response data from Proxmox VE (dynamic ExpandoObject)
    public dynamic Response { get; set; }
    
    // HTTP response information
    public HttpStatusCode StatusCode { get; set; }
    public string ReasonPhrase { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    
    // Utility properties and methods
    public bool ResponseInError { get; }
    public IDictionary<string, object> ResponseToDictionary();
    public string GetError();
}
```

---

## 🔧 Basic Examples

### 🖥️ Virtual Machine Management

<details>
<summary><strong>📊 VM Configuration</strong></summary>

```csharp
using Corsinvest.ProxmoxVE.Api;

var client = new PveClient("pve.example.com");
await client.Login("admin@pve", "password");

// Get VM configuration
var vm = client.Nodes["pve1"].Qemu[100];
var config = await vm.Config.VmConfig();

if (config.IsSuccessStatusCode)
{
    var vmData = config.Response.data;
    Console.WriteLine($"VM Name: {vmData.name}");
    Console.WriteLine($"Memory: {vmData.memory} MB");
    Console.WriteLine($"CPUs: {vmData.cores}");
    Console.WriteLine($"OS Type: {vmData.ostype}");
}

// Update VM configuration
var updateResult = await vm.Config.Set(
    memory: 8192,  // 8GB RAM
    cores: 4       // 4 CPU cores
);
// OR using specific method
var updateResult = await vm.Config.UpdateVm(
    memory: 8192,
    cores: 4
);

if (updateResult.IsSuccessStatusCode)
{
    Console.WriteLine("✅ VM configuration updated!");
}
```

</details>

<details>
<summary><strong>📸 Snapshot Management</strong></summary>

```csharp
// Create snapshot
var snapshot = await client.Nodes["pve1"].Qemu[100]
    .Snapshot.Create("backup-before-update", description: "Pre-update backup");
// OR using specific method
var snapshot = await client.Nodes["pve1"].Qemu[100]
    .Snapshot.Snapshot("backup-before-update", "Pre-update backup");

if (snapshot.IsSuccessStatusCode)
{
    Console.WriteLine("✅ Snapshot created successfully!");
}

// List snapshots
var snapshots = await client.Nodes["pve1"].Qemu[100]
    .Snapshot.SnapshotList();

if (snapshots.IsSuccessStatusCode)
{
    Console.WriteLine("📸 Available snapshots:");
    foreach (var snap in snapshots.Response.data)
    {
        Console.WriteLine($"  - {snap.name}: {snap.description} ({snap.snaptime})");
    }
}

// Delete snapshot
var deleteResult = await client.Nodes["pve1"].Qemu[100]
    .Snapshot["backup-before-update"].Delsnapshot();

if (deleteResult.IsSuccessStatusCode)
{
    Console.WriteLine("🗑️ Snapshot deleted successfully!");
}
```

</details>

<details>
<summary><strong>🔄 VM Status Management</strong></summary>

```csharp
var vm = client.Nodes["pve1"].Qemu[100];

// Get current status
var status = await vm.Status.Current.VmStatus();
Console.WriteLine($"Current status: {status.Response.data.status}");
Console.WriteLine($"CPU usage: {status.Response.data.cpu:P2}");
Console.WriteLine($"Memory usage: {status.Response.data.mem / status.Response.data.maxmem:P2}");

// Start VM
if (status.Response.data.status == "stopped")
{
    var startResult = await vm.Status.Start.VmStart();
    if (startResult.IsSuccessStatusCode)
    {
        Console.WriteLine("🚀 VM started successfully!");
    }
}

// Stop VM
var stopResult = await vm.Status.Stop.VmStop();
if (stopResult.IsSuccessStatusCode)
{
    Console.WriteLine("⏹️ VM stopped successfully!");
}

// Restart VM
var restartResult = await vm.Status.Reboot.VmReboot();
if (restartResult.IsSuccessStatusCode)
{
    Console.WriteLine("🔄 VM restarted successfully!");
}
```

</details>

### 📦 Container Management

<details>
<summary><strong>🐳 LXC Container Operations</strong></summary>

```csharp
// Access LXC container
var container = client.Nodes["pve1"].Lxc[101];

// Get container configuration
var config = await container.Config.VmConfig();
if (config.IsSuccessStatusCode)
{
    var ctData = config.Response.data;
    Console.WriteLine($"Container: {ctData.hostname}");
    Console.WriteLine($"OS Template: {ctData.ostemplate}");
    Console.WriteLine($"Memory: {ctData.memory} MB");
}

// Container status operations
var status = await container.Status.Current.VmStatus();
Console.WriteLine($"Status: {status.Response.data.status}");

// Start container
if (status.Response.data.status == "stopped")
{
    await container.Status.Start.VmStart();
    Console.WriteLine("🚀 Container started!");
}

// Create container snapshot
var snapshot = await container.Snapshot.Snapshot("backup-snapshot");
if (snapshot.IsSuccessStatusCode)
{
    Console.WriteLine("📸 Container snapshot created!");
}
```

</details>

### 🏗️ Cluster Operations

<details>
<summary><strong>📊 Cluster Status and Resources</strong></summary>

```csharp
// Get cluster status
var clusterStatus = await client.Cluster.Status.Status();
if (clusterStatus.IsSuccessStatusCode)
{
    Console.WriteLine("🏗️ Cluster Status:");
    foreach (var item in clusterStatus.Response.data)
    {
        Console.WriteLine($"  {item.type}: {item.name} - {item.status}");
    }
}

// Get cluster resources
var resources = await client.Cluster.Resources.Resources();
if (resources.IsSuccessStatusCode)
{
    Console.WriteLine("📊 Cluster Resources:");
    foreach (var resource in resources.Response.data)
    {
        if (resource.type == "node")
        {
            Console.WriteLine($"  Node: {resource.node} - CPU: {resource.cpu:P2}, Memory: {resource.mem / resource.maxmem:P2}");
        }
        else if (resource.type == "qemu")
        {
            Console.WriteLine($"  VM: {resource.vmid} ({resource.name}) on {resource.node} - {resource.status}");
        }
    }
}

// Get node information
var nodes = await client.Nodes.Index();
if (nodes.IsSuccessStatusCode)
{
    Console.WriteLine("🖥️ Available Nodes:");
    foreach (var node in nodes.Response.data)
    {
        Console.WriteLine($"  {node.node}: {node.status} - Uptime: {node.uptime}s");
    }
}
```

</details>

### 💾 Storage Management

<details>
<summary><strong>🗄️ Storage Operations</strong></summary>

```csharp
// List storage on a node
var storages = await client.Nodes["pve1"].Storage.Index();
if (storages.IsSuccessStatusCode)
{
    Console.WriteLine("💾 Available Storage:");
    foreach (var storage in storages.Response.data)
    {
        Console.WriteLine($"  {storage.storage}: {storage.type} - {storage.avail / (1024*1024*1024):F2} GB available");
    }
}

// Get specific storage details
var localStorage = await client.Nodes["pve1"].Storage["local"].Status();
if (localStorage.IsSuccessStatusCode)
{
    var storageData = localStorage.Response.data;
    Console.WriteLine($"Storage: {storageData.storage}");
    Console.WriteLine($"Type: {storageData.type}");
    Console.WriteLine($"Total: {storageData.total / (1024*1024*1024):F2} GB");
    Console.WriteLine($"Used: {storageData.used / (1024*1024*1024):F2} GB");
    Console.WriteLine($"Available: {storageData.avail / (1024*1024*1024):F2} GB");
}

// List storage content
var content = await client.Nodes["pve1"].Storage["local"].Content.Index();
if (content.IsSuccessStatusCode)
{
    Console.WriteLine("📁 Storage Content:");
    foreach (var item in content.Response.data)
    {
        Console.WriteLine($"  {item.volid}: {item.format} - {item.size / (1024*1024):F2} MB");
    }
}
```

</details>

---

## 🔧 Advanced Features

### 📈 Response Type Switching

```csharp
// Default JSON responses
client.ResponseType = "json";
var vmConfig = await client.Nodes["pve1"].Qemu[100].Config.VmConfig();

// Switch to PNG for charts/graphs
client.ResponseType = "png";
var cpuChart = await client.Nodes["pve1"].Rrd.Rrd("cpu", "day");
var imageData = cpuChart.Response; // Base64 encoded PNG

// Use in HTML
Console.WriteLine($"<img src=\"data:image/png;base64,{imageData}\" />");

// Switch back to JSON
client.ResponseType = "json";
```

### ⏳ Task Management

```csharp
// Long-running operations return task IDs
var createResult = await client.Nodes["pve1"].Qemu.CreateVm(
    vmid: 999,
    name: "test-vm",
    memory: 2048
);

if (createResult.IsSuccessStatusCode)
{
    var taskId = createResult.Response.data;
    Console.WriteLine($"Task started: {taskId}");
    
    // Monitor task progress
    while (true)
    {
        var taskStatus = await client.Nodes["pve1"].Tasks[taskId].Status.ReadTaskStatus();
        
        if (taskStatus.IsSuccessStatusCode)
        {
            var status = taskStatus.Response.data.status;
            
            if (status == "stopped")
            {
                var exitStatus = taskStatus.Response.data.exitstatus;
                Console.WriteLine($"Task completed with status: {exitStatus}");
                break;
            }
            else if (status == "running")
            {
                Console.WriteLine("Task still running...");
                await Task.Delay(2000);
            }
        }
    }
}
```

### 🔐 SSL and Security

```csharp
var client = new PveClient("pve.example.com")
{
    // Enable SSL certificate validation
    ValidateCertificate = true,
    
    // Set custom timeout
    Timeout = TimeSpan.FromMinutes(10)
};

// Use API token for secure authentication
client.ApiToken = "automation@pve!secure-token=uuid-here";

// API calls now use validated SSL and secure token
var result = await client.Version.Version();
```

---

## 🚀 Best Practices

### ✅ **Recommended Patterns**

```csharp
// 1. Always check IsSuccessStatusCode
var result = await client.Cluster.Status.Status();
if (result.IsSuccessStatusCode)
{
    // Process successful response
    ProcessClusterStatus(result.Response.data);
}
else
{
    // Handle error appropriately
    _logger.LogError($"API call failed: {result.GetError()}");
}

// 2. Use API tokens for automation
var client = new PveClient("pve.cluster.com");
client.ApiToken = Environment.GetEnvironmentVariable("PROXMOX_API_TOKEN");

// 3. Configure timeouts for long operations
client.Timeout = TimeSpan.FromMinutes(15);

// 4. Enable SSL validation in production
client.ValidateCertificate = true;

// 5. Use custom HttpClient for enterprise scenarios
var httpClient = new HttpClient(new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = ValidateCustomCert,
    Proxy = corporateProxy
});
var client = new PveClient("pve.company.com", httpClient);

// 6. Dispose of HttpClient properly (or use HttpClientFactory)
using var httpClient = new HttpClient();
var client = new PveClient("pve.example.com", httpClient);
```

### ❌ **Common Pitfalls to Avoid**

```csharp
// Don't ignore error handling
var result = await client.Nodes["pve1"].Qemu[100].Status.Start.VmStart();
// Missing: if (!result.IsSuccessStatusCode) { ... }

// Don't hardcode credentials
await client.Login("root", "password123"); // Bad
// Better: Use environment variables or secure storage

// Don't assume dynamic properties exist
Console.WriteLine(result.Response.data.nonexistent); // May throw
// Better: Check if property exists or use null-conditional operators
```

---

## 🔗 Related Packages

- **[Corsinvest.ProxmoxVE.Api.Extension](../Extension.md)** - Helper methods and utilities
- **[Corsinvest.ProxmoxVE.Api.Shared](../Shared.md)** - Common models and types
- **[Corsinvest.ProxmoxVE.Api.Shell](../Shell.md)** - Console application helpers

---

## 📞 Support

- **[GitHub Issues](https://github.com/Corsinvest/cv4pve-api-dotnet/issues)** - Bug reports and questions
- **[NuGet Package](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api)** - Download and version info
- **[Commercial Support](https://www.corsinvest.it/cv4pve)** - Professional services

---

<div align="center">
  <sub>Part of the cv4pve-tools suite | Made with ❤️ in Italy 🇮🇹</sub>
</div>
