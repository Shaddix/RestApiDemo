using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace RestApiDemo.Persistence
{
    /// <summary>
    /// This class is to allow running powershell EF commands from the project folder without
    /// specifying Startup class (without triggering the whole startup during EF operations
    /// like add/remove migrations).
    /// </summary>
    public class
        PostgresDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();

            // For `dotnet ef migrations remove` to work, we need a real connection string here.
            optionsBuilder.UseNpgsql(
                "Server=localhost;Database=restapidemo;Port=5432;Username=postgres;"
                + "Password=password;");

            return new MainDbContext(optionsBuilder.Options);
        }
    }
}