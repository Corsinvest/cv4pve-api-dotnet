/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

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
        var dowSel = string.Join("|", dow);

        //format [day(s)] [[start-time(s)][/repetition-time(s)]]

        var dowHash = new List<string>();
        var parts = schedule.Trim().ToLower().Split(' ').ToList();

        if (parts[parts.Count - 1] == "utc") { parts.RemoveAt(parts.Count - 1); }

        if (Regex.Match(parts[0], dowSel).Success)
        {
            //parse dow
            foreach (var item in parts[0].Split(','))
            {
                if (Regex.Match(item, $"^{dowSel}").Success)
                {
                    dowHash.Add(item);
                }
                else
                {
                    var match1 = Regex.Match(item, $@"^{dowSel}\.\.{dowSel}");
                    if (!match1.Success) { throw new PveException($"Invalid format {item}"); }

                    for (int i = Array.IndexOf(dow, match1.Groups[0].Value); i < Array.IndexOf(dow, match1.Groups[1].Value); i++)
                    {
                        dowHash.Add(dow[i]);
                    }
                }
            }

            parts.RemoveAt(0);
        }
        else
        {
            dowHash.AddRange(dow);
        }

        if (Regex.Match(parts[0], @"\-").Success)
        {
            parts.RemoveAt(0);
            throw new PveException("Date specification not implemented");
        }

        var chars = "[0-9*/.,]";
        var matchAllHours = false;
        var matchAllMinutes = false;
        var hoursHash = new List<int>();
        var minutesHash = new List<int>();

        var match = Regex.Match(parts[0], $"^{chars}:{chars}");
        if (match.Success)
        {
            foreach (var item in match.Groups[0].Value.Split('.')) { ParseSingleTimeSpec(item, 24, ref matchAllHours, hoursHash); }
            foreach (var item in match.Groups[1].Value.Split('.')) { ParseSingleTimeSpec(item, 60, ref matchAllMinutes, minutesHash); }
        }
        else if (Regex.Match(parts[0], $"^{chars}+$").Success)
        {
            matchAllHours = true;
            foreach (var item in parts[0].Split('.')) { ParseSingleTimeSpec(item, 60, ref matchAllMinutes, minutesHash); }
        }
        else
        {
            throw new ArgumentException("Unable to parse calendar event");
        }

        return (string.Join(",", dowHash),
                string.Join(",", matchAllHours ? Enumerable.Range(0, 23) : hoursHash.OrderBy(a => a)),
                string.Join(",", matchAllMinutes ? Enumerable.Range(0, 59) : minutesHash.OrderBy(a => a)));
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
}
