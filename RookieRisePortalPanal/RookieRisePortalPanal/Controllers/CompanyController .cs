using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieRisePortalPanal.Services.CompaniesServices.DTO;
using RookieRisePortalPanal.Services.CompanyService;
using RookieRisePortalPanal.Services.CompanyService.DTO;

namespace RookieRisePortalPanal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController (ICompanyService companyService) : ControllerBase
    {
       
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await companyService.GetAllAsync();

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await companyService.GetByIdAsync(id);

            return Ok(new
            {
                success = true,
                data = result
            });
        }
       
        [Authorize]
        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo([FromForm] UploadLogoDto dto)
        {
            var logoName = await companyService.UploadLogoAsync(dto);
            return Ok(new
            {
                success = true,
                message = "Logo uploaded successfully",
                data = logoName
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
        {
            var result = await companyService.CreateAsync(dto);

            return Ok(new
            {
                success = true,
                message = "Company created successfully",
                data = result
            });
        }

       
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCompanyDto dto)
        {
            await companyService.UpdateAsync(dto);

            return Ok(new
            {
                success = true,
                message = "Company updated successfully"
            });
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await companyService.DeleteAsync(id);

            return Ok(new
            {
                success = true,
                message = "Company deleted successfully"
            });
        }

        
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            await companyService.RestoreAsync(id);

            return Ok(new
            {
                success = true,
                message = "Company restored successfully"
            });
        }
    }
}