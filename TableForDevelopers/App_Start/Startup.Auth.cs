using Microsoft.Owin;
using Owin;
using TableForDevelopers.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.DataProtection;

[assembly: OwinStartup(typeof(TableForDevelopers.App_Start.Startup))]

namespace TableForDevelopers.App_Start
{
    public class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            // настраиваем контекст и менеджер
            DataProtectionProvider = app.GetDataProtectionProvider();
            app.CreatePerOwinContext(ApplicationContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),

            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "232919813622-920u27en14gbkf5ktgmlihr61ov4i4qs.apps.googleusercontent.com",
                ClientSecret = "xE4H6mQD4msfm4YUaCEjg3qn",
                //Provider = new GoogleOAuth2AuthenticationProvider(),
                //TokenEndpoint = "https://oauth2.googleapis.com/token",
            });
        }
    }
}