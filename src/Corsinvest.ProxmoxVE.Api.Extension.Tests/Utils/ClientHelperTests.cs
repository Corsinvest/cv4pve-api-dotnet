/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using System.Net.Sockets;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Utils;

public class ClientHelperTests
{
    [Theory]
    [InlineData("pve1", "pve1", 8006)]
    [InlineData("pve1:8007", "pve1", 8007)]
    [InlineData("pve1.example.com:443", "pve1.example.com", 443)]
    [InlineData("10.1.1.90", "10.1.1.90", 8006)]
    [InlineData("10.1.1.90:8007", "10.1.1.90", 8007)]
    [InlineData("[fe80::1]", "fe80::1", 8006)]
    [InlineData("[fe80::1]:8007", "fe80::1", 8007)]
    [InlineData("fe80::1", "fe80::1", 8006)]
    [InlineData("::1", "::1", 8006)]
    public void Host_and_port_are_read(string entry, string host, int port)
        => Assert.Equal((host, port), ClientHelper.ParseHostAndPort(entry));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("pve1,pve2")]
    [InlineData("[fe80::1")]
    [InlineData("[]")]
    [InlineData("[]:8006")]
    [InlineData("[fe80::1]8006")]
    [InlineData(":8006")]
    [InlineData("pve1:")]
    [InlineData("pve1:abc")]
    [InlineData("pve1:0")]
    [InlineData("pve1:-1")]
    [InlineData("pve1:65536")]
    [InlineData("[fe80::1]:")]
    [InlineData("[fe80::1]:abc")]
    public void Entry_that_is_not_a_host_is_refused(string? entry)
        => Assert.Throws<ArgumentException>(() => ClientHelper.ParseHostAndPort(entry!));

    [Fact]
    public void List_of_hosts_is_split_at_commas_ignoring_spaces_and_empty_entries()
    {
        var endpoints = ClientHelper.ParseHostEndpoints(" pve1 , 10.1.1.90:8007,, [fe80::1]:8008 ,");

        Assert.Equal(["pve1:8006", "10.1.1.90:8007", "fe80::1:8008"], endpoints.Select(a => a.ToString()));
        Assert.Equal("fe80::1", endpoints[2].Host);
        Assert.Equal(8008, endpoints[2].Port);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Empty_list_of_hosts_has_no_endpoints(string? hosts)
        => Assert.Empty(ClientHelper.ParseHostEndpoints(hosts!));

    [Fact]
    public void List_with_an_entry_that_is_not_a_host_is_refused()
        => Assert.Throws<ArgumentException>(() => ClientHelper.ParseHostEndpoints("pve1,pve2:abc"));

    private static int ClosedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    [Fact]
    public async Task First_host_that_answers_is_used()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var open = ((IPEndPoint)listener.LocalEndpoint).Port;

            var (client, endpoint) = await ClientHelper.GetClientFromHAAsync($"127.0.0.1:{ClosedPort()},127.0.0.1:{open},127.0.0.1:{ClosedPort()}", 2000);

            Assert.Equal(open, endpoint.Port);
            Assert.Equal("127.0.0.1", client.Host);
            Assert.Equal(open, client.Port);
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public async Task Reachable_host_is_reachable_and_closed_port_is_not()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var open = ((IPEndPoint)listener.LocalEndpoint).Port;

            Assert.True(await ClientHelper.IsHostReachableAsync(new ClientHelper.HostEndpoint("127.0.0.1", open), 2000));
            Assert.False(await ClientHelper.IsHostReachableAsync(new ClientHelper.HostEndpoint("127.0.0.1", ClosedPort()), 2000));
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public async Task No_host_that_answers_says_which_hosts_were_tried()
    {
        var first = ClosedPort();
        var second = ClosedPort();

        var ex = await Assert.ThrowsAsync<PveException>(() => ClientHelper.GetClientFromHAAsync($"127.0.0.1:{first},127.0.0.1:{second}", 2000));

        Assert.Contains($"127.0.0.1:{first}", ex.Message);
        Assert.Contains($"127.0.0.1:{second}", ex.Message);
    }

    [Fact]
    public async Task Cancellation_of_the_caller_stops_the_search()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => ClientHelper.IsHostReachableAsync(new ClientHelper.HostEndpoint("192.0.2.1", 8006), 5000, cancellation.Token));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Login_without_hosts_is_refused(string? hosts)
        => await Assert.ThrowsAsync<ArgumentException>(() => ClientHelper.GetClientAndTryLoginAsync(hosts!, apiToken: "a@pve!t=x"));

    [Fact]
    public async Task Login_without_token_or_user_and_password_is_refused()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            await Assert.ThrowsAsync<ArgumentException>(() => ClientHelper.GetClientAndTryLoginAsync($"127.0.0.1:{port}", timeout: 2000));
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public async Task Login_with_a_token_checks_it_and_reports_the_refusal()
    {
        var handler = new FakeHandler(_ =>
        {
            var response = FakeHandler.Json(HttpStatusCode.Unauthorized, """{"data":null}""");
            response.ReasonPhrase = "Authentication failed!";
            return response;
        });
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var ex = await Assert.ThrowsAsync<PveException>(() => ClientHelper.GetClientAndTryLoginAsync($"127.0.0.1:{port}",
                                                                                                         apiToken: "nobody@pve!x=y",
                                                                                                         httpClient: new HttpClient(handler)));

            Assert.Contains("Authentication failed!", ex.Message);
            Assert.Equal("/api2/json/version", handler.Requests.Single().RequestUri!.AbsolutePath);
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public async Task Login_with_a_token_returns_the_client_of_the_host_that_answers()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":{"version":"9.2"}}"""));
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var client = await ClientHelper.GetClientAndTryLoginAsync($"127.0.0.1:{ClosedPort()},127.0.0.1:{port}",
                                                                      apiToken: "automation@pve!app=secret",
                                                                      validateCertificate: false,
                                                                      httpClient: new HttpClient(handler));

            Assert.Equal(port, client.Port);
            Assert.Equal("automation@pve!app=secret", client.ApiToken);
            Assert.False(client.ValidateCertificate);
        }
        finally
        {
            listener.Stop();
        }
    }
}
