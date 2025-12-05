# Corsinvest.ProxmoxVE.Api.Console

```bash
dotnet add package Corsinvest.ProxmoxVE.Api.Console
```

## Key Features

- **Command-Line Helpers** - Utilities for console application development
- **Output Formatting** - Table and data formatting for console output
- **Configuration Management** - Helper methods for application configuration
- **Logging Integration** - Console logging and output utilities
- **Error Handling** - Standardized error handling for CLI applications
- **Authentication Helpers** - Common authentication patterns for console apps

---

## Command-Line Application Helpers

### Basic Console Application Setup

```csharp
using Corsinvest.ProxmoxVE.Api;
using Corsinvest.ProxmoxVE.Api.Console;

class Program
{
    static async Task Main(string[] args)
    {
        var app = new ConsoleHelper("My Proxmox VE Tool", "1.0.0");
        
        // Add command-line options
        app.AddOption("--host", "Proxmox VE host", required: true);
        app.AddOption("--username", "Username", required: true);
        app.AddOption("--password", "Password");
        app.AddOption("--api-token", "API Token");
        app.AddOption("--vmid", "VM ID", type: typeof(int));
        
        // Parse arguments
        if (!app.Parse(args))
        {
            app.ShowHelp();
            return;
        }
        
        try
        {
            // Create and configure client
            var client = new PveClient(app.GetValue("host"));
            
            // Authentication
            if (app.HasValue("api-token"))
            {
                client.ApiToken = app.GetValue("api-token");
            }
            else
            {
                var username = app.GetValue("username");
                var password = app.GetValue("password");
                
                if (!await client.Login(username, password))
                {
                    app.WriteError("Authentication failed");
                    return;
                }
            }
            
            // Your application logic here
            await RunApplication(client, app);
        }
        catch (Exception ex)
        {
            app.WriteError($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
    
    static async Task RunApplication(PveClient client, ConsoleHelper app)
    {
        // Application implementation
        var vmId = app.GetValue<int>("vmid");
        // ... your code here
    }
}
```

### Authentication Patterns

```csharp
using Corsinvest.ProxmoxVE.Api.Console;

// Helper method for common authentication scenarios
public static async Task<PveClient> CreateAuthenticatedClient(ConsoleHelper app)
{
    var host = app.GetRequiredValue("host");
    var client = new PveClient(host);
    
    // Set SSL validation if specified
    if (app.HasValue("validate-certificate"))
    {
        client.ValidateCertificate = app.GetValue<bool>("validate-certificate");
    }
    
    // Set timeout if specified
    if (app.HasValue("timeout"))
    {
        client.Timeout = TimeSpan.FromSeconds(app.GetValue<int>("timeout"));
    }
    
    // Authentication methods
    if (app.HasValue("api-token"))
    {
        // API Token authentication
        client.ApiToken = app.GetValue("api-token");
        app.WriteInfo("Using API Token authentication");
    }
    else
    {
        // Username/password authentication
        var username = app.GetRequiredValue("username");
        var password = app.GetRequiredValue("password");
        var otp = app.GetValue("otp"); // Two-factor authentication
        
        app.WriteInfo($"Logging in as {username}...");
        
        bool success = string.IsNullOrEmpty(otp) 
            ? await client.Login(username, password)
            : await client.Login(username, password, otp);
            
        if (!success)
        {
            throw new Exception("Authentication failed");
        }
        
        app.WriteSuccess("Authentication successful");
    }
    
    return client;
}
```

---

## Output Formatting

### Table Display

```csharp
using Corsinvest.ProxmoxVE.Api.Console;

// Display data in table format
public static void DisplayVmList(IEnumerable<VmInfo> vms, ConsoleHelper app)
{
    var table = new ConsoleTable(app);
    
    // Define columns
    table.AddColumns("NODE", "VMID", "NAME", "STATUS", "CPU", "MEMORY");
    
    // Add data rows
    foreach (var vm in vms)
    {
        table.AddRow(
            vm.Node,
            vm.VmId.ToString(),
            vm.Name ?? "N/A",
            vm.Status,
            $"{vm.Cpu:P1}",
            $"{vm.Mem.ToHumanReadableSize()} / {vm.MaxMem.ToHumanReadableSize()}"
        );
    }
    
    // Display table
    table.Render();
}

// Display snapshots in table format
public static void DisplaySnapshots(IEnumerable<SnapshotInfo> snapshots, ConsoleHelper app)
{
    var table = new ConsoleTable(app);
    table.AddColumns("NAME", "DESCRIPTION", "DATE", "PARENT", "STATE");
    
    foreach (var snapshot in snapshots.OrderBy(s => s.SnapTime))
    {
        table.AddRow(
            snapshot.Name,
            snapshot.Description ?? "",
            snapshot.SnapTime.FromUnixTime().ToString("yyyy-MM-dd HH:mm:ss"),
            snapshot.Parent ?? "no-parent",
            snapshot.Running ? "with RAM" : ""
        );
    }
    
    table.Render();
}
```

