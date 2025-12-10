# Common Issues and Solutions

This guide covers common issues, configuration patterns, and solutions when working with the Proxmox VE API in .NET.

---

## Indexed Parameters (IDictionary)

Many VM/CT configuration methods use indexed parameters represented as `IDictionary<int, string>` where the key is the index and the value is the configuration string.

### Understanding Indexed Parameters

Proxmox VE uses indexed parameters for devices that can have multiple instances. In the .NET API, all indexed parameters are represented as `IDictionary<int, string>` where the key is the device index (0, 1, 2...) and the value is the configuration string.

**Common Parameters:**
- **netN** - Network interfaces
- **scsiN** / **virtioN** / **sataN** / **ideN** - Disk devices
- **ipconfigN** - Cloud-init network configuration
- **hostpciN** / **usbN** - Hardware passthrough
- **mpN** - LXC mount points (containers only)

> **Note:** Proxmox VE supports many other indexed parameters. All use the same `IDictionary<int, string>` pattern. For a complete list, refer to the [Proxmox VE API Documentation](https://pve.proxmox.com/pve-docs/api-viewer/).

### Basic Usage

```csharp
using Corsinvest.ProxmoxVE.Api;

var client = new PveClient("pve.example.com");
await client.Login("root@pam", "password");

// Configure network interfaces
var networks = new Dictionary<int, string>
{
    { 0, "model=virtio,bridge=vmbr0,firewall=1" },
    { 1, "model=e1000,bridge=vmbr1" }
};

// Configure disks
var disks = new Dictionary<int, string>
{
    { 0, "local-lvm:32,cache=writethrough" },
    { 1, "local-lvm:64,iothread=1" }
};

await client.Nodes["pve1"].Qemu[100].Config.UpdateVm(
    netN: networks,
    scsiN: disks
);
```

---

## Network Configuration (netN)

### Network Interface Syntax

Format: `model=<model>,bridge=<bridge>[,option=value,...]`

### Common Parameters

| Parameter | Description | Example Values |
|-----------|-------------|----------------|
| model | Network card model | virtio, e1000, rtl8139, vmxnet3 |
| bridge | Bridge to connect to | vmbr0, vmbr1 |
| firewall | Enable firewall | 0, 1 |
| link_down | Disconnect interface | 0, 1 |
| macaddr | MAC address | A2:B3:C4:D5:E6:F7 |
| mtu | MTU size | 1500, 9000 |
| queues | Number of queues | 1, 2, 4, 8 |
| rate | Rate limit (MB/s) | 10, 100 |
| tag | VLAN tag | 100, 200 |
| trunks | VLAN trunks | 10;20;30 |

### Examples

```csharp
// Basic VirtIO network
var networks = new Dictionary<int, string>
{
    { 0, "model=virtio,bridge=vmbr0" }
};

// Network with VLAN and firewall
var networks = new Dictionary<int, string>
{
    { 0, "model=virtio,bridge=vmbr0,tag=100,firewall=1" }
};

// Multiple networks with different settings
var networks = new Dictionary<int, string>
{
    { 0, "model=virtio,bridge=vmbr0,firewall=1" },
    { 1, "model=e1000,bridge=vmbr1,rate=100" },
    { 2, "model=virtio,bridge=vmbr0,tag=200,queues=4" }
};
```

---

## Disk Configuration

### Disk Syntax

Format: `<storage>:<size>[,option=value,...]`

Or for existing volumes: `<storage>:<volume>[,option=value,...]`

### Storage Types

- **scsiN** - SCSI disks (0-30), most common, supports all features
- **virtioN** - VirtIO disks (0-15), high performance
- **sataN** - SATA disks (0-5), legacy compatibility
- **ideN** - IDE disks (0-3), legacy, often used for CD-ROM
- **efidisk0** - EFI disk for UEFI boot

### Common Disk Parameters

| Parameter | Description | Example Values |
|-----------|-------------|----------------|
| cache | Cache mode | none, writethrough, writeback, directsync, unsafe |
| discard | Enable TRIM/discard | on, ignore |
| iothread | Enable IO thread | 0, 1 |
| ssd | SSD emulation | 0, 1 |
| backup | Include in backup | 0, 1 |
| replicate | Enable replication | 0, 1 |
| media | Media type | disk, cdrom |
| size | Disk size | 32G, 100G, 1T |

### SCSI Disk Examples

```csharp
// Basic SCSI disk - 32GB
var disks = new Dictionary<int, string>
{
    { 0, "local-lvm:32" }
};

// SCSI disk with options
var disks = new Dictionary<int, string>
{
    { 0, "local-lvm:32,cache=writethrough,iothread=1,discard=on" }
};

// Multiple SCSI disks
var disks = new Dictionary<int, string>
{
    { 0, "local-lvm:32,cache=writethrough,iothread=1" },  // OS disk
    { 1, "local-lvm:100,cache=none,iothread=1,discard=on" },  // Data disk
    { 2, "local-lvm:200,backup=0" }  // Temp disk, no backup
};
```

### VirtIO Disk Examples

```csharp
// VirtIO disks for maximum performance
var disks = new Dictionary<int, string>
{
    { 0, "local-lvm:32,cache=writethrough,discard=on" },
    { 1, "ceph-storage:100,cache=none,iothread=1" }
};
```

### SATA/IDE Examples

```csharp
// SATA disk
var sataDisks = new Dictionary<int, string>
{
    { 0, "local-lvm:32" }
};

// IDE CD-ROM
var ideDisks = new Dictionary<int, string>
{
    { 2, "local:iso/ubuntu-22.04.iso,media=cdrom" }
};
```

### EFI Disk

```csharp
// EFI disk for UEFI boot
string efidisk = "local-lvm:1,efitype=4m,pre-enrolled-keys=0";

await client.Nodes["pve1"].Qemu[100].Config.UpdateVm(
    bios: "ovmf",
    efidisk0: efidisk
);
```

---

## Cloud-Init Configuration (ipconfigN)

### IP Configuration Syntax

Format: `ip=<address>,gw=<gateway>[,option=value,...]`

### Examples

```csharp
// DHCP on all interfaces
var ipconfig = new Dictionary<int, string>
{
    { 0, "ip=dhcp" }
};

// Static IP configuration
var ipconfig = new Dictionary<int, string>
{
    { 0, "ip=192.168.1.100/24,gw=192.168.1.1" }
};

// Multiple interfaces with different configs
var ipconfig = new Dictionary<int, string>
{
    { 0, "ip=192.168.1.100/24,gw=192.168.1.1" },  // Management
    { 1, "ip=10.0.0.100/24" },  // Internal network
    { 2, "ip=dhcp" }  // External network via DHCP
};

// IPv6 with auto-configuration
var ipconfig = new Dictionary<int, string>
{
    { 0, "ip=192.168.1.100/24,gw=192.168.1.1,ip6=auto" }
};
```

---

## Complete Example

### Linux VM with VirtIO and Cloud-Init

```csharp
var client = new PveClient("pve.example.com");
await client.Login("admin@pve", "password");

// VM identifiers
int vmid = 101;
string vmName = "ubuntu-server";
string node = "pve1";

// Hardware resources
int memory = 4096;  // 4GB RAM
int cores = 2;
int sockets = 1;

// Configure VirtIO disks
var disks = new Dictionary<int, string>
{
    { 0, "local-lvm:32,cache=writethrough,discard=on" }
};

// Configure network interfaces
var networks = new Dictionary<int, string>
{
    { 0, "model=virtio,bridge=vmbr0,firewall=1" }
};

// Cloud-init IP configuration
var ipconfig = new Dictionary<int, string>
{
    { 0, "ip=192.168.1.100/24,gw=192.168.1.1" }
};

// OS and boot settings
string ostype = "l26";
string scsihw = "virtio-scsi-single";
string boot = "order=virtio0";
string agent = "enabled=1";

// Cloud-init credentials and network
string ciuser = "admin";
string cipassword = "SecurePassword123!";
string sshkeys = "ssh-rsa AAAAB3NzaC1yc2E...";
string nameserver = "8.8.8.8 8.8.4.4";
string searchdomain = "example.com";

var result = await client.Nodes[node].Qemu.CreateVm(
    vmid: vmid,
    name: vmName,
    memory: memory,
    cores: cores,
    sockets: sockets,
    ostype: ostype,
    virtioN: disks,
    netN: networks,
    ipconfigN: ipconfig,
    scsihw: scsihw,
    boot: boot,
    agent: agent,
    ciuser: ciuser,
    cipassword: cipassword,
    sshkeys: sshkeys,
    nameserver: nameserver,
    searchdomain: searchdomain
);

Console.WriteLine($"VM {vmid} created successfully with cloud-init!");
```

---

## Common Troubleshooting

### VM Won't Start

**Check configuration:**
```csharp
var result = await client.Nodes["pve1"].Qemu[100].Config.VmConfig();
// Verify configuration is valid
Console.WriteLine(result.Response.data);
```

**Common issues:**
- Missing boot disk: Verify `boot` parameter points to valid disk
- Invalid network bridge: Check bridge exists on node
- Insufficient resources: Verify memory/CPU allocation

### Disk Not Found

Verify storage exists and has space:
```csharp
var storages = await client.Nodes["pve1"].Storage.Index();
foreach (var storage in storages.Response.data)
{
    Console.WriteLine($"Storage: {storage.storage}");
    Console.WriteLine($"  Type: {storage.type}");
    Console.WriteLine($"  Available: {storage.avail}");
}
```

### Network Issues

Verify bridge configuration:
```csharp
var networks = await client.Nodes["pve1"].Network.Index();
foreach (var net in networks.Response.data)
{
    if (net.type == "bridge")
    {
        Console.WriteLine($"Bridge: {net.iface}");
    }
}
```

---

For more details on specific parameters and options, refer to the [Proxmox VE API Documentation](https://pve.proxmox.com/pve-docs/api-viewer/).
