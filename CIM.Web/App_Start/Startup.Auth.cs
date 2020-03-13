using CIM.Common;
using CIM.Data;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;

namespace CIM.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(CIMDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromDays(90.0),
                SlidingExpiration = true,
                LoginPath = new PathString("/Account/Login"),
                CookieSecure = CookieSecureOption.SameAsRequest,
                CookieManager = new SystemWebChunkingCookieManager()
                //Provider = new CookieAuthenticationProvider
                //{
                //    // Enables the application to validate the security stamp when the user logs in.
                //    // This is a security feature which is used when you change a password or add an external login to your account.
                //    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser, int>(
                //        validateInterval: TimeSpan.FromMinutes(10),
                //        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                //        getUserIdCallback: id => id.GetUserId<int>())
                //}
            }); ;
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigHelper.GetKey("GG:ClientId"),
                ClientSecret = ConfigHelper.GetKey("GG:ClientSecret"),
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    //OnAuthenticated = ctx =>
                    //{
                    //    string domain = ctx.User.Value<string>("hd");
                    //    if (domain != "fpt.edu.vn")
                    //        throw new Exception("You must sign in with a fpt.edu.vn email address");

                    //    return Task.FromResult(0);
                    //},

                    //OnApplyRedirect = ctx =>
                    //{
                    //    ctx.Response.Redirect(ctx.RedirectUri + "&hd=" + System.Net.WebUtility.UrlEncode("fpt.edu.vn"));
                    //}
                }
            });
        }
    }
}