using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Shared.Common.Auth.Util;

public static class ClaimsPrincipalParser
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private class ClientPrincipal
    {
        [JsonPropertyName("auth_typ")] public string? IdentityProvider { get; set; }
        [JsonPropertyName("name_typ")] public string? NameClaimType { get; set; }
        [JsonPropertyName("role_typ")] public string? RoleClaimType { get; set; }
        [JsonPropertyName("claims")] public IEnumerable<ClientPrincipalClaim> Claims { get; set; } = [];
    }

    private class ClientPrincipalClaim
    {
        [JsonPropertyName("typ")] public string? Type { get; set; }
        [JsonPropertyName("val")] public string? Value { get; set; }
    }

    public static ClaimsPrincipal ParseRoles(HttpRequest req)
    {
        var principal = new ClientPrincipal();

        if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            var data = header[0];
            if (string.IsNullOrWhiteSpace(data))
            {
                return new ClaimsPrincipal();
            }

            var decoded = Convert.FromBase64String(data);
            var json = Encoding.UTF8.GetString(decoded);
            principal = JsonSerializer.Deserialize<ClientPrincipal>(json, JsonSerializerOptions);
        }

        if (principal == null)
        {
            return new ClaimsPrincipal();
        }

        var identity = new ClaimsIdentity(principal.IdentityProvider, principal.NameClaimType, principal.RoleClaimType);

        identity.AddClaims(
            principal.Claims
                .Where(c => c is { Type: "roles", Value: not null })
                .Select(c => new Claim(c.Type!, c.Value!))
        );

        return new ClaimsPrincipal(identity);
    }
}