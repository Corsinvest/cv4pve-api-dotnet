# Corsinvest.ProxmoxVE.Api

[![License](https://img.shields.io/github/license/Corsinvest/cv4pve-api-dotnet.svg)](LICENSE.md) ![Nuget](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.svg?label=Nuget%20%20Api) ![Nuget](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Extension.svg?label=Nuget%20%20Extension) ![Nuget](https://img.shields.io/nuget/v/Corsinvest.ProxmoxVE.Api.Metadata.svg?label=Nuget%20%20Metadata) [![AppVeyor branch](https://img.shields.io/appveyor/ci/franklupo/cv4pve-api-dotnet/master.svg)](https://ci.appveyor.com/project/franklupo/cv4pve-api-dotnet)

Proxmox VE Client API .Net

[Proxmox VE Api](https://pve.proxmox.com/pve-docs/api-viewer/)

[Nuget Api](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api)

[Nuget Extension](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Extension)

[Nuget Metadata](https://www.nuget.org/packages/Corsinvest.ProxmoxVE.Api.Metadata)

```text
   ______                _                      __
  / ____/___  __________(_)___ _   _____  _____/ /_
 / /   / __ \/ ___/ ___/ / __ \ | / / _ \/ ___/ __/
/ /___/ /_/ / /  (__  ) / / / / |/ /  __(__  ) /_
\____/\____/_/  /____/_/_/ /_/|___/\___/____/\__/

Corsinvest for Proxmox VE Api Client  (Made in Italy)
```

## Copyright and License

Copyright: Corsinvest Srl
For licensing details please visit [LICENSE.md](LICENSE.md)

## Commercial Support

This software is part of a suite of tools called cv4pve-tools. If you want commercial support, visit the [site](https://www.corsinvest.it/cv4pve-tools)

## General

The client is generated from a JSON Api on Proxmox VE.

## Main features

* Easy to learn
* Method named
* Method native suffix Rest (prevent)
  * GetRest
  * CreateRest (Post)
  * SetRest (Put)
  * DeleteRest
* Set ResponseType json, png
* Full class and method generated from documentation (about client)
* Comment any method and parameters
* Parameters indexed eg [n] is structured in array index and value
* Tree structure
  * client.Nodes["pve1"].Qemu[100].Snapshot().snapshotList().Response.data
* Return data Proxmox VE
* Debug Level show to console information REST call
* Return result
  * Request
  * Response
  * Status
* Last result action
* Task utility
  * WaitForTaskToFinish
  * TaskIsRunning
  * GetExitStatusTask
* Method direct access
  * Get
  * Create (Post)
  * Set (Put)
  * Delete
* Login return bool if access
* Return Result class more information
* ClientBase lite function

## Result

The result is class **Result** and contain properties:

* **Response** returned from Proxmox VE (data,errors,...) dynamic [ExpandoObject](https://msdn.microsoft.com/en-US/library/system.dynamic.expandoobject(v=vs.110).aspx)
* **ResponseToDictionary** return response to dictionary ```IDictionary<String, object>```
* **ResponseInError** (bool) : Contains errors from Proxmox VE.
* **StatusCode** (System.Net.HttpStatusCode): Status code of the HTTP response.
* **ReasonPhrase** (string): The reason phrase which typically is sent by servers together with the status code.
* **IsSuccessStatusCode** (bool) : Gets a value that indicates if the HTTP response was successful.
* **GetError()** (string) : Get error.

Example result:

```json
{
  "data": {
    "smbios1": "uuid=9246585e-0c8b-4d02-8fe2-f48fd0da3975",
    "ide2": "none,media=cdrom",
    "onboot": 1,
    "boot": "cdn",
    "cores": 2,
    "agent": 1,
    "memory": 4096,
    "numa": 0,
    "bootdisk": "virtio0",
    "sockets": 1,
    "net0": "virtio=3A:39:38:30:36:31,bridge=vmbr0",
    "parent": "auto4hours170904080002",
    "digest": "acafde32daab50bce801fef2e029440c54ebe2f7",
    "vga": "qxl",
    "virtio0": "local-zfs:vm-100-disk-1,cache=writeback,size=50G",
    "ostype": "win8",
    "name": "phenometa"
  }
}
```

## Usage

```C#
var client = new PveClient("10.92.90.91");
if (client.Login("root", "password"))
{
    var vm = client.Nodes["pve1"].Qemu[100];

    //config vm
    var config = vm.Config.VmConfig();
    Console.WriteLine(Client.ObjectToJson(config.Response));

    //create snapshot
    var response = vm.Snapshot.Snapshot("pippo2311");

    //update snapshot description
    vm.Snapshot["pippo2311"].Config.UpdateSnapshotConfig("description");

    //delete snapshot
    vm.Snapshot["pippo2311"].Delsnapshot();

    //list of snapshot
    foreach (var snapshot in vm.Snapshot.SnapshotList().Response.data)
    {
        Console.WriteLine(Client.ObjectToJson(snapshot));
        Console.WriteLine(snapshot.name);
    }

    //change response type from json to png
    client.ResponseType = "png";
    var dataImg = client.Nodes["pve1"].Rrd.Rrd("cpu", "day").Response;
    Console.WriteLine("<img src=\"{dataImg}\" \>");
}
```
