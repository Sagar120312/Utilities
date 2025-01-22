using SingleSignOnUtility.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace SingleSignOnUtility.Operation.Interface
{
    public interface IGoogleOpenIdConnectService
    {
        string GenerateAuthenticationUrl(out string state, out string nonce);
        Task<UserProfile> ValidateAndDecodeIdToken(string idToken, string nonce);
    }
}
