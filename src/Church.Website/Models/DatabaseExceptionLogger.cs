//-----------------------------------------------------------------------------
// <copyright file="DatabaseExceptionLogger.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Microsoft.Nebula.ResourceProvider.Models
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.ExceptionHandling;

    using Church.Models.EntityFramework;
    using Church.Website.Models;

    public class DatabaseExceptionLogger : ExceptionLogger
    {
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            using (var entity = new FrameworkEntities())
            {
                var ipAddress = Utilities.GetClientIp(context.Request);
                var username = context.RequestContext.Principal.Identity.Name;

                return entity.LogExceptionAsync(ipAddress, username, context.Exception);
            }
        }
    }
}