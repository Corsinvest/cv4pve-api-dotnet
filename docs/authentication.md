# Authentication Guide

This guide covers all authentication methods available for connecting to Proxmox VE.

## Authentication Methods

### **API Token (Recommended)**

API tokens are the most secure method for automation and applications.

```csharp
var client = new PveClient("pve.example.com");

// Set API token (no login() call needed)
client.ApiToken = "user@realm!tokenid=uuid";

// Ready to use
var version = await client.Version.Version();
```

**Format:** `USER@REALM!TOKENID=UUID`

**Example:** `automation@pve!api-token=12345678-1234-1234-1234-123456789abc`

### **Username/Password**

Traditional authentication with username and password.

```csharp
var client = new PveClient("pve.example.com");

// Basic login
bool success = await client.Login("root", "password");

// Login with specific realm
bool success = await client.Login("admin@pve", "password");

// Login with PAM realm (default)
bool success = await client.Login("user@pam", "password");
```

### **Two-Factor Authentication (2FA)**

For accounts with Two-Factor Authentication enabled.

```csharp
var client = new PveClient("pve.example.com");

// Login with TOTP/OTP code
bool success = await client.Login("admin@pve", "password", "123456");

// The third parameter is the 6-digit code from your authenticator app
```

---

## Creating API Tokens

### **Via Proxmox VE Web Interface**

1. **Login** to Proxmox VE web interface
2. **Navigate** to Datacenter Permissions API Tokens
3. **Click** "Add" button
4. **Configure** token:
   - **User:** Select user (e.g., `root@pam`)
   - **Token ID:** Choose name (e.g., `api-automation`)
   - **Privilege Separation:** Uncheck for full user permissions
   - **Comment:** Optional description
5. **Click** "Add" and **copy the token** (you won't see it again!)

### **Via Command Line**

```bash
# Create API token
pveum user token add root@pam api-automation --privsep=0

# List tokens
pveum user token list root@pam

# Remove token
pveum user token remove root@pam api-automation
```

### **Example Token Creation**

```bash
# Create token for automation user
pveum user add automation@pve --password "secure-password"
pveum user token add automation@pve api-token --privsep=0 --comment "API automation"

# Grant necessary permissions
pveum aclmod / -user automation@pve -role Administrator
```

---

## Security Best Practices

### **DO's**

```csharp
// Use API tokens for automation
client.ApiToken = Environment.GetEnvironmentVariable("PROXMOX_API_TOKEN");

// Store credentials securely
var username = Environment.GetEnvironmentVariable("PROXMOX_USER");
var password = Environment.GetEnvironmentVariable("PROXMOX_PASS");

// Enable SSL validation in production
var client = new PveClient("pve.company.com")
{
    ValidateCertificate = true
};

// Use specific user accounts (not root)
await client.Login("automation@pve", password);
```

### **DON'Ts**

```csharp
// Don't hardcode credentials
await client.Login("root", "password123"); // Bad!

// Don't disable SSL validation in production
client.ValidateCertificate = false; // Only for development!

// Don't use overly permissive tokens
// Create tokens with minimal required permissions
```

---

## Permission Management

### **Creating Dedicated Users**

```bash
# Create user for API access
pveum user add api-user@pve --password "secure-password" --comment "API automation user"

# Create custom role with specific permissions
pveum role add ApiUser -privs "VM.Audit,VM.Config.Disk,VM.Config.Memory,VM.PowerMgmt,VM.Snapshot"

# Assign role to user
pveum aclmod / -user api-user@pve -role ApiUser
```

### **Common Permission Sets**

```bash
# Read-only access
pveum role add ReadOnly -privs "VM.Audit,Datastore.Audit,Sys.Audit"

# VM management
pveum role add VMManager -privs "VM.Audit,VM.Config.Disk,VM.Config.Memory,VM.PowerMgmt,VM.Snapshot,VM.Clone"

# Full administrator (use with caution)
pveum aclmod / -user user@pve -role Administrator
```

---

## Environment Configuration

### **Environment Variables**

```bash
# Set environment variables
export PROXMOX_HOST="pve.example.com"
export PROXMOX_API_TOKEN="user@pve!token=uuid"

# Or for username/password
export PROXMOX_USER="admin@pve"
export PROXMOX_PASS="secure-password"
```

### **Application Configuration**

```csharp
using Microsoft.Extensions.Configuration;

// Load from appsettings.json
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var client = new PveClient(config["Proxmox:Host"]);

// Use API token if available
var apiToken = config["Proxmox:ApiToken"];
if (!string.IsNullOrEmpty(apiToken))
{
    client.ApiToken = apiToken;
}
else
{
    // Fallback to username/password
    var username = config["Proxmox:Username"];
    var password = config["Proxmox:Password"];
    await client.Login(username, password);
}
```

### **Configuration File Example**

```json
{
  "Proxmox": {
    "Host": "pve.example.com",
    "ApiToken": "user@pve!token=uuid",
    "ValidateCertificate": true,
    "Timeout": 120
  }
}
```

---

## Troubleshooting Authentication

### **Common Issues**

#### **"Authentication Failed"**
```csharp
// Check credentials
try 
{
    bool success = await client.Login("user@pam", "password");
    if (!success)
    {
        Console.WriteLine("Invalid credentials");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Login error: {ex.Message}");
}
```

#### **"Permission Denied"**
```bash
# Check user permissions
pveum user list
pveum aclmod / -user user@pve -role Administrator
```

#### **"Invalid API Token"**
```csharp
// Verify token format
client.ApiToken = "user@realm!tokenid=uuid"; // Correct format

// Check if token exists
// Token format: USER@REALM!TOKENID=SECRET
```

### **Testing Authentication**

```csharp
public static async Task<bool> TestAuthentication(PveClient client)
{
    try
    {
        var version = await client.Version.Version();
        if (version.IsSuccessStatusCode)
        {
            Console.WriteLine("Authentication successful");
            Console.WriteLine($"Connected to Proxmox VE {version.Response.data.version}");
            return true;
        }
        else
        {
            Console.WriteLine($"Authentication failed: {version.ReasonPhrase}");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection error: {ex.Message}");
        return false;
    }
}
```

---

## Authentication Examples

### **Enterprise Setup**

```csharp
// Corporate environment with proxy and custom certificates
var handler = new HttpClientHandler()
{
    Proxy = new WebProxy("http://proxy.company.com:8080"),
    UseProxy = true,
    ServerCertificateCustomValidationCallback = ValidateCustomCert
};

var httpClient = new HttpClient(handler);
var client = new PveClient("pve.company.com", httpClient)
{
    ValidateCertificate = true,
    Timeout = TimeSpan.FromMinutes(5)
};

client.ApiToken = Environment.GetEnvironmentVariable("PROXMOX_API_TOKEN");
```

### **Home Lab Setup**

```csharp
// Simple home lab setup
var client = new PveClient("192.168.1.100")
{
    ValidateCertificate = false, // Self-signed cert
    Timeout = TimeSpan.FromMinutes(2)
};

await client.Login("root@pam", Environment.GetEnvironmentVariable("PVE_PASSWORD"));
```

### **Cloud/Automation Setup**

```csharp
// Automated deployment script
var client = new PveClient(Environment.GetEnvironmentVariable("PROXMOX_HOST"))
{
    ValidateCertificate = true
};

// Use API token for automation
client.ApiToken = Environment.GetEnvironmentVariable("PROXMOX_API_TOKEN");

// Verify connection before proceeding
if (!await TestAuthentication(client))
{
    Environment.Exit(1);
}
```
