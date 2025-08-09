using FitMe.Contracts.Product;
using FitMe.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FitMe.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public DbSet<OTP>OTP { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart>Cart { get; set; }
    public DbSet<CartItems>CartItems { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
       _httpContextAccessor = httpContextAccessor;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        var cascadeFks = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);
        foreach (var fk in cascadeFks)
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        modelBuilder.Entity<OTP>()
            .HasOne(o => o.User)
            .WithMany(u => u.Otps)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(modelBuilder);
      

    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        foreach (var entity in entries)
        {
            var CurrentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()!;
            if (entity.State == EntityState.Added)
            {
                entity.Property(x => x.CreatedById).CurrentValue = CurrentUserId;

            }
            else if (entity.State == EntityState.Modified)
            {
                entity.Property(x => x.UpdatedById).CurrentValue = CurrentUserId;
                entity.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;

            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