### Formatted Output

```csharp
// Different output formats based on user preference
public static void DisplayResults(object data, ConsoleHelper app)
{
    var outputFormat = app.GetValue("output") ?? "table";
    
    switch (outputFormat.ToLower())
    {
        case "json":
            app.WriteJson(data);
            break;
            
        case "csv":
            app.WriteCsv(data);
            break;
            
        case "table":
        default:
            app.WriteTable(data);
            break;
    }
}

// Progress indication for long operations
public static async Task<T> WithProgress<T>(Func<Task<T>> operation, string message, ConsoleHelper app)
{
    app.WriteInfo($"{message}...");
    
    var result = await operation();
    
    app.WriteSuccess($"{message} completed");
    return result;
}
```

---

## Configuration Management

### Configuration File Support

```csharp
using Corsinvest.ProxmoxVE.Api.Console;

// Load configuration from file
public class AppConfig
{
    public string Host { get; set; }
    public string Username { get; set; }
    public string ApiToken { get; set; }
    public bool ValidateCertificate { get; set; } = false;
    public int Timeout { get; set; } = 100;
}

// Configuration helper
public static AppConfig LoadConfiguration(ConsoleHelper app)
{
    var configFile = app.GetValue("config");
    
    if (!string.IsNullOrEmpty(configFile) && File.Exists(configFile))
    {
        var json = File.ReadAllText(configFile);
        return JsonConvert.DeserializeObject<AppConfig>(json);
    }
    
    return new AppConfig();
}

// Apply configuration with command-line override
public static void ApplyConfiguration(AppConfig config, ConsoleHelper app)
{
    // Command-line arguments override config file values
    app.SetDefaultValue("host", config.Host);
    app.SetDefaultValue("username", config.Username);
    app.SetDefaultValue("api-token", config.ApiToken);
    app.SetDefaultValue("validate-certificate", config.ValidateCertificate.ToString());
    app.SetDefaultValue("timeout", config.Timeout.ToString());
}
```

### Parameter Files

```csharp
// Support for parameter files (e.g., @params.txt)
public static string[] ProcessParameterFiles(string[] args)
{
    var expandedArgs = new List<string>();
    
    foreach (var arg in args)
    {
        if (arg.StartsWith("@") && File.Exists(arg.Substring(1)))
        {
            // Load parameters from file
            var fileArgs = File.ReadAllLines(arg.Substring(1))
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            
            expandedArgs.AddRange(fileArgs);
        }
        else
        {
            expandedArgs.Add(arg);
        }
    }
    
    return expandedArgs.ToArray();
}
```

---

## Logging and Output

### Colored Console Output

```csharp
using Corsinvest.ProxmoxVE.Api.Console;

// Different output levels with colors
app.WriteInfo("Processing VMs...");           // Blue text
app.WriteSuccess("Operation completed");      // Green text
app.WriteWarning("Some VMs were skipped");    // Yellow text
app.WriteError("Failed to connect to host"); // Red text

// Debug output (only shown with --debug flag)
app.WriteDebug("API call details: GET /api2/json/nodes");

// Verbose output (only shown with --verbose flag)
app.WriteVerbose("Checking VM configuration...");

// Custom colored output
app.WriteColor("Custom message", ConsoleColor.Magenta);
```

### Progress Reporting

```csharp
// Progress bar for long operations
public static async Task ProcessVmsWithProgress(IEnumerable<VmInfo> vms, 
    Func<VmInfo, Task> operation, ConsoleHelper app)
{
    var vmList = vms.ToList();
    var total = vmList.Count;
    var completed = 0;
    
    app.WriteInfo($"Processing {total} VMs...");
    
    foreach (var vm in vmList)
    {
        try
        {
            await operation(vm);
            completed++;
            
            // Update progress
            var percentage = (double)completed / total * 100;
            app.WriteProgress($"Progress: {completed}/{total} ({percentage:F1}%)");
        }
        catch (Exception ex)
        {
            app.WriteError($"Failed to process VM {vm.VmId}: {ex.Message}");
        }
    }
    
    app.WriteSuccess($"Completed processing {completed}/{total} VMs");
}

// Simple spinner for indeterminate operations
public static async Task<T> WithSpinner<T>(Func<Task<T>> operation, string message, ConsoleHelper app)
{
    using (var spinner = app.CreateSpinner(message))
    {
        return await operation();
    }
}
```

