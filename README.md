# EnterpriseVE.ProxmoxVE.Api
ProxmoVE Client API .Net

[ProxmoxVE Api](https://pve.proxmox.com/pve-docs/api-viewer/)

[Nuget](https://www.nuget.org/packages/EnterpriseVE.ProxmoxVE.Api) 

```
    ______      __                       _              _    ________
   / ____/___  / /____  _________  _____(_)_______     | |  / / ____/
  / __/ / __ \/ __/ _ \/ ___/ __ \/ ___/ / ___/ _ \    | | / / __/
 / /___/ / / / /_/  __/ /  / /_/ / /  / (__  )  __/    | |/ / /___
/_____/_/ /_/\__/\___/_/  / .___/_/  /_/____/\___/     |___/_____/
                         /_/

                                                       (Made in Italy)
```

# General
The client is generated from a JSON Api on ProxmoxVE. 

# Result
The result is dynamic [ExpandoObject](https://msdn.microsoft.com/en-US/library/system.dynamic.expandoobject(v=vs.110).aspx) and  contains more property:
- returned from ProxmoxVE (data,errors,...) 
- **InError** (bool) : Contains errors.
- **Response** (ExpandoObject): response Http request.
  - **StatusCode** (System.Net.HttpStatusCode): Status code of the HTTP response.
  - **ReasonPhrase** (string): The reason phrase which typically is sent by servers together with the status code.
  - **IsSuccessStatusCode** (bool) : Gets a value that indicates if the HTTP response was successful.
  

Example:

With errors:
```json
{
  "errors": {
    "snapname": "invalid format - invalid configuration ID 'Test 2311'\n"
  },
  "data": null,
  "Response": {
    "StatusCode": 400,
    "ReasonPhrase": "Parameter verification failed.",
    "IsSuccessStatusCode": false
  },
  "InError": true
}
```

Normal result
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
  },
  "Response": {
    "StatusCode": 200,
    "ReasonPhrase": "OK",
    "IsSuccessStatusCode": true
  },  
  "InError": false
}
```

# Usage

```c#
var client = new Client("10.92.90.91");
if (client.Login("root", "password"))
{
    var vm = client.Nodes["pve1"].Qemu[100];

    //config vm 
    var config = vm.Config.VmConfig();
    Console.WriteLine(Client.ObjectToJson(config));

    //create snapshot
    dynamic ret = vm.Snapshot.Snapshot("pippo2311");

    //update snapshot description
    vm.Snapshot["pippo2311"].Config.UpdateSnapshotConfig("descr");

    //delete snapshot
    vm.Snapshot["pippo2311"].Delsnapshot();

    //list of snapshot 
    foreach (dynamic snapshot in vm.Snapshot.SnapshotList().data)
    {
        Console.WriteLine(Client.ObjectToJson(snapshot));
        Console.WriteLine(snapshot.name);
    }
}
```