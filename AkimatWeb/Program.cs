using AkimatWeb.Domain;
using AkimatWeb.Domain.Repositories.Abstract;
using AkimatWeb.Domain.Repositories.EntityFramework;
using AkimatWeb.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace AkimatWeb;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        AppConfig config = builder.Configuration.GetSection("Project").Get<AppConfig>() ?? new AppConfig();

        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? builder.Configuration["DATABASE_URL"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection немесе DATABASE_URL толтырылуы керек.");

        // Database
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        builder.Services.AddTransient<INewsRepository, EFNewsRepository>();
        builder.Services.AddTransient<IAnnouncementsRepository, EFAnnouncementsRepository>();
        builder.Services.AddTransient<IApplicationsRepository, EFApplicationsRepository>();
        builder.Services.AddTransient<IServicesRepository, EFServicesRepository>();
        builder.Services.AddTransient<DataManager>();
        builder.Services.AddTransient<IMapObjectsRepository, EFMapObjectsRepository>();
        builder.Services.AddTransient<IPollsRepository, EFPollsRepository>();
        builder.Services.AddSingleton<CloudinaryService>();

        builder.Services.AddSingleton(config);

        // Identity
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // ✅ Localization сервистерін тіркеу
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

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

        // ✅ Қолдаулы мәдениеттерді анықтау
        var supportedCultures = new[]
        {
            new CultureInfo("kk-KZ"),
            new CultureInfo("ru-RU")
        };

        // ✅ RequestLocalization + CookieRequestCultureProvider дұрыс тіркеу
        // МАҢЫЗДЫ: CookieRequestCultureProvider тізімде бірінші болуы керек,
        // сонда cookie мәні QueryString мен AcceptLanguage-тен басымдықты алады.
        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("kk-KZ"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };

        // CookieRequestCultureProvider провайдерін бірінші орынға қою
        localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());

        app.UseRequestLocalization(localizationOptions);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute("areas", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

        // База тексеру
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                db.Database.Migrate();
                Console.WriteLine(">>>> БАЗА СӘТТІ ҚОСЫЛДЫ ЖӘНЕ ЖАҢАРТЫЛДЫ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>>> БАЗАҒА ҚОСЫЛУ ҚАТЕСІ: {ex.Message}");
            }
        }

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