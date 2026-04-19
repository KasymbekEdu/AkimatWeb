using AkimatWeb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AkimatWeb.Controllers;

public class NewsController : Controller
{
    private readonly DataManager _data;

    public NewsController(DataManager data) => _data = data;

    public IActionResult Index(int page = 1)
    {
        const int pageSize = 9;
        var allNews = _data.News.GetAll().Where(n => n.IsPublished).ToList();
        ViewBag.TotalPages = (int)Math.Ceiling(allNews.Count / (double)pageSize);
        ViewBag.CurrentPage = page;
        var news = allNews.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return View(news);
    }

    public async Task<IActionResult> Details(int id)
    {
        var news = await _data.News.GetByIdAsync(id);
        if (news is null || !news.IsPublished) return NotFound();
        return View(news);
    }
}
