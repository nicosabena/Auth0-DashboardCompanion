namespace DashboardCompanion.Matching
{
    using System.Text.RegularExpressions;

    public static class RulesScriptParser
    {
        /// <summary>
        /// Looks into a script to find a specific reference to a client app.
        /// It looks for matches like
        /// <code>context.clientName === 'ClientName'</code>, allowing also
        /// clientIds as well as clientNames. The parsing understands single and double
        /// quotes and inequality operators.
        /// <para>It only looks for a single reference. Returns null if no reference is found.</para>
        /// </summary>
        /// <param name="ruleScript">The script to search.</param>
        /// <returns>An instance of <see cref="RuleClientReference" /> or null if no reference is found.</returns>
        public static RuleClientReference FindAppReference(string ruleScript)
        {
            var regExp = new Regex(@"context.(clientName|clientId)\s*([=!])==?\s*(?:('|"")((?:[^'""\\]*(?:\\.)?)*)\3)");

            var match = regExp.Match(ruleScript);
            if (!match.Success)
            {
                return null;
            }
            var reference = match.Groups[4].Value.Replace("\\\"", "\"").Replace("\\'", "'");
            var negatedReference = match.Groups[2].Value == "!";
            return 
                match.Groups[1].Value == "clientName" 
                    ? new RuleClientReference() { ClientName = reference }
                    : new RuleClientReference() { ClientId = reference };
        }
    }
}