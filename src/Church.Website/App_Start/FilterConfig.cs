namespace Church.Website
{
    using Church.Website.Models;
    using System.Web.Mvc;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new DatabaseExceptionFilterAttribute());
        }
    }
}
