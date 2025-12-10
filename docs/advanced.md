# Advanced Usage Guide

This guide covers complex scenarios, best practices, and advanced patterns for experienced developers.

## Enterprise Configuration

### **Custom HttpClient Setup**

```csharp
// Corporate environment with proxy and certificate validation
var handler = new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    {
        // Custom certificate validation for corporate environments
        var allowedThumbprints = new[] { "A1:B2:C3...", "F6:E5:D4..." };
        return allowedThumbprints.Contains(cert.GetCertHashString());
    },
    Proxy = new WebProxy("http://proxy.company.com:8080")
    {
        Credentials = new NetworkCredential("proxyuser", "proxypass")
    },
    UseProxy = true
};

var httpClient = new HttpClient(handler)
{
    Timeout = TimeSpan.FromMinutes(10)
};

var client = new PveClient("pve.company.com", httpClient)
{
    ValidateCertificate = true,
    ApiToken = Environment.GetEnvironmentVariable("PROXMOX_API_TOKEN")
};
```

### **Resilient Operations**

```csharp
// Retry policy with exponential backoff
public static async Task<T> WithRetry<T>(Func<Task<T>> operation, int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (attempt < maxRetries && IsRetriableError(ex))
        {
            var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
            Console.WriteLine($"Attempt {attempt} failed, retrying in {delay.TotalSeconds}s: {ex.Message}");
            await Task.Delay(delay);
        }
    }
    
    return await operation(); // Final attempt
}

private static bool IsRetriableError(Exception ex)
{
    return ex is HttpRequestException || ex is TaskCanceledException;
}

// Usage
var result = await WithRetry(() => client.Nodes["pve1"].Qemu[100].Status.Start.VmStart());
```

---

## Task and Resource Management

### **Long-Running Operations**

```csharp
// Complete task management with progress
public static async Task<bool> ExecuteWithProgress(
    PveClient client, 
    Func<Task<Result>> operation, 
    string node,
    string description)
{
    Console.WriteLine($"Starting: {description}");

    var result = await operation();
    var taskId = result.Response.data.ToString();
    return await WaitForTaskCompletion(client, node, taskId, description);
}

private static async Task<bool> WaitForTaskCompletion(
    PveClient client, 
    string node, 
    string taskId, 
    string description)
{
    var timeout = TimeSpan.FromMinutes(30);
    var start = DateTime.Now;
    
    while (DateTime.Now - start < timeout)
    {
        var status = await client.Nodes[node].Tasks[taskId].Status.ReadTaskStatus();

        if (status.Response.data.status == "stopped")
        {
            var success = status.Response.data.exitstatus == "OK";
            Console.WriteLine($"{description}: {status.Response.data.exitstatus} ({(success ? "Success" : "Failed")})");
            return success;
        }

        await Task.Delay(2000);
    }
    
    Console.WriteLine($"Timeout: {description} timed out");
    return false;
}
```

### **Bulk Operations**

```csharp
// Perform operations on multiple VMs with concurrency control
public static async Task<Dictionary<int, bool>> BulkVmOperation(
    PveClient client, 
    IEnumerable<int> vmIds, 
    Func<PveClient, string, int, Task<Result>> operation,
    string operationName)
{
    var resources = await client.Cluster.Resources.Get();
    var vmLocations = resources
        .Where(r => r.Type == "qemu" && vmIds.Contains(r.VmId))
        .ToDictionary(r => r.VmId, r => r.Node);
    
    var semaphore = new SemaphoreSlim(5); // Limit concurrent operations
    var results = new ConcurrentDictionary<int, bool>();
    
    var tasks = vmIds.Select(async vmId =>
    {
        if (!vmLocations.TryGetValue(vmId, out string node))
        {
            Console.WriteLine($"VM {vmId} not found");
            results[vmId] = false;
            return;
        }
        
        await semaphore.WaitAsync();
        try
        {
            var result = await operation(client, node, vmId);
            var success = result.IsSuccessStatusCode;
            
            Console.WriteLine($"VM {vmId} {operationName}: {(success ? "Success" : $"Failed - {result.GetError()}")}");
            results[vmId] = success;
        }
        finally
        {
            semaphore.Release();
        }
    });
    
    await Task.WhenAll(tasks);
    return new Dictionary<int, bool>(results);
}

// Usage examples
var startResults = await BulkVmOperation(
    client, 
    new[] { 100, 101, 102 },
    (c, node, vmId) => c.Nodes[node].Qemu[vmId].Status.Start.VmStart(),
    "start"
);

var snapshotResults = await BulkVmOperation(
    client,
    new[] { 100, 101, 102 },
    (c, node, vmId) => c.Nodes[node].Qemu[vmId].Snapshot.Snapshot($"backup-{DateTime.Now:yyyyMMdd}"),
    "snapshot"
);
```

