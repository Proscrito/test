using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using yamvc.Core.Service;

namespace yamvc.Core.WebSocket
{
    public class ManagerMessageHandler : WebSocketHandler
    {
        private IHttpContextAccessor _httpContextAccessor;

        public ManagerMessageHandler(WebSocketConnectionManager webSocketConnectionManager, IHttpContextAccessor httpContextAccessor) : base(webSocketConnectionManager)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task OnConnected(System.Net.WebSockets.WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);
            var login = _httpContextAccessor.HttpContext.User.Identity.Name;
            UserManager.Instance.AssignSocket(login, socketId);
            var user = UserManager.Instance.Find(login);

            await SendMessageToAllAsync(JsonConvert.SerializeObject(user));
        }

        public override async Task ReceiveAsync(System.Net.WebSockets.WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketIdToLogout = Encoding.UTF8.GetString(buffer, 0, result.Count);

            await SendMessageAsync(socketIdToLogout, "logout");
        }
    }
}