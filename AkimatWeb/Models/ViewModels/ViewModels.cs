using System.ComponentModel.DataAnnotations;
using AkimatWeb.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace AkimatWeb.Models.ViewModels;

public class ApplicationCreateVM
{
    [Required(ErrorMessage = "Аты-жөніңізді енгізіңіз")]
    [MaxLength(150)]
    [Display(Name = "Аты-жөні")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон нөміріңізді енгізіңіз")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email форматы дұрыс емес")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Санатты таңдаңыз")]
    [Display(Name = "Санат")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Өтінішіңізді жазыңыз")]
    [MinLength(20, ErrorMessage = "Кем дегенде 20 символ жазыңыз")]
    [Display(Name = "Өтінішіңіз")]
    public string Description { get; set; } = string.Empty;
    public string? FilePath { get; set; }
}

public class LoginVM
{
    [Required(ErrorMessage = "Email міндетті")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Құпия сөз міндетті")]
    [DataType(DataType.Password)]
    [Display(Name = "Құпия сөз")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Мені есте сақта")]
    public bool RememberMe { get; set; }
}

public class HomeIndexVM
{
    public IList<News> LatestNews { get; set; } = new List<News>();
    public IList<Announcement> LatestAnnouncements { get; set; } = new List<Announcement>();
    public IList<ServiceCategory> ServiceCategories { get; set; } = new List<ServiceCategory>();
}

public class AdminDashboardVM
{
    public int PendingApplications { get; set; }
    public int TotalApplications { get; set; }
    public int TotalNews { get; set; }
    public int TotalAnnouncements { get; set; }
}

public class NewsCreateVM
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsPublished { get; set; }

    public List<IFormFile>? Images { get; set; }
}