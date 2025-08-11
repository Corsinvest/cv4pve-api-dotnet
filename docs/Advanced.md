# Advanced Usage Guide 🚀

This guide covers complex scenarios, best practices, and advanced patterns for experienced developers.

## 🏢 Enterprise Patterns

### 🔧 **Custom HttpClient Configuration**

```csharp
// Enterprise setup with proxy and custom certificates
var handler = new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = ValidateCustomCert,
    Proxy = new WebProxy("http://proxy.company.com:8080"),
    UseProxy = true
};

var httpClient = new HttpClient(handler);
var client = new PveClient("pve.company.com", httpClient)
{
    ValidateCertificate = true,
    Timeout = TimeSpan.FromMinutes(10)
};
```

### 🔄 **Retry Policies**

```csharp
// Retry failed operations with exponential backoff
public static async Task<T> WithRetry<T>(Func<Task<T>> operation, int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
            await Task.Delay(delay);
        }
    }
    
    return await operation(); // Final attempt
}
```

---

## ⏳ Task Management

### 🔄 **Long-Running Operations**

```csharp
// Wait for task completion with timeout
public static async Task<bool> WaitForTask(PveClient client, string node, string taskId)
{
    var timeout = TimeSpan.FromMinutes(30);
    var start = DateTime.Now;
    
    while (DateTime.Now - start < timeout)
    {
        var status = await client.Nodes[node].Tasks[taskId].Status.ReadTaskStatus();
        
        if (status.Response.data.status == "stopped")
        {
            return status.Response.data.exitstatus == "OK";
        }
        
        await Task.Delay(2000);
    }
    
    throw new TimeoutException("Task did not complete within timeout");
}
```

### 📊 **Task Monitoring**

```csharp
// Monitor multiple tasks simultaneously
public static async Task MonitorTasks(PveClient client, Dictionary<string, string> tasks)
{
    var activeTasks = new Dictionary<string, string>(tasks);
    
    while (activeTasks.Any())
    {
        var completedTasks = new List<string>();
        
        foreach (var (taskId, node) in activeTasks)
        {
            var status = await client.Nodes[node].Tasks[taskId].Status.ReadTaskStatus();
            if (status.Response.data.status == "stopped")
            {
                var success = status.Response.data.exitstatus == "OK";
                Console.WriteLine($"Task {taskId}: {(success ? "✅" : "❌")}");
                completedTasks.Add(taskId);
            }
        }
        
        completedTasks.ForEach(id => activeTasks.Remove(id));
        await Task.Delay(3000);
    }
}
```

---

## 🔧 Resource Management

### 🎯 **Smart Resource Discovery**

```csharp
// Find resources across the cluster
public static async Task<ClusterResource> FindResource(PveClient client, string nameOrId)
{
    var resources = await client.Cluster.Resources.Get();
    
    // Try by ID first
    if (int.TryParse(nameOrId, out int vmId))
    {
        return resources.FirstOrDefault(r => r.VmId == vmId);
    }
    
    // Then by name (case insensitive)
    return resources.FirstOrDefault(r => 
        r.Name?.Equals(nameOrId, StringComparison.OrdinalIgnoreCase) == true);
}
```

### 📊 **Cluster Health Monitoring**

```csharp
// Comprehensive cluster health check
public static async Task<ClusterHealth> CheckClusterHealth(PveClient client)
{
    var resources = await client.Cluster.Resources.Get();
    var nodes = resources.Where(r => r.Type == "node");
    var vms = resources.Where(r => r.Type == "qemu");
    var containers = resources.Where(r => r.Type == "lxc");
    
    return new ClusterHealth
    {
        TotalNodes = nodes.Count(),
        OnlineNodes = nodes.Count(n => n.Status == "online"),
        TotalVMs = vms.Count(),
        RunningVMs = vms.Count(v => v.Status == "running"),
        TotalContainers = containers.Count(),
        RunningContainers = containers.Count(c => c.Status == "running"),
        AverageCpuUsage = nodes.Average(n => n.Cpu ?? 0),
        AverageMemoryUsage = nodes.Average(n => n.Mem.ToPercentage(n.MaxMem))
    };
}
```

---

## 🛡️ Error Handling Strategies

### 🎯 **Centralized Error Handling**

```csharp
public static async Task<Result> SafeApiCall(Func<Task<Result>> apiCall)
{
    try
    {
        var result = await apiCall();
        
        if (!result.IsSuccessStatusCode)
        {
            Console.WriteLine($"API Error: {result.StatusCode} - {result.GetError()}");
        }
        
        return result;
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Network error: {ex.Message}");
        throw;
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("Request timeout");
        throw;
    }
}
```

---

## 🔄 Bulk Operations

### ⚡ **Parallel VM Operations**

```csharp
// Perform operations on multiple VMs in parallel
public static async Task<Dictionary<int, bool>> BulkVmOperation(
    PveClient client, 
    IEnumerable<int> vmIds, 
    string operation)
{
    var semaphore = new SemaphoreSlim(5); // Limit concurrent operations
    var results = new ConcurrentDictionary<int, bool>();
    
    var tasks = vmIds.Select(async vmId =>
    {
        await semaphore.WaitAsync();
        try
        {
            var resource = await FindResource(client, vmId.ToString());
            if (resource != null)
            {
                var vm = client.Nodes[resource.Node].Qemu[vmId];
                var result = operation switch
                {
                    "start" => await vm.Status.Start.VmStart(),
                    "stop" => await vm.Status.Stop.VmStop(),
                    _ => throw new ArgumentException($"Unknown operation: {operation}")
                };
                
                results[vmId] = result.IsSuccessStatusCode;
            }
        }
        finally
        {
            semaphore.Release();
        }
    });
    
    await Task.WhenAll(tasks);
    return new Dictionary<int, bool>(results);
}
```

---

## 📊 Performance Optimization

### ⚡ **Response Caching**

```csharp
public class CachedPveClient
{
