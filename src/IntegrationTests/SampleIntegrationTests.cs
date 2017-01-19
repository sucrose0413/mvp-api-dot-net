using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace IntegrationTests
{
    [TestClass]
    public class SampleIntegrationTests
    {
        [TestMethod]
        public void GetContributionTypesTest()
        {
            var subscriptionKey = IntegrationTests.Properties.Settings.Default.SubscriptionKey;
            var authorizationBearer = IntegrationTests.Properties.Settings.Default.AuthorizationBearer;

            Dictionary<string, List<string>> customHeaders = new Dictionary<string, List<string>>();
            customHeaders.Add("Authorization", new List<string>() { authorizationBearer });
            MVP.MVPProduction prodClient = new MVP.MVPProduction();
            var result = prodClient.GetContributionTypesWithHttpMessagesAsync(
                null, subscriptionKey, customHeaders).Result;
            Assert.AreNotEqual(0, result.Body.Count, "Contributions Types could not be fetched");
        }
    }
}
