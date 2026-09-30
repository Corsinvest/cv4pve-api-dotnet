/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Metadata;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Metadata;

public class RendererTests
{
    private static ParameterApi Param(string renderer)
        => GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache(
            """{"/version":{"methods":{"get":{"returnParams":[{"name":"value","type":"integer","renderer":""" + $"\"{renderer}\"" + "}]}}}}")!).SubClasses.Single().Methods.Single().ReturnParameters.Single();

    [Theory]
    [InlineData(0L, "0 B")]
    [InlineData(512L, "512 B")]
    [InlineData(1536L, "1.50 KiB")]
    [InlineData(270057619456L, "251.51 GiB")]
    public void Bytes(long value, string text) => Assert.Equal(text, Param("bytes").RendererValue(value));

    [Theory]
    [InlineData(0d, "0%")]
    [InlineData(0.0444d, "4.44%")]
    [InlineData(1d, "100%")]
    public void Percentage(double value, string text) => Assert.Equal(text, Param("fraction_as_percentage").RendererValue(value));

    [Theory]
    [InlineData(0L, "0s")]
    [InlineData(5L, "5s")]
    [InlineData(3605L, "1h 0m 5s")]
    [InlineData(7906391L, "91d 12h 13m 11s")]
    public void Duration(long value, string text) => Assert.Equal(text, Param("duration").RendererValue(value));

    [Fact]
    public void Dates()
    {
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1759233743).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                     Param("timestamp").RendererValue(1759233743L));
        Assert.Equal("2025-09-30 12:02:23", Param("timestamp_gmt").RendererValue(1759233743L));
        Assert.Equal(string.Empty, Param("timestamp").RendererValue(0L));
    }

    [Fact]
    public void Value_that_is_not_a_number_stays_as_it_is()
        => Assert.Equal("n/a", Param("bytes").RendererValue("n/a"));
}
