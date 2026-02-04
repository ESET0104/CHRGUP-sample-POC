using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201.Handlers;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.WebSockets;
using System.Text.Json;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201
{
  
    public static class OcppRouter
    {
        public static async Task RouteAsync(
            string json,
            string chargePointId,
            string tenantId,
            WebSocket socket)
        {
            var message = OcppMessage.Parse(json);

            if (message.MessageType == 3 || message.MessageType == 4) 
            {

                if (RequestStore.TryTake(message.MessageId, out var pending))
                {
                    await OcppMessage.HandleCallResult(pending, message.Payload);
                }
                return;
            }

            switch (message.Action)
            {
                case "BootNotification":
                    Console.WriteLine("redirected to bootnot handler--v201");
                    await BootNotificationHandler.Handle(
                        chargePointId,
                        message.MessageId,
                        socket);
                    break;

                case "Authorize":
                    Console.WriteLine("redirected to auth handler--v201");
                    await AuthorizeHandler.Handle(
                        message.MessageId,
                        message.Payload,
                        chargePointId,
                        socket);
                    break;

                case "Heartbeat":
                    Console.WriteLine("redirected to heartbeat handler--v201");
                    await HeartbeatHandler.Handle(
                        message.MessageId,
                        chargePointId,
                        socket);
                    break;

                case "TransactionEvent":
                    Console.WriteLine("redirected to transevt handler--v201");
                    await TransactionEventHandler.Handle(
                        message.MessageId,
                        message.Payload,
                        chargePointId,
                        socket);
                    break;

                case "MeterValues":
                    Console.WriteLine("redirected to mv handler--v201");
                    await MeterValuesHandler.Handle(
                        message.MessageId,
                        message.Payload,
                        chargePointId,
                        socket);
                    break;

                case "StatusNotification":
                    Console.WriteLine("redirected to statnot handler--v201");
                    await StatusNotificationHandler.Handle(
                        message.MessageId,
                        message.Payload,
                        chargePointId,
                        socket);
                    break;
            }
        }

    }

}
