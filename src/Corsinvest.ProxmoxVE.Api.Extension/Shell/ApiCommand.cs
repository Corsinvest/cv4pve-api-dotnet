/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell;

/// <summary>An API call to run: method, path and parameters.</summary>
public sealed record ApiCommand(MethodType Method, string Resource, IReadOnlyDictionary<string, object> Parameters);

/// <summary>
/// A named API call with placeholders, e.g. <c>get /nodes/{node}/qemu/{vmid}/config</c>. Where aliases come
/// from and how they are saved is up to the caller.
/// </summary>
public sealed record ApiAlias(string Name, string Description, string Command, bool Confirm = false);

/// <summary>Why an alias could not be turned into an <see cref="ApiCommand"/>.</summary>
public enum ApiCommandError
{
    /// <summary>Fewer arguments than placeholders.</summary>
    MissingArguments,

    /// <summary>More arguments than placeholders.</summary>
    UnexpectedArgument,

    /// <summary>The alias needs --yes.</summary>
    ConfirmationRequired,

    /// <summary>--guest names no VM or container of the cluster.</summary>
    GuestNotFound,

    /// <summary>A parameter is repeated or an option misses its value.</summary>
    InvalidParameter,
}

/// <summary>Result of an alias expansion: the command, or the reason it could not be built.</summary>
public sealed record ApiCommandResult(ApiCommand? Command, ApiCommandError? Error, string? Detail);
