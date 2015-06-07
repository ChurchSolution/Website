//-----------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church.Website.Models
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;

    using Church.Models;

    public static class SiteConstants
    {
        public const string MemberPageUrl = "/Member";
        public const string DefaultPageUrl = "/";
    }

    public enum SiteRoles
    {
        // Users who can control everything
        Administrators,
        // Users who can upload/modify things
        PowerUsers,
        // General users
        Users
    }

    public static class Utilities
    {
        private static readonly Lazy<IWebConfiguration> configuration =
            new Lazy<IWebConfiguration>(() => ConfigurationManager.GetSection("churchWeb") as ChurchWebSection);
        public static IWebConfiguration Configuration { get { return configuration.Value; } }

        public static IFactory CreateFactory(string cultureName)
        {
            // Set culture infomation and set LongDatePattern if needed
            var culture = CultureInfo.CurrentUICulture.Name == cultureName ?
                CultureInfo.CurrentUICulture :
                CultureInfo.CreateSpecificCulture(cultureName);
            Resources.Framework.Culture = culture;

            // Read from disk if the assembly is not loaded yet
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals(Configuration.ChurchWebsiteLibrary)) ??
                Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, Configuration.ChurchWebsiteLibrary) + ".dll");
            var type = assembly.GetTypes().First(t => typeof(IFactory).IsAssignableFrom(t) && 0 == (TypeAttributes.Interface & t.Attributes));

            return type.GetMethod("Create").Invoke(null, new object[] { culture }) as IFactory;
        }

        public static string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            //else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty prop;
            //    prop = (RemoteEndpointMessageProperty)this.Request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}
            else
            {
                return string.Empty;
            }
        }

        //    private static readonly Lazy<ILogger> dataLogger =
        //        new Lazy<ILogger>(() => new Logger(Utilities.CreateDataProvider()));
        //    public static ILogger DataLogger { get { return dataLogger.Value; } }

        //    public static AbstractDataProvider CreateDataProvider()
        //    {
        //        // Read from disk if the assembly is not loaded yet
        //        //var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
        //        //    .FirstOrDefault(t => typeof(AbstractDataProvider).IsAssignableFrom(t) && 0 == (TypeAttributes.Abstract & t.Attributes)) ??
        //        //    Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, Configuration.ChurchWebsiteLibrary) + ".dll").GetTypes()
        //        //    .First(t => typeof(AbstractDataProvider).IsAssignableFrom(t));
        //        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals(Configuration.ChurchWebsiteLibrary))??
        //            Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, Configuration.ChurchWebsiteLibrary) + ".dll");
        //        var type = assembly.GetTypes().First(t => typeof(AbstractDataProvider).IsAssignableFrom(t) && 0 == (TypeAttributes.Abstract & t.Attributes));

        //        return Activator.CreateInstance(type) as AbstractDataProvider;
        //    }

        //    public static void SetupRoles()
        //    {
        //        foreach (var r in Enum.GetValues(typeof(SiteRoles)))
        //        {
        //            Roles.CreateRole(r.ToString());
        //        }
        //    }

        //    public static bool AdminExists()
        //    {
        //        return Roles.RoleExists(SiteRoles.Administrators.ToString());
        //    }

        //    public static bool IsGuest(string username)
        //    {
        //        return IsGuest(role => Roles.IsUserInRole(username, role));
        //    }

        //    public static bool IsGuest()
        //    {
        //        return IsGuest(role => Roles.IsUserInRole(role));
        //    }

        //    private static bool IsGuest(Func<string, bool> callback)
        //    {
        //        bool res = true;
        //        foreach (var r in Enum.GetValues(typeof(SiteRoles)))
        //        {
        //            if (callback(r.ToString()))
        //            {
        //                res = false;
        //                break;
        //            }
        //        }

        //        return res;
        //    }

        //    public static bool IsAdmin()
        //    {
        //        return Roles.IsUserInRole(SiteRoles.Administrators.ToString());
        //    }

        //    public static bool IsPowerUser()
        //    {
        //        return Roles.IsUserInRole(SiteRoles.PowerUsers.ToString());
        //    }

        public static MvcHtmlString ToHtml(string text)
        {
            string[] lineSeperators = new string[] { Environment.NewLine };
            string builder = string.Join("<br />", text.Split(lineSeperators, StringSplitOptions.None).Select(s => HttpUtility.HtmlEncode(s)));
            return MvcHtmlString.Create(builder);
        }

        //    public static string GetIPAddress(HttpRequestBase request)
        //    {
        //        string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //        if (!string.IsNullOrEmpty(ip))
        //        {
        //            string[] ipRange = ip.Split(',');
        //            ip = ipRange[ipRange.Length - 1];
        //        }
        //        else
        //        {
        //            ip = request.UserHostAddress;
        //        }

        //        return ip;
        //    }

        //    public static void SendEmail(string relayServer, string fromAddress, string toAddress, string toName, string bccAddress, string subject, string body)
        //    {
        //        SmtpClient smtpClient = new SmtpClient(relayServer);
        //        MailMessage message = new MailMessage
        //        {
        //            From = new MailAddress(fromAddress),
        //            Subject = subject,
        //            Priority = MailPriority.Normal,
        //            Body = body,
        //            IsBodyHtml = true,
        //        };
        //        message.To.Add(new MailAddress(toAddress, toName));
        //        // Bcc the email to webmaster
        //        message.Bcc.Add(new MailAddress(bccAddress));
        //        smtpClient.Send(message);
        //    }
    }

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    //public sealed class LocalizedDisplayNameAttribute : DisplayNameAttribute
    //{
    //    public LocalizedDisplayNameAttribute(Type resourceType, string resourceKey)
    //        : base(LookupResource(resourceType, resourceKey))
    //    {
    //    }
    //    private static string LookupResource(Type resourceManagerProvider, string resourceKey)
    //    {
    //        PropertyInfo propertyInfo = resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic).FirstOrDefault(p => p.PropertyType == typeof(ResourceManager));
    //        return null == propertyInfo ? resourceKey : (propertyInfo.GetValue(null, null) as ResourceManager).GetString(resourceKey);
    //    }
    //}
}
