namespace DashboardCompanion.Models
{
    using Auth0.Core;

    public class RelatedRuleViewModel
    {
        public RelatedRuleViewModel(Rule rule)
        {
            this.Rule = rule;
        }

        public Rule Rule { get; set; }

        public bool SpecificForApp { get; set; }
    }
}