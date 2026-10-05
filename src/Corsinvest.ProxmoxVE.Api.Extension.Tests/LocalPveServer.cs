/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

/// <summary>
/// HTTPS server on a free local port with a self-signed certificate, as a node has: it answers the REST calls
/// from a function and gives the WebSocket connections to another one. For the code that opens its own connection.
/// </summary>
internal sealed class LocalPveServer : IAsyncDisposable
{
    public sealed record Request(string Method, string Target, IReadOnlyDictionary<string, string> Headers, string Body)
    {
        public string Path => Uri.UnescapeDataString(Target.Split('?')[0]);
        public NameValueCollection Query => System.Web.HttpUtility.ParseQueryString(Target.Contains('?') ? Target[(Target.IndexOf('?') + 1)..] : string.Empty);
    }

    private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
    private readonly X509Certificate2 _certificate = CreateCertificate();
    private readonly CancellationTokenSource _cts = new();
    private readonly ConcurrentQueue<Request> _requests = new();
    private readonly TaskCompletionSource _socketClosed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Task _acceptLoop;

    public LocalPveServer()
    {
        _listener.Start();
        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _acceptLoop = AcceptLoopAsync();
    }

    public string Host => "127.0.0.1";
    public int Port { get; }

    /// <summary>Answer of a REST call: status, reason and JSON body.</summary>
    public Func<Request, (int Status, string Reason, string Json)> Rest { get; set; } = _ => (200, "OK", """{"data":null}""");

    /// <summary>What the server does with a WebSocket connection. The connection ends when the function returns.</summary>
    public Func<WebSocket, Request, CancellationToken, Task> Socket { get; set; } = (_, _, _) => Task.CompletedTask;

    public IReadOnlyList<Request> Requests => [.. _requests];

    /// <summary>Completed when the first WebSocket connection is over, or a client went away before asking for one.</summary>
    public Task SocketClosed => _socketClosed.Task;

    public PveClient Client() => new(Host, Port);

    public static Task SendTextAsync(WebSocket socket, string text, CancellationToken cancellationToken)
        => socket.SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Binary, true, cancellationToken);

    /// <summary>A whole message, null when the other side closes.</summary>
    public static async Task<byte[]?> ReceiveAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        var buffer = new byte[16384];
        using var message = new MemoryStream();
        WebSocketReceiveResult result;
        do
        {
            result = await socket.ReceiveAsync(buffer, cancellationToken);
            if (result.MessageType == WebSocketMessageType.Close) { return null; }
            message.Write(buffer, 0, result.Count);
        } while (!result.EndOfMessage);

        return message.ToArray();
    }

    private static X509Certificate2 CreateCertificate()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var names = new SubjectAlternativeNameBuilder();
        names.AddIpAddress(IPAddress.Loopback);
        request.CertificateExtensions.Add(names.Build());

        using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));

        // through a PKCS#12 so the private key can be used by a TLS server on every system
        var pfx = certificate.Export(X509ContentType.Pfx);
#if NET9_0_OR_GREATER
        return X509CertificateLoader.LoadPkcs12(pfx, null);
#else
        return new X509Certificate2(pfx);
#endif
    }

    private async Task AcceptLoopAsync()
    {
        try
        {
            while (true)
            {
                var tcp = await _listener.AcceptTcpClientAsync(_cts.Token);
                _ = Task.Run(() => ServeAsync(tcp));
            }
        }
        catch (Exception ex) when (ex is OperationCanceledException or SocketException or ObjectDisposedException) { }
    }

    private async Task ServeAsync(TcpClient tcp)
    {
        try
        {
            using var connection = tcp;
            await using var stream = new SslStream(tcp.GetStream());
            await stream.AuthenticateAsServerAsync(new SslServerAuthenticationOptions { ServerCertificate = _certificate }, _cts.Token);

            var request = await ReadRequestAsync(stream);
            if (request == null)
            {
                // a client that refuses the certificate closes without a request
                _socketClosed.TrySetResult();
                return;
            }
            _requests.Enqueue(request);

            if (request.Headers.TryGetValue("Sec-WebSocket-Key", out var key))
            {
                // answer of the WebSocket handshake, RFC 6455
                var accept = Convert.ToBase64String(SHA1.HashData(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11")));
                var protocol = request.Headers.GetValueOrDefault("Sec-WebSocket-Protocol");
                await WriteAsync(stream, "HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\n"
                                         + $"Sec-WebSocket-Accept: {accept}\r\n"
                                         + (protocol == null ? string.Empty : $"Sec-WebSocket-Protocol: {protocol}\r\n")
                                         + "\r\n");

                using var socket = WebSocket.CreateFromStream(stream, true, protocol, Timeout.InfiniteTimeSpan);
                try
                {
                    await Socket(socket, request, _cts.Token);
                }
                finally
                {
                    _socketClosed.TrySetResult();
                }
            }
            else
            {
                var (status, reason, json) = Rest(request);
                var body = Encoding.UTF8.GetBytes(json);
                await WriteAsync(stream, $"HTTP/1.1 {status} {reason}\r\nContent-Type: application/json\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n");
                await stream.WriteAsync(body, _cts.Token);
                await stream.FlushAsync(_cts.Token);
            }
        }
        catch
        {
            // a client that goes away, or a refused certificate: nothing to answer
            _socketClosed.TrySetResult();
        }
    }

    private Task WriteAsync(Stream stream, string text) => stream.WriteAsync(Encoding.ASCII.GetBytes(text), _cts.Token).AsTask();

    private async Task<Request?> ReadRequestAsync(Stream stream)
    {
        // the head one byte at a time, so nothing of the body or of the WebSocket data is read here
        var head = new List<byte>();
        var one = new byte[1];
        while (head.Count < 4 || head[^4] != '\r' || head[^3] != '\n' || head[^2] != '\r' || head[^1] != '\n')
        {
            if (await stream.ReadAsync(one, _cts.Token) == 0) { return null; }
            head.Add(one[0]);
        }

        var lines = Encoding.ASCII.GetString([.. head]).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        var first = lines[0].Split(' ');
        var headers = lines.Skip(1)
                           .Select(a => a.Split(':', 2))
                           .ToDictionary(a => a[0].Trim(), a => a[1].Trim(), StringComparer.OrdinalIgnoreCase);

        var body = new byte[headers.TryGetValue("Content-Length", out var length) ? int.Parse(length) : 0];
        await stream.ReadExactlyAsync(body, _cts.Token);

        return new(first[0], first[1], headers, Encoding.UTF8.GetString(body));
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _listener.Stop();
        await _acceptLoop;
        _certificate.Dispose();
        _cts.Dispose();
    }
}
