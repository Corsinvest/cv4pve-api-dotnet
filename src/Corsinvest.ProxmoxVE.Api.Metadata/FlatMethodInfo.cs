/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>Flat cache method info</summary>
public record FlatMethodInfo(
    string? Comment,
    string? ReturnType,
    string? ReturnLinkHRef,
    FlatParamInfo[]? Params,
    FlatParamInfo[]? ReturnParams);
