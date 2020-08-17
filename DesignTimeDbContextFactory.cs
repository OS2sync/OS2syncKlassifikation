using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StsKlassifikation.DBContext;
using System.IO;

namespace StsKlassifikation
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ClassificationContext>
    {
        public ClassificationContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("sqlserver");

            var builder = new DbContextOptionsBuilder<ClassificationContext>();
            builder.UseSqlServer(connectionString);

            return new ClassificationContext(builder.Options);
        }
    }
}
