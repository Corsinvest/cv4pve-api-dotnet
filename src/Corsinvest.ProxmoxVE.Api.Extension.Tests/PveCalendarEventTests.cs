/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Extension.Scheduling;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

public class PveCalendarEventTests
{
    // shortcuts

    [Fact]
    public void Minutely_matches_every_minute()
    {
        var c = PveCalendarEvent.Parse("minutely");
        Assert.Equal(24, c.Hours.Count);
        Assert.Equal(60, c.Minutes.Count);
    }

    [Fact]
    public void Hourly_matches_top_of_every_hour()
    {
        var c = PveCalendarEvent.Parse("hourly");
        Assert.Equal(24, c.Hours.Count);
        Assert.Equal([0], c.Minutes);
    }

    [Fact]
    public void Daily_is_midnight_every_day()
    {
        var c = PveCalendarEvent.Parse("daily");
        Assert.Equal([0], c.Hours);
        Assert.Equal([0], c.Minutes);
        Assert.Equal(12, c.Months.Count);
        Assert.Equal(31, c.DaysOfMonth.Count);
        Assert.Equal(7, c.DaysOfWeek.Count);
    }

    [Fact]
    public void Weekly_is_monday_midnight()
    {
        var c = PveCalendarEvent.Parse("weekly");
        Assert.Equal([1], c.DaysOfWeek); // monday
        Assert.Equal([0], c.Hours);
        Assert.Equal([0], c.Minutes);
    }

    [Fact]
    public void Monthly_is_first_of_month_midnight()
    {
        var c = PveCalendarEvent.Parse("monthly");
        Assert.Equal([1], c.DaysOfMonth);
        Assert.Equal(12, c.Months.Count);
        Assert.Equal([0], c.Hours);
        Assert.Equal([0], c.Minutes);
    }

    [Fact]
    public void Yearly_is_january_first_midnight()
    {
        var c = PveCalendarEvent.Parse("yearly");
        Assert.Equal([1], c.Months);
        Assert.Equal([1], c.DaysOfMonth);
        Assert.Equal([0], c.Hours);
        Assert.Equal([0], c.Minutes);
    }

    [Fact]
    public void Annually_is_alias_for_yearly()
    {
        var a = PveCalendarEvent.Parse("annually");
        var y = PveCalendarEvent.Parse("yearly");
        Assert.Equal(a.Months, y.Months);
        Assert.Equal(a.DaysOfMonth, y.DaysOfMonth);
    }

    [Fact]
    public void Quarterly_is_jan_apr_jul_oct_first()
    {
        var c = PveCalendarEvent.Parse("quarterly");
        Assert.Equal([1, 4, 7, 10], c.Months);
        Assert.Equal([1], c.DaysOfMonth);
    }

    [Fact]
    public void Semiannually_is_jan_jul_first()
    {
        var c = PveCalendarEvent.Parse("semiannually");
        Assert.Equal([1, 7], c.Months);
        Assert.Equal([1], c.DaysOfMonth);
    }

    // weekdays

    [Theory]
    [InlineData("sun", 0)]
    [InlineData("mon", 1)]
    [InlineData("tue", 2)]
    [InlineData("wed", 3)]
    [InlineData("thu", 4)]
    [InlineData("fri", 5)]
    [InlineData("sat", 6)]
    public void Single_weekday_token(string token, int expected)
    {
        var c = PveCalendarEvent.Parse($"{token} 00:00");
        Assert.Equal([expected], c.DaysOfWeek);
    }

    [Fact]
    public void Weekday_range_mon_to_fri()
    {
        var c = PveCalendarEvent.Parse("mon..fri 09:00");
        Assert.Equal([1, 2, 3, 4, 5], c.DaysOfWeek);
        Assert.Equal([9], c.Hours);
        Assert.Equal([0], c.Minutes);
    }

