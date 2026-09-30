/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.Text.RegularExpressions;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell;

/// <summary>
/// Command-line logic for the API: parameters written as --key value, placeholders, and aliases.
/// Options of the caller (output format, wait, verbose…) must be removed before calling it.
/// </summary>
public static partial class ApiCommandLine
{
    /// <summary>
    /// Reads tokens in order: <c>--key=value</c>; <c>--key value</c> when the next token does not start with
    /// <c>--</c>; <c>--key</c> alone is <c>true</c>. Other tokens are positional.
    /// </summary>
    /// <exception cref="ArgumentException">A key is given more than once.</exception>
    public static (IReadOnlyList<KeyValuePair<string, string>> Parameters, IReadOnlyList<string> Positional)
        ParseParameters(IReadOnlyList<string> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        var parameters = new List<KeyValuePair<string, string>>();
        var positional = new List<string>();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (!IsKey(token))
            {
                positional.Add(token);
                continue;
            }

            var body = token[2..];
            string key, value;
            var eq = body.IndexOf('=');
            if (eq > 0)
            {
                key = body[..eq];
                value = body[(eq + 1)..];
            }
            else if (i + 1 < tokens.Count && !IsKey(tokens[i + 1]))
            {
                key = body;
                value = tokens[++i];
            }
            else
            {
                key = body;
                value = "true";
            }

            if (parameters.Any(a => string.Equals(a.Key, key, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"Parameter '--{key}' is given more than once.");
            }
            parameters.Add(new(key, value));
        }

        return (parameters, positional);
    }

    /// <summary>The <c>{…}</c> parts of a command, in order.</summary>
    public static IReadOnlyList<string> GetPlaceholders(string command)
    {
        ArgumentNullException.ThrowIfNull(command);
        return [.. PlaceholderRegex().Matches(command).Select(a => a.Groups[1].Value)];
    }

    /// <summary>
    /// Turns an alias and the tokens written after its name into the call to run. Handles <c>--yes</c>/<c>-y</c>,
    /// <c>--guest</c>/<c>-g</c> (looked up in the cluster with <paramref name="client"/>), positional arguments
    /// for the placeholders, in order, and <c>--key value</c> parameters.
    /// </summary>
    /// <param name="alias">Alias to expand.</param>
    /// <param name="tokens">Tokens after the alias name, without the caller's own options.</param>
    /// <param name="client">Client, needed only with --guest.</param>
    /// <param name="cancellationToken">Stops the lookup of --guest.</param>
    /// <exception cref="ArgumentNullException">--guest is given and <paramref name="client"/> is null.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public static async Task<ApiCommandResult> ExpandAliasAsync(ApiAlias alias,
                                                                IReadOnlyList<string> tokens,
                                                                PveClient? client = null,
                                                                CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(alias);
        ArgumentNullException.ThrowIfNull(tokens);

        string? guest = null;
        var yes = false;
        var rest = new List<string>();
        for (var i = 0; i < tokens.Count; i++)
        {
            switch (tokens[i])
            {
                case "--guest" or "-g":
                    if (i + 1 >= tokens.Count) { return Fail(ApiCommandError.InvalidParameter, $"Option '{tokens[i]}' needs a VM ID or a name."); }
                    guest = tokens[++i];
                    break;
                case "--yes" or "-y": yes = true; break;
                default: rest.Add(tokens[i]); break;
            }
        }

        if (alias.Confirm && !yes) { return Fail(ApiCommandError.ConfirmationRequired, alias.Name); }

        IReadOnlyList<KeyValuePair<string, string>> userParameters;
        IReadOnlyList<string> positional;
        try { (userParameters, positional) = ParseParameters(rest); }
        catch (ArgumentException ex) { return Fail(ApiCommandError.InvalidParameter, ex.Message); }

        var command = alias.Command;
        if (guest != null)
        {
            ArgumentNullException.ThrowIfNull(client);
            cancellationToken.ThrowIfCancellationRequested();
            var found = await FindGuestAsync(client, command, guest);
            if (found == null) { return Fail(ApiCommandError.GuestNotFound, guest); }
            command = command.Replace("{node}", found.Node)
                             .Replace("{vmid}", found.VmId.ToString())
                             .Replace("{vmtype}", found.VmType == VmType.Lxc ? "lxc" : "qemu");
        }

        var placeholders = GetPlaceholders(command);
        if (positional.Count > placeholders.Count) { return Fail(ApiCommandError.UnexpectedArgument, positional[placeholders.Count]); }
        if (positional.Count < placeholders.Count)
        {
            return Fail(ApiCommandError.MissingArguments, string.Join(", ", placeholders.Skip(positional.Count).Select(a => $"{{{a}}}")));
        }

