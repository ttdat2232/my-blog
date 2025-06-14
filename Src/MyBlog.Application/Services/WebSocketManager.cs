using System.Collections.Concurrent;
using System.Net.WebSockets;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Services.Cache;
using MyBlog.Core.Services.Socket;

namespace MyBlog.Application.Services;

public class WebSocketManager(ICacheService _cacheService) : IWebSocketManager
{
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

    public async Task AddConnectionAsync(
        string connectionId,
        WebSocket webSocket,
        UserId userId,
        CancellationToken cancellationToken = default
    )
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
            _defaultExpiry,
            cancellationToken
        );
        var userConnections =
            await _cacheService.GetAsync<List<string>>(
                [USER_CONNECTIONS_PREFIX, userId.Value.ToString()],
                cancellationToken
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

    public Task<WebSocket?> GetConnection(
        string connectionId,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(_localConnections[connectionId] ?? default);

    public async Task<IEnumerable<string>> GetConnectionIds(
        CancellationToken cancellationToken = default
    ) =>
        (await _cacheService.GetAsync<List<string>>([ALL_CONNECTIONS_KEY], cancellationToken))
        ?? [];

    public async Task<IEnumerable<string>> GetConnectionsByUserId(
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        var userConnections =
            (
                await _cacheService.GetAsync<List<string>>(
                    [USER_CONNECTIONS_PREFIX, userId],
                    cancellationToken
                )
            ) ?? [];

        return userConnections.Where(IsConnected);
    }

    public bool IsConnected(string connectionId)
    {
        if (!_localConnections.TryGetValue(connectionId, out var socket))
            return false;
        return socket.State == WebSocketState.Open;
    }

    public async Task RemoveConnectionAsync(
        string connectionId,
        CancellationToken cancellationToken = default
    )
    {
        if (
            _localConnections.TryRemove(connectionId, out var webSocket)
            && webSocket.State == WebSocketState.Open
        )
        {
            await webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Connection closed",
                cancellationToken
            );
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

    public async Task SendMessageAsync(
        string connectionId,
        string message,
        CancellationToken cancellationToken = default
    )
    {
        var webSocket = await GetConnection(connectionId, cancellationToken);
        if (webSocket == null)
            return;

        if (webSocket.State == WebSocketState.Open)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                System.Net.WebSockets.WebSocketMessageType.Text,
                true,
                cancellationToken
            );
        }
    }

    public async Task SendMessageToUserAsync(
        string userId,
        string message,
        CancellationToken cancellationToken = default
    )
    {
        var connections = (await GetConnectionsByUserId(userId, cancellationToken)).ToList();
        var tasks = connections.Select(connectionId =>
            SendMessageAsync(connectionId, message, cancellationToken)
        );
        await Task.WhenAll(tasks);
    }

    public async Task SendMessageToAllAsync(
        string message,
        CancellationToken cancellationToken = default
    )
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
                    cancellationToken
                );
            });

        await Task.WhenAll(tasks);
    }
}
