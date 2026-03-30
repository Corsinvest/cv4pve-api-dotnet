/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Pressure Info
/// </summary>
public interface IPressureInfo
{
    /// <summary>PSI CPU pressure (some) — requires PVE 9.0+.</summary>
    [JsonProperty("pressurecpusome")]
    double PressureCpuSome { get; set; }

    /// <summary>PSI I/O pressure (some) — requires PVE 9.0+.</summary>
    [JsonProperty("pressureiosome")]
    double PressureIoSome { get; set; }

    /// <summary>PSI I/O pressure (full) — requires PVE 9.0+.</summary>
    [JsonProperty("pressureiofull")]
    double PressureIoFull { get; set; }

    /// <summary>PSI memory pressure (some) — requires PVE 9.0+.</summary>
    [JsonProperty("pressurememorysome")]
    double PressureMemorySome { get; set; }

    /// <summary>PSI memory pressure (full) — requires PVE 9.0+.</summary>
    [JsonProperty("pressurememoryfull")]
    double PressureMemoryFull { get; set; }
}