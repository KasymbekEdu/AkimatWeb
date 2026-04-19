using AkimatWeb.Domain.Models;
using AkimatWeb.Domain.Repositories.Abstract;
using AkimatWeb.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AkimatWeb.Domain.Repositories.EntityFramework;

public class EFNewsRepository : INewsRepository
{
    private readonly AppDbContext _db;
    public EFNewsRepository(AppDbContext db) => _db = db;

    public IQueryable<News> GetAll() =>
    _db.News
        .Include(n => n.Images)
        .OrderByDescending(n => n.PublishedAt);

    public async Task<News?> GetByIdAsync(int id) =>
        await _db.News
            .Include(n => n.Images)
            .FirstOrDefaultAsync(n => n.Id == id);

    public async Task CreateAsync(News news)
    {
        await _db.News.AddAsync(news);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(News news)
    {
        _db.News.Update(news);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.News.FindAsync(id);
        if (entity != null) { _db.News.Remove(entity); await _db.SaveChangesAsync(); }
    }
}

public class EFAnnouncementsRepository : IAnnouncementsRepository
{
    private readonly AppDbContext _db;
    public EFAnnouncementsRepository(AppDbContext db) => _db = db;

    public IQueryable<Announcement> GetAll() =>
        _db.Announcements.OrderByDescending(a => a.PublishedAt);

    public async Task<Announcement?> GetByIdAsync(int id) =>
        await _db.Announcements.FindAsync(id);

    public async Task CreateAsync(Announcement announcement)
    {
        await _db.Announcements.AddAsync(announcement);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Announcement announcement)
    {
        _db.Announcements.Update(announcement);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Announcements.FindAsync(id);
        if (entity != null) { _db.Announcements.Remove(entity); await _db.SaveChangesAsync(); }
    }
}

public class EFApplicationsRepository : IApplicationsRepository
{
    private readonly AppDbContext _db;
    public EFApplicationsRepository(AppDbContext db) => _db = db;

    public IQueryable<Application> GetAll() =>
        _db.Applications.OrderByDescending(a => a.SubmittedAt);

    public async Task<Application?> GetByIdAsync(int id) =>
        await _db.Applications.FindAsync(id);

    public async Task<Application?> GetByTrackingNumberAsync(string trackingNumber) =>
        await _db.Applications.FirstOrDefaultAsync(a => a.TrackingNumber == trackingNumber.ToUpper());

    public async Task CreateAsync(Application application)
    {
        await _db.Applications.AddAsync(application);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int id, ApplicationStatus status, string? adminComment)
    {
        var app = await _db.Applications.FindAsync(id);
        if (app is null) return;
        app.Status = status;
        app.AdminComment = adminComment;
        app.ReviewedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var app = await _db.Applications.FindAsync(id);
        if (app is null) return;

        if (!string.IsNullOrEmpty(app.FilePath))
        {
            var relativePath = app.FilePath.TrimStart('/');
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }

        _db.Applications.Remove(app);
        await _db.SaveChangesAsync();
    }
}

public class EFServicesRepository : IServicesRepository
{
    private readonly AppDbContext _db;
    public EFServicesRepository(AppDbContext db) => _db = db;

    public IQueryable<ServiceCategory> GetAllCategories() =>
        _db.ServiceCategories.Include(c => c.Services).OrderBy(c => c.SortOrder);

    public IQueryable<ServiceItem> GetAllServices() =>
        _db.ServiceItems.Include(s => s.Category).OrderBy(s => s.SortOrder);

    public async Task<ServiceCategory?> GetCategoryByIdAsync(int id) =>
        await _db.ServiceCategories.Include(c => c.Services).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<ServiceItem?> GetServiceByIdAsync(int id) =>
        await _db.ServiceItems.FindAsync(id);

    public async Task CreateCategoryAsync(ServiceCategory category)
    {
        await _db.ServiceCategories.AddAsync(category);
        await _db.SaveChangesAsync();
    }

    public async Task CreateServiceAsync(ServiceItem service)
    {
        await _db.ServiceItems.AddAsync(service);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var entity = await _db.ServiceCategories.FindAsync(id);
        if (entity != null) { _db.ServiceCategories.Remove(entity); await _db.SaveChangesAsync(); }
    }

    public async Task DeleteServiceAsync(int id)
    {
        var entity = await _db.ServiceItems.FindAsync(id);
        if (entity != null) { _db.ServiceItems.Remove(entity); await _db.SaveChangesAsync(); }
    }
}
