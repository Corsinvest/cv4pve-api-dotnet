/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Config
/// </summary>
public class VmConfig : ModelBase
{
    /// <summary>
    /// Architecture type.
    /// </summary>
    [JsonProperty("arch")]
    public string Arch { get; set; }

    /// <summary>
    /// Parent
    /// </summary>
    [JsonProperty("parent")]
    public string Parent { get; set; }

    /// <summary>
    /// Memory
    /// </summary>
    [JsonProperty("memory")]
    public long Memory { get; set; }

    /// <summary>
    /// Os type
    /// </summary>
    [JsonProperty("ostype")]
    public string OsType { get; set; }

    /// <summary>
    /// Vm Os Type
    /// </summary>
    public VmOsType VmOsType
        => OsType switch
        {
            "l26" or "l24" => VmOsType.Linux,
            "win11" or "win10" or "win8" or "win7" or "w2k8" or "wxp" or "w2k" => VmOsType.Windows,
            "solaris" => VmOsType.Solaris,
            "other" => VmOsType.Other,
            _ => VmOsType.Linux,
        };

    /// <summary>
    /// OsType Decode
    /// </summary>
    public string OsTypeDecode
        => OsType switch
        {
            "l26" => "Linux 5.x - 2.6 Kernel",
            "l24" => "Linux 2.4 Kernel",
            "win11" => "Microsoft Windows 11/2022/2025",
            "win10" => "Microsoft Windows 10/2016/2019",
            "win8" => "Microsoft Windows 8.x/2012/2012r2",
            "win7" => "Microsoft Windows 7/2008r2",
            "wvista" => "Microsoft Windows Vista",
            "w2k8" => "Microsoft Windows Vista/2008",
            "w2k3" => "Microsoft Windows 2003",
            "wxp" => "Microsoft Windows XP/2003",
            "w2k" => "Microsoft Windows 2000",
            "solaris" => "Solaris Kernel",
            "other" => "Other",
            _ => OsType,
        };

    /// <summary>
    /// Lock
    /// </summary>
    public bool IsLocked => !string.IsNullOrWhiteSpace(Lock);

    /// <summary>
    /// On boot
    /// </summary>
    [JsonProperty("onboot")]
    public bool OnBoot { get; set; }

    /// <summary>
    /// On boot
    /// </summary>
    [JsonProperty("protection")]
    public bool Protection { get; set; }

    /// <summary>
    /// On boot
    /// </summary>
    [JsonProperty("lock")]
    public string Lock { get; set; }

    /// <summary>
    /// Disks
    /// </summary>
    public IEnumerable<VmDisk> Disks { get; private set; } = [];

    /// <summary>
    /// Networks
    /// </summary>
    public IEnumerable<VmNetwork> Networks { get; private set; } = [];

    [OnDeserialized]
    internal void OnSerializedMethod(StreamingContext context)
    {
        ReadDisks();
        ReadNetworks();
    }

