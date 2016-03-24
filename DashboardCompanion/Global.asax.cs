namespace DashboardCompanion
{
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static bool IsConfigured { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        static MvcApplication()
        {
            var requiredValues = new[] { "auth0:Domain", "auth0:ClientId", "auth0:ClientSecret", "ApiAccessToken" };
            IsConfigured = requiredValues.All(v => !string.IsNullOrEmpty(ConfigurationManager.AppSettings[v]));
        }
    }
}
