using Microsoft.AspNetCore.Identity;
using RookieRisePortalPanal.Data.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieRisePortalPanal.Data.Entities
{
    public class AppUser : IdentityUser<Guid>, ITrackableEntity
    {

        public Company? Company { get; set; }
        // tracking imp
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }


        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }


        public bool IsDeleted { get; set; } = false; // soft delete
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
