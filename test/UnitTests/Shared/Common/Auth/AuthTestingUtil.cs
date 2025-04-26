using System.Text;

namespace UnitTests.Shared.Common.Auth;

public static class AuthTestingUtil
{
    public static string GetValidPrincipalClaim(string roleName)
    {
        var str = $$"""
                    {
                        "auth_typ": "aad",
                        "claims": [
                            {
                                "typ": "aud",
                                "val": "some_value"
                            },
                            {
                                "typ": "name",
                                "val": "First Last"
                            },
                            {
                                "typ": "roles",
                                "val": "{{roleName}}"
                            },
                            {
                                "typ": "roles",
                                "val": "admin"
                            }
                        ],
                        "name_typ": "some_typ",
                        "role_typ": "some_other_typ"
                    }
                    """;
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }

    public static string GetPrincipalClaimWithNullRole()
    {
        const string str = """
                           {
                               "auth_typ": "aad",
                               "claims": [
                                   {
                                       "typ": "aud",
                                       "val": "some_value"
                                   },
                                   {
                                       "typ": "name",
                                       "val": "First Last"
                                   },
                                   {
                                       "typ": "roles",
                                       "val": null
                                   }
                               ],
                               "name_typ": "some_typ",
                               "role_typ": "some_other_typ"
                           }
                           """;
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }
}