# Task Management Guide ⏳

Understanding and managing long-running operations in Proxmox VE.

## 🎯 Understanding Tasks

Many Proxmox VE operations are asynchronous and return a task ID instead of immediate results:

```csharp
// Operations that return task IDs
var result = await client.Nodes["pve1"].Qemu[100].Clone.CloneVm(newid: 101);
if (result.IsSuccessStatusCode)
{
    var taskId = result.Response.data; // Returns: "UPID:pve1:..."
    Console.WriteLine($"Task started: {taskId}");
}
```

## 📊 Task Status

### 🔍 **Checking Task Status**
```csharp
public static async Task<TaskStatus> GetTaskStatus(PveClient client, string node, string taskId)
{
    var result = await client.Nodes[node].Tasks[taskId].Status.ReadTaskStatus();
    
    if (result.IsSuccessStatusCode)
    {
        var data = result.Response.data;
        return new TaskStatus
        {
            Status = data.status,        // "running", "stopped"
            ExitStatus = data.exitstatus, // "OK" if successful
            StartTime = data.starttime,
            EndTime = data.endtime,
            Progress = data.progress,     // For operations with progress
            Log = data.log               // Task log entries
        };
    }
    
    throw new Exception($"Failed to get task status: {result.GetError()}");
}
```

### ⏱️ **Waiting for Completion**
```csharp
public static async Task<bool> WaitForTaskCompletion(
    PveClient client, 
    string node, 
    string taskId, 
    TimeSpan timeout = default,
    IProgress<string> progress = null)
{
    if (timeout == default) timeout = TimeSpan.FromMinutes(30);
    
    var startTime = DateTime.Now;
    var lastStatus = "";
    
    while (DateTime.Now - startTime < timeout)
    {
        var statusResult = await client.Nodes[node].Tasks[taskId].Status.ReadTaskStatus();
        
        if (!statusResult.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to check task status: {statusResult.GetError()}");
        }
        
        var data = statusResult.Response.data;
        var currentStatus = data.status;
        
        // Report progress if status changed
        if (currentStatus != lastStatus)
        {
            progress?.Report($"Task {taskId}: {currentStatus}");
            lastStatus = currentStatus;
        }
        
        // Check if task completed
        if (currentStatus == "stopped")
        {
            var exitStatus = data.exitstatus;
            var success = exitStatus == "OK";
            
            progress?.Report($"Task {taskId} {(success ? "completed" : "failed")}: {exitStatus}");
            return success;
        }
        
        await Task.Delay(2000); // Check every 2 seconds
    }
    
    throw new TimeoutException($"Task {taskId} did not complete within {timeout}");
}
```

## 🔄 Common Task Operations

### 📸 **VM Clone with Progress**
```csharp
public static async Task<bool> CloneVmWithProgress(
    PveClient client, 
    string node, 
    int sourceVmId, 
    int targetVmId, 
    string newName)
{
    Console.WriteLine($"🔄 Cloning VM {sourceVmId} to {targetVmId}...");
    
    // Start clone operation
    var cloneResult = await client.Nodes[node].Qemu[sourceVmId].Clone.CloneVm(
        newid: targetVmId,
        name: newName
    );
    
    if (!cloneResult.IsSuccessStatusCode)
    {
        Console.WriteLine($"❌ Failed to start clone: {cloneResult.GetError()}");
        return false;
    }
    
    var taskId = cloneResult.Response.data;
    
    // Wait for completion with progress reporting
    var progress = new Progress<string>(status => Console.WriteLine($"📊 {status}"));
    
    try
    {
        var success = await WaitForTaskCompletion(client, node, taskId, TimeSpan.FromMinutes(60), progress);
        
        if (success)
        {
            Console.WriteLine($"✅ VM cloned successfully: {sourceVmId} → {targetVmId}");
        }
        else
        {
            Console.WriteLine($"❌ VM clone failed");
        }
        
        return success;
    }
    catch (TimeoutException)
    {
        Console.WriteLine($"⏰ Clone operation timed out");
        return false;
    }
}
```

