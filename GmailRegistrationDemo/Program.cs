using GmailRegistrationDemo.Data.Data;
using GmailRegistrationDemo.Data.Models;
using GmailRegistrationDemo.Services.Services;
using Microsoft.EntityFrameworkCore;

namespace GmailRegistrationDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Configure the ConnectionString and DbContext class
            builder.Services.AddDbContext<GmailDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs"));
            });

            // Add Session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set appropriate timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register Services
            builder.Services.AddScoped<GenerateEmailSuggestions>();
            builder.Services.AddSingleton<PhoneVerification>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Enable Session

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Registration}/{action=Step1}/{id?}");

            app.Run();
        }
    }
}



//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
