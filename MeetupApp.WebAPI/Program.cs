using MeetupApp.DataBase;
using Microsoft.EntityFrameworkCore;
using MeetupApp.WebAPI.Extensions;

namespace MeetupApp.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.RegisterLogging();

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<MeetupAppDbContext>(
                optionBuilder => optionBuilder.UseSqlServer(connectionString));

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.RegisterBusinessServices();
            builder.Services.RegisterRepositories();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.RegisterSwaggerGenerator(builder);

            builder.Services.RegisterAuth(builder);

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();

            // Using custom middleware to catch errors globally
            app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}