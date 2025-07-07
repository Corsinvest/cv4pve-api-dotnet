/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils;

/// <summary>
/// Enumerable Extensions
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Sum for ulong
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="summer"></param>
    /// <returns></returns>
    public static ulong Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> summer)
    {
        ulong total = 0;
        foreach (var item in source) { total += summer(item); }
        return total;
    }

    /// <summary>
    /// Average for ulong
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="summer"></param>
    /// <returns></returns>
    public static ulong Average<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> summer)
        => source.Sum(summer) / Convert.ToUInt64(source.Count());
}