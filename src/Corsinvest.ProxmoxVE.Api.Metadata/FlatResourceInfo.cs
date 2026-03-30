/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>Flat cache resource — keys, children, methods</summary>
public record FlatResourceInfo(string[]? Keys,
                               FlatChildInfo[]? Children,
                               Dictionary<string,
                               FlatMethodInfo>? Methods);
