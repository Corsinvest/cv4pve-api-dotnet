# Corsinvest.ProxmoxVE.Api.Metadata

<div align="center">

[![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Metadata.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Metadata)
[![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Metadata.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Metadata)

**Proxmox VE API Documentation Reader**

*Extract structure and metadata from Proxmox VE API documentation*

</div>

---

## Overview

The `Corsinvest.ProxmoxVE.Api.Metadata` package reads the official Proxmox VE API documentation and extracts structural information. This package is primarily used for code generation and API exploration.

## Installation

```bash
dotnet add package Corsinvest.ProxmoxVE.Api.Metadata
```

> **Note:** This package is primarily used by developers working with API generation tools and advanced API exploration.

## Key Features

- **API Documentation Parsing** - Read and parse Proxmox VE API documentation
- **Structure Extraction** - Extract API endpoint structure and hierarchy  
- **Parameter Analysis** - Analyze method parameters and their types
- **Endpoint Discovery** - Discover all available API endpoints
- **Documentation Extraction** - Extract method descriptions and parameter docs
- **Code Generation Support** - Provide metadata for code generation tools

---

## Basic Usage

### Reading API Documentation

```csharp
using Corsinvest.ProxmoxVE.Api.Metadata;

// Create metadata reader
var client = new PveClient("pve.example.com");
await client.Login("admin@pve", "password");

var metadataReader = new ApiMetadataReader(client);

// Load API documentation
var apiDoc = await metadataReader.LoadDocumentationAsync();
Console.WriteLine($"Found {apiDoc.Endpoints.Count} API endpoints");
```

### Exploring API Structure

```csharp
// Get API structure
var structure = await metadataReader.GetApiStructureAsync();

// Browse available paths
foreach (var path in structure.RootPaths)
{
    Console.WriteLine($"Path: {path.Path}");
    
    foreach (var method in path.Methods)
    {
        Console.WriteLine($"  {method.HttpMethod}: {method.Description}");
    }
}
```

### Discovering Endpoints

```csharp
// Get all endpoints
var endpoints = await metadataReader.GetEndpointsAsync();

// Filter by type
var vmEndpoints = endpoints.Where(e => e.Path.Contains("/qemu/"));
var nodeEndpoints = endpoints.Where(e => e.Path.StartsWith("/nodes"));

Console.WriteLine($"VM endpoints: {vmEndpoints.Count()}");
Console.WriteLine($"Node endpoints: {nodeEndpoints.Count()}");
```

---

## Metadata Models

### API Endpoint

```csharp
public class ApiEndpoint
{
    public string Path { get; set; }           // "/nodes/{node}/qemu/{vmid}/config"
    public string Method { get; set; }         // "GET", "POST", "PUT", "DELETE"
    public string Description { get; set; }    // Method description
    public List<ApiParameter> Parameters { get; set; }
    public ApiReturnType ReturnType { get; set; }
}
```

### API Parameter

```csharp
public class ApiParameter
{
    public string Name { get; set; }          // Parameter name
    public string Type { get; set; }          // "string", "integer", "boolean"
    public string Description { get; set; }   // Parameter description
    public bool Required { get; set; }        // Is parameter required
    public object DefaultValue { get; set; }  // Default value if any
}
```

### API Structure

```csharp
public class ApiStructure
{
    public List<ApiPathNode> RootPaths { get; set; }
    public Dictionary<string, ApiResource> Resources { get; set; }
    public string Version { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

---

## Use Cases

### Code Generation

The metadata package is used to generate the main API client code:

- Extract all API endpoints and their parameters
- Generate strongly-typed method signatures
- Create the hierarchical API structure (client.Nodes["pve1"].Qemu[100]...)
- Generate documentation and IntelliSense comments

### Documentation Generation

Generate comprehensive API documentation:

- Create markdown documentation from API metadata
- Generate parameter reference guides
- Build interactive API explorers
- Create code examples for each endpoint

### API Exploration

Discover and explore available API functionality:

- Browse the complete API hierarchy
- Find endpoints by functionality
- Analyze parameter requirements
- Understand return data structures

---

## Integration

### With Core API

The metadata package helps understand what's available in the core API:

```csharp
// Use metadata to discover available VM operations
var vmEndpoints = await metadataReader.GetEndpointsAsync()
    .Where(e => e.Path.Contains("/qemu/"));

foreach (var endpoint in vmEndpoints)
{
    Console.WriteLine($"{endpoint.Method} {endpoint.Path}");
    // Shows: GET /nodes/{node}/qemu/{vmid}/config
    //        POST /nodes/{node}/qemu/{vmid}/snapshot
    //        etc.
}
```

### For Development Tools

The package provides foundation for development tools:

- IDE extensions that provide API completion
- Code generators for other languages
- API testing tools
- Documentation generators

---

## Best Practices

### **Using for Discovery**

```csharp
// Use metadata to understand API capabilities
var endpoints = await metadataReader.GetEndpointsAsync();
var snapshotOps = endpoints.Where(e => e.Path.Contains("snapshot"));

// Check parameter requirements before API calls
foreach (var endpoint in snapshotOps)
{
    var requiredParams = endpoint.Parameters.Where(p => p.Required);
    Console.WriteLine($"Required parameters: {string.Join(", ", requiredParams.Select(p => p.Name))}");
}
```

### **For Code Generation**

```csharp
// Use structured data for generating typed interfaces
var structure = await metadataReader.GetApiStructureAsync();

// Generate method signatures based on actual API structure
// Example: Generate client.Nodes[node].Qemu[vmid].Config.Get()
```
