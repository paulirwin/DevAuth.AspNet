using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DevAuth.AspNet
{
    public class BitBucketAuthenticationClient : OAuthClient
    {
        public static readonly ServiceProviderDescription BitBucketServiceDescription;

        static BitBucketAuthenticationClient()
        {
            ServiceProviderDescription spd = new ServiceProviderDescription();

            spd.RequestTokenEndpoint = new MessageReceivingEndpoint("https://bitbucket.org/api/1.0/oauth/request_token", HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);
            spd.UserAuthorizationEndpoint = new MessageReceivingEndpoint("https://bitbucket.org/api/1.0/oauth/authenticate", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);
            spd.AccessTokenEndpoint = new MessageReceivingEndpoint("https://bitbucket.org/api/1.0/oauth/access_token", HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);
            spd.TamperProtectionElements = new[] { new HmacSha1SigningBindingElement() };

            BitBucketServiceDescription = spd;
        }

        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public BitBucketAuthenticationClient(string consumerKey, string consumerSecret)
            : this(consumerKey, consumerSecret, new AuthenticationOnlyCookieOAuthTokenManager())
        {
        }

        public BitBucketAuthenticationClient(string consumerKey, string consumerSecret, IOAuthTokenManager tokenManager)
            : base("BitBucket", BitBucketServiceDescription, new SimpleConsumerTokenManager(consumerKey, consumerSecret, tokenManager))
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        public string ConsumerKey
        {
            get { return _consumerKey; }
        }

        public string ConsumerSecret
        {
            get { return _consumerSecret; }
        }

        protected override AuthenticationResult VerifyAuthenticationCore(AuthorizedTokenResponse response)
        {
            return VerifyAuthenticationInternal(response);
        }

        internal AuthenticationResult VerifyAuthenticationInternal(AuthorizedTokenResponse response)
        {
            var msg = response as ITokenSecretContainingMessage;

            if (msg == null)
                return AuthenticationResult.Failed;

            string oauthToken = msg.Token;
            string oauthTokenSecret = msg.TokenSecret;

            var authenticator = OAuth1Authenticator.ForProtectedResource(_consumerKey, _consumerSecret, oauthToken, oauthTokenSecret);

            Uri uri = new Uri("https://bitbucket.org/api/1.0/user");

            var request = new RestRequest(Method.GET);
            request.Resource = uri.AbsoluteUri;

            var client = new RestClient();
            client.Authenticator = authenticator;

            var apiresponse = client.Execute(request);

            if (apiresponse.StatusCode != HttpStatusCode.OK)
                return AuthenticationResult.Failed;

            string userName = null;

            try
            {
                var json = JObject.Parse(apiresponse.Content);

                userName = json["user"]["username"].ToString();
            }
            catch
            {
                return AuthenticationResult.Failed;
            }

            Dictionary<string, string> strs = new Dictionary<string, string>()
            {
                { "accesstoken", response.AccessToken }
            };

            return new AuthenticationResult(true, base.ProviderName, userName, userName, strs);
        }
    }
}
