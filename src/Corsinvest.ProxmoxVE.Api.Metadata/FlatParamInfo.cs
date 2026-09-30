/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>Flat cache parameter info: every field of <see cref="ParameterApi"/>.</summary>
public record FlatParamInfo(string Name,
                            string? Type,
                            string? TypeText,
                            string? Description,
                            bool? Optional,
                            string? Default,
                            int? Minimum,
                            long? Maximum,
                            string[]? EnumValues,
                            string? Renderer = null,
                            string? VerboseDescription = null,
                            FlatFormatInfo[]? Formats = null,
                            FlatParamInfo[]? Items = null);

/// <summary>Flat cache format property info: every field of <see cref="ParameterFormatApi"/>.</summary>
public record FlatFormatInfo(string Name,
                             string? Type,
                             string? Description,
                             bool? Optional,
                             int? Minimum,
                             int? Maximum,
                             string? DefaultKey,
                             string? FormatDescription,
                             string? Format,
                             string? Alias,
                             int? MaxLength,
                             string[]? EnumValues);
