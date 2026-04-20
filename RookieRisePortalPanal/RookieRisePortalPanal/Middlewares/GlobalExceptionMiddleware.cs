using Microsoft.AspNetCore.Diagnostics;
using RookieRisePortalPanal.Repositories.Exceptions;
using System.Globalization;

namespace RookieRisePortalPanal.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Unhandled Exception");

                context.Response.Clear();
                context.Response.ContentType = "application/json";

                var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                string message = culture == "ar"
                    ? error switch
                    {
                        UnAuthorizedException => "البريد الإلكتروني أو كلمة المرور غير صحيحة",
                        ValidationException => "بيانات غير صحيحة",
                        DuplicatedEmailBadRequestException => "هذا البريد مستخدم بالفعل",
                        UserNotFoundException => "المستخدم غير موجود",
                        _ => "خطأ في السيرفر"
                    }
                    : error switch
                    {
                        UnAuthorizedException => "Invalid email or password",
                        ValidationException => "Validation error",
                        DuplicatedEmailBadRequestException => "Email already exists",
                        UserNotFoundException => "User not found",
                        _ => "Server error"
                    };

                context.Response.StatusCode = error switch
                {
                    UnAuthorizedException => 401,
                    ValidationException => 400,
                    DuplicatedEmailBadRequestException => 400,
                    UserNotFoundException => 404,
                    _ => 500
                };

                await context.Response.WriteAsJsonAsync(new { message });
            }
        }
    }
}

