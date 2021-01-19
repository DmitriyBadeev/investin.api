using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace InvestIn.Identity
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources(string apiFinance)
        {
            return new List<ApiResource>
            {
                new ApiResource(apiFinance)
            };
        }

        public static IEnumerable<Client> GetSpaClient(string financeClientId, List<string> financeRedirects,
            string apiFinance)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = financeClientId,
                    ClientName = "InvestIn",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AccessTokenLifetime = 3600 * 5,
                    RequireConsent = false,
                    AllowedCorsOrigins = 
                    { 
                        "http://localhost:3000", 
                        "http://localhost:3001", 
                        "https://investin.badeev.info"
                    },
                    PostLogoutRedirectUris = new List<string> 
                    {
                        "https://investin.badeev.info/signout",
                        "http://localhost:3000/signout"
                    },
                    RedirectUris = financeRedirects,
                    AllowedScopes = 
                    { 
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile,
                        apiFinance
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
    }
}
