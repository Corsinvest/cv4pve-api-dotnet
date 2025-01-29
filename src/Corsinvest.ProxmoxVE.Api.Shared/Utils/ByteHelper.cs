/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils;

/// <summary>
/// Byte helper
/// </summary>
public static class ByteHelper
{
    private static readonly string[] DecimalUnits = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];
    private static readonly string[] BinaryUnits = ["B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB"];

    /// <summary>
    /// Converts a string with a unit of measurement (MB, MiB, GB, GiB, etc.) into bytes.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static long ToBytes(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) { throw new ArgumentException("Invalid input."); }

        var pattern = @"^(\d+(\.\d+)?)\s*(" + string.Join("|", DecimalUnits) + string.Join("|", BinaryUnits) + ")$";
        //@"^(\d+(\.\d+)?)\s*(B|KB|MB|GB|TB|PB|EB|KIB|MIB|GIB|TIB|PIB|EIB)$"
        var match = Regex.Match(input.Trim(), pattern, RegexOptions.IgnoreCase);
        if (!match.Success) { throw new FormatException("Invalid format. Use '1.5 GB' or '2 MiB'."); }

        var value = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

        return match.Groups[3].Value.ToUpper() switch
        {
            "B" => (long)value,
            "KB" => (long)(value * 1_000),
            "MB" => (long)(value * 1_000_000),
            "GB" => (long)(value * 1_000_000_000),
            "TB" => (long)(value * 1_000_000_000_000),
            "PB" => (long)(value * 1_000_000_000_000_000),
            "EB" => (long)(value * 1_000_000_000_000_000_000),

            "KIB" => (long)(value * 1_024),
            "MIB" => (long)(value * 1_048_576),
            "GIB" => (long)(value * 1_073_741_824),
            "TIB" => (long)(value * 1_099_511_627_776),
            "PIB" => (long)(value * 1_125_899_906_842_624),
            "EIB" => (long)(value * 1_152_921_504_606_846_976),
            _ => throw new NotSupportedException("Unity not supported!")
        };
    }

    /// <summary>
    /// Converts a byte value into a human-readable string (e.g., "1.5 GiB").
    /// Uses binary units (MiB, GiB) for values above 1 KiB.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="useBinaryUnits"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string ToSizeString(this double bytes, bool useBinaryUnits)
    {
        if (bytes < 0) { throw new ArgumentOutOfRangeException(nameof(bytes), "The value must be positive."); }

        var units = useBinaryUnits ? BinaryUnits : DecimalUnits;
        var baseValue = useBinaryUnits ? 1024.0 : 1000.0;

        var unitIndex = 0;
        double size = bytes;

        while (size >= baseValue && unitIndex < units.Length - 1)
        {
            size /= baseValue;
            unitIndex++;
        }

        return $"{size:0.##} {units[unitIndex]}";
    }
}
