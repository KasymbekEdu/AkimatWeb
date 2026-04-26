using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AkimatWeb.Controllers;

public class CultureController : Controller
{
    [HttpGet]
    public IActionResult Set(string culture, string returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            returnUrl = "/";

        if (!string.IsNullOrWhiteSpace(culture))
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = true
                });
        }

        return LocalRedirect(returnUrl);
    }
}
