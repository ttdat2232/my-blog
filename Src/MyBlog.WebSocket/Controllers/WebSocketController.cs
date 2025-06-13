using Microsoft.AspNetCore.Mvc;
using MyBlog.Core.Services.Socket;

namespace MyBlog.WebSocket.Controllers;

public class WebSocketController(IWebSocketManager _webSocketManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetConnectionIds() =>
        Ok(await _webSocketManager.GetConnectionIds());
}
