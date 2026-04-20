
using AutoMapper;
using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Services.CompanyService.DTO;

namespace RookieRisePortalPanal.Services.MappingProfile
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            // ✅ Company → DTO
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.CompanyId,
                    opt => opt.MapFrom(src => src.Id)) // 🔥 مهم جدًا
                .ForMember(dest => dest.UserEmail,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : null));

            // ✅ Create DTO → Company
            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // بيتعمل في السيرفيس
                .ForMember(dest => dest.LogoPath, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // ✅ Update DTO → Company
            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.CompanyId)) // 🔥 مهم
                .ForMember(dest => dest.LogoPath, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}

