using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using yamvc.Core.WebSocket;

namespace yamvc.Core.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler handler)
        {
            return app.Map(path, _app => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }
    }
}