        // Split the alias command first, then fill each token: a value with spaces stays one token.
        // A value that goes into the path (the second token) must stay one segment of it: "/", "..", "?"
        // or "#" would call another endpoint.
        var next = 0;
        string? notASegment = null;
        var parts = SplitCommand(command).Select((part, index) => PlaceholderRegex().Replace(part, _ =>
                                         {
                                             var value = positional[next++];
                                             if (index == 1 && !IsPathSegment(value)) { notASegment ??= value; }
                                             return value;
                                         }))
                                         .ToList();
        if (notASegment != null)
        {
            return Fail(ApiCommandError.InvalidParameter,
                        $"'{notASegment}' cannot be part of the path: it cannot be empty, '.' or '..', or contain / \\ ? # %.");
        }

        if (parts.Count < 2 || !TryParseMethod(parts[0], out var method))
        {
            return Fail(ApiCommandError.InvalidParameter, $"Alias '{alias.Name}' has no valid method and path: '{alias.Command}'.");
        }

        var merged = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        try
        {
            var (aliasParameters, stray) = ParseParameters(parts.Skip(2).ToList());
            if (stray.Count > 0)
            {
                return Fail(ApiCommandError.InvalidParameter, $"Alias '{alias.Name}' has a value without --key: '{stray[0]}'.");
            }

            foreach (var item in aliasParameters.Concat(userParameters))
            {
                if (!merged.TryAdd(item.Key, item.Value))
                {
                    return Fail(ApiCommandError.InvalidParameter, $"Parameter '--{item.Key}' is given more than once.");
                }
            }
        }
        catch (ArgumentException ex) { return Fail(ApiCommandError.InvalidParameter, ex.Message); }

        return new(new ApiCommand(method, parts[1], merged), null, null);
    }

    private static ApiCommandResult Fail(ApiCommandError error, string detail) => new(null, error, detail);

    private static bool IsPathSegment(string value)
        => value.Length > 0 && value is not ("." or "..") && value.IndexOfAny(['/', '\\', '?', '#', '%']) < 0;

    /// <summary>Splits an alias command on spaces, keeping text between single or double quotes as one token.</summary>
    private static List<string> SplitCommand(string command)
    {
        var ret = new List<string>();
        var current = new System.Text.StringBuilder();
        char? quote = null;
        foreach (var c in command)
        {
            if (quote.HasValue)
            {
                if (c == quote.Value) { quote = null; }
                else { current.Append(c); }
            }
            else if (c is '"' or '\'') { quote = c; }
            else if (c == ' ')
            {
                if (current.Length > 0) { ret.Add(current.ToString()); current.Clear(); }
            }
            else { current.Append(c); }
        }
        if (current.Length > 0) { ret.Add(current.ToString()); }
        return ret;
    }

    private static bool TryParseMethod(string verb, out MethodType method)
    {
        switch (verb.ToLowerInvariant())
        {
            case "get": method = MethodType.Get; return true;
            case "set" or "put": method = MethodType.Set; return true;
            case "create" or "post": method = MethodType.Create; return true;
            case "delete": method = MethodType.Delete; return true;
            default: method = default; return false;
        }
    }

    /// <summary>
    /// Finds a guest by VM ID or name (without case, first match). A path <c>/nodes/{node}/qemu/{vmid}</c>
    /// looks only among VMs, <c>/lxc/</c> only among containers, anything else among both.
    /// </summary>
    private static async Task<ClusterResource?> FindGuestAsync(PveClient client, string command, string idOrName)
    {
        var path = SplitCommand(command).ElementAtOrDefault(1) ?? string.Empty;
        VmType? only = path.Contains("/{node}/qemu/{vmid}", StringComparison.OrdinalIgnoreCase)
                        ? VmType.Qemu
                        : path.Contains("/{node}/lxc/{vmid}", StringComparison.OrdinalIgnoreCase)
                            ? VmType.Lxc
                            : null;

        var guests = (await client.Cluster.Resources.GetAsync(ClusterResourceType.Vm))
                        .Where(a => !a.IsUnknown && (only == null || a.VmType == only));

        return long.TryParse(idOrName, out var vmId)
                ? guests.FirstOrDefault(a => a.VmId == vmId)
                : guests.FirstOrDefault(a => string.Equals(a.Name, idOrName, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsKey(string token) => token.Length > 2 && token.StartsWith("--", StringComparison.Ordinal);

    [GeneratedRegex(@"{\s*(.+?)\s*}")]
    private static partial Regex PlaceholderRegex();
}
