namespace DashboardCompanion.Helpers
{
    using System;
    using System.Configuration;
    using System.Web.Mvc;

    using Auth0.Core;

    public static class UriBuilders
    {
        private static readonly Uri ManagementUri =
            new UriBuilder("https", ConfigurationManager.AppSettings["auth0:DashboardDomain"], 443, "").Uri;

        public static string ClientSettings(this UrlHelper urlHelper, Client client)
        {
            return new Uri(ManagementUri, $"#/applications/{client.ClientId}/settings").ToString();
        }

        public static string RuleSettings(this UrlHelper urlHelper, Rule rule)
        {
            return new Uri(ManagementUri, $"#/rules/{rule.Id}").ToString();
        }
    }
}