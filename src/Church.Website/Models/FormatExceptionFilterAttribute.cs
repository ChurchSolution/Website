// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatExceptionFilterAttribute.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormatExceptionFilterAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Website.Models
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Filters;

    /// <summary>
    /// Provides the filter for FormatException.
    /// </summary>
    public class FormatExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Handles the exception within the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is FormatException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                                       {
                                           Content = new StringContent(context.Exception.Message)
                                       };
            }
        }
    }
}