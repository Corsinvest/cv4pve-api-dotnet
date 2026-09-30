/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell;

/// <summary>Runs an API call and returns its answer as data.</summary>
public static class ApiRequest
{
    /// <summary>
    /// Runs <paramref name="command"/>. API errors do not throw: they give <see cref="ApiResponse.IsSuccess"/> false.
    /// </summary>
    /// <param name="client">Client, logged in.</param>
    /// <param name="command">Call to run.</param>
    /// <param name="wait">When set and the call starts a task, wait for it and fill <see cref="ApiResponse.Task"/>.</param>
    /// <param name="cancellationToken">Stops before the call or while waiting for the task.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public static async Task<ApiResponse> ExecuteAsync(PveClient client,
                                                       ApiCommand command,
                                                       ApiWaitOptions? wait = null,
                                                       CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(command);
        cancellationToken.ThrowIfCancellationRequested();

        var parameters = new Dictionary<string, object>(command.Parameters);
        var result = command.Method switch
        {
            MethodType.Get => await client.GetAsync(command.Resource, parameters),
            MethodType.Set => await client.SetAsync(command.Resource, parameters),
            MethodType.Create => await client.CreateAsync(command.Resource, parameters),
            MethodType.Delete => await client.DeleteAsync(command.Resource, parameters),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command.Method, "Unknown method"),
        };

        // Read the answer as a dictionary: dynamic access to a missing "data" member would throw.
        var answer = result.Response as IDictionary<string, object>;
        var errors = answer != null && answer.TryGetValue("errors", out var errorsValue) && errorsValue is IDictionary<string, object> errorsMap
                        ? errorsMap.ToDictionary(a => a.Key, a => a.Value?.ToString() ?? string.Empty)
                        : [];
        var hasData = answer != null && answer.ContainsKey("data");
        var data = hasData ? answer!["data"] : null;

        // Every answer of Proxmox VE has a "data" member, null included: a 2xx without it (empty body,
        // a proxy answering JSON of its own) cannot be used.
        var isSuccess = result.IsSuccessStatusCode && !result.ResponseInError && hasData;
        var error = isSuccess
                        ? null
                        : result.IsSuccessStatusCode && !result.ResponseInError
                            ? $"The answer has no data (HTTP {(int)result.StatusCode})."
                            : result.ReasonPhrase;
        var upid = isSuccess && data is string text && text.StartsWith("UPID:", StringComparison.Ordinal) ? text : null;

        return new ApiResponse
        {
            Command = command,
            StatusCode = (int)result.StatusCode,
            IsSuccess = isSuccess,
            Error = error,
            ParameterErrors = errors,
            Data = data,
            Raw = result.Response,
            Upid = upid,
            Task = upid != null && wait != null ? await WaitAsync(client, upid, wait, cancellationToken) : null,
        };
    }

    private static async Task<ApiTaskOutcome> WaitAsync(PveClient client, string upid, ApiWaitOptions wait, CancellationToken cancellationToken)
    {
        var timeout = wait.Timeout is { } limit && limit > TimeSpan.Zero ? limit : Timeout.InfiniteTimeSpan;
        var interval = wait.Interval is { } every && every > TimeSpan.Zero ? every : TimeSpan.FromSeconds(1);
        var start = System.Diagnostics.Stopwatch.GetTimestamp();
        try
        {
            // The last check may come after the timeout: a task finished by then counts as finished.
            var running = true;
            while (running && (timeout == Timeout.InfiniteTimeSpan || System.Diagnostics.Stopwatch.GetElapsedTime(start) < timeout))
            {
                await Task.Delay(interval, cancellationToken);
                running = await client.TaskIsRunningAsync(upid);
            }

            if (running) { return new(false, null, false); }
            var exitStatus = await client.GetExitStatusTaskAsync(upid);
            return new(true, exitStatus, exitStatus?.StartsWith("OK", StringComparison.OrdinalIgnoreCase) == true);
        }
        catch (PveResultException)
        {
            // The task status cannot be read (node down, permission): how it ended is unknown.
            return new(false, null, false);
        }
    }
}
