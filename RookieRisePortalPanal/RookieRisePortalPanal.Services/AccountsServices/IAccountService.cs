using RookieRisePortalPanal.Services.AccountService.DTO;


namespace RookieRisePortalPanal.Services.AccountService
{
    public interface IAccountService
    {
        Task<UserResultDto> LoginAsync(LoginDto loginDto);
        Task<UserResultDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> SetPasswordAsync(SetPasswordDto setPasswordDto);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<UserResultDto> GetCurrentUserAsync(string email);
    }
}
