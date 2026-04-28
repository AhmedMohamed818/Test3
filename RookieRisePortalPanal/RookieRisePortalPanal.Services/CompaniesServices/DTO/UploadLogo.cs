using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieRisePortalPanal.Services.CompaniesServices.DTO
{
    public class UploadLogoDto
    {
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public IFormFile Logo { get; set; }
    }
}
