using Microsoft.EntityFrameworkCore;
using RookieRisePortalPanal.Data.Context;
using RookieRisePortalPanal.Data.Entities;

namespace RookieRisePortalPanal.Repositories.UsersRepository
{
    public class UserRepository (RookieRiseDbContext context) : IUserRepository
    {
        

        public async Task<List<AppUser>> GetAllAsync()
        {
            return await context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AppUser?> GetByIdAsync(Guid id)
        {
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.ToUpperInvariant();

            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        }


        public async Task<Dictionary<Guid, string?>> GetEmailsByIdsAsync(List<Guid> ids)
        {
            return await context.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);
        }



    }
}