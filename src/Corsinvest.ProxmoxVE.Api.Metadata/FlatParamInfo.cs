/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>Flat cache parameter info</summary>
public record FlatParamInfo(
    string Name,
    string? Type,
    string? TypeText,
    string? Description,
    bool? Optional,
    string? Default,
    int? Minimum,
    long? Maximum,
    string[]? EnumValues);