    private void ReadNetworks()
    {
        var networks = new List<VmNetwork>();

        foreach (var key in ExtensionData.Keys)
        {
            if (key.StartsWith("net"))
            {
                var network = new VmNetwork();
                network.Id = key;

                foreach (var item in (ExtensionData[key] + "").Split(','))
                {
                    Match match;
                    if (this is VmConfigQemu)
                    {
                        if ((match = Regex.Match(item, "^(ne2k_pci|e1000e?|e1000-82540em|e1000-82544gc|e1000-82545em|vmxnet3|rtl8139|pcnet|virtio|ne2k_isa|i82551|i82557b|i82559er)(=([0-9a-f]{2}(:[0-9a-f]{2}){5}))?$", RegexOptions.IgnoreCase)).Success)
                        {
                            network.Model = match.Groups[1].Value.ToLower();
                            if (match.Groups[3].Success) { network.MacAddress = match.Groups[3].Value; }
                        }
                        else if ((match = Regex.Match(item, @"^bridge=(\S+)$")).Success)
                        {
                            network.Bridge = match.Groups[1].Value;
                        }
                        else if ((match = Regex.Match(item, @"^rate=(\d+(\.\d+)?|\.\d+)$")).Success)
                        {
                            network.Rate = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                        }
                        else if ((match = Regex.Match(item, @"^tag=(\d+(\.\d+)?)$")).Success)
                        {
                            network.Tag = int.Parse(match.Groups[1].Value);
                        }
                        else if ((match = Regex.Match(item, @"^firewall=(\d+)$")).Success)
                        {
                            network.Firewall = match.Groups[1].Value == "1";
                        }
                        else if ((match = Regex.Match(item, @"^link_down=(\d+)$")).Success)
                        {
                            network.Disconnect = match.Groups[1].Value == "1";
                        }
                        else if ((match = Regex.Match(item, @"^queues=(\d+)$")).Success)
                        {
                            network.Queues = int.Parse(match.Groups[1].Value);
                        }
                        else if ((match = Regex.Match(item, @"^trunks=(\d+(?:-\d+)?(?:;\d+(?:-\d+)?)*)$")).Success)
                        {
                            network.Trunks = match.Groups[1].Value;
                        }
                        else if ((match = Regex.Match(item, @"^mtu=(\d+)$")).Success)
                        {
                            network.Mtu = int.Parse(match.Groups[1].Value);
                        }
                    }
                    else if (this is VmConfigLxc)
                    {
                        if ((match = Regex.Match(item, @"^(bridge)=(\S+)$")).Success)
                        {
                            network.Bridge = match.Groups[2].Value;
                        }
                        else if ((match = Regex.Match(item, @"^(gw)=(\S+)$")).Success)
                        {
                            network.Gateway = match.Groups[2].Value;
                        }
                        else if ((match = Regex.Match(item, @"^(gw6)=(\S+)$")).Success)
                        {
                            network.Gateway6 = match.Groups[2].Value;
                        }
                        else if ((match = Regex.Match(item, @"^(hwaddr)=(\S+)$")).Success)
                        {
                            network.MacAddress = match.Groups[2].Value;
                        }
                        else if ((match = Regex.Match(item, @"^(rate)=(\S+)$")).Success)
                        {
                            network.Rate = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                        }
                        else if ((match = Regex.Match(item, @"^(tag)=(\S+)$")).Success)
                        {
                            network.Tag = int.Parse(match.Groups[2].Value);
                        }
                        else if ((match = Regex.Match(item, @"^(mtu)=(\S+)$")).Success)
                        {
                            network.Mtu = int.Parse(match.Groups[2].Value);
                        }
                        else if ((match = Regex.Match(item, @"^(name)=(\S+)$")).Success)
                        {
                            network.Name = match.Groups[2].Value;
                        }
                        else if ((match = Regex.Match(item, @"^(ip)=(\S+)$")).Success)
                        {
                            network.IpAddress = match.Groups[2].Value;
                        }
                        else if ((match = Regex.Match(item, @"^(ip6)=(\S+)$")).Success)
                        {
                            network.IpAddress6 = match.Groups[2].Value;
                        }
                        else if ((match = Regex.Match(item, @"^firewall=(\d+)$")).Success)
                        {
                            network.Firewall = match.Groups[1].Value == "1";
                        }
                        else if ((match = Regex.Match(item, @"^link_down=(\d+)$")).Success)
                        {
                            network.LinkDown = match.Groups[1].Value == "1";
                        }
                    }
                }

                networks.Add(network);
            }
        }

        Networks = networks;
    }

    private void ReadDisks()
    {
        var disks = new List<VmDisk>();
        foreach (var key in ExtensionData.Keys)
        {
            var def = ExtensionData[key] + "";
            if (key == "rootfs"
                //bus match
                || (Regex.IsMatch(key, @"(efidisk|tpmstate|virtio|ide|scsi|sata|mp)\d+") && !Regex.IsMatch(def, "media=cdrom")))
            {
                var infos = def.Split(',');
                var storage = string.Empty;
                var fileName = string.Empty;
                var passthrough = false;
                var device = string.Empty;
                var mountSourcePath = string.Empty;

                if (infos[0].Contains(":"))
                {
                    var data = infos[0].Split(':');
                    storage = data[0];
                    fileName = data[1];
                }
                else if (infos[0].StartsWith("/dev"))
                {
                    passthrough = true;
                    device = infos[0];
                }
                else if (key.StartsWith("mp"))
                {
                    mountSourcePath = infos[0];
                }
                else
                {
                    storage = infos[0];
                }

                var backup = false;
                if (this is VmConfigQemu)
                {
                    backup = !infos.Contains("backup=0");
                }
                else if (this is VmConfigLxc)
                {
                    backup = infos.Contains("backup=1");
                }

                disks.Add(new VmDisk
                {
                    Id = key,
                    Storage = storage,
                    FileName = fileName,
                    Device = device,
                    Passthrough = passthrough,
                    Size = infos.Where(a => a.StartsWith("size=")).Select(a => a[5..]).FirstOrDefault(),
                    MountPoint = infos.Where(a => a.StartsWith("mp=")).Select(a => a[3..]).FirstOrDefault(),
                    MountSourcePath = mountSourcePath,
                    Backup = backup
                });
            }
        }
        Disks = disks;
    }
}