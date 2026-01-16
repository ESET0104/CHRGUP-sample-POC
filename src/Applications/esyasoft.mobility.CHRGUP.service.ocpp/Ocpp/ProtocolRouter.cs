using System.Text.Json;
using System.Net.WebSockets;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp201;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp16;


namespace esyasoft.mobility.CHRGUP.service.ocpp.Ocpp
{
    public class ProtocolRouter
    {

        public static async Task RouteAsync(
            string json,
            string chargerId,
            string tenantId,
            WebSocket socket)
        {



            var doc = JsonDocument.Parse(json);
            var payload = doc.RootElement.GetArrayLength() > 3
                ? doc.RootElement[3]
                : default;

            var action = doc.RootElement[2].GetString();

            if (action != "BootNotification" && !ChargerProtocolStore.HasBooted(chargerId))
            {
                Console.WriteLine($"Protocol violation: {chargerId} sent {action} before BootNotification");
                return;
            }

            // Detect protocol only once (BootNotification)
            if (doc.RootElement[2].GetString() == "BootNotification")
            {
                ChargerProtocolStore.MarkBooted(chargerId);

                if (payload.TryGetProperty("chargingStation", out _))
                {
                    ChargerProtocolStore.Set(chargerId, OcppProtocol.V201);
                }
                else
                {
                    ChargerProtocolStore.Set(chargerId, OcppProtocol.V16);
                }
            }

            var protocol = ChargerProtocolStore.Get(chargerId);

            if (protocol == OcppProtocol.V201)
            {
                await Ocpp201.OcppRouter.RouteAsync(json, chargerId, tenantId, socket);
            }
            else
            {
                await Ocpp16Router.RouteAsync(json, chargerId, tenantId, socket);
            }

        }
    }
}
