
namespace RookieRisePortalPanal.Data.Entities
{
    public class Company : ITrackableEntity
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string WebsiteUrl { get; set; }
        public string? LogoPath { get; set; }

        
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;




        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }


        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }


        public bool IsDeleted { get; set; } = false; 
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}