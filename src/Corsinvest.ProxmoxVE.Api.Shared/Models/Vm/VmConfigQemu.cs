/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Linq;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Config data qemu
/// </summary>
public class VmConfigQemu : VmConfig
{
    /// <summary>
    /// Enable/disable ACPI.
    /// </summary>
    [JsonProperty("acpi")]
    public bool Acpi { get; set; } = true;

    /// <summary>
    /// Emulated CPU type.
    /// </summary>
    /// <value></value>
    [JsonProperty("cpu")]
    public string Cpu { get; set; }

    /// <summary>
    /// Keyboard layout for VNC server. The default is read from the'/etc/pve/datacenter.cfg' configuration file. It should not be necessary to set it.
    /// </summary>
    /// <value></value>
    [JsonProperty("keyboard")]
    public string Keyboard { get; set; }

    /// <summary>
    /// Vga.
    /// </summary>
    /// <value></value>
    [JsonProperty("vga")]
    public string Vga { get; set; }

    /// <summary>
    /// Arbitrary arguments passed to kvm.
    /// </summary>
    [JsonProperty("args")]
    public string Args { get; set; }

    /// <summary>
    /// Configure a audio device, useful in combination with QXL/Spice.
    /// </summary>
    [JsonProperty("audio0")]
    public string Audio0 { get; set; }

    /// <summary>
    /// Enable/disable communication with the Qemu Guest Agent and its properties.
    /// </summary>
    [JsonProperty("agent")]
    public string Agent { get; set; }

    /// <summary>
    /// Agent enabled.
    /// </summary>
    public bool AgentEnabled
        => !string.IsNullOrWhiteSpace(Agent) && Agent.Split(',').Select(a => a.Trim()).Any(a => a == "1");

    /// <summary>
    /// Boot Disk
    /// </summary>
    [JsonProperty("bootdisk")]
    public string BootDisk { get; set; }

    /// <summary>
    /// Startup and shutdown behavior. Order is a non-negative number defining the general startup order.
    /// Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds,
    /// which specifies a delay to wait before the next VM is started or stopped.
    /// </summary>
    [JsonProperty("startup")]
    public string StartUp { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Sockets.
    /// </summary>
    [JsonProperty("sockets")]
    public int Sockets { get; set; } = 1;

    /// <summary>
    /// SCSI controller model
    /// </summary>
    /// <value></value>
    [JsonProperty("scsihw")]
    public string ScsiHw { get; set; }

    /// <summary>
    /// Amount of target RAM for the VM in MB. Using zero disables the ballon driver.
    /// </summary>
    /// <value></value>
    [JsonProperty("balloon")]
    public int Balloon { get; set; }

    /// <summary>
    /// cloud-init: Sets DNS search domains for a container. Create will automatically
    /// use the setting from the host if neither searchdomain nor nameserver are set.
    /// </summary>
    /// <value></value>
    [JsonProperty("searchdomain")]
    public string SearchDomain { get; set; }

    /// <summary>
    /// Select BIOS implementation.
    /// </summary>
    /// <value></value>
    [JsonProperty("bios")]
    public string Bios { get; set; } = "seabios";

    /// <summary>
    /// Name.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Boot
    /// </summary>
    [JsonProperty("boot")]
    public string Boot { get; set; }

    /// <summary>
    /// Numa
    /// </summary>
    [JsonProperty("numa")]
    public bool Numa { get; set; }

    /// <summary>
    /// Enable/disable the USB tablet device. This device is usually needed to allow absolute mouse positioning with VNC.
    /// Else the mouse runs out of sync with normal VNC clients. If you're running lots of console-only guests on one host,
    /// you may consider disabling this to save some context switches. This is turned off by default if you use spice (`qm set <![CDATA[&lt;]]>vmid<![CDATA[&gt;]]> --vga qxl`).
    /// </summary>
    [JsonProperty("tablet")]
    public bool Tablet { get; set; } = true;

    /// <summary>
    /// Enable/disable \ hardware virtualization.
    /// </summary>
    /// <value></value>
    [JsonProperty("kvm")]
    public bool Kvm { get; set; } = true;

    /// <summary>
    /// et the real time clock (RTC) to local time. This is enabled by default if the `ostype` indicates a Microsoft Windows OS.
    /// </summary>
    /// <value></value>
    [JsonProperty("localtime")]
    public bool Localtime { get; set; }

    /// <summary>
    /// Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets.
    /// Number is relative to weights of all other running VMs. Using zero disables auto-ballooning.
    /// Auto-ballooning is done by pvestatd.
    /// </summary>
    /// <value></value>
    [JsonProperty("shares")]
    public int Shares { get; set; } = 1000;

    /// <summary>
    /// Specify SMBIOS type 1 fields.
    /// </summary>
    /// <value></value>
    [JsonProperty("smbios1")]
    public string Smbios1 { get; set; }

    /// <summary>
    /// The number of cores per socket.
    /// </summary>
    [JsonProperty("cores")]
    public int Cores { get; set; } = 1;
}