    [Fact]
    public void Weekday_list_mon_wed_fri()
    {
        var c = PveCalendarEvent.Parse("mon,wed,fri 14:30");
        Assert.Equal([1, 3, 5], c.DaysOfWeek);
        Assert.Equal([14], c.Hours);
        Assert.Equal([30], c.Minutes);
    }

    [Fact]
    public void Weekday_no_prefix_means_all_days()
    {
        var c = PveCalendarEvent.Parse("02:00");
        Assert.Equal(7, c.DaysOfWeek.Count);
    }

    // time HH:MM

    [Fact]
    public void Simple_hour_minute()
    {
        var c = PveCalendarEvent.Parse("09:00");
        Assert.Equal([9], c.Hours);
        Assert.Equal([0], c.Minutes);
    }

    [Fact]
    public void Wildcard_hour_specific_minute()
    {
        var c = PveCalendarEvent.Parse("*:30");
        Assert.Equal(24, c.Hours.Count);
        Assert.Equal([30], c.Minutes);
    }

    [Fact]
    public void Step_every_15_minutes()
    {
        var c = PveCalendarEvent.Parse("*:0/15");
        Assert.Equal(24, c.Hours.Count);
        Assert.Equal([0, 15, 30, 45], c.Minutes);
    }

    [Fact]
    public void Step_every_4_hours()
    {
        var c = PveCalendarEvent.Parse("0/4:00");
        Assert.Equal([0, 4, 8, 12, 16, 20], c.Hours);
        Assert.Equal([0], c.Minutes);
    }

    [Fact]
    public void Hour_list()
    {
        var c = PveCalendarEvent.Parse("9,12,15:00");
        Assert.Equal([9, 12, 15], c.Hours);
    }

    [Fact]
    public void Hour_range()
    {
        var c = PveCalendarEvent.Parse("9..17:00");
        Assert.Equal(Enumerable.Range(9, 9), c.Hours.OrderBy(x => x));
    }

    // date spec

    [Fact]
    public void Systemd_full_wildcard_is_daily()
    {
        var c = PveCalendarEvent.Parse("*-*-* 02:00");
        Assert.Equal(12, c.Months.Count);
        Assert.Equal(31, c.DaysOfMonth.Count);
        Assert.Equal([2], c.Hours);
        Assert.Equal([0], c.Minutes);
    }

    [Fact]
    public void Date_spec_month_day()
    {
        var c = PveCalendarEvent.Parse("12-25 00:00");
        Assert.Equal([12], c.Months);
        Assert.Equal([25], c.DaysOfMonth);
    }

    [Fact]
    public void Date_spec_wildcard_month_specific_day()
    {
        var c = PveCalendarEvent.Parse("*-15 12:00");
        Assert.Equal(12, c.Months.Count);
        Assert.Equal([15], c.DaysOfMonth);
    }

    // utc

    [Fact]
    public void Trailing_utc_is_recorded()
    {
        var c = PveCalendarEvent.Parse("09:00 utc");
        Assert.True(c.Utc);
        Assert.Equal([9], c.Hours);
    }

    [Fact]
    public void Without_utc_flag_is_false()
    {
        var c = PveCalendarEvent.Parse("09:00");
        Assert.False(c.Utc);
    }

    // matches / next

    [Fact]
    public void Matches_works_for_quarterly()
    {
        var c = PveCalendarEvent.Parse("quarterly");
        Assert.True(c.Matches(new DateTime(2026, 1, 1, 0, 0, 0)));
        Assert.True(c.Matches(new DateTime(2026, 4, 1, 0, 0, 0)));
        Assert.True(c.Matches(new DateTime(2026, 7, 1, 0, 0, 0)));
        Assert.True(c.Matches(new DateTime(2026, 10, 1, 0, 0, 0)));
        Assert.False(c.Matches(new DateTime(2026, 2, 1, 0, 0, 0)));
        Assert.False(c.Matches(new DateTime(2026, 1, 2, 0, 0, 0)));
        Assert.False(c.Matches(new DateTime(2026, 1, 1, 1, 0, 0)));
    }

