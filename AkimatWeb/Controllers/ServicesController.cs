using AkimatWeb.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AkimatWeb.Controllers;

public class ServicesController : Controller
{
    private readonly DataManager _data;

    public ServicesController(DataManager data) => _data = data;

    public IActionResult Index()
    {
        var categories = _data.Services.GetAllCategories().ToList();
        return View(categories);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _data.Services.GetCategoryByIdAsync(id);
        if (category is null) return NotFound();
        return View(category);
    }
}
