
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Data.Entities.Enums;

namespace RookieRisePortalPanal.Data.Context
{
    public class RookieRiseDbContext
        : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
      

        public RookieRiseDbContext(DbContextOptions<RookieRiseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<UserToken> AppUserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================
            // AppUser
            // =========================
            builder.Entity<AppUser>(entity =>
            {
                entity.HasQueryFilter(x => !x.IsDeleted);
            });

            // =========================
            // Company
            // =========================
            builder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.NameEn)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.NameAr)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.WebsiteUrl)
                      .HasMaxLength(500);

                entity.Property(c => c.LogoPath)
                      .HasMaxLength(500);

                // One-to-One (Company ↔ User)
                entity.HasOne(c => c.User)
                      .WithOne(u => u.Company)
                      .HasForeignKey<Company>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(x => !x.IsDeleted);
            });

            // =========================
            // UserToken
            // =========================
            builder.Entity<UserToken>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Token)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(t => t.Type)
                      .IsRequired();

                entity.Property(t => t.ExpirationTime)
                      .IsRequired();

                entity.Property(t => t.IsUsed)
                      .HasDefaultValue(false);

                // Relation with AppUser (Many Tokens → One User)
                entity.HasOne<AppUser>()
                      .WithMany()
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}




