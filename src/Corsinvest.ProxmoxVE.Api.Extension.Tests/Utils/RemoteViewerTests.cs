/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using System.Net.Sockets;
using System.Text;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Utils;

/// <summary>
/// Files for remote-viewer and the local port that carries VNC over the WebSocket of the node.
/// </summary>
public class RemoteViewerTests
{
    private const string VncTicket = "PVEVNC:6720F1A0::abc+def/ghi==";

    private static string PathOf(HttpRequestMessage request) => Uri.UnescapeDataString(request.RequestUri!.AbsolutePath);

    private static FakeHandler SpiceHandler()
        => new(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":{"type":"spice","host":"pvespiceproxy:abc","tls-port":61000,"password":"secret","proxy":"http://pve01:3128"}}"""));

    private static FakeHandler RefusedHandler(string reason)
        => new(_ =>
        {
            var response = FakeHandler.Json(HttpStatusCode.InternalServerError, """{"data":null}""");
            response.ReasonPhrase = reason;
            return response;
        });

    private static string[] TempFiles() => Directory.GetFiles(Path.GetTempPath(), "*.tmp");

    [Fact]
    public async Task Spice_file_of_a_guest_is_written_for_the_viewer()
    {
        var handler = SpiceHandler();
        var output = new StringWriter();
        var before = TempFiles();

        var (error, fileName) = await RemoteViewerHelper.PrepareSpiceAsync(FakeHandler.Client(handler), "pve01", VmType.Qemu, 100, output: output);

        try
        {
            Assert.Null(error);
            Assert.EndsWith(".vv", fileName);
            var lines = await File.ReadAllLinesAsync(fileName!);
            Assert.Equal("[virt-viewer]", lines[0]);
            Assert.Contains("type=spice", lines);
            Assert.Contains("tls-port=61000", lines);

            // without a proxy the viewer goes through the host of the client
            var request = handler.Requests.Single();
            Assert.Equal("/api2/json/nodes/pve01/qemu/100/spiceproxy", PathOf(request));
            Assert.Contains("\"proxy\":\"pve01\"", await request.Content!.ReadAsStringAsync());
            Assert.Equal("SPICE proxy: pve01" + Environment.NewLine, output.ToString());

            // only the file of the viewer is left in the temporary folder
            Assert.False(File.Exists(Path.ChangeExtension(fileName, ".tmp")));
            Assert.Empty(TempFiles().Except(before).Where(a => Path.GetFileNameWithoutExtension(a) == Path.GetFileNameWithoutExtension(fileName)));
        }
        finally
        {
            File.Delete(fileName!);
        }
    }

    [Fact]
    public async Task Spice_file_of_a_container_and_of_a_node_with_a_proxy()
    {
        var handler = SpiceHandler();
        var client = FakeHandler.Client(handler);

        var (_, container) = await RemoteViewerHelper.PrepareSpiceAsync(client, "pve01", VmType.Lxc, 200, "proxy.example.com");
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/spiceproxy", PathOf(handler.Requests.Last()));
        Assert.Contains("\"proxy\":\"proxy.example.com\"", await handler.Requests.Last().Content!.ReadAsStringAsync());

        var output = new StringWriter();
        var (error, node) = await RemoteViewerHelper.PrepareSpiceAsync(client, "pve01", " ", output);
        Assert.Null(error);
        Assert.Equal("/api2/json/nodes/pve01/spiceshell", PathOf(handler.Requests.Last()));
        Assert.Contains("\"proxy\":\"pve01\"", await handler.Requests.Last().Content!.ReadAsStringAsync());
        Assert.Equal("SPICE proxy: pve01" + Environment.NewLine, output.ToString());

        Assert.StartsWith("[virt-viewer]", await File.ReadAllTextAsync(container!));
        Assert.StartsWith("[virt-viewer]", await File.ReadAllTextAsync(node!));
        File.Delete(container!);
        File.Delete(node!);
    }

    [Fact]
    public async Task Refused_request_gives_the_reason_and_no_file()
    {
        var client = FakeHandler.Client(RefusedHandler("VM 100 not running"));
        var output = new StringWriter();

        Assert.Equal(("VM 100 not running", null), await RemoteViewerHelper.PrepareSpiceAsync(client, "pve01", VmType.Qemu, 100, output: output));
        Assert.Equal(("VM 100 not running", null), await RemoteViewerHelper.PrepareSpiceAsync(client, "pve01", output: output));
        Assert.Equal(("VM 100 not running", null, null), await RemoteViewerHelper.PrepareVncAsync(client, "pve01", VmType.Qemu, 100, output));
        Assert.Equal(("VM 100 not running", null, null), await RemoteViewerHelper.PrepareVncAsync(client, "pve01", VmType.Lxc, 200, output));
        Assert.Equal(("VM 100 not running", null, null), await RemoteViewerHelper.PrepareVncAsync(client, "pve01", output));
        Assert.Equal(string.Empty, output.ToString());
    }

    // a node that gives a VNC ticket and answers on the WebSocket by sending back what it receives
    private static LocalPveServer VncServer()
    {
        var server = new LocalPveServer
        {
            Rest = request => request.Path.EndsWith("/vncproxy") || request.Path.EndsWith("/vncshell")
                                ? (200, "OK", "{\"data\":{\"ticket\":\"" + VncTicket + "\",\"port\":\"5901\",\"user\":\"root@pam\"}}")
                                : (501, "Not expected", """{"data":null}"""),
            Socket = async (socket, _, cancellationToken) =>
            {
                while (await LocalPveServer.ReceiveAsync(socket, cancellationToken) is { } message)
                {
                    if (Encoding.ASCII.GetString(message) == "bye") { return; }
                    await socket.SendAsync(message, System.Net.WebSockets.WebSocketMessageType.Binary, true, cancellationToken);
                }
            },
        };
        return server;
    }

    private static async Task<string> ExchangeAsync(NetworkStream stream, string text)
    {
        await stream.WriteAsync(Encoding.ASCII.GetBytes(text));
        var buffer = new byte[text.Length];
        await stream.ReadExactlyAsync(buffer).AsTask().WaitAsync(TimeSpan.FromSeconds(10));
        return Encoding.ASCII.GetString(buffer);
    }

    [Theory]
    [InlineData(VmType.Qemu, 100, "/api2/json/nodes/pve01/qemu/100/vncproxy", "/api2/json/nodes/pve01/qemu/100/vncwebsocket", "VNC: VM 100")]
    [InlineData(VmType.Lxc, 200, "/api2/json/nodes/pve01/lxc/200/vncproxy", "/api2/json/nodes/pve01/lxc/200/vncwebsocket", "VNC: CT 200")]
    public async Task Vnc_of_a_guest_goes_through_a_local_port(VmType vmType, long vmId, string proxyPath, string socketPath, string title)
    {
        await using var server = VncServer();
        var client = server.Client();
        client.ApiToken = "automation@pve!app=secret";
        var output = new StringWriter();

        var (error, fileName, bridge) = await RemoteViewerHelper.PrepareVncAsync(client, "pve01", vmType, vmId, output);

        Assert.Null(error);
        await using (bridge)
        {
            Assert.Equal(["[virt-viewer]", "type=vnc", "host=127.0.0.1", $"port={bridge!.LocalPort}", $"password={VncTicket}", $"title={title}", "delete-this-file=1"],
                         await File.ReadAllLinesAsync(fileName!));
            File.Delete(fileName!);
            Assert.False(File.Exists(Path.ChangeExtension(fileName, ".tmp")));

            Assert.Equal([$"VNC WebSocket URL: wss://127.0.0.1:{server.Port}{socketPath}?port=5901&vncticket=PVEVNC%3a6720F1A0%3a%3aabc%2bdef%2fghi%3d%3d",
                          $"VNC local port: {bridge.LocalPort}"],
                         output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

            // what the viewer writes on the local port reaches the node, and the answer comes back
            using var viewer = new TcpClient();
            await viewer.ConnectAsync(IPAddress.Loopback, bridge.LocalPort);
            Assert.Equal("RFB 003.008\n", await ExchangeAsync(viewer.GetStream(), "RFB 003.008\n"));
            Assert.Equal("second", await ExchangeAsync(viewer.GetStream(), "second"));
        }

        var requests = server.Requests;
        Assert.Equal(proxyPath, requests[0].Path);
        Assert.Contains("\"websocket\":1", requests[0].Body.Replace("true", "1"));
        Assert.Equal(socketPath, requests[1].Path);
        Assert.Equal("5901", requests[1].Query["port"]);
        Assert.Equal(VncTicket, requests[1].Query["vncticket"]);
        Assert.Equal("binary", requests[1].Headers["Sec-WebSocket-Protocol"]);
    }

    [Fact]
    public async Task Vnc_of_a_node_goes_through_a_local_port()
    {
        await using var server = VncServer();
        var client = server.Client();
        client.ApiToken = "automation@pve!app=secret";

        var (error, fileName, bridge) = await RemoteViewerHelper.PrepareVncAsync(client, "pve01");

        Assert.Null(error);
        await using (bridge)
        {
            Assert.Contains("title=VNC: Node pve01", await File.ReadAllLinesAsync(fileName!));
            File.Delete(fileName!);

            using var viewer = new TcpClient();
            await viewer.ConnectAsync(IPAddress.Loopback, bridge!.LocalPort);
            Assert.Equal("hello", await ExchangeAsync(viewer.GetStream(), "hello"));
        }

        Assert.Equal("/api2/json/nodes/pve01/vncshell", server.Requests[0].Path);
        Assert.Equal("/api2/json/nodes/pve01/vncwebsocket", server.Requests[1].Path);
    }

    [Fact]
    public async Task Bridge_sends_the_session_cookie_to_the_node()
    {
        await using var server = VncServer();
        await using var bridge = new VncWebSocketBridge();

        bridge.Start($"wss://127.0.0.1:{server.Port}/api2/json/nodes/pve01/vncwebsocket?port=5901&vncticket=T", "127.0.0.1", "PVE:root@pam:TICKET");

        using var viewer = new TcpClient();
        await viewer.ConnectAsync(IPAddress.Loopback, bridge.LocalPort);
        Assert.Equal("hello", await ExchangeAsync(viewer.GetStream(), "hello"));
        Assert.Contains("PVEAuthCookie=PVE:root@pam:TICKET", server.Requests.Single().Headers["Cookie"]);
    }

    [Fact]
    public async Task Node_that_closes_the_session_closes_the_viewer()
    {
        await using var server = VncServer();
        await using var bridge = new VncWebSocketBridge();
        bridge.Start($"wss://127.0.0.1:{server.Port}/vncwebsocket", "127.0.0.1", "PVE:root@pam:TICKET");

        using var viewer = new TcpClient();
        await viewer.ConnectAsync(IPAddress.Loopback, bridge.LocalPort);
        var stream = viewer.GetStream();
        Assert.Equal("hello", await ExchangeAsync(stream, "hello"));

        // the test server ends the session when it reads "bye"
        await stream.WriteAsync(Encoding.ASCII.GetBytes("bye"));

        Assert.Equal(0, await stream.ReadAsync(new byte[16]).AsTask().WaitAsync(TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public async Task Viewer_that_closes_ends_the_session_on_the_node()
    {
        await using var server = VncServer();
        await using var bridge = new VncWebSocketBridge();
        bridge.Start($"wss://127.0.0.1:{server.Port}/vncwebsocket", "127.0.0.1", "PVE:root@pam:TICKET");

        using (var viewer = new TcpClient())
        {
            await viewer.ConnectAsync(IPAddress.Loopback, bridge.LocalPort);
            Assert.Equal("hello", await ExchangeAsync(viewer.GetStream(), "hello"));
        }

        // without this the console session stays open on the node until the tool exits
        await server.SocketClosed.WaitAsync(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task Bridge_with_a_checked_certificate_does_not_connect_to_a_self_signed_node()
    {
        await using var server = VncServer();
        await using var bridge = new VncWebSocketBridge();
        bridge.Start($"wss://127.0.0.1:{server.Port}/vncwebsocket", "127.0.0.1", "PVE:root@pam:TICKET", validateCertificate: true);

        await server.SocketClosed.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Empty(server.Requests);
    }

    [Fact]
    public async Task Bridge_that_is_never_used_can_be_disposed()
    {
        var bridge = new VncWebSocketBridge();
        Assert.InRange(bridge.LocalPort, 1, 65535);

        await bridge.DisposeAsync();

        using var viewer = new TcpClient();
        await Assert.ThrowsAnyAsync<SocketException>(() => viewer.ConnectAsync(IPAddress.Loopback, bridge.LocalPort));
    }

    [Fact]
    public void Viewer_is_started_with_the_file_and_the_options()
    {
        var output = new StringWriter();

        // dotnet is a program that every system of the tests has: "--version" ends with 0
        var exitCode = RemoteViewerHelper.Launch("dotnet", "--version", string.Empty, true, output);

        Assert.Equal(0, exitCode);
        var lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.StartsWith("Run FileName: ", lines[0]);
        Assert.StartsWith("Run Arguments: ", lines[1]);
        Assert.Contains("--version", lines[1]);
    }

    [Fact]
    public void Viewer_that_fails_gives_1()
        => Assert.Equal(1, RemoteViewerHelper.Launch("dotnet", "file-that-does-not-exist.dll", string.Empty, true));

    [Fact]
    public void Viewer_that_is_not_waited_gives_0()
        => Assert.Equal(0, RemoteViewerHelper.Launch("dotnet", "--info", string.Empty, false));
}