---

## Error Handling

### Standardized Error Handling

```csharp
using Corsinvest.ProxmoxVE.Api.Console;

// Comprehensive error handling wrapper
public static async Task<int> SafeExecute(Func<Task> operation, ConsoleHelper app)
{
    try
    {
        await operation();
        return 0; // Success
    }
    catch (PveException pveEx)
    {
        app.WriteError($"Proxmox VE Error: {pveEx.Message}");
        if (app.IsDebugEnabled)
        {
            app.WriteDebug($"Stack trace: {pveEx.StackTrace}");
        }
        return 1;
    }
    catch (HttpRequestException httpEx)
    {
        app.WriteError($"Network Error: {httpEx.Message}");
        app.WriteWarning("Check your network connection and host address");
        return 2;
    }
    catch (UnauthorizedAccessException authEx)
    {
        app.WriteError($"Authentication Error: {authEx.Message}");
        app.WriteWarning("Check your credentials or API token");
        return 3;
    }
    catch (Exception ex)
    {
        app.WriteError($"Unexpected Error: {ex.Message}");
        if (app.IsDebugEnabled)
        {
            app.WriteDebug($"Exception type: {ex.GetType().Name}");
            app.WriteDebug($"Stack trace: {ex.StackTrace}");
        }
        return 99;
    }
}

// Retry logic for transient failures
public static async Task<T> WithRetry<T>(Func<Task<T>> operation, int maxRetries, TimeSpan delay, ConsoleHelper app)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            app.WriteWarning($"Attempt {attempt} failed: {ex.Message}");
            app.WriteInfo($"Retrying in {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);
        }
    }
    
    // Final attempt without catching exception
    return await operation();
}
```

---

## Common CLI Patterns

### VM/CT Selection

```csharp
// Helper for VM/CT selection by various criteria
public static async Task<IEnumerable<VmInfo>> SelectVms(PveClient client, ConsoleHelper app)
{
    var allVms = await client.GetVmsAsync();
    
    // Filter by VMID
    if (app.HasValue("vmid"))
    {
        var vmId = app.GetValue<int>("vmid");
        return allVms.Where(vm => vm.VmId == vmId);
    }
    
    // Filter by name pattern
    if (app.HasValue("name"))
    {
        var namePattern = app.GetValue("name");
        var regex = new Regex(namePattern.Replace("*", ".*"), RegexOptions.IgnoreCase);
        return allVms.Where(vm => regex.IsMatch(vm.Name ?? ""));
    }
    
    // Filter by node
    if (app.HasValue("node"))
    {
        var node = app.GetValue("node");
        return allVms.Where(vm => vm.Node.Equals(node, StringComparison.OrdinalIgnoreCase));
    }
    
    // Filter by status
    if (app.HasValue("status"))
    {
        var status = app.GetValue("status");
        return allVms.Where(vm => vm.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
    }
    
    // Return all if no filters specified
    return allVms;
}
```

### Batch Operations

```csharp
// Pattern for batch operations with confirmation
public static async Task<bool> ExecuteBatchOperation(
    IEnumerable<VmInfo> vms, 
    string operationName,
    Func<VmInfo, Task<bool>> operation,
    ConsoleHelper app)
{
    var vmList = vms.ToList();
    
    if (!vmList.Any())
    {
        app.WriteWarning("No VMs selected for operation");
        return false;
    }
    
    // Show what will be affected
    app.WriteInfo($"The following VMs will be affected by '{operationName}':");
    foreach (var vm in vmList)
    {
        app.WriteInfo($"  - {vm.Name} ({vm.VmId}) on {vm.Node}");
    }
    
    // Confirmation prompt (unless --yes flag is used)
    if (!app.HasFlag("yes") && !app.HasFlag("force"))
    {
        if (!app.Confirm($"Continue with {operationName}?"))
        {
            app.WriteInfo("Operation cancelled");
            return false;
        }
    }
    
    // Execute operation
    var success = 0;
    var failed = 0;
    
    foreach (var vm in vmList)
    {
        try
        {
            app.WriteInfo($"Processing {vm.Name} ({vm.VmId})...");
            
            if (await operation(vm))
            {
                app.WriteSuccess($"{operationName} completed for {vm.Name}");
                success++;
            }
            else
            {
                app.WriteError($"{operationName} failed for {vm.Name}");
                failed++;
            }
        }
        catch (Exception ex)
        {
            app.WriteError($"{operationName} failed for {vm.Name}: {ex.Message}");
            failed++;
        }
    }
    
    // Summary
    app.WriteInfo($"Operation completed: {success} successful, {failed} failed");
    return failed == 0;
}
```

