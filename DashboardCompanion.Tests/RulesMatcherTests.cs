namespace DashboardCompanion.Tests
{
    using System.Linq;

    using Auth0.Core;

    using DashboardCompanion.Matching;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RulesMatcherTests
    {
        [TestMethod]
        public void FindsMatches()
        {
            var apps = new[]
                           {
                               new Client() { ClientId = "Id0", Name = "Name0" },
                               new Client() { ClientId = "Id1", Name = "Name1" },
                               new Client() { ClientId = "Id2", Name = "Name2" },
                               new Client() { ClientId = "Id3", Name = "RepeatedName" },
                               new Client() { ClientId = "Id4", Name = "RepeatedName" }
                           };

            var rules = new[]
                            {
                                new Rule() { Id = "r0", Script = "xxxx if (context.clientId === 'Id0') xxx"},
                                new Rule() { Id = "r1", Script = "xxx if (context.clientId === 'Id1') xxx"},
                                new Rule() { Id = "r2", Script = "xxx if (context.clientId !== 'Id0') xxx"},
                                new Rule() { Id = "r3", Script = "xxx if (context.clientName === 'Name0') xxx"},
                                new Rule() { Id = "r4", Script = "xxx NoAppReference xxx"},
                                new Rule() { Id = "r5", Script = "xxx if (context.clientId === 'Id9') xxx"},
                                new Rule() { Id = "r6", Script = "xxx if (context.clientName === 'RepeatedName') xxx"}
                            };

            var result = RulesMatcher.FindMatches(rules, apps);

            Assert.IsTrue(result[rules[0]].MatchedClients.Contains(apps[0]));
            Assert.IsTrue(result[rules[2]].MatchedClients.Contains(apps[0]));
            Assert.IsTrue(result[rules[3]].MatchedClients.Contains(apps[0]));
            Assert.IsTrue(result[rules[1]].MatchedClients.Contains(apps[1]));
            Assert.IsTrue(result[rules[5]].Reference.ClientId == "Id9" && !result[rules[5]].MatchedClients.Any(),
                "There should be a mention to Rule 5, even if the client reference doesn't match any client.");
            Assert.IsNull(result[rules[4]], "Rule 4 should have a null value because it has no reference.");
            Assert.IsTrue(result[rules[6]].MatchedClients.Contains(apps[3]) && result[rules[6]].MatchedClients.Contains(apps[4]), 
                "Rule 6 should contain reference to both applications that have the same name.");
            Assert.AreEqual(rules.Length, result.Count);
        }
    }
}
