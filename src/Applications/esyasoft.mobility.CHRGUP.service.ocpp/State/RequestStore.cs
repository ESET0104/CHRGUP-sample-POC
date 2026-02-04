using System.Collections.Concurrent;

namespace esyasoft.mobility.CHRGUP.service.ocpp.State
{
    public class RequestStore
    {
        private static readonly ConcurrentDictionary<string, PendingOcppRequest> _pending = new();

        public static void Add(string messageId, PendingOcppRequest req)
            => _pending[messageId] = req;

        public static bool TryTake(string messageId, out PendingOcppRequest req)
            => _pending.TryRemove(messageId, out req!);
    }

    public enum PendingOcppRequestType
    {
        RemoteStart,
        RemoteStop
    }

    public record PendingOcppRequest(
        string ChargerId,
        string SessionId,
        PendingOcppRequestType Type
    );
}
