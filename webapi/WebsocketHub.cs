using Microsoft.AspNetCore.SignalR;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net.WebSockets;

namespace webapi;

public interface IWebSocketHub
{
    IDisposable RegisterWebsocket(WebSocket ws);
}

public class WebSocketHub : IWebSocketHub, IHostedService, IDisposable
{
    private readonly ILogger<WebSocketHub> logger;

    private readonly object socketsLock = new();
    private IImmutableSet<WebSocket> sockets = ImmutableHashSet<WebSocket>.Empty;
    private Timer? timer = null;

    public WebSocketHub(ILogger<WebSocketHub> logger)
    {
        this.logger = logger;
    }

    private TimeSpan AnnounceInterval { get; } = TimeSpan.FromSeconds(2);

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Background Service running.");
        timer = new Timer(DoWork, null, TimeSpan.Zero, AnnounceInterval);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }

    private async void DoWork(object? state)
    {
        foreach (var s in sockets)
        {
            try
            {
                if (s.State == WebSocketState.Closed
                    || s.State == WebSocketState.CloseSent
                    || s.State == WebSocketState.CloseReceived
                    || s.State == WebSocketState.Aborted)
                {

                    lock (socketsLock)
                    {
                        sockets = sockets.Remove(s);
                    }
                    continue;
                }
                else if (s.State == WebSocketState.Open)
                {
                    string msg = $"Hi socket ${s.GetHashCode()} ${DateTime.Now}";
                    var bytes = System.Text.Encoding.UTF8.GetBytes(msg);
                    await s.SendAsync(
                        bytes,
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error while websocket ${s}");
            }
        }
    }

    public IDisposable RegisterWebsocket(WebSocket ws)
    {
        return new WebSocetRegistrationCancellation(this, ws);
    }

    private class WebSocetRegistrationCancellation : IDisposable
    {
        private readonly WebSocketHub hub;
        private readonly WebSocket ws;

        public WebSocetRegistrationCancellation(WebSocketHub websocketHub, WebSocket ws)
        {
            this.hub = websocketHub;
            this.ws = ws;

            lock (hub.socketsLock)
            {
                hub.sockets = hub.sockets.Add(ws);
            }
        }

        public void Dispose()
        {
            lock (hub.socketsLock)
            {
                hub.sockets = hub.sockets.Remove(ws);
            }
        }
    }
}
