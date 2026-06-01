/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Extension.Scheduling;

/// <summary>
/// Thrown when a PVE calendar event string cannot be parsed.
/// </summary>
public class PveCalendarParseException(string input, string reason)
    : Exception($"Invalid calendar event '{input}': {reason}")
{
    /// <summary>Original input string that failed to parse.</summary>
    public string Input { get; } = input;

    /// <summary>Human-readable reason.</summary>
    public string Reason { get; } = reason;
}
