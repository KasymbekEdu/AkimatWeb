using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AkimatWeb.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    public IActionResult Switch(string culture, string returnUrl = "/")
    {
        var allowed = new[] { "kk-KZ", "ru-RU" };
        if (!allowed.Contains(culture)) culture = "kk-KZ";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false,
                SameSite = SameSiteMode.Lax
            }
        );

        if (!Url.IsLocalUrl(returnUrl)) returnUrl = "/";
        return Redirect(returnUrl);
    }
}