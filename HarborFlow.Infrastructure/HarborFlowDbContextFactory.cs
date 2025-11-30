using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HarborFlow.Infrastructure
{
    public class HarborFlowDbContextFactory : IDesignTimeDbContextFactory<HarborFlowDbContext>
    {
        public HarborFlowDbContext CreateDbContext(string[] args)
        {
            // This factory is used by the EF Core tools (e.g., for migrations) at design time.
            // It manually builds the configuration to get the connection string.

            // Get the path to the appsettings.json in the WPF project
            var basePath = Directory.GetCurrentDirectory();
            var wpfProjectPath = Path.GetFullPath(Path.Combine(basePath, "../HarborFlow.Wpf"));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(wpfProjectPath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HarborFlowDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlite(connectionString);

            return new HarborFlowDbContext(optionsBuilder.Options);
        }
    }
}
