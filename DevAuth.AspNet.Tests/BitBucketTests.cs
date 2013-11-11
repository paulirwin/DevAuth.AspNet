using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.OAuth.Messages;

namespace DevAuth.AspNet.Tests
{
    [TestClass]
    public class BitBucketTests
    {
        [TestMethod]
        public void StaticCtorTest()
        {
            var spd = BitBucketAuthenticationClient.BitBucketServiceDescription;

            Assert.IsNotNull(spd);

            var rte = spd.RequestTokenEndpoint;

            Assert.IsNotNull(rte);
            Assert.AreEqual("https://bitbucket.org/api/1.0/oauth/request_token", rte.Location.ToString());
            Assert.AreEqual(HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest, rte.AllowedMethods);

            var uae = spd.UserAuthorizationEndpoint;

            Assert.IsNotNull(uae);
            Assert.AreEqual("https://bitbucket.org/api/1.0/oauth/authenticate", uae.Location.ToString());
            Assert.AreEqual(HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest, uae.AllowedMethods);

            var ate = spd.AccessTokenEndpoint;

            Assert.IsNotNull(ate);
            Assert.AreEqual("https://bitbucket.org/api/1.0/oauth/access_token", ate.Location.ToString());
            Assert.AreEqual(HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest, ate.AllowedMethods);

            var tpe = spd.TamperProtectionElements;

            Assert.IsNotNull(tpe);
            Assert.AreEqual(1, tpe.Length);
            Assert.IsInstanceOfType(tpe[0], typeof(HmacSha1SigningBindingElement));
        }

        [TestMethod]
        public void CtorTest1()
        {
            const string key = "MyConsumerKey";
            const string secret = "MyConsumerSecret";

            var bb = new BitBucketAuthenticationClient(key, secret);

            Assert.AreEqual(key, bb.ConsumerKey);
            Assert.AreEqual(secret, bb.ConsumerSecret);
            Assert.AreEqual("BitBucket", bb.ProviderName);
        }

        [TestMethod]
        public void CtorTest2()
        {
            const string key = "MyConsumerKey";
            const string secret = "MyConsumerSecret";

            var bb = new BitBucketAuthenticationClient(key, secret, new AuthenticationOnlyCookieOAuthTokenManager());

            Assert.AreEqual(key, bb.ConsumerKey);
            Assert.AreEqual(secret, bb.ConsumerSecret);
            Assert.AreEqual("BitBucket", bb.ProviderName);            
        }
    }
}
