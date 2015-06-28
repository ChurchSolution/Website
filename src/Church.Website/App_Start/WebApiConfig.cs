namespace Church.Website
{
    using Microsoft.Nebula.ResourceProvider.Models;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Configure routes
            config.Routes.MapHttpRoute("BibleVersePattern", "bible/{bibleId}/Abbreviations", new { controller = "bible", action = "GetAbbreviationsAsync", });
            config.Routes.MapHttpRoute("BibleVerses", "bible/{bibleId}/Abbreviations/{Abbreviation}", new { controller = "bible", action = "GetAbbreviationAsync", });
            config.Routes.MapHttpRoute(
                "BibleChapter",
                "bible/{bibleId}/book/{order}/chapter/{chapterOrder}",
                new { controller = "bible", action = "GetChapterAsync" });

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            config.Services.Add(typeof(IExceptionLogger), new DatabaseExceptionLogger());
        }
    }
}
