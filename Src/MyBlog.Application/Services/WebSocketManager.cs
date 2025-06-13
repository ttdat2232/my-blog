using System.Collections.Concurrent;
using System.Net.WebSockets;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Services.Cache;
using MyBlog.Core.Services.Socket;

namespace MyBlog.Application.Services;

public class WebSocketManager(ICacheService _cacheService) : IWebSocketManager
{
    // private const string ROOM_CONNECTIONS_PREFIX = "ws:room";
    private const string CONNECTION_INFO_PREFIX = "ws:connection";
    private const string USER_CONNECTIONS_PREFIX = "ws:user";
    private const string ALL_CONNECTIONS_KEY = "ws:all_connections";
    private readonly TimeSpan _defaultExpiry = TimeSpan.FromHours(24);
    private readonly ConcurrentDictionary<string, WebSocket> _localConnections = new();

    private sealed record WebSocketConnection(
        string Id,
        Guid UserId,
        string ConnectionId,
        DateTime ConnectedAt,
        bool IsActive,
        Dictionary<string, object> Metadata
    );

    public async Task AddConnectionAsync(string connectionId, WebSocket webSocket, UserId userId)
    {
        _localConnections[connectionId] = webSocket;
        var connectionInfo = new WebSocketConnection(
            string.Empty,
            userId.Value,
            connectionId,
            DateTime.UtcNow,
            true,
            new()
        );

        await _cacheService.SetAsync(
            [CONNECTION_INFO_PREFIX, connectionId],
            connectionInfo,
            _defaultExpiry
        );
        var userConnections =
            await _cacheService.GetAsync<List<string>>(
                [USER_CONNECTIONS_PREFIX, userId.Value.ToString()]
            ) ?? new();
        if (!userConnections.Contains(connectionId))
        {
            userConnections.Add(connectionId);
            await _cacheService.SetAsync(
                [USER_CONNECTIONS_PREFIX, userId.Value.ToString()],
                userConnections,
                _defaultExpiry
            );
        }
    }

    public Task<WebSocket?> GetConnection(string connectionId) =>
        Task.FromResult(_localConnections[connectionId] ?? default);

    public async Task<IEnumerable<string>> GetConnectionIds() =>
        await _cacheService.GetAsync<List<string>>([ALL_CONNECTIONS_KEY]) ?? [];

    public async Task<IEnumerable<string>> GetConnectionsByUserId(string userId)
    {
        var userConnections =
            await _cacheService.GetAsync<List<string>>([USER_CONNECTIONS_PREFIX, userId]) ?? [];

        return userConnections.Where(IsConnected);
    }

    public bool IsConnected(string connectionId)
    {
        if (!_localConnections.TryGetValue(connectionId, out var socket))
            return false;
        return socket.State == WebSocketState.Closed || socket.State == WebSocketState.Aborted;
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        if (_localConnections.TryRemove(connectionId, out var webSocket))
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Connection closed",
                    CancellationToken.None
                );
            }
        }
        var connectionInfo = await _cacheService.GetAsync<WebSocketConnection>(
            [CONNECTION_INFO_PREFIX, connectionId]
        );

        if (connectionInfo != null)
        {
            var userConnections = await _cacheService.GetAsync<List<string>>(
                [USER_CONNECTIONS_PREFIX, connectionInfo.UserId.ToString()]
            );

            if (userConnections != null)
            {
                userConnections.Remove(connectionId);
                if (userConnections.Any())
                {
                    await _cacheService.SetAsync(
                        [USER_CONNECTIONS_PREFIX, connectionInfo.UserId.ToString()],
                        userConnections,
                        _defaultExpiry
                    );
                }
                else
                {
                    await _cacheService.RemoveAsync<List<string>>(
                        [USER_CONNECTIONS_PREFIX, connectionInfo.UserId.ToString()]
                    );
                }
            }

            var allConnections = await _cacheService.GetAsync<List<string>>([ALL_CONNECTIONS_KEY]);

            if (allConnections != null)
            {
                allConnections.Remove(connectionId);
                await _cacheService.SetAsync([ALL_CONNECTIONS_KEY], allConnections, _defaultExpiry);
            }

            await _cacheService.RemoveAsync<WebSocketConnection>(
                [CONNECTION_INFO_PREFIX, connectionId]
            );
        }

        Serilog.Log.Information("Connection {ConnectionId} removed", connectionId);
    }

    public async Task SendMessageAsync(string connectionId, string message)
    {
        var webSocket = await GetConnection(connectionId);
        if (webSocket == null)
            return;

        if (webSocket.State == WebSocketState.Open)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                System.Net.WebSockets.WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
    }

    public async Task SendMessageToUserAsync(string userId, string message)
    {
        var connections = await GetConnectionsByUserId(userId);
        var tasks = connections.Select(connectionId => SendMessageAsync(connectionId, message));
        await Task.WhenAll(tasks);
    }

    public async Task SendMessageToAllAsync(string message)
    {
        var tasks = _localConnections
            .Values.Where(ws => ws.State == WebSocketState.Open)
            .Select(async webSocket =>
            {
                var buffer = System.Text.Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    System.Net.WebSockets.WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            });

        await Task.WhenAll(tasks);
    }
}
