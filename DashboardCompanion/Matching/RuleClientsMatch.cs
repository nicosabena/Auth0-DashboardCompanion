namespace DashboardCompanion.Matching
{
    using System;
    using System.Collections.Generic;

    using Auth0.Core;

    /// <summary>
    /// Represent a relationship between a rule, a reference to a client,
    /// and all clients that match the reference.
    /// </summary>
    public class RuleClientsMatch
    {
        public RuleClientsMatch(Rule rule, RuleClientReference reference, IEnumerable<Client> matchedClients)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }
            this.MatchedClients = new HashSet<Client>(matchedClients);
            this.Rule = rule;
            this.Reference = reference;
        }

        public Rule Rule { get; private set; }

        /// <summary>
        /// The client reference as found on the rule script.
        /// </summary>
        public RuleClientReference Reference { get; private set; }

        /// <summary>
        /// Zero or more clients that match the reference.
        /// </summary>
        public HashSet<Client> MatchedClients { get; private set; } 
    }
}