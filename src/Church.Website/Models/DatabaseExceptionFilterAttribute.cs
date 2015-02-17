namespace Church.Website.Models
{
    using System;
    using System.Web.Mvc;

    public class DatabaseExceptionFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            using (var entity = new FrameworkEntities())
            {
                if (filterContext != null && filterContext.Exception != null)
                {
                    var ipAddress = filterContext.HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                    var username = (filterContext.Controller as Controller).User.Identity.Name;

                    entity.LogExceptionAsync(ipAddress, username, filterContext.Exception);
                }
            }

            base.OnException(filterContext);
        }
    }
}
