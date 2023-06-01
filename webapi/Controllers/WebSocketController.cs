using Microsoft.AspNetCore.Mvc;

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Timers;

namespace webapi.Controllers;

#if true
public class WebSocketController : ControllerBase
{
    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            var str = System.Text.Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, receiveResult.Count));

            var str2 = Regex.Replace(str, "^ping", "pong");

            await webSocket.SendAsync(
                System.Text.Encoding.UTF8.GetBytes(str2),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}
#endif
