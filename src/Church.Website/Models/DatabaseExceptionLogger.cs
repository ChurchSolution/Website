//-----------------------------------------------------------------------------
// <copyright file="DatabaseExceptionLogger.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Microsoft.Nebula.ResourceProvider.Models
{
    using Church.Website.Models;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.ExceptionHandling;

    public class DatabaseExceptionLogger : ExceptionLogger
    {
        private static Lazy<FrameworkEntities> entities = new Lazy<FrameworkEntities>(() => new FrameworkEntities());

        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            var ipAddress = Utilities.GetClientIp(context.Request);
            var username = context.RequestContext.Principal.Identity.Name;

            return DatabaseExceptionLogger.entities.Value.LogExceptionAsync(ipAddress, username, context.Exception);
        }
    }
}