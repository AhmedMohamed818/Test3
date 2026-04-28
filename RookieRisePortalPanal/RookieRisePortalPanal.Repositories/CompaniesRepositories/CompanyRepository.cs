using Microsoft.EntityFrameworkCore;
using RookieRisePortalPanal.Data.Context;
using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Repositories.CompaniesRepository;

public class CompanyRepository(RookieRiseDbContext context) : ICompanyRepository
{
    public async Task<List<Company>> GetAllAsync()
    {
        return await context.Companies
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Company?> GetByIdAsync(Guid id)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<Company?> GetByUserIdAsync(Guid userId)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }
    public async Task<Company?> GetByUserEmailAsync(string email)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) return null;

        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == user.Id);
    }

    public async Task AddAsync(Company company)
    {
        await context.Companies.AddAsync(company);
    }
    public async Task UpdateLogoAsync(Guid companyId, string logoName)
    {
        await context.Companies
            .Where(c => c.Id == companyId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.LogoPath, logoName)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow)
            );
    }
    public async Task<int> UpdateAsync(Company entity, Guid userId)
    {
        return await context.Companies
            .Where(c => c.Id == entity.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.NameEn, entity.NameEn)
                .SetProperty(c => c.NameAr, entity.NameAr)
                .SetProperty(c => c.WebsiteUrl, entity.WebsiteUrl)
                .SetProperty(c => c.LogoPath, entity.LogoPath)
                .SetProperty(c => c.UpdatedBy, userId)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow)
            );
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        await context.Companies
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.IsDeleted, true)
                .SetProperty(c => c.DeletedAt, DateTime.UtcNow)
                .SetProperty(c => c.DeletedBy, userId)
            );
    }

    public async Task RestoreAsync(Guid id)
    {
        await context.Companies
            .IgnoreQueryFilters()
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.IsDeleted, false)
                .SetProperty(c => c.DeletedAt, (DateTime?)null)
                .SetProperty(c => c.DeletedBy, (Guid?)null)
            );
    }
}