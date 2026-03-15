// Copyright (C) TBC Bank. All Rights Reserved.

using System.Text;
using Discounts.Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Discounts.API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JwtSettings__SecretKey")
                ?? configuration["JwtSettings:SecretKey"]
                ?? "TBCDiscountsSecretKey12345678901234567890!");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"] ?? "TBC.WebApi",
                    ValidAudience = configuration["JwtSettings:Audience"] ?? "TBC.WebApi",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin));
                options.AddPolicy("MerchantOnly", policy => policy.RequireRole(Roles.Merchant));
                options.AddPolicy("CustomerOnly", policy => policy.RequireRole(Roles.Customer));
            });

            return services;
        }
    }
}
