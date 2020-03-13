using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

namespace CIM.Web.Filters
{
    public class ForceUserLogoutFilter : ActionFilterAttribute
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var UserManager = filterContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = UserManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;

                // check user if not active will force user logout and clear session.
                if (user != null && !user.Active)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    filterContext.HttpContext.Session.Clear();
                    filterContext.HttpContext.Session.Abandon();
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}