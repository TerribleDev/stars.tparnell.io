using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;

namespace stars.tparnell.io
{
    public static class MVC
    {
        public static IEnumerable<AuthenticationDescription> GetExternalProviders(this HttpContext context)
        {
            return context.Authentication.GetAuthenticationSchemes()
                .Where(a => !string.IsNullOrWhiteSpace(a.DisplayName));
        }

        public static bool IsProviderSupported(this HttpContext context, string provider)
        {
            return context.GetExternalProviders()
                .Any(a => a.DisplayName.Equals(provider));
        }

        public static string GithubTokenOrDefault(this IEnumerable<Claim> claims)
        {
            return claims?
               .FirstOrDefault(a => a.Issuer.Equals("GitHub", StringComparison.OrdinalIgnoreCase) && a.Type.Equals("access_token", StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }
}