using AutoMapper;
using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Services.AccountService.DTO;
using RookieRisePortalPanal.Services.CompanyService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieRisePortalPanal.Services.CompaniesServices.DTO
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {

            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.CompanyId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserEmail,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : null));



            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LogoPath, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());


            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.LogoPath, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
