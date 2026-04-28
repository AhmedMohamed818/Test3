
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



        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {

            base.OnModelCreating(modelbuilder);
            modelbuilder.Entity<AppUser>(entity =>
            {
                entity.HasQueryFilter(x => !x.IsDeleted);
            });

            modelbuilder.Entity<AppUser>()
            .HasIndex(u => u.NormalizedEmail)
            .HasFilter("[IsDeleted] = 0") 
            .IsUnique();

            modelbuilder.Entity<AppUser>()
                .HasIndex(u => u.NormalizedUserName)
                .HasFilter("[IsDeleted] = 0")
                .IsUnique();


            modelbuilder.Entity<Company>(entity =>
            {
                
                entity.HasOne(c => c.User)
                       .WithOne()
                      .HasForeignKey<Company>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(x => !x.IsDeleted);
            });



            modelbuilder.Entity<UserToken>(entity =>
            {
                
                entity.HasOne<AppUser>(t => t.User)
                      .WithMany()
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            





        }
    }
}




