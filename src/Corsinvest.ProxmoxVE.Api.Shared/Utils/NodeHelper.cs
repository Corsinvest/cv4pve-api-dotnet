/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils;

/// <summary>
/// Node Helper
/// </summary>
public static class NodeHelper
{
    /// <summary>
    /// Decode level support
    /// </summary>
    public static NodeLevel DecodeLevelSupport(string level)
        => level switch
        {
            "c" => NodeLevel.Community,
            "p" => NodeLevel.Premium,
            "b" => NodeLevel.Basic,
            "s" => NodeLevel.Standard,
            _ => NodeLevel.None,
        };

    /// <summary>
    /// Calculates the x86-64 CPU compatibility level from a CPU flags string
    /// (as returned by NodeStatus.CpuInfo.Flags).
    /// The level determines the safest QEMU CPU type for live migration.
    /// </summary>
    public static CpuX86Level GetCpuX86Level(string flags)
    {
        if (string.IsNullOrWhiteSpace(flags)) { return CpuX86Level.V1; }

        var f = new HashSet<string>(flags.Split(' '), StringComparer.OrdinalIgnoreCase);

        var v2 = new[] { "cx16", "lahf_lm", "popcnt", "sse4_1", "sse4_2", "ssse3" };
        var v3 = new[] { "avx", "avx2", "bmi1", "bmi2", "fma", "f16c", "abm", "movbe" };
        var v4 = new[] { "avx512f", "avx512bw", "avx512cd", "avx512dq", "avx512vl" };

        if (!v2.All(f.Contains)) { return CpuX86Level.V1; }
        if (!v3.All(f.Contains)) { return CpuX86Level.V2Aes; }
        if (!v4.All(f.Contains)) { return CpuX86Level.V3; }
        return CpuX86Level.V4;
    }
}