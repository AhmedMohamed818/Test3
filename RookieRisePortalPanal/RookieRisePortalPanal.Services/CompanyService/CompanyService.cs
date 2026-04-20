
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RookieRisePortalPanal.Data.Context;
using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Repositories.CompaniesRepository;
using RookieRisePortalPanal.Services.CompanyService.DTO;

namespace RookieRisePortalPanal.Services.CompanyService
{
    public class CompanyService(
        RookieRiseDbContext dbContext,
        ICompanyRepository companyRepository,
        IMapper mapper) : ICompanyService
    {

        // ✅ Get All
        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var companies = await companyRepository.GetAllAsync();

            // ⚠️ نجيب UserEmail يدوي عشان مفيش Include
            var userIds = companies.Select(c => c.UserId).ToList();

            var users = await dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            var result = mapper.Map<List<CompanyDto>>(companies);

            // inject email
            foreach (var dto in result)
            {
                var company = companies.First(c => c.Id == dto.CompanyId);

                dto.UserEmail = users.ContainsKey(company.UserId)
                    ? users[company.UserId]
                    : null;
            }

            return result;
        }

        // ✅ Get By Id
        public async Task<CompanyDto> GetByIdAsync(Guid id)
        {
            var company = await companyRepository.GetByIdAsync(id);

            if (company == null)
                throw new Exception("Company not found");

            var dto = mapper.Map<CompanyDto>(company);

            // ⚠️ UserEmail
            dto.UserEmail = await dbContext.Users
                .Where(u => u.Id == company.UserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            return dto;
        }

        // ✅ Create
        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            var company = mapper.Map<Company>(dto);

            company.Id = Guid.NewGuid();
            company.CreatedAt = DateTime.UtcNow;

            await companyRepository.AddAsync(company);
            await dbContext.SaveChangesAsync();

            return mapper.Map<CompanyDto>(company);
        }

        // ✅ Update
        public async Task UpdateAsync(UpdateCompanyDto dto)
        {
            var company = mapper.Map<Company>(dto);

            var affected = await companyRepository.UpdateAsync(company, Guid.Empty);

            if (affected == 0)
                throw new Exception("Company not found");
        }

        // ✅ Delete (Soft Delete)
        public async Task DeleteAsync(Guid id)
        {
            await companyRepository.DeleteAsync(id, Guid.Empty);
            await dbContext.SaveChangesAsync();
        }

        // ✅ Restore
        public async Task RestoreAsync(Guid id)
        {
            await companyRepository.RestoreAsync(id);
            await dbContext.SaveChangesAsync();
        }
    }
}

