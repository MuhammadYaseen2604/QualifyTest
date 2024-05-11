using CreativeTim.Argon.DotNetCore.Free.Models.DynamicNav;
using CreativeTim.Argon.DotNetCore.Free.Models.Identity;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CreativeTim.Argon.DotNetCore.Free.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<NavigationItem> navigationItems { get; set; }

        // Other DbSet properties as needed

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Customize model configuration
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NavigationItem>()
               .HasMany(n => n.ParentNavigation)
               .WithOne()
               .HasForeignKey(n => n.ParentId)
               .OnDelete(DeleteBehavior.Restrict); // Specify ON DELETE NO ACTION or ON UPDATE NO ACTION here
        }
    }
}