---

## Monitoring and Health Checks

### **Cluster Health Assessment**

```csharp
public class ClusterHealthMonitor
{
    private readonly PveClient _client;
    
    public ClusterHealthMonitor(PveClient client)
    {
        _client = client;
    }
    
    public async Task<ClusterHealthReport> GetHealthReport()
    {
        var resources = await _client.Cluster.Resources.Get();
        
        var nodes = resources.Where(r => r.Type == "node").ToList();
        var vms = resources.Where(r => r.Type == "qemu").ToList();
        var containers = resources.Where(r => r.Type == "lxc").ToList();
        
        return new ClusterHealthReport
        {
            Timestamp = DateTime.Now,
            Nodes = new NodeSummary
            {
                Total = nodes.Count,
                Online = nodes.Count(n => n.Status == "online"),
                AverageCpuUsage = nodes.Average(n => n.Cpu ?? 0),
                AverageMemoryUsage = nodes.Average(n => (double)(n.Mem ?? 0) / (n.MaxMem ?? 1))
            },
            VirtualMachines = new VmSummary
            {
                Total = vms.Count,
                Running = vms.Count(v => v.Status == "running"),
                Stopped = vms.Count(v => v.Status == "stopped"),
                HighCpuUsage = vms.Count(v => (v.Cpu ?? 0) > 0.8)
            },
            Containers = new ContainerSummary
            {
                Total = containers.Count,
                Running = containers.Count(c => c.Status == "running"),
                Stopped = containers.Count(c => c.Status == "stopped")
            }
        };
    }
    
    public async Task<List<Alert>> CheckAlerts()
    {
        var alerts = new List<Alert>();
        var resources = await _client.Cluster.Resources.Get();
        
        // Check for offline nodes
        var offlineNodes = resources.Where(r => r.Type == "node" && r.Status != "online");
        alerts.AddRange(offlineNodes.Select(node => new Alert
        {
            Severity = AlertSeverity.Critical,
            Message = $"Node {node.Node} is offline",
            Resource = node.Node
        }));
        
        // Check for high resource usage
        var highCpuNodes = resources.Where(r => r.Type == "node" && (r.Cpu ?? 0) > 0.9);
        alerts.AddRange(highCpuNodes.Select(node => new Alert
        {
            Severity = AlertSeverity.Warning,
            Message = $"Node {node.Node} has high CPU usage: {node.Cpu:P1}",
            Resource = node.Node
        }));
        
        return alerts;
    }
}

// Usage
var monitor = new ClusterHealthMonitor(client);
var health = await monitor.GetHealthReport();
var alerts = await monitor.CheckAlerts();

Console.WriteLine($"Cluster Health: {health.Nodes.Online}/{health.Nodes.Total} nodes online");
Console.WriteLine($"VMs: {health.VirtualMachines.Running}/{health.VirtualMachines.Total} running");

foreach (var alert in alerts.Where(a => a.Severity == AlertSeverity.Critical))
{
    Console.WriteLine($"CRITICAL: {alert.Message}");
}
```

---

## Architecture Patterns

### **Repository Pattern**