---

## Real-World Examples

### Complete CLI Tool Example

```csharp
using Corsinvest.ProxmoxVE.Api;
using Corsinvest.ProxmoxVE.Api.Console;
using Corsinvest.ProxmoxVE.Api.Extension;

class VmManagerTool
{
    static async Task<int> Main(string[] args)
    {
        var app = new ConsoleHelper("VM Manager", "1.0.0");
        
        // Common options
        app.AddOption("--host", "Proxmox VE host", required: true);
        app.AddOption("--username", "Username");
        app.AddOption("--password", "Password");
        app.AddOption("--api-token", "API Token");
        app.AddOption("--timeout", "Timeout in seconds", type: typeof(int), defaultValue: 100);
        app.AddOption("--validate-certificate", "Validate SSL certificate", type: typeof(bool));
        
        // Selection options
        app.AddOption("--vmid", "VM ID", type: typeof(int));
        app.AddOption("--name", "VM name pattern (supports wildcards)");
        app.AddOption("--node", "Node name");
        app.AddOption("--status", "VM status filter");
        
        // Operation options
        app.AddOption("--action", "Action to perform", required: true, 
            allowedValues: new[] { "list", "start", "stop", "snapshot", "status" });
        app.AddOption("--snapshot-name", "Snapshot name");
        app.AddOption("--description", "Description");
        
        // Output options
        app.AddOption("--output", "Output format", defaultValue: "table",
            allowedValues: new[] { "table", "json", "csv" });
        app.AddFlag("--yes", "Skip confirmation prompts");
        app.AddFlag("--debug", "Enable debug output");
        app.AddFlag("--verbose", "Enable verbose output");
        
        if (!app.Parse(args))
        {
            app.ShowHelp();
            return 1;
        }
        
        return await SafeExecute(async () =>
        {
            var client = await CreateAuthenticatedClient(app);
            var vms = await SelectVms(client, app);
            var action = app.GetValue("action");
            
            switch (action)
            {
                case "list":
                    await ListVms(vms, app);
                    break;
                    
                case "start":
                    await StartVms(client, vms, app);
                    break;
                    
                case "stop":
                    await StopVms(client, vms, app);
                    break;
                    
                case "snapshot":
                    await CreateSnapshots(client, vms, app);
                    break;
                    
                case "status":
                    await ShowVmStatus(client, vms, app);
                    break;
            }
        }, app);
    }
    
    static async Task ListVms(IEnumerable<VmInfo> vms, ConsoleHelper app)
    {
        var vmList = vms.ToList();
        app.WriteInfo($"Found {vmList.Count} VMs");
        DisplayVmList(vmList, app);
    }
    
    static async Task StartVms(PveClient client, IEnumerable<VmInfo> vms, ConsoleHelper app)
    {
        await ExecuteBatchOperation(vms, "start", async vm =>
        {
            var result = await client.Nodes[vm.Node].Qemu[vm.VmId].Status.Start.VmStart();
            return result.IsSuccessStatusCode;
        }, app);
    }
    
    // Additional methods...
}
```

---

## Best Practices

### **Command-Line Design**

```csharp
// Use consistent option naming
app.AddOption("--host", "Proxmox VE host");           // Good
app.AddOption("--dry-run", "Simulate operation");     // Good

// Provide sensible defaults
app.AddOption("--timeout", "Timeout in seconds", defaultValue: 100);
app.AddOption("--output", "Output format", defaultValue: "table");

// Use validation for critical options
app.AddOption("--action", "Action to perform", required: true,
    allowedValues: new[] { "start", "stop", "restart" });
```

### **Error Handling**

```csharp
// Provide helpful error messages
try
{
    await client.Login(username, password);
}
catch (Exception ex)
{
    app.WriteError($"Authentication failed: {ex.Message}");
    app.WriteWarning("Verify your credentials and try again");
    return 1;
}

// Use appropriate exit codes
return success ? 0 : 1; // Standard success/failure codes
```

### **Output Formatting**

```csharp
// Support multiple output formats
var outputFormat = app.GetValue("output") ?? "table";
switch (outputFormat)
{
    case "json":
        app.WriteJson(data);
        break;
    case "csv":
        app.WriteCsv(data);
        break;
    default:
        app.WriteTable(data);
        break;
}
```
