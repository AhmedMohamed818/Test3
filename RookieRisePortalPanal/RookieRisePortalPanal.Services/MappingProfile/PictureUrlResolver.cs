using AutoMapper;
using Microsoft.Extensions.Configuration;
using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Services.AccountService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieRisePortalPanal.Services.MappingProfile
{
    public class PictureUrlResolver(IConfiguration configuration) : IValueResolver<Company, RegisterDto, string>
    {
        public string Resolve(Company source, RegisterDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.LogoPath))
            {
                return $"{configuration["BaseUrl"]}/{source.LogoPath}";
            }
            return string.Empty;
        }


    }
}
