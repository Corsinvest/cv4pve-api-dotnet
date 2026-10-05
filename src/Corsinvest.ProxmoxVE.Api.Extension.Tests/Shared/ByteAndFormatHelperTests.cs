/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Globalization;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shared;

// the readable sizes follow the culture of the thread: the expected texts here are the invariant ones
public class ByteAndFormatHelperTests : IDisposable
{
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;

    public ByteAndFormatHelperTests() => CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    public void Dispose() => CultureInfo.CurrentCulture = _culture;

    [Theory]
    [InlineData("512 B", 512)]
    [InlineData("1 KB", 1_000)]
    [InlineData("1 KiB", 1_024)]
    [InlineData("1.5 GB", 1_500_000_000)]
    [InlineData("2 MiB", 2_097_152)]
    [InlineData("4GiB", 4_294_967_296)]
    [InlineData(" 1 tib ", 1_099_511_627_776)]
    [InlineData("0.5 kib", 512)]
    public void Size_with_a_unit_is_converted_to_bytes(string text, long bytes)
        => Assert.Equal(bytes, ByteHelper.ToBytes(text));

    [Fact]
    public void Size_is_read_with_a_point_whatever_the_culture()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("it-IT");

        Assert.Equal(1_500_000_000, ByteHelper.ToBytes("1.5 GB"));
    }

    [Theory]
    [InlineData("1,5 GB")]
    [InlineData("GB")]
    [InlineData("10")]
    [InlineData("10 XB")]
    [InlineData("-1 GB")]
    [InlineData("1 G")]
    public void Size_in_another_format_is_refused(string text)
        => Assert.Throws<FormatException>(() => ByteHelper.ToBytes(text));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Empty_size_is_refused(string? text)
        => Assert.Throws<ArgumentException>(() => ByteHelper.ToBytes(text!));

    [Theory]
    [InlineData("528K", 540_672)]
    [InlineData("4M", 4_194_304)]
    [InlineData("32G", 34_359_738_368)]
    [InlineData("1T", 1_099_511_627_776)]
    [InlineData("2P", 2_251_799_813_685_248)]
    [InlineData(" 32g ", 34_359_738_368)]
    public void Size_of_a_proxmox_ve_configuration_is_converted_to_bytes(string text, long bytes)
        => Assert.Equal(bytes, ByteHelper.ParsePveSize(text));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("G")]
    [InlineData("32")]
    [InlineData("32X")]
    [InlineData("1.5G")]
    [InlineData("abcG")]
    public void Size_of_a_configuration_that_is_not_recognized_is_zero(string? text)
        => Assert.Equal(0, ByteHelper.ParsePveSize(text!));

    [Theory]
    [InlineData(0, true, "0 B")]
    [InlineData(1023, true, "1023 B")]
    [InlineData(1024, true, "1 KiB")]
    [InlineData(1536, true, "1.5 KiB")]
    [InlineData(8_589_934_592, true, "8 GiB")]
    [InlineData(1_000, false, "1 KB")]
    [InlineData(1_500_000_000, false, "1.5 GB")]
    [InlineData(8_589_934_592, false, "8.59 GB")]
    public void Bytes_are_written_as_a_readable_size(double bytes, bool binary, string expected)
        => Assert.Equal(expected, bytes.ToSizeString(binary));

    [Fact]
    public void Size_beyond_the_largest_unit_stays_in_it()
        => Assert.Equal("2000 EB", 2e21.ToSizeString(false));

    [Fact]
    public void Negative_bytes_are_refused()
        => Assert.Throws<ArgumentOutOfRangeException>(() => (-1.0).ToSizeString(true));

    [Fact]
    public void Format_helper_writes_sizes_in_decimal_units()
    {
        Assert.Equal("1.5 GB", FormatHelper.FromBytes(1_500_000_000));
        Assert.Equal("1 KB", FormatHelper.FromBits(1_000));
    }

    [Theory]
    [InlineData(50, 200, 0.25)]
    [InlineData(0, 200, 0)]
    [InlineData(200, 200, 1)]
    [InlineData(5, 0, 5)]
    [InlineData(0, 0, 0)]
    public void Percentage_does_not_divide_by_zero(ulong usage, ulong size, double expected)
        => Assert.Equal(expected, FormatHelper.CalculatePercentage(usage, size));

    [Fact]
    public void Usage_is_written_with_percentage_and_sizes()
        => Assert.Equal("25% (1 GB of 4 GB)", FormatHelper.UsageInfo(1_000_000_000, 4_000_000_000));

    [Fact]
    public void Cpu_usage_is_written_with_the_number_of_cpus()
        => Assert.Equal("12.5 % of 8 CPU(s)", FormatHelper.CpuInfo(0.125, 8).Replace("12.5%", "12.5 %"));

    [Theory]
    [InlineData(0, "")]
    [InlineData(59, "0 days 00:00:59")]
    [InlineData(3_661, "0 days 01:01:01")]
    [InlineData(90_061, "1 days 01:01:01")]
    [InlineData(8_640_000, "100 days 00:00:00")]
    public void Uptime_is_written_as_days_and_time(double seconds, string expected)
        => Assert.Equal(expected, FormatHelper.UptimeInfo(seconds));

    [Theory]
    [InlineData("images", "Disk image")]
    [InlineData("backup", "VZDump backup file")]
    [InlineData("vztmpl", "Container template")]
    [InlineData("iso", "ISO image")]
    [InlineData("rootdir", "Container")]
    [InlineData("snippets", "Snippets")]
    [InlineData("import", "Import")]
    [InlineData("something-new", "")]
    public void Content_of_a_storage_has_a_description(string content, string expected)
        => Assert.Equal(expected, FormatHelper.ContentToDescription(content));
}
