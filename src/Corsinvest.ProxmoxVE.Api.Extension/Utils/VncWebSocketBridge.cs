/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */
#nullable enable

using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Bridges a Proxmox VNC WebSocket to a local TCP port so that
/// remote-viewer (which speaks plain TCP VNC) can connect.
/// The bridge carries a single session: when the viewer or the node ends it, or one direction fails,
/// both connections are closed and the local port stops accepting, so the viewer sees the disconnect.
/// </summary>
public sealed class VncWebSocketBridge : IAsyncDisposable
{
    private readonly TcpListener _listener;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _cts = new();
    private Task? _acceptLoop;

    //time between the pings to the node; on .NET 9 and later also the time to wait for the answer
    private static readonly TimeSpan KeepAlive = TimeSpan.FromSeconds(30);

    //time given to the close handshake of the websocket
    private static readonly TimeSpan CloseTimeout = TimeSpan.FromSeconds(5);

    /// <summary>Local TCP port that remote-viewer should connect to.</summary>
    public int LocalPort { get; }

    /// <summary>Creates the bridge and starts listening on a random local port.</summary>
    public VncWebSocketBridge() : this(null) { }

    /// <summary>
    /// Creates the bridge and starts listening on a random local port.
    /// The end of the session and every failure of the bridge are written to <paramref name="logger"/>.
    /// </summary>
    public VncWebSocketBridge(ILogger? logger)
    {
        _logger = logger ?? NullLogger.Instance;
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        LocalPort = ((IPEndPoint)_listener.LocalEndpoint).Port;
    }

    /// <summary>
    /// Connects to the Proxmox VNC WebSocket and waits for remote-viewer to connect.
    /// The node's certificate is checked when <paramref name="validateCertificate"/> is true: pass
    /// <c>PveClient.ValidateCertificate</c>, since the session cookie is sent on this connection too.
    /// Default false, the same default as <c>PveClient</c>.
    /// The bridge runs on the thread pool: it does not depend on the context of the caller, such as a UI thread.
    /// </summary>
    public void Start(string wsUrl, string host, string pveAuthCookie, bool validateCertificate = false)
        //without Task.Run every await of the relay would go back to the context of the caller:
        //a busy or throttled UI thread would stop the session without an error or a close
        => _acceptLoop = Task.Run(() => AcceptLoopAsync(wsUrl, host, pveAuthCookie, validateCertificate, _cts.Token));

    private async Task AcceptLoopAsync(string wsUrl,
                                       string host,
                                       string pveAuthCookie,
                                       bool validateCertificate,
                                       CancellationToken ct)
    {
        try
        {
            //disposed when the viewer or the node closes: the console session on the node ends with it
            using var ws = new ClientWebSocket();
            ws.Options.AddSubProtocol("binary");
            var cookies = new CookieContainer();
            cookies.Add(new Cookie("PVEAuthCookie", pveAuthCookie, "/", host) { Secure = true });
            ws.Options.Cookies = cookies;
            if (!validateCertificate) { ws.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true; }

            //a connection that dies without a close is noticed instead of waiting for TCP to give up
            ws.Options.KeepAliveInterval = KeepAlive;
#if NET9_0_OR_GREATER
            ws.Options.KeepAliveTimeout = KeepAlive;
#endif

            await ws.ConnectAsync(new Uri(wsUrl), ct).ConfigureAwait(false);
            _logger.LogDebug("VNC bridge: connected to the node, waiting for the viewer on port {Port}", LocalPort);

            //disposed before the websocket: the viewer sees the TCP close whenever the session is gone
            using var tcp = await _listener.AcceptTcpClientAsync(ct).ConfigureAwait(false);
            tcp.NoDelay = true;

            //a single viewer per session: a second one is refused instead of hanging on a dead port
            _listener.Stop();
            _logger.LogDebug("VNC bridge: viewer connected");

            //when either direction ends or fails, the other one is cancelled
            using var session = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var stream = tcp.GetStream();
            await Task.WhenAll(TcpToWsAsync(stream, ws, session), WsToTcpAsync(ws, stream, session)).ConfigureAwait(false);

            _logger.LogDebug("VNC bridge: session ended");
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            //the bridge was disposed
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "VNC bridge: {Message}", ex.Message);
        }
        finally
        {
            _listener.Stop();
        }
    }

    private async Task TcpToWsAsync(NetworkStream src, ClientWebSocket dst, CancellationTokenSource session)
    {
        var buf = new byte[65536];
        try
        {
            int read;
            while ((read = await src.ReadAsync(buf, session.Token).ConfigureAwait(false)) > 0)
            {
                await dst.SendAsync(buf.AsMemory(0, read), WebSocketMessageType.Binary, true, session.Token).ConfigureAwait(false);
            }

            _logger.LogDebug("VNC bridge: the viewer closed the connection");
            await CloseAsync(dst).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (session.IsCancellationRequested)
        {
            //the other direction ended first
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "VNC bridge: viewer to node stopped: {Message}", ex.Message);
        }
        finally
        {
            session.Cancel();
        }
    }

    private async Task WsToTcpAsync(ClientWebSocket src, NetworkStream dst, CancellationTokenSource session)
    {
        var buf = new byte[65536];
        try
        {
            while (true)
            {
                var result = await src.ReceiveAsync(buf.AsMemory(), session.Token).ConfigureAwait(false);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogDebug("VNC bridge: the node closed the session ({Status} {Description})",
                                     src.CloseStatus,
                                     src.CloseStatusDescription);
                    await CloseAsync(src).ConfigureAwait(false);
                    break;
                }
                await dst.WriteAsync(buf.AsMemory(0, result.Count), session.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (session.IsCancellationRequested)
        {
            //the other direction ended first
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "VNC bridge: node to viewer stopped: {Message}", ex.Message);
        }
        finally
        {
            session.Cancel();
        }
    }

    //close handshake towards the node, so the console session ends there too
    private async Task CloseAsync(ClientWebSocket ws)
    {
        if (ws.State is not (WebSocketState.Open or WebSocketState.CloseReceived)) { return; }

        using var timeout = new CancellationTokenSource(CloseTimeout);
        try
        {
            await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, timeout.Token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            //the connections are disposed anyway
            _logger.LogDebug(ex, "VNC bridge: close of the websocket failed: {Message}", ex.Message);
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _listener.Stop();
        if (_acceptLoop != null) { await _acceptLoop.ConfigureAwait(false); }
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
