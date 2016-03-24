namespace DashboardCompanion.Models
{
    using System.Collections.Generic;

    using Auth0.Core;

    using DashboardCompanion.Matching;

    public class AppsViewModel
    {
        public IEnumerable<ClientToRulesViewModel> Clients { get; set; }

        public IEnumerable<KeyValuePair<Rule, RuleClientsMatch>> Rules { get; set; }
    }

    public class ClientToRulesViewModel
    {
        public Client Client { get; set; }

        public IEnumerable<RelatedRuleViewModel> Rules { get; set; }
    }
}