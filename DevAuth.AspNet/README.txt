Usage:

Build.
Add DevAuth.AspNet.dll as a reference.
Ensure you have the following NuGet packages installed: DotNetOpenAuth.AspNet, Newtonsoft.Json, and RestSharp.
Add "using DevAuth.AspNet;" to wherever you configure your OAuth (Global.asax.cs or AuthConfig.cs).
Add both or either of the following to set up the authentication clients, replacing the client ID/secret values:
	OAuthWebSecurity.RegisterClient(new BitBucketAuthenticationClient("myclientID", "myclientSecret"), "BitBucket", null); 
	OAuthWebSecurity.RegisterClient(new GitHubAuthenticationClient("myclientID", "myclientSecret"), "GitHub", null);