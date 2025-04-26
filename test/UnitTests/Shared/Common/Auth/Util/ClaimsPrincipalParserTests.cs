using Microsoft.AspNetCore.Http;
using Shared.Common.Auth.Util;

namespace UnitTests.Shared.Common.Auth.Util;

public class ClaimsPrincipalParserTests
{
    [Fact]
    public void Parse_ReturnsClaimsPrincipal_WhenHeaderValid()
    {
        var roleName = Guid.NewGuid().ToString();
        var context = new DefaultHttpContext();
        context.Request.Headers["x-ms-client-principal"] = AuthTestingUtil.GetValidPrincipalClaim(roleName);

        var result = ClaimsPrincipalParser.ParseRoles(context.Request);

        Assert.Contains(result.Claims, c => c.Type == "roles" && c.Value == roleName);
    }
    
    [Fact]
    public void Parse_ReturnsEmptyClaimsPrincipal_WhenHeaderNotPresent()
    {
        var context = new DefaultHttpContext();

        var result = ClaimsPrincipalParser.ParseRoles(context.Request);

        Assert.Empty(result.Claims);
    }
    
    [Fact]
    public void Parse_ReturnsEmptyClaimsPrincipal_WhenHeaderHasNullRoles()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["x-ms-client-principal"] = AuthTestingUtil.GetPrincipalClaimWithNullRole();

        var result = ClaimsPrincipalParser.ParseRoles(context.Request);

        Assert.Empty(result.Claims);
    }
}