using AkimatWeb.Domain;
using AkimatWeb.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace AkimatWeb.Controllers;

public class AnnouncementsController : Controller
{
    private readonly DataManager _data;

    public AnnouncementsController(DataManager data) => _data = data;

    public IActionResult Index(AnnouncementType? type)
    {
        var query = _data.Announcements.GetAll().Where(a => a.IsActive);
        if (type.HasValue) query = query.Where(a => a.Type == type.Value);
        ViewBag.CurrentType = type;
        return View(query.ToList());
    }

    public async Task<IActionResult> Details(int id)
    {
        var ann = await _data.Announcements.GetByIdAsync(id);
        if (ann is null || !ann.IsActive) return NotFound();
        return View(ann);
    }
}
