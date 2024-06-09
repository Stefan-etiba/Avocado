using Domain.Entities;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration Configuration)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddTransient<ISmsRepository, NotificationsRepository>();
        services.AddTransient<IPaymentRepository, PaymentRepository>();
        services.AddTransient<IUserRepository, StudentRepository>();
        services.AddSingleton<DapperContext>();
        services.AddHttpClient();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Integration Demo",
                Description = "Student to Bank Integration Demo APIs"
            });
            
            c.AddSecurityDefinition(
                JwtBearerDefaults.AuthenticationScheme, //Name the security scheme
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http, 
                    Scheme = "bearer" 
                }
            );

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = JwtBearerDefaults.AuthenticationScheme, //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                }
            );
        });

        // For Identity  
        services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.User.RequireUniqueEmail = true; })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });

        services.AddCors(options =>
        {
            options.AddPolicy("EnableCORS", corsPolicyBuilder =>
            {
                corsPolicyBuilder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}