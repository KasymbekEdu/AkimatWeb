using AkimatWeb.Domain.Repositories.Abstract;

namespace AkimatWeb.Domain;

public class DataManager
{
    public INewsRepository News { get; }
    public IAnnouncementsRepository Announcements { get; }
    public IApplicationsRepository Applications { get; }
    public IServicesRepository Services { get; }

    public DataManager(
        INewsRepository news,
        IAnnouncementsRepository announcements,
        IApplicationsRepository applications,
        IServicesRepository services)
    {
        News = news;
        Announcements = announcements;
        Applications = applications;
        Services = services;
    }
}
