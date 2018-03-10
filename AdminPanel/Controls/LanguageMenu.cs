using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic;
using System.Linq;

namespace Buran.Core.MvcLibrary.AdminPanel.Controls
{
    public static class EditPageExtenders
    {
        public class LanguageItem
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Flag { get; set; }
        }

        public static HtmlString LanguageBar(this IHtmlHelper helper, List<LanguageItem> languageList, string languageSetUrl, string defaultLanguageCode = "en")
        {
            var langCode = defaultLanguageCode;

            var cultureCookie = helper.ViewContext.HttpContext.Request.Cookies
                .FirstOrDefault(d => d.Key == CookieRequestCultureProvider.DefaultCookieName);
            if (cultureCookie.Value != null)
            {
                var cookieS1 = cultureCookie.Value.Split('|');
                var cookieS2 = cookieS1[0].Split('=');
                langCode = cookieS2[1];
            }

            var currentLang = languageList.First(d => d.Code == langCode);
            var otherLang = languageList.First(d => d.Code != langCode);
            var urlHelper = new UrlHelper(helper.ViewContext);
            return new HtmlString(string.Format(@"<li class='dropdown dropdown-language'>
                    <a href='#' class='dropdown-toggle' data-toggle='dropdown' data-hover='dropdown' data-close-others='true'>
                        <img src='{0}'>
                        <span class='langname'>
                            {1}
                        </span>
                        <i class='fa fa-angle-down'></i>
                    </a>
                    <ul class='dropdown-menu'>
                        <li>
                            <a href='{4}'>
                                <img alt='' src='{2}'> {3}
                            </a>
                        </li>
                    </ul>
                </li>",
                urlHelper.Content(currentLang.Flag),
                currentLang.Name,
                urlHelper.Content(otherLang.Flag),
                otherLang.Name,
                urlHelper.Content(languageSetUrl + otherLang.Code)
                )
            );
        }
    }
}
