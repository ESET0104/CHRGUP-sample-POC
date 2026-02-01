using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16.Handlers;
using System.Net.WebSockets;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16
{
    public class Ocpp16Router
    {
        public static async Task RouteAsync(
            string json,
            string chargerId,
            string tenantId,
            WebSocket socket)
        {
            var msg = JsonDocument.Parse(json).RootElement;
            var action = msg[2].GetString();
            var payload = msg[3];
            var messageId = msg[1].GetString();

            switch (action)
            {
                case "BootNotification":
                    Console.WriteLine("redirected to bootnot handler--v16");
                    await BootHandler.Handle(chargerId, messageId, socket);
                    break;

                case "Authorize":
                    Console.WriteLine("redirected to auth handler--v16");
                    await AuthorizeHandler.Handle(payload, chargerId, messageId, socket);
                    break;

                case "Heartbeat":
                    Console.WriteLine("redirected to heartbeat handler--v16");
                    await HeartbeatHandler.Handle(chargerId, messageId, socket);
                    break;

                case "StartTransaction":
                    Console.WriteLine("redirected to startevt handler--v16");
                    await TransactionHandler.HandleStart(payload, chargerId, messageId);
                    break;

                case "StopTransaction":
                    Console.WriteLine("redirected to stopevt handler--v16");
                    await TransactionHandler.HandleStop(payload, chargerId, messageId);
                    break;

                case "MeterValues":
                    Console.WriteLine("redirected to mv handler--v16");
                    await MeterHandler.Handle(payload, chargerId);
                    break;

                case "StatusNotification":
                    Console.WriteLine("redirected to statnot handler--v16");
                    await StatusNotificationHandler.Handle(payload, chargerId);
                    break;
            }
        }
    }
}
