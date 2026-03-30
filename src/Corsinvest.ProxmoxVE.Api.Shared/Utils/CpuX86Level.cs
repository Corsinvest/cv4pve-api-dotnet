/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils;

/// <summary>
/// x86-64 CPU compatibility level for safe live migration across cluster nodes.
/// </summary>
public class CpuX86Level : IComparable<CpuX86Level>
{
    /// <summary>x86-64-v1 — any x86-64 CPU</summary>
    public static readonly CpuX86Level V1 = new(1, "x86-64-v1");
    /// <summary>x86-64-v2-AES — Core i gen2+ / Opteron (~2008)</summary>
    public static readonly CpuX86Level V2Aes = new(2, "x86-64-v2-AES");
    /// <summary>x86-64-v3 — Haswell / Zen 1 (~2013)</summary>
    public static readonly CpuX86Level V3 = new(3, "x86-64-v3");
    /// <summary>x86-64-v4 — Skylake-X / Zen 4 (~2017)</summary>
    public static readonly CpuX86Level V4 = new(4, "x86-64-v4");

    /// <summary>Numeric level (1–4)</summary>
    public int Level { get; }

    /// <summary>Human-readable name (e.g. "x86-64-v3")</summary>
    public string Name { get; }

    private CpuX86Level(int level, string name) { Level = level; Name = name; }

    /// <inheritdoc/>
    public int CompareTo(CpuX86Level other) => Level.CompareTo(other == null ? 0 : other.Level);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
