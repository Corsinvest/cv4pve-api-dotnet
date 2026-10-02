# <img src="https://raw.githubusercontent.com/Corsinvest/cv4pve-api-dotnet/master/icon.png" alt="" height="36" align="top"> cv4pve-api-dotnet

```
   ______                _                      __
  / ____/___  __________(_)___ _   _____  _____/ /_
 / /   / __ \/ ___/ ___/ / __ \ | / / _ \/ ___/ __/
/ /___/ /_/ / /  (__  ) / / / / |/ /  __(__  ) /_
\____/\____/_/  /____/_/_/ /_/|___/\___/____/\__/

Proxmox VE API Client for .NET (Made in Italy)
```

[![License](https://img.shields.io/github/license/Corsinvest/cv4pve-api-dotnet.svg?style=flat-square)](https://github.com/Corsinvest/cv4pve-api-dotnet/blob/master/LICENSE.md)
[![.NET](https://img.shields.io/badge/.NET-8.0%2B-blue?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api)
[![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api)

> **The Proxmox VE API from .NET**: a client with a method for every endpoint of the Proxmox VE API, running in your application and talking only to the API.
>
> **[Documentation](https://corsinvest.github.io/cv4pve-api-dotnet/)**

---

<p align="center">
  <img src="https://raw.githubusercontent.com/Corsinvest/cv4pve-api-dotnet/master/docs/src/assets/dotnet.svg" alt=".NET logo" width="120">
</p>

## Why

An application that manages Proxmox VE (a customer portal, a scheduled job, a monitoring or billing tool) has to speak its REST API: tickets and tokens, paths, parameters, JSON, tasks that end later. Written by hand it is a layer of HTTP code to build and to keep up with every Proxmox VE release.

cv4pve-api-dotnet is that layer, generated from the API itself. The calls follow the tree of the API, so the [Proxmox VE API viewer](https://pve.proxmox.com/pve-docs/api-viewer/) is also the reference of the client.

It **runs in your application and uses only the Proxmox VE API**: nothing to install on the nodes, no SSH.

---

## Features

- **The whole API**: a method for every endpoint and HTTP method, generated from the Proxmox VE API schema; `/nodes/{node}/qemu/{vmid}/config` is `client.Nodes["pve01"].Qemu[100].Config`.
- **One Result for every call**: the HTTP outcome and the Proxmox VE data, read as dynamic, as a dictionary or as a typed model. A failed call does not throw.
- **API token or password**: with two-factor authentication, OpenID, your own `HttpClient`, and a list of nodes to try.
- **Tasks**: start a backup, a clone or a migration, wait for its task and read whether it succeeded.
- **VMs by id, name or pattern**: the Extension package finds VMs and containers by id, name, range, node, pool or tag, with exclusions.
- **Typed models**: the Shared package has the classes for the data of cluster, nodes, VMs, containers, storage and access.
- **Command line tools**: the Console package gives a tool the connection options and the behaviour of the cv4pve suite.
- **Cross-platform**: .NET 8, 9 and 10 on Windows, Linux and macOS.

---

## Quick start

```bash
dotnet add package Corsinvest.ProxmoxVE.Api
```

```csharp
using Corsinvest.ProxmoxVE.Api;

// connect to any node of the cluster, with an API token
var client = new PveClient("pve01")
{
    ApiToken = "automation@pve!app=aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
};

// GET /nodes/{node}/qemu/{vmid}/status/current
var result = await client.Nodes["pve01"].Qemu[100].Status.Current.VmStatus();

Console.WriteLine(result.IsSuccessStatusCode
                    ? $"VM {result.Response.data.vmid} is {result.Response.data.status}"
                    : $"{(int)result.StatusCode} {result.ReasonPhrase}");
```

With the Extension package, without knowing the node:

```csharp
using Corsinvest.ProxmoxVE.Api.Extension;

// every guest with the tag "production", except VM 105
foreach (var vm in await client.GetVmsAsync("@tag-production,-105"))
{
    Console.WriteLine($"{vm.VmId} {vm.Name} {vm.Node} {vm.Status}");
}
```

What the token needs: [Permissions](https://corsinvest.github.io/cv4pve-api-dotnet/permissions/).

---

## Package suite

| Package | Version | Downloads | Description |
|---------|---------|-----------|-------------|
| [Corsinvest.ProxmoxVE.Api](https://corsinvest.github.io/cv4pve-api-dotnet/packages/api/) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api) | The client: a method for every endpoint of the Proxmox VE API. |
| [Corsinvest.ProxmoxVE.Api.Extension](https://corsinvest.github.io/cv4pve-api-dotnet/packages/extension/) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Extension.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Extension) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Extension.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Extension) | Typed reads, VMs and containers by id, name or pattern, power, snapshots, the API shell. |
| [Corsinvest.ProxmoxVE.Api.Shared](https://corsinvest.github.io/cv4pve-api-dotnet/packages/shared/) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Shared.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Shared) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Shared.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Shared) | The typed models of the Proxmox VE data, tables and utilities. |
| [Corsinvest.ProxmoxVE.Api.Console](https://corsinvest.github.io/cv4pve-api-dotnet/packages/console/) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Console.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Console) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Console.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Console) | Helpers to build a command line tool for Proxmox VE. |
| [Corsinvest.ProxmoxVE.Api.Metadata](https://corsinvest.github.io/cv4pve-api-dotnet/packages/metadata/) | [![NuGet](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Metadata.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Metadata) | [![Downloads](https://img.shields.io/nuget/dt/Corsinvest.ProxmoxVE.Api.Metadata.svg?style=flat-square)](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Metadata) | The schema of the Proxmox VE API as objects. |

The first two numbers of the version are the Proxmox VE version the client was generated from: 9.2.x is for Proxmox VE 9.2.

---

## Documentation

| | |
|---|---|
| [Getting started](https://corsinvest.github.io/cv4pve-api-dotnet/getting-started/) | Install, connect, first calls |
| [Connection](https://corsinvest.github.io/cv4pve-api-dotnet/connection/) | API token or password, two-factor authentication, OpenID, certificates, timeout, custom HttpClient |
| [Permissions](https://corsinvest.github.io/cv4pve-api-dotnet/permissions/) | The user, the token and the privileges an application needs |
| [Concepts](https://corsinvest.github.io/cv4pve-api-dotnet/concepts/api-structure/) | API structure, results, indexed parameters, tasks, errors |
| [Packages](https://corsinvest.github.io/cv4pve-api-dotnet/packages/api/) | What each package adds |
| [Examples](https://corsinvest.github.io/cv4pve-api-dotnet/examples/common-tasks/) | Common tasks, creating a VM, bulk operations |
| [Troubleshooting](https://corsinvest.github.io/cv4pve-api-dotnet/troubleshooting/) | Logging and the common errors |

---

## Related tools

Prefer a command line? [cv4pve-cli](https://github.com/Corsinvest/cv4pve-cli) calls the same API from any shell. From PowerShell: [cv4pve-api-powershell](https://github.com/Corsinvest/cv4pve-api-powershell). The whole suite: [corsinvest.it/cv4pve](https://www.corsinvest.it/en/cv4pve/).

---

## Support

Professional support and consulting available through [Corsinvest](https://www.corsinvest.it/en/cv4pve/).

---

Part of [cv4pve](https://www.corsinvest.it/cv4pve) suite | Made with ❤️ in Italy by [Corsinvest](https://www.corsinvest.it)

Copyright © Corsinvest Srl
