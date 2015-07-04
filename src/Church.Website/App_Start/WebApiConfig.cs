// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the WebApiConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Website
{
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;
    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;

    using Church.Models;

    using Microsoft.Nebula.ResourceProvider.Models;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Enable OData URLs
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Sermon>("Sermons");
            builder.EntitySet<Material>("Materials");
            config.MapODataServiceRoute("ODataRoute", "OData", builder.GetEdmModel());

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
