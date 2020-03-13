using CIM.Web.Filters;
using System.Web.Mvc;

namespace CIM.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ForceUserLogoutFilter());
        }
    }
}