using Microsoft.Identity.Client;

namespace Shared.Common.Auth.Services;

public interface ITokenAcquirer
{
    /// <summary>
    /// Acquires a token for the specified scope.
    /// </summary>
    /// <param name="scope">The scope for which the token is requested.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
    Task<AuthenticationResult> AcquireTokenAsync(string scope);
}

/// <summary>
/// Implementation of <see cref="ITokenAcquirer"/> that uses a confidential client application to acquire tokens.
/// </summary>
public class TokenAcquirer : ITokenAcquirer
{
    private readonly IConfidentialClientApplication _confidentialClientApplication;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenAcquirer"/> class.
    /// </summary>
    /// <param name="confidentialClientApplication">The confidential client application used to acquire tokens.</param>
    public TokenAcquirer(IConfidentialClientApplication confidentialClientApplication)
    {
        _confidentialClientApplication = confidentialClientApplication;
    }

    /// <inheritdoc/>
    public async Task<AuthenticationResult> AcquireTokenAsync(string scope)
    {
        return await _confidentialClientApplication.AcquireTokenForClient(new[] { scope }).ExecuteAsync();
    }
}