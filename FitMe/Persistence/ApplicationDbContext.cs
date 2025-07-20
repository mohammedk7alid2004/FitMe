
namespace FitMe.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)  : IdentityDbContext<ApplicationUser>(options)
{

}

