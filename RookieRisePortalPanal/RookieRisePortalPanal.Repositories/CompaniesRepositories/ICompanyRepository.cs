using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Repositories.CompaniesRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieRisePortalPanal.Repositories.CompaniesRepository
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(Guid id);
        Task<Company?> GetByUserIdAsync(Guid userId);

        Task<Company?> GetByUserEmailAsync(string email);
        Task UpdateLogoAsync(Guid companyId, string logoName);


        Task AddAsync(Company company);
        Task<int> UpdateAsync(Company entity, Guid userId);

        Task DeleteAsync(Guid id, Guid userId);
        Task RestoreAsync(Guid id);
    }
}
