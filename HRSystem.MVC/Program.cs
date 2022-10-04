using HRSystem.Models;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.Extensions.FileProviders;
using HRSystem.Repositories;

namespace Extrade.MVC
{

    public class program
    {
        public static int Main()
        {

            var builder = WebApplication.CreateBuilder();

            builder.Services.AddRazorPages();
            

            builder.Services.AddDbContext<HRContext>(i =>
            {
                i.UseLazyLoadingProxies().UseSqlServer
                (builder.Configuration.GetConnectionString("HRSystem"));
            });

            builder.Services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<HRContext>();
            builder.Services.Configure<IdentityOptions>(p =>
            {
                p.Password.RequiredLength = 6;
                p.Password.RequireDigit = false;
                p.Password.RequireUppercase = false;
                p.Password.RequiredUniqueChars = 0;
                p.Password.RequireNonAlphanumeric = false;
                p.Password.RequireLowercase = false;
                p.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                p.Lockout.MaxFailedAccessAttempts = 5;
            });
            builder.Services.AddScoped(typeof(UnitOfWork));
            builder.Services.AddScoped(typeof(HRRepository));
            builder.Services.AddScoped(typeof(EmployeeRepository));
            builder.Services.AddScoped(typeof(AttendanceRepository));
            builder.Services.AddScoped(typeof(OfficialHolidaysRepository));
            builder.Services.AddScoped(typeof(GroupRepository));
            builder.Services.AddScoped(typeof(PaymentRepository));


            builder.Services.Configure<SignInOptions>(p =>
            {
                p.RequireConfirmedEmail = false;
            });
            builder.Services.ConfigureApplicationCookie(c =>
            {
                c.AccessDeniedPath = "/Login";
                c.LoginPath = "/Login";

            });
            builder.Services.AddControllersWithViews();
            
            var app = builder.Build();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapDefaultControllerRoute();
            app.Run();
            return 0;
        }
    }
}