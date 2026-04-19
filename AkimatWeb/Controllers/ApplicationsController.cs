using AkimatWeb.Domain;
using AkimatWeb.Domain.Models;
using AkimatWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AkimatWeb.Controllers;

public class ApplicationsController : Controller
{
    private readonly DataManager _data;

    public ApplicationsController(DataManager data) => _data = data;

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

            if (file.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("", "Файл көлемі 10 MB-тан аспауы керек.");
            }

            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + ext;
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                vm.FilePath = "/uploads/" + fileName;
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
