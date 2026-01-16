using esyasoft.mobility.CHRGUP.service.ocpp.Messaging;
using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp;
using esyasoft.mobility.CHRGUP.service.ocpp.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using esyasoft.mobility.CHRGUP.service.ocpp.WebSockets;

namespace esyasoft.mobility.CHRGUP.service.ocpp.WebSockets
{
    public enum LockedOcppProtocol
    {
        Unknown = 0,
        Ocpp16 = 16,
        Ocpp201 = 201
    }
    public class ChargerConnection
    {
        private readonly WebSocket _socket;
        private readonly string _chargePointId;
        private readonly string _tenantId;
        public LockedOcppProtocol LockedProtocol { get; private set; } = LockedOcppProtocol.Unknown;

        


        public ChargerConnection(string chargePointId, string tenantId, WebSocket socket)
        {
            _chargePointId = chargePointId;
            _tenantId = tenantId;
            _socket = socket;

            ChargerConnectionManager.RegisterConnection(chargePointId, this);
        }
        public void LockProtocol(LockedOcppProtocol protocol)
        {
            if (LockedProtocol == LockedOcppProtocol.Unknown)
            {
                LockedProtocol = protocol;
                return;
            }

            if (LockedProtocol != protocol)
            {
                throw new InvalidOperationException(
                    $"Protocol already locked to {LockedProtocol} but received {protocol}");
            }
        }

        public async Task ListenAsync()
        {
            ChargerConnectionManager.Add(_chargePointId, _socket);
            bool gracefulClose = false;

            try
            {
                var buffer = new byte[8192];


                while (_socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    try
                    {
                        result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                    }
                    catch (WebSocketException ex)
                    {
                        Console.WriteLine(
                            $"WebSocket abruptly closed for charger {_chargePointId}: {ex.Message}"
                        );
                        break;
                    }
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        gracefulClose = true;
                        break;
                    }
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var doc = JsonDocument.Parse(json);
                    //var payload = doc.RootElement[3];
                    var payload = doc.RootElement.GetArrayLength() > 3 ? doc.RootElement[3] : default;
                    await ProtocolRouter.RouteAsync(
                        json,
                        _chargePointId,
                        _tenantId,
                        //payload,
                        _socket);
                }
            }

            finally
            {
                ChargerProtocolStore.Remove(_chargePointId);
                CanonicalSessionStore.Remove(_chargePointId);
                HeartbeatStore.Remove(_chargePointId);
                ChargerConnectionManager.Remove(_chargePointId);
                if (!gracefulClose)
                {
                    Console.WriteLine($"Charger {_chargePointId} disconnected unexpectedly");

                    var state = ChargerStateStore.Get(_chargePointId);
                    await RabbitMqEventPublisher.PublishAsync(
                        "event.charger.faulted",
                        new
                        {
                            ChargerId = _chargePointId,
                            FaultCode = "PowerLoss",
                            Timestamp = DateTime.UtcNow
                        }
                    );
                    //if (state.ActiveSessionId != null)
                    //{

                    //    state.ActiveSessionId = null;
                    //}
                    var session = CanonicalSessionStore.GetOrCreate(_chargePointId, ChargerProtocolStore.Get(_chargePointId));
                    session.Active = false;
                }

                try
                {
                    if (_socket.State != WebSocketState.Closed)
                    {
                        await _socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Connection closed",
                            CancellationToken.None);
                    }
                }
                catch { }
            }

        }
    }
}
