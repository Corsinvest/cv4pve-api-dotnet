/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell;

/// <summary>How long and how often to check a task started by the call.</summary>
/// <param name="Timeout">Maximum wait; null or zero waits until the task ends.</param>
/// <param name="Interval">Time between checks; null is 1 second.</param>
public sealed record ApiWaitOptions(TimeSpan? Timeout = null, TimeSpan? Interval = null);

/// <summary>How a task ended.</summary>
/// <param name="Finished">False when the wait timed out or the status could not be read.</param>
/// <param name="ExitStatus">Exit status of Proxmox VE, e.g. "OK".</param>
/// <param name="Succeeded">The exit status starts with "OK".</param>
public sealed record ApiTaskOutcome(bool Finished, string? ExitStatus, bool Succeeded);

/// <summary>Answer of an API call, as data.</summary>
public sealed record ApiResponse
{
    /// <summary>The call that was sent.</summary>
    public required ApiCommand Command { get; init; }

    /// <summary>HTTP status.</summary>
    public int StatusCode { get; init; }

    /// <summary>2xx status and no "errors" in the answer.</summary>
    public bool IsSuccess { get; init; }

    /// <summary>Reason given by Proxmox VE when the call failed.</summary>
    public string? Error { get; init; }

    /// <summary>Errors per parameter, e.g. "foo" → "property is not defined in schema…".</summary>
    public IReadOnlyDictionary<string, string> ParameterErrors { get; init; } = new Dictionary<string, string>();

    /// <summary>"data" of the answer: object, list, value or null.</summary>
    public object? Data { get; init; }

    /// <summary>The whole answer.</summary>
    public object? Raw { get; init; }

    /// <summary>ID of the task the call started.</summary>
    public string? Upid { get; init; }

    /// <summary>How the task ended, when waited.</summary>
    public ApiTaskOutcome? Task { get; init; }
}
