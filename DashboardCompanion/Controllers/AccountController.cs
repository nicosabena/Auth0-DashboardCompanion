namespace DashboardCompanion.Controllers
{
    using System.IdentityModel.Services;
    using System.Web.Mvc;

    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            if (!MvcApplication.IsConfigured)
            {
                // wrong config, redirect to Try
                return this.RedirectToAction("Try");
            }

            return this.View();
        }

        public ActionResult Logout()
        {
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            return this.Redirect("/");
        }
    }
}