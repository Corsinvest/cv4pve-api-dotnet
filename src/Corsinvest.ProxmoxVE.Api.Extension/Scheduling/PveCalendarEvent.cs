/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Extension.Scheduling;

/// <summary>
/// Parsed PVE calendar event (systemd OnCalendar subset used by Proxmox VE
/// for backup / replication / job schedules).
///
/// Supports:
///   shortcuts: minutely, hourly, daily, weekly, monthly, quarterly, semiannually, yearly/annually
///   weekdays:  sun..sat, ranges (mon..fri), lists (mon,wed,fri)
///   time:      HH:MM, *:MM, HH:*, *:0/15, 0/4:00, lists, ranges
///   date:      *-*-*, MM-DD (wildcards), with optional weekday prefix
///   trailing 'utc' suffix (parsed and recorded; no timezone conversion)
///
/// Fixed dates (YYYY-MM-DD) are NOT supported (not expressible as a recurring schedule).
/// </summary>
public sealed partial class PveCalendarEvent
{
    /// <summary>Canonical weekday names in PVE order (sun=0 … sat=6).</summary>
    public static readonly string[] WeekdayNames = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"];

    /// <summary>Months that match (1..12).</summary>
    public IReadOnlySet<int> Months { get; }

    /// <summary>Days of month that match (1..31).</summary>
    public IReadOnlySet<int> DaysOfMonth { get; }

    /// <summary>Days of week that match (0=sun … 6=sat).</summary>
    public IReadOnlySet<int> DaysOfWeek { get; }

    /// <summary>Hours that match (0..23).</summary>
    public IReadOnlySet<int> Hours { get; }

    /// <summary>Minutes that match (0..59).</summary>
    public IReadOnlySet<int> Minutes { get; }

    /// <summary>True when the original input had a trailing 'utc' qualifier.</summary>
    public bool Utc { get; }

    /// <summary>Original input string (after trim, before normalization).</summary>
    public string Source { get; }

    private PveCalendarEvent(string source, bool utc,
                             IReadOnlySet<int> months,
                             IReadOnlySet<int> dom,
                             IReadOnlySet<int> dow,
                             IReadOnlySet<int> hours,
                             IReadOnlySet<int> minutes)
    {
        Source = source;
        Utc = utc;
        Months = months;
        DaysOfMonth = dom;
        DaysOfWeek = dow;
        Hours = hours;
        Minutes = minutes;
    }

