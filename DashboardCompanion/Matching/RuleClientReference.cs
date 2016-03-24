namespace DashboardCompanion.Matching
{
    using System;

    using Auth0.Core;

    /// <summary>
    /// Represents a reference to a client application in a rule.
    /// </summary>
    public class RuleClientReference
    {
        /// <summary>
        /// The client name used.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// The client id used.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Returns true if the reference matches a given client.
        /// </summary>
        public bool Matches(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return this.ClientName != null && this.ClientName == client.Name
                   || this.ClientId != null && this.ClientId == client.ClientId;
        }

        /// <summary>
        /// Indicate that the reference was from an inequality operator ("!==", i.e. "all but this").
        /// This actually doesn't mean much... It might be something like this
        /// <code>
        ///     if(context.clientName !== 'My other app'){
        ///        return callback(null, user, context);
        ///     }
        /// </code>
        /// or this
        /// <code>
        ///     if(context.clientName !== 'My other app'){
        ///        ... execute rule logic
        ///     }
        ///     callback(null, user, context);
        /// </code>
        /// </summary>
        public bool IsNegated { get; set; }
    }
}
