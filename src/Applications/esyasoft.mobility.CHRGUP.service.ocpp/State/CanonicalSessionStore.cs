using System.Collections.Concurrent;
namespace esyasoft.mobility.CHRGUP.service.ocpp.State
{
    public class CanonicalSessionStore
    {
        private static readonly ConcurrentDictionary<(string, int), CanonicalChargeSession> _sessions = new();

        //private static string Key(string chargerId, int evseId)
        //    => $"{chargerId}::{evseId}";


        public static CanonicalChargeSession GetOrCreate(string chargerId, OcppProtocol protocol,int evseId=1)
        {
            return _sessions.GetOrAdd((chargerId, evseId), _ => new CanonicalChargeSession
            {
                ChargerId = chargerId,
                Protocol = protocol,
                EvseID = evseId
            });
        }

        public static IEnumerable<CanonicalChargeSession> GetAllForCharger(string chargerId)
        {
            return _sessions.Values.Where(s => s.ChargerId == chargerId);
        }

        public static void Remove(string chargerId)
        {
            foreach (var key in _sessions.Keys)
            {
                if (key.Item1 == chargerId)
                    _sessions.TryRemove(key, out _);
            }
        }
    }
}
