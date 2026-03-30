/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Qemu Agent NetworkGetInterfaces
/// </summary>
public class VmQemuAgentNetworkGetInterfaces : ModelBase
{
    /// <summary>
    /// Result
    /// </summary>
    [JsonProperty("result")]
    public IEnumerable<ResultInt> Result { get; set; } = [];

    /// <summary>
    /// IpAddress
    /// </summary>
    public class Ip
    {
        /// <summary>
        /// IpAddress Type
        /// </summary>
        [JsonProperty("ip-address-type")]
        public string IpAddressType { get; set; }

        /// <summary>
        /// IpAddress
        /// </summary>
        [JsonProperty("ip-address")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Prefix
        /// </summary>
        [JsonProperty("prefix")]
        public int Prefix { get; set; }
    }

    /// <summary>
    /// Statistics
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// Rx Packets
        /// </summary>
        [JsonProperty("rx-packets")]
        public long RxPackets { get; set; }

        /// <summary>
        /// Rx Bytes
        /// </summary>
        [JsonProperty("rx-bytes")]
        public long RxBytes { get; set; }

        /// <summary>
        /// Tx Errors
        /// </summary>
        [JsonProperty("tx-errs")]
        public long TxErrors { get; set; }

        /// <summary>
        /// Tx Dropped
        /// </summary>
        [JsonProperty("tx-dropped")]
        public long TxDropped { get; set; }

        /// <summary>
        /// Rx Dropped
        /// </summary>
        [JsonProperty("rx-dropped")]
        public long RxDropped { get; set; }

        /// <summary>
        /// Rx Errors
        /// </summary>
        [JsonProperty("rx-errs")]
        public long RxErrors { get; set; }

        /// <summary>
        /// Tx Bytes
        /// </summary>
        [JsonProperty("tx-bytes")]
        public long TxBytes { get; set; }

        /// <summary>
        /// Tx Packets
        /// </summary>
        [JsonProperty("tx-packets")]
        public long TxPackets { get; set; }
    }

    /// <summary>
    /// Result
    /// </summary>
    public class ResultInt
    {
        /// <summary>
        /// IpAddresses
        /// </summary>
        [JsonProperty("ip-addresses")]
        public IEnumerable<Ip> IpAddresses { get; set; } = [];

        /// <summary>
        /// Statistics
        /// </summary>
        [JsonProperty("statistics")]
        public Statistics Statistics { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// HardwareAddress
        /// </summary>
        [JsonProperty("hardware-address")]
        public string HardwareAddress { get; set; }
    }
}