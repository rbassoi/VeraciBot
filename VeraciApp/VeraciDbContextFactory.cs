using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VeraciBot.Data;

namespace VeraciApp
{
    public class VeraciDbContextFactory : IDesignTimeDbContextFactory<VeraciDbContext>
    {
        public VeraciDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<VeraciDbContext>()
                .UseSqlite("Data Source=veracibot.db", b => b.MigrationsAssembly("VeraciApp"))
                .Options;
            return new VeraciDbContext(options);
        }
    }
}
