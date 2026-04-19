using AkimatWeb.Domain;
using AkimatWeb.Domain.Models;
using AkimatWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AkimatWeb.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly DataManager _data;
    private readonly IWebHostEnvironment _env;

    public AdminController(DataManager data, IWebHostEnvironment env)
    {
        _data = data;
        _env = env;
    }

    // ─── Dashboard ───────────────────────────────────────────────────────────

    public IActionResult Index()
    {
        var vm = new AdminDashboardVM
        {
            PendingApplications = _data.Applications.GetAll()
                .Count(a => a.Status == ApplicationStatus.Pending),
            TotalApplications = _data.Applications.GetAll().Count(),
            TotalNews         = _data.News.GetAll().Count(),
            TotalAnnouncements = _data.Announcements.GetAll().Count()
        };
        return View(vm);
    }

    public IActionResult AccessDenied() => View();

    // ─── Applications ────────────────────────────────────────────────────────

    public IActionResult Applications(ApplicationStatus? status)
    {
        var query = _data.Applications.GetAll();
        if (status.HasValue) query = query.Where(a => a.Status == status.Value);
        ViewBag.CurrentStatus = status;
        return View(query.ToList());
    }

    [HttpPost]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        await _data.Applications.DeleteAsync(id);
        return RedirectToAction(nameof(Applications));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateApplicationStatus(
        int id, ApplicationStatus status, string? comment)
    {
        await _data.Applications.UpdateStatusAsync(id, status, comment);
        return RedirectToAction(nameof(Applications));
    }

    // ─── News ────────────────────────────────────────────────────────────────

    public IActionResult News() =>
        View(_data.News.GetAll().ToList());

    [HttpGet]
    public IActionResult CreateNews() => View(new News());

    [HttpPost]
    public async Task<IActionResult> CreateNews(NewsCreateVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var news = new News
        {
            Title = vm.Title,
            Content = vm.Content,
            IsPublished = vm.IsPublished,
            PublishedAt = DateTime.UtcNow
        };

        if (vm.Images != null && vm.Images.Any())
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "news");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in vm.Images)
            {
                if (file == null || file.Length == 0)
                    continue;

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                    continue;

                var fileName = Guid.NewGuid().ToString() + ext;
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                news.Images.Add(new NewsImage
                {
                    ImagePath = "/uploads/news/" + fileName
                });
            }
        }

        await _data.News.CreateAsync(news);

        return RedirectToAction(nameof(News));
    }

    [HttpGet]
    public async Task<IActionResult> EditNews(int id)
    {
        var news = await _data.News.GetByIdAsync(id);
        if (news is null) return NotFound();
        return View(news);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditNews(News news, IFormFile? image)
    {
        ModelState.Remove(nameof(news.ImagePath));
        if (!ModelState.IsValid) return View(news);

        if (image != null && image.Length > 0)
            news.ImagePath = await SaveImageAsync(image, "news");

        await _data.News.UpdateAsync(news);
        return RedirectToAction(nameof(News));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteNews(int id)
    {
        await _data.News.DeleteAsync(id);
        return RedirectToAction(nameof(News));
    }

    // ─── Announcements ───────────────────────────────────────────────────────

    public IActionResult Announcements() =>
        View(_data.Announcements.GetAll().ToList());

    [HttpGet]
    public IActionResult CreateAnnouncement() => View(new Announcement());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAnnouncement(Announcement announcement)
    {
        if (!ModelState.IsValid) return View(announcement);
        announcement.PublishedAt = DateTime.UtcNow;
        await _data.Announcements.CreateAsync(announcement);
        return RedirectToAction(nameof(Announcements));
    }

    [HttpGet]
    public async Task<IActionResult> EditAnnouncement(int id)
    {
        var ann = await _data.Announcements.GetByIdAsync(id);
        if (ann is null) return NotFound();
        return View(ann);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAnnouncement(Announcement announcement)
    {
        if (!ModelState.IsValid) return View(announcement);
        await _data.Announcements.UpdateAsync(announcement);
        return RedirectToAction(nameof(Announcements));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAnnouncement(int id)
    {
        await _data.Announcements.DeleteAsync(id);
        return RedirectToAction(nameof(Announcements));
    }

    // ─── Services ────────────────────────────────────────────────────────────

    public IActionResult Services() =>
        View(_data.Services.GetAllCategories().ToList());

    [HttpGet]
    public IActionResult CreateServiceCategory() => View(new ServiceCategory());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateServiceCategory(ServiceCategory category)
    {
        if (!ModelState.IsValid) return View(category);
        await _data.Services.CreateCategoryAsync(category);
        return RedirectToAction(nameof(Services));
    }

    [HttpGet]
    public IActionResult CreateService()
    {
        ViewBag.Categories = _data.Services.GetAllCategories().ToList();
        return View(new ServiceItem());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateService(ServiceItem service)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _data.Services.GetAllCategories().ToList();
            return View(service);
        }
        await _data.Services.CreateServiceAsync(service);
        return RedirectToAction(nameof(Services));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteServiceCategory(int id)
    {
        await _data.Services.DeleteCategoryAsync(id);
        return RedirectToAction(nameof(Services));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteService(int id)
    {
        await _data.Services.DeleteServiceAsync(id);
        return RedirectToAction(nameof(Services));
    }

    // ─── Helper ──────────────────────────────────────────────────────────────

    private async Task<string> SaveImageAsync(IFormFile image, string folder)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext)) return string.Empty;

        var dir = Path.Combine(_env.WebRootPath, "img", folder);
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(dir, fileName);

        await using var stream = System.IO.File.Create(path);
        await image.CopyToAsync(stream);

        return $"/img/{folder}/{fileName}";
    }
}
