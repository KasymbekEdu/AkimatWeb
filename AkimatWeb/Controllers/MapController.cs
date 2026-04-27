using AkimatWeb.Domain;
using AkimatWeb.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace AkimatWeb.Controllers;

public class MapController : Controller
{
    private readonly DataManager _data;
    public MapController(DataManager data) => _data = data;

    // Публичный бет
    public IActionResult Index()
    {
        var objects = _data.MapObjects.GetAll()
            .Where(m => m.IsActive)
            .ToList();
        return View(objects);
    }

    // GeoJSON API — Leaflet картасы осыдан оқиды
    [HttpGet("/api/map/objects")]
    public IActionResult GetGeoJson()
    {
        var objects = _data.MapObjects.GetAll()
            .Where(m => m.IsActive)
            .ToList();

        var features = objects.Select(m => new
        {
            type = "Feature",
            geometry = new
            {
                type = "Point",
                coordinates = new[] { m.Longitude, m.Latitude }
            },
            properties = new
            {
                id = m.Id,
                title = m.Title,
                description = m.Description,
                type = m.Type.ToString(),
                status = m.Status.ToString(),
                startDate = m.StartDate.ToString("dd.MM.yyyy"),
                endDate = m.EndDate.HasValue
                    ? m.EndDate.Value.ToString("dd.MM.yyyy")
                    : null
            }
        });

        return Json(new { type = "FeatureCollection", features });
    }
}