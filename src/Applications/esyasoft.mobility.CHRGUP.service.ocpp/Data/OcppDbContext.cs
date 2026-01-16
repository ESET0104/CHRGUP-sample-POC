using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace esyasoft.mobility.CHRGUP.service.ocpp.Data
{
    public class OcppDbContext : DbContext
    {
        public OcppDbContext(DbContextOptions<OcppDbContext> options)
            : base(options) { }

        public DbSet<OcppCharger> Chargers => Set<OcppCharger>();
    }
}
