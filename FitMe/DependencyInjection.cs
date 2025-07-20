namespace FitMe;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services ,IConfiguration configuration)
    {
        services.AddAuthConfig(configuration);
        return services;
    }
    public static IServiceCollection AddAuthConfig(this IServiceCollection services ,IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        return services;
    }
}
