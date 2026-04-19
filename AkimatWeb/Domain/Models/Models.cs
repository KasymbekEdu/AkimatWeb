using System.ComponentModel.DataAnnotations;

namespace AkimatWeb.Domain.Models;

public class News
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? ImagePath { get; set; }

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    public bool IsPublished { get; set; } = true;
    public List<NewsImage> Images { get; set; } = new();
}

public class Announcement
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public AnnouncementType Type { get; set; } = AnnouncementType.General;

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;
}

public enum AnnouncementType
{
    General,
    Tender,
    RoadClosure,
    Emergency
}

public class Application
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    public string? AdminComment { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public string TrackingNumber { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();

    public string? UserId { get; set; }
    public string? FilePath { get; set; }

}

public enum ApplicationStatus
{
    Pending,
    InReview,
    Approved,
    Rejected
}

public class ServiceCategory
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public ICollection<ServiceItem> Services { get; set; } = new List<ServiceItem>();
}

public class ServiceItem
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ExternalUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public int CategoryId { get; set; }
    public ServiceCategory? Category { get; set; }
}

public class NewsImage
{
    public int Id { get; set; }
    public string ImagePath { get; set; } = null!;

    public int NewsId { get; set; }
    public News News { get; set; } = null!;
    public List<NewsImage> Images { get; set; } = new();
}