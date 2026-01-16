using esyasoft.mobility.CHRGUP.service.ocpp.Data;
using Microsoft.EntityFrameworkCore;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Services
{
    public class ChargerAuthService
    {
        private readonly OcppDbContext _db;

        public ChargerAuthService(OcppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ValidateAsync(string chargerId, string tenantId)
        {
            var charger = await _db.Chargers
                .FirstOrDefaultAsync(c =>
                    c.ChargerId == chargerId &&
                    c.TenantId == tenantId &&
                    c.IsEnabled);

            if (charger == null)
                return false;

            charger.LastSeen = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
