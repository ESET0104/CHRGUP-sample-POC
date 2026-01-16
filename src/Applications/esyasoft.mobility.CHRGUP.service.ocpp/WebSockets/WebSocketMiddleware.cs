using esyasoft.mobility.CHRGUP.service.ocpp.Services;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;

namespace esyasoft.mobility.CHRGUP.service.ocpp.WebSockets
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            var chargePointId = context.Request.Query["chargePointId"].ToString();
            var tenantId = context.Request.Query["tenantId"].ToString();

            //
            var scopeFactory = context.RequestServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var auth = scope.ServiceProvider.GetRequiredService<ChargerAuthService>();

            var valid = await auth.ValidateAsync(chargePointId, tenantId);

            if (!valid)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid charger");
                return;
            }
            //


            var socket = await context.WebSockets.AcceptWebSocketAsync();

            var connection = new ChargerConnection(
                chargePointId,
                tenantId,
                socket
            );

            await connection.ListenAsync();
        }
    }

}
