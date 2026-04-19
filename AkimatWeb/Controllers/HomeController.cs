using AkimatWeb.Domain;
using AkimatWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AkimatWeb.Controllers;

public class HomeController : Controller
{
    private readonly DataManager _data;

    public HomeController(DataManager data) => _data = data;

    public IActionResult Index()
    {
        var vm = new HomeIndexVM
        {
            LatestNews = _data.News.GetAll()
                .Where(n => n.IsPublished)
                .Take(6)
                .ToList(),
            LatestAnnouncements = _data.Announcements.GetAll()
                .Where(a => a.IsActive)
                .Take(4)
                .ToList(),
            ServiceCategories = _data.Services.GetAllCategories()
                .Take(6)
                .ToList()
        };
        return View(vm);
    }

    public IActionResult About() => View();

    public IActionResult Contacts() => View();

    public IActionResult Error() => View();
}
