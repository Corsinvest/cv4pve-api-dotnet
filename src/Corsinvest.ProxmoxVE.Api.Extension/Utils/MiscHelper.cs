/*
using System.Runtime.InteropServices;
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Corsinvest.ProxmoxVE.Api.Shared;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// BackupHelper
/// </summary>
public static class MiscHelper
{
    //public static DateTime ParseDateBackup(string value) => DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);

    /// <summary>
    /// Day of week
    /// </summary>
    public static string[] DayOfWeek { get; } = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"];

    /// <summary>
    /// Parse Calendar Event
    /// </summary>
    /// <param name="schedule"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static (string dow, string hours, string minutes) ParseCalendarEvent(string schedule)
    {
        var dow = DayOfWeek;

        // Handle shorthands
        schedule = schedule.Trim().ToLower() switch
        {
            "hourly" => "*:00",
            "daily" => "00:00",
            "weekly" => "mon 00:00",
            "monthly" => "01 00:00",
            "quarterly" => "01-01,04-01,07-01,10-01 00:00",
            "semiannually" => "01-01,07-01 00:00",
            "annually" or "yearly" => "01-01 00:00",
            var s => s
        };

        var parts = schedule.Split([' '], StringSplitOptions.RemoveEmptyEntries).ToList();
        if (parts.Count == 0) { throw new ArgumentException("Empty calendar event"); }

        // Strip trailing UTC
        if (parts[^1] == "utc") { parts.RemoveAt(parts.Count - 1); }

        // --- DOW ---
        var dowSel = string.Join("|", dow);
        var dowHash = new List<string>();

        if (parts.Count > 0 && Regex.IsMatch(parts[0], $"^({dowSel})[,.]|^({dowSel})$"))
        {
            foreach (var item in parts[0].Split(','))
            {
                var rangeMatch = Regex.Match(item, $@"^({dowSel})\.\.({dowSel})$");
                if (rangeMatch.Success)
                {
                    var start = Array.IndexOf(dow, rangeMatch.Groups[1].Value);
                    var end = Array.IndexOf(dow, rangeMatch.Groups[2].Value);
                    if (start < 0 || end < 0) { throw new PveException($"Invalid day range: {item}"); }
                    for (var i = start; i <= end; i++) { dowHash.Add(dow[i]); }
                }
                else if (Regex.IsMatch(item, $"^({dowSel})$"))
                {
                    dowHash.Add(item);
                }
                else
                {
                    throw new PveException($"Invalid day of week: {item}");
                }
            }
            parts.RemoveAt(0);
        }
        else
        {
            dowHash.AddRange(dow);
        }

        if (parts.Count == 0) { throw new ArgumentException("Missing time specification"); }

        // Date spec (e.g. 2024-01-01) — not implemented
        if (Regex.IsMatch(parts[0], @"^\d{4}-\d{2}-\d{2}$"))
        {
            throw new PveException("Date specification not implemented");
        }

        var matchAllHours = false;
        var matchAllMinutes = false;
        var hoursHash = new List<int>();
        var minutesHash = new List<int>();

        var colonIdx = parts[0].IndexOf(':');
        if (colonIdx >= 0)
        {
            // HH:MM format — both sides may be lists
            var hourPart = parts[0][..colonIdx];
            var minutePart = parts[0][(colonIdx + 1)..];
            foreach (var item in hourPart.Split(',')) { ParseSingleTimeSpec(item, 24, ref matchAllHours, hoursHash); }
            foreach (var item in minutePart.Split(',')) { ParseSingleTimeSpec(item, 60, ref matchAllMinutes, minutesHash); }
        }
        else
        {
            // Only minutes specified (e.g. "*:30" already handled above; bare number = minutes)
            matchAllHours = true;
            foreach (var item in parts[0].Split(',')) { ParseSingleTimeSpec(item, 60, ref matchAllMinutes, minutesHash); }
        }

        return (string.Join(",", dowHash),
                string.Join(",", matchAllHours ? Enumerable.Range(0, 24) : hoursHash.OrderBy(a => a)),
                string.Join(",", matchAllMinutes ? Enumerable.Range(0, 60) : minutesHash.OrderBy(a => a)));
    }

    private static void ParseSingleTimeSpec(string item, int max, ref bool matchAll, List<int> hash)
    {
        var match = Regex.Match(item, @"^((?:\*|[0-9]+))(?:\/([1-9][0-9]*))?$");
        if (match.Success)
        {
            if (match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value))
            {
                var repetition = int.Parse(match.Groups[2].Value);
                var start = match.Groups[1].Value == "*" ? 0 : int.Parse(match.Groups[1].Value);
                if (start >= max) { throw new ArgumentOutOfRangeException($"Value '{start}' out of range"); }
                if (repetition >= max) { throw new ArgumentOutOfRangeException($"Repetition  '{repetition}' out of range"); }

                while (start < max)
                {
                    hash.Add(start);
                    start += repetition;
                }
            }
            else if (match.Groups[1].Value == "*")
            {
                matchAll = true;
            }
            else
            {
                var start = int.Parse(match.Groups[1].Value);
                if (start >= max) { throw new ArgumentOutOfRangeException($"Value '{start}' out of range"); }
                hash.Add(start);
            }
        }
        else
        {
            match = Regex.Match(item, @"^([0-9]+)\.\.([1-9][0-9]*)$");
            if (!match.Success) { throw new ArgumentException($"Unable to parse calendar event '{item}"); }

            var start = int.Parse(match.Groups[1].Value);
            if (start >= max) { throw new ArgumentOutOfRangeException($"Range start '{start}' out of range"); }
            var end = int.Parse(match.Groups[2].Value);
            if (end >= max) { throw new ArgumentOutOfRangeException($"Range end '{end}' out of range"); }

            for (int i = start; i <= end; i++) { hash.Add(i); }
        }
    }

    /// <summary>
    /// Opens a URL in the default system browser (cross-platform).
    /// </summary>
    public static void OpenBrowser(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            System.Diagnostics.Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            System.Diagnostics.Process.Start("open", url);
        }
    }


}
