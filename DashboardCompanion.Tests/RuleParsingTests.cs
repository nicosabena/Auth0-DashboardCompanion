namespace DashboardCompanion.Tests
{
    using Auth0.Core;

    using DashboardCompanion.Matching;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RulesScriptParserTests
    {
        private void AssertNameReference(string ruleScript, string expectedAppName)
        {
            var reference = RulesScriptParser.FindAppReference(ruleScript);
            Assert.IsNotNull(reference);
            Assert.AreEqual(expectedAppName, reference.ClientName);
        }

        private void AssertIdReference(string ruleScript, string expectedAppId)
        {
            var rule = new Rule() { Script = ruleScript };
            var reference = RulesScriptParser.FindAppReference(ruleScript);
            Assert.IsNotNull(reference);
            Assert.AreEqual(expectedAppId, reference.ClientId);
        }


        [TestMethod]
        public void GetsClientNameFromIdentity()
        {
            this.AssertNameReference(
                "function (user, context, callback) {\nif (context.clientName === 'TheAppToCheckAccessTo')\n{\nvar d = new Date().getDay();\n",
                "TheAppToCheckAccessTo");
        }

        [TestMethod]
        public void GetsClientNameFromNonIdentity()
        {
            this.AssertNameReference(
                "function (user, context, callback) {\nif (context.clientName !== 'TheAppToCheckAccessTo')\n{\nvar d = new Date().getDay();\n",
                "TheAppToCheckAccessTo");
        }

        [TestMethod]
        public void GetsClientNameFromEquality()
        {
            this.AssertNameReference(
                "function (user, context, callback) {\nif (context.clientName == 'TheAppToCheckAccessTo')\n{\nvar d = new Date().getDay();\n",
                "TheAppToCheckAccessTo");
        }

        [TestMethod]
        public void GetsClientNameFromNonEquality()
        {
            this.AssertNameReference(
                "function (user, context, callback) {\nif (context.clientName != 'TheAppToCheckAccessTo')\n{\nvar d = new Date().getDay();\n",
                "TheAppToCheckAccessTo");
        }

        [TestMethod]
        public void WhitespaceDoesntMatter()
        {
            this.AssertNameReference(
                "function (user, context, callback) {\nif (context.clientName!=   \n  'TheAppToCheckAccessTo')\n{\nvar d = new Date().getDay();\n",
                "TheAppToCheckAccessTo");
        }

        [TestMethod]
        public void TakesDoubleQuotes()
        {
            this.AssertNameReference(
                "function (user, context, callback) {\nif (context.clientName === \"TheAppToCheckAccessTo\")\n{\nvar d = new Date().getDay();\n",
                "TheAppToCheckAccessTo");
        }


        [TestMethod]
        public void GetsClientId()
        {
            this.AssertIdReference(
                "function (user, context, callback) {\nif (context.clientId === 'TheClientIdToCheckAccessTo')  \n{\nvar d = new Date().getDay();\n",
                "TheClientIdToCheckAccessTo");
        }

        [TestMethod]
        public void TakesNameWithEscapedQuote()
        {
            this.AssertIdReference(
                @"function (user, context, callback) {\nif (context.clientId === 'This name\'s got a quote' )\n{\nvar d = new Date().getDay();\n",
                "This name's got a quote");
        }

        [TestMethod]
        public void TakesNameWithEscapedDoubleQuote()
        {
            this.AssertIdReference(
                @"function (user, context, callback) {\nif (context.clientId === ""This app is \""great\"""" )\n{\nvar d = new Date().getDay();\n",
                "This app is \"great\"");
        }
    }
}