### 📦 **Container Creation**
```csharp
public static async Task<bool> CreateContainer(
    PveClient client, 
    string node, 
    int vmId, 
    string template, 
    ContainerConfig config)
{
    // Start container creation
    var createResult = await client.Nodes[node].Lxc.CreateVm(
        vmid: vmId,
        ostemplate: template,
        hostname: config.Hostname,
        memory: config.Memory,
        rootfs: config.RootFs
    );
    
    if (!createResult.IsSuccessStatusCode)
    {
        Console.WriteLine($"❌ Failed to create container: {createResult.GetError()}");
        return false;
    }
    
    var taskId = createResult.Response.data;
    Console.WriteLine($"📦 Creating container {vmId} (Task: {taskId})");
    
    return await WaitForTaskCompletion(client, node, taskId, TimeSpan.FromMinutes(10));
}
```

## 📊 Monitoring Multiple Tasks

### 🔄 **Parallel Task Monitoring**
```csharp
public static async Task<Dictionary<string, bool>> MonitorMultipleTasks(
    PveClient client, 
    Dictionary<string, string> tasks) // taskId -> node
{
    var results = new Dictionary<string, bool>();
    var activeTasks = new Dictionary<string, string>(tasks);
    
    Console.WriteLine($"📊 Monitoring {activeTasks.Count} tasks...");
    
    while (activeTasks.Any())
    {
        var completedTasks = new List<string>();
        
        // Check each active task
        foreach (var (taskId, node) in activeTasks)
        {
            try
            {
                var statusResult = await client.Nodes[node].Tasks[taskId].Status.ReadTaskStatus();
                
                if (statusResult.IsSuccessStatusCode && statusResult.Response.data.status == "stopped")
                {
                    var success = statusResult.Response.data.exitstatus == "OK";
                    results[taskId] = success;
                    completedTasks.Add(taskId);
                    
                    Console.WriteLine($"{(success ? "✅" : "❌")} Task {taskId}: {statusResult.Response.data.exitstatus}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error checking task {taskId}: {ex.Message}");
                results[taskId] = false;
                completedTasks.Add(taskId);
            }
        }
        
        // Remove completed tasks
        foreach (var taskId in completedTasks)
        {
            activeTasks.Remove(taskId);
        }
        
        if (activeTasks.Any())
        {
            await Task.Delay(3000); // Check every 3 seconds
        }
    }
    
    return results;
}
```

### 📈 **Task Progress Display**
```csharp
public static async Task DisplayTaskProgress(PveClient client, string node, string taskId)
{
    var startTime = DateTime.Now;
    
    while (true)
    {
        var statusResult = await client.Nodes[node].Tasks[taskId].Status.ReadTaskStatus();
        
        if (statusResult.IsSuccessStatusCode)
        {
            var data = statusResult.Response.data;
            var elapsed = DateTime.Now - startTime;
            
            Console.Clear();
            Console.WriteLine($"Task Progress: {taskId}");
            Console.WriteLine($"Status: {data.status}");
            Console.WriteLine($"Elapsed: {elapsed:hh\\:mm\\:ss}");
            
            if (data.progress != null)
            {
                Console.WriteLine($"Progress: {data.progress}%");
                
                // Draw progress bar
                var progressBar = new string('█', (int)(data.progress / 5));
                var remaining = new string('░', 20 - progressBar.Length);
                Console.WriteLine($"[{progressBar}{remaining}] {data.progress}%");
            }
            
            if (data.status == "stopped")
            {
                Console.WriteLine($"Final Status: {data.exitstatus}");
                break;
            }
        }
        
        await Task.Delay(1000);
    }
}
```

## 🛠️ Task Utilities

### 📋 **Task History**
```csharp
public static async Task<List<TaskInfo>> GetRecentTasks(PveClient client, string node, int limit = 10)
{
    var result = await client.Nodes[node].Tasks.NodeTasks(limit: limit);
    
    if (result.IsSuccessStatusCode)
    {
        var tasks = new List<TaskInfo>();
        
        foreach (var task in result.Response.data)
        {
            tasks.Add(new TaskInfo
            {
                Id = task.upid,
                Type = task.type,
                Status = task.status,
                ExitStatus = task.exitstatus,
                StartTime = DateTimeOffset.FromUnixTimeSeconds((long)task.starttime).DateTime,
                EndTime = task.endtime != null ? DateTimeOffset.FromUnixTimeSeconds((long)task.endtime).DateTime : null,
                User = task.user,
                Node = task.node
            });
        }
        
        return tasks.OrderByDescending(t => t.StartTime).ToList();
    }
    
    return new List<TaskInfo>();
}
```

