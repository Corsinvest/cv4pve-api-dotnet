# API Structure Guide 🌳

Understanding the hierarchical structure of the Proxmox VE API and how it maps to the .NET client.

## 🏗️ Tree Structure

The API follows the exact structure of the [Proxmox VE API](https://pve.proxmox.com/pve-docs/api-viewer/):

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

## 🔧 HTTP Method Mapping

| HTTP Method | C# Method | Purpose | Example |
|-------------|-----------|---------|---------|
| `GET` | `await resource.Get()` | Retrieve information | `await vm.Config.Get()` |
| `POST` | `await resource.Create(parameters)` | Create resources | `await vm.Snapshot.Create("snap-name")` |
| `PUT` | `await resource.Set(parameters)` | Update resources | `await vm.Config.Set(memory: 4096)` |
| `DELETE` | `await resource.Delete()` | Remove resources | `await vm.Delete()` |

## 📊 Common Endpoints

### 🏗️ **Cluster Level**
```csharp
client.Cluster.Status.Status()           // GET /cluster/status
client.Cluster.Resources.Resources()     // GET /cluster/resources
client.Version.Version()                 // GET /version
```

### 🖥️ **Node Level**
```csharp
client.Nodes.Index()                     // GET /nodes
client.Nodes["pve1"].Status.Status()     // GET /nodes/pve1/status
client.Nodes["pve1"].Storage.Index()     // GET /nodes/pve1/storage
```

### 🔧 **VM Operations**
```csharp
client.Nodes["pve1"].Qemu[100].Config.VmConfig()        // GET config
client.Nodes["pve1"].Qemu[100].Status.Current.VmStatus() // GET status
client.Nodes["pve1"].Qemu[100].Status.Start.VmStart()   // POST start
client.Nodes["pve1"].Qemu[100].Snapshot.SnapshotList()  // GET snapshots
```

### 📦 **Container Operations**
```csharp
client.Nodes["pve1"].Lxc[101].Config.VmConfig()         // GET config
client.Nodes["pve1"].Lxc[101].Status.Current.VmStatus() // GET status
client.Nodes["pve1"].Lxc[101].Status.Start.VmStart()    // POST start
```

## 🎯 Parameters and Indexers

### 🔢 **Numeric Indexers**
```csharp
client.Nodes["pve1"].Qemu[100]     // VM ID 100
client.Nodes["pve1"].Lxc[101]      // Container ID 101
```

### 📝 **String Indexers**
```csharp
client.Nodes["pve1"]                    // Node name
client.Nodes["pve1"].Storage["local"]   // Storage name
client.Nodes["pve1"].Qemu[100].Snapshot["snap1"] // Snapshot name
```

### ⚙️ **Method Parameters**
```csharp
// Parameters as method arguments
await vm.Config.Set(memory: 4096, cores: 2);

// Parameters as objects
await vm.Snapshot.Snapshot("backup", "Description here");
```

<div align="center">
  <sub>Made with ❤️ in Italy 🇮🇹 by <a href="https://www.corsinvest.it">Corsinvest</a></sub>
</div>
