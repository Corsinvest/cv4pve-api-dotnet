/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

public static partial class NetworkDiagramBuilder
{
    private static string JoinAsString<T>(this IEnumerable<T> source, string separator)
        => string.Join(separator, source);

    private static string[] SplitWords(this string source)
        => string.IsNullOrEmpty(source)
            ? []
            : source.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    private static string Escape(string s)
        => s.Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");

    private static string TooltipLines(params (string Key, string? Value)[] fields)
        => fields.Where(f => !string.IsNullOrWhiteSpace(f.Value))
                 .Select(f => $"{f.Key}: {f.Value}")
                 .JoinAsString("\n");

    private static string InterfaceTitle(string name, string? comment)
        => string.IsNullOrWhiteSpace(comment) ? name : $"{name} · {comment.Trim()}";

    private static string ActiveStatus(bool active) => active ? "Active" : "Inactive";
    private static string? MtuLabel(int? mtu) => mtu.HasValue ? $"MTU {mtu}" : null;
    private static string YesNo(bool value) => value ? "Yes" : "No";

    private static string BoxLabel(params string?[] lines)
        => lines.Where(l => !string.IsNullOrWhiteSpace(l))
                .SelectMany(l => l!.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .JoinAsString("\n");

    private static string? LabeledValue(string key, string? value)
        => string.IsNullOrWhiteSpace(value) ? null : $"{key}: {value}";

    private static string BuildSdnComment(SdnVnetRow v)
    {
        var parts = new List<string> { $"SDN vnet · zone {v.Zone} ({v.ZoneType})" };
        if (v.Tag is int tag) { parts.Add($"VLAN {tag}"); }
        if (!string.IsNullOrWhiteSpace(v.ZoneBridge)) { parts.Add($"via {v.ZoneBridge}"); }
        if (!string.IsNullOrWhiteSpace(v.Alias)) { parts.Add(v.Alias); }
        return string.Join(" · ", parts);
    }
}
