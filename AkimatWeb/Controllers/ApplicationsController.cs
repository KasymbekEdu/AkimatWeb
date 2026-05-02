using AkimatWeb.Domain;
using AkimatWeb.Domain.Models;
using AkimatWeb.Models.ViewModels;
using AkimatWeb.Infrastructure; // CloudinaryService үшін қажет
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;

namespace AkimatWeb.Controllers;

public class ApplicationsController : Controller
{
    private readonly DataManager _data;
    private readonly CloudinaryService _cloudinaryService; // Сервисті қостық

    public ApplicationsController(DataManager data, CloudinaryService cloudinaryService)
    {
        _data = data;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    public IActionResult Submit() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ApplicationCreateVM vm, IFormFile? file)
    {
        if (file != null && file.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".mp4" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError("", "Тек JPG, JPEG, PNG немесе MP4 файлдарын жүктеуге болады.");
            }

            // Cloudinary тегін тарифінде видеолар үшін 10MB аз болуы мүмкін, 
            // бірақ сенің шектеуіңді қалдырдым. Қаласаң 50MB-қа дейін көтере аласың.
            if (file.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("", "Файл көлемі 10 MB-тан аспауы керек.");
            }

            if (ModelState.IsValid)
            {
                // ЕСКІ ЛОГИКА (wwwroot-қа сақтау) АЛЫП ТАСТАЛДЫ
                // ЖАҢА ЛОГИКА: Cloudinary-ге жүктеу
                var mediaUrl = await _cloudinaryService.UploadFileAsync(file, "applications");

                if (!string.IsNullOrEmpty(mediaUrl))
                {
                    vm.FilePath = mediaUrl; // Енді базада https://res.cloudinary.com... сілтемесі сақталады
                }
                else
                {
                    ModelState.AddModelError("", "Файлды жүктеу кезінде қате кетті.");
                }
            }
        }

        if (!ModelState.IsValid) return View(vm);

        var application = new Application
        {
            FullName = vm.FullName,
            Phone = vm.Phone,
            Email = vm.Email,
            Category = vm.Category,
            Description = vm.Description,
            FilePath = vm.FilePath,
            UserId = User.Identity?.IsAuthenticated == true
                          ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                          : null
        };

        await _data.Applications.CreateAsync(application);

        return RedirectToAction(nameof(Confirmation),
            new { trackingNumber = application.TrackingNumber });
    }

    public IActionResult Confirmation(string trackingNumber)
    {
        ViewBag.TrackingNumber = trackingNumber;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Track(string? number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return View((Application?)null);

        var app = await _data.Applications.GetByTrackingNumberAsync(number);
        return View(app);
    }
}