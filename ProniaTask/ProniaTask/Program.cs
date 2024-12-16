using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProniaTask.Abstractions;
using ProniaTask.DAL;
using ProniaTask.Models;
using ProniaTask.Services;
using System.Configuration;
using IMailService = ProniaTask.Abstractions.IMailService;
using MailService = ProniaTask.Services.MailService;

namespace ProniaTask
{
    //slider update
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddIdentity<AppUser,IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequiredLength = 8;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            }).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders();
            builder.Services.AddScoped<LayoutService>();
            builder.Services.AddDbContext<AppDBContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Mssql"));
            });
            builder.Services.AddScoped<IMailService,MailService>();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            var app = builder.Build();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Manage}/{action=Index}"
          );

            app.MapControllerRoute(
                name: "default",
                pattern:"{controller=Home}/{action=Index}/{id?}"
                );
            app.UseStaticFiles();
            app.Run();
        }
    }
}
