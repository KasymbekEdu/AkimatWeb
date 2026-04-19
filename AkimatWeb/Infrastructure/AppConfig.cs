namespace AkimatWeb.Infrastructure;

public class AppConfig
{
    public DatabaseConfig Database { get; set; } = new();
    public TinyMCEConfig TinyMCE { get; set; } = new();
    public CompanyConfig Company { get; set; } = new();
}

public class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
}

public class TinyMCEConfig
{
    public string ApiKey { get; set; } = string.Empty;
}

public class CompanyConfig
{
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyPhone { get; set; } = string.Empty;
    public string CompanyPhoneShort { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
}
