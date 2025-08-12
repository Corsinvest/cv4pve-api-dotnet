# Result Handling Guide 📊

Understanding how to work with API responses and the Result class.

## 📋 Result Class

Every API call returns a `Result` object:

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

## ✅ Checking Success

```csharp
var result = await client.Nodes["pve1"].Qemu[100].Config.VmConfig();

if (result.IsSuccessStatusCode)
{
    // Success - process the data
    Console.WriteLine($"VM Name: {result.Response.data.name}");
}
else
{
    // Error - handle the failure
    Console.WriteLine($"Error: {result.GetError()}");
    Console.WriteLine($"Status: {result.StatusCode}");
}
```

## 📊 Accessing Response Data

### 🎯 **Dynamic Access**
```csharp
var result = await vm.Config.VmConfig();
if (result.IsSuccessStatusCode)
{
    var data = result.Response.data;
    Console.WriteLine($"VM Name: {data.name}");
    Console.WriteLine($"Memory: {data.memory}");
    Console.WriteLine($"Cores: {data.cores}");
}
```

### 📋 **Dictionary Access**
```csharp
var result = await vm.Config.VmConfig();
if (result.IsSuccessStatusCode)
{
    var dict = result.ResponseToDictionary();
    foreach (var item in dict)
    {
        Console.WriteLine($"{item.Key}: {item.Value}");
    }
}
```

### 🔧 **Extension Methods**
```csharp
using Corsinvest.ProxmoxVE.Api.Extension;

// Get strongly-typed results
var vmConfig = await client.Nodes["pve1"].Qemu[100].Config.Get();
Console.WriteLine($"VM: {vmConfig.Name} - {vmConfig.Memory} MB");
```

## ⚠️ Error Handling

### 📋 **Basic Error Checking**
```csharp
var result = await vm.Status.Start.VmStart();

if (!result.IsSuccessStatusCode)
{
    Console.WriteLine($"Failed to start VM: {result.GetError()}");
    Console.WriteLine($"HTTP Status: {result.StatusCode} - {result.ReasonPhrase}");
}
```

### 🎯 **Detailed Error Information**
```csharp
var result = await vm.Config.Set(memory: 999999); // Invalid value

if (result.ResponseInError)
{
    Console.WriteLine("Proxmox VE returned an error:");
    Console.WriteLine(result.GetError());
}

if (!result.IsSuccessStatusCode)
{
    Console.WriteLine($"HTTP Error: {result.StatusCode}");
    
    // Check specific status codes
    switch (result.StatusCode)
    {
        case HttpStatusCode.Unauthorized:
            Console.WriteLine("Authentication failed");
            break;
        case HttpStatusCode.Forbidden:
            Console.WriteLine("Permission denied");
            break;
        case HttpStatusCode.BadRequest:
            Console.WriteLine("Invalid request parameters");
            break;
    }
}
```

## 📊 Working with Different Response Types

### 📋 **List Responses**
```csharp
var result = await client.Cluster.Resources.Resources();
if (result.IsSuccessStatusCode)
{
    foreach (var resource in result.Response.data)
    {
        Console.WriteLine($"{resource.type}: {resource.id}");
    }
}

// Using Extension methods
var resources = await client.Cluster.Resources.Get();
foreach (var resource in resources.Where(r => r.Type == "qemu"))
{
    Console.WriteLine($"VM: {resource.Name} ({resource.VmId})");
}
```

### 📝 **Task Responses**
```csharp
// Operations that return task IDs
var result = await vm.Snapshot.Snapshot("backup-snapshot");
if (result.IsSuccessStatusCode)
{
    var taskId = result.Response.data;
    Console.WriteLine($"Task started: {taskId}");
    
    // Monitor task progress...
}
```

### 🖼️ **Image Responses**
```csharp
// Change response type for charts
client.ResponseType = "png";
var chartResult = await client.Nodes["pve1"].Rrd.Rrd("cpu", "day");

if (chartResult.IsSuccessStatusCode)
{
    var base64Image = chartResult.Response;
    Console.WriteLine($"<img src=\"data:image/png;base64,{base64Image}\" />");
}

// Switch back to JSON
client.ResponseType = "json";
```

## 🎯 Best Practices

### ✅ **Always Check Success**
```csharp
// ✅ Good practice
var result = await vm.Status.Start.VmStart();
if (result.IsSuccessStatusCode)
{
    Console.WriteLine("VM started successfully");
}
else
{
    Console.WriteLine($"Failed to start VM: {result.GetError()}");
}

// ❌ Don't ignore errors
await vm.Status.Start.VmStart(); // Missing error handling
```

### 📊 **Use Extension Methods for Type Safety**
```csharp
// ✅ Strongly-typed with IntelliSense
var vmConfig = await client.Nodes["pve1"].Qemu[100].Config.Get();
Console.WriteLine($"Memory: {vmConfig.Memory} MB");

// ❌ Dynamic - prone to runtime errors
var result = await client.Nodes["pve1"].Qemu[100].Config.VmConfig();
Console.WriteLine($"Memory: {result.Response.data.memory}"); // No compile-time checking
```

### 🔧 **Handle Null Values**
```csharp
var result = await vm.Config.VmConfig();
if (result.IsSuccessStatusCode)
{
    var data = result.Response.data;
    
    // ✅ Safe access
    var vmName = data.name ?? "Unnamed VM";
    var description = data.description ?? "No description";
    
    Console.WriteLine($"VM: {vmName} - {description}");
}
```

---

<div align="center">
  <sub>Part of <a href="https://www.cv4pve-tools.com">cv4pve-tools</a> suite | Made with ❤️ in Italy by <a href="https://www.corsinvest.it">Corsinvest</a></sub>
</div>
