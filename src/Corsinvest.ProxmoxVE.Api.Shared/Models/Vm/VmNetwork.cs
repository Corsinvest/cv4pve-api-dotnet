/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Network
/// </summary>
public class VmNetwork
{
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Bridge
    /// </summary>
    public string Bridge { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Queues
    /// </summary>
    public int? Queues { get; set; }

    /// <summary>
    /// Tag
    /// </summary>
    public int? Tag { get; set; }

    /// <summary>
    /// Firewall
    /// </summary>
    public bool Firewall { get; set; }

    /// <summary>
    /// Gateway
    /// </summary>
    public string Gateway { get; set; }

    /// <summary>
    /// Ip Address
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Ip Address 6
    /// </summary>
    public string IpAddress6 { get; set; }

    /// <summary>
    /// Gateway 6
    /// </summary>
    public string Gateway6 { get; set; }

    /// <summary>
    /// MacAddress
    /// </summary>
    public string MacAddress { get; set; }

    /// <summary>
    /// Model
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// Rate
    /// </summary>
    public double? Rate { get; set; }

    /// <summary>
    /// Disconnect
    /// </summary>
    public bool Disconnect { get; set; }

    /// <summary>
    /// Trunks
    /// </summary>
    public string Trunks { get; set; }

    /// <summary>
    /// Mtu
    /// </summary>
    public int? Mtu { get; set; }

    /// <summary>
    /// LinkDown
    /// </summary>
    public bool LinkDown { get; set; }
}
