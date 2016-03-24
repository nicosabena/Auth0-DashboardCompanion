namespace DashboardCompanion.Controllers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Auth0.ManagementApi;

    using DashboardCompanion.Matching;
    using DashboardCompanion.Models;

    [HandleError(View = "Error")]
    public class AppsController : Controller
    {
        public ActionResult Try()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<ActionResult> Try(TryPostData model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("Try");
            }

            return await this.Show(model.Domain, model.ApiToken);
        }

        public async Task<ActionResult> Index()
        {
            if (!MvcApplication.IsConfigured)
            {
                // wrong config, redirect to Try
                return this.RedirectToAction("Try");
            }

            if (!this.User.Identity.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
            }

            var token = ConfigurationManager.AppSettings["ApiAccessToken"];
            var domain = ConfigurationManager.AppSettings["auth0:Domain"];

            return await this.Show(domain, token);
        }

        private async Task<ActionResult> Show(string domain, string apiToken)
        {
            var apiUri = new UriBuilder("https", domain, 443, "api/v2").Uri;

            var apiClient = new ManagementApiClient(apiToken, apiUri);

            var appsTask = apiClient.Clients.GetAllAsync(fields: "name,client_id,global");
            var rulesTask = apiClient.Rules.GetAllAsync(fields: "name,script,id,enabled,order");

            // if we want rules for all stages:
            //var rulesStages = new[] { "login_success", "login_failure", "pre_authorize", "user_registration", "user_blocked" };
            //var rulesTask =
            //    Task.WhenAll(
            //        rulesStages.Select(
            //            stage => apiClient.Rules.GetAllAsync(fields: "name,script,id,enabled,order", stage: stage)))
            //        .ContinueWith(t => t.Result.SelectMany(r => r));
            await Task.WhenAll(appsTask, rulesTask);

            var apps = appsTask.Result
                // "All Applications" is a special client app, not meant to be managed by the user.
                .Where(a => a.Name != "All Applications");
            var rules = rulesTask.Result;

            var matches = RulesMatcher.FindMatches(rules, apps);

            var model = new AppsViewModel()
                            {
                                Clients = apps.Select(
                                    c => new ClientToRulesViewModel()
                                             {
                                                 Client = c,
                                                 Rules =
                                                     matches.Where(
                                                         p => p.Value == null || p.Value.MatchedClients.Contains(c))
                                                     .Select(
                                                         p => new RelatedRuleViewModel(p.Key)
                                                                  {
                                                                      SpecificForApp = p.Value != null
                                                                  })
                                             }),
                                Rules = matches
                            };

            this.ViewBag.Domain = domain;

            return this.View("Index", model);
        }
    }
}