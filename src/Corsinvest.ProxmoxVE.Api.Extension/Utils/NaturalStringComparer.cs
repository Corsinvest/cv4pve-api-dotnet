/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Compares strings treating digit runs as numbers, so "net2" &lt; "net10" and "vmbr2" &lt; "vmbr10".
/// Non-digit parts compare case-insensitively; null sorts first.
/// </summary>
public sealed class NaturalStringComparer : IComparer<string?>
{
    /// <summary>Shared instance.</summary>
    public static NaturalStringComparer Instance { get; } = new();

    /// <inheritdoc/>
    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y)) { return 0; }
        if (x == null) { return -1; }
        if (y == null) { return 1; }

        int i = 0, j = 0;
        while (i < x.Length && j < y.Length)
        {
            if (char.IsDigit(x[i]) && char.IsDigit(y[j]))
            {
                var si = i;
                var sj = j;
                while (i < x.Length && char.IsDigit(x[i])) { i++; }
                while (j < y.Length && char.IsDigit(y[j])) { j++; }

                var a = x[si..i].TrimStart('0');
                var b = y[sj..j].TrimStart('0');
                if (a.Length != b.Length) { return a.Length.CompareTo(b.Length); }

                var cmp = string.CompareOrdinal(a, b);
                if (cmp != 0) { return cmp; }
            }
            else
            {
                var cmp = char.ToUpperInvariant(x[i]).CompareTo(char.ToUpperInvariant(y[j]));
                if (cmp != 0) { return cmp; }
                i++;
                j++;
            }
        }

        return (x.Length - i).CompareTo(y.Length - j);
    }
}