```csharp
public interface IProxmoxRepository
{
    Task<IEnumerable<VmInfo>> GetVmsAsync(string nodeFilter = null);
    Task<VmConfig> GetVmConfigAsync(string node, int vmId);
    Task<bool> StartVmAsync(string node, int vmId);
    Task<bool> CreateSnapshotAsync(string node, int vmId, string name, string description = null);
}

public class ProxmoxRepository : IProxmoxRepository
{
    private readonly PveClient _client;
    private readonly ILogger<ProxmoxRepository> _logger;
    
    public ProxmoxRepository(PveClient client, ILogger<ProxmoxRepository> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task<IEnumerable<VmInfo>> GetVmsAsync(string nodeFilter = null)
    {
        _logger.LogDebug($"Getting VMs for node filter: {nodeFilter}");
        
        var resources = await _client.Cluster.Resources.Get();
        var vms = resources.Where(r => r.Type == "qemu");
        
        if (!string.IsNullOrEmpty(nodeFilter))
        {
            vms = vms.Where(vm => vm.Node.Equals(nodeFilter, StringComparison.OrdinalIgnoreCase));
        }
        
        return vms;
    }
    
    public async Task<VmConfig> GetVmConfigAsync(string node, int vmId)
    {
        _logger.LogDebug($"Getting config for VM {vmId} on node {node}");
        
        return await _client.Nodes[node].Qemu[vmId].Config.Get();
    }
    
    public async Task<bool> StartVmAsync(string node, int vmId)
    {
        _logger.LogInformation($"Starting VM {vmId} on node {node}");
        
        var result = await _client.Nodes[node].Qemu[vmId].Status.Start.VmStart();
        
        if (result.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Successfully started VM {vmId}");
            return true;
        }
        else
        {
            _logger.LogError($"Failed to start VM {vmId}: {result.GetError()}");
            return false;
        }
    }
    
    public async Task<bool> CreateSnapshotAsync(string node, int vmId, string name, string description = null)
    {
        _logger.LogInformation($"Creating snapshot {name} for VM {vmId} on node {node}");
        
        var result = await _client.Nodes[node].Qemu[vmId].Snapshot.Snapshot(name, description);
        return result.IsSuccessStatusCode;
    }
}
```

---

## Error Handling and Logging

### **Centralized Error Management**

```csharp
public static class ProxmoxOperations
{
    public static async Task<r> SafeExecute<T>(
        Func<Task<r>> operation, 
        string operationName,
        ILogger logger = null)
    {
        try
        {
            logger?.LogDebug($"Executing: {operationName}");
            var stopwatch = Stopwatch.StartNew();
            
            var result = await operation();
            stopwatch.Stop();
            
            if (result.IsSuccessStatusCode)
            {
                logger?.LogInformation($"{operationName} completed in {stopwatch.ElapsedMilliseconds}ms");
            }
            else
            {
                logger?.LogWarning($"{operationName} failed: {result.GetError()} (took {stopwatch.ElapsedMilliseconds}ms)");
            }
            
            return result;
        }
        catch (HttpRequestException ex)
        {
            logger?.LogError(ex, $"Network error during {operationName}");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            logger?.LogError(ex, $"Timeout during {operationName}");
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, $"Unexpected error during {operationName}");
            throw;
        }
    }
}

// Usage
var result = await ProxmoxOperations.SafeExecute(
    () => client.Nodes["pve1"].Qemu[100].Status.Start.VmStart(),
    "Start VM 100",
    logger
);
```

---

## Best Practices Summary

### **Performance**
- Use HttpClientFactory for connection pooling
- Implement retry policies for resilience
- Limit concurrent operations with SemaphoreSlim
- Cache frequently accessed data

### **Security**
- Always use API tokens in production
- Enable SSL certificate validation
- Store credentials securely (environment variables, key vault)
- Implement proper audit logging

### **Architecture**
- Use repository pattern for testability
- Implement centralized error handling
- Use dependency injection for configuration
- Separate concerns with proper abstractions

### **Monitoring**
- Log all operations with appropriate levels
- Implement health checks and alerting
- Monitor task completion and failures
- Track performance metrics
