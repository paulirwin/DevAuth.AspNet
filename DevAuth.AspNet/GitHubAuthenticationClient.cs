using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DevAuth.AspNet
{
    public class GitHubAuthenticationClient : OAuth2Client
    {
        public const string AuthorizationEndpoint = "https://github.com/login/oauth/authorize";

        public const string TokenEndpoint = "https://github.com/login/oauth/access_token";

        public const string UsersEndpoint = "https://api.github.com/user";

        private readonly string _clientID;

        private readonly string _clientSecret;

        public GitHubAuthenticationClient(string clientID, string clientSecret)
            : base("GitHub")
        {
            _clientID = clientID;
            _clientSecret = clientSecret;
        }

        public string ClientID
        {
            get { return _clientID; }
        }

        public string ClientSecret
        {
            get { return _clientSecret; }
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            return GetServiceLoginUrlInternal(returnUrl);
        }

        internal Uri GetServiceLoginUrlInternal(Uri returnUrl)
        {
            UriBuilder uriBuilder = new UriBuilder(AuthorizationEndpoint);
            uriBuilder.AppendQueryArgument("client_id", _clientID);
            uriBuilder.AppendQueryArgument("redirect_uri", returnUrl.AbsoluteUri);
            //uriBuilder.AppendQueryArgument("scope", "user");

            return uriBuilder.Uri;
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            var client = new RestClient();

            var builder = new UriBuilder(UsersEndpoint);

            builder.AppendQueryArgument("access_token", accessToken);

            var request = new RestRequest(builder.Uri.AbsoluteUri, Method.GET);

            var response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return new Dictionary<string, string>();

            var json = JObject.Parse(response.Content);

            var strs = new Dictionary<string, string>();

            strs["id"] = json["id"].ToString();
            strs["username"] = json["login"].ToString();

            try
            {
                strs["name"] = json["name"].ToString();
            }
            catch
            {
            }

            return strs;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            try
            {
                var client = new RestClient();

                var request = new RestRequest(TokenEndpoint, Method.POST);

                request.AddParameter("client_id", _clientID);
                request.AddParameter("redirect_uri", returnUrl.AbsoluteUri);
                request.AddParameter("client_secret", _clientSecret);
                request.AddParameter("code", authorizationCode);

                request.AddHeader("accept", "application/json");

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var oAuth2AccessTokenDatum = JsonConvert.DeserializeObject<OAuth2AccessTokenData>(response.Content);
                    if (oAuth2AccessTokenDatum != null)
                    {
                        return oAuth2AccessTokenDatum.AccessToken;
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
