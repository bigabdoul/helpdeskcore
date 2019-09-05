using HelpDeskCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace HelpDeskCore.Design
{
    public class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("appsettings.development.json");

            var connectionString = builder.Build().GetConnectionString("DefaultConnection");

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var context = new DbContextOptionsBuilder<ApplicationDbContext>()
                  .UseSqlServer(connectionString);

                return new ApplicationDbContext(context.Options);
            }

            throw new InvalidProgramException("Cannot load the default connection string for design time DbContext.");
        }
    }
}
