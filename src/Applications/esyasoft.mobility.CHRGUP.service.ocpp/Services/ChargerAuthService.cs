using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Services
{
    public class ChargerAuthService
    {
        private readonly AppDbContext _db;

        public ChargerAuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ValidateAsync(string chargerId, string tenantId)
        {
            var charger = await _db.chargerConfigs
                .FirstOrDefaultAsync(c =>
                    c.ChargerId == chargerId);

            if (charger == null)
            {
                Console.WriteLine("charger not found in config");
                return false;
            }
            else
                return true;
            
        }
    }
}
