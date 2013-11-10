DevAuth.AspNet
==============

DotNetOpenAuth ASP.NET OAuth Clients for GitHub and BitBucket.

## Usage

(until I get around to making a NuGet package)

1. `git clone https://github.com/paulirwin/DevAuth.AspNet.git`
2. Build.
3. Reference DevAuth.AspNet.dll
4. In Global.asax.cs or AuthConfig.cs, add `using DevAuth.AspNet;`
5. In Global.asax.cs or AuthConfig.cs, add both or either of the following:
    `OAuthWebSecurity.RegisterClient(new BitBucketAuthenticationClient("myclientID", "myclientSecret"), "BitBucket", null);`
    `OAuthWebSecurity.RegisterClient(new GitHubAuthenticationClient("myclientID", "myclientSecret"), "GitHub", null);`
