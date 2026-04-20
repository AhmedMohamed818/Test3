using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RookieRisePortalPanal.Data.Context;
using RookieRisePortalPanal.Data.Entities;
using RookieRisePortalPanal.Data.Entities.Enums;
using RookieRisePortalPanal.Repositories.CompaniesRepository;
using RookieRisePortalPanal.Repositories.Exceptions;
using RookieRisePortalPanal.Repositories.TokenRepository;
using RookieRisePortalPanal.Repositories.UsersRepository;
using RookieRisePortalPanal.Services.AccountService.DTO;
using RookieRisePortalPanal.Services.EmailService;
using RookieRisePortalPanal.Services.EmailService.DTO;
using RookieRisePortalPanal.Services.Helper;
using RookieRisePortalPanal.Services.JwtService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RookieRisePortalPanal.Services.AccountService
{
    public class AccountService(RookieRiseDbContext context,
            ICompanyRepository companyRepository,
            UserManager<AppUser> userManager,
            IUserRepository userRepository,
            IUserTokenRepository userTokenRepo,
            IEmailService emailService,
            IConfiguration configuration,
            IJwtService jwtService,
            ILogger<AccountService> logger) : IAccountService
    {


        // 🔐 LOGIN
        public async Task<UserResultDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                logger.LogInformation("Login attempt for {Email}", loginDto.Email);

                var user = await userRepository.GetByEmailWithCompanyAsync(loginDto.Email);
                //  await context.Entry(user).Reference(u => u.Company).LoadAsync();

                if (user == null)
                {
                    logger.LogWarning("User not found {Email}", loginDto.Email);
                    throw new UnAuthorizedException("Invalid email");
                }
                var company = await context.Companies
                      .AsNoTracking()
                      .FirstOrDefaultAsync(c => c.UserId == user.Id);

                var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);

                if (!isPasswordValid)
                {
                    logger.LogWarning("Invalid password for {Email}", loginDto.Email);
                    throw new UnAuthorizedException("Invalid password");
                }

                logger.LogInformation("User logged in successfully {Email}", loginDto.Email);

                Console.WriteLine(user.Id);
                Console.WriteLine(company == null ? "NULL" : "FOUND");

                Console.WriteLine("BEFORE TOKEN");

                var token = await jwtService.GenerateTokenAsync(user);
                Console.WriteLine("AFTER TOKEN");
                return new UserResultDto
                {
                    Email = user.Email,
                    NameEn = company?.NameEn,
                    NameAr = company?.NameAr,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login for {Email}", loginDto.Email);
                throw;
            }
        }

        // 📝 REGISTER
        public async Task<UserResultDto> RegisterAsync([FromBody] RegisterDto dto)
        {
            try
            {
                logger.LogInformation("Register attempt for {Email}", dto.Email);

                var exists = await userManager.FindByEmailAsync(dto.Email);

                if (exists != null)
                {
                    logger.LogWarning("Duplicate email {Email}", dto.Email);
                    throw new DuplicatedEmailBadRequestException(dto.Email);
                }
                if (dto.LogoPath is not null)
                {
                    dto.LogoName = DocumentSettings.UplodeFile(dto.LogoPath, "images");

                }


                // 1. Create User
                var user = new AppUser
                {
                    Email = dto.Email,
                    UserName = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                var result = await userManager.CreateAsync(user);

                if (!result.Succeeded)
                    throw new ValidationException(result.Errors.Select(e => e.Description));


                // 2. Create Company مربوط باليوزر
                var company = new Company
                {
                    NameEn = dto.NameEn,
                    NameAr = dto.NameAr,
                    WebsiteUrl = dto.WebsiteUrl,
                    LogoPath = dto.LogoName,
                    UserId = user.Id
                };

                await companyRepository.AddAsync(company);
                await context.SaveChangesAsync();


                // 3. Token
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var userToken = new UserToken
                {
                    UserId = user.Id,
                    Token = token,
                    Type = TokenType.SetPassword,
                    ExpirationTime = DateTime.UtcNow.AddHours(1),
                    CreatedAt = DateTime.UtcNow
                };

                await context.AddAsync(userToken);
                await context.SaveChangesAsync();


                // 4. Email
                var encodedToken = Uri.EscapeDataString(token);
                var baseUrl = configuration["BaseUrl"];
                var link = $"{baseUrl}/setpassword?email={user.Email}&token={encodedToken}";

                var emailDto = new SendEmailDto
                {
                    To = user.Email,
                    Subject = "Set Password",
                    Body = link
                };

                await emailService.SendEmailAsync(emailDto);


                // 5. Response
                return new UserResultDto
                {
                    Email = user.Email,
                    NameEn = company.NameEn,
                    NameAr = company.NameAr
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during registration for {Email}", dto.Email);
                throw;
            }
        }

        // 🔑 SET PASSWORD
        public async Task<bool> SetPasswordAsync(SetPasswordDto dto)
        {
            try
            {
                logger.LogInformation("Set password attempt for {Email}", dto.Email);

                var user = await userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                    throw new UserNotFoundException(dto.Email);

                var decodedToken = Uri.UnescapeDataString(dto.Token);

                var userToken = await userTokenRepo
                    .GetValidTokenAsync(decodedToken, user.Id, TokenType.SetPassword);

                if (userToken == null)
                    throw new InvalidTokenException("Invalid token");

                var result = await userManager.ResetPasswordAsync(
                    user,
                    decodedToken,
                    dto.NewPassword
                );

                if (!result.Succeeded)
                    throw new ValidationException(result.Errors.Select(e => e.Description));

                userToken.IsUsed = true;
                await context.SaveChangesAsync();

                logger.LogInformation("Password set successfully for {Email}", dto.Email);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while setting password for {Email}", dto.Email);
                throw;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<UserResultDto> GetCurrentUserAsync(string email)
        {
            try
            {
                var user = await userRepository.GetByEmailWithCompanyAsync(email);

                if (user == null)
                    throw new UserNotFoundException(email);

                return new UserResultDto
                {
                    Email = user.Email,
                    Token = await jwtService.GenerateTokenAsync(user)
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting current user {Email}", email);
                throw;
            }
        }
    }
}
