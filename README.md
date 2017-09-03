# EnterpriseVE.ProxmoxVE.Api
ProxmoVE Client API DotNet

[ProxmoxVE Api](https://pve.proxmox.com/pve-docs/api-viewer/)

[Nuget] (https://www.nuget.org/packages/EnterpriseVE.ProxmoxVE.Api) 

```
    ______      __                       _              _    ________
   / ____/___  / /____  _________  _____(_)_______     | |  / / ____/
  / __/ / __ \/ __/ _ \/ ___/ __ \/ ___/ / ___/ _ \    | | / / __/
 / /___/ / / / /_/  __/ /  / /_/ / /  / (__  )  __/    | |/ / /___
/_____/_/ /_/\__/\___/_/  / .___/_/  /_/____/\___/     |___/_____/
                         /_/

                                                       (Made in Italy)
```

#General

The client is generated from a JSON Api on ProxmoxVE. 

# Usage

```c#
var client = new Client("10.92.90.91");
client.Login("root", "password");

foreach (dynamic item in client.Nodes["pve1"].Qemu[100].Snapshot.SnapshotList())
{
    Console.WriteLine(item.name);
    Console.WriteLine(Client.ObjectToJson(item.name));
}
```