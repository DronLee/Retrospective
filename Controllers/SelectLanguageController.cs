using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Retrospective.Controllers
{
    public class SelectLanguageController : Controller
    {
        private readonly RequestLocalizationOptions _requestLocalizationOptions;
        
        public SelectLanguageController(IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            _requestLocalizationOptions = requestLocalizationOptions.Value;
        }

        public IActionResult Index(string returnUrl = null)
        {
            // Бессмыслено после попытки создания или входа в тему опять отправлять по этому пути после выбора языка.
            // Поэтому будет отправка на Welcome. 
            returnUrl = Regex.Replace(returnUrl, "(?<=Home/)(Create)|(Entry)", "Welcome");
            ViewData["ReturnUrl"] = returnUrl;
            return View(_requestLocalizationOptions.SupportedUICultures);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string cultureName, string returnUrl = null)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName)),
            new CookieOptions {Expires = DateTimeOffset.UtcNow.AddYears(1)});

            if(returnUrl != null)
                return LocalRedirect(returnUrl);
            else
                return RedirectToAction("Welcome", "Subject");
        }
    }
}