/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Hardware Pci
/// </summary>
public class NodeHardwarePci : ModelBase
{
    /// <summary>
    /// The Subsystem Device ID.
    /// </summary>
    [JsonProperty("subsystem_device")]
    public string SubsystemDevice { get; set; }

    /// <summary>
    /// The Subsystem Vendor ID.
    /// </summary>
    [JsonProperty("subsystem_vendor")]
    public string SubsystemVendor { get; set; }

    /// <summary>
    /// Device Name
    /// </summary>
    /// <value></value>
    [JsonProperty("device_name")]
    public string DeviceName { get; set; }

    /// <summary>
    /// The Vendor ID.
    /// </summary>
    [JsonProperty("vendor")]
    public string Vendor { get; set; }

    /// <summary>
    /// Vendor Name
    /// </summary>
    /// <value></value>
    [JsonProperty("vendor_name")]
    public string VendorName { get; set; }

    /// <summary>
    /// The Device ID.
    /// </summary>
    [JsonProperty("device")]
    public string Device { get; set; }

    /// <summary>
    /// Subsystem Vendor Name
    /// </summary>
    /// <value></value>
    [JsonProperty("subsystem_vendor_name")]
    public string SubsystemVendorName { get; set; }

    /// <summary>
    /// The PCI Class of the device.
    /// </summary>
    [JsonProperty("class")]
    public string Class { get; set; }

    /// <summary>
    /// The IOMMU group in which the device is in. If no IOMMU group is detected, it is set to -1.
    /// </summary>
    [JsonProperty("iommugroup")]
    public int IommuGroup { get; set; }

    /// <summary>
    /// The PCI ID.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Subsystem Device Name
    /// </summary>
    /// <value></value>
    [JsonProperty("subsystem_device_name")]
    public string SubsystemDeviceName { get; set; }
    /// <summary>
    /// If set, marks that the device is capable of creating mediated devices.
    /// </summary>
    [JsonProperty("mdev")]
    public bool Mdev { get; set; }
}
