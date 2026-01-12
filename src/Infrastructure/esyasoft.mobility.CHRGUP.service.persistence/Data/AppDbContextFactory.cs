using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace esyasoft.mobility.CHRGUP.service.persistence.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Applications/esyasoft.mobility.CHRGUP.service.api"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connString, b =>
                b.MigrationsAssembly("esyasoft.mobility.CHRGUP.service.persistence"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
