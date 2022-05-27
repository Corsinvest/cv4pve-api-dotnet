/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node hardware usb
    /// </summary>
    public class NodeHardwareUsb
    {
        /// <summary>
        /// Dev Num
        /// </summary>
        /// <value></value>
        [JsonProperty("devnum")]
        public int DevNum { get; set; }

        /// <summary>
        /// Product
        /// </summary>
        /// <value></value>
        [JsonProperty("product")]
        public string Product { get; set; }

        /// <summary>
        /// Vendid
        /// </summary>
        /// <value></value>
        [JsonProperty("vendid")]
        public string Vendid { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        /// <value></value>
        [JsonProperty("level")]
        public int Level { get; set; }

        /// <summary>
        /// Manufacturer
        /// </summary>
        /// <value></value>
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Bus num
        /// </summary>
        /// <value></value>
        [JsonProperty("busnum")]
        public int BusNum { get; set; }

        /// <summary>
        /// ProdId
        /// </summary>
        /// <value></value>
        [JsonProperty("prodid")]
        public string ProdId { get; set; }

        /// <summary>
        /// Class
        /// </summary>
        /// <value></value>
        [JsonProperty("class")]
        public int Class { get; set; }

        /// <summary>
        /// Speed
        /// </summary>
        /// <value></value>
        [JsonProperty("speed")]
        public string Speed { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        /// <value></value>
        [JsonProperty("port")]
        public int Port { get; set; }

        /// <summary>
        /// Usb path
        /// </summary>
        /// <value></value>
        [JsonProperty("usbpath")]
        public string UsbPath { get; set; }
    }
}