### 🗑️ **Task Cleanup**
```csharp
public static async Task<bool> StopTask(PveClient client, string node, string taskId)
{
    var result = await client.Nodes[node].Tasks[taskId].Delete();
    
    if (result.IsSuccessStatusCode)
    {
        Console.WriteLine($"🛑 Task {taskId} stopped");
        return true;
    }
    else
    {
        Console.WriteLine($"❌ Failed to stop task {taskId}: {result.GetError()}");
        return false;
    }
}
```

## 🎯 Best Practices

### ✅ **Timeout Management**
```csharp
// ✅ Set appropriate timeouts for different operations
var timeouts = new Dictionary<string, TimeSpan>
{
    ["clone"] = TimeSpan.FromHours(2),
    ["backup"] = TimeSpan.FromHours(4),
    ["snapshot"] = TimeSpan.FromMinutes(10),
    ["start"] = TimeSpan.FromMinutes(5),
    ["stop"] = TimeSpan.FromMinutes(5)
};

var timeout = timeouts.GetValueOrDefault(operationType, TimeSpan.FromMinutes(30));
await WaitForTaskCompletion(client, node, taskId, timeout);
```

### 📊 **Error Recovery**
```csharp
public static async Task<bool> RobustTaskWait(PveClient client, string node, string taskId)
{
    const int maxRetries = 3;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await WaitForTaskCompletion(client, node, taskId);
        }
        catch (HttpRequestException ex) when (attempt < maxRetries)
        {
            Console.WriteLine($"⚠️  Network error checking task (attempt {attempt}): {ex.Message}");
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
    
    // Final attempt
    return await WaitForTaskCompletion(client, node, taskId);
}
```

### 🔄 **Batch Operations with Tasks**
```csharp
public static async Task<Dictionary<int, bool>> BulkVmClone(
    PveClient client, 
    string node, 
    int sourceVmId, 
    IEnumerable<int> targetVmIds)
{
    var tasks = new Dictionary<string, int>(); // taskId -> targetVmId
    var results = new Dictionary<int, bool>();
    
    // Start all clone operations
    foreach (var targetVmId in targetVmIds)
    {
        try
        {
            var cloneResult = await client.Nodes[node].Qemu[sourceVmId].Clone.CloneVm(newid: targetVmId);
            
            if (cloneResult.IsSuccessStatusCode)
            {
                var taskId = cloneResult.Response.data;
                tasks[taskId] = targetVmId;
                Console.WriteLine($"🔄 Started clone to VM {targetVmId} (Task: {taskId})");
            }
            else
            {
                Console.WriteLine($"❌ Failed to start clone to VM {targetVmId}: {cloneResult.GetError()}");
                results[targetVmId] = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception starting clone to VM {targetVmId}: {ex.Message}");
            results[targetVmId] = false;
        }
    }
    
    // Monitor all tasks
    var taskResults = await MonitorMultipleTasks(
        client, 
        tasks.ToDictionary(kvp => kvp.Key, kvp => node)
    );
    
    // Map task results back to VM IDs
    foreach (var (taskId, success) in taskResults)
    {
        if (tasks.TryGetValue(taskId, out int vmId))
        {
            results[vmId] = success;
        }
    }
    
    return results;
}
```

## 📚 Task Information Models

```csharp
public class TaskInfo
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public string ExitStatus { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string User { get; set; }
    public string Node { get; set; }
    
    public TimeSpan? Duration => EndTime?.Subtract(StartTime);
    public bool IsCompleted => Status == "stopped";
    public bool IsSuccessful => ExitStatus == "OK";
}

public class TaskStatus
{
    public string Status { get; set; }
    public string ExitStatus { get; set; }
    public long StartTime { get; set; }
    public long? EndTime { get; set; }
    public double? Progress { get; set; }
    public string Log { get; set; }
}
```

---

<div align="center">
  <sub>Part of <a href="https://www.cv4pve-tools.com">cv4pve-tools</a> suite | Made with ❤️ in Italy by <a href="https://www.corsinvest.it">Corsinvest</a></sub>
</div>
