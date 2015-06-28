namespace Church.Website.Models
{
    using System.Globalization;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    public class CultureFilterAttribute : ActionFilterAttribute
    {
        private const string CultureKey = "culture";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var culture = filterContext.HttpContext.Request.Cookies[CultureKey];
            if (culture == null)
            {
                culture = new HttpCookie(CultureKey, Utilities.Configuration.Languages.Default.ID);
                filterContext.HttpContext.Response.AppendCookie(culture);
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture.Value);

            base.OnActionExecuting(filterContext);
        }
    }
}