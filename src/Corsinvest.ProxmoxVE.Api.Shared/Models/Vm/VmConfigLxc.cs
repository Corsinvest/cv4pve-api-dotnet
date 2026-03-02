/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Config LXC
/// </summary>
public class VmConfigLxc : VmConfig
{
    /// <summary>
    /// Set a host name for the container.
    /// </summary>
    [JsonProperty("hostname")]
    public string Hostname { get; set; }

    /// <summary>
    /// Sets DNS server IP address for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.
    /// </summary>
    [JsonProperty("nameserver")]
    public string Nameserver { get; set; }

    /// <summary>
    /// Sets DNS search domains for a container. Create will automatically use the setting from the host if you neither set searchdomain nor nameserver.
    /// </summary>
    [JsonProperty("searchdomain")]
    public string SearchDomain { get; set; }

    /// <summary>
    /// Amount of SWAP for the container in MB.
    /// </summary>
    [JsonProperty("swap")]
    public int Swap { get; set; }

    /// <summary>
    /// The number of cores assigned to the container. A container can use all available cores by default.
    /// </summary>
    [JsonProperty("cores")]
    public int Cores { get; set; }

    /// <summary>
    /// Makes the container run as unprivileged user. For creation, the default is 1. For restore, the default is the value from the backup. (Should not be modified manually.)
    /// </summary>
    [JsonProperty("unprivileged")]
    public bool Unprivileged { get; set; }

    /// <summary>
    /// Description for the Container. Shown in the web-interface CT's summary. This is saved as comment inside the configuration file.
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Limit of CPU usage. NOTE: If the computer has 2 CPUs, it has a total of '2' CPU time. Value '0' indicates no CPU limit.
    /// </summary>
    [JsonProperty("cpulimit")]
    public double CpuLimit { get; set; }

    /// <summary>
    /// CPU weight for a container, will be clamped to [1, 10000] in cgroup v2.
    /// </summary>
    [JsonProperty("cpuunits")]
    public int CpuUnits { get; set; }

    /// <summary>
    /// Script that will be executed during various steps in the containers lifetime.
    /// </summary>
    [JsonProperty("hookscript")]
    public string Hookscript { get; set; }

    /// <summary>
    /// Time zone to use in the container. If option isn't set, then nothing will be done. Can be set to 'host' to match the host time zone, or an arbitrary time zone option from /usr/share/zoneinfo/zone.tab
    /// </summary>
    [JsonProperty("timezone")]
    public string Timezone { get; set; }

    /// <summary>
    /// Specify the number of tty available to the container
    /// </summary>
    [JsonProperty("tty")]
    public int Tty { get; set; }

    /// <summary>
    /// Console mode. By default, the console command tries to open a connection to one of the available tty devices. By setting cmode to 'console' it tries to attach to /dev/console instead. If you set cmode...
    /// </summary>
    [JsonProperty("cmode")]
    public string Cmode { get; set; }

    /// <summary>
    /// Attach a console device (/dev/console) to the container.
    /// </summary>
    [JsonProperty("console")]
    public bool Console { get; set; }

    /// <summary>
    /// Try to be more verbose. For now this only enables debug log-level on start.
    /// </summary>
    [JsonProperty("debug")]
    public bool Debug { get; set; }

    /// <summary>
    /// Command to run as init, optionally with arguments; may start with an absolute path, relative path, or a binary in $PATH.
    /// </summary>
    [JsonProperty("entrypoint")]
    public string Entrypoint { get; set; }

    /// <summary>
    /// The container runtime environment as NUL-separated list. Replaces any lxc.environment.runtime entries in the config.
    /// </summary>
    [JsonProperty("env")]
    public string Env { get; set; }

    /// <summary>
    /// Allow containers access to advanced features.
    /// </summary>
    [JsonProperty("features")]
    public string Features { get; set; }

    /// <summary>
    /// Array of lxc low-level configurations ([[key1, value1], [key2, value2] ...]).
    /// </summary>
    [JsonProperty("lxc")]
    public IEnumerable<string[]> Lxc { get; set; } = [];

    /// <summary>
    /// Startup and shutdown behavior. Order is a non-negative number defining the general startup order. Shutdown in done with reverse ordering. Additionally you can set the 'up' or 'down' delay in seconds, ...
    /// </summary>
    [JsonProperty("startup")]
    public string Startup { get; set; }

    /// <summary>
    /// OS architecture type.
    /// </summary>
    [JsonProperty("arch")]
    public new string Arch { get; set; }

    /// <summary>
    /// Amount of RAM for the container in MB.
    /// </summary>
    [JsonProperty("memory")]
    public new long Memory { get; set; }

    /// <summary>
    /// OS type. This is used to setup configuration inside the container, and corresponds to the --os-type LXC option.
    /// </summary>
    [JsonProperty("ostype")]
    public new string OsType { get; set; }

    /// <summary>
    /// Tags of the Container. This is only meta information.
    /// </summary>
    [JsonProperty("tags")]
    public new string Tags { get; set; }

    /// <summary>
    /// Specifies whether a container will be started during system bootup.
    /// </summary>
    [JsonProperty("onboot")]
    public new bool OnBoot { get; set; }

    /// <summary>
    /// Lock/unlock the container.
    /// </summary>
    [JsonProperty("lock")]
    public new string Lock { get; set; }

    /// <summary>
    /// Sets the protection flag of the container. This will prevent the CT or CT's disk removal.
    /// </summary>
    [JsonProperty("protection")]
    public new bool Protection { get; set; }
}
