using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.Data.Abstractions;
using MeetupApp.Data.Repositories;
using MeetupApp.DataBase.Entities;
using MeetupApp.Business.ServicesImplementations;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.WebAPI.Utils;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Events;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace MeetupApp.WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterLogging(this IHostBuilder host)
        {
            host.UseSerilog((ctx, lc) => lc
              .WriteTo.Console()
              .WriteTo.File(GetPathToLogFile(), LogEventLevel.Information));
        }

        // Returns the path for log file recording.
        private static string GetPathToLogFile()
        {
            var sb = new StringBuilder();
            sb.Append(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            sb.Append(@"\logs\");
            sb.Append($"{DateTime.Now:yyyyMMddhhmmss}");
            sb.Append("data.log");
            return sb.ToString();
        }

        public static void RegisterRepositories(this IServiceCollection collection)
        {
            collection.AddScoped<IRepository<User>, Repository<User>>();
            collection.AddScoped<IRoleRepository, RoleRepository>();
            collection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            collection.AddScoped<IEventRepository, EventRepository>();
            collection.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void RegisterBusinessServices(this IServiceCollection collection)
        {
            collection.AddScoped<IRoleService, RoleService>();
            collection.AddScoped<IUserService, UserService>();
            collection.AddScoped<IRefreshTokenService, RefreshTokenService>();
            collection.AddScoped<IEventService, EventService>();
            collection.AddScoped<IJwtUtil, JwtUtil>();
        }

        public static void RegisterAuth(this IServiceCollection collection, WebApplicationBuilder builder)
        {
            collection.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                // Only for develop environment.
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = builder.Configuration["Token:Issuer"],
                    ValidAudience = builder.Configuration["Token:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:JwtSecret"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void RegisterSwaggerGenerator(this IServiceCollection collection, WebApplicationBuilder builder)
        {
            collection.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(builder.Configuration["XmlDoc"]);
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
        }
    }
}
