using RookieRisePortalPanal.Data.Entities.Enums;
namespace RookieRisePortalPanal.Data.Entities
{
    public class UserToken
    {
        public Guid Id { get; set; }


        public string Token { get; set; } = null!;

        public TokenType Type { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; }



        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;


    }
}
