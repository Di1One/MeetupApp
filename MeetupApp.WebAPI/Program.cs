using MeetupApp.Business.ServicesImplementations;
using MeetupApp.Core.ServiceAbstractions;
using MeetupApp.Data.Abstractions.Repositories;
using MeetupApp.Data.Abstractions;
using MeetupApp.DataBase;
using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
using MeetupApp.Data.Repositories;
using MeetupApp.WebAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MeetupApp.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) => lc
               .WriteTo.Console()
               .WriteTo.File(GetPathToLogFile(),
                   LogEventLevel.Information));

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<MeetupAppDbContext>(
                optionBuilder => optionBuilder.UseSqlServer(connectionString));

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add business services
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IJwtUtil, JwtUtil>();

            // Add repositories
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<RefreshToken>, Repository<RefreshToken>>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(builder.Configuration["XmlDoc"]);
            });

            builder.Services
                .AddAuthentication(options =>
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

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
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
    }
}