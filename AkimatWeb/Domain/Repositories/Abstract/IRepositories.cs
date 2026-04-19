using AkimatWeb.Domain.Models;

namespace AkimatWeb.Domain.Repositories.Abstract;

public interface INewsRepository
{
    IQueryable<News> GetAll();
    Task<News?> GetByIdAsync(int id);
    Task CreateAsync(News news);
    Task UpdateAsync(News news);
    Task DeleteAsync(int id);
}

public interface IAnnouncementsRepository
{
    IQueryable<Announcement> GetAll();
    Task<Announcement?> GetByIdAsync(int id);
    Task CreateAsync(Announcement announcement);
    Task UpdateAsync(Announcement announcement);
    Task DeleteAsync(int id);
}

public interface IApplicationsRepository
{
    IQueryable<Application> GetAll();
    Task<Application?> GetByIdAsync(int id);
    Task<Application?> GetByTrackingNumberAsync(string trackingNumber);
    Task CreateAsync(Application application);
    Task UpdateStatusAsync(int id, ApplicationStatus status, string? adminComment);
    Task DeleteAsync(int id);
}

public interface IServicesRepository
{
    IQueryable<ServiceCategory> GetAllCategories();
    IQueryable<ServiceItem> GetAllServices();
    Task<ServiceCategory?> GetCategoryByIdAsync(int id);
    Task<ServiceItem?> GetServiceByIdAsync(int id);
    Task CreateCategoryAsync(ServiceCategory category);
    Task CreateServiceAsync(ServiceItem service);
    Task DeleteCategoryAsync(int id);
    Task DeleteServiceAsync(int id);
}
