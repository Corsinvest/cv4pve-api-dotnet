/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node hardware usb
/// </summary>
public class NodeHardwareUsb : ModelBase
{
    /// <summary>
    /// Dev Num
    /// </summary>
    [JsonProperty("devnum")]
    public int DevNum { get; set; }

    /// <summary>
    /// Product
    /// </summary>
    [JsonProperty("product")]
    public string Product { get; set; }

    /// <summary>
    /// Vendid
    /// </summary>
    [JsonProperty("vendid")]
    public string Vendid { get; set; }

    /// <summary>
    /// Level
    /// </summary>
    [JsonProperty("level")]
    public int Level { get; set; }

    /// <summary>
    /// Manufacturer
    /// </summary>
    [JsonProperty("manufacturer")]
    public string Manufacturer { get; set; }

    /// <summary>
    /// Bus num
    /// </summary>
    [JsonProperty("busnum")]
    public int BusNum { get; set; }

    /// <summary>
    /// ProdId
    /// </summary>
    [JsonProperty("prodid")]
    public string ProdId { get; set; }

    /// <summary>
    /// Class
    /// </summary>
    [JsonProperty("class")]
    public int Class { get; set; }

    /// <summary>
    /// Speed
    /// </summary>
    [JsonProperty("speed")]
    public string Speed { get; set; }

    /// <summary>
    /// Port
    /// </summary>
    [JsonProperty("port")]
    public int Port { get; set; }

    /// <summary>
    /// Usb path
    /// </summary>
    [JsonProperty("usbpath")]
    public string UsbPath { get; set; }
    /// <summary>
    /// Serial number.
    /// </summary>
    [JsonProperty("serial")]
    public string Serial { get; set; }
}
