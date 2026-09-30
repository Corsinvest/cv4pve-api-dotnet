/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shell;

public class NotJsonAnswerTests
{
    [Fact]
    public async Task Success_status_with_a_body_that_is_not_json_is_an_error()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Text(HttpStatusCode.OK, "no such file '/version'")));
        var result = await client.GetAsync("/version");

        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        Assert.Equal("The answer is not JSON (HTTP 200): no such file '/version'", result.ReasonPhrase);
    }

    [Fact]
    public async Task Error_status_is_kept()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Text(HttpStatusCode.NotImplemented, "Method not implemented")));
        var result = await client.GetAsync("/nodex");

        Assert.Equal(HttpStatusCode.NotImplemented, result.StatusCode);
        Assert.StartsWith("The answer is not JSON (HTTP 501):", result.ReasonPhrase);
    }

    [Fact]
    public void Long_body_is_cut_and_put_on_one_line()
    {
        var response = PveClientBase.NotJsonResponse(HttpStatusCode.NotFound, "line1\r\nline2 " + new string('x', 200));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.DoesNotContain("\n", response.ReasonPhrase);
        Assert.EndsWith("…", response.ReasonPhrase);
    }
}
