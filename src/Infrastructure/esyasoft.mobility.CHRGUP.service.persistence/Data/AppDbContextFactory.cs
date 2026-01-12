using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace esyasoft.mobility.CHRGUP.service.persistence.Data
{
    public class AppDbContextFactory
        : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var current = Directory.GetCurrentDirectory();

            // Walk UP until we find "src"
            while (!Directory.Exists(Path.Combine(current, "src")))
            {
                current = Directory.GetParent(current)?.FullName
                          ?? throw new DirectoryNotFoundException(
                              "Could not locate solution root containing 'src' folder.");
            }

            var apiPath = Path.Combine(
                current,
                "src",
                "Applications",
                "esyasoft.mobility.CHRGUP.service.api");

            if (!Directory.Exists(apiPath))
                throw new DirectoryNotFoundException(
                    $"API project not found at: {apiPath}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'Default' not found.");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    connectionString,
                    b => b.MigrationsAssembly(
                        "esyasoft.mobility.CHRGUP.service.persistence"))
                .Options;

            return new AppDbContext(options);
        }
    }
}