    /// <summary>
    /// Parse a PVE calendar event. Throws <see cref="PveCalendarParseException"/> on failure.
    /// </summary>
    public static PveCalendarEvent Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new PveCalendarParseException(input ?? "", "empty input");
        }

        var original = input.Trim();
        var normalized = ExpandShortcut(original.ToLowerInvariant());

        var tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        if (tokens.Count == 0)
        {
            throw new PveCalendarParseException(original, "no tokens after normalization");
        }

        var utc = false;
        if (tokens[^1] == "utc")
        {
            utc = true;
            tokens.RemoveAt(tokens.Count - 1);
            if (tokens.Count == 0)
            {
                throw new PveCalendarParseException(original, "'utc' without preceding spec");
            }
        }

        var dowSet = AllDaysOfWeek();
        if (IsWeekdayToken(tokens[0]))
        {
            dowSet = ParseWeekdayList(tokens[0], original);
            tokens.RemoveAt(0);
            if (tokens.Count == 0)
            {
                throw new PveCalendarParseException(original, "missing time after weekday spec");
            }
        }

        var months = AllMonths();
        var dom = AllDaysOfMonth();
        if (IsDateSpecToken(tokens[0]))
        {
            (months, dom) = ParseDateSpec(tokens[0], original);
            tokens.RemoveAt(0);
            if (tokens.Count == 0)
            {
                throw new PveCalendarParseException(original, "missing time after date spec");
            }
        }

        var (hours, minutes) = ParseTime(tokens[0], original);

        if (tokens.Count > 1)
        {
            throw new PveCalendarParseException(original, $"unexpected trailing tokens: {string.Join(' ', tokens.Skip(1))}");
        }

        return new PveCalendarEvent(original, utc, months, dom, dowSet, hours, minutes);
    }

    /// <summary>
    /// Try to parse a PVE calendar event. Returns true on success.
    /// </summary>
    public static bool TryParse(string input, [NotNullWhen(true)] out PveCalendarEvent? result)
    {
        try
        {
            result = Parse(input);
            return true;
        }
        catch (PveCalendarParseException)
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Returns true if the given moment matches this calendar event.
    /// Seconds are ignored.
    /// </summary>
    public bool Matches(DateTime moment)
        => Months.Contains(moment.Month)
        && DaysOfMonth.Contains(moment.Day)
        && DaysOfWeek.Contains((int)moment.DayOfWeek)
        && Hours.Contains(moment.Hour)
        && Minutes.Contains(moment.Minute);

    /// <summary>
    /// Returns the next occurrence at or after <paramref name="from"/> (truncated to minute),
    /// or null if no occurrence exists within one year (defensive bound for malformed schedules).
    /// </summary>
    public DateTime? NextOccurrence(DateTime from)
    {
        var candidate = new DateTime(from.Year, from.Month, from.Day, from.Hour, from.Minute, 0, from.Kind);
        var limit = candidate.AddYears(1);
        while (candidate <= limit)
        {
            if (Matches(candidate)) { return candidate; }
            candidate = candidate.AddMinutes(1);
        }
        return null;
    }

    // shortcut expansion

    private static string ExpandShortcut(string s) => s switch
    {
        "minutely" => "*:*",
        "hourly" => "*:00",
        "daily" => "*-*-* 00:00",
        "weekly" => "mon *-*-* 00:00",
        "monthly" => "*-*-01 00:00",
        "quarterly" => "*-01,04,07,10-01 00:00",
        "semiannually" or "semi-annually" => "*-01,07-01 00:00",
        "annually" or "yearly" => "*-01-01 00:00",
        _ => s,
    };

    // weekday

    private static bool IsWeekdayToken(string token)
    {
        var first = token.Split([',', '.'], 2)[0];
        return Array.IndexOf(WeekdayNames, first) >= 0;
    }

    private static IReadOnlySet<int> ParseWeekdayList(string token, string original)
    {
        var result = new SortedSet<int>();
        foreach (var item in token.Split(','))
        {
            var rangeMatch = WeekdayRangeRegex().Match(item);
            if (rangeMatch.Success)
            {
                var start = Array.IndexOf(WeekdayNames, rangeMatch.Groups[1].Value);
                var end = Array.IndexOf(WeekdayNames, rangeMatch.Groups[2].Value);
                if (start < 0 || end < 0 || start > end)
                {
                    throw new PveCalendarParseException(original, $"invalid weekday range '{item}'");
                }
                for (var i = start; i <= end; i++) { result.Add(i); }
            }
            else
            {
                var idx = Array.IndexOf(WeekdayNames, item);
                if (idx < 0)
                {
                    throw new PveCalendarParseException(original, $"invalid weekday '{item}'");
                }
                result.Add(idx);
            }
        }
        return result;
    }

    // date spec

    private static bool IsDateSpecToken(string token)
        => token.Contains('-') && !token.Contains(':');

    private static (IReadOnlySet<int> months, IReadOnlySet<int> dom) ParseDateSpec(string token, string original)
    {
        var parts = token.Split('-');
        if (parts.Length is not 2 and not 3)
        {
            throw new PveCalendarParseException(original, $"invalid date spec '{token}'");
        }

        // 3-part: Y-M-D — only wildcard year is supported
        if (parts.Length == 3)
        {
            if (parts[0] != "*")
            {
                throw new PveCalendarParseException(original, "fixed-year date specs are not supported (use *-MM-DD)");
            }
            return (ParseIntList(parts[1], 1, 12, original, "month"),
                    ParseIntList(parts[2], 1, 31, original, "day-of-month"));
        }

        // 2-part: M-D
        return (ParseIntList(parts[0], 1, 12, original, "month"),
                ParseIntList(parts[1], 1, 31, original, "day-of-month"));
    }

    private static IReadOnlySet<int> ParseIntList(string token, int min, int max, string original, string fieldName)
    {
        if (token == "*") { return new HashSet<int>(Enumerable.Range(min, max - min + 1)); }

        var result = new SortedSet<int>();
        foreach (var item in token.Split(','))
        {
            ParseIntListItem(item, min, max, original, fieldName, result);
        }
        return result;
    }

    private static void ParseIntListItem(string item, int min, int max, string original, string fieldName, SortedSet<int> dst)
    {
        var step = StepRegex().Match(item);
        if (step.Success)
        {
            var startStr = step.Groups[1].Value;
            var stepVal = int.Parse(step.Groups[2].Value);
            var start = startStr == "*" ? min : int.Parse(startStr);
            if (start < min || start > max) { throw new PveCalendarParseException(original, $"{fieldName} '{start}' out of range [{min}..{max}]"); }
            if (stepVal < 1) { throw new PveCalendarParseException(original, $"{fieldName} step must be >= 1"); }

            for (var v = start; v <= max; v += stepVal) { dst.Add(v); }
            return;
        }

        var range = RangeRegex().Match(item);
        if (range.Success)
        {
            var lo = int.Parse(range.Groups[1].Value);
            var hi = int.Parse(range.Groups[2].Value);
            if (lo < min || hi > max || lo > hi)
            {
                throw new PveCalendarParseException(original, $"{fieldName} range '{item}' invalid for [{min}..{max}]");
            }
            for (var v = lo; v <= hi; v++) { dst.Add(v); }
            return;
        }

        if (item == "*")
        {
            for (var v = min; v <= max; v++) { dst.Add(v); }
            return;
        }

        if (!int.TryParse(item, out var single)) { throw new PveCalendarParseException(original, $"invalid {fieldName} token '{item}'"); }
        if (single < min || single > max) { throw new PveCalendarParseException(original, $"{fieldName} '{single}' out of range [{min}..{max}]"); }
        dst.Add(single);
    }

    // time HH:MM

    private static (IReadOnlySet<int> hours, IReadOnlySet<int> minutes) ParseTime(string token, string original)
    {
        var colonIdx = token.IndexOf(':');
        if (colonIdx < 0)
        {
            throw new PveCalendarParseException(original, $"time spec '{token}' must contain ':'");
        }

        var hourTok = token[..colonIdx];
        var minTok = token[(colonIdx + 1)..];

        var hours = ParseIntList(hourTok, 0, 23, original, "hour");
        var minutes = ParseIntList(minTok, 0, 59, original, "minute");
        return (hours, minutes);
    }

    // all-of helpers

    private static IReadOnlySet<int> AllMonths() => new HashSet<int>(Enumerable.Range(1, 12));
    private static IReadOnlySet<int> AllDaysOfMonth() => new HashSet<int>(Enumerable.Range(1, 31));
    private static IReadOnlySet<int> AllDaysOfWeek() => new HashSet<int>(Enumerable.Range(0, 7));

    [GeneratedRegex(@"^([a-z]{3})\.\.([a-z]{3})$")]
    private static partial Regex WeekdayRangeRegex();

    [GeneratedRegex(@"^(\*|[0-9]+)\/([1-9][0-9]*)$")]
    private static partial Regex StepRegex();

    [GeneratedRegex(@"^([0-9]+)\.\.([0-9]+)$")]
    private static partial Regex RangeRegex();
}
