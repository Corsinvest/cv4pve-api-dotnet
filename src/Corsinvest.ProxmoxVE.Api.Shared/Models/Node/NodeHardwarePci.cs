/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Hardware Pci
/// </summary>
public class NodeHardwarePci
{
    /// <summary>
    /// Subsystem Device
    /// </summary>
    /// <value></value>
    [JsonProperty("subsystem_device")]
    public string SubsystemDevice { get; set; }

    /// <summary>
    /// Subsystem Vendor
    /// </summary>
    /// <value></value>
    [JsonProperty("subsystem_vendor")]
    public string SubsystemVendor { get; set; }

    /// <summary>
    /// Device Name
    /// </summary>
    /// <value></value>
    [JsonProperty("device_name")]
    public string DeviceName { get; set; }

    /// <summary>
    /// Vendor
    /// </summary>
    /// <value></value>
    [JsonProperty("vendor")]
    public string Vendor { get; set; }

    /// <summary>
    /// Vendor Name
    /// </summary>
    /// <value></value>
    [JsonProperty("vendor_name")]
    public string VendorName { get; set; }

    /// <summary>
    /// Device
    /// </summary>
    /// <value></value>
    [JsonProperty("device")]
    public string Device { get; set; }

    /// <summary>
    /// Subsystem Vendor Name
    /// </summary>
    /// <value></value>
    [JsonProperty("subsystem_vendor_name")]
    public string SubsystemVendorName { get; set; }

    /// <summary>
    /// Class
    /// </summary>
    /// <value></value>
    [JsonProperty("class")]
    public string Class { get; set; }

    /// <summary>
    /// Iommu Group
    /// </summary>
    /// <value></value>
    [JsonProperty("iommugroup")]
    public int IommuGroup { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    /// <value></value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Subsystem Device Name
    /// </summary>
    /// <value></value>
    [JsonProperty("subsystem_device_name")]
    public string SubsystemDeviceName { get; set; }
}