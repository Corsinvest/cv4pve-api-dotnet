/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

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
    [JsonProperty("cpu")]
    public string Cpu { get; set; }

    /// <summary>
    /// Keyboard layout for VNC server. This option is generally not required and is often better handled from within the guest OS.
    /// </summary>
    [JsonProperty("keyboard")]
    public string Keyboard { get; set; }

    /// <summary>
    /// Configure the VGA hardware.
    /// </summary>
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
    /// Enable/disable communication with the QEMU Guest Agent and its properties.
    /// </summary>
    [JsonProperty("agent")]
    public string Agent { get; set; }

    /// <summary>
    /// Agent enabled.
    /// </summary>
    public bool AgentEnabled
        => !string.IsNullOrWhiteSpace(Agent) && Agent.Split(',').Select(a => a.Trim()).Any(a => a == "1");

    /// <summary>
    /// Enable booting from specified disk. Deprecated: Use 'boot: order=foo;bar' instead.
    /// </summary>
    [JsonProperty("bootdisk")]
    public string BootDisk { get; set; }

    /// <summary>
    /// Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, ...
    /// </summary>
    [JsonProperty("startup")]
    public string StartUp { get; set; }

    /// <summary>
    /// Description for the VM. Shown in the web-interface VM's summary. This is saved as comment inside the configuration file.
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// The number of CPU sockets.
    /// </summary>
    [JsonProperty("sockets")]
    public int Sockets { get; set; } = 1;

    /// <summary>
    /// SCSI controller model
    /// </summary>
    [JsonProperty("scsihw")]
    public string ScsiHw { get; set; }

    /// <summary>
    /// Amount of target RAM for the VM in MiB. Using zero disables the ballon driver.
    /// </summary>
    [JsonProperty("balloon")]
    public int Balloon { get; set; }

    /// <summary>
    /// cloud-init: Sets DNS search domains for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.
    /// </summary>
    [JsonProperty("searchdomain")]
    public string SearchDomain { get; set; }

    /// <summary>
    /// Select BIOS implementation.
    /// </summary>
    [JsonProperty("bios")]
    public string Bios { get; set; } = "seabios";

    /// <summary>
    /// Set a name for the VM. Only used on the configuration web interface.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Specify guest boot order. Use the 'order=' sub-property as usage with no key or 'legacy=' is deprecated.
    /// </summary>
    [JsonProperty("boot")]
    public string Boot { get; set; }

    /// <summary>
    /// Enable/disable NUMA.
    /// </summary>
    [JsonProperty("numa")]
    public bool Numa { get; set; }

    /// <summary>
    /// Enable/disable the USB tablet device.
    /// </summary>
    [JsonProperty("tablet")]
    public bool Tablet { get; set; } = true;

    /// <summary>
    /// Enable/disable KVM hardware virtualization.
    /// </summary>
    [JsonProperty("kvm")]
    public bool Kvm { get; set; } = true;

    /// <summary>
    /// Set the real time clock (RTC) to local time. This is enabled by default if the `ostype` indicates a Microsoft Windows OS.
    /// </summary>
    [JsonProperty("localtime")]
    public bool Localtime { get; set; }

    /// <summary>
    /// Amount of memory shares for auto-ballooning. The larger the number is, the more memory this VM gets. Number is relative to weights of all other running VMs. Using zero disables auto-ballooning. Auto-b...
    /// </summary>
    [JsonProperty("shares")]
    public int Shares { get; set; } = 1000;

    /// <summary>
    /// Specify SMBIOS type 1 fields.
    /// </summary>
    [JsonProperty("smbios1")]
    public string Smbios1 { get; set; }

    /// <summary>
    /// The number of cores per socket.
    /// </summary>
    [JsonProperty("cores")]
    public int Cores { get; set; } = 1;

    /// <summary>
    /// Limit of CPU usage.
    /// </summary>
    [JsonProperty("cpulimit")]
    public double CpuLimit { get; set; }

    /// <summary>
    /// CPU weight for a VM, will be clamped to [1, 10000] in cgroup v2.
    /// </summary>
    [JsonProperty("cpuunits")]
    public int CpuUnits { get; set; }

    /// <summary>
    /// Number of hotplugged vcpus.
    /// </summary>
    [JsonProperty("vcpus")]
    public int Vcpus { get; set; }

    /// <summary>
    /// Script that will be executed during various steps in the vms lifetime.
    /// </summary>
    [JsonProperty("hookscript")]
    public string Hookscript { get; set; }

    /// <summary>
    /// cloud-init: Sets DNS server IP address for a container. Create will automatically use the setting from the host if neither searchdomain nor nameserver are set.
    /// </summary>
    [JsonProperty("nameserver")]
    public string Nameserver { get; set; }

    /// <summary>
    /// Allow reboot. If set to '0' the VM exit on reboot.
    /// </summary>
    [JsonProperty("reboot")]
    public bool Reboot { get; set; }

    /// <summary>
    /// Enable/disable time drift fix.
    /// </summary>
    [JsonProperty("tdf")]
    public bool Tdf { get; set; }

    /// <summary>
    /// List of host cores used to execute guest processes, for example: 0,5,8-11
    /// </summary>
    [JsonProperty("affinity")]
    public string Affinity { get; set; }

    /// <summary>
    /// Allow memory pages of this guest to be merged via KSM (Kernel Samepage Merging).
    /// </summary>
    [JsonProperty("allow-ksm")]
    public bool? AllowKsm { get; set; }

    /// <summary>
    /// Secure Encrypted Virtualization (SEV) features by AMD CPUs
    /// </summary>
    [JsonProperty("amd-sev")]
    public string AmdSev { get; set; }

    /// <summary>
    /// Automatic restart after crash (currently ignored).
    /// </summary>
    [JsonProperty("autostart")]
    public bool Autostart { get; set; }

    /// <summary>
    /// cloud-init: Specify custom files to replace the automatically generated ones at start.
    /// </summary>
    [JsonProperty("cicustom")]
    public string Cicustom { get; set; }

    /// <summary>
    /// cloud-init: Password to assign the user. Using this is generally not recommended. Use ssh keys instead. Also note that older cloud-init versions do not support hashed passwords.
    /// </summary>
    [JsonProperty("cipassword")]
    public string Cipassword { get; set; }

    /// <summary>
    /// Specifies the cloud-init configuration format. The default depends on the configured operating system type (`ostype`. We use the `nocloud` format for Linux, and `configdrive2` for windows.
    /// </summary>
    [JsonProperty("citype")]
    public string Citype { get; set; }

    /// <summary>
    /// cloud-init: do an automatic package upgrade after the first boot.
    /// </summary>
    [JsonProperty("ciupgrade")]
    public bool Ciupgrade { get; set; }

    /// <summary>
    /// cloud-init: User name to change ssh keys and password for instead of the image's configured default user.
    /// </summary>
    [JsonProperty("ciuser")]
    public string Ciuser { get; set; }

    /// <summary>
    /// Configure a disk for storing EFI vars.
    /// </summary>
    [JsonProperty("efidisk0")]
    public string Efidisk0 { get; set; }

    /// <summary>
    /// Freeze CPU at startup (use 'c' monitor command to start execution).
    /// </summary>
    [JsonProperty("freeze")]
    public bool Freeze { get; set; }

    /// <summary>
    /// Selectively enable hotplug features. This is a comma separated list of hotplug features: 'network', 'disk', 'cpu', 'memory', 'usb' and 'cloudinit'. Use '0' to disable hotplug completely. Using '1' as ...
    /// </summary>
    [JsonProperty("hotplug")]
    public string Hotplug { get; set; }

    /// <summary>
    /// Enables hugepages memory. Sets the size of hugepages in MiB. If the value is set to 'any' then 1 GiB hugepages will be used if possible, otherwise the size will fall back to 2 MiB.
    /// </summary>
    [JsonProperty("hugepages")]
    public string Hugepages { get; set; }

    /// <summary>
    /// Trusted Domain Extension (TDX) features by Intel CPUs
    /// </summary>
    [JsonProperty("intel-tdx")]
    public string IntelTdx { get; set; }

    /// <summary>
    /// Inter-VM shared memory. Useful for direct communication between VMs, or to the host.
    /// </summary>
    [JsonProperty("ivshmem")]
    public string Ivshmem { get; set; }

    /// <summary>
    /// Use together with hugepages. If enabled, hugepages will not not be deleted after VM shutdown and can be used for subsequent starts.
    /// </summary>
    [JsonProperty("keephugepages")]
    public bool Keephugepages { get; set; }

    /// <summary>
    /// Specify the QEMU machine.
    /// </summary>
    [JsonProperty("machine")]
    public string Machine { get; set; }

    /// <summary>
    /// Some (read-only) meta-information about this guest.
    /// </summary>
    [JsonProperty("meta")]
    public string Meta { get; set; }

    /// <summary>
    /// Set maximum tolerated downtime (in seconds) for migrations. Should the migration not be able to converge in the very end, because too much newly dirtied RAM needs to be transferred, the limit will be ...
    /// </summary>
    [JsonProperty("migrate_downtime")]
    public double MigrateDowntime { get; set; }

    /// <summary>
    /// Set maximum speed (in MB/s) for migrations. Value 0 is no limit.
    /// </summary>
    [JsonProperty("migrate_speed")]
    public int MigrateSpeed { get; set; }

    /// <summary>
    /// Configure a VirtIO-based Random Number Generator.
    /// </summary>
    [JsonProperty("rng0")]
    public string Rng0 { get; set; }

    /// <summary>
    /// List of VirtIO network devices and their effective host_mtu setting. A value of 0 means that the host_mtu parameter is to be avoided for the corresponding device. This is used internally for snapshots...
    /// </summary>
    [JsonProperty("running-nets-host-mtu")]
    public string RunningNetsHostMtu { get; set; }

    /// <summary>
    /// Specifies the QEMU '-cpu' parameter of the running vm. This is used internally for snapshots.
    /// </summary>
    [JsonProperty("runningcpu")]
    public string RunningCpu { get; set; }

    /// <summary>
    /// Specifies the QEMU machine type of the running vm. This is used internally for snapshots.
    /// </summary>
    [JsonProperty("runningmachine")]
    public string RunningMachine { get; set; }

    /// <summary>
    /// The number of CPUs. Please use option -sockets instead.
    /// </summary>
    [JsonProperty("smp")]
    public int Smp { get; set; }

    /// <summary>
    /// Timestamp for snapshots.
    /// </summary>
    [JsonProperty("snaptime")]
    public long Snaptime { get; set; }

    /// <summary>
    /// Configure additional enhancements for SPICE.
    /// </summary>
    [JsonProperty("spice_enhancements")]
    public string SpiceEnhancements { get; set; }

    /// <summary>
    /// cloud-init: Setup public SSH keys (one key per line, OpenSSH format).
    /// </summary>
    [JsonProperty("sshkeys")]
    public string Sshkeys { get; set; }

    /// <summary>
    /// Set the initial date of the real time clock. Valid format for date are:'now' or '2006-06-17T16:01:21' or '2006-06-17'.
    /// </summary>
    [JsonProperty("startdate")]
    public string Startdate { get; set; }

    /// <summary>
    /// Configure a Disk for storing TPM state. The format is fixed to 'raw'.
    /// </summary>
    [JsonProperty("tpmstate0")]
    public string Tpmstate0 { get; set; }

    /// <summary>
    /// Set VM Generation ID. Use '1' to autogenerate on create or update, pass '0' to disable explicitly.
    /// </summary>
    [JsonProperty("vmgenid")]
    public string Vmgenid { get; set; }

    /// <summary>
    /// Reference to a volume which stores the VM state. This is used internally for snapshots.
    /// </summary>
    [JsonProperty("vmstate")]
    public string Vmstate { get; set; }

    /// <summary>
    /// Default storage for VM state volumes/files.
    /// </summary>
    [JsonProperty("vmstatestorage")]
    public string Vmstatestorage { get; set; }

    /// <summary>
    /// Create a virtual hardware watchdog device.
    /// </summary>
    [JsonProperty("watchdog")]
    public string Watchdog { get; set; }
}