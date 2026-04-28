
using AutoMapper;
using RookieRisePortalPanal.Data.Context;
using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Repositories.CompaniesRepository;
using RookieRisePortalPanal.Repositories.UsersRepository;
using RookieRisePortalPanal.Services.CompaniesServices.DTO;
using RookieRisePortalPanal.Services.CompanyService.DTO;
using RookieRisePortalPanal.Services.Helper;

namespace RookieRisePortalPanal.Services.CompanyService
{
    public class CompanyService(
            RookieRiseDbContext dbContext,  
    IUserRepository userRepository,  
        ICompanyRepository companyRepository,
        IMapper mapper) : ICompanyService
    {

      
        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var companies = await companyRepository.GetAllAsync();

            
            var userIds = companies.Select(c => c.UserId).ToList();

            var users = await userRepository.GetEmailsByIdsAsync(userIds); 


            var result = mapper.Map<List<CompanyDto>>(companies);

            
            foreach (var dto in result)
            {
                var company = companies.First(c => c.Id == dto.CompanyId);

                dto.UserEmail = users.ContainsKey(company.UserId)
                    ? users[company.UserId]
                    : null;
            }

            return result;
        }

       
        public async Task<CompanyDto> GetByIdAsync(Guid id)
        {
            var company = await companyRepository.GetByIdAsync(id);

            if (company == null)
                throw new Exception("Company not found");

            var dto = mapper.Map<CompanyDto>(company);
            var users = await userRepository.GetEmailsByIdsAsync([company.UserId]);

            dto.UserEmail = users.GetValueOrDefault(company.UserId);


            return dto;
        }

        
        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            var company = mapper.Map<Company>(dto);

            company.Id = Guid.NewGuid();
            company.CreatedAt = DateTime.UtcNow;

            await companyRepository.AddAsync(company);
            await dbContext.SaveChangesAsync();

            return mapper.Map<CompanyDto>(company);
        }
        public async Task<string> UploadLogoAsync(UploadLogoDto dto)
        {
            var company = await companyRepository.GetByIdAsync(dto.CompanyId);

            if (company == null)
                throw new Exception("Company not found");

            var logoName = await DocumentSettings.UploadFileAsync(dto.Logo, "images");

            var entity = new Company { Id = dto.CompanyId, LogoPath = logoName };
            await companyRepository.UpdateLogoAsync(dto.CompanyId, logoName);

            return logoName;
        }

      
        public async Task UpdateAsync(UpdateCompanyDto dto)
        {
            var company = mapper.Map<Company>(dto);

            var affected = await companyRepository.UpdateAsync(company, Guid.Empty);

            if (affected == 0)
                throw new Exception("Company not found");
        }

    
        public async Task DeleteAsync(Guid id)
        {
            await companyRepository.DeleteAsync(id, Guid.Empty);
            await dbContext.SaveChangesAsync();
        }

    
        public async Task RestoreAsync(Guid id)
        {
            await companyRepository.RestoreAsync(id);
            await dbContext.SaveChangesAsync();
        }
    }
}

