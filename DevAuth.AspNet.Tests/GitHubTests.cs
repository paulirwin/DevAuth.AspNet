using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp.Contrib;

namespace DevAuth.AspNet.Tests
{
    [TestClass]
    public class GitHubTests
    {
        [TestMethod]
        public void CtorTest()
        {
            const string clientID = "MyClientID";
            const string clientSecret = "MyClientSecret";

            var gh = new GitHubAuthenticationClient(clientID, clientSecret);

            Assert.AreEqual(clientID, gh.ClientID);
            Assert.AreEqual(clientSecret, gh.ClientSecret);
            Assert.AreEqual("GitHub", gh.ProviderName);
        }

        [TestMethod]
        public void GetServiceLoginUrl_ShouldBuildCorrectUrl()
        {
            const string clientID = "MyClientID";
            const string clientSecret = "MyClientSecret";
            const string redirectUri = "http://localhost/Account/ExternalLoginCallback";

            var gh = new GitHubAuthenticationClient(clientID, clientSecret);

            var result = gh.GetServiceLoginUrlInternal(new Uri(redirectUri));

            Assert.IsNotNull(result);

            string expected = "https://github.com/login/oauth/authorize?client_id=myclientid&redirect_uri=http:%2f%2flocalhost%2faccount%2fexternallogincallback";

            Assert.AreEqual(expected, result.ToString().ToLower());
        }
    }
}
