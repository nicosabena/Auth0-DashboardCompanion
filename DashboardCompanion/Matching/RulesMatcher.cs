namespace DashboardCompanion.Matching
{
    using System.Collections.Generic;
    using System.Linq;

    using Auth0.Core;

    public static class RulesMatcher
    {
        /// <summary>
        /// Finds matches between the rules provided and the list of client apps.
        /// </summary>
        /// <param name="rules">The list of rules.</param>
        /// <param name="clients">The list of clients.</param>
        /// <returns>A dictionary with all the rules as keys, and information about
        /// a specific reference and client matches (or null) as values.</returns>
        public static IDictionary<Rule, RuleClientsMatch> FindMatches(IEnumerable<Rule> rules, IEnumerable<Client> clients)
        {
            var analizedRules = rules.Select(
                r => new
                         {
                             Rule = r,
                             Reference = RulesScriptParser.FindAppReference(r.Script)
                         })
                .Select(rf => new
                                  {
                                      Rule = rf.Rule,
                                      Match = rf.Reference != null ? new RuleClientsMatch(rf.Rule, rf.Reference, clients.Where(rf.Reference.Matches)) : null
                                  })
                .ToDictionary(m => m.Rule, m => m.Match);

            return analizedRules;
        }
    }
}