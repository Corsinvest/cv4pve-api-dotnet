/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node certificate
/// </summary>
public class NodeCertificate : ModelBase
{
    /// <summary>
    /// Fingerprint
    /// </summary>
    [JsonProperty("fingerprint")]
    public string Fingerprint { get; set; }

    /// <summary>
    /// FileName
    /// </summary>
    [JsonProperty("filename")]
    public string FileName { get; set; }

    /// <summary>
    /// Notafter
    /// </summary>
    [JsonProperty("notafter")]
    public long NotAfter { get; set; }

    /// <summary>
    /// Issuer
    /// </summary>
    [JsonProperty("issuer")]
    public string Issuer { get; set; }

    /// <summary>
    /// PublicKeyType
    /// </summary>
    [JsonProperty("public-key-type")]
    public string PublicKeyType { get; set; }

    /// <summary>
    /// Subject
    /// </summary>
    [JsonProperty("subject")]
    public string Subject { get; set; }

    /// <summary>
    /// Pem
    /// </summary>
    [JsonProperty("pem")]
    public string Pem { get; set; }

    /// <summary>
    /// PublicKey Bits
    /// </summary>
    [JsonProperty("public-key-bits")]
    public int PublicKeyBits { get; set; }

    /// <summary>
    /// San
    /// </summary>
    [JsonProperty("san")]
    public IEnumerable<string> San { get; set; } = [];

    /// <summary>
    /// Notbefore
    /// </summary>
    [JsonProperty("notbefore")]
    public long NotBefore { get; set; }
}