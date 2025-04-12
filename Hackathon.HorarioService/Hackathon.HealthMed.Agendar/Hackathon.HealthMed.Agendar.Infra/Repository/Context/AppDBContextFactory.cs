using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hackathon.HealthMed.Agendar.Infra.Repository.Context;

public class AppDBContextFactory : IDesignTimeDbContextFactory<AppDBContext>
{
    public AppDBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=HealthMedDB;User Id=sa;Password=SuaSenha123!;TrustServerCertificate=True;MultipleActiveResultSets=true;");

        return new AppDBContext(optionsBuilder.Options);
    }
}
