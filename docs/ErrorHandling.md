# Error Handling Guide 🛡️

Comprehensive guide to handling errors and exceptions when working with the Proxmox VE API.

## 🎯 Types of Errors

### 🌐 **Network Errors**
```csharp
try
{
    var client = new PveClient("invalid-host.local");
    var result = await client.Version.Version();
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
    // Handle: DNS resolution, connection refused, network timeout
}
catch (TaskCanceledException ex)
{
    Console.WriteLine($"Request timeout: {ex.Message}");
    // Handle: Request took too long
}
```

### 🔐 **Authentication Errors**
```csharp
try
{
    var client = new PveClient("pve.local");
    bool success = await client.Login("user@pam", "wrong-password");
    
    if (!success)
    {
        Console.WriteLine("❌ Authentication failed - check credentials");
    }
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"❌ Authentication error: {ex.Message}");
}
```

### 📊 **API Response Errors**
```csharp
var result = await client.Nodes["pve1"].Qemu[999].Config.VmConfig();

if (!result.IsSuccessStatusCode)
{
    switch (result.StatusCode)
    {
        case HttpStatusCode.NotFound:
            Console.WriteLine("❌ VM not found");
            break;
        case HttpStatusCode.Forbidden:
            Console.WriteLine("❌ Permission denied");
            break;
        case HttpStatusCode.BadRequest:
            Console.WriteLine($"❌ Bad request: {result.GetError()}");
            break;
        default:
            Console.WriteLine($"❌ API error: {result.StatusCode} - {result.ReasonPhrase}");
            break;
    }
}
```

## 🔧 Error Handling Patterns

### 📋 **Basic Pattern**
```csharp
public static async Task<bool> SafeVmOperation(PveClient client, string node, int vmId)
{
    try
    {
        var result = await client.Nodes[node].Qemu[vmId].Status.Start.VmStart();
        
        if (result.IsSuccessStatusCode)
        {
            Console.WriteLine($"✅ VM {vmId} started successfully");
            return true;
        }
        else
        {
            Console.WriteLine($"❌ Failed to start VM {vmId}: {result.GetError()}");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Exception starting VM {vmId}: {ex.Message}");
        return false;
    }
}
```

### 🎯 **Centralized Error Handler**
```csharp
public static class ErrorHandler
{
    public static async Task<r> SafeApiCall<T>(Func<Task<r>> apiCall, string operation = "API call")
    {
        try
        {
            var result = await apiCall();
            
            if (!result.IsSuccessStatusCode)
            {
                LogApiError(result, operation);
            }
            
            return result;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Network error during {operation}: {ex.Message}");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"❌ Timeout during {operation}: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error during {operation}: {ex.Message}");
            throw;
        }
    }
    
    private static void LogApiError(Result result, string operation)
    {
        Console.WriteLine($"❌ {operation} failed:");
        Console.WriteLine($"   Status: {result.StatusCode} - {result.ReasonPhrase}");
        
        if (result.ResponseInError)
        {
            Console.WriteLine($"   Details: {result.GetError()}");
        }
    }
}

// Usage
var result = await ErrorHandler.SafeApiCall(
    () => client.Nodes["pve1"].Qemu[100].Status.Start.VmStart(),
    "Starting VM 100"
);
```

### 🔄 **Retry Logic**
```csharp
public static async Task<r> WithRetry<T>(
    Func<Task<r>> operation, 
    int maxRetries = 3, 
    string operationName = "operation")
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            var result = await operation();
            
            if (result.IsSuccessStatusCode)
            {
                return result;
            }
            
            // Don't retry client errors (4xx), only server errors (5xx)
            if ((int)result.StatusCode < 500)
            {
                Console.WriteLine($"❌ {operationName} failed with client error: {result.StatusCode}");
                return result;
            }
            
            if (attempt < maxRetries)
            {
                Console.WriteLine($"⚠️  {operationName} failed (attempt {attempt}/{maxRetries}), retrying...");
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
            }
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            Console.WriteLine($"⚠️  {operationName} threw exception (attempt {attempt}/{maxRetries}): {ex.Message}");
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }
    }
    
    // Final attempt without catching exceptions
    return await operation();
}

// Usage
var result = await WithRetry(
    () => client.Nodes["pve1"].Qemu[100].Config.VmConfig(),
    maxRetries: 3,
    operationName: "Get VM config"
);
```

## 🚨 Common Error Scenarios

### 🔐 **Permission Issues**
```csharp
public static void HandlePermissionError(Result result)
{
    if (result.StatusCode == HttpStatusCode.Forbidden)
    {
        Console.WriteLine("❌ Permission denied. Check:");
        Console.WriteLine("   - User has required permissions");
        Console.WriteLine("   - API token has correct privileges");
        Console.WriteLine("   - Resource exists and user has access");
    }
}
```

### 🔍 **Resource Not Found**
```csharp
public static async Task<bool> VmExists(PveClient client, string node, int vmId)
{
    try
    {
        var result = await client.Nodes[node].Qemu[vmId].Config.VmConfig();
        return result.IsSuccessStatusCode;
    }
    catch (HttpRequestException)
    {
        return false; // Network error, can't determine
    }
}

// Usage
if (!await VmExists(client, "pve1", 100))
{
    Console.WriteLine("❌ VM 100 does not exist on node pve1");
    return;
}
```

### ⏱️ **Timeout Handling**
```csharp
var client = new PveClient("pve.local")
{
    Timeout = TimeSpan.FromMinutes(5) // Increase timeout for long operations
};

try
{
    var result = await client.Nodes["pve1"].Qemu[100].Clone.CloneVm(newid: 101);
}
catch (TaskCanceledException ex)
{
    if (ex.CancellationToken.IsCancellationRequested)
    {
        Console.WriteLine("❌ Operation was cancelled");
    }
    else
    {
        Console.WriteLine("❌ Operation timed out - try increasing client timeout");
    }
}
```

## 🎯 Best Practices

### ✅ **Defensive Programming**
```csharp
// ✅ Always validate input
public static async Task<r> GetVmConfig(PveClient client, string node, int vmId)
{
    if (string.IsNullOrWhiteSpace(node))
        throw new ArgumentException("Node name cannot be empty", nameof(node));
    
    if (vmId <= 0)
        throw new ArgumentException("VM ID must be positive", nameof(vmId));
    
    return await client.Nodes[node].Qemu[vmId].Config.VmConfig();
}

// ✅ Check for null responses
var result = await client.Cluster.Resources.Resources();
if (result.IsSuccessStatusCode && result.Response?.data != null)
{
    foreach (var resource in result.Response.data)
    {
        // Process resource
    }
}
```

### 📊 **Graceful Degradation**
```csharp
public static async Task<ClusterStatus> GetClusterStatus(PveClient client)
{
    try
    {
        var result = await client.Cluster.Status.Status();
        if (result.IsSuccessStatusCode)
        {
            return ParseClusterStatus(result.Response.data);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Could not get cluster status: {ex.Message}");
    }
    
    // Return fallback status
    return new ClusterStatus { Status = "unknown", LastUpdate = DateTime.Now };
}
```

### 🔍 **Detailed Logging**
```csharp
public static async Task<r> LoggedApiCall<T>(Func<Task<r>> apiCall, string operation)
{
    Console.WriteLine($"🔄 Starting: {operation}");
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        var result = await apiCall();
        stopwatch.Stop();
        
        if (result.IsSuccessStatusCode)
        {
            Console.WriteLine($"✅ {operation} completed in {stopwatch.ElapsedMilliseconds}ms");
        }
        else
        {
            Console.WriteLine($"❌ {operation} failed after {stopwatch.ElapsedMilliseconds}ms: {result.GetError()}");
        }
        
        return result;
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"❌ {operation} threw exception after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
        throw;
    }
}
```

<div align="center">
  <sub>Made with ❤️ in Italy 🇮🇹 by <a href="https://www.corsinvest.it">Corsinvest</a></sub>
</div>