    [Fact]
    public void Matches_respects_weekday()
    {
        var c = PveCalendarEvent.Parse("mon..fri 09:00");
        // 2026-06-01 is monday
        Assert.True(c.Matches(new DateTime(2026, 6, 1, 9, 0, 0)));
        // 2026-06-06 is saturday
        Assert.False(c.Matches(new DateTime(2026, 6, 6, 9, 0, 0)));
    }

    [Fact]
    public void NextOccurrence_for_weekly()
    {
        var c = PveCalendarEvent.Parse("weekly");
        // start sun 2026-06-07 14:00; next monday midnight is 2026-06-08 00:00
        var next = c.NextOccurrence(new DateTime(2026, 6, 7, 14, 0, 0));
        Assert.Equal(new DateTime(2026, 6, 8, 0, 0, 0), next);
    }

    [Fact]
    public void NextOccurrence_for_step()
    {
        var c = PveCalendarEvent.Parse("*:0/15");
        var next = c.NextOccurrence(new DateTime(2026, 6, 1, 10, 7, 0));
        Assert.Equal(new DateTime(2026, 6, 1, 10, 15, 0), next);
    }

    // tryparse

    [Fact]
    public void TryParse_returns_true_on_valid()
    {
        Assert.True(PveCalendarEvent.TryParse("daily", out var c));
        Assert.NotNull(c);
    }

    [Fact]
    public void TryParse_returns_false_on_invalid()
    {
        Assert.False(PveCalendarEvent.TryParse("garbage", out var c));
        Assert.Null(c);
    }

    // error cases

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("garbage")]
    [InlineData("25:00")]              // hour out of range
    [InlineData("12:99")]              // minute out of range
    [InlineData("mon..sun 09:00")]    // reversed range
    [InlineData("13-01 00:00")]       // invalid month
    [InlineData("01-32 00:00")]       // invalid day
    [InlineData("2024-01-15 09:00")]  // fixed-year date is not supported
    [InlineData("09:00 foo")]          // trailing garbage
    [InlineData("utc")]                // utc alone
    public void Parse_throws_on_invalid_input(string input)
    {
        Assert.Throws<PveCalendarParseException>(() => PveCalendarEvent.Parse(input));
    }

    [Fact]
    public void Exception_carries_input_and_reason()
    {
        var ex = Assert.Throws<PveCalendarParseException>(() => PveCalendarEvent.Parse("25:00"));
        Assert.Equal("25:00", ex.Input);
        Assert.Contains("hour", ex.Reason);
    }

    // real-world PVE schedules

    [Theory]
    [InlineData("mon 02:00")]            // weekly backup monday 2am
    [InlineData("sat 23:00")]            // weekly backup saturday late
    [InlineData("*:0/15")]               // replication every 15 minutes
    [InlineData("0/4:00")]               // every 4 hours
    [InlineData("*-*-* 02:00")]          // daily 2am explicit
    [InlineData("mon,wed,fri 06:30")]    // selected days
    [InlineData("mon..fri 22:00")]       // weekdays evening
    [InlineData("01-01 00:00")]          // jan 1st
    [InlineData("daily")]
    [InlineData("hourly")]
    [InlineData("weekly")]
    [InlineData("monthly")]
    [InlineData("yearly")]
    [InlineData("quarterly")]
    public void Common_pve_schedules_parse_without_error(string schedule)
    {
        var c = PveCalendarEvent.Parse(schedule);
        Assert.NotNull(c);
        Assert.NotEmpty(c.Months);
        Assert.NotEmpty(c.DaysOfMonth);
        Assert.NotEmpty(c.DaysOfWeek);
        Assert.NotEmpty(c.Hours);
        Assert.NotEmpty(c.Minutes);
    }
}
