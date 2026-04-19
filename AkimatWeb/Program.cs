using AkimatWeb.Domain;
using AkimatWeb.Domain.Repositories.Abstract;
using AkimatWeb.Domain.Repositories.EntityFramework;
using AkimatWeb.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AkimatWeb;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        AppConfig config = builder.Configuration.GetSection("Project").Get<AppConfig>() ?? new AppConfig();

        if (string.IsNullOrWhiteSpace(config.Database.ConnectionString))
            throw new InvalidOperationException("Project:Database:ConnectionString appsettings.json ішінде толтырылуы керек.");

        // Database
        builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlServer(config.Database.ConnectionString));

        // Repositories
        builder.Services.AddTransient<INewsRepository, EFNewsRepository>();
        builder.Services.AddTransient<IAnnouncementsRepository, EFAnnouncementsRepository>();
        builder.Services.AddTransient<IApplicationsRepository, EFApplicationsRepository>();
        builder.Services.AddTransient<IServicesRepository, EFServicesRepository>();
        builder.Services.AddTransient<DataManager>();

        // Config singleton for views
        builder.Services.AddSingleton(config);

        // Identity
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "AkimatAuth";
            options.Cookie.HttpOnly = true;
            options.LoginPath = "/account/login";
            options.AccessDeniedPath = "/admin/accessdenied";
            options.SlidingExpiration = true;
        });

        builder.Services.AddControllersWithViews();

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        WebApplication app = builder.Build();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
        {
            app.UseExceptionHandler("/home/error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute("areas", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

        // Seed roles and admin user
        await SeedDataAsync(app);

        await app.RunAsync("http://0.0.0.0:8080");
    }

    private static async Task SeedDataAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));

        const string adminEmail = "admin@akimat.kz";
        const string adminPassword = "Admin123!";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
