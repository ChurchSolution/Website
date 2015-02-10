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
            config.Routes.MapHttpRoute("BibleVersePattern", "api/bible/versePattern", new { controller = "bible", action = "GetVersePatternAsync", });
            config.Routes.MapHttpRoute("Bible", "api/bible/{book}/{chapter}", new { controller = "bible", action = "GetBibleAsync" });
            config.Routes.MapHttpRoute("BibleVerses", "api/bible/{Abbreviation}", new { controller = "bible", action = "GetVersesAsync", });

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            config.Services.Add(typeof(IExceptionLogger), new DatabaseExceptionLogger());
        }
    }
}
