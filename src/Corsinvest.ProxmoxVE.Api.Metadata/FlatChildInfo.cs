/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>Flat cache child node</summary>
public record FlatChildInfo(string Name,
                            bool? Indexed,      // null = false (omitted)
                            bool? HasChildren); // null = false (omitted)
