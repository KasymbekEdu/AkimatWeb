using AkimatWeb.Domain.Repositories.Abstract;

namespace AkimatWeb.Domain;

public class DataManager
{
    public INewsRepository News { get; }
    public IAnnouncementsRepository Announcements { get; }
    public IApplicationsRepository Applications { get; }
    public IServicesRepository Services { get; }
    public IMapObjectsRepository MapObjects { get; }
    public IPollsRepository Polls { get; }

    public DataManager(
        INewsRepository news,
        IAnnouncementsRepository announcements,
        IApplicationsRepository applications,
        IServicesRepository services,
        IMapObjectsRepository mapObjects,
        IPollsRepository polls)
    {
        News = news;
        Announcements = announcements;
        Applications = applications;
        Services = services;
        MapObjects = mapObjects;
        Polls = polls;
    }
}