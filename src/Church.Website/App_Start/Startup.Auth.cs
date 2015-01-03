namespace Church.Website
{
    using Church.Website.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.MicrosoftAccount;
    using Owin;
    using System;

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            var dbContext = ApplicationDbContext.Create();
            app.CreatePerOwinContext(() => dbContext);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //var microsoftOptions = new MicrosoftAccountAuthenticationOptions
            //{
            //    Caption = "",
            //    ClientId = "",
            //    ClientSecret = "",
            //};
            //microsoftOptions.Scope.Add("wl.basic");
            //microsoftOptions.Scope.Add("wl.emails");
            //app.UseMicrosoftAccountAuthentication(microsoftOptions);

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            const string RoleAdministrators = "Administrators";
            const string RoleUsers = "Users";
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));
            if (!roleManager.RoleExists(RoleAdministrators))
            {
                this.CreateRole(roleManager, RoleAdministrators);
                this.CreateRole(roleManager, RoleUsers);

                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(dbContext));
            }
        }

        private bool CreateRole(RoleManager<IdentityRole> roleManager, string name)
        {
            var idResult = roleManager.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }

        private ApplicationUser CreateUser(ApplicationUserManager userManager, string userName, string name, string email)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Name = name,
                Email = email,
            };
            var idResult = userManager.Create(user);
            return idResult.Succeeded ? user : null;
        }
    }
}
