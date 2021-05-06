using Microsoft.AspNetCore.Builder;

namespace WebSocketServer.MiddleWare
{
    public static class WebSocketServerMiddlewareExtentions
    {
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketServerMiddleware>();
        }
    